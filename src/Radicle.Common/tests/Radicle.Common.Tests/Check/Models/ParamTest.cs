namespace Radicle.Common.Check.Models;

using System;
using System.Collections.Generic;
using Radicle.Common.Check.Models.Generic;
using Xunit;

public class ParamTest
{
    [Fact]
    public void Param_GeneralInput_Returns()
    {
        Assert.IsAssignableFrom<IParam<object>>(
                Ensure.Param(new object()));
    }

    [Fact]
    public void IsSpecified_GeneralInput_ReturnsTrue()
    {
        Assert.True(Ensure.Param(new object()).IsSpecified);
    }

    [Fact]
    public void Optional_GeneralInput_ReturnsIParam()
    {
        Assert.IsAssignableFrom<IParam<object>>(
                Ensure.Optional((object?)null));
    }

    [Fact]
    public void OptionalIsSpecified_GeneralInput_ReturnsTrue()
    {
        Assert.True(Ensure.Optional(new object()).IsSpecified);
    }

    [Fact]
    public void OptionalIsSpecified_GeneralInput_ReturnsFalse()
    {
        Assert.False(Ensure.Optional((object?)null).IsSpecified);
    }

    [Fact]
    public void That_NullPredicate_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Ensure.Param(new object()).That(null!));
    }

    [Fact]
    public void OptionalThat_NullPredicate_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Ensure.Optional((object?)null).That(null!));
    }

    [Fact]
    public void That_true_predicate_Works()
    {
        Ensure.Param(new object()).That(_ => true);
    }

    [Fact]
    public void That_FalsePredicate_Throws()
    {
        ArgumentException exc = Assert.Throws<ArgumentException>(() =>
            Ensure.Param(new object()).That(_ => false, arg => $"foo {arg.Name}"));

        Assert.Equal("foo new object() (Parameter 'new object()')", exc.Message);
    }

    [Fact]
    public void OptionalThat_NullInput_PredicateIsNotCalled()
    {
        bool called = false;

        Ensure.Optional((object?)null).That(_ => called = true);

        Assert.False(called);
    }

    [Fact]
    public void Value_ReferenceType_ReturnsOriginalValue()
    {
        object value = new();

        Assert.Equal(value, Ensure.Param(value).Value);
    }

    [Fact]
    public void Value_ValueType_ReturnsOriginalValue()
    {
        const long value = 42L;

        Assert.Equal(value, Ensure.Param(value).Value);
    }

    [Fact]
    public void Value_NullReferenceType_Throws()
    {
        Assert.Throws<InvalidOperationException>(() =>
                Ensure.Optional((object?)null).Value);
    }

    [Fact]
    public void Value_NullCollectionType_Throws()
    {
        Assert.Throws<InvalidOperationException>(() =>
                Ensure.Optional((long[]?)null).Value);
    }

    [Fact]
    public void Value_NullDictionaryType_Throws()
    {
        Assert.Throws<InvalidOperationException>(() =>
                Ensure.OptionalDictionary<object, object>(null).Value);
    }

    [Fact]
    public void Value_NullValueType_Throws()
    {
        Assert.Throws<InvalidOperationException>(() =>
                Ensure.Optional((TestStruct?)null).Value);
    }

    [Fact]
    public void Value_NullNumericType_Throws()
    {
        Assert.Throws<InvalidOperationException>(() =>
                Ensure.Optional((long?)null).Value);
    }

    [Fact]
    public void ValueOrDefault_ReferenceType_ReturnsOriginalValue()
    {
        object? value = new();

        Assert.Equal(value, Ensure.Optional(value).ValueOrDefault());
    }

    [Fact]
    public void ValueOrDefault_CollectionType_ReturnsOriginalValue()
    {
        long[]? value = new[] { 42L };

        Assert.Equal(value, Ensure.Optional(value).ValueOrDefault());
    }

    [Fact]
    public void ValueOrDefault_DictionaryType_ReturnsOriginalValue()
    {
        Dictionary<long, long>? value = new() { { 42, 42 } };

        Assert.Equal(value, Ensure.OptionalDictionary(value).ValueOrDefault());
    }

    [Fact]
    public void ValueOrDefault_ValueType_ReturnsOriginalValue()
    {
        TestStruct? value = new TestStruct(42);

        Assert.Equal(value, Ensure.Optional(value).ValueOrDefault());
    }

    [Fact]
    public void ValueOrDefault_NumericType_ReturnsOriginalValue()
    {
        long? value = 42L;

        Assert.Equal(value, Ensure.Optional(value).ValueOrDefault());
    }

    [Fact]
    public void ValueOrDefault_NullReferenceType_ReturnsNull()
    {
        Assert.Null(Ensure.Optional((object?)null).ValueOrDefault());
    }

    [Fact]
    public void ValueOrDefault_NullCollectionType_ReturnsNull()
    {
        Assert.Null(Ensure.Optional((long[]?)null).ValueOrDefault());
    }

    [Fact]
    public void ValueOrDefault_NullDictionaryType_ReturnsNull()
    {
        Assert.Null(Ensure.OptionalDictionary((IDictionary<long, long>?)null).ValueOrDefault());
    }

    [Fact]
    public void ValueOrDefault_NullValueType_ReturnsDefaultStruct()
    {
        TestStruct expected = default;

        Assert.Equal(expected, Ensure.Optional((TestStruct?)null).ValueOrDefault());
    }

    [Fact]
    public void ValueOrDefault_NullNumericType_ReturnsNull()
    {
        Assert.Null(Ensure.Optional((long?)null).ValueOrDefault());
    }

    [Fact]
    public void ValueOr_ReferenceType_ReturnsOriginalValue()
    {
        object? value = new();

        Assert.Equal(value, Ensure.Optional(value).ValueOr(new object()));
        Assert.Equal(value, Ensure.Optional(value).ValueOr(() => new object()));
    }

    [Fact]
    public void ValueOr_CollectionType_ReturnsOriginalValue()
    {
        long[]? value = new[] { 42L };

        Assert.Equal(value, Ensure.Optional(value).ValueOr(Array.Empty<long>()));
        Assert.Equal(value, Ensure.Optional(value).ValueOr(() => Array.Empty<long>()));
    }

    [Fact]
    public void ValueOr_DictionaryType_ReturnsOriginalValue()
    {
        Dictionary<long, long>? value = new() { { 42, 42 } };

        Assert.Equal(value, Ensure.OptionalDictionary(value).ValueOr(new Dictionary<long, long>()));
        Assert.Equal(value, Ensure.OptionalDictionary(value).ValueOr(() => new Dictionary<long, long>()));
    }

    [Fact]
    public void ValueOr_ValueType_ReturnsOriginalValue()
    {
        TestStruct? value = new TestStruct(42);

        Assert.Equal(value, Ensure.Optional(value).ValueOr(new TestStruct(13)));
        Assert.Equal(value, Ensure.Optional(value).ValueOr(() => new TestStruct(13)));
    }

    [Fact]
    public void ValueOr_NumericType_ReturnsOriginalValue()
    {
        long? value = 42L;

        Assert.Equal(value, Ensure.Optional(value).ValueOr(13L));
        Assert.Equal(value, Ensure.Optional(value).ValueOr(() => 13L));
    }

    [Fact]
    public void ValueOr_NullReferenceType_fallback()
    {
        object expected = new();

        Assert.Equal(expected, Ensure.Optional((object?)null).ValueOr(expected));
        Assert.Equal(expected, Ensure.Optional((object?)null).ValueOr(() => expected));
    }

    [Fact]
    public void ValueOr_NullCollectionType_returns_fallback()
    {
        long[] expected = new[] { 42L };

        Assert.Equal(expected, Ensure.Optional((long[]?)null).ValueOr(expected));
        Assert.Equal(expected, Ensure.Optional((long[]?)null).ValueOr(() => expected));
    }

    [Fact]
    public void ValueOr_NullDictionaryType_returns_fallback()
    {
        Dictionary<long, long> expected = new() { { 42, 42 } };

        Assert.Equal(expected, Ensure.OptionalDictionary((IDictionary<long, long>?)null).ValueOr(expected));
        Assert.Equal(expected, Ensure.OptionalDictionary((IDictionary<long, long>?)null).ValueOr(() => expected));
    }

    [Fact]
    public void ValueOr_NullValueType_returns_fallback()
    {
        TestStruct expected = new(42);

        Assert.Equal(expected, Ensure.Optional((TestStruct?)null).ValueOr(expected));
        Assert.Equal(expected, Ensure.Optional((TestStruct?)null).ValueOr(() => expected));
    }

    [Fact]
    public void ValueOr_NullNumericType_returns_fallback()
    {
        const long expected = 42L;

        Assert.Equal(expected, Ensure.Optional((long?)null).ValueOr(expected));
        Assert.Equal(expected, Ensure.Optional((long?)null).ValueOr(() => expected));
    }

    private struct TestStruct : IEquatable<TestStruct>
    {
        public int V;

        public TestStruct(int v)
        {
            this.V = v;
        }

        public bool Equals(TestStruct other)
        {
            return other.V == this.V;
        }

        public override bool Equals(object? obj)
        {
            return obj is TestStruct s && this.Equals(s);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.V);
        }
    }
}
