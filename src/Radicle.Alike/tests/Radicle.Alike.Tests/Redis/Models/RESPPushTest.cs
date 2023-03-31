namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections.Generic;
using Xunit;

public class RESPPushTest
{
    [Fact]
    public void Constructor_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RESPPush(null!));
    }

    [Fact]
    public void Constructor_NullValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RESPPush(new RESPValue[] { null! }));
    }

    [Fact]
    public void Constructor_EmptyInput_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new RESPPush(Array.Empty<RESPValue>()));
    }

    [Fact]
    public void Constructor_NonSimpleStringFirstArgument_Throws()
    {
        Assert.Throws<ArgumentException>(() => new RESPPush(new RESPValue[] { new RESPNumber(1) }));
    }

    [Fact]
    public void Equals_Simple_ReturnsTrue()
    {
        RESPPush one = new(new RESPValue[] { new RESPSimpleString("pubsub") });
        RESPPush other = new(new RESPValue[] { new RESPSimpleString("pubsub") });

        Assert.Equal(one.GetHashCode(), other.GetHashCode());
        Assert.True(one == other);
        Assert.False(one != other);
    }

    [Fact]
    public void Equals_Equal_ReturnsTrue()
    {
        RESPPush one = new(new RESPValue[] { new RESPSimpleString("monitor"), new RESPNumber(1) })
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };

        RESPPush other = new(new RESPValue[] { new RESPSimpleString("monitor"), new RESPNumber(1) })
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
        RESPPush one = new(new RESPValue[] { new RESPSimpleString("monitor"), new RESPNumber(1) });

        RESPPush other = new(new RESPValue[] { new RESPSimpleString("monitor"), new RESPNumber(2) });

        Assert.True(one != other);
        Assert.False(one == other);
    }

    [Fact]
    public void Equals_AttribsDifferent_ReturnsFalse()
    {
        RESPPush one = new(new RESPValue[] { new RESPSimpleString("monitor"), new RESPNumber(1) })
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };

        RESPPush other = new(new RESPValue[] { new RESPSimpleString("monitor"), new RESPNumber(1) })
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
        RESPPush one = new(new RESPValue[] { new RESPSimpleString("monitor"), new RESPNumber(1) });

        RESPPush other = new(new RESPValue[] { new RESPSimpleString("monitor"), new RESPNumber(1), new RESPNumber(2) });

        Assert.True(one != other);
        Assert.False(one == other);
    }
}
