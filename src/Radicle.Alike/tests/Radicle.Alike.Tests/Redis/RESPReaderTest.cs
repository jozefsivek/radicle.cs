namespace Radicle.Alike.Redis;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Radicle.Alike.Redis.Models;
using Xunit;

public class RESPReaderTest
{
    [Fact]
    public void Constructor_NullStream_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RESPReader(null!));
    }

    [Fact]
    public void Constructor_NonReadableStream_Throws()
    {
        string tmpFilePath = Path.GetTempFileName();

        try
        {
            using FileStream nonReadableStream = File.OpenWrite(Path.GetTempFileName());
            Assert.Throws<ArgumentException>(() => new RESPReader(nonReadableStream));
        }
        finally
        {
            File.Delete(tmpFilePath);
        }
    }

    [Fact]
    public async Task Dispose_NotOpenInnerStream_Disposes()
    {
        MemoryStream stream = new();
        RESPReader reader;

        using (reader = new RESPReader(stream))
        {
            // pass
        }

        Assert.Throws<ObjectDisposedException>(() =>
                reader.Read());
        await Assert.ThrowsAsync<ObjectDisposedException>(() =>
                reader.ReadAsync()).ConfigureAwait(false);
        Assert.Throws<ObjectDisposedException>(() =>
                stream.Write(new byte[] { 0x00 }, 0, 1));
    }

    [Fact]
    public async Task Dispose_OpenInnerStream_DoesNotDisposes()
    {
        using MemoryStream stream = new(1);
        RESPReader reader;

        using (reader = new RESPReader(stream, leaveOpen: true))
        {
            // pass
        }

        Assert.Throws<ObjectDisposedException>(() =>
            reader.Read());
        await Assert.ThrowsAsync<ObjectDisposedException>(() =>
            reader.ReadAsync()).ConfigureAwait(false);
        await stream.WriteAsync((new byte[] { 0x00 }).AsMemory(0, 1)).ConfigureAwait(false);
    }

    [Theory]
    [InlineData(new char[] { '|' })] // attribute
    [InlineData(new char[] { '(' })] // big number
    [InlineData(new char[] { '!' })] // blob error
    [InlineData(new char[] { '#' })] // boolean
    [InlineData(new char[] { ',' })] // double
    [InlineData(new char[] { '%' })] // map
    [InlineData(new char[] { '_' })] // RESP3 null
    [InlineData(new char[] { '>' })] // push
    [InlineData(new char[] { '~' })] // set
    [InlineData(new char[] { '=' })] // verbatim string
    [InlineData(new char[]
    {
        '*', '1', '\r', '\n',
        '|',
    })] // nested verbatim string
    public void Read_RESP3ValueInRESP2Mode_Throws(char[] inputRESP3)
    {
        MemoryStream stream = new(inputRESP3.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream, protocolVersion: RESPVersions.RESP2);

        Assert.Throws<FormatException>(() => reader.Read());
    }

    [Theory]
    [InlineData(new char[] { '|' })] // attribute
    [InlineData(new char[] { '(' })] // big number
    [InlineData(new char[] { '!' })] // blob error
    [InlineData(new char[] { '#' })] // boolean
    [InlineData(new char[] { ',' })] // double
    [InlineData(new char[] { '%' })] // map
    [InlineData(new char[] { '_' })] // RESP3 null
    [InlineData(new char[] { '>' })] // push
    [InlineData(new char[] { '~' })] // set
    [InlineData(new char[] { '=' })] // verbatim string
    [InlineData(new char[]
    {
        '*', '1', '\r', '\n',
        '|',
    })] // nested verbatim string
    public async Task ReadAsync_RESP3ValueInRESP2Mode_Throws(char[] inputRESP3)
    {
        MemoryStream stream = new(inputRESP3.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream, protocolVersion: RESPVersions.RESP2);

        await Assert.ThrowsAsync<FormatException>(() => reader.ReadAsync()).ConfigureAwait(false);
    }

    [Theory]
    [InlineData(new char[] { 'f' })]
    [InlineData(new char[] { '\t' })]
    [InlineData(new char[] { '[', '9', '\r', '\n' })]
    public void Read_InvalidValueType_Throws(char[] invalidInput)
    {
        byte invalidFormat = (byte)invalidInput[0];
        MemoryStream stream = new(invalidInput.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream, protocolVersion: RESPVersions.RESP2);

        FormatException exception = Assert.Throws<FormatException>(() => reader.Read());
        Assert.Contains($"Unexpected type byte 0x{invalidFormat:x2}", exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(new char[]
    {
        '*', '\r', '\n',
    })]
    [InlineData(new char[]
    {
        '*', '0', '.', '0', '\r', '\n',
    })]
    [InlineData(new char[]
    {
        '*', '0', 'e', '\r', '\n',
    })]
    [InlineData(new char[]
    {
        '$', '\r', '\n',
    })]
    [InlineData(new char[]
    {
        '$', 'e', '\r', '\n',
    })]
    [InlineData(new char[]
    {
        '$', ',', '\r', '\n',
    })]
    public void Read_InvalidLength_Throws(char[] invalidInput)
    {
        MemoryStream stream = new(invalidInput.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream, protocolVersion: RESPVersions.RESP2);

        FormatException exception = Assert.Throws<FormatException>(() => reader.Read());
        Assert.Contains(
                "Expected 64-bin unsigned number, got:",
                exception.Message,
                StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(new char[]
    {
        '*', '1', '\r', '\r', // here we explicitly search for new line
    })]
    [InlineData(new char[]
    {
        '*', '1', '\n', // disallowed character
    })]
    [InlineData(new char[]
    {
        '*', '1', '\r', '\n',
        '+', '1', '\r', '\b', // here we explicitly search for new line
    })]
    [InlineData(new char[]
    {
        '$', '2', '\r', '\n',
        '$', '1', '\n', '\n', // here is the end of line read implicitly
    })]
    public void Read_InvalidEndOfLine_Throws(char[] invalidInput)
    {
        MemoryStream stream = new(invalidInput.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream, protocolVersion: RESPVersions.RESP2);

        FormatException exception = Assert.Throws<FormatException>(() => reader.Read());
        Assert.Contains(
                "Unexpected character found while reading RESP new line, ",
                exception.Message,
                StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(new char[]
    {
        '*', '1', '\r',
    })]
    [InlineData(new char[]
    {
        '*', '1', '\r', '\n',
        '+', '1',
    })]
    [InlineData(new char[]
    {
        '$', '9', '\r', '\n',
        'f', 'b', '\n', '\n',
    })]
    [InlineData(new char[]
    {
        '+', '9', '\r',
    })]
    [InlineData(new char[]
    {
        '+', '9',
    })]
    [InlineData(new char[]
    {
        '+',
    })]
    [InlineData(new char[]
    {
        '$', '2', '\r', '\n',
        'a', 'b', '\r',
    })]
    [InlineData(new char[]
    {
        '$', '2', '\r', '\n',
        'b', 'b',
    })]
    [InlineData(new char[]
    {
        '$', '2', '\r', '\n',
        'c',
    })]
    [InlineData(new char[]
    {
        '$', '2', '\r', '\n',
    })]
    public void Read_UnexpectedEndOfStream_Throws(char[] invalidInput)
    {
        MemoryStream stream = new(invalidInput.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream, protocolVersion: RESPVersions.RESP2);

        IOException exception = Assert.Throws<IOException>(() => reader.Read());
        Assert.Contains(
                "End of the stream reached while reading RESP",
                exception.Message,
                StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(new char[] { 'f' })]
    public async Task ReadAsync_InvalidValueType_Throws(char[] invalidInput)
    {
        MemoryStream stream = new(invalidInput.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream);

        FormatException exception = await Assert.ThrowsAsync<FormatException>(() =>
                reader.ReadAsync()).ConfigureAwait(false);
        Assert.Contains(
                "Unexpected type byte 0x66",
                exception.Message,
                StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(new char[]
    {
        '*', '\r', '\n',
    })]
    [InlineData(new char[]
    {
        '*', '0', '.', '0', '\r', '\n',
    })]
    [InlineData(new char[]
    {
        '*', '0', 'e', '\r', '\n',
    })]
    [InlineData(new char[]
    {
        '$', '\r', '\n',
    })]
    [InlineData(new char[]
    {
        '$', 'e', '\r', '\n',
    })]
    [InlineData(new char[]
    {
        '$', ',', '\r', '\n',
    })]
    public async Task ReadAsync_InvalidLength_Throws(char[] invalidInput)
    {
        MemoryStream stream = new(invalidInput.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream);

        FormatException exception = await Assert.ThrowsAsync<FormatException>(() =>
                reader.ReadAsync()).ConfigureAwait(false);
        Assert.Contains(
                "Expected 64-bin unsigned number, got:",
                exception.Message,
                StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(new char[]
    {
        '*', '1', '\r', '\r', // here we explicitly search for new line sequence
    })]
    [InlineData(new char[]
    {
        '*', '1', '\r', '\n',
        '+', '1', '\r', '\b', // here we explicitly search for new line sequence
    })]
    [InlineData(new char[]
    {
        '$', '2', '\r', '\n',
        '$', '1', '\n', '\n', // here is the end of line sequence read implicitly
        '$', '2', '\r',
    })]
    public async Task ReadAsync_InvalidEndOfLine_Throws(char[] invalidInput)
    {
        MemoryStream stream = new(invalidInput.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream, protocolVersion: RESPVersions.RESP2);

        FormatException exception = await Assert.ThrowsAsync<FormatException>(() =>
                reader.ReadAsync()).ConfigureAwait(false);
        Assert.Contains(
                "Unexpected character found while reading RESP new line, ",
                exception.Message,
                StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(new char[]
    {
        '*', '1', '\r',
    })]
    [InlineData(new char[]
    {
        '*', '1', '\r', '\n',
        '+', '1',
    })]
    [InlineData(new char[]
    {
        '$', '9', '\r', '\n',
        'f', 'b', '\n', '\n',
    })]
    [InlineData(new char[]
    {
        '+', '9', '\r',
    })]
    [InlineData(new char[]
    {
        '+', '9',
    })]
    [InlineData(new char[]
    {
        '+',
    })]
    [InlineData(new char[]
    {
        '$', '2', '\r', '\n',
        'a', 'b', '\r',
    })]
    [InlineData(new char[]
    {
        '$', '2', '\r', '\n',
        'b', 'b',
    })]
    [InlineData(new char[]
    {
        '$', '2', '\r', '\n',
        'c',
    })]
    [InlineData(new char[]
    {
        '$', '2', '\r', '\n',
    })]
    public async Task ReadAsync_UnexpectedEndOfStream_Throws(char[] invalidInput)
    {
        MemoryStream stream = new(invalidInput.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream, protocolVersion: RESPVersions.RESP2);

        IOException exception = await Assert.ThrowsAsync<IOException>(() =>
                reader.ReadAsync()).ConfigureAwait(false);
        Assert.Contains(
                "End of the stream reached while reading RESP",
                exception.Message,
                StringComparison.Ordinal);
    }

    /* -------------------- value by type read test -------------------- */

    [Theory]
    [InlineData(
        new char[] { '*', '0', '\r', '\n' },
        new string[] { })]
    [InlineData(
        new char[]
        {
            '*', '1', '\r', '\n',
            '+', '\r', '\n',
        },
        new string[] { "" })]
    [InlineData(
        new char[]
        {
            '*', '2', '\r', '\n',
            '+', 'f', '\r', '\n',
            '+', 'b', 'a', 'r', '\r', '\n',
        },
        new string[] { "f", "bar" })]
    public void Read_Array_Works(
            char[] payload,
            string[] expectedStrings)
    {
        MemoryStream stream = new(payload.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream, protocolVersion: RESPVersions.RESP2);
        RESPArray expected = new(expectedStrings.Select(s => new RESPSimpleString(s)));

        RESPValue actual = reader.Read();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(
        new char[] { '*', '0', '\r', '\n' },
        new string[] { })]
    [InlineData(
        new char[]
        {
            '*', '1', '\r', '\n',
            '+', '\r', '\n',
        },
        new string[] { "" })]
    [InlineData(
        new char[]
        {
            '*', '2', '\r', '\n',
            '+', 'f', '\r', '\n',
            '+', 'b', 'a', 'r', '\r', '\n',
        },
        new string[] { "f", "bar" })]
    public async Task ReadAsync_Array_Works(
            char[] payload,
            string[] expectedStrings)
    {
        MemoryStream stream = new(payload.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream, protocolVersion: RESPVersions.RESP2);
        RESPArray expected = new(expectedStrings.Select(s => new RESPSimpleString(s)));

        RESPValue actual = await reader.ReadAsync().ConfigureAwait(false);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(
        new char[]
        {
            '$', '0', '\r', '\n',
            '\r', '\n',
        },
        "")]
    [InlineData(
        new char[]
        {
            '$', '1', '\r', '\n',
            'f', '\r', '\n',
        },
        "f")]
    [InlineData(
        new char[]
        {
            '$', '7', '\r', '\n',
            'f', 'o', 'o', '\n', 'b', 'a', 'r', '\r', '\n',
        },
        "foo\nbar")]
    public void Read_BlobString_Works(
            char[] payload,
            string expectedString)
    {
        MemoryStream stream = new(payload.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream, protocolVersion: RESPVersions.RESP2);
        RESPBlobString expected = new(expectedString);

        RESPValue actual = reader.Read();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(
        new char[]
        {
            '$', '0', '\r', '\n',
            '\r', '\n',
        },
        "")]
    [InlineData(
        new char[]
        {
            '$', '1', '\r', '\n',
            'f', '\r', '\n',
        },
        "f")]
    [InlineData(
        new char[]
        {
            '$', '7', '\r', '\n',
            'f', 'o', 'o', '\n', 'b', 'a', 'r', '\r', '\n',
        },
        "foo\nbar")]
    public async Task ReadAsync_BlobString_Works(
            char[] payload,
            string expectedString)
    {
        MemoryStream stream = new(payload.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream);
        RESPBlobString expected = new(expectedString);

        RESPValue actual = await reader.ReadAsync().ConfigureAwait(false);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(new char[] { '$', '-', '1', '\r', '\n' })]
    [InlineData(new char[] { '*', '-', '1', '\r', '\n' })]
    public void Read_Null_Works(
            char[] payload)
    {
        MemoryStream stream = new(payload.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream, protocolVersion: RESPVersions.RESP2);
        RESPNull expected = RESPNull.Instance;

        RESPValue actual = reader.Read();

        Assert.Equal(expected, actual);
    }

    /* TODO: null in RESP3 */

    [Theory]
    [InlineData(new char[] { '$', '-', '1', '\r', '\n' })]
    [InlineData(new char[] { '*', '-', '1', '\r', '\n' })]
    public void Read_RESP2NullInRESP3_Throws(
            char[] payload)
    {
        MemoryStream stream = new(payload.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream);
        RESPNull expected = RESPNull.Instance;

        FormatException exception = Assert.Throws<FormatException>(() =>
                reader.Read());

        Assert.Contains(
               "Expected 64-bin unsigned number, got: -1. Note the null value serialization is changed in RESP3",
               exception.Message,
               StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(new char[] { '$', '-', '1', '\r', '\n' })]
    [InlineData(new char[] { '*', '-', '1', '\r', '\n' })]
    public async Task ReadAsync_RESP2NullInRESP3_Throws(
            char[] payload)
    {
        MemoryStream stream = new(payload.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream);
        RESPNull expected = RESPNull.Instance;

        FormatException exception = await Assert.ThrowsAsync<FormatException>(() =>
                reader.ReadAsync()).ConfigureAwait(false);

        Assert.Contains(
               "Expected 64-bin unsigned number, got: -1. Note the null value serialization is changed in RESP3",
               exception.Message,
               StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(
        new char[] { ':', '0', '\r', '\n' },
        0)]
    [InlineData(
        new char[] { ':', '1', '\r', '\n' },
        1)]
    [InlineData(
        new char[]
        {
            ':', '9', '2', '2', '3', '3', '7', '2', '0', '3', '6', '8', '5', '4', '7', '7', '5', '8', '0', '7', '\r', '\n',
        },
        long.MaxValue)]
    [InlineData(
        new char[]
        {
            ':', '-', '9', '2', '2', '3', '3', '7', '2', '0', '3', '6', '8', '5', '4', '7', '7', '5', '8', '0', '8', '\r', '\n',
        },
        long.MinValue)]
    public void Read_Number_Works(
            char[] payload,
            long number)
    {
        MemoryStream stream = new(payload.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream, protocolVersion: RESPVersions.RESP2);
        RESPNumber expected = new(number);

        RESPValue actual = reader.Read();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(
        new char[] { ':', '0', '\r', '\n' },
        0)]
    [InlineData(
        new char[] { ':', '1', '\r', '\n' },
        1)]
    [InlineData(
        new char[]
        {
            ':', '9', '2', '2', '3', '3', '7', '2', '0', '3', '6', '8', '5', '4', '7', '7', '5', '8', '0', '7', '\r', '\n',
        },
        long.MaxValue)]
    [InlineData(
        new char[]
        {
            ':', '-', '9', '2', '2', '3', '3', '7', '2', '0', '3', '6', '8', '5', '4', '7', '7', '5', '8', '0', '8', '\r', '\n',
        },
        long.MinValue)]
    public async Task ReadAsync_Number_Works(
            char[] payload,
            long number)
    {
        MemoryStream stream = new(payload.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream);
        RESPNumber expected = new(number);

        RESPValue actual = await reader.ReadAsync().ConfigureAwait(false);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(
        new char[] { '-', '\r', '\n' },
        "")]
    [InlineData(
        new char[] { '-', 'E', 'R', 'R', '\r', '\n' },
        "ERR")]
    [InlineData(
        new char[]
        {
            '-', 'E', 'R', 'R', ' ', 'f', 'o', 'o', ' ', 'b', 'a', 'r', '\r', '\n',
        },
        "ERR foo bar")]
    public void Read_SimpleError_Works(
            char[] payload,
            string expectedString)
    {
        MemoryStream stream = new(payload.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream, protocolVersion: RESPVersions.RESP2);
        RESPSimpleError expected = new(expectedString);

        RESPValue actual = reader.Read();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(
        new char[] { '-', '\r', '\n' },
        "")]
    [InlineData(
        new char[] { '-', 'E', 'R', 'R', '\r', '\n' },
        "ERR")]
    [InlineData(
        new char[]
        {
            '-', 'E', 'R', 'R', ' ', 'f', 'o', 'o', ' ', 'b', 'a', 'r', '\r', '\n',
        },
        "ERR foo bar")]
    public async Task ReadAsync_SimpleError_Works(
            char[] payload,
            string expectedString)
    {
        MemoryStream stream = new(payload.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream);
        RESPSimpleError expected = new(expectedString);

        RESPValue actual = await reader.ReadAsync().ConfigureAwait(false);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(
        new char[] { '+', '\r', '\n' },
        "")]
    [InlineData(
        new char[] { '+', 'O', 'K', '\r', '\n' },
        "OK")]
    [InlineData(
        new char[]
        {
            '+', 'f', 'o', 'o', ' ', 'b', 'a', 'r', '\r', '\n',
        },
        "foo bar")]
    public void Read_SimpleString_Works(
            char[] payload,
            string expectedString)
    {
        MemoryStream stream = new(payload.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream, protocolVersion: RESPVersions.RESP2);
        RESPSimpleString expected = new(expectedString);

        RESPValue actual = reader.Read();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(
        new char[] { '+', '\r', '\n', },
        "")]
    [InlineData(
        new char[] { '+', 'O', 'K', '\r', '\n' },
        "OK")]
    [InlineData(
        new char[]
        {
            '+', 'f', 'o', 'o', ' ', 'b', 'a', 'r', '\r', '\n',
        },
        "foo bar")]
    public async Task ReadAsync_SimpleString_Works(
            char[] payload,
            string expectedString)
    {
        MemoryStream stream = new(payload.Select(ch => (byte)ch).ToArray());
        using RESPReader reader = new(stream);
        RESPSimpleString expected = new(expectedString);

        RESPValue actual = await reader.ReadAsync().ConfigureAwait(false);

        Assert.Equal(expected, actual);
    }
}
