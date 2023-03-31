namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections.Generic;
using Xunit;

public class RESPMapTest
{
    [Fact]
    public void Constructor_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RESPMap(null!));
    }

    [Fact]
    public void Constructor_NullValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RESPMap(
            new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("foo"), null! },
        }));
    }

    [Fact]
    public void Constructor_NullKey_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RESPMap(
            new Dictionary<RESPValue, RESPValue>()
        {
            { null!, new RESPSimpleString("bar") },
        }));
    }

    [Fact]
    public void Equals_Empty_ReturnsTrue()
    {
        RESPMap one = new(Array.Empty<KeyValuePair<RESPValue, RESPValue>>());
        RESPMap other = new(Array.Empty<KeyValuePair<RESPValue, RESPValue>>());

        Assert.Equal(one.GetHashCode(), other.GetHashCode());
        Assert.True(one == other);
        Assert.False(one != other);
    }

    [Fact]
    public void Equals_Equal_ReturnsTrue()
    {
        RESPMap one = new(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
        })
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };

        RESPMap other = new(new Dictionary<RESPValue, RESPValue>()
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
        RESPMap one = new(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
        });

        RESPMap other = new(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
        });

        Assert.True(one != other);
        Assert.False(one == other);
    }

    [Fact]
    public void Equals_NonEqualKeys_ReturnsFalse()
    {
        RESPMap one = new(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
        });

        RESPMap other = new(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("Foo"), new RESPSimpleString("bar") },
        });

        Assert.True(one != other);
        Assert.False(one == other);
    }

    [Fact]
    public void Equals_AttribsDifferent_ReturnsFalse()
    {
        RESPMap one = new(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
        })
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };

        RESPMap other = new(new Dictionary<RESPValue, RESPValue>()
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
        RESPMap one = new(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
        });

        RESPMap other = new(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            { new RESPSimpleString("Foo"), new RESPSimpleString("bar") },
        });

        Assert.True(one != other);
        Assert.False(one == other);
    }
}
