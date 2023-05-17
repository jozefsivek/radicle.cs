namespace Radicle.Common;

using System;
using System.Linq;
using Xunit;

public class RollingCollectionTest
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Count_ReflectsAmount(int expectedCount)
    {
        RollingCollection<int> roller = new(4);

        for (int i = 0; i < expectedCount; i++)
        {
            roller.Add(i);
        }

        Assert.Equal(Math.Clamp(expectedCount, 0, 4), roller.Count);
    }

    [Fact]
    public void IsReadOnly_ReturnsFalse()
    {
        Assert.False(new RollingCollection<int>(0).IsReadOnly);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(6)]
    public void Clear_ClearsInstance(int count)
    {
        RollingCollection<int> roller = new(4);

        for (int i = 0; i < count; i++)
        {
            roller.Add(i);
        }

        roller.Clear();

        Assert.Empty(roller);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(3, true)]
    [InlineData(5, false)]
    [InlineData(-2, false)]
    public void Contains_ValidValue_Works(int item, bool expected)
    {
        RollingCollection<int> roller = new(4);

        for (int i = 0; i < 4; i++)
        {
            roller.Add(i);
        }

        Assert.Equal(expected, roller.Contains(item));
    }

    [Fact]
    public void CopyTo_NullArray_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                new RollingCollection<int>(0).CopyTo(null!, 0));
    }

    [Fact]
    public void CopyTo_NegativeIndex_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                new RollingCollection<int>(0).CopyTo(Array.Empty<int>(), -1));
    }

    [Fact]
    public void CopyTo_SmallDestination_Throws()
    {
        RollingCollection<int> roller = new(4);

        for (int i = 0; i < 4; i++)
        {
            roller.Add(i);
        }

        Assert.Throws<ArgumentException>(() =>
                roller.CopyTo(Array.Empty<int>(), 0));
    }

    [Theory]
    [InlineData(0, new int[] { -1, -1, -1, -1 })]
    [InlineData(1, new int[] { 0, -1, -1, -1 })]
    [InlineData(4, new int[] { 0, 1, 2, 3 })]
    [InlineData(5, new int[] { 1, 2, 3, 4 })]
    public void CopyTo_SameSizedArray_Works(int count, int[] expected)
    {
        RollingCollection<int> roller = new(4);

        for (int i = 0; i < count; i++)
        {
            roller.Add(i);
        }

        int[] destination = new int[] { -1, -1, -1, -1 };

        roller.CopyTo(destination, 0);

        Assert.Equal(expected, destination);
    }

    [Theory]
    [InlineData(0, new int[] { -1, -1, -1, -1, -1, -1 })]
    [InlineData(1, new int[] { -1, -1, 0, -1, -1, -1 })]
    [InlineData(4, new int[] { -1, -1, 0, 1, 2, 3 })]
    [InlineData(5, new int[] { -1, -1, 1, 2, 3, 4 })]
    public void CopyTo_LargeArray_Works(int count, int[] expected)
    {
        RollingCollection<int> roller = new(4);

        for (int i = 0; i < count; i++)
        {
            roller.Add(i);
        }

        int[] destination = new int[] { -1, -1, -1, -1, -1, -1 };

        roller.CopyTo(destination, 2);

        Assert.Equal(expected, destination);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(10)]
    public void Enumeration_Works(int added)
    {
        RollingCollection<int> roller = new(4);

        for (int i = 0; i < added; i++)
        {
            roller.Add(i);
        }

        int[] expected = Enumerable
                .Range(Math.Max(added - 4, 0), Math.Min(4, added))
                .ToArray();

        Assert.Equal(expected, roller);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Remove_ExistingItem_ReturnsTrue(int removed)
    {
        RollingCollection<int> roller = new(4);
        int[] expected = new int[3];
        int index = 0;

        for (int i = 0; i < 4; i++)
        {
            roller.Add(i);

            if (i != removed)
            {
                expected[index++] = i;
            }
        }

        Assert.Equal(4, roller.Count);

        Assert.True(roller.Remove(removed));

        Assert.Equal(3, roller.Count);

        Assert.Equal(expected, roller);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(10)]
    public void Remove_NonExistingItem_ReturnsTrue(int removed)
    {
        RollingCollection<int> roller = new(4);
        int[] expected = new int[4];
        int index = 0;

        for (int i = 0; i < 4; i++)
        {
            roller.Add(i);

            expected[index++] = i;
        }

        Assert.Equal(4, roller.Count);

        Assert.False(roller.Remove(removed));

        Assert.Equal(4, roller.Count);

        Assert.Equal(expected, roller);
    }

    [Fact]
    public void TryGetLastValue_EmptyCollection_ReturnsFalse()
    {
        Assert.False(new RollingCollection<int>(1).TryGetLast(out _));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(5)]
    public void TryGetLastValue_Collection_ReturnsTrue(int added)
    {
        RollingCollection<int> roller = new(4);

        for (int i = 0; i < added; i++)
        {
            roller.Add(i);
        }

        Assert.True(roller.TryGetLast(out int last));
        Assert.Equal(added - 1, last);
    }
}
