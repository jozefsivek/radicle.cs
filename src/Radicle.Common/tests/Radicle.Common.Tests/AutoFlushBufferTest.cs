namespace Radicle.Common;

using System;
using System.Threading.Tasks;
using Xunit;

public class AutoFlushBufferTest
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Constructor_InvalidCapacity_Throws(int invalidCapacity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                new AutoFlushBuffer<int, int>(
                    invalidCapacity,
                    0,
                    (int _, int _) => ValueTask.FromResult(true),
                    (int _) => ValueTask.CompletedTask));
    }

    [Fact]
    public void Constructor_NullAggregate_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                new AutoFlushBuffer<object, int>(
                    1,
                    null!,
                    (object _, int _) => ValueTask.FromResult(true),
                    (object _) => ValueTask.CompletedTask));
    }

    [Fact]
    public void Constructor_NullAggregator_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                new AutoFlushBuffer<int, int>(
                    1,
                    0,
                    null!,
                    (int _) => ValueTask.CompletedTask));
    }

    [Fact]
    public void Constructor_NullFlusher_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                new AutoFlushBuffer<int, int>(
                    1,
                    0,
                    (int _, int _) => ValueTask.FromResult(true),
                    null!));
    }

    [Fact]
    public async Task EnqueueAsync_NullItem_Throws()
    {
        AutoFlushBuffer<object, object> buffer = new(
                1,
                this,
                (object _, object _) => ValueTask.FromResult(true),
                (object _) => ValueTask.CompletedTask);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await buffer.EnqueueAsync(null!).ConfigureAwait(false)).ConfigureAwait(false);
    }

    [Fact]
    public async Task EnqueueAsync_BellowCapacity_DoesNotFlush()
    {
        bool flushed = false;
        AutoFlushBuffer<int, int> buffer = new(
                10,
                0,
                (int _, int _) => ValueTask.FromResult(true),
                (int _) =>
                {
                    flushed = true;
                    return ValueTask.CompletedTask;
                });

        await buffer.EnqueueAsync(1).ConfigureAwait(false);

        Assert.False(flushed);
    }

    [Fact]
    public async Task EnqueueAsync_ReachedCapacity_Flushes()
    {
        bool flushed = false;
        AutoFlushBuffer<int, int> buffer = new(
                2,
                0,
                (int _, int _) => ValueTask.FromResult(true),
                (int _) =>
                {
                    flushed = true;
                    return ValueTask.CompletedTask;
                });

        await buffer.EnqueueAsync(1).ConfigureAwait(false);
        await buffer.EnqueueAsync(1).ConfigureAwait(false);

        Assert.True(flushed);
    }

    [Fact]
    public async Task EnqueueAsync_MultipleCapacity_Flushes()
    {
        int flushed = 0;
        AutoFlushBuffer<int, int> buffer = new(
                2,
                0,
                (int _, int _) => ValueTask.FromResult(true),
                (int _) =>
                {
                    flushed++;
                    return ValueTask.CompletedTask;
                });

        for (int i = 0; i < 10; i++)
        {
            await buffer.EnqueueAsync(1).ConfigureAwait(false);
        }

        Assert.Equal(10 / buffer.Capacity, flushed);
    }

    [Fact]
    public async Task EnqueueAsync_ReachedCapacitySkippedItem_Flushes()
    {
        bool flushed = false;
        AutoFlushBuffer<int, int> buffer = new(
                2,
                0,
                (int _, int item) =>
                {
                    return item == 0
                            ? ValueTask.FromResult(false)
                            : ValueTask.FromResult(true);
                },
                (int _) =>
                {
                    flushed = true;
                    return ValueTask.CompletedTask;
                });

        await buffer.EnqueueAsync(1).ConfigureAwait(false);
        await buffer.EnqueueAsync(0).ConfigureAwait(false);

        Assert.False(flushed);

        await buffer.EnqueueAsync(1).ConfigureAwait(false);

        Assert.True(flushed);
    }

    [Fact]
    public async Task EnqueueAndFlushAsync_NullInput_Throws()
    {
        AutoFlushBuffer<int, int> buffer = new(
                10,
                0,
                (int _, int _) => ValueTask.FromResult(true),
                (int _) => ValueTask.CompletedTask);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await buffer.EnqueueAndFlushAsync(null!).ConfigureAwait(false)).ConfigureAwait(false);
    }

    [Fact]
    public async Task EnqueueAndFlushAsync_NullItem_Throws()
    {
        AutoFlushBuffer<int, object> buffer = new(
                10,
                0,
                (int _, object _) => ValueTask.FromResult(true),
                (int _) => ValueTask.CompletedTask);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await buffer.EnqueueAndFlushAsync(new object[] { null! })
                    .ConfigureAwait(false)).ConfigureAwait(false);
    }

    [Fact]
    public async Task EnqueueAndFlushAsync_BellowCapacity_Flushes()
    {
        bool flushed = false;
        AutoFlushBuffer<int, int> buffer = new(
                10,
                0,
                (int _, int _) => ValueTask.FromResult(true),
                (int _) =>
                {
                    flushed = true;
                    return ValueTask.CompletedTask;
                });

        await buffer.EnqueueAndFlushAsync(new int[] { 1, 1 }).ConfigureAwait(false);

        Assert.True(flushed);
    }

    [Fact]
    public async Task FlushAsync_Flushes()
    {
        bool flushed = false;
        AutoFlushBuffer<int, int> buffer = new(
                10,
                0,
                (int _, int _) => ValueTask.FromResult(true),
                (int _) =>
                {
                    flushed = true;
                    return ValueTask.CompletedTask;
                });

        await buffer.EnqueueAsync(1).ConfigureAwait(false);
        await buffer.EnqueueAsync(1).ConfigureAwait(false);
        await buffer.EnqueueAsync(1).ConfigureAwait(false);
        await buffer.EnqueueAsync(1).ConfigureAwait(false);

        Assert.False(flushed);

        await buffer.FlushAsync().ConfigureAwait(false);

        Assert.True(flushed);
    }

    [Fact]
    public async Task FlushAsync_MultipleCalls_Flushes()
    {
        int flushed = 0;
        AutoFlushBuffer<int, int> buffer = new(
                10,
                0,
                (int _, int _) => ValueTask.FromResult(true),
                (int _) =>
                {
                    flushed++;
                    return ValueTask.CompletedTask;
                });

        await buffer.EnqueueAsync(1).ConfigureAwait(false);
        await buffer.EnqueueAsync(1).ConfigureAwait(false);

        Assert.Equal(0, flushed);

        await buffer.FlushAsync().ConfigureAwait(false);

        Assert.Equal(1, flushed);

        await buffer.FlushAsync().ConfigureAwait(false);

        Assert.Equal(2, flushed);
    }
}
