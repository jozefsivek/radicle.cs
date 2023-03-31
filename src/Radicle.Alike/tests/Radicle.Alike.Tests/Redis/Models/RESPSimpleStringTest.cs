namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections.Generic;
using Xunit;

public class RESPSimpleStringTest
{
    [Fact]
    public void Constructor_NullStringValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RESPSimpleString((string)null!));
    }

    [Fact]
    public void Constructor_NullValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RESPSimpleString((IEnumerable<byte>)null!));
    }

    [Theory]
    [InlineData("foo\n")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("\n\r")]
    [InlineData("\r")]
    public void Constructor_StringWithNewLines_Throws(string srt)
    {
        Assert.Throws<ArgumentException>(() => new RESPSimpleString(srt));
    }

    [Theory]
    [InlineData(new byte[] { 0, 1, (byte)'\n' })]
    [InlineData(new byte[] { (byte)'\n' })]
    [InlineData(new byte[] { (byte)'\r', (byte)'\n' })]
    [InlineData(new byte[] { (byte)'\n', (byte)'\r' })]
    [InlineData(new byte[] { (byte)'\r' })]
    public void Constructor_ValueWithNewLines_Throws(byte[] value)
    {
        Assert.Throws<ArgumentException>(() => new RESPSimpleString(value));
    }

    [Theory]
    [InlineData("")]
    [InlineData("The quick brown fox jumps over the lazy dog")]
    public void Equals_Equal_ReturnsTrue(string str)
    {
        RESPSimpleString one = new(str);
        RESPSimpleString other = new(str);

        Assert.Equal(one.GetHashCode(), other.GetHashCode());
        Assert.True(one == other);
        Assert.False(one != other);
    }

    [Theory]
    [InlineData("")]
    [InlineData("The quick brown fox jumps over the lazy dog")]
    public void Equals_EqualWithAttribute_ReturnsTrue(string str)
    {
        RESPSimpleString one = new(str)
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };
        RESPSimpleString other = new(str)
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
    [InlineData(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 12, 14, 15, 16, 17, 18 })]
    [InlineData(new byte[] { 92, 2, 42, 5, 46, 8, 6, 86, 84 })]
    [InlineData(new byte[] { 255 })]
    public void Equals_EqualBinary_ReturnsTrue(byte[] value)
    {
        RESPSimpleString one = new(value);
        RESPSimpleString other = new(value);

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
        RESPSimpleString one = new(first);
        RESPSimpleString other = new(second);

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
        RESPSimpleString one = new(first);
        RESPSimpleString other = new(second);

        Assert.True(one != other);
        Assert.False(one == other);
    }

    [Fact]
    public void Equals_AttribsDifferent_ReturnsFalse()
    {
        RESPSimpleString one = new(string.Empty)
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };

        RESPSimpleString other = new(string.Empty)
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
