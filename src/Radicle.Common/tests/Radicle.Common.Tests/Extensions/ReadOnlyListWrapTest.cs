namespace Radicle.Common.Extensions;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

public class ReadOnlyListWrapTest
{
    [Fact]
    public void ReadOnlyListWrap_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                new ReadOnlyListWrap<string>(null!));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(34)]
    [InlineData(512)]
    public void Count_List_ReturnsExpectedResult(int expected)
    {
        List<string> list = Enumerate(expected, _ => string.Empty).ToList();

        Assert.Equal(expected, new ReadOnlyListWrap<string>(list).Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(34)]
    [InlineData(512)]
    public void Count_ReadOnlyList_ReturnsExpectedResult(int expected)
    {
        ImmutableArray<string> list = Enumerate(expected, _ => string.Empty).ToImmutableArray();

        Assert.Equal(expected, new ReadOnlyListWrap<string>(list).Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(34)]
    [InlineData(512)]
    public void Count_Enumerable_ReturnsExpectedResult(int expected)
    {
        IEnumerable<string> enumerable = Enumerate(expected, _ => string.Empty);

        Assert.Equal(expected, new ReadOnlyListWrap<string>(enumerable).Count);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1024)]
    public void Index_ListInvalidIndex_Throws(int index)
    {
        List<string> list = Enumerate(34, _ => string.Empty).ToList();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ReadOnlyListWrap<string>(list)[index]);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1024)]
    public void Index_EeadOnlyListInvalidIndex_Throws(int index)
    {
        ImmutableArray<string> readOnlyList = Enumerate(34, _ => string.Empty).ToImmutableArray();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ReadOnlyListWrap<string>(readOnlyList)[index]);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1024)]
    public void Index_EnumerableInvalidIndex_Throws(int index)
    {
        IEnumerable<string> enumerable = Enumerate(34, _ => string.Empty);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ReadOnlyListWrap<string>(enumerable)[index]);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(12, 0)]
    [InlineData(12, 6)]
    [InlineData(12, 11)]
    [InlineData(512, 42)]
    public void Index_List_ReturnsExpectedResult(
            int count,
            int index)
    {
        List<int> list = Enumerate(
                count,
                i => i == index ? 1 : 0).ToList();

        Assert.Equal(1, new ReadOnlyListWrap<int>(list)[index]);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(12, 0)]
    [InlineData(12, 6)]
    [InlineData(12, 11)]
    [InlineData(512, 42)]
    public void Index_ReadOnlyList_ReturnsExpectedResult(
            int count,
            int index)
    {
        ImmutableArray<int> readOnlyList = Enumerate(
                count,
                i => i == index ? 1 : 0).ToImmutableArray();

        Assert.Equal(1, new ReadOnlyListWrap<int>(readOnlyList)[index]);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(12, 0)]
    [InlineData(12, 6)]
    [InlineData(12, 11)]
    [InlineData(512, 42)]
    public void Index_Enumerable_ReturnsExpectedResult(
            int count,
            int index)
    {
        IEnumerable<int> enumerable = Enumerate(
                count,
                i => i == index ? 1 : 0);

        Assert.Equal(1, new ReadOnlyListWrap<int>(enumerable)[index]);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(42)]
    [InlineData(1024)]
    public void Enumeration_List_works(int count)
    {
        List<int> list = Enumerate(
                count,
                i => i).ToList();
        ReadOnlyListWrap<int> al = new(list);
        int expected = 0;

        foreach (int item in al)
        {
            Assert.Equal(expected++, item);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(42)]
    [InlineData(1024)]
    public void Enumeration_ReadOnlyList_works(int count)
    {
        ImmutableArray<int> list = Enumerate(
                count,
                i => i).ToImmutableArray();
        ReadOnlyListWrap<int> al = new(list);
        int expected = 0;

        foreach (int item in al)
        {
            Assert.Equal(expected++, item);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(42)]
    [InlineData(1024)]
    public void Enumeration_Enumerable_works(int count)
    {
        IEnumerable<int> list = Enumerate(
                count,
                i => i);
        ReadOnlyListWrap<int> al = new(list);
        int expected = 0;

        foreach (int item in al)
        {
            Assert.Equal(expected++, item);
        }
    }

    /// <summary>
    /// Create ad-hoc enumerable with requested count.
    /// </summary>
    /// <typeparam name="T">Type of the items in enumerable.</typeparam>
    /// <param name="count">Desired count.</param>
    /// <param name="valueFactory">Value factory.</param>
    /// <returns>Enumerable.</returns>
    private static IEnumerable<T> Enumerate<T>(int count, Func<int, T> valueFactory)
    {
        for (int i = 0; i < count; i++)
        {
            yield return valueFactory(i);
        }
    }
}
