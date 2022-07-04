namespace Radicle.Common.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class EnumerableExtensionsTest
{
    private readonly IEnumerable<string> nullEnumerable = default!;

    private readonly IEnumerable<string> emptyEnumerable = Array.Empty<string>();

    private readonly IList<string?> nonEmptyList = new List<string?>()
    {
        null,
        string.Empty,
        null,
        " ",
        null,
        "\t",
        "foo",
        "\t",
        null,
    };

    private readonly IEnumerable<string?> nonEmptyEnumerable = new[]
    {
        null,
        string.Empty,
        null,
        " ",
        null,
        "\t",
        "foo",
        "\t",
        null,
    };

    [Fact]
    public void Extend_NullInputDestination_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ((List<string>)null!).Extend(Array.Empty<string>()));
    }

    [Fact]
    public void Extend_NullInputSource_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new List<string>().Extend(null!));
    }

    [Fact]
    public void ExtendIfAny_NullInputDestination_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ((List<string>)null!).ExtendIfAny(Array.Empty<string>()));
    }

    [Fact]
    public void ExtendIfAny_NullInputSource_DoesNotThrow()
    {
        Assert.NotNull(new List<string>().ExtendIfAny(null));
    }

    [Fact]
    public void Extend_EmptySource_PreservesDestination()
    {
        int[] original = new[] { 3, 14, 15, 9, 26, 535 };
        int i = 0;

        // extend with empty collection
        foreach (int item in original.ToList().Extend(Array.Empty<int>()))
        {
            Assert.Equal(original[i++], item);
        }
    }

    [Fact]
    public void Extend_NonTrivialInput_Extends()
    {
        int[] original = new[] { 3, 14, 15, 9, 26, 535 };
        int[] extension = new[] { 8, 979, 3 };
        int[] expected = new[] { 3, 14, 15, 9, 26, 535, 8, 979, 3 };
        int i = 0;

        // extend non empty collection
        foreach (int item in original.ToList().Extend(extension))
        {
            Assert.Equal(expected[i++], item);
        }
    }

    [Fact]
    public void Extend_EmptyInputDestination_Extends()
    {
        int[] extension = new[] { 8, 979, 3 };
        int i = 0;

        // extend empty collection
        foreach (int item in new List<int>().Extend(extension))
        {
            Assert.Equal(extension[i++], item);
        }
    }

    [Fact]
    public void Extend_SetLikeInputDestination_Extends()
    {
        int[] original = new[] { 3, 14, 15, 9, 26, 535 };
        int[] extension = new[] { 8, 979, 3 };
        int[] expected = new[] { 3, 14, 15, 9, 26, 535, 8, 979 };

        // extend set
        Assert.True(new HashSet<int>(expected)
                .SetEquals(
                    new HashSet<int>(original).Extend(extension)));
    }

    [Fact]
    public void ExtendIfAny_EmptySource_PreservesDestination()
    {
        int[] original = new[] { 3, 14, 15, 9, 26, 535 };
        int i = 0;

        // extend with empty collection
        foreach (int item in original.ToList().ExtendIfAny(Array.Empty<int>()))
        {
            Assert.Equal(original[i++], item);
        }
    }

    [Fact]
    public void ExtendIfAny_NonTrivialInput_Extends()
    {
        int[] original = new[] { 3, 14, 15, 9, 26, 535 };
        int[] extension = new[] { 8, 979, 3 };
        int[] expected = new[] { 3, 14, 15, 9, 26, 535, 8, 979, 3 };
        int i = 0;

        // extend non empty collection
        foreach (int item in original.ToList().ExtendIfAny(extension))
        {
            Assert.Equal(expected[i++], item);
        }
    }

    [Fact]
    public void ExtendIfAny_EmptyInputDestination_Extends()
    {
        int[] extension = new[] { 8, 979, 3 };
        int i = 0;

        // extend empty collection
        foreach (int item in new List<int>().ExtendIfAny(extension))
        {
            Assert.Equal(extension[i++], item);
        }
    }

    [Fact]
    public void ExtendIfAny_SetLikeInputDestination_Extends()
    {
        int[] original = new[] { 3, 14, 15, 9, 26, 535 };
        int[] extension = new[] { 8, 979, 3 };
        int[] expected = new[] { 3, 14, 15, 9, 26, 535, 8, 979 };

        // extend set
        Assert.True(new HashSet<int>(expected).SetEquals(new HashSet<int>(original).ExtendIfAny(extension)));
    }

    [Fact]
    public void ArrayBatch_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(
                () => this.nullEnumerable.ArrayBatch(10));
    }

    [Fact]
    public void ArrayBatch_ZeroInput_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
                () => this.nonEmptyEnumerable.ArrayBatch(0));
    }

    [Fact]
    public void ArrayBatch_NegativeInput_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
                () => this.nonEmptyEnumerable.ArrayBatch(-3));
    }

    /// <summary>
    /// Mind ArrayBatch is only proxy for Batch.
    /// </summary>
    [Fact]
    public void ArrayBatch_NonTrivialInput_Works()
    {
        int[] source = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        int c = 0;

        foreach (int[] batch in source.ArrayBatch(3))
        {
            Assert.IsAssignableFrom<int[]>(batch);
            Assert.Equal(3, batch.Length);

            Assert.Equal(c + 1, batch[0]);
            Assert.Equal(c + 2, batch[1]);
            Assert.Equal(c + 3, batch[2]);

            c += 3;
        }
    }

    [Fact]
    public void Batch_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(
                () => this.nullEnumerable.Batch(10));
    }

    [Fact]
    public void Batch_ZeroInput_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
                () => this.nonEmptyEnumerable.Batch(0));
    }

    [Fact]
    public void Batch_NegativeInput_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
                () => this.nonEmptyEnumerable.Batch(-3));
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new[] { 3, 3, 3 }, 3)]
    [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new[] { 4, 4, 1 }, 4)]
    [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new[] { 9 }, 9)]
    [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new[] { 9 }, 10)]
    [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new[] { 9 }, 11)]
    [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new[] { 9 }, 12)]
    [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new[] { 9 }, 13)]
    [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new[] { 9 }, 15)]
    [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new[] { 9 }, 16)]
    [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new[] { 9 }, 17)]
    [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new[] { 9 }, 256)]
    public void Batch_NonTrivialInput_Works(
            int[] source,
            int[] expectedBatchSizes,
            int batchSize)
    {
        int c = 0;
        int batchIndex = 0;

        foreach (IEnumerable<int> batch in source.Batch(batchSize))
        {
            int i = 0;

            foreach (int item in batch)
            {
                c++;
                i++;

                Assert.Equal(c, item);
            }

            Assert.Equal(expectedBatchSizes[batchIndex], i);

            batchIndex++;
        }

        Assert.Equal(source.Length, c);
    }

    [Fact]
    public void BatchWithGetter_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => this.nullEnumerable.Batch(() => 10));
    }

    [Fact]
    public void BatchWithGetter_NullGetter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => this.nonEmptyEnumerable.Batch(null!));
    }

    [Fact]
    public void BatchWithGetter_OutOfBoundsBatchSize_IsClipped()
    {
        int[] source = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        int c = 0;
        int b = 0;

        foreach (IEnumerable<int> batch in source.Batch(() => b--))
        {
            Assert.IsAssignableFrom<int[]>(batch);
            Assert.Single(batch);

            foreach (int item in batch)
            {
                Assert.Equal(1 + c++, item);
            }
        }
    }

    [Fact]
    public void BatchWithGetter_VariableBatchSizes_Works()
    {
        int[] source = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        int[] batchSizes = new[] { 3, 2, 1, 4 };
        int c = 0;
        int b = 0;

        foreach (IEnumerable<int> batch in source.Batch(() => batchSizes[b]))
        {
            Assert.IsAssignableFrom<int[]>(batch);
            Assert.Equal(batchSizes[b], batch.Count());

            b++;

            foreach (int item in batch)
            {
                Assert.Equal(1 + c++, item);
            }
        }
    }

    [Fact]
    public void BatchWithGetter_VariableBatchSizesWithOutOfBounds_Works()
    {
        int[] source = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        int[] batchSizes = new[] { 1, 3, 5, 128 };
        int c = 0;
        int b = 0;

        foreach (IEnumerable<int> batch in source.Batch(() => batchSizes[b]))
        {
            Assert.IsAssignableFrom<int[]>(batch);
            Assert.Equal(Math.Min(batchSizes[b], source.Length - c), batch.Count());

            b++;

            foreach (int item in batch)
            {
                Assert.Equal(1 + c++, item);
            }
        }
    }

    [Fact]
    public void BatchWithGetter_DynamicallySetBatchSizes_Works()
    {
        int[] source = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        int currentBatchSize = 4;
        int c = 0;

        // and one with getter changed from the loop
        foreach (IEnumerable<int> batch in source.Batch(() => currentBatchSize))
        {
            Assert.IsAssignableFrom<int[]>(batch);

            if (c == 0)
            {
                Assert.Equal(4, batch.Count());
            }
            else
            {
                Assert.Equal(2, batch.Count());
            }

            foreach (int item in batch)
            {
                Assert.Equal(1 + c++, item);

                // this should be ignored by batching
                currentBatchSize = 100;
            }

            // this will be applied on the next step
            currentBatchSize = 2;
        }
    }

    [Fact]
    public void Sub_NullInputSource_Throws()
    {
        Assert.Throws<ArgumentNullException>(
                () => this.nullEnumerable.Sub(this.emptyEnumerable));
    }

    [Fact]
    public void Sub_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(
                () => this.emptyEnumerable.Sub(this.nullEnumerable));
    }

    [Fact]
    public void Sub_EmptyInputOrSource_Works()
    {
        Assert.Empty(this.emptyEnumerable.Sub(this.nonEmptyEnumerable));
        Assert.Empty(this.emptyEnumerable.Sub(this.emptyEnumerable));
    }

    [Fact]
    public void Sub_EmptyInput_PreservesSource()
    {
        int[] source = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        int c = 0;

        foreach (int item in source.Sub(Array.Empty<int>()))
        {
            Assert.Equal(source[c++], item);
        }
    }

    [Fact]
    public void Sub_InputWithRepeats_RemovesFirstItemOccurence()
    {
        object[] source = new object[]
        {
            new object(), // 0
            new object(), // 1
            string.Empty, // 2
            new object(), // 3
            3, // 4
            3, // 5
            3, // 6
            TimeSpan.FromDays(3.3), // 7
            TimeSpan.FromHours(1), // 8
            TimeSpan.FromDays(3.3), // 9
            TimeSpan.FromHours(1), // 10
            string.Empty, // 11
        };
        object[] subtraction = new object[] { string.Empty, source[3], TimeSpan.FromHours(1), source[0], 3, 3 };
        object[] expected = new object[]
        {
            source[1],
            3,
            source[7],
            TimeSpan.FromDays(3.3),
            TimeSpan.FromHours(1),
            string.Empty,
        };
        int c = 0;

        foreach (object item in source.Sub(subtraction))
        {
            object expectedItem = expected[c++];

            if (expectedItem.GetType().IsByRef)
            {
                Assert.True(ReferenceEquals(expectedItem, item));
            }
            else
            {
                Assert.Equal(expectedItem, item);
            }
        }
    }

    [Fact]
    public void Mul_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => this.nullEnumerable.Mul(3));
    }

    [Fact]
    public void Mul_EmptyInput_Yields()
    {
        Assert.Empty(this.emptyEnumerable.Mul(13));
        Assert.Empty(this.emptyEnumerable.Mul(13, this.emptyEnumerable));
    }

    [Fact]
    public void Mul_ZeroRepeats_Yields()
    {
        Assert.Empty(this.emptyEnumerable.Mul(0));
        Assert.Empty(this.nonEmptyEnumerable.Mul(0, new[] { "a", "b" }));
    }

    [Fact]
    public void Mul_NegativeRpeats_Yields()
    {
        Assert.Empty(this.nonEmptyEnumerable.Mul(-13));
        Assert.Empty(this.emptyEnumerable.Mul(-3, new[] { "a", "b" }));
    }

    [Theory]
    [InlineData(new[] { 3, 14159, 265358, 97, 93 }, 1, new[] { 44 }, new[] { 3, 14159, 265358, 97, 93 })]
    [InlineData(new[] { 3, 14159, 265358, 97, 93 }, 1, null, new[] { 3, 14159, 265358, 97, 93 })]
    [InlineData(new[] { 97, 93 }, 3, null, new[] { 97, 93, 97, 93, 97, 93 })]
    [InlineData(new[] { 3 }, 4, new[] { 44 }, new[] { 3, 44, 3, 44, 3, 44, 3 })]
    [InlineData(new[] { 14159, 265358 }, 2, new[] { 97, 93 }, new[] { 14159, 265358, 97, 93, 14159, 265358 })]
    public void Mul_NonTrivialInput_Works(
            IEnumerable<int> source,
            int multiplier,
            IEnumerable<int> delimiter,
            IEnumerable<int> expected)
    {
        IEnumerable<(int First, int Second)> s = source
                .Mul(multiplier, delimiter).Zip(expected);

        Assert.True(s.All(item => item.First == item.Second));
        Assert.Equal(expected.Count(), s.Count());
    }

    [Fact]
    public void RandomPick_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(
                () => this.nullEnumerable.RandomPick().ToArray());
    }

    [Fact]
    public void RandomPick_NegativeCount_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
                () => this.nonEmptyList.RandomPick(count: -1).ToArray());
    }

    [Fact]
    public void RandomPick_NegativeMin_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
                () => this.nonEmptyList.RandomPick(min: -1).ToArray());
    }

    [Fact]
    public void RandomPick_EmptyInput_DoesNotThrow()
    {
        Assert.NotNull(this.emptyEnumerable.RandomPick());
    }

    [Fact]
    public void RandomPick_EmptyInput_Yields()
    {
        Assert.Empty(this.emptyEnumerable.RandomPick());
    }

    [Fact]
    public void RandomPick_EmptyInputZeroCount_Yields()
    {
        Assert.Empty(this.emptyEnumerable.RandomPick(count: 0));
    }

    [Fact]
    public void RandomPick_ZeroCount_Yields()
    {
        Assert.Empty(this.nonEmptyEnumerable.RandomPick(count: 0));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(7)]
    [InlineData(42)]
    public void RandomPick_FixedCount_Works(int count)
    {
        IEnumerable<string?> result = this.nonEmptyEnumerable
                .RandomPick(count: count);

        Assert.Equal(count, result.Count());
        Assert.Empty(result.Except(this.nonEmptyEnumerable));
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(10, 1)]
    [InlineData(10, 10)]
    [InlineData(42, 24)]
    public void RandomPick_FuzzyCount_Works(int count, int min)
    {
        IEnumerable<string?> result = this.nonEmptyEnumerable
                .RandomPick(count: count, fuzzy: true, min: min);

        int resultCount = result.Count();

        Assert.True(min <= resultCount && resultCount <= count);
        Assert.Empty(result.Except(this.nonEmptyEnumerable));
    }
}
