namespace Radicle.Common.Check.Models;

using System;
using System.Collections.Generic;
using Radicle.Common.Check.Models.Generic;
using Xunit;

public class DictionaryParamTest
{
    [Fact]
    public void Dictionary_dictionaryInput_ReturnsIDictionaryArgument()
    {
        Assert.IsAssignableFrom<IDictionaryParam<IEnumerable<KeyValuePair<object, object>>, object, object>>(
                Ensure.Dictionary(new Dictionary<object, object>()));
    }

    [Fact]
    public void OptionalDictionary_dictionaryInput_ReturnsIDictionaryArgument()
    {
        Assert.IsAssignableFrom<IDictionaryParam<IEnumerable<KeyValuePair<object, object>>, object, object>>(
                Ensure.OptionalDictionary((Dictionary<object, object>?)null));
    }

    [Fact]
    public void Optional_NullInput_Works()
    {
        Ensure.OptionalDictionary((Dictionary<object, object>?)null).NotEmpty();
        Ensure.OptionalDictionary((Dictionary<object, object>?)null).AllNotNull();
        Ensure.OptionalDictionary((Dictionary<object, object>?)null).AllValuesNotNull();
        Ensure.OptionalDictionary((Dictionary<object, object>?)null).AllKeys();

        Ensure.OptionalDictionary((Dictionary<object, string>?)null).NotEmpty();
        Ensure.OptionalDictionary((Dictionary<object, string>?)null).AllNotNull();
        Ensure.OptionalDictionary((Dictionary<object, string>?)null).AllValuesNotNull();
        Ensure.OptionalDictionary((Dictionary<object, string>?)null).AllKeys();

#pragma warning disable IDE0004 // Remove Unnecessary Cast
        Ensure.OptionalDictionary((Dictionary<string, string>?)null).NotEmpty();
        Ensure.OptionalDictionary((Dictionary<string, string>?)null).AllNotNull();
        Ensure.OptionalDictionary((Dictionary<string, string>?)null).AllValuesNotNull();
        Ensure.OptionalDictionary((Dictionary<string, string>?)null).AllKeys();
#pragma warning restore IDE0004 // Remove Unnecessary Cast
    }

    [Fact]
    public void AllNotNull_ValidInput_Works()
    {
        Dictionary<object, object> value = new()
        {
            { new object(), new object() },
        };

        Ensure.Dictionary(value)
                .AllNotNull();
    }

    [Fact]
    public void AllKeys_ValidInput_Works()
    {
        Dictionary<object, object> value = new()
        {
            { new object(), null! },
        };

        Ensure.Dictionary(value)
                .AllKeys();
    }

    [Fact]
    public void AllValuesNotNull_ValidInput_Works()
    {
        Dictionary<object, object> value = new()
        {
            { new object(), new object() },
        };

        Ensure.Dictionary(value)
                .AllValuesNotNull();
    }

    [Fact]
    public void AllNotNull_PassesOriginalParameter()
    {
        bool isParamItem = false;
        object k = new();
        object v = new();
        Dictionary<object, object> value = new()
        {
            { k, v },
        };

        Ensure.Dictionary(value)
                .AllNotNull(item => isParamItem =
                    ReferenceEquals(item.Key, k)
                    && ReferenceEquals(item.Value, v));

        Assert.True(isParamItem);
    }

