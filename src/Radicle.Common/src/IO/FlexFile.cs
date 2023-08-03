namespace Radicle.Common.IO;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Radicle.Common.Base;
using Radicle.Common.Check;
using Radicle.Common.Compression;
using Radicle.Common.MetaData;

/// <summary>
/// Wrapper for <see cref="FileStream"/> with support
/// for detecting compression (".gz", ".z") or use (stdin "-")
/// from the file names. For memory streams compression see
/// <see cref="Compression.Gzip"/>.
/// </summary>
/// <remarks>Why to do this: sometimes having less wrappers in the code is better
///     and this is exactly that, just call <see cref="Open(string, FileMode)"/>
///     and read or write directly via <see cref="ToStream"/>
///     of pass the instance to writer accepting streams and let
///     the instance implicitly convert itself to <see cref="Stream"/>.</remarks>
[Experimental("Experimental use only")]
public sealed class FlexFile : AsyncDisposable
{
    private const string Gzip = ".gz";

    private const string AltGzip = ".z";

    private const string StdIn = "-";

    private static readonly string[] Extensions = new[]
    {
        Gzip,
        AltGzip,
    };

    // Disposed in base
#pragma warning disable CA2213 // Disposable fields should be disposed
    private readonly Stream originalStream;

    private readonly Stream stream;
#pragma warning restore CA2213 // Disposable fields should be disposed

    private FlexFile(FileStream fileStream)
    {
        this.originalStream = Ensure.Param(fileStream).Value;
        this.Add(this.originalStream, 1);

        if (fileStream.Name.EndsWith(Gzip, StringComparison.OrdinalIgnoreCase)
                || fileStream.Name.EndsWith(AltGzip, StringComparison.OrdinalIgnoreCase))
        {
            GZipStream compressedStream;

            if (fileStream.CanWrite)
            {
                compressedStream = new GZipStream(
                        this.originalStream,
                        compressionLevel: CompressionLevel.Optimal);
            }
            else if (fileStream.CanRead)
            {
                compressedStream = new GZipStream(
                        this.originalStream,
                        CompressionMode.Decompress);
            }
            else
            {
                throw new NotSupportedException(
                        "BUG: File stream is neither readable nor writable.");
            }

            // compressedStream disposes fileStream for us
            this.stream = compressedStream;
            this.CompressionFormat = CompressionFormat.Gzip;

            this.Add(compressedStream, 0);
        }
        else
        {
            this.stream = this.originalStream;
        }
    }

    private FlexFile(
            FileMode mode,
            FileAccess access)
    {
        bool read = access switch
        {
            FileAccess.Read => true,
            FileAccess.ReadWrite => throw new NotSupportedException(
                    "STDID/OUT does not read and write at the same time"),
            FileAccess.Write => false,
            _ => throw new NotSupportedException(
                    $"BUG: {access} access is not supported"),
        };

        switch (mode)
        {
            case FileMode.Append:
            case FileMode.Create:
            case FileMode.CreateNew:
                if (read)
                {
                    throw new NotSupportedException(
                        $"STDID does not support {mode} mode");
                }

                break;
            case FileMode.Open:
            case FileMode.OpenOrCreate:
                // can be both
                break;
            case FileMode.Truncate:
                throw new NotSupportedException(
                        "STDID/OUT does not support truncation");
            default:
                throw new NotSupportedException(
                        $"BUG: {mode} mode is not supported");
        }

        if (read)
        {
            this.originalStream = this.stream = Console.OpenStandardInput();
        }
        else
        {
            this.originalStream = this.stream = Console.OpenStandardOutput();
        }

        this.Add(this.originalStream);
    }

    /// <summary>
    /// Gets collection of supported compression extensions by
    /// this class with included period, e.g. ".gz".
    /// </summary>
    public static IEnumerable<string> SupportedCompressionExtensions => Extensions;

    /// <summary>
    /// Gets a value indicating whether this instance wraps compressed
    /// stream.
    /// </summary>
    public bool IsCompressed => !ReferenceEquals(this.originalStream, this.stream);

    /// <summary>
    /// Gets compression format of this stream.
    /// </summary>
    public CompressionFormat CompressionFormat { get; } = CompressionFormat.None;

    /// <summary>
    /// Implicitly convert the <paramref name="flexFile"/> instant to an actual
    /// <see cref="Stream"/> which can be read or written to. Mind corrupt files
    /// which can cause <see cref="InvalidDataException"/> when reading.
    /// </summary>
    /// <param name="flexFile">Flex file instance ot convert.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     is required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this
    ///     instance is disposed.</exception>
    public static implicit operator Stream(FlexFile flexFile)
    {
        Ensure.Param(flexFile).Done();

        return flexFile.ToStream();
    }

