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
    public void ExplicitConversion_FromNullRESPArray_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => (RESPMap)(RESPArray)null!);
    }

    [Fact]
    public void ExplicitConversion_FromUnEvenRESPArray_Throws()
    {
        RESPArray array = new(new RESPSimpleString[] { "a", "foo", "b" });

        Assert.Throws<ArgumentOutOfRangeException>(() => (RESPMap)array);
    }

    [Fact]
    public void ExplicitConversion_FromRESPArray_Works()
    {
        RESPArray array = new(new RESPSimpleString[] { "a", "foo", "b", "bar" });

        RESPMap map = (RESPMap)array;

        Assert.Equal(2u, map.Length);
        Assert.Equal(new RESPSimpleString("foo"), map.Items[new RESPSimpleString("a")]);
        Assert.Equal(new RESPSimpleString("bar"), map.Items[new RESPSimpleString("b")]);
    }

    [Fact]
    public void FromRESPArray_FromNullRESPArray_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => RESPMap.FromRESPArray(null!));
    }

    [Fact]
    public void FromRESPArray_FromUnEvenRESPArray_Throws()
    {
        RESPArray array = new(new RESPSimpleString[] { "a", "foo", "b" });

        Assert.Throws<ArgumentOutOfRangeException>(() => RESPMap.FromRESPArray(array));
    }

    [Fact]
    public void FromRESPArray_FromRESPArray_Works()
    {
        RESPArray array = new(new RESPSimpleString[] { "a", "foo", "b", "bar" });

        RESPMap map = RESPMap.FromRESPArray(array);

        Assert.Equal(2u, map.Length);
        Assert.Equal(new RESPSimpleString("foo"), map.Items[new RESPSimpleString("a")]);
        Assert.Equal(new RESPSimpleString("bar"), map.Items[new RESPSimpleString("b")]);
    }

    [Fact]
    public void ToRESPArray_NullRESPMap_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => RESPMap.ToRESPArray(null!));
    }

    [Fact]
    public void ToRESPArray_Array_Works()
    {
        RESPArray expected = new(new RESPSimpleString[] { "a", "foo", "b", "bar" });
        RESPMap map = RESPMap.FromRESPArray(expected);
        RESPArray result = RESPMap.ToRESPArray(map);

        Assert.Equal(4u, result.Length);
        Assert.Contains(expected.Items[0], result);
        Assert.Contains(expected.Items[1], result);
        Assert.Contains(expected.Items[2], result);
        Assert.Contains(expected.Items[3], result);
    }

    [Fact]
    public void ToRESPArray_Self_Works()
    {
        RESPArray expected = new(new RESPSimpleString[] { "a", "foo", "b", "bar" });
        RESPMap map = RESPMap.FromRESPArray(expected);
        RESPArray result = map.ToRESPArray();

        Assert.Equal(4u, result.Length);
        Assert.Contains(expected.Items[0], result);
        Assert.Contains(expected.Items[1], result);
        Assert.Contains(expected.Items[2], result);
        Assert.Contains(expected.Items[3], result);
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
