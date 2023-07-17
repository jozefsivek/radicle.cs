namespace Radicle.Common.Statistics;

using System;
using System.Collections.Generic;
using Radicle.Common.Statistics.Models;
using Xunit;

public class DiscreteValueDistributionTest
{
    [Fact]
    public void Constructor_NullCollection_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                new DiscreteValueDistribution<int>(null!));
    }

    [Fact]
    public void Constructor_NullValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                new DiscreteValueDistribution<int>(new WeightedValue<int>[] { null! }));
    }

    [Fact]
    public void Constructor_NoValues_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DiscreteValueDistribution<int>(Array.Empty<WeightedValue<int>>()));
    }

    [Fact]
    public void Count_RepeatingValues_ReturnsLenght()
    {
        WeightedValue<int> v1 = new(0, 1.0);
        WeightedValue<int> v2 = new(1, 1.0);

        DiscreteValueDistribution<int> distribution = new(new[] { v1, v2, v2 });

        Assert.Equal(3, distribution.Count);
    }

    [Fact]
    public void TryRemove_NullValue_Throws()
    {
        WeightedValue<int> v1 = new(0, 1.0);

        DiscreteValueDistribution<int> distribution = new(new[] { v1 });

        Assert.Throws<ArgumentNullException>(() =>
                distribution.TryRemove(null!, out _));
    }

    [Fact]
    public void TryRemove_PresentValue_RemovesFirstOccurence()
    {
        WeightedValue<int> v1 = new(0, 1.0);
        WeightedValue<int> v2 = new(1, 1.0);

        DiscreteValueDistribution<int> distribution = new(new[] { v1, v2, v2 });

        Assert.True(distribution.TryRemove(v2, out DiscreteValueDistribution<int>? newD));

        Assert.NotNull(newD);

        Assert.Equal(2, newD.Count);
        Assert.Equal(new[] { v1, v2 }, newD);
    }

    [Fact]
    public void TryRemove_NonPresentValue_ReturnsDistribution()
    {
        WeightedValue<int> v1 = new(0, 1.0);
        WeightedValue<int> v2 = new(1, 1.0);
        WeightedValue<int> v3 = new(2, 1.0);

        DiscreteValueDistribution<int> distribution = new(new[] { v1, v2, v2 });

        Assert.True(distribution.TryRemove(v3, out DiscreteValueDistribution<int>? newD));

        Assert.NotNull(newD);

        Assert.Equal(distribution, newD);
        Assert.Equal(3, newD.Count);
        Assert.Equal(new[] { v1, v2, v2 }, newD);
    }

    [Fact]
    public void Contains_PresentValue_ReturnsTrue()
    {
        WeightedValue<int> v1 = new(0, 1.0);
        WeightedValue<int> v2 = new(1, 1.0);

        DiscreteValueDistribution<int> distribution = new(new[] { v1, v2, v2 });

        Assert.True(distribution.Contains(v1));
        Assert.True(distribution.Contains(v2));
    }

    [Fact]
    public void Contains_NonPresentValue_ReturnsTrue()
    {
        WeightedValue<int> v1 = new(0, 1.0);
        WeightedValue<int> v2 = new(1, 1.0);

        DiscreteValueDistribution<int> distribution = new(new[] { v1 });

        Assert.False(distribution.Contains(v2));
    }

    [Fact]
    public void Next_TrivialDistribution_ReturnsValue()
    {
        WeightedValue<int> v1 = new(0, 1.0);

        DiscreteValueDistribution<int> distribution = new(new[] { v1 });

        for (int i = 0; i < 4; i++)
        {
            Assert.Equal(v1, distribution.Next());
        }
    }

    [Fact]
    public void Next_NonTrivialDistribution_ReturnsDistributedValues()
    {
        WeightedValue<int> v0 = new(0, 1.0);
        WeightedValue<int> v1 = new(1, 3.0);
        WeightedValue<int> v2 = new(2, 10.0);
        WeightedValue<int> v3 = new(3, 1.0);

        WeightedValue<int>[] weights = new[] { v0, v1, v2, v3 };

        DiscreteValueDistribution<int> distribution =
                new(weights);

        const int rounds = 512;
        List<WeightedValue<int>> samples = new(rounds);

        int[] counts = new int[4];
        int[] dispersion = new int[4];

        for (int i = 0; i < rounds; i++)
        {
            WeightedValue<int> next = distribution.Next();

            samples.Add(next);

            counts[next.Value]++;
        }

        Assert.True(counts[2] > counts[1]);
        Assert.True(counts[1] > counts[0]);
        Assert.True(counts[1] > counts[3]);

        for (int i = 0; i < 4; i++)
        {
            dispersion[i] = samples.LastIndexOf(weights[i]) - samples.IndexOf(weights[i]);
        }

        Assert.True(dispersion[1] > (256 * 3 / 15));
        Assert.True(dispersion[2] > (256 * 10 / 15));
    }
}
