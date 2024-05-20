namespace Radicle.Common.Compression;

using System;
using System.Linq;
using Xunit;

public class VLQTest
{
    private readonly byte[] wrapAroundZeroPayload = new byte[]
    {
        0b_1100_0000,
        0b_1000_0000,
        0b_1000_0000,
        0b_1000_0000,

        0b_1000_0000,
        0b_1000_0000,
        0b_1000_0000,
        0b_1000_0000,

        0b_1000_0000,
        0b_1000_0000,

        0b_0000_0000,
    };

    public static TheoryData<byte[]> CorruptPayloads => new()
    {
        { Array.Empty<byte>() },
        /* truncated payloads */
        { new byte[] { 0b_10000001 } },
        { new byte[] { 0b_10111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111 } },
    };

    public static TheoryData<ulong, byte[]> TestPayloads => new()
    {
        { 0UL, new byte[] { 0b_0000_0000 } },
        { 1UL, new byte[] { 0b_0000_0001 } },
        { 43UL, new byte[] { 0b_0010_1011 } },
        { 127UL, new byte[] { 0b_0111_1111 } },
        { 128UL, new byte[] { 0b_10000001, 0b_00000000 } },
        { 8192UL, new byte[] { 0b_11000000, 0b_00000000 } },
        { 16383UL, new byte[] { 0b_11111111, 0b_01111111 } },
        { 16384UL, new byte[] { 0b_1000_0001, 0b_10000000, 0b_00000000 } },
        { 2097150UL, new byte[] { 0b_11111111, 0b_11111111, 0b_01111110 } },
        { 2097153UL, new byte[] { 0b_10000001, 0b_10000000, 0b_10000000, 0b_00000001 } },
        { 134217728UL, new byte[] { 0b_11000000, 0b_10000000, 0b_10000000, 0b_00000000 } },
        { 268435454UL, new byte[] { 0b_11111111, 0b_11111111, 0b_11111111, 0b_01111110 } },
        { ulong.MaxValue, new byte[] { 0b_10000001, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_01111111 } },
    };

    public static TheoryData<ulong, byte[]> TestPayloadsWithSuffix => new()
    {
        { 0UL, new byte[] { 0b_0000_0000, 0b_0000_0000 } },
        { 16383UL, new byte[] { 0b_11111111, 0b_01111111, 0b_11111111 } },
        { ulong.MaxValue, new byte[] { 0b_10111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_01111111, 0b_10111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_01111111 } },
    };

    [Theory]
    [MemberData(nameof(TestPayloads))]
    public void GetEncodePayloadLength_ValidInput_Works(
            ulong value,
            byte[] expected)
    {
        Assert.Equal(expected.Length, VLQ.GetEncodePayloadLength(value));
    }

