namespace Radicle.Common.Compression;

using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

public class GzipTest
{
    public static IEnumerable<object[]> TooShortPayloads =>
            new List<object[]>()
    {
        new object[]
        {
            Array.Empty<byte>(),
        },
        new object[]
        {
            new byte[] { 0x00 },
        },
        new object[]
        {
            new byte[] { 0xff },
        },
        new object[]
        {
            new byte[] { 0x25 },
        },
        new object[]
        {
            new byte[] { 0xa1 },
        },
        new object[]
        {
            new byte[]
            {
                0xa1, 0x6d,
            },
        },
        new object[]
        {
            new byte[]
            {
                0xa1, 0x6d, 0xf9,
            },
        },
        new object[]
        {
            new byte[]
            {
                0xa1, 0x6d, 0xf9, 0x84,
            },
        },
        new object[]
        {
            new byte[]
            {
                0xa1, 0x6d, 0xf9, 0x84,
                0x9f,
            },
        },
        new object[]
        {
            new byte[]
            {
                0x7d, 0x93, 0x8d, 0xf3,
                0x2e, 0x3a,
            },
        },
        new object[]
        {
            new byte[]
            {
                0x7d, 0x93, 0x8d, 0xf3,
                0x2e, 0x3a, 0xf5,
            },
        },
        new object[]
        {
            new byte[]
            {
                0x7d, 0x93, 0x8d, 0xf3,
                0x2e, 0x3a, 0xf5, 0xf8,
            },
        },
        new object[]
        {
            new byte[]
            {
                0xa1, 0x6d, 0xf9, 0x84,
                0x9f, 0x75, 0x45, 0x83,
                0x4e,
            },
        },
    };

    public static IEnumerable<object[]> CorruptPayloads =>
            new List<object[]>()
    {
        new object[]
        {
            new byte[]
            {
                0x7d, 0x93, 0x8d, 0xf3,
                0x2e, 0x3a, 0xf5, 0xf8,
                0xca, 0xbc,
            },
        },
        new object[]
        {
            new byte[]
            {
                0xa1, 0x6d, 0xf9, 0x84,
                0x9f, 0x75, 0x45, 0x83,
                0x4e, 0xba, 0x2a,
            },
        },
        new object[]
        {
            new byte[]
            {
                0xa1, 0x6d, 0xf9, 0x84,
                0x9f, 0x75, 0x45, 0x83,
                0x4e, 0xba, 0x2a, 0xdf,
            },
        },
        new object[]
        {
            new byte[]
            {
                0x7d, 0x93, 0x8d, 0xf3,
                0x2e, 0x3a, 0xf5, 0xf8,
                0xca, 0xbc, 0x50, 0x49,
                0x5f, 0xb4, 0xf4, 0x14,
            },
        },
        new object[]
        {
            new byte[]
            {
                0xa1, 0x6d, 0xf9, 0x84,
                0x9f, 0x75, 0x45, 0x83,
                0x4e, 0xba, 0x2a, 0xdf,
                0x7b, 0x52, 0x45, 0x34,
                0x95, 0x4a, 0x85, 0x76,
                0x81, 0x0c, 0x64, 0x29,
                0x48, 0x6b, 0xcd, 0x13,
                0x2a, 0x36, 0x6e, 0xa8,
            },
        },
    };

    [Fact]
    public void Compress_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Gzip.Compress(null!));
    }

    [Fact]
    public void Compress_NullStringInput_Thsows()
    {
        Assert.Throws<ArgumentNullException>(() => Gzip.Compress((string)null!));
    }

    [Fact]
    public void Uncompress_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Gzip.Uncompress(null!));
    }

    [Fact]
    public void UncompressString_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Gzip.UncompressString(null!));
    }

    [Fact]
    public void Compress_SimpleString_ReturnsCompressedPayload()
    {
        string simpleString = new('0', 863);

        Assert.True(Gzip.Compress(simpleString).Length < simpleString.Length);
    }

    [Fact]
    public void Compress_SimpleInput_ReturnsCompressedPayload()
    {
        byte[] simpleBytes = new byte[863];

        Assert.True(Gzip.Compress(simpleBytes).Length < simpleBytes.Length);
    }

    [Fact]
    public void Compress_String_RoundtripWorks()
    {
        const string value = "The quick brown fox jumps over the lazy dog";

        Assert.Equal(value, Gzip.UncompressString(Gzip.Compress(value)));
    }

    [Fact]
    public void Compress_Binary_RoundtripWorks()
    {
        byte[] value = new byte[] { 0, 1, 2, 3, 4 };

        Assert.Equal(value, Gzip.Uncompress(Gzip.Compress(value)));
    }

    [Fact]
    public void Compress_EmptyInput_Works()
    {
        byte[] payload = Gzip.Compress(Array.Empty<byte>());

        Assert.NotEmpty(payload);
        Assert.True(payload.Length > 18); // 18B = (10B) headers + (4B) CRC32 + (4B) ISIZE
    }

    [Theory]
    [MemberData(nameof(CorruptPayloads))]
    public void Uncompress_CorruptPayload_Throws(byte[] corruptedData)
    {
        Assert.Throws<ArgumentException>(() => Gzip.Uncompress(corruptedData));
    }

    [Theory]
    [MemberData(nameof(TooShortPayloads))]
    public void Uncompress_ShortPayload_Throws(byte[] corruptedData)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Gzip.Uncompress(corruptedData));
    }

    [Theory]
    [MemberData(nameof(CorruptPayloads))]
    public void UncompressString_CorruptPayload_Throws(byte[] corruptedData)
    {
        Assert.Throws<ArgumentException>(() => Gzip.Uncompress(corruptedData));
    }

    [Theory]
    [MemberData(nameof(TooShortPayloads))]
    public void UncompressString_ShortPayload_Throws(byte[] corruptedData)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Gzip.Uncompress(corruptedData));
    }

    [Fact]
    public void UncompressString_CorruptUTF8Payload_Throws()
    {
        Encoding enc = new UTF8Encoding(true, true);

        // see https://www.cl.cam.ac.uk/~mgk25/ucs/examples/UTF-8-test.txt
        byte[] corruptUTF8 = new byte[]
        {
            0xfe, 0xfe, 0xff, 0xff,
        };

        Assert.Throws<DecoderFallbackException>(() =>
                Gzip.UncompressString(Gzip.Compress(corruptUTF8), encoding: enc));
    }
}
