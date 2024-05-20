namespace Radicle.Common.Profiling;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class CountersCollectionTest
{
    [Fact]
    public void IsEmpty_EmptyCollection_RetrunsTrue()
    {
        Assert.True(new CountersDictionary().IsEmpty);
    }

    [Fact]
    public void IsEmpty_NonEmptyCollection_RetrunsFalse()
    {
        CountersDictionary collection = new();

        collection.Incr("foo");

        Assert.False(collection.IsEmpty);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    public void Count_Collection_RetrunsCount(int length)
    {
        CountersDictionary collection = new();

        for (int i = 0; i < length; i++)
        {
            collection.Incr($"foo{i}");
        }

        Assert.Equal(length, collection.Count);
    }

    [Fact]
    public void Indexer_NullKey_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => CountersDictionary.Default[null!]);
    }

    [Fact]
    public void Indexer_NonExistingKey_Throws()
    {
        CountersDictionary collection = new();

        collection.Incr("foo");

        Assert.Throws<KeyNotFoundException>(() => collection["bar"]);
    }

    [Fact]
    public void Indexer_ExistingKey_ReturnsCounter()
    {
        CountersDictionary collection = new();

        collection.Incr("foo");
        collection.Incr("foo");

        Assert.Equal(2L, collection["foo"]);
    }

    [Fact]
    public void GetElapsed_NullKey_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => CountersDictionary.Default.GetElapsed(null!));
    }

    [Fact]
    public void GetElapsed_NonExistingKey_Throws()
    {
        CountersDictionary collection = new();

        collection.Incr("foo");

        Assert.Throws<KeyNotFoundException>(() => collection.GetElapsed("bar"));
    }

    [Fact]
    public void GetElapsed_ExistingKey_ReturnsCounter()
    {
        CountersDictionary collection = new();

        collection.Incr("foo", TimeSpan.FromSeconds(1));

        Assert.Equal(TimeSpan.FromSeconds(1), collection.GetElapsed("foo"));
    }

    [Fact]
    public void StartThread_ReturnsLinkedThread()
    {
        CountersDictionary collection = new();

        TimeThread thread = collection.StartThread();

        Assert.True(collection.IsEmpty);

        thread.Measure("foo");

        thread.Measure("bar");

        Assert.Equal(2, collection.Count);
        Assert.True(new HashSet<string>(new[] { "foo", "bar" })
                .IsSupersetOf(collection.Keys));
    }

    [Fact]
    public void Decr_NullKey_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                CountersDictionary.Default.Decr(null!));
    }

    [Theory]
    [InlineData(new long[] { 0 })]
    [InlineData(new long[] { 1, })]
    [InlineData(new long[] { 1, -1 })]
    [InlineData(new long[] { 100, 200, -50, 1024 })]
    public void Decr_ValidInput_Decrements(long[] amounts)
    {
        CountersDictionary collection = new();
        long expected = 0L;

        foreach (long d in amounts)
        {
            collection.Decr("foo", d);
            expected -= d;
        }

        Assert.Equal(expected, collection["foo"]);
    }

    [Fact]
    public void Decr_Overflow_DoesNotThrow()
    {
        CountersDictionary collection = new();

        collection.Decr("foo", long.MaxValue);
        collection.Decr("foo", 2);

        Assert.Equal(long.MaxValue, collection["foo"]);
    }

    [Fact]
    public void DecrTimeSpan_NullKey_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                CountersDictionary.Default.Decr(null!, TimeSpan.Zero));
    }

    [Theory]
    [InlineData(new long[] { 0 })]
    [InlineData(new long[] { 1, })]
    [InlineData(new long[] { 1, -1 })]
    [InlineData(new long[] { 100, 200, -50, 1024 })]
    public void DecrTimeSpan_ValidInput_Decrements(long[] amounts)
    {
        CountersDictionary collection = new();
        TimeSpan expected = TimeSpan.Zero;

        foreach (long d in amounts)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(d);
            collection.Decr("foo", t);
            expected -= t;
        }

        Assert.Equal(expected, collection.GetElapsed("foo"));
    }

    [Fact]
    public void DecrTimeSpan_Overflow_DoesNotThrow()
    {
        CountersDictionary collection = new();

        collection.Decr("foo", TimeSpan.MaxValue);
        collection.Decr("foo", TimeSpan.FromTicks(2));

        Assert.Equal(TimeSpan.MaxValue, collection.GetElapsed("foo"));
    }

    [Fact]
    public void Incr_NullKey_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                CountersDictionary.Default.Incr(null!));
    }

    [Theory]
    [InlineData(new long[] { 0 })]
    [InlineData(new long[] { 1, })]
    [InlineData(new long[] { 1, -1 })]
    [InlineData(new long[] { 100, 200, -50, 1024 })]
    public void Incr_ValidInput_Increments(long[] amounts)
    {
        CountersDictionary collection = new();
        long expected = 0L;

        foreach (long d in amounts)
        {
            collection.Incr("foo", d);
            expected += d;
        }

        Assert.Equal(expected, collection["foo"]);
    }

    [Fact]
    public void Incr_Overflow_DoesNotThrow()
    {
        CountersDictionary collection = new();

        collection.Incr("foo", long.MaxValue);
        collection.Incr("foo");

        Assert.Equal(long.MinValue, collection["foo"]);
    }

    [Fact]
    public void IncrTimeSpan_NullKey_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                CountersDictionary.Default.Incr(null!, TimeSpan.Zero));
    }

    [Theory]
    [InlineData(new long[] { 0 })]
    [InlineData(new long[] { 1, })]
    [InlineData(new long[] { 1, -1 })]
    [InlineData(new long[] { 100, 200, -50, 1024 })]
    public void IncrTimeSpan_ValidInput_Increments(long[] amounts)
    {
        CountersDictionary collection = new();
        TimeSpan expected = TimeSpan.Zero;

        foreach (long d in amounts)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(d);
            collection.Incr("foo", t);
            expected += t;
        }

        Assert.Equal(expected, collection.GetElapsed("foo"));
    }

    [Fact]
    public void IncrTimeSpan_Overflow_DoesNotThrow()
    {
        CountersDictionary collection = new();

        collection.Incr("foo", TimeSpan.MaxValue);
        collection.Incr("foo", TimeSpan.FromTicks(1));

        Assert.Equal(TimeSpan.MinValue, collection.GetElapsed("foo"));
    }

    [Fact]
    public void Set_NullKey_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                CountersDictionary.Default.Set(null!, 0L));
    }

    [Fact]
    public void Set_Counter_OverridesAndSets()
    {
        CountersDictionary collection = new();

        collection.Incr("foo", 1);
        collection.Incr("foo", -42);
        collection.Incr("foo", 10024);

        long pastValue = collection["foo"];

        collection.Set("foo", 1);

        Assert.Equal(1, collection["foo"]);
        Assert.NotEqual(pastValue, collection["foo"]);
    }

    [Fact]
    public void SetTimeSpan_NullKey_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                CountersDictionary.Default.Set(null!, TimeSpan.Zero));
    }

    [Fact]
    public void SetTimeSpan_Counter_OverridesAndSets()
    {
        CountersDictionary collection = new();

        collection.Incr("foo", TimeSpan.FromTicks(1));
        collection.Incr("foo", TimeSpan.FromTicks(-42));
        collection.Incr("foo", TimeSpan.FromTicks(10024));

        TimeSpan pastValue = collection.GetElapsed("foo");

        collection.Set("foo", TimeSpan.FromHours(1));

        Assert.Equal(TimeSpan.FromHours(1), collection.GetElapsed("foo"));
        Assert.NotEqual(pastValue, collection.GetElapsed("foo"));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(long.MaxValue)]
    [InlineData(long.MinValue)]
    public void SetTimeSpan_StoresTicks(long ticks)
    {
        CountersDictionary collection = new();

        collection.Set("foo", TimeSpan.FromTicks(ticks));

        Assert.Equal(ticks, collection["foo"]);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(long.MaxValue)]
    [InlineData(long.MinValue)]
    public void GetElapsed_NumericCounter_ReturnsExpectedTimeSpan(long ticks)
    {
        CountersDictionary collection = new();

        collection.Set("foo", ticks);

        Assert.Equal(TimeSpan.FromTicks(ticks), collection.GetElapsed("foo"));
    }

    [Fact]
    public void Clear_NonEmptyCollection_ClearsCollection()
    {
        CountersDictionary collection = new();

        collection.Set("foo", 1);

        Assert.False(collection.IsEmpty);

        collection.Clear();

        Assert.True(collection.IsEmpty);
    }

    [Fact]
    public void Clear_NonEmptyCollection_ClearsStyle()
    {
        CountersDictionary collection = new();

        collection.Set("foo", TimeSpan.MaxValue);

        collection.Clear();

        collection.Set("foo", 42);

        Assert.Equal("- foo: 42", collection.ToString());
    }

    [Fact]
    public void ToString_NumericCounters_ReturnsNumericOutput()
    {
        CountersDictionary collection = new();

        collection.Set("foo", 1);
        collection.Incr("foo", 8);
        collection.Decr("foo", 32);
        collection.Decr("bar", -32);

        string expected =
                "- bar: 32"
                + Environment.NewLine
                + "- foo: -23";

        Assert.Equal(expected, collection.ToString());
    }

    [Fact]
    public void ToString_TimeCounters_ReturnsTimeOutput()
    {
        CountersDictionary collection = new();

        collection.Set("foo", TimeSpan.FromSeconds(1));
        collection.Incr("foo", TimeSpan.FromSeconds(8));
        collection.Decr("foo", TimeSpan.FromSeconds(32));
        collection.Decr("bar", TimeSpan.FromMilliseconds(-32));
        collection.Decr("Foo", TimeSpan.FromHours(1));

        string expected =
                "- bar: 32.000000ms"
                + Environment.NewLine
                + "- foo: -00:00:23.000"
                + Environment.NewLine
                + "- Foo: -01:00:00.000";

        Assert.Equal(expected, collection.ToString());
    }

    [Fact]
    public void ToString_EmptyCollection_ReturnsEmptyLabel()
    {
        CountersDictionary collection = new();

        Assert.Equal("(empty)", collection.ToString());
    }

    [Fact]
    public void ContainsKey_ExistingKey_ReturnsTrue()
    {
        CountersDictionary collection = new();

        collection.Set("foo", 0);

        Assert.True(collection.ContainsKey("foo"));
    }

    [Fact]
    public void ContainsKey_NonExistingKey_ReturnsFalse()
    {
        CountersDictionary collection = new();

        Assert.False(collection.ContainsKey("foo"));
    }

    [Fact]
    public void TryGetValue_NullKey_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                CountersDictionary.Default.TryGetValue(null!, out _));
    }

    [Fact]
    public void TryGetValue_NonExistingKey_ReturnsFalse()
    {
        CountersDictionary collection = new();

        collection.Incr("foo");

        Assert.False(collection.TryGetValue("bar", out _));
    }

    [Fact]
    public void TryGetValue_ExistingKey_ReturnsCounter()
    {
        CountersDictionary collection = new();

        collection.Incr("foo");
        collection.Incr("foo");

        Assert.True(collection.TryGetValue("foo", out long value));
        Assert.Equal(2L, value);
    }

    [Fact]
    public void TryGetElapsed_NullKey_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                CountersDictionary.Default.TryGetElapsed(null!, out _));
    }

    [Fact]
    public void TryGetElapsed_NonExistingKey_ReturnsFalse()
    {
        CountersDictionary collection = new();

        collection.Incr("foo");

        Assert.False(collection.TryGetElapsed("bar", out _));
    }

    [Fact]
    public void TryGetElapsed_ExistingKey_ReturnsCounter()
    {
        CountersDictionary collection = new();

        collection.Incr("foo", TimeSpan.FromSeconds(1));

        Assert.True(collection.TryGetElapsed("foo", out TimeSpan time));
        Assert.Equal(TimeSpan.FromSeconds(1), time);
    }

    [Fact]
    public void EnumerateElapsed_ReturnsCouters()
    {
        CountersDictionary collection = new();

        collection.Set("foo", TimeSpan.Zero);
        collection.Set("bar", TimeSpan.MinValue);

        Assert.Equal(2, collection.Count);
        Assert.True(new HashSet<string>(new[] { "foo", "bar" })
                .IsSupersetOf(collection.EnumerateElapsed().Select(i => i.Key)));
    }

    [Fact]
    public async Task IncrDecr_ConcurentUse_Works()
    {
        CountersDictionary collection = new();
        const int tasksCount = 4;
        const int incrs = 256;
        const int decrs = 128;
        using SemaphoreSlim semaphore = new(0, tasksCount);
        Task[] tasks = new Task[tasksCount];

        for (int i = 0; i < tasksCount; i++)
        {
            tasks[i] = Task.Run(() =>
                    UseCounter(collection, semaphore, "foo", incrs, decrs));
        }

        semaphore.Release(4);

        await Task.WhenAll(tasks);

        Assert.Equal(tasksCount * (incrs - decrs), collection["foo"]);
    }

    private static void UseCounter(
            CountersDictionary collection,
            SemaphoreSlim semaphore,
            string key,
            int increments,
            int decremets)
    {
        semaphore.Wait();

        for (int i = 0; i < increments; i++)
        {
            collection.Incr(key);
        }

        for (int i = 0; i < decremets; i++)
        {
            collection.Decr(key);
        }

        semaphore.Release();
    }
}
