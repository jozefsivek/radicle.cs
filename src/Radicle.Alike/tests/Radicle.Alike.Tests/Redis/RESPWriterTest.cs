namespace Radicle.Alike.Redis;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Radicle.Alike.Redis.Models;
using Xunit;

public class RESPWriterTest
{
    [Fact]
    public void Constructor_NullStream_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RESPWriter(null!));
    }

    [Fact]
    public void Constructor_NonWritableStream_Throws()
    {
        using MemoryStream nonWritableStream = new(
                Array.Empty<byte>(),
                writable: false);

        Assert.Throws<ArgumentException>(() => new RESPWriter(nonWritableStream));
    }

    [Fact]
    public async Task Dispose_NotOpenInnerStream_Disposes()
    {
        MemoryStream stream = new();
        RESPWriter writer;

        using (writer = new RESPWriter(stream))
        {
            // pass
        }

        Assert.Throws<ObjectDisposedException>(() =>
                writer.Write(RESPNull.Instance));
        await Assert.ThrowsAsync<ObjectDisposedException>(() =>
                writer.WriteAsync(RESPNull.Instance).AsTask()).ConfigureAwait(false);
        Assert.Throws<ObjectDisposedException>(() =>
                stream.Write(new byte[] { 0x00 }, 0, 1));
    }

    [Fact]
    public async Task Dispose_OpenInnerStream_DoesNotDisposes()
    {
        using MemoryStream stream = new(1);
        RESPWriter writer;

        using (writer = new RESPWriter(stream, leaveOpen: true))
        {
            // pass
        }

        Assert.Throws<ObjectDisposedException>(() =>
            writer.Write(RESPNull.Instance));
        await Assert.ThrowsAsync<ObjectDisposedException>(() =>
            writer.WriteAsync(RESPNull.Instance).AsTask()).ConfigureAwait(false);
        await stream.WriteAsync((new byte[] { 0x00 }).AsMemory(0, 1)).ConfigureAwait(false);
    }

    [Fact]
    public void Write_NullInput_Throws()
    {
        using RESPWriter writer = new(new MemoryStream());

        Assert.Throws<ArgumentNullException>(() => writer.Write(null!));
    }

    [Fact]
    public async Task WriteAsync_NullInput_Throws()
    {
        using RESPWriter writer = new(new MemoryStream());

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
                writer.WriteAsync(null!).AsTask()).ConfigureAwait(false);
    }

    [Fact]
    public void Write_RESP3ValueInRESP2Mode_Throws()
    {
        using RESPWriter writer = new(new MemoryStream(), protocolVersion: RESPVersions.RESP2);

        Assert.Throws<InvalidOperationException>(() => writer.Write(RESPAttributeValue.Empty));
        Assert.Throws<InvalidOperationException>(() => writer.Write(new RESPBigNumber(0)));
        Assert.Throws<InvalidOperationException>(() => writer.Write(new RESPBlobError(string.Empty)));
        Assert.Throws<InvalidOperationException>(() => writer.Write(RESPBoolean.True));
        Assert.Throws<InvalidOperationException>(() => writer.Write(new RESPDouble(0.0)));
        Assert.Throws<InvalidOperationException>(() => writer.Write(RESPMap.Empty));
        Assert.Throws<InvalidOperationException>(() =>
                writer.Write(new RESPPush(new RESPValue[] { new RESPSimpleString("pubsub") })));
        Assert.Throws<InvalidOperationException>(() => writer.Write(RESPSet.Empty));
        Assert.Throws<InvalidOperationException>(() =>
                writer.Write(new RESPVerbatimString(VerbatimStringType.Text, string.Empty)));
    }

    [Fact]
    public async Task WriteAsync_RESP3ValueInRESP2Mode_Throws()
    {
        using RESPWriter writer = new(new MemoryStream(), protocolVersion: RESPVersions.RESP2);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
                writer.WriteAsync(RESPAttributeValue.Empty).AsTask()).ConfigureAwait(false);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
                writer.WriteAsync(new RESPBigNumber(0)).AsTask()).ConfigureAwait(false);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
                writer.WriteAsync(new RESPBlobError(string.Empty)).AsTask()).ConfigureAwait(false);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
                writer.WriteAsync(RESPBoolean.True).AsTask()).ConfigureAwait(false);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
                writer.WriteAsync(new RESPDouble(0.0)).AsTask()).ConfigureAwait(false);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
                writer.WriteAsync(RESPMap.Empty).AsTask()).ConfigureAwait(false);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
                writer.WriteAsync(new RESPPush(new RESPValue[] { new RESPSimpleString("pubsub") })).AsTask()).ConfigureAwait(false);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
                writer.WriteAsync(RESPSet.Empty).AsTask()).ConfigureAwait(false);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
                writer.WriteAsync(new RESPVerbatimString(VerbatimStringType.Text, string.Empty)).AsTask()).ConfigureAwait(false);
    }

    [Theory]
    [InlineData(
        new char[]
        {
            '*', '0', '\r', '\n',
        },
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
    public void Write_Array_Works(
            char[] expectedCharacters,
            string[] strs)
    {
        MemoryStream stream = new(expectedCharacters.Length);
        using RESPWriter writer = new(stream, protocolVersion: RESPVersions.RESP2);
        IEnumerable<byte> expected = expectedCharacters.Select(ch => (byte)ch);

        writer.Write(new RESPArray(strs.Select(s => new RESPSimpleString(s))));

        Assert.Equal(expected, stream.ToArray());
    }

    [Theory]
    [InlineData(
        new char[]
        {
            '*', '0', '\r', '\n',
        },
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
    public async Task WriteAsync_Array_Works(
            char[] expectedCharacters,
            string[] strs)
    {
        MemoryStream stream = new(expectedCharacters.Length);
        using RESPWriter writer = new(stream);
        IEnumerable<byte> expected = expectedCharacters.Select(ch => (byte)ch);

        await writer.WriteAsync(new RESPArray(strs.Select(s => new RESPSimpleString(s))))
                .ConfigureAwait(false);

        Assert.Equal(expected, stream.ToArray());
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
    public void Write_BlobString_Works(
            char[] expectedCharacters,
            string str)
    {
        MemoryStream stream = new(expectedCharacters.Length);
        using RESPWriter writer = new(stream, protocolVersion: RESPVersions.RESP2);
        IEnumerable<byte> expected = expectedCharacters.Select(ch => (byte)ch);

        writer.Write(new RESPBlobString(str));

        Assert.Equal(expected, stream.ToArray());
    }

    [Theory]
    [InlineData(
        new byte[]
        {
            (byte)'$', (byte)'0', 13, 10,
            13, 10,
        },
        new byte[] { })]
    [InlineData(
        new byte[]
        {
            (byte)'$', (byte)'1', 13, 10,
            0, 13, 10,
        },
        new byte[] { 0 })]
    [InlineData(
        new byte[]
        {
            (byte)'$', (byte)'6', 13, 10,
            0, 1, 2, 3, 4, 255, 13, 10,
        },
        new byte[] { 0, 1, 2, 3, 4, 255 })]
    public void WriteAsync_BlobString_Works(
            byte[] expected,
            byte[] bytes)
    {
        MemoryStream stream = new(expected.Length);
        using RESPWriter writer = new(stream);

        writer.Write(new RESPBlobString(bytes));

        Assert.Equal(expected, stream.ToArray());
    }

    [Theory]
    [InlineData(
        new char[]
        {
            '$', '-', '1', '\r', '\n',
        })]
    public void Write_Null_Works(
            char[] expectedCharacters)
    {
        MemoryStream stream = new(expectedCharacters.Length);
        using RESPWriter writer = new(stream, protocolVersion: RESPVersions.RESP2);
        IEnumerable<byte> expected = expectedCharacters.Select(ch => (byte)ch);

        writer.Write(RESPNull.Instance);

        Assert.Equal(expected, stream.ToArray());
    }

    /* TODO: null in RESP3 */

    [Theory]
    [InlineData(
        new char[]
        {
            ':', '0', '\r', '\n',
        },
        0)]
    [InlineData(
        new char[]
        {
            ':', '1', '\r', '\n',
        },
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
    public void Write_Number_Works(
            char[] expectedCharacters,
            long number)
    {
        MemoryStream stream = new(expectedCharacters.Length);
        using RESPWriter writer = new(stream, protocolVersion: RESPVersions.RESP2);
        IEnumerable<byte> expected = expectedCharacters.Select(ch => (byte)ch);

        writer.Write(new RESPNumber(number));

        Assert.Equal(expected, stream.ToArray());
    }

    [Theory]
    [InlineData(
        new char[]
        {
            ':', '0', '\r', '\n',
        },
        0)]
    [InlineData(
        new char[]
        {
            ':', '1', '\r', '\n',
        },
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
    public async Task WriteAsync_Number_Works(
            char[] expectedCharacters,
            long number)
    {
        MemoryStream stream = new(expectedCharacters.Length);
        using RESPWriter writer = new(stream);
        IEnumerable<byte> expected = expectedCharacters.Select(ch => (byte)ch);

        await writer.WriteAsync(new RESPNumber(number)).ConfigureAwait(false);

        Assert.Equal(expected, stream.ToArray());
    }

    [Theory]
    [InlineData(
        new char[]
        {
            '-', '\r', '\n',
        },
        "")]
    [InlineData(
        new char[]
        {
            '-', 'E', 'R', 'R', '\r', '\n',
        },
        "ERR")]
    [InlineData(
        new char[]
        {
            '-', 'E', 'R', 'R', ' ', 'f', 'o', 'o', ' ', 'b', 'a', 'r', '\r', '\n',
        },
        "ERR foo bar")]
    public void Write_SimpleError_Works(
            char[] expectedCharacters,
            string str)
    {
        MemoryStream stream = new(expectedCharacters.Length);
        using RESPWriter writer = new(stream, protocolVersion: RESPVersions.RESP2);
        IEnumerable<byte> expected = expectedCharacters.Select(ch => (byte)ch);

        writer.Write(new RESPSimpleError(str));

        Assert.Equal(expected, stream.ToArray());
    }

    [Theory]
    [InlineData(
        new byte[]
        {
            (byte)'-', 13, 10,
        },
        new byte[] { })]
    [InlineData(
        new byte[]
        {
            (byte)'-', 0, 13, 10,
        },
        new byte[] { 0 })]
    [InlineData(
        new byte[]
        {
            (byte)'-', 0, 1, 2, 3, 4, 255, 13, 10,
        },
        new byte[] { 0, 1, 2, 3, 4, 255 })]
    public void WriteAsync_SimpleError_Works(
            byte[] expected,
            byte[] bytes)
    {
        MemoryStream stream = new(expected.Length);
        using RESPWriter writer = new(stream);

        writer.Write(new RESPSimpleError(bytes));

        Assert.Equal(expected, stream.ToArray());
    }

    [Theory]
    [InlineData(
        new char[]
        {
            '+', '\r', '\n',
        },
        "")]
    [InlineData(
        new char[]
        {
            '+', 'O', 'K', '\r', '\n',
        },
        "OK")]
    [InlineData(
        new char[]
        {
            '+', 'f', 'o', 'o', ' ', 'b', 'a', 'r', '\r', '\n',
        },
        "foo bar")]
    public void Write_SimpleString_Works(
            char[] expectedCharacters,
            string str)
    {
        MemoryStream stream = new(expectedCharacters.Length);
        using RESPWriter writer = new(stream, protocolVersion: RESPVersions.RESP2);
        IEnumerable<byte> expected = expectedCharacters.Select(ch => (byte)ch);

        writer.Write(new RESPSimpleString(str));

        Assert.Equal(expected, stream.ToArray());
    }

    [Theory]
    [InlineData(
        new byte[]
        {
            (byte)'+', 13, 10,
        },
        new byte[] { })]
    [InlineData(
        new byte[]
        {
            (byte)'+', 0, 13, 10,
        },
        new byte[] { 0 })]
    [InlineData(
        new byte[]
        {
            (byte)'+', 0, 1, 2, 3, 4, 255, 13, 10,
        },
        new byte[] { 0, 1, 2, 3, 4, 255 })]
    public void WriteAsync_SimpleString_Works(
            byte[] expected,
            byte[] bytes)
    {
        MemoryStream stream = new(expected.Length);
        using RESPWriter writer = new(stream);

        writer.Write(new RESPSimpleString(bytes));

        Assert.Equal(expected, stream.ToArray());
    }
}
