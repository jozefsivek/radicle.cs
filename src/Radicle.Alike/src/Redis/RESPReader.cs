namespace Radicle.Alike.Redis;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Radicle.Alike.Redis.Models;
using Radicle.Common.Check;

/// <summary>
/// Stream wrapper class providing RESP protocol reading functionality.
/// In this regard it can be also used for append only file (AOF)
/// reading. This class is not thread safe.
/// </summary>
public class RESPReader : RESPStreamWrapper
{
    /*
    Streamed string example:

    $?<CR><LF>
    ;4<CR><LF>
    Hell<CR><LF>
    ;5<CR><LF>
    o wor<CR><LF>
    ;1<CR><LF>
    d<CR><LF>
    ;0<CR><LF>

    aggregated data types of type Array, Set and Map, not specifying the length, but instead using an explicit terminator
    END type: `.<CR><LF>`

    %?<CR><LF>
    +a<CR><LF>
    :1<CR><LF>
    +b<CR><LF>
    :2<CR><LF>
    .<CR><LF>
     */

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPReader"/> class.
    /// Create new instance of <see cref="RESPReader"/>
    /// reading from given <paramref name="stream"/>.
    /// </summary>
    /// <remarks>Note that given <paramref name="stream"/>
    /// is automatically disposed when this instance is
    /// disposed.</remarks>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="leaveOpen">Flag specifying to leave
    ///     <paramref name="stream"/> open (and hence not disposed)
    ///     when disposing this instance. Defaults to
    ///     <see langword="false"/> so the <paramref name="stream"/>
    ///     is disposed when this instance is disposed.</param>
    /// <param name="strict">Use strict parsing, e.g. throw on
    ///     RESP2 null values in RESP3 or throw when verbatim
    ///     string type is not recognized instead of fallback behaviour.</param>
    /// <param name="protocolVersion">Protocol version to use.
    ///     Defaults to latest.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if any required argument is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="stream"/> is not readable.</exception>
    public RESPReader(
            Stream stream,
            bool leaveOpen = false,
            bool strict = false,
            RESPVersions protocolVersion = RESPVersions.RESP3)
        : base(stream, leaveOpen: leaveOpen, protocolVersion: protocolVersion)
    {
        Ensure.Param(stream)
                .That(s => s.CanRead, arg => $"{arg.Description} is not readable stream")
                .Done();

        this.Strict = strict;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this reader is in strict mode,
    /// e.g. throw on RESP2 null values in RESP3 or throw when verbatim
    /// string type is not recognized instead of fallback behaviour.
    /// </summary>
    public bool Strict { get; set; }

    /// <summary>
    /// Read RESP value.
    /// </summary>
    /// <returns>Instance of <see cref="RESPValue"/>.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if this instance is
    ///     already disposed.</exception>
    /// <exception cref="FormatException">Throw when
    ///     format error is encountered or the type is not supported by this
    ///     <see cref="RESPStreamWrapper.ProtocolVersion"/>. Note this is different
    ///     in comparison to <see cref="InvalidOperationException"/> in writer
    ///     as the user is not in controll of the payload.</exception>
    /// <exception cref="IOException">Thrown is case of I/O exception
    ///     or when reading past end of file.</exception>
    public RESPValue Read()
    {
        this.EnsureNotDisposed();

        RESPAttributeValue? attrib = default;

        while (true)
        {
            RESPValue value = ReadAny(
                    this.Stream,
                    this.Strict ? this.ProtocolVersion : this.ProtocolVersion | RESPVersions.RESP2,
                    attribs: attrib);

            if (value is RESPAttributeValue a)
            {
                attrib = a;
            }
            else
            {
                return value;
            }
        }
    }

    /// <summary>
    /// Read RESP value.
    /// </summary>
    /// <returns>Instance of <see cref="RESPValue"/>.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if this instance is
    ///     already disposed.</exception>
    /// <exception cref="FormatException">Throw when
    ///     format error is encountered or the type is not supported by this
    ///     <see cref="RESPStreamWrapper.ProtocolVersion"/>. Note this is different
    ///     in comparison to <see cref="InvalidOperationException"/> in writer
    ///     as the user is not in controll of the payload.</exception>
    /// <exception cref="IOException">Thrown is case of I/O exception
    ///     or when reading past end of file.</exception>
    public async ValueTask<RESPValue> ReadAsync()
    {
        this.EnsureNotDisposed();

        RESPAttributeValue? attribs = default;

        while (true)
        {
            RESPValue value = await ReadAnyAsync(
                    this.Stream,
                    this.Strict ? this.ProtocolVersion : this.ProtocolVersion | RESPVersions.RESP2,
                    attribs: attribs).ConfigureAwait(false);

            if (value is RESPAttributeValue a)
            {
                attribs = a;
            }
            else
            {
                return value;
            }
        }
    }

    private static RESPValue ReadAny(
            Stream stream,
            RESPVersions compatibility,
            RESPAttributeValue? attribs = null)
    {
        int b = stream.ReadByte();

        if (b == -1)
        {
            throw new IOException("End of the stream reached while reading RESP value.");
        }

        if (!compatibility.HasFlag(RESPVersions.RESP3)
                && RESPNames.InitBytesRESP3.TryGetValue(b, out RESPDataType t))
        {
            throw new FormatException(
                    $"Can not read {t} in {compatibility}");
        }

        return b switch
        {
            RESPNames.SimpleString => ReadSimpleString(stream, attribs),
            RESPNames.SimpleError => ReadSimpleError(stream, attribs),
            RESPNames.Array => ReadArray(stream, compatibility, attribs),
            RESPNames.Number => ReadNumber(stream, attribs),
            RESPNames.BlobString => ReadBlobString(stream, compatibility, attribs),
            RESPNames.Null
                or RESPNames.FloatingPointNumber
                or RESPNames.BlobError
                or RESPNames.Map
                or RESPNames.Boolean
                or RESPNames.Set
                or RESPNames.VerbatimString
                or RESPNames.BigNumber
                or RESPNames.Push
                or RESPNames.Attribute => throw new NotImplementedException(
                        "RESP3 is not yet implemented."),
            _ => throw new FormatException($"Unexpected type byte 0x{b:x2}"),
        };
    }

    private static async ValueTask<RESPValue> ReadAnyAsync(
            Stream stream,
            RESPVersions compatibility,
            RESPAttributeValue? attribs = null)
    {
        int b = stream.ReadByte();

        if (b == -1)
        {
            throw new IOException("End of the stream reached while reading RESP value.");
        }

        if (!compatibility.HasFlag(RESPVersions.RESP3)
                && RESPNames.InitBytesRESP3.TryGetValue(b, out RESPDataType t))
        {
            throw new FormatException(
                    $"Can not read {t} in {compatibility}");
        }

        return b switch
        {
            RESPNames.SimpleString => ReadSimpleString(stream, attribs),
            RESPNames.SimpleError => ReadSimpleError(stream, attribs),
            RESPNames.Array => await ReadArrayAsync(stream, compatibility, attribs)
                    .ConfigureAwait(false),
            RESPNames.Number => ReadNumber(stream, attribs),
            RESPNames.BlobString => await ReadBlobStringAsync(stream, compatibility, attribs)
                    .ConfigureAwait(false),
            RESPNames.Null
                or RESPNames.FloatingPointNumber
                or RESPNames.BlobError
                or RESPNames.Map
                or RESPNames.Boolean
                or RESPNames.Set
                or RESPNames.VerbatimString
                or RESPNames.BigNumber
                or RESPNames.Push
                or RESPNames.Attribute => throw new NotImplementedException(
                        "RESP3 is not yet implemented."),
            _ => throw new FormatException($"Unexpected type byte 0x{b:x2}"),
        };
    }

    private static RESPValue ReadArray(
            Stream stream,
            RESPVersions compatibility,
            RESPAttributeValue? attribs = null)
    {
        (bool isNull, ulong length) = ReadPayloadLength(
                stream,
                compatibility: compatibility,
                typeAllowsNull: true);

        if (isNull)
        {
            if (attribs is null)
            {
                return RESPNull.Instance;
            }

            return new RESPNull()
            {
                Attribs = attribs,
            };
        }

        if (length > int.MaxValue)
        {
            throw new NotSupportedException($"Reading collections above {int.MaxValue} items is not supported");
        }

        RESPValue[] items = new RESPValue[length];
        RESPAttributeValue? itemAttribs = default;
        int position = 0;

        while (position < items.Length)
        {
            RESPValue value = ReadAny(
                    stream,
                    compatibility: compatibility,
                    attribs: itemAttribs);

            if (value is RESPAttributeValue a)
            {
                itemAttribs = a;
            }
            else
            {
                items[position++] = value;
            }
        }

        return new RESPArray(items)
        {
            Attribs = attribs,
        };
    }

    private static async ValueTask<RESPValue> ReadArrayAsync(
            Stream stream,
            RESPVersions compatibility,
            RESPAttributeValue? attribs = null)
    {
        (bool isNull, ulong length) = ReadPayloadLength(
                stream,
                compatibility: compatibility,
                typeAllowsNull: true);

        if (isNull)
        {
            if (attribs is null)
            {
                return RESPNull.Instance;
            }

            return new RESPNull()
            {
                Attribs = attribs,
            };
        }

        if (length > int.MaxValue)
        {
            throw new NotSupportedException($"Reading collections above {int.MaxValue} items is not supported");
        }

        RESPValue[] items = new RESPValue[length];
        RESPAttributeValue? itemAttribs = default;
        int position = 0;

        while (position < items.Length)
        {
            RESPValue value = await ReadAnyAsync(
                    stream,
                    compatibility: compatibility,
                    attribs: itemAttribs).ConfigureAwait(false);

            if (value is RESPAttributeValue a)
            {
                itemAttribs = a;
            }
            else
            {
                items[position++] = value;
            }
        }

        return new RESPArray(items)
        {
            Attribs = attribs,
        };
    }

    private static RESPValue ReadBlobString(
            Stream stream,
            RESPVersions compatibility,
            RESPAttributeValue? attribs = null)
    {
        (bool isNull, ulong length) = ReadPayloadLength(
                stream,
                compatibility: compatibility,
                typeAllowsNull: true);

        if (isNull)
        {
            if (attribs is null)
            {
                return RESPNull.Instance;
            }

            return new RESPNull()
            {
                Attribs = attribs,
            };
        }

        byte[] payload = Array.Empty<byte>();

        if (length > 0)
        {
            payload = ReadPayload(stream, length);
        }

        // finally read the new line and discard it
        ReadAndEnsureNewLine(stream);

        return new RESPBlobString(payload)
        {
            Attribs = attribs,
        };
    }

    private static async ValueTask<RESPValue> ReadBlobStringAsync(
            Stream stream,
            RESPVersions compatibility,
            RESPAttributeValue? attribs = null)
    {
        (bool isNull, ulong length) = ReadPayloadLength(
                stream,
                compatibility: compatibility,
                typeAllowsNull: true);

        if (isNull)
        {
            if (attribs is null)
            {
                return RESPNull.Instance;
            }

            return new RESPNull()
            {
                Attribs = attribs,
            };
        }

        byte[] payload = Array.Empty<byte>();

        if (length > 0)
        {
            payload = await ReadPayloadAsync(stream, length).ConfigureAwait(false);
        }

        // finally read the new line and discard it
        ReadAndEnsureNewLine(stream);

        return new RESPBlobString(payload)
        {
            Attribs = attribs,
        };
    }

    private static RESPValue ReadNumber(
            Stream stream,
            RESPAttributeValue? attribs)
    {
        List<byte> buffer = ReadUntilNewLine(stream);

        try
        {
            long number = long.Parse(
                    RESPNames.DefaultEncoding.GetString(buffer.ToArray()),
                    CultureInfo.InvariantCulture);

            return new RESPNumber(number)
            {
                Attribs = attribs,
            };
        }
        catch (ArgumentException exc)
        {
            string dump = string.Join(" ", buffer.Select(b => $"0x{b:x2}"));

            throw new FormatException($"Expected UTF-8, got: {dump}", exc);
        }
        catch (Exception exc) when (exc is FormatException or OverflowException)
        {
            string str = RESPNames.DefaultEncoding.GetString(buffer.ToArray());

            throw new FormatException($"Expected 64-bit number, got: {str}", exc);
        }
    }

    private static RESPValue ReadSimpleError(
            Stream stream,
            RESPAttributeValue? attribs)
    {
        try
        {
            return new RESPSimpleError(ReadUntilNewLine(stream, capacity: 64))
            {
                Attribs = attribs,
            };
        }
        catch (ArgumentException exc)
        {
            throw new FormatException(
                    $"New line ('\\r' or '\\n') in {RESPSimpleError.HumanName}",
                    exc);
        }
    }

    private static RESPValue ReadSimpleString(
            Stream stream,
            RESPAttributeValue? attribs)
    {
        try
        {
            return new RESPSimpleString(ReadUntilNewLine(stream, capacity: 16))
            {
                Attribs = attribs,
            };
        }
        catch (ArgumentException exc)
        {
            throw new FormatException(
                    $"New line ('\\r' or '\\n') in {RESPSimpleString.HumanName}",
                    exc);
        }
    }

    /// <summary>
    /// Read RESP 64-bit payload length.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="compatibility">Compatibility of the read.</param>
    /// <param name="typeAllowsNull">The read type allows special null value
    ///     in RESP2.</param>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="FormatException">Thrown if the stream
    ///     is not equal to <see cref="RESPNames.NewLine"/>.</exception>
    private static (bool IsNull, ulong Length) ReadPayloadLength(
            Stream stream,
            RESPVersions compatibility,
            bool typeAllowsNull = false)
    {
        List<byte> lengthBuffer = ReadUntilNewLine(stream);

        if (
                typeAllowsNull
                && compatibility == RESPVersions.RESP2
                && lengthBuffer.Count == RESPNames.RESP2NullLength.Length
                && lengthBuffer[0] == RESPNames.RESP2NullLength[0] // quickly check '-'
                && RESPNames.RESP2NullLength.SequenceEqual(lengthBuffer))
        {
            return (true, default);
        }

        ulong length;

        try
        {
            length = ulong.Parse(
                    Encoding.UTF8.GetString(lengthBuffer.ToArray()),
                    CultureInfo.InvariantCulture);
        }
        catch (ArgumentException exc)
        {
            string dump = string.Join(" ", lengthBuffer.Select(b => $"0x{b:x2}"));

            throw new FormatException($"Expected UTF-8, got: {dump}", exc);
        }
        catch (Exception exc) when (exc is FormatException or OverflowException)
        {
            string str = Encoding.UTF8.GetString(lengthBuffer.ToArray());

            if (str == "-1")
            {
                throw new FormatException(
                        $"Expected 64-bin unsigned number, got: {str}"
                            + ". Note the null value serialization is changed in RESP3"
                            + " and `*-1` and `$-1` are not null values anymore"
                            + " ( https://github.com/redis/redis-specifications/blob/master/protocol/RESP3.md ).",
                        exc);
            }

            throw new FormatException($"Expected 64-bin unsigned number, got: {str}.", exc);
        }

        return (false, length);
    }

    /// <summary>
    /// Read payload of given length from the given <paramref name="stream"/>.
    /// Note reading a large payload in one go is not supported by <see cref="Stream"/>
    /// directly, use this method.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="length">Length of bytes to read.</param>
    /// <returns>Read buffer.</returns>
    /// <exception cref="IOException">Thrown in case of I/O exception.</exception>
    /// <exception cref="NotSupportedException">Thrown if reading
    ///     payload lenght above currently supported <see cref="int.MaxValue"/>.</exception>
    private static byte[] ReadPayload(
            Stream stream,
            ulong length)
    {
        // see for example:
        // https://docs.microsoft.com/en-us/dotnet/api/system.io.stream.read?view=net-6.0#System_IO_Stream_Read_System_Byte___System_Int32_System_Int32_
        // or https://stackoverflow.com/questions/7542235/read-specific-number-of-bytes-from-networkstream
        if (length > int.MaxValue)
        {
            throw new NotSupportedException($"Reading payloads above {int.MaxValue} is not supported");
        }

        byte[] buffer = new byte[length];
        int bytesToRead = (int)length;
        int offset = 0;

        while (bytesToRead > 0)
        {
            int read = stream.Read(buffer, offset, bytesToRead);
            offset += read;
            bytesToRead -= read;

            if (read == 0)
            {
                throw new IOException(
                        "End of the stream reached while reading RESP blob, "
                        + $"expected {length} bytes, got {offset} bytes");
            }
            else if (bytesToRead < 0)
            {
                throw new IOException(
                        "Unexpected read length in the stream, "
                        + $"expected {length} bytes, got {offset} bytes");
            }
        }

        return buffer;
    }

    /// <summary>
    /// Read payload of given length from the given <paramref name="stream"/>.
    /// Note reading a large payload in one go is not supported by <see cref="Stream"/>
    /// directly, use this method.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="length">Length of bytes to read.</param>
    /// <returns>Read buffer.</returns>
    /// <exception cref="IOException">Thrown in case of I/O exception.</exception>
    /// <exception cref="NotSupportedException">Thrown if reading
    ///     payload lenght above currently supported <see cref="int.MaxValue"/>.</exception>
    private static async ValueTask<byte[]> ReadPayloadAsync(
            Stream stream,
            ulong length)
    {
        // see for example:
        // https://docs.microsoft.com/en-us/dotnet/api/system.io.stream.read?view=net-6.0#System_IO_Stream_Read_System_Byte___System_Int32_System_Int32_
        // or https://stackoverflow.com/questions/7542235/read-specific-number-of-bytes-from-networkstream
        if (length > int.MaxValue)
        {
            throw new NotSupportedException($"Reading payloads above {int.MaxValue} is not supported");
        }

        byte[] buffer = new byte[length];
        int bytesToRead = (int)length;
        int offset = 0;

        while (bytesToRead > 0)
        {
#pragma warning disable CA1835 // Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'
            int read = await stream.ReadAsync(buffer, offset, bytesToRead).ConfigureAwait(false);
#pragma warning restore CA1835 // Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'
            offset += read;
            bytesToRead -= read;

            if (read == 0)
            {
                throw new IOException(
                        "End of the stream reached while reading RESP blob, "
                        + $"expected {length} bytes, got {offset} bytes");
            }
            else if (bytesToRead < 0)
            {
                throw new IOException(
                        "Unexpected read length in the stream, "
                        + $"expected {length} bytes, got {offset} bytes");
            }
        }

        return buffer;
    }

    /// <summary>
    /// Read RESP new line.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="FormatException">Thrown if the stream
    ///     is not equal to <see cref="RESPNames.NewLine"/>.</exception>
    private static void ReadAndEnsureNewLine(
            Stream stream)
    {
        byte position = 0;

        do
        {
            int b = stream.ReadByte();

            if (b == RESPNames.NewLine[position])
            {
                position++;
            }
            else if (b == -1)
            {
                throw new IOException("End of the stream reached while reading RESP line delimeter.");
            }
            else
            {
                byte expected = RESPNames.NewLine[position];
                byte got = (byte)b;

                throw new FormatException(
                        "Unexpected character found while reading RESP new line, "
                        + $"expected 0x{expected:x2}, got 0x{got:x2}");
            }
        }
        while (position < RESPNames.NewLine.Length);
    }

    /// <summary>
    /// Read bytes of the stream until the RESP end of line.
    /// Use for reads with unspecified length.
    /// </summary>
    /// <param name="stream">Sream to read from.</param>
    /// <param name="includeNewLine">Optionally include
    ///     the new line itself in the result.</param>
    /// <param name="capacity">Initial capacity of the buffer for reading.</param>
    /// <returns>Read bytes.</returns>
    /// <exception cref="IOException">I/O error or
    ///     if end of file was reached.</exception>
    /// <exception cref="FormatException">Thrown if the stream
    ///     does not contain correct <see cref="RESPNames.NewLine"/>,
    ///     as new line has started but not finished correctly
    ///     or .</exception>
    private static List<byte> ReadUntilNewLine(
            Stream stream,
            bool includeNewLine = false,
            int capacity = 8)
    {
        int positionNewLine = 0;
        int b;
        List<byte> resultBuffer = new(capacity: capacity);

        do
        {
            b = stream.ReadByte();

            if (b == RESPNames.NewLine[positionNewLine])
            {
                positionNewLine++;

                if (includeNewLine)
                {
                    resultBuffer.Add((byte)b);
                }
            }
            else if (b == -1)
            {
                throw new IOException("End of the stream reached while reading RESP line.");
            }
            else if (RESPNames.AllNewLineBytes.Contains((byte)b) || positionNewLine != 0)
            {
                byte expected = RESPNames.NewLine[positionNewLine];
                byte got = (byte)b;

                throw new FormatException(
                        "Unexpected character found while reading RESP new line, "
                        + $"expected 0x{expected:x2}, got 0x{got:x2}");
            }
            else
            {
                resultBuffer.Add((byte)b);
            }
        }
        while (positionNewLine < RESPNames.NewLine.Length);

        return resultBuffer;
    }
}
