namespace Radicle.Alike.Redis.Models;

using System.Collections.Generic;
using Xunit;

public class RESPDoubleTest
{
    [Theory]
    [InlineData(0)]
    [InlineData(double.Epsilon)]
    [InlineData(double.MinValue)]
    [InlineData(double.MaxValue)]
    [InlineData(double.NegativeInfinity)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NaN)]
    public void Equals_Equal_ReturnsTrue(double number)
    {
        RESPDouble one = new(number);
        RESPDouble other = new(number);

        Assert.Equal(one.GetHashCode(), other.GetHashCode());
        Assert.True(one == other);
        Assert.False(one != other);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(double.Epsilon)]
    [InlineData(double.MinValue)]
    [InlineData(double.MaxValue)]
    [InlineData(double.NegativeInfinity)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NaN)]
    public void Equals_EqualWithAttribute_ReturnsTrue(double number)
    {
        RESPDouble one = new(number)
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };
        RESPDouble other = new(number)
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

    [Fact]
    public void Equals_EqualDifferentlySourcedNaN_ReturnsTrue()
    {
        // see https://en.wikipedia.org/wiki/NaN
        RESPDouble one = new(0.0 / 0.0);
        RESPDouble other = new(double.NegativeInfinity * 0.0);

        Assert.Equal(one.GetHashCode(), other.GetHashCode());
        Assert.True(one == other);
        Assert.False(one != other);
    }

    [Theory]
    [InlineData(0.0, 1.0)]
    [InlineData(double.MaxValue, double.MinValue)]
    [InlineData(double.MinValue, double.NaN)]
    [InlineData(double.MinValue, double.PositiveInfinity)]
    [InlineData(double.MinValue, double.NegativeInfinity)]
    public void Equals_NonEqual_ReturnsFalse(double first, double second)
    {
        RESPDouble one = new(first);
        RESPDouble other = new(second);

        Assert.True(one != other);
        Assert.False(one == other);
    }

    [Fact]
    public void Equals_AttribsDifferent_ReturnsFalse()
    {
        RESPDouble one = new(0.0)
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };

        RESPDouble other = new(0.0)
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