    [Fact]
    public void Encode_NullDestination_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                VLQ.EncodeTo(null!, 0uL));
    }

    [Fact]
    public void Encode_NegativeIndex_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                VLQ.EncodeTo(Array.Empty<byte>(), 0uL, destinationIndex: -1));
    }

    [Fact]
    public void Encode_TooLargeIndex_Throws()
    {
        Assert.Throws<IndexOutOfRangeException>(() =>
                VLQ.EncodeTo(new byte[1], 0uL, destinationIndex: 10));
    }

    [Fact]
    public void Encode_ShortDestination_Throws()
    {
        Assert.Throws<IndexOutOfRangeException>(() =>
                VLQ.EncodeTo(new byte[1], ulong.MaxValue, destinationIndex: 0));
    }

    [Theory]
    [MemberData(nameof(TestPayloads))]
    public void Encode_ValidInput_Works(
            ulong value,
            byte[] expected)
    {
        Assert.Equal(expected, VLQ.Encode(value).ToArray());
    }

    [Theory]
    [MemberData(nameof(TestPayloads))]
    public void EncodeTo_ValidInput_Works(
            ulong value,
            byte[] expected)
    {
        byte[] destination = new byte[32];

        int followupIndex = VLQ.EncodeTo(destination, value);

        Assert.Equal(VLQ.GetEncodePayloadLength(value), followupIndex);

        byte[] expectedDest = new byte[destination.Length];
        expected.CopyTo(expectedDest, 0);

        Assert.Equal(expectedDest, destination);
    }

    [Theory]
    [MemberData(nameof(TestPayloads))]
    public void EncodeTo_ValidInputNonZeroIndex_Works(
            ulong value,
            byte[] expected)
    {
        byte[] destination = new byte[32];
        const int index = 3;
        int destinationIndex = index;

        destinationIndex = VLQ.EncodeTo(
                destination,
                value,
                destinationIndex: destinationIndex);

        Assert.Equal(VLQ.GetEncodePayloadLength(value) + index, destinationIndex);

        byte[] expectedDest = new byte[destination.Length];
        expected.CopyTo(expectedDest, index);

        Assert.Equal(expectedDest, destination);
    }

    [Fact]
    public void DecodeFirstFrom_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => VLQ.DecodeFirstFrom(null!));
    }

    [Theory]
    [MemberData(nameof(CorruptPayloads))]
    public void DecodeFirstFrom_CorruptInput_Throws(
            byte[] corruptInput)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => VLQ.DecodeFirstFrom(corruptInput));
    }

    [Theory]
    [MemberData(nameof(TestPayloads))]
    public void DecodeFirstFrom_ValidInput_Works(
            ulong expected,
            byte[] payload)
    {
        Assert.Equal(expected, VLQ.DecodeFirstFrom(payload));
    }

    [Fact]
    public void DecodeFirstFrom_TooLargeInput_Wraps()
    {
        Assert.Equal(0uL, VLQ.DecodeFirstFrom(this.wrapAroundZeroPayload));
    }

    [Theory]
    [MemberData(nameof(TestPayloadsWithSuffix))]
    public void DecodeFirstFrom_InputWithSuffix_Works(
            ulong expected,
            byte[] payload)
    {
        Assert.Equal(expected, VLQ.DecodeFirstFrom(payload));
    }

    [Fact]
    public void Decode_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => VLQ.Decode(null!));
    }

    [Theory]
    [MemberData(nameof(CorruptPayloads))]
    public void Decode_CorruptInput_Throws(
            byte[] corruptInput)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => VLQ.Decode(corruptInput));
    }

    [Theory]
    [MemberData(nameof(TestPayloadsWithSuffix))]
    public void Decode_InputWithSuffix_Throws(
            ulong expected,
            byte[] payload)
    {
        _ = expected;

        Assert.Throws<ArgumentException>(() => VLQ.Decode(payload));
    }

    [Theory]
    [MemberData(nameof(TestPayloads))]
    public void Decode_ValidInput_Throws(
            ulong expected,
            byte[] payload)
    {
        Assert.Equal(expected, VLQ.Decode(payload));
    }

    [Fact]
    public void Decode_TooLargeInput_Wraps()
    {
        Assert.Equal(0uL, VLQ.Decode(this.wrapAroundZeroPayload));
    }

    [Fact]
    public void ReadNext_ContinuationByte_ReturnsTrue()
    {
        for (byte b = 0b_1000_0000; b is <= byte.MaxValue and > byte.MinValue; b++)
        {
            Assert.True(VLQ.DecodeReadNext(b, out ulong v));
            Assert.Equal((ulong)(b & 0b_0111_1111), v);
        }
    }

    [Fact]
    public void ReadNext_TerminationByte_ReturnsFalse()
    {
        for (byte b = byte.MinValue; b < 0b_1000_0000; b++)
        {
            Assert.False(VLQ.DecodeReadNext(b, out ulong v));
            Assert.Equal((ulong)(b & 0b_0111_1111), v);
        }
    }
}