    /// <summary>
    /// Try to get possible path for given base path.
    /// This method tests first existence of the <paramref name="basePath"/>
    /// as well as consequently all the supported compression extensions added
    /// to it (e.g. for Gzip the <paramref name="basePath"/> + ".gz" will
    /// be probed).
    /// </summary>
    /// <remarks>You can see the supported extensions in
    /// <see cref="SupportedCompressionExtensions"/>.</remarks>
    /// <param name="basePath">Base file path with
    ///     no compression extensions.</param>
    /// <param name="foundPath">First fount path leading
    ///     to an existing file if any.</param>
    /// <returns><see langword="true"/> if file for <paramref name="basePath"/>
    ///     or its supported compressed variant was found,
    ///     <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static bool TryGetPossiblePath(
            string basePath,
            [NotNullWhen(returnValue: true)] out string? foundPath)
    {
        Ensure.Param(basePath).Done();

        foundPath = default;

        // avoid false positives from probing files
        // names just as SupportedCompressionExtensions
        if (basePath.EndsWith(Path.DirectorySeparatorChar)
                || basePath.EndsWith(Path.AltDirectorySeparatorChar))
        {
            return false;
        }

        if (File.Exists(basePath))
        {
            foundPath = basePath;
            return true;
        }

        foreach (string extension in SupportedCompressionExtensions)
        {
            string alternativePath = basePath + extension;

            if (File.Exists(alternativePath))
            {
                foundPath = alternativePath;
                return true;
            }
        }

        return false;
    }

    /* See https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/IO/File.cs */

    /* TODO: enable in dotnet 6 or later
    /// <summary>
    /// Initializes a new instance of the <see cref="FlexFile"/> class with the specified path, creation mode, read/write and sharing permission, the access other FileStreams can have to the same file, the buffer size, additional file options and the allocation size.
    /// </summary>
    /// <remarks><see cref="FileStream(string, FileStreamOptions)"/> for information about exceptions.</remarks>
    /// <param name="path">The file to open.</param>
    /// <param name="options">An object that describes optional <see cref="FileStream"/> parameters to use.</param>
    /// <returns>A <see cref="FlexFile"/> instance that wraps the opened file.</returns>
    public static FlexFile Open(
            string path,
            System.IO.FileStreamOptions options)
    {
#pragma warning disable CA2000 // Dispose objects before losing scope
        return new FlexFile(File.Open(path, options));
#pragma warning restore CA2000 // Dispose objects before losing scope
    }
    */

    /// <summary>
    /// Opens a <see cref="FlexFile"/> on the specified path with read/write access with no sharing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Default compression level is <see cref="CompressionLevel.Optimal"/>.
    /// </para>
    /// </remarks>
    /// <param name="path">The file to open.</param>
    /// <param name="mode">A <see cref="FileMode"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten.</param>
    /// <returns>A <see cref="FlexFile"/> opened in the specified mode and path, with read/write access and not shared.</returns>
    /// <exception cref="ArgumentException">.NET Framework and .NET Core versions older than 2.1: path is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for invalid characters by using the <see cref="Path.GetInvalidPathChars"/> method.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
    /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid, (for example, it is on an unmapped drive).</exception>
    /// <exception cref="IOException">An I/O error occurred while opening the file.</exception>
    /// <exception cref="UnauthorizedAccessException"><paramref name="path"/> specified a file that is read-only. -or- This operation is not supported on the current platform. -or- <paramref name="path"/> specified a directory. -or- The caller does not have the required permission. -or- <paramref name="mode"/> is Create and the specified file is a hidden file.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="mode"/> specified an invalid value.</exception>
    /// <exception cref="FileNotFoundException">The file specified in <paramref name="path"/> was not found.</exception>
    /// <exception cref="NotSupportedException"><paramref name="path"/> is in an invalid format.</exception>
    public static FlexFile Open(
            string path,
            FileMode mode)
    {
        Ensure.Param(path).Done();

#pragma warning disable CA2000 // Dispose objects before losing scope
        return path.Equals(StdIn, StringComparison.Ordinal)
                ? new FlexFile(
                    mode,
                    mode is FileMode.Append or FileMode.Create or FileMode.CreateNew ? FileAccess.Write : FileAccess.Read)
                : new FlexFile(File.Open(path, mode));
#pragma warning restore CA2000 // Dispose objects before losing scope
    }

    /// <summary>
    /// Opens a <see cref="FlexFile"/> on the specified path with read/write access with no sharing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Default compression level is <see cref="CompressionLevel.Optimal"/>.
    /// </para>
    /// </remarks>
    /// <param name="path">The file to open.</param>
    /// <param name="mode">A <see cref="FileMode"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten.</param>
    /// <param name="access">A <see cref="FileAccess"/> value that specifies the operations that can be performed on the file.</param>
    /// <returns>An unshared <see cref="FlexFile"/> that provides access to the specified file, with the specified mode and access.</returns>
    /// <exception cref="ArgumentException">.NET Framework and .NET Core versions older than 2.1: path is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for invalid characters by using the <see cref="Path.GetInvalidPathChars"/> method
    ///     -or- <paramref name="access"/> specified Read and mode specified Create, CreateNew, Truncate, or Append.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
    /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid, (for example, it is on an unmapped drive).</exception>
    /// <exception cref="IOException">An I/O error occurred while opening the file.</exception>
    /// <exception cref="UnauthorizedAccessException"><paramref name="path"/> specified a file that is read-only. -or- This operation is not supported on the current platform. -or- <paramref name="path"/> specified a directory. -or- The caller does not have the required permission. -or- <paramref name="mode"/> is Create and the specified file is a hidden file.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="mode"/> or <paramref name="access"/> specified an invalid value.</exception>
    /// <exception cref="FileNotFoundException">The file specified in <paramref name="path"/> was not found.</exception>
    /// <exception cref="NotSupportedException"><paramref name="path"/> is in an invalid format.</exception>
    public static FlexFile Open(
            string path,
            FileMode mode,
            FileAccess access)
    {
        Ensure.Param(path).Done();

#pragma warning disable CA2000 // Dispose objects before losing scope
        return path.Equals(StdIn, StringComparison.Ordinal)
                ? new FlexFile(mode, access)
                : new FlexFile(File.Open(path, mode, access));
#pragma warning restore CA2000 // Dispose objects before losing scope
    }

    /// <summary>
    /// Opens a <see cref="FlexFile"/> on the specified path with read/write access with no sharing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Default compression level is <see cref="CompressionLevel.Optimal"/>.
    /// </para>
    /// </remarks>
    /// <param name="path">The file to open.</param>
    /// <param name="mode">A <see cref="FileMode"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten.</param>
    /// <param name="access">A <see cref="FileAccess"/> value that specifies the operations that can be performed on the file.</param>
    /// <param name="share">A <see cref="FileShare"/> value specifying the type of access other threads have to the file.</param>
    /// <returns>A <see cref="FlexFile"/> on the specified path, having the specified mode with read, write, or read/write access and the specified sharing option.</returns>
    /// <exception cref="ArgumentException">.NET Framework and .NET Core versions older than 2.1: path is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for invalid characters by using the <see cref="Path.GetInvalidPathChars"/> method
    ///     -or- <paramref name="access"/> specified Read and mode specified Create, CreateNew, Truncate, or Append.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
    /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid, (for example, it is on an unmapped drive).</exception>
    /// <exception cref="IOException">An I/O error occurred while opening the file.</exception>
    /// <exception cref="UnauthorizedAccessException"><paramref name="path"/> specified a file that is read-only. -or- This operation is not supported on the current platform. -or- <paramref name="path"/> specified a directory. -or- The caller does not have the required permission. -or- <paramref name="mode"/> is Create and the specified file is a hidden file.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="mode"/>, <paramref name="access"/>, or <paramref name="share"/> specified an invalid value.</exception>
    /// <exception cref="FileNotFoundException">The file specified in <paramref name="path"/> was not found.</exception>
    /// <exception cref="NotSupportedException"><paramref name="path"/> is in an invalid format.</exception>
    public static FlexFile Open(
            string path,
            FileMode mode,
            FileAccess access,
            FileShare share)
    {
        Ensure.Param(path).Done();

#pragma warning disable CA2000 // Dispose objects before losing scope
        return path.Equals(StdIn, StringComparison.Ordinal)
                ? new FlexFile(mode, access)
                : new FlexFile(File.Open(path, mode, access, share));
#pragma warning restore CA2000 // Dispose objects before losing scope
    }

    /// <summary>
    /// Convert this instance to actual stream which can be used
    /// to write or read from. Mind corrupt files which can cause
    /// <see cref="InvalidDataException"/> when reading.
    /// </summary>
    /// <returns>Instance of <see cref="Stream"/>.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if this
    ///     instance is disposed.</exception>
    public Stream ToStream()
    {
        this.EnsureNotDisposed();

        return this.stream;
    }

    /// <summary>
    /// Read whole string till the end of stream from this instance.
    /// </summary>
    /// <remarks>This method is mainly for testing and very small payloads.</remarks>
    /// <param name="encoding">Encoding to use, defaults to UTF8.</param>
    /// <returns>String payload.</returns>
    /// <exception cref="InvalidOperationException">Thrown
    ///     if this instance represents non readable stream.</exception>
    /// <exception cref="InvalidDataException">Thrown
    ///     if data stream is corrupt.</exception>
    public async Task<string> ReadAllTextAsync(
            Encoding? encoding = null)
    {
        if (!this.stream.CanRead)
        {
            throw new InvalidOperationException(
                    "Reading is not possible from this instance.");
        }

        encoding ??= Encoding.UTF8;

        using StreamReader s = new(this.stream, encoding);

        return await s.ReadToEndAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Read all bytes till the end of stream from this instance.
    /// </summary>
    /// <remarks>This method is mainly for testing and very small payloads.</remarks>
    /// <returns>Array of bytes.</returns>
    /// <exception cref="InvalidOperationException">Thrown
    ///     if this instance represents non readable stream.</exception>
    /// <exception cref="InvalidDataException">Thrown
    ///     if data stream is corrupt.</exception>
    public byte[] ReadAllBytes()
    {
        if (!this.stream.CanRead)
        {
            throw new InvalidOperationException(
                    "Reading is not possible from this instance.");
        }

        List<byte> result = new();

        while (true)
        {
            int next = this.stream.ReadByte();

            if (next == -1)
            {
                break;
            }

            result.Add((byte)next);
        }

        return result.ToArray();
    }
}
