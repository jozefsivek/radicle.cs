namespace Radicle.Alike.Redis;

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Radicle.Alike.Redis.Extensions;
using Radicle.Alike.Redis.Models;
using Radicle.Common.Check;

// TODO: profile this implementation against the one with spans

/// <summary>
/// Stream wrapper class providing RESP protocol writing functionality.
/// In this regard it can be also used for append only file (AOF)
/// writing. This class is not thread safe.
/// </summary>
public class RESPWriter : RESPStreamWrapper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RESPWriter"/> class.
    /// </summary>
    /// <remarks>Note that given <paramref name="stream"/>
    /// is automatically disposed when this instance is
    /// disposed.</remarks>
    /// <param name="stream">Stream to write to.</param>
    /// <param name="leaveOpen">Flag specifying to leave
    ///     <paramref name="stream"/> open (and hence not disposed)
    ///     when disposing this instance. Defaults to
    ///     <see langword="false"/> so the <paramref name="stream"/>
    ///     is disposed when this instance is disposed.</param>
    /// <param name="protocolVersion">Protocol version to use.
    ///     Defaults to latest.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if any required argument is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="stream"/> is not writable.</exception>
    public RESPWriter(
            Stream stream,
            bool leaveOpen = false,
            RESPVersions protocolVersion = RESPVersions.RESP3)
        : base(stream, leaveOpen: leaveOpen, protocolVersion: protocolVersion)
    {
        Ensure.Param(stream)
                .That(s => s.CanWrite, arg => $"{arg.Description} is not writable stream")
                .Done();
    }

    /// <summary>
    /// Gets or sets a value indicating whether
    /// this instance is disposed.
    /// </summary>
    internal bool Disposed { get; set; }

    /// <summary>
    /// Write given RESP value. Synchronous overrides are
    /// handy when dealing with in-memory streams.
    /// </summary>
    /// <param name="value">Value to write.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if any required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this instance is
    ///     already disposed.</exception>
    /// <exception cref="InvalidOperationException">Thrown
    ///     if <paramref name="value"/> can not be written because
    ///     the type is not supported by this
    ///     <see cref="RESPStreamWrapper.ProtocolVersion"/>.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    public void Write(RESPValue value)
    {
        Ensure.Param(value).Done();

        this.EnsureNotDisposed();

        if (!value.Type.GetVersion().HasFlag(this.ProtocolVersion))
        {
            throw new InvalidOperationException(
                    $"Can not write {value.Type} in {this.ProtocolVersion}");
        }

        WriteAny(this.Stream, value, this.ProtocolVersion);
    }

    /// <summary>
    /// Write given RESP value.
    /// </summary>
    /// <param name="value">Value to write.</param>
    /// <returns>Awaitable value task.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if any required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this instance is
    ///     already disposed.</exception>
    /// <exception cref="InvalidOperationException">Thrown
    ///     if <paramref name="value"/> can not be written because
    ///     the type is not supported by this
    ///     <see cref="RESPStreamWrapper.ProtocolVersion"/>.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    public async ValueTask WriteAsync(RESPValue value)
    {
        Ensure.Param(value).Done();

        this.EnsureNotDisposed();

        if (!value.Type.GetVersion().HasFlag(this.ProtocolVersion))
        {
            throw new InvalidOperationException(
                    $"Can not write {value.Type} in {this.ProtocolVersion}");
        }

        await WriteAnyAsync(this.Stream, value, this.ProtocolVersion).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsyncCore()
    {
        if (this.Disposed)
        {
            return;
        }
        else if (this.LeaveOpen)
        {
            // see https://github.com/dotnet/corefx/blob/09e2417cd0505df4558535651efb1bbcffdf0c59/src/Common/src/CoreLib/System/IO/BinaryWriter.cs#L80
            await this.Stream.FlushAsync().ConfigureAwait(false);
        }

        await base.DisposeAsyncCore().ConfigureAwait(false);

        this.Disposed = true;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (this.Disposed)
        {
            return;
        }
        else if (this.LeaveOpen)
        {
            // see https://github.com/dotnet/corefx/blob/09e2417cd0505df4558535651efb1bbcffdf0c59/src/Common/src/CoreLib/System/IO/BinaryWriter.cs#L80
            this.Stream.Flush();
        }

        base.Dispose(disposing);

        this.Disposed = true;
    }

    private static byte[] EncodeLength(ulong length)
    {
        return RESPNames.DefaultEncoding.GetBytes(length.ToString(CultureInfo.InvariantCulture));
    }

    private static void WriteAny(
            Stream stream,
            RESPValue value,
            RESPVersions compatibility)
    {
        switch (value.Type)
        {
            case RESPDataType.SimpleString:
                Write(stream, (RESPSimpleString)value);
                break;
            case RESPDataType.SimpleError:
                Write(stream, (RESPSimpleError)value);
                break;
            case RESPDataType.Array:
                Write(stream, (RESPArray)value, compatibility);
                break;
            case RESPDataType.Number:
                Write(stream, (RESPNumber)value);
                break;
            case RESPDataType.BlobString:
                Write(stream, (RESPBlobString)value);
                break;
            case RESPDataType.Null:
                WriteNull(stream, compatibility);
                break;

            case RESPDataType.FloatingPointNumber:
            case RESPDataType.BlobError:
            case RESPDataType.Map:
            case RESPDataType.Boolean:
            case RESPDataType.Set:
            case RESPDataType.VerbatimString:
            case RESPDataType.BigNumber:
            case RESPDataType.Push:
            case RESPDataType.Attribute:
                throw new NotImplementedException("RESP3 is not yet implemented.");
            default:
                throw new NotSupportedException($"BUG: {value.Type} is not supported.");
        }
    }

    private static async ValueTask WriteAnyAsync(
            Stream stream,
            RESPValue value,
            RESPVersions compatibility)
    {
        switch (value.Type)
        {
            case RESPDataType.SimpleString:
                await WriteAsync(stream, (RESPSimpleString)value).ConfigureAwait(false);
                break;
            case RESPDataType.SimpleError:
                await WriteAsync(stream, (RESPSimpleError)value).ConfigureAwait(false);
                break;
            case RESPDataType.Array:
                await WriteAsync(stream, (RESPArray)value, compatibility).ConfigureAwait(false);
                break;
            case RESPDataType.Number:
                await WriteAsync(stream, (RESPNumber)value).ConfigureAwait(false);
                break;
            case RESPDataType.BlobString:
                await WriteAsync(stream, (RESPBlobString)value).ConfigureAwait(false);
                break;
            case RESPDataType.Null:
                await WriteNullAsync(stream, compatibility).ConfigureAwait(false);
                break;

            case RESPDataType.FloatingPointNumber:
            case RESPDataType.BlobError:
            case RESPDataType.Map:
            case RESPDataType.Boolean:
            case RESPDataType.Set:
            case RESPDataType.VerbatimString:
            case RESPDataType.BigNumber:
            case RESPDataType.Push:
            case RESPDataType.Attribute:
                throw new NotImplementedException("RESP3 is not yet implemented.");
            default:
                throw new NotSupportedException($"BUG: {value.Type} is not supported.");
        }
    }

    private static void Write(Stream stream, RESPSimpleString simpleString)
    {
        // +<value><CR><LF>
        stream.WriteByte((byte)RESPNames.SimpleString);
        stream.Write(simpleString.Value.ToArray());
        stream.Write(RESPNames.NewLine);
    }

    private static async ValueTask WriteAsync(Stream stream, RESPSimpleString simpleString)
    {
        // +<value><CR><LF>
        stream.WriteByte((byte)RESPNames.SimpleString);
        await stream.WriteAsync(simpleString.Value.ToArray()).ConfigureAwait(false);
        await stream.WriteAsync(RESPNames.NewLine).ConfigureAwait(false);
    }

    private static void Write(Stream stream, RESPSimpleError simpleError)
    {
        // -CODE <value><CR><LF>
        // "CODE" is part of error
        stream.WriteByte((byte)RESPNames.SimpleError);
        stream.Write(simpleError.Value.ToArray());
        stream.Write(RESPNames.NewLine);
    }

    private static async ValueTask WriteAsync(Stream stream, RESPSimpleError simpleError)
    {
        // -CODE <value><CR><LF>
        // "CODE" is part of error
        stream.WriteByte((byte)RESPNames.SimpleError);
        await stream.WriteAsync(simpleError.Value.ToArray()).ConfigureAwait(false);
        await stream.WriteAsync(RESPNames.NewLine).ConfigureAwait(false);
    }

    private static void Write(Stream stream, RESPArray array, RESPVersions compatibility)
    {
        // *<numelements><CR><LF> ... numelements other types ...
        stream.WriteByte((byte)RESPNames.Array);
        stream.Write(EncodeLength(array.Length));
        stream.Write(RESPNames.NewLine);

        foreach (RESPValue item in array)
        {
            WriteAny(stream, item, compatibility);
        }
    }

    private static async ValueTask WriteAsync(Stream stream, RESPArray array, RESPVersions compatibility)
    {
        // *<numelements><CR><LF> ... numelements other types ...
        stream.WriteByte((byte)RESPNames.Array);
        await stream.WriteAsync(EncodeLength(array.Length)).ConfigureAwait(false);
        await stream.WriteAsync(RESPNames.NewLine).ConfigureAwait(false);

        foreach (RESPValue item in array)
        {
            await WriteAnyAsync(stream, item, compatibility).ConfigureAwait(false);
        }
    }

    private static void Write(Stream stream, RESPNumber number)
    {
        // :<signed 64-bit integer><CR><LF>
        stream.WriteByte((byte)RESPNames.Number);

        byte[] value = RESPNames.DefaultEncoding.GetBytes(number.Value.ToString(CultureInfo.InvariantCulture));

        stream.Write(value);
        stream.Write(RESPNames.NewLine);
    }

    private static async ValueTask WriteAsync(Stream stream, RESPNumber number)
    {
        // :<signed 64-bit integer><CR><LF>
        stream.WriteByte((byte)RESPNames.Number);

        byte[] value = RESPNames.DefaultEncoding.GetBytes(number.Value.ToString(CultureInfo.InvariantCulture));

        await stream.WriteAsync(value).ConfigureAwait(false);
        await stream.WriteAsync(RESPNames.NewLine).ConfigureAwait(false);
    }

    private static void Write(Stream stream, RESPBlobString blobString)
    {
        // $<length>\r\n<bytes>\r\n
        stream.WriteByte((byte)RESPNames.BlobString);
        stream.Write(EncodeLength(blobString.Length));
        stream.Write(RESPNames.NewLine);

        stream.Write(blobString.Value.ToArray());
        stream.Write(RESPNames.NewLine);
    }

    private static async ValueTask WriteAsync(Stream stream, RESPBlobString blobString)
    {
        // $<length>\r\n<bytes>\r\n
        stream.WriteByte((byte)RESPNames.BlobString);
        await stream.WriteAsync(EncodeLength(blobString.Length)).ConfigureAwait(false);
        await stream.WriteAsync(RESPNames.NewLine).ConfigureAwait(false);

        await stream.WriteAsync(blobString.Value.ToArray()).ConfigureAwait(false);
        await stream.WriteAsync(RESPNames.NewLine).ConfigureAwait(false);
    }

    private static void WriteNull(Stream stream, RESPVersions compatibility)
    {
        if (compatibility.HasFlag(RESPVersions.RESP3))
        {
            // RESP3: _\r\n
            stream.WriteByte((byte)RESPNames.Null);
        }
        else if (compatibility == RESPVersions.RESP2)
        {
            // RESP2 v2 *-1 and $-1 null values
            stream.Write(RESPNames.RESP2NullAltBlobString);
        }

        stream.Write(RESPNames.NewLine);
    }

    private static async ValueTask WriteNullAsync(Stream stream, RESPVersions compatibility)
    {
        if (compatibility.HasFlag(RESPVersions.RESP3))
        {
            // RESP3: _\r\n
            stream.WriteByte((byte)RESPNames.Null);
        }
        else if (compatibility == RESPVersions.RESP2)
        {
            // RESP2 v2 *-1 and $-1 null values
            await stream.WriteAsync(RESPNames.RESP2NullAltBlobString).ConfigureAwait(false);
        }

        await stream.WriteAsync(RESPNames.NewLine).ConfigureAwait(false);
    }
}
