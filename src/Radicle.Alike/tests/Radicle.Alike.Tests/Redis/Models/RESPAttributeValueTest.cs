namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections.Generic;
using Xunit;

public class RESPAttributeValueTest
{
    [Fact]
    public void Constructor_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RESPAttributeValue(null!));
    }

    [Fact]
    public void Constructor_NullValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RESPAttributeValue(
            new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("foo"), null! },
        }));
    }

    [Fact]
    public void Constructor_NullKey_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RESPAttributeValue(
            new Dictionary<RESPValue, RESPValue>()
        {
            { null!, new RESPSimpleString("bar") },
        }));
    }

    [Fact]
    public void Equals_Empty_ReturnsTrue()
    {
        RESPAttributeValue one = new(Array.Empty<KeyValuePair<RESPValue, RESPValue>>());
        RESPAttributeValue other = new(Array.Empty<KeyValuePair<RESPValue, RESPValue>>());

        Assert.Equal(one.GetHashCode(), other.GetHashCode());
        Assert.True(one == other);
        Assert.False(one != other);
    }

    [Fact]
    public void Equals_Equal_ReturnsTrue()
    {
        RESPAttributeValue one = new(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
        })
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };

        RESPAttributeValue other = new(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
        })
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
        RESPAttributeValue one = new(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
        });

        RESPAttributeValue other = new(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
        });

        Assert.True(one != other);
        Assert.False(one == other);
    }

    [Fact]
    public void Equals_NonEqualKeys_ReturnsFalse()
    {
        RESPAttributeValue one = new(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
        });

        RESPAttributeValue other = new(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("Foo"), new RESPSimpleString("bar") },
        });

        Assert.True(one != other);
        Assert.False(one == other);
    }

    [Fact]
    public void Equals_AttribsDifferent_ReturnsFalse()
    {
        RESPAttributeValue one = new(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
        })
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };

        RESPAttributeValue other = new(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
        })
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
        RESPAttributeValue one = new(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
        });

        RESPAttributeValue other = new(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            { new RESPSimpleString("Foo"), new RESPSimpleString("bar") },
        });

        Assert.True(one != other);
        Assert.False(one == other);
    }
}
