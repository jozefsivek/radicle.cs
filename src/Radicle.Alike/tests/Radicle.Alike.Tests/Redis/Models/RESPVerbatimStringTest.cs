namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections.Generic;
using Xunit;

public class RESPVerbatimStringTest
{
    [Fact]
    public void Constructor_NullStringValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                new RESPVerbatimString(VerbatimStringType.Text, (string)null!));
    }

    [Fact]
    public void Constructor_NullValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                new RESPVerbatimString(VerbatimStringType.Text, (IEnumerable<byte>)null!));
    }

    [Theory]
    [InlineData(VerbatimStringType.Text, "")]
    [InlineData(VerbatimStringType.Text, "The quick brown fox jumps over the lazy dog")]
    [InlineData(VerbatimStringType.Markdown, "# Heading")]
    [InlineData(VerbatimStringType.Markdown, "[link to](http://www.example.com)")]
    public void Equals_Equal_ReturnsTrue(VerbatimStringType type, string str)
    {
        RESPVerbatimString one = new(type, str);
        RESPVerbatimString other = new(type, str);

        Assert.Equal(one.GetHashCode(), other.GetHashCode());
        Assert.True(one == other);
        Assert.False(one != other);
    }

    [Theory]
    [InlineData(VerbatimStringType.Markdown, "")]
    [InlineData(VerbatimStringType.Text, "The quick brown fox jumps over the lazy dog")]
    public void Equals_EqualWithAttribute_ReturnsTrue(VerbatimStringType type, string str)
    {
        RESPVerbatimString one = new(type, str)
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };
        RESPVerbatimString other = new(type, str)
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
    [InlineData(VerbatimStringType.Markdown, new byte[] { })]
    [InlineData(VerbatimStringType.Text, new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 })]
    [InlineData(VerbatimStringType.Text, new byte[] { 92, 2, 42, 5, 46, 8, 6, 86, 84 })]
    [InlineData(VerbatimStringType.Text, new byte[] { 255 })]
    public void Equals_EqualBinary_ReturnsTrue(VerbatimStringType type, byte[] value)
    {
        RESPVerbatimString one = new(type, value);
        RESPVerbatimString other = new(type, value);

        Assert.Equal(one.GetHashCode(), other.GetHashCode());
        Assert.True(one == other);
        Assert.False(one != other);
    }

    [Theory]
    [InlineData(VerbatimStringType.Text, "", " ")]
    [InlineData(VerbatimStringType.Text, "A", "a")]
    [InlineData(VerbatimStringType.Text, "AE", "\u00C6")] // Æ - LATIN CAPITAL LETTER AE
    [InlineData(VerbatimStringType.Text, "The quick brown fox jumps over the lazy dog", "The slow brown fox jumps over the lazy dog")]
    public void Equals_NonEqualStrings_ReturnsFalse(
            VerbatimStringType type,
            string first,
            string second)
    {
        RESPVerbatimString one = new(type, first);
        RESPVerbatimString other = new(type, second);

        Assert.True(one != other);
        Assert.False(one == other);
    }

    [Theory]
    [InlineData(VerbatimStringType.Text, new byte[] { }, new byte[] { 0 })]
    [InlineData(VerbatimStringType.Text, new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 4 })]
    [InlineData(VerbatimStringType.Text, new byte[] { 0 }, new byte[] { 255 })]
    [InlineData(VerbatimStringType.Text, new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3, 4 })]
    public void Equals_NonEqual_ReturnsFalse(
            VerbatimStringType type,
            byte[] first,
            byte[] second)
    {
        RESPVerbatimString one = new(type, first);
        RESPVerbatimString other = new(type, second);

        Assert.True(one != other);
        Assert.False(one == other);
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("\u00C6")] // Æ - LATIN CAPITAL LETTER AE
    [InlineData("The lazy brown fox jumps over the lazy dog")]
    public void Equals_NonEqualType_ReturnsFalse(
            string str)
    {
        RESPVerbatimString one = new(VerbatimStringType.Text, str);
        RESPVerbatimString other = new(VerbatimStringType.Markdown, str);

        Assert.True(one != other);
        Assert.False(one == other);
    }

    [Theory]
    [InlineData(new byte[] { })]
    [InlineData(new byte[] { 1, 2, 3 })]
    [InlineData(new byte[] { 0 })]
    public void Equals_NonEqualTypeAndBinaryValue_ReturnsFalse(
            byte[] value)
    {
        RESPVerbatimString one = new(VerbatimStringType.Text, value);
        RESPVerbatimString other = new(VerbatimStringType.Markdown, value);

        Assert.True(one != other);
        Assert.False(one == other);
    }

    [Fact]
    public void Equals_AttribsDifferent_ReturnsFalse()
    {
        RESPVerbatimString one = new(VerbatimStringType.Text, "foo")
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };

        RESPVerbatimString other = new(VerbatimStringType.Text, "foo")
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
