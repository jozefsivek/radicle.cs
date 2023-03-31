namespace Radicle.Alike.Redis.Models;

using System.Collections.Generic;
using Xunit;

public class RESPBooleanTest
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Equals_Equal_ReturnsTrue(bool value)
    {
        RESPBoolean one = new(value);
        RESPBoolean other = new(value);

        Assert.Equal(one.GetHashCode(), other.GetHashCode());
        Assert.True(one == other);
        Assert.False(one != other);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Equals_EqualWithAttribute_ReturnsTrue(bool value)
    {
        RESPBoolean one = new(value)
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };
        RESPBoolean other = new(value)
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
    [InlineData(true)]
    [InlineData(false)]
    public void Equals_NonEqual_ReturnsFalse(bool value)
    {
        RESPBoolean one = new(!value);
        RESPBoolean other = new(value);

        Assert.True(one != other);
        Assert.False(one == other);
    }

    [Fact]
    public void Equals_AttribsDifferent_ReturnsFalse()
    {
        RESPBoolean one = new(true)
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };

        RESPBoolean other = new(true)
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
