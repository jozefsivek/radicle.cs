namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections.Generic;
using Xunit;

public class RESPSimpleErrorTest
{
    [Fact]
    public void Constructor_NullStringValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RESPSimpleError((string)null!));
    }

    [Fact]
    public void Constructor_NullValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RESPSimpleError((IEnumerable<byte>)null!));
    }

    [Theory]
    [InlineData("foo\n")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("\n\r")]
    [InlineData("\r")]
    public void Constructor_StringWithNewLines_Throws(string srt)
    {
        Assert.Throws<ArgumentException>(() => new RESPSimpleError(srt));
    }

    [Theory]
    [InlineData(new byte[] { 0, 1, (byte)'\n' })]
    [InlineData(new byte[] { (byte)'\n' })]
    [InlineData(new byte[] { (byte)'\r', (byte)'\n' })]
    [InlineData(new byte[] { (byte)'\n', (byte)'\r' })]
    [InlineData(new byte[] { (byte)'\r' })]
    public void Constructor_ValueWithNewLines_Throws(byte[] value)
    {
        Assert.Throws<ArgumentException>(() => new RESPSimpleError(value));
    }

    [Theory]
    [InlineData("")]
    [InlineData("ERR error text")]
    public void Equals_Equal_ReturnsTrue(string str)
    {
        RESPSimpleError one = new(str);
        RESPSimpleError other = new(str);

        Assert.Equal(one.GetHashCode(), other.GetHashCode());
        Assert.True(one == other);
        Assert.False(one != other);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ERR error text")]
    public void Equals_EqualWithAttribute_ReturnsTrue(string str)
    {
        RESPSimpleError one = new(str)
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };
        RESPSimpleError other = new(str)
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
        RESPSimpleError one = new(value);
        RESPSimpleError other = new(value);

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
        RESPSimpleError one = new(first);
        RESPSimpleError other = new(second);

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
        RESPSimpleError one = new(first);
        RESPSimpleError other = new(second);

        Assert.True(one != other);
        Assert.False(one == other);
    }

    [Fact]
    public void Equals_AttribsDifferent_ReturnsFalse()
    {
        RESPSimpleError one = new(string.Empty)
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };

        RESPSimpleError other = new(string.Empty)
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
