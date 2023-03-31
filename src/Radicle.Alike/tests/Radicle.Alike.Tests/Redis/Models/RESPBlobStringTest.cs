namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections.Generic;
using Xunit;

public class RESPBlobStringTest
{
    [Fact]
    public void Constructor_NullStringValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RESPBlobString((string)null!));
    }

    [Fact]
    public void Constructor_NullValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RESPBlobString((IEnumerable<byte>)null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("The quick brown fox jumps over the lazy dog")]
    public void Equals_Equal_ReturnsTrue(string str)
    {
        RESPBlobString one = new(str);
        RESPBlobString other = new(str);

        Assert.Equal(one.GetHashCode(), other.GetHashCode());
        Assert.True(one == other);
        Assert.False(one != other);
    }

    [Theory]
    [InlineData("")]
    [InlineData("The quick brown fox jumps over the lazy dog")]
    public void Equals_EqualWithAttribute_ReturnsTrue(string str)
    {
        RESPBlobString one = new(str)
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };
        RESPBlobString other = new(str)
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };

        Assert.Equal(one.GetHashCode(), other.GetHashCode());
        Assert.True(one == other);
        Assert.False(one != other);
    }

    [Theory]
    [InlineData(new byte[] { })]
    [InlineData(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 })]
    [InlineData(new byte[] { 92, 2, 42, 5, 46, 8, 6, 86, 84 })]
    [InlineData(new byte[] { 255 })]
    public void Equals_EqualBinary_ReturnsTrue(byte[] value)
    {
        RESPBlobString one = new(value);
        RESPBlobString other = new(value);

        Assert.Equal(one.GetHashCode(), other.GetHashCode());
        Assert.True(one == other);
        Assert.False(one != other);
    }

    [Theory]
    [InlineData("", " ")]
    [InlineData("A", "a")]
    [InlineData("AE", "\u00C6")] // Æ - LATIN CAPITAL LETTER AE
    [InlineData("The quick brown fox jumps over the lazy dog", "The slow brown fox jumps over the lazy dog")]
    public void Equals_NonEqualStrings_ReturnsFalse(string first, string second)
    {
        RESPBlobString one = new(first);
        RESPBlobString other = new(second);

        Assert.True(one != other);
        Assert.False(one == other);
    }

    [Theory]
    [InlineData(new byte[] { }, new byte[] { 0 })]
    [InlineData(new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 4 })]
    [InlineData(new byte[] { 0 }, new byte[] { 255 })]
    [InlineData(new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3, 4 })]
    public void Equals_NonEqual_ReturnsFalse(byte[] first, byte[] second)
    {
        RESPBlobString one = new(first);
        RESPBlobString other = new(second);

        Assert.True(one != other);
        Assert.False(one == other);
    }

    [Fact]
    public void Equals_AttribsDifferent_ReturnsFalse()
    {
        RESPBlobString one = new(string.Empty)
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };

        RESPBlobString other = new(string.Empty)
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
            }),
        };

        Assert.True(one != other);
        Assert.False(one == other);
    }
}
