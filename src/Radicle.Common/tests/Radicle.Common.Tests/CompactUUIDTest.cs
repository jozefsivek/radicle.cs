namespace Radicle.Common;

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

public class CompactUUIDTest
{
    [Fact]
    public void GenerateRawByteArray_ReturnsV4GUID()
    {
        for (int i = 0; i < 24; i++)
        {
            // xxxxxxxx-xxxx-Mxxx-Nxxx-xxxxxxxxxxxx
            // assert M = 0100 == 0x4
            Assert.Equal(0b_0100_0000, 0b_1111_0000 & CompactUUID.GenerateRawByteArray()[6]);
        }
    }

    [Fact]
    public void GenerateRawByteArray_ReturnsVariant1GUID()
    {
        for (int i = 0; i < 24; i++)
        {
            // xxxxxxxx-xxxx-Mxxx-Nxxx-xxxxxxxxxxxx
            // assert N = 10xx = 0x8 to 0xb
            Assert.Equal(0b_1000_0000, 0b_1100_0000 & CompactUUID.GenerateRawByteArray()[8]);
        }
    }

    [Fact]
    public void GenerateRawByteArray_Returns16Bytes()
    {
        Assert.Equal(16, CompactUUID.GenerateRawByteArray().Length);
    }

    [Fact]
    public void GenerateRawByteArray_returns_16_random_bytes()
    {
        decimal average = 0;
        const int N = 8;

        for (int i = 0; i < N; i++)
        {
            average += CompactUUID.GenerateRawByteArray().Sum(b => b);
        }

        Assert.InRange(average / (N * 16), 32, 256 - 32);
    }

    [Fact]
    public void Generate_Returns22Characters()
    {
        Assert.Equal(22, CompactUUID.Generate().Length);
    }

    [Fact]
    public void Generate_ReturnsBase64Characters()
    {
        Assert.Matches(new Regex(@"\A[a-zA-Z0-9\+\/]+\z"), CompactUUID.GenerateWebSafe());
    }

    [Fact]
    public void GenerateWebSafe_Returns22Characters()
    {
        Assert.Equal(22, CompactUUID.Generate().Length);
    }

    [Fact]
    public void GenerateWebSafe_ReturnsBase64Characters()
    {
        Assert.Matches(new Regex(@"\A[a-zA-Z0-9-_]+\z"), CompactUUID.GenerateWebSafe());
    }

    [Fact]
    public void ShuffleFromToMSMixedEndianWith_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                CompactUUID.ShuffleFromToMSMixedEndianWith(null!));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(17)]
    [InlineData(42)]
    public void ShuffleFromToMSMixedEndianWith_IncorrectSizedInput_Throws(int size)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
                () => CompactUUID.ShuffleFromToMSMixedEndianWith(new byte[size]));
    }

    [Fact]
    public void ShuffleFromToMSMixedEndianWith_ValidInput_Works()
    {
        byte[] original = Enumerable.Range(0, 16).Select(i => (byte)i).ToArray();
        byte[] shuffled = new byte[16];
        Array.Copy(original, shuffled, 16);
        CompactUUID.ShuffleFromToMSMixedEndianWith(shuffled);

        Assert.Equal(3, shuffled[0]);
        Assert.Equal(2, shuffled[1]);
        Assert.Equal(1, shuffled[2]);
        Assert.Equal(0, shuffled[3]);

        Assert.Equal(5, shuffled[4]);
        Assert.Equal(4, shuffled[5]);

        Assert.Equal(7, shuffled[6]);
        Assert.Equal(6, shuffled[7]);

        for (int i = 8; i < 16; i++)
        {
            Assert.Equal(i, shuffled[i]);
        }
    }
}
