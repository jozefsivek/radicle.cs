namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections.Generic;
using Xunit;

public class RESPSetTest
{
    [Fact]
    public void Constructor_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RESPSet(null!));
    }

    [Fact]
    public void Constructor_NullValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RESPSet(new RESPValue[] { null! }));
    }

    [Fact]
    public void Equals_Empty_ReturnsTrue()
    {
        RESPSet one = new(Array.Empty<RESPValue>());
        RESPSet other = RESPSet.Empty;

        Assert.Equal(one.GetHashCode(), other.GetHashCode());
        Assert.True(one == other);
        Assert.False(one != other);
    }

    [Fact]
    public void Equals_Equal_ReturnsTrue()
    {
        RESPSet one = new(new RESPValue[] { new RESPSimpleString("foo"), new RESPNumber(1) })
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };

        RESPSet other = new(new RESPValue[] { new RESPNumber(1), new RESPSimpleString("foo") })
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
    public void Equals_NonEqual_ReturnsFalse()
    {
        RESPSet one = new(new RESPValue[] { new RESPSimpleString("foo"), new RESPNumber(1) });

        RESPSet other = new(new RESPValue[] { new RESPNumber(2), new RESPSimpleString("foo") });

        Assert.True(one != other);
        Assert.False(one == other);
    }

    [Fact]
    public void Equals_AttribsDifferent_ReturnsFalse()
    {
        RESPSet one = new(new RESPValue[] { new RESPSimpleString("foo"), new RESPNumber(1) })
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };

        RESPSet other = new(new RESPValue[] { new RESPSimpleString("foo"), new RESPNumber(1) })
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
            }),
        };

        Assert.True(one != other);
        Assert.False(one == other);
    }

    [Fact]
    public void Equals_NonEqualLength_ReturnsFalse()
    {
        RESPSet one = new(new RESPValue[] { new RESPSimpleString("foo"), new RESPNumber(1) });

        RESPSet other = new(new RESPValue[] { new RESPSimpleString("foo"), new RESPNumber(1), new RESPNumber(2) });

        Assert.True(one != other);
        Assert.False(one == other);
    }
}
