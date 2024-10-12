namespace Radicle.Common.Compression;

using System;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Radicle.Common.Check;

/// <summary>
/// No hassle API for Gzip compression.
/// </summary>
public static class Gzip
{
    /// <summary>
    /// Threshold above which it is meaningfull to compress
    /// normal text string with GZip compression.
    /// </summary>
    public const int TextStringCompressionThreshold = 250;

    /// <summary>
    /// Threshold above which it is meaningfull to compress
    /// random text string with GZip compression.
    /// </summary>
    public const int RandomStringCompressionThreshold = 270;

    /*
    See the RFC http://www.zlib.org/rfc-gzip.html for the full spec.

    Comparing c# API and the command line tool to get payload of the empty
    array:

    payload     [00]           [a1]           [] (in file called "empty")
    method      c#             c#             gzip
    compressed*
                1f 8b 08 00    -- -- -- --    -- -- -- 08  <- [1f 8b] gzip (2B) +
                                                              CM deflate method +
                                                              FLG (FNAME is on for gzip CLI)
                00 00 00 00    -- -- -- --    14 72 37 60  <- MTIME (4B) of file if any
                00 13          -- --          -- 03 65 6d  <- XFL (1B) optimal compression +
                                                              OS (1B) +
                                                              fine name (2B) ..
                                              70 74 79 00  <- .. file name "empty" (4B)
                63 00 00 8d    5B 08 __ f3    03 00        <- compressed blocks
                ef 02 d2 01    7c d3 73 01    00 00 00 00  <- CRC-32 (4B)
                00 00 00       -- -- --       -- -- -- 00  <- ISIZE size of the original input data modulo 2^32

    * - payload ("--" stands for same as before, left; chunks are aligned to see difference)
     */
    private static readonly ImmutableArray<byte> EmptyGzipPayload = new byte[]
    {
        0x1f, 0x8b, 0x08, 0x00,
        0x00, 0x00, 0x00, 0x00,
        0x00, 0x13,
        0x03, 0x00,
        0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00,
    }.ToImmutableArray();

    /// <summary>
    /// Determine if the given bytes appear to be compressed payload.
    /// </summary>
    /// <param name="bytes">Bytes to check.</param>
    /// <returns><see langword="true"/> if the payload appears to be compressed;
    /// -or- <see langword="false"/> otherwise.</returns>
    public static bool IsProbablyCompressedPayload(byte[] bytes)
    {
        Ensure.Param(bytes).Done();

        // lengths + magic number
        return (bytes.Length >= 10) && (bytes[0] == 0x1f) && (bytes[1] == 0x8b);
    }

    /// <summary>
    /// Compress given string.
    /// </summary>
    /// <param name="value">String to compress.</param>
    /// <param name="encoding">Encoding to use to retrieve bytes
    ///     for given <paramref name="value"/>.
    ///     Defaults to <see cref="Encoding.UTF8"/>.</param>
    /// <returns>Compressed payload.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langowrd="null"/>.</exception>
    /// <exception cref="EncoderFallbackException">Thrown if fallback occurred
    ///     during call to <see cref="Encoding.GetBytes(string)"/>
    ///     (see Character Encoding in .NET for complete explanation).</exception>
    public static byte[] Compress(
            string value,
            Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        byte[] bytes = encoding.GetBytes(Ensure.Param(value).Value);

        return Compress(bytes);
    }

    /// <summary>
    /// Compress given byte input.
    /// </summary>
    /// <remarks>See http://stackoverflow.com/questions/7343465/compression-decompression-string-with-c-sharp .</remarks>
    /// <param name="bytes">Bytes to compress.</param>
    /// <returns>Compressed payload.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langowrd="null"/>.</exception>
    public static byte[] Compress(byte[] bytes)
    {
        Ensure.Param(bytes).Done();

        // Compressing empty payload produces corrupted payload,
        // see https://stackoverflow.com/questions/24024886/using-gzipstream-to-compress-empty-input-results-in-an-invalid-gz-file-in-c-shar
        if (bytes.Length == 0)
        {
            return EmptyGzipPayload.ToArray();
        }

        using MemoryStream inStream = new(bytes);
        using MemoryStream outStream = new();

        using (GZipStream gs = new(outStream, CompressionMode.Compress))
        {
            inStream.CopyTo(gs);
        }

        return outStream.ToArray();
    }

    /// <summary>
    /// Uncompress compressed payload to string.
    /// </summary>
    /// <param name="bytes">Bytes to uncompress.</param>
    /// <param name="encoding">Encoding to use to retrieve bytes
    ///     for given <paramref name="bytes"/>.
    ///     Defaults to <see cref="Encoding.UTF8"/>.</param>
    /// <returns>Uncompressed string.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langowrd="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="bytes"/>
    ///     is not valid gzip payload or the uncompressed byte array
    ///     contains invalid Unicode code points.</exception>
    /// <exception cref="DecoderFallbackException">Thrown if s fallback occurred
    ///     during call to <see cref="Encoding.GetString(byte[])"/>
    ///     (see Character Encoding in .NET for complete explanation). </exception>
    public static string UncompressString(
            byte[] bytes,
            Encoding? encoding = null)
    {
        byte[] unpacked = Uncompress(bytes);
        encoding ??= Encoding.UTF8;

        return encoding.GetString(unpacked);
    }

    /// <summary>
    /// Uncompress payload.
    /// </summary>
    /// <param name="bytes">Bytes to uncompress.</param>
    /// <returns>Uncompressed bytes.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langowrd="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if
    ///     <paramref name="bytes"/> is not valid gzip payload
    ///     because it is too short.</exception>
    /// <exception cref="ArgumentException">Thrown if the payload
    ///     is invalid.</exception>
    public static byte[] Uncompress(
            byte[] bytes)
    {
        // 10 till OS tag
        Ensure.Param(bytes).InRange(10, int.MaxValue).Done();

        using MemoryStream inStream = new(bytes);
        using MemoryStream outStream = new();

        try
        {
            using GZipStream gzipStream = new(inStream, CompressionMode.Decompress);

            gzipStream.CopyTo(outStream);
        }
        catch (InvalidDataException exc)
        {
            throw new ArgumentException(exc.Message, exc);
        }

        return outStream.ToArray();
    }
}
