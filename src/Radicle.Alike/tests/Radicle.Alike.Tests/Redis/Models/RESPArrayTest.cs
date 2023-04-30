namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections.Generic;
using Xunit;

public class RESPArrayTest
{
    [Fact]
    public void Constructor_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RESPArray(null!));
    }

    [Fact]
    public void Constructor_NullValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RESPArray(new RESPValue[] { null! }));
    }

    [Fact]
    public void ToRESPMap_FromUnEvenSelf_Throws()
    {
        RESPArray array = new(new RESPSimpleString[] { "a", "foo", "b" });

        Assert.Throws<ArgumentOutOfRangeException>(() => array.ToRESPMap());
    }

    [Fact]
    public void ToRESPMap_Self_Works()
    {
        RESPArray array = new(new RESPSimpleString[] { "a", "foo", "b", "bar" });

        RESPMap map = array.ToRESPMap();

        Assert.Equal(2u, map.Length);
        Assert.Equal(new RESPSimpleString("foo"), map.Items[new RESPSimpleString("a")]);
        Assert.Equal(new RESPSimpleString("bar"), map.Items[new RESPSimpleString("b")]);
    }

    [Fact]
    public void Equals_Empty_ReturnsTrue()
    {
        RESPArray one = new(Array.Empty<RESPValue>());
        RESPArray other = new(Array.Empty<RESPValue>());

        Assert.Equal(one.GetHashCode(), other.GetHashCode());
        Assert.True(one == other);
        Assert.False(one != other);
    }

    [Fact]
    public void Equals_Equal_ReturnsTrue()
    {
        RESPArray one = new(new RESPValue[] { RESPArray.Empty, new RESPNumber(1) })
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };

        RESPArray other = new(new RESPValue[] { RESPArray.Empty, new RESPNumber(1) })
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
        RESPArray one = new(new RESPValue[] { RESPArray.Empty, new RESPNumber(1) });

        RESPArray other = new(new RESPValue[] { RESPArray.Empty, new RESPNumber(2) });

        Assert.True(one != other);
        Assert.False(one == other);
    }

    [Fact]
    public void Equals_AttribsDifferent_ReturnsFalse()
    {
        RESPArray one = new(new RESPValue[] { RESPArray.Empty, new RESPNumber(1) })
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("bar") },
            }),
        };

        RESPArray other = new(new RESPValue[] { RESPArray.Empty, new RESPNumber(1) })
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
        RESPArray one = new(new RESPValue[] { RESPArray.Empty, new RESPNumber(1) });

        RESPArray other = new(new RESPValue[] { RESPArray.Empty, new RESPNumber(1), new RESPNumber(2) });

        Assert.True(one != other);
        Assert.False(one == other);
    }
}
