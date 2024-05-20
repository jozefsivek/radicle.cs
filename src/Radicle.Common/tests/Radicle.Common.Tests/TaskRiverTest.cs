namespace Radicle.Common;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class TaskRiverTest
{
    [Fact]
    public async Task TaskRiver_KnownCount_Works()
    {
        ConcurrentBag<int> completed = new();
        HashSet<int> expected = new() { 1, 2, 3, 4, 5 };
        TaskRiver river = new(
                maximumCapacity: 2,
                fullCallCount: expected.Count);

        foreach (int number in expected)
        {
            bool full = river.Enqueue(Task.Run(async () =>
            {
                await Task.Yield();

                completed.Add(number);
            }));

            if (full)
            {
                await river.FlushAsync();
            }
        }

        Assert.True(expected.SetEquals(completed));
        Assert.True(river.IsEmpty);
    }

    [Fact]
    public async Task TaskRiver_UnknownCount_Works()
    {
        ConcurrentBag<int> completed = new();
        HashSet<int> expected = new() { 1, 2, 3, 4, 5 };
        TaskRiver river = new(
                maximumCapacity: 2);

        foreach (int number in expected)
        {
            bool full = river.Enqueue(Task.Run(async () =>
            {
                await Task.Yield();

                completed.Add(number);
            }));

            if (full)
            {
                await river.FlushAsync();
            }
        }

        // at this point the river can be empty some times
        await river.FlushAllAsync();

        Assert.True(expected.SetEquals(completed));
        Assert.True(river.IsEmpty);
    }

    [Fact]
    public async Task FlushAsync_SingleTask_AwaitsIt()
    {
        TaskRiver river = new(maximumCapacity: 2);
        ConcurrentBag<int> completed = new();
        HashSet<int> expected = new() { 1 };

        river.Enqueue(Task.Run(async () =>
        {
            await Task.Delay(1).ConfigureAwait(false);

            completed.Add(1);
        }));

        Assert.False(river.IsEmpty);

        await river.FlushAsync();

        Assert.True(expected.SetEquals(completed));
        Assert.True(river.IsEmpty);
    }

    [Fact]
    public async Task FlushAsync_MultipleTask_AwaitsFirstCompleted()
    {
        TaskRiver river = new(maximumCapacity: 2);
        ConcurrentBag<int> completed = new();
        HashSet<int> expected = new() { 1 };
        using SemaphoreSlim semaphore = new(0, 1);

        river.Enqueue(Task.Run(() => completed.Add(1)));

        river.Enqueue(Task.Run(async () =>
        {
            await semaphore.WaitAsync().ConfigureAwait(false);

            completed.Add(2);
        }));

        Assert.False(river.IsEmpty);

        await river.FlushAsync();

        Assert.True(expected.SetEquals(completed));
        Assert.False(river.IsEmpty);

        semaphore.Release();

        await river.FlushAsync();

        expected.Add(2);
        Assert.True(expected.SetEquals(completed));
        Assert.True(river.IsEmpty);
    }

    [Fact]
    public async Task FlushAsync_Cancelled_CancellsAndFlushesAll()
    {
        TaskRiver river = new(maximumCapacity: 2);
        using CancellationTokenSource source = new();
        int completed = 0;

        for (int i = 0; i < 2; i++)
        {
            river.Enqueue(Task.Run(async () =>
            {
                await Task.Delay(100).ConfigureAwait(false);

                _ = Interlocked.Increment(ref completed);
            }));
        }

        Assert.False(river.IsEmpty);
        Assert.True(river.IsFull);

        source.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                    await river.FlushAsync(cancellationToken: source.Token).ConfigureAwait(false));

        Assert.True(completed < 2);

        Assert.True(river.IsEmpty);
        Assert.False(river.IsFull);
    }

    [Fact]
    public async Task FlushAsync_FailedTask_FlushesAll()
    {
        TaskRiver river = new(maximumCapacity: 2);

        for (int i = 0; i < 2; i++)
        {
            river.Enqueue(Task.Run(() => throw new ArgumentException()));
        }

        Assert.False(river.IsEmpty);
        Assert.True(river.IsFull);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
                    await river.FlushAsync().ConfigureAwait(false));

        Assert.True(river.IsEmpty);
        Assert.False(river.IsFull);
    }

    [Fact]
    public async Task FlushAsync_CancelledTask_FlushesAll()
    {
        TaskRiver river = new(maximumCapacity: 2);
        using CancellationTokenSource source = new();

        source.Cancel();

        for (int i = 0; i < 2; i++)
        {
            river.Enqueue(Task.Run(() => source.Token.ThrowIfCancellationRequested()));
        }

        Assert.False(river.IsEmpty);
        Assert.True(river.IsFull);

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                    await river.FlushAsync().ConfigureAwait(false));

        Assert.True(river.IsEmpty);
        Assert.False(river.IsFull);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task FlushAllAsync_Flushes(int enqueued)
    {
        TaskRiver river = new(maximumCapacity: 2);
        ConcurrentBag<int> completed = new();
        HashSet<int> expected = new();

        for (int i = 0; i < enqueued; i++)
        {
            int number = i + 1;

            expected.Add(number);

            river.Enqueue(Task.Run(() => completed.Add(number)));
        }

        Assert.False(river.IsEmpty);

        await river.FlushAllAsync();

        Assert.True(expected.SetEquals(completed));
        Assert.True(river.IsEmpty);
    }

    [Fact]
    public async Task FlushAllAsync_Cancelled_CancellsAndFlushes()
    {
        TaskRiver river = new(maximumCapacity: 2);
        using CancellationTokenSource source = new();
        int completed = 0;

        for (int i = 0; i < 2; i++)
        {
            river.Enqueue(Task.Run(async () =>
            {
                await Task.Delay(100).ConfigureAwait(false);

                _ = Interlocked.Increment(ref completed);
            }));
        }

        Assert.False(river.IsEmpty);
        Assert.True(river.IsFull);

        source.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                    await river.FlushAllAsync(cancellationToken: source.Token).ConfigureAwait(false));

        Assert.True(completed < 2);

        Assert.True(river.IsEmpty);
        Assert.False(river.IsFull);
    }

    [Fact]
    public async Task FlushAllAsync_FailedTask_FlushesAll()
    {
        TaskRiver river = new(maximumCapacity: 2);

        for (int i = 0; i < 2; i++)
        {
            river.Enqueue(Task.Run(() => throw new ArgumentException()));
        }

        Assert.False(river.IsEmpty);
        Assert.True(river.IsFull);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
                    await river.FlushAllAsync().ConfigureAwait(false));

        Assert.True(river.IsEmpty);
        Assert.False(river.IsFull);
    }

    [Fact]
    public async Task FlushAllAsync_CancelledTask_FlushesAll()
    {
        TaskRiver river = new(maximumCapacity: 2);
        using CancellationTokenSource source = new();

        source.Cancel();

        for (int i = 0; i < 2; i++)
        {
            river.Enqueue(Task.Run(() => source.Token.ThrowIfCancellationRequested()));
        }

        Assert.False(river.IsEmpty);
        Assert.True(river.IsFull);

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                    await river.FlushAllAsync().ConfigureAwait(false));

        Assert.True(river.IsEmpty);
        Assert.False(river.IsFull);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(13)]
    public async Task Enqueue_FlushAsync_Works(int rounds)
    {
        TaskRiver river = new(
                maximumCapacity: 2,
                fullCallCount: rounds);
        int completed = 0;

        for (int i = 0; i < rounds; i++)
        {
            if (river.Enqueue(Task.Run(async () =>
                {
                    await Task.Delay(1).ConfigureAwait(false);
                    _ = Interlocked.Increment(ref completed);
                })))
            {
                await river.FlushAsync();
            }
        }

        Assert.Equal(rounds, completed);
        Assert.True(river.IsEmpty);
    }
}
