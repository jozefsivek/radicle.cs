namespace Radicle.Common;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class TaskBufferTest
{
    [Fact]
    public async Task TaskBuffer_KnownCount_Works()
    {
        ConcurrentBag<int> completed = new();
        HashSet<int> expected = new() { 1, 2, 3, 4, 5 };
        TaskBuffer buffer = new(
                maximumCapacity: 2,
                fullCallCount: expected.Count);

        foreach (int number in expected)
        {
            bool full = buffer.Enqueue(Task.Run(async () =>
            {
                await Task.Yield();

                completed.Add(number);
            }));

            if (full)
            {
                await buffer.FlushAsync().ConfigureAwait(false);
            }
        }

        Assert.True(expected.SetEquals(completed));
        Assert.True(buffer.IsEmpty);
    }

    [Fact]
    public async Task TaskBuffer_UnknownCount_Works()
    {
        ConcurrentBag<int> completed = new();
        HashSet<int> expected = new() { 1, 2, 3, 4, 5 };
        TaskBuffer buffer = new(
                maximumCapacity: 2);

        foreach (int number in expected)
        {
            bool full = buffer.Enqueue(Task.Run(async () =>
            {
                await Task.Yield();

                completed.Add(number);
            }));

            if (full)
            {
                await buffer.FlushAsync().ConfigureAwait(false);
            }
        }

        Assert.False(buffer.IsEmpty);

        await buffer.FlushAsync().ConfigureAwait(false);

        Assert.True(expected.SetEquals(completed));
        Assert.True(buffer.IsEmpty);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task FlushAsync_Flushes(int enqueued)
    {
        TaskBuffer buffer = new(maximumCapacity: 2);
        ConcurrentBag<int> completed = new();
        HashSet<int> expected = new();

        for (int i = 0; i < enqueued; i++)
        {
            int number = i + 1;

            expected.Add(number);

            buffer.Enqueue(Task.Run(() => completed.Add(number)));
        }

        Assert.False(buffer.IsEmpty);

        await buffer.FlushAsync().ConfigureAwait(false);

        Assert.True(expected.SetEquals(completed));
        Assert.True(buffer.IsEmpty);
    }

    [Fact]
    public async Task FlushAsync_Cancelled_CancellsAndFlushesAll()
    {
        TaskBuffer buffer = new(maximumCapacity: 2);
        using CancellationTokenSource source = new();
        int completed = 0;

        for (int i = 0; i < 2; i++)
        {
            buffer.Enqueue(Task.Run(async () =>
            {
                await Task.Delay(50).ConfigureAwait(false);

                _ = Interlocked.Increment(ref completed);
            }));
        }

        Assert.False(buffer.IsEmpty);
        Assert.True(buffer.IsFull);

        source.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                    await buffer.FlushAsync(cancellationToken: source.Token).ConfigureAwait(false))
                .ConfigureAwait(false);

        Assert.True(completed < 2);

        Assert.True(buffer.IsEmpty);
        Assert.False(buffer.IsFull);
    }

    [Fact]
    public async Task FlushAsync_FailedTask_FlushesAll()
    {
        TaskBuffer buffer = new(maximumCapacity: 2);

        for (int i = 0; i < 2; i++)
        {
            buffer.Enqueue(Task.Run(() => throw new ArgumentException()));
        }

        Assert.False(buffer.IsEmpty);
        Assert.True(buffer.IsFull);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
                    await buffer.FlushAsync().ConfigureAwait(false))
                .ConfigureAwait(false);

        Assert.True(buffer.IsEmpty);
        Assert.False(buffer.IsFull);
    }

    [Fact]
    public async Task FlushAsync_CancelledTask_FlushesAll()
    {
        TaskBuffer buffer = new(maximumCapacity: 2);
        using CancellationTokenSource source = new();

        source.Cancel();

        for (int i = 0; i < 2; i++)
        {
            buffer.Enqueue(Task.Run(() => source.Token.ThrowIfCancellationRequested()));
        }

        Assert.False(buffer.IsEmpty);
        Assert.True(buffer.IsFull);

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                    await buffer.FlushAsync().ConfigureAwait(false))
                .ConfigureAwait(false);

        Assert.True(buffer.IsEmpty);
        Assert.False(buffer.IsFull);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(13)]
    public async Task Enqueue_FlushAsync_Works(int rounds)
    {
        TaskBuffer buffer = new(
                maximumCapacity: 2,
                fullCallCount: rounds);
        int completed = 0;

        for (int i = 0; i < rounds; i++)
        {
            if (buffer.Enqueue(Task.Run(() => Interlocked.Increment(ref completed))))
            {
                await buffer.FlushAsync().ConfigureAwait(false);
            }
        }

        Assert.Equal(rounds, completed);
        Assert.True(buffer.IsEmpty);
    }
}
