namespace Radicle.Common.Check.Base;

using System;
using System.Collections.Generic;
using Xunit;

public class TypedNameTest
{
    [Fact]
    public void TypedName_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ConcreteCaseSensitiveName(null!));
        Assert.Throws<ArgumentNullException>(() => new ConcreteCaseInsensitiveName(null!));
        Assert.Throws<ArgumentNullException>(() => new ConcreteLowerCaseName(null!));
    }

    [Fact]
    public void TypedName_OutOfBoundsInput_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                new ConcreteCaseSensitiveName(
                    new string('d', ConcreteCaseSensitiveName.Specification.MinLength - 1)));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                new ConcreteCaseSensitiveName(
                    new string('d', ConcreteCaseSensitiveName.Specification.MaxLength + 1)));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
                new ConcreteCaseInsensitiveName(
                    new string('d', ConcreteCaseInsensitiveName.Specification.MinLength - 1)));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                new ConcreteCaseInsensitiveName(
                    new string('d', ConcreteCaseInsensitiveName.Specification.MaxLength + 1)));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
                new ConcreteLowerCaseName(
                    new string('d', ConcreteLowerCaseName.Specification.MinLength - 1)));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                new ConcreteLowerCaseName(
                    new string('d', ConcreteLowerCaseName.Specification.MaxLength + 1)));
    }

    [Theory]
    [InlineData("not a valid name")]
    [InlineData("1spam")]
    [InlineData("1")]
    public void TypedName_InvalidInput_Throws(string invalidItem)
    {
        Assert.Throws<ArgumentException>(() => new ConcreteCaseSensitiveName(invalidItem));
        Assert.Throws<ArgumentException>(() => new ConcreteCaseInsensitiveName(invalidItem));
    }

    [Theory]
    [InlineData("not a valid name")]
    [InlineData("Foo")]
    public void LowerCaseTypedName_InvalidInput_Throws(string invalidItem)
    {
        Assert.Throws<ArgumentException>(() => new ConcreteLowerCaseName(invalidItem));
    }

    [Theory]
    [InlineData("A", "A")]
    [InlineData("a", "a")]
    [InlineData("Name", "Name")]
    public void Value_PreservesOriginalCase(string expected, string input)
    {
        Assert.Equal(expected, new ConcreteCaseSensitiveName(input).Value);
        Assert.Equal(expected, new ConcreteCaseInsensitiveName(input).Value);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Name")]
    public void InvariantValue_CaseSensitive_PreservesOriginalCase(string input)
    {
        Assert.Equal(input, new ConcreteCaseSensitiveName(input).InvariantValue);
    }

    [Theory]
    [InlineData("A", "a")]
    [InlineData("Name", "name")]
    public void InvariantValue_CaseInsensitive_ReturnsLowerCase(string input, string expected)
    {
        Assert.Equal(expected, new ConcreteCaseInsensitiveName(input).InvariantValue);
    }

    [Fact]
    public void ToString_Works()
    {
        Assert.Equal("Test name: \"Group\"", new ConcreteCaseSensitiveName("Group").ToString());
        Assert.Equal("Test name: \"group\"", new ConcreteCaseInsensitiveName("Group").ToString());
    }

    [Fact]
    public void Equals_Null_Works()
    {
        ConcreteCaseSensitiveName a = new("foo");
        ConcreteCaseSensitiveName b = null!;

#pragma warning disable CA1508 // Avoid dead conditional code
        Assert.False(a.Equals((object?)b));
        Assert.False(a.Equals(b));

        Assert.False(a == b);
        Assert.False(b == a);
        Assert.False((object)a == b);
        Assert.False((object)b == a);
        Assert.False(a == (object)b);
        Assert.False(b == (object)a);

        Assert.True(a != b);
        Assert.True(b != a);
        Assert.True((object)a != b);
        Assert.True((object)b != a);
        Assert.True(a != (object)b);
        Assert.True(b != (object)a);
#pragma warning restore CA1508 // Avoid dead conditional code
    }

    [Theory]
    [InlineData("Name")]
    [InlineData("snake_name")]
    public void Equals_DifferentTypes_Works(string item)
    {
        ConcreteCaseInsensitiveName a = new(item);
        ConcreteCaseInsensitiveName b = new(item);
        ConcreteCaseSensitiveName c = new(item);
        string d = item;

        Assert.True(a.Equals(b));
        Assert.False(a.Equals(c));
        Assert.False(a.Equals(d));
    }

    [Theory]
    [InlineData("Name")]
    [InlineData("snake_name")]
    public void Equals_EqualValues_Works(string input)
    {
        ConcreteCaseSensitiveName a = new(input);
        ConcreteCaseSensitiveName b = new(input);

        Assert.True(a.Equals(b));

        Assert.True(a == b);
        Assert.True(a == (object)b);
        Assert.True((object)a == b);

        Assert.False(a != b);
        Assert.False(a != (object)b);
        Assert.False((object)a != b);
    }

    [Theory]
    [InlineData("Name", "name")]
    [InlineData("Name", "nAmE")]
    [InlineData("snake_name", "Snake_name")]
    [InlineData("snake_name", "SNAKE_NAME")]
    public void Equals_MixedCaseEqualValues_Works(string aInput, string bInput)
    {
        ConcreteCaseInsensitiveName a = new(aInput);
        ConcreteCaseInsensitiveName b = new(bInput);

        Assert.True(a.Equals(b));

        Assert.True(a == b);
        Assert.True(a == (object)b);
        Assert.True((object)a == b);

        Assert.False(a != b);
        Assert.False(a != (object)b);
        Assert.False((object)a != b);
    }

    [Theory]
    [InlineData("Name", "foo")]
    [InlineData("Name", "name")]
    [InlineData("Name", "nAmE")]
    [InlineData("snake_name", "Snake_name")]
    [InlineData("snake_name", "SNAKE_NAME")]
    [InlineData("snake_name", "SnakeName")]
    public void Equals_NonEqual_Works(string aInput, string bInput)
    {
        ConcreteCaseSensitiveName a = new(aInput);
        ConcreteCaseSensitiveName b = new(bInput);

        Assert.False(a.Equals(b));

        Assert.False(a == b);
        Assert.False(a == (object)b);
        Assert.False((object)a == b);

        Assert.True(a != b);
        Assert.True(a != (object)b);
        Assert.True((object)a != b);
    }

    [Fact]
    public void CompareTo_NonMatchingType_Throws()
    {
        ConcreteCaseInsensitiveName a = new("a");

        Assert.Throws<ArgumentException>(() => a.CompareTo(new object()));
    }

    [Fact]
    public void CompareTo_Null_Works()
    {
        ConcreteCaseSensitiveName a = new("a");

        Assert.Equal(1, a.CompareTo(null));

        Assert.Equal(1, a.CompareTo((object?)null)); // b follows a

        Assert.True(a > null);
        Assert.True(a >= null);

#pragma warning disable SA1131 // Use readable conditions
        Assert.True(null < a);
        Assert.True(null <= a);
#pragma warning restore SA1131 // Use readable conditions
    }

    [Fact]
    public void CompareTo_Works()
    {
        ConcreteCaseSensitiveName a = new("a");
        ConcreteCaseSensitiveName b = new("b");

        Assert.Equal(-1, a.CompareTo(b));
        Assert.Equal(0, a.CompareTo(a));
        Assert.Equal(1, b.CompareTo(a));

        Assert.Equal(-1, a.CompareTo((object)b)); // a precedes b
        Assert.Equal(0, a.CompareTo((object)a));
        Assert.Equal(1, b.CompareTo((object)a)); // b follows a

        Assert.True(b > a);
        Assert.True(b >= a);
        Assert.True(a < b);
        Assert.True(a <= b);
        Assert.False(a > b);
        Assert.False(a >= b);
        Assert.False(b < a);
        Assert.False(b <= a);
    }

    [Fact]
    public void CompareTo_CaseInsensitive_Works()
    {
        ConcreteCaseInsensitiveName a = new("a");
        ConcreteCaseInsensitiveName a2 = new("A");
        ConcreteCaseInsensitiveName b = new("B");

        Assert.Equal(-1, a.CompareTo(b));
        Assert.Equal(0, a.CompareTo(a2));
        Assert.Equal(1, b.CompareTo(a));

        Assert.Equal(-1, a.CompareTo((object)b)); // a precedes b
        Assert.Equal(0, a.CompareTo((object)a2));
        Assert.Equal(1, b.CompareTo((object)a)); // b follows a

        Assert.True(b > a);
        Assert.True(b >= a);
        Assert.True(a < b);
        Assert.True(a <= b);
        Assert.False(a > b);
        Assert.False(a >= b);
        Assert.False(b < a);
        Assert.False(b <= a);
    }

    [Fact]
    public void TypedName_UseInDictionary_Works()
    {
        Dictionary<ConcreteCaseInsensitiveName, string> d = new()
        {
            { new ConcreteCaseInsensitiveName("A"), "a" },
            { new ConcreteCaseInsensitiveName("b"), "b" },
            { new ConcreteCaseInsensitiveName("c"), "c" },
        };

        Assert.Equal("a", d[new ConcreteCaseInsensitiveName("A")]);
        Assert.Equal("a", d[new ConcreteCaseInsensitiveName("a")]);
    }

    [Theory]
    [InlineData("Name")]
    [InlineData("snake_name")]
    public void GetHashCode_EqualValues_Works(string input)
    {
        ConcreteCaseSensitiveName a = new(input);
        ConcreteCaseSensitiveName b = new(input);

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Theory]
    [InlineData("Name", "name")]
    [InlineData("Name", "nAmE")]
    [InlineData("snake_name", "Snake_name")]
    [InlineData("snake_name", "SNAKE_NAME")]
    public void GetHashCode_MixedCaseEqualValues_Works(string aInput, string bInput)
    {
        ConcreteCaseInsensitiveName a = new(aInput);
        ConcreteCaseInsensitiveName b = new(bInput);

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Theory]
    [InlineData("Name", "foo")]
    [InlineData("Name", "name")]
    [InlineData("Name", "nAmE")]
    [InlineData("snake_name", "Snake_name")]
    [InlineData("snake_name", "SNAKE_NAME")]
    [InlineData("snake_name", "SnakeName")]
    public void GetHashCode_NonEqualValues_Works(string aInput, string bInput)
    {
        ConcreteCaseSensitiveName a = new(aInput);
        ConcreteCaseSensitiveName b = new(bInput);

        Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
    }
}
