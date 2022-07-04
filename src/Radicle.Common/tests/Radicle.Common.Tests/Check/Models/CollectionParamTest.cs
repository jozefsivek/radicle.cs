namespace Radicle.Common.Check.Models;

using System;
using System.Collections.Generic;
using Radicle.Common.Check.Models.Generic;
using Xunit;

public class CollectionParamTest
{
    [Fact]
    public void Collection_CollectionInput_ReturnsICollectionParam()
    {
        Assert.IsAssignableFrom<ICollectionParam<IEnumerable<object>, object>>(
                Ensure.Collection(Array.Empty<object>()));
    }

    [Fact]
    public void OptionalCollection_CollectionInput_ReturnsICollectionParam()
    {
        Assert.IsAssignableFrom<ICollectionParam<IEnumerable<object>, object>>(
                Ensure.OptionalCollection((object[]?)null));
    }

    [Fact]
    public void Optional_NullInput_does_not_fail()
    {
        Ensure.OptionalCollection((object[]?)null).NotEmpty();
        Ensure.OptionalCollection((object[]?)null).AllNotNull();
        Ensure.OptionalCollection((object[]?)null).InRange(0, 256);

#pragma warning disable IDE0004 // Remove Unnecessary Cast
        Ensure.OptionalCollection((string[]?)null).NotEmpty();
        Ensure.OptionalCollection((string[]?)null).AllNotNull();
        Ensure.OptionalCollection((string[]?)null).InRange(0, 256);
#pragma warning restore IDE0004 // Remove Unnecessary Cast
    }

    [Fact]
    public void NotEmpty_NonEmptyInput_Works()
    {
        Ensure.Collection(new List<object>() { new object() })
                .NotEmpty();
    }

    [Fact]
    public void NotEmpty_EmptyInput_Throws()
    {
        ArgumentOutOfRangeException exc = Assert.Throws<ArgumentOutOfRangeException>(
                () => Ensure.Collection(Array.Empty<object>()).NotEmpty());

        Assert.StartsWith(
                "Parameter 'Array.Empty<object>()' cannot be an empty enumerable.",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Fact]
    public void AllNotNull_ValidInput_Works()
    {
        Ensure.Collection(new List<object>() { new object() })
                .AllNotNull();
    }

    [Fact]
    public void AllNotNull_PassesOriginalParameter()
    {
        bool isOriginalParameter = false;
        object value = new();

        Ensure.Collection(new object[] { value })
                .AllNotNull(item => isOriginalParameter =
                    item is not null
                    && ReferenceEquals(item, value));

        Assert.True(isOriginalParameter);
    }

    [Fact]
    public void AllNotNull_InvalidInput_Throws()
    {
        ArgumentNullException exc = Assert.Throws<ArgumentNullException>(
                () => Ensure.Collection(new object[] { null! }).AllNotNull());

        Assert.StartsWith(
                "Collection parameter 'new object[] { null! }' contains null value at index [0].",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Fact]
    public void AllNotNull_ArgumentNullException_IsEnriched()
    {
        object[][] testCollection = new object[][]
        {
            new object[] { 1 },
            new object[] { null! },
            new object[] { 1 },
        };
        ArgumentNullException exc = Assert.Throws<ArgumentNullException>(
                () => Ensure
                    .Collection(testCollection)
                    .AllNotNull(v => Ensure.Collection(v).AllNotNull()));

        Assert.Equal(
                "Collection parameter 'testCollection' at index [1]: Collection parameter 'v' contains null value at index [0]. (Parameter 'v')",
                exc.Message);
    }

    [Fact]
    public void AllNotNull_ArgumentOutOfRangeException_IsEnriched()
    {
        object[][] testCollection = new object[][]
        {
            new object[] { 1 },
            new object[] { 1, 2 },
            new object[] { 1 },
        };
        ArgumentOutOfRangeException exc = Assert.Throws<ArgumentOutOfRangeException>(
                () => Ensure
                    .Collection(testCollection)
                    .AllNotNull(v => Ensure.Collection(v).InRange(1, 1)));

        Assert.StartsWith(
                "Collection parameter 'testCollection' at index [1]: Parameter 'v' length must be in range [1, 1]. (Parameter 'v')",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Fact]
    public void AllNotNull_ArgumentException_IsEnriched()
    {
        int[] testCollection = new int[]
        {
            1,
            2,
            3,
        };
        ArgumentException exc = Assert.Throws<ArgumentException>(
                () => Ensure
                    .Collection(testCollection)
                    .AllNotNull(v => Ensure.Param(v).That(v => v != 3)));

        Assert.Equal(
                "Collection parameter 'testCollection' at index [2]: Parameter 'v' with value: 3 is not valid. (Parameter 'v')",
                exc.Message);
    }

    [Theory]
    [InlineData(0, 0, 0, true, true)]
    [InlineData(1, 1, 1, true, true)]
    [InlineData(2, 2, 10, true, false)]
    [InlineData(3, 2, 3, false, true)]
    [InlineData(4, 3, 5, false, false)]
    public void InRange_ValidInput_Works(
            int length,
            int min,
            int max,
            bool lower,
            bool upper)
    {
        object[] value = new object[length];

        Ensure.Collection(value).InRange(min, max, lower, upper);
    }

    [Theory]
    [InlineData(10, 0, 0, true, true)]
    [InlineData(0, 1, 1, true, true)]
    [InlineData(3, 0, 3, true, false)]
    [InlineData(4, 4, 10, false, true)]
    [InlineData(1, 1, 10, false, false)]
    [InlineData(10, 1, 10, false, false)]
    public void InRange_InvalidInput_Throws(
            int length,
            int min,
            int max,
            bool lower,
            bool upper)
    {
        object[] value = new object[length];

        ArgumentOutOfRangeException exc = Assert.Throws<ArgumentOutOfRangeException>(
                () => Ensure.Collection(value).InRange(min, max, lower, upper));

        string range = Dump.Range(min, max, lower, upper);

        Assert.StartsWith(
                $"Parameter 'value' length must be in range {range}.",
                exc.Message,
                StringComparison.Ordinal);
    }
}