    [Fact]
    public void AllNotNull_InvalidInput_Throws()
    {
        Dictionary<int, object> value = new()
        {
            { 42, null! },
        };

        ArgumentNullException exc = Assert.Throws<ArgumentNullException>(
                () => Ensure.Dictionary(value).AllNotNull());

        Assert.StartsWith(
                "Dictionary parameter 'value' contains null value at key [42].",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Fact]
    public void AllValuesNotNull_InvalidInput_Throws()
    {
        Dictionary<int, object> value = new()
        {
            { 42, null! },
        };

        ArgumentNullException exc = Assert.Throws<ArgumentNullException>(
                () => Ensure.Dictionary(value).AllValuesNotNull());

        Assert.StartsWith(
                "Dictionary parameter 'value' contains null value at key [42].",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Fact]
    public void AllNotNull_ArgumentNullException_IsEnriched()
    {
        Dictionary<string, object[]> testDict = new()
        {
            { "a", new object[] { 1 } },
            { "b", new object[] { null! } },
            { "c", new object[] { 1 } },
        };
        ArgumentNullException exc = Assert.Throws<ArgumentNullException>(
                () => Ensure
                    .Dictionary(testDict)
                    .AllNotNull(v => Ensure.Collection(v.Value).AllNotNull()));

        Assert.Equal(
                "Dictionary parameter 'testDict' at key [b]: Collection parameter 'v.Value' contains null value at index [0]. (Parameter 'v.Value')",
                exc.Message);
    }

    [Fact]
    public void AllNotNull_ArgumentOutOfRangeException_IsEnriched()
    {
        Dictionary<string, object[]> testDict = new()
        {
            { "a", new object[] { 1 } },
            { "b", new object[] { 1, 2 } },
            { "c", new object[] { 1 } },
        };
        ArgumentOutOfRangeException exc = Assert.Throws<ArgumentOutOfRangeException>(
                () => Ensure
                    .Dictionary(testDict)
                    .AllNotNull(v => Ensure.Collection(v.Value).InRange(1, 1)));

        Assert.StartsWith(
                "Dictionary parameter 'testDict' at key [b]: Parameter 'v.Value' length must be in range [1, 1]. (Parameter 'v.Value')",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Fact]
    public void AllNotNull_ArgumentException_IsEnriched()
    {
        Dictionary<string, int> testDict = new()
        {
            { "a", 1 },
            { "b", 2 },
            { "c", 3 },
        };
        ArgumentException exc = Assert.Throws<ArgumentException>(
                () => Ensure
                    .Dictionary(testDict)
                    .AllNotNull(v => Ensure.Param(v.Value).That(v => v != 3)));

        Assert.Equal(
                "Dictionary parameter 'testDict' at key [c]: Parameter 'v.Value' with value: 3 is not valid. (Parameter 'v.Value')",
                exc.Message);
    }

    [Fact]
    public void AllValuesNotNull_ArgumentNullException_IsEnriched()
    {
        Dictionary<string, object[]> testDict = new()
        {
            { "a", new object[] { 1 } },
            { "b", new object[] { null! } },
            { "c", new object[] { 1 } },
        };
        ArgumentNullException exc = Assert.Throws<ArgumentNullException>(
                () => Ensure
                    .Dictionary(testDict)
                    .AllValuesNotNull(v => Ensure.Collection(v).AllNotNull()));

        Assert.Equal(
                "Dictionary parameter 'testDict' at key [b], value error: Collection parameter 'v' contains null value at index [0]. (Parameter 'v')",
                exc.Message);
    }

    [Fact]
    public void AllValuesNotNull_ArgumentOutOfRangeException_IsEnriched()
    {
        Dictionary<string, object[]> testDict = new()
        {
            { "a", new object[] { 1 } },
            { "b", new object[] { 1, 2 } },
            { "c", new object[] { 1 } },
        };
        ArgumentOutOfRangeException exc = Assert.Throws<ArgumentOutOfRangeException>(
                () => Ensure
                    .Dictionary(testDict)
                    .AllValuesNotNull(v => Ensure.Collection(v).InRange(1, 1)));

        Assert.StartsWith(
                "Dictionary parameter 'testDict' at key [b], value error: Parameter 'v' length must be in range [1, 1]. (Parameter 'v')",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Fact]
    public void AllValuesNotNull_ArgumentException_IsEnriched()
    {
        Dictionary<string, int> testDict = new()
        {
            { "a", 1 },
            { "b", 2 },
            { "c", 3 },
        };
        ArgumentException exc = Assert.Throws<ArgumentException>(
                () => Ensure
                    .Dictionary(testDict)
                    .AllValuesNotNull(v => Ensure.Param(v).That(v => v != 3)));

        Assert.Equal(
                "Dictionary parameter 'testDict' at key [c], value error: Parameter 'v' with value: 3 is not valid. (Parameter 'v')",
                exc.Message);
    }

    [Fact]
    public void AllKeys_ArgumentNullException_IsEnriched()
    {
        Dictionary<string, object[]> testDict = new()
        {
            { "a", new object[] { 1 } },
            { "b", new object[] { 1 } },
            { "c", new object[] { 1 } },
        };
        ArgumentNullException exc = Assert.Throws<ArgumentNullException>(
                () => Ensure
                    .Dictionary(testDict)
                    .AllKeys(v =>
                    {
                        if (v == "b")
                        {
                            throw new ArgumentNullException("foo");
                        }
                    }));

        Assert.Equal(
                "Dictionary parameter 'testDict' at key [b], key error: Value cannot be null. (Parameter 'foo')",
                exc.Message);
    }

    [Fact]
    public void AllKeys_ArgumentOutOfRangeException_IsEnriched()
    {
        Dictionary<string, object[]> testDict = new()
        {
            { "a", new object[] { 1 } },
            { "bc", new object[] { 1 } },
            { "c", new object[] { 1 } },
        };
        ArgumentOutOfRangeException exc = Assert.Throws<ArgumentOutOfRangeException>(
                () => Ensure
                    .Dictionary(testDict)
                    .AllKeys(v => Ensure.Param(v).InRange(1, 1)));

        Assert.StartsWith(
                "Dictionary parameter 'testDict' at key [bc], key error: Parameter 'v' with value: 'bc' length must be in range [1, 1] (Parameter 'v')",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Fact]
    public void AllKeys_ArgumentException_IsEnriched()
    {
        Dictionary<string, int> testDict = new()
        {
            { "a", 1 },
            { "b", 2 },
            { "c", 3 },
        };
        ArgumentException exc = Assert.Throws<ArgumentException>(
                () => Ensure
                    .Dictionary(testDict)
                    .AllKeys(v => Ensure.Param(v).That(v => v != "c")));

        Assert.Equal(
                "Dictionary parameter 'testDict' at key [c], key error: Parameter 'v' with value: 'c' is not valid. (Parameter 'v')",
                exc.Message);
    }
}
