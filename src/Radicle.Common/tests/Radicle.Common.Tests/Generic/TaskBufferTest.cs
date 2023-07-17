namespace Radicle.Common.Generic;

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
        List<int> results = new(5);
        List<int> metas = new(5);
        List<int> expectedResults = new() { 1, 2, 3, 4, 5 };
        HashSet<int> expected = new(expectedResults);
        TaskBuffer<int, int> buffer = new(
                maximumCapacity: 2,
                fullCallCount: expected.Count);

        foreach (int number in expected)
        {
            bool full = buffer.Enqueue(
                Task.Run(async () =>
                {
                    await Task.Yield();

                    completed.Add(number);

                    return number;
                }),
                number);

            if (full)
            {
                await foreach ((int result, int meta) in buffer.FlushAsync().ConfigureAwait(false))
                {
                    results.Add(result);
                    metas.Add(meta);
                }
            }
        }

        Assert.True(expected.SetEquals(completed));
        Assert.Equal(expectedResults, results);
        Assert.Equal(expectedResults, metas);
        Assert.True(buffer.IsEmpty);
    }

    [Fact]
    public async Task TaskBuffer_UnknownCount_Works()
    {
        ConcurrentBag<int> completed = new();
        List<int> results = new(5);
        List<int> metas = new(5);
        List<int> expectedResults = new() { 1, 2, 3, 4, 5 };
        HashSet<int> expected = new(expectedResults);
        TaskBuffer<int, int> buffer = new(
                maximumCapacity: 2);

        foreach (int number in expected)
        {
            bool full = buffer.Enqueue(
                Task.Run(async () =>
                {
                    await Task.Yield();

                    completed.Add(number);

                    return number;
                }),
                number);

            if (full)
            {
                await foreach ((int result, int meta) in buffer.FlushAsync().ConfigureAwait(false))
                {
                    results.Add(result);
                    metas.Add(meta);
                }
            }
        }

        Assert.False(buffer.IsEmpty);

        await foreach ((int result, int meta) in buffer.FlushAsync().ConfigureAwait(false))
        {
            results.Add(result);
            metas.Add(meta);
        }

        Assert.True(expected.SetEquals(completed));
        Assert.Equal(expectedResults, results);
        Assert.Equal(expectedResults, metas);
        Assert.True(buffer.IsEmpty);
    }

    [Fact]
    public async Task FlushAsync_TasksWithMetaData_ReturnsCorrectMetadata()
    {
        TaskBuffer<string, int> buffer = new(maximumCapacity: 12);

        for (int i = 0; i < 10; i++)
        {
            buffer.Enqueue(Task.FromResult($"{i}"), i);
        }

        await foreach ((string result, int meta) in buffer.FlushAsync())
        {
            Assert.Equal($"{meta}", result);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task FlushAsync_Flushes(int enqueued)
    {
        TaskBuffer<int, int> buffer = new(maximumCapacity: 3);

        for (int i = 0; i < enqueued; i++)
        {
            int number = i + 1;

            buffer.Enqueue(
                    Task.Run(() => number),
                    number);
        }

        Assert.False(buffer.IsEmpty);

        int expected = 1;

        await foreach ((int result, int meta) in buffer.FlushAsync().ConfigureAwait(false))
        {
            Assert.Equal(expected, result);
            Assert.Equal(expected, meta);

            expected++;
        }

        Assert.True(buffer.IsEmpty);
    }

    [Fact]
    public async Task FlushAsync_Cancelled_CancellsAndFlushesAll()
    {
        TaskBuffer<int, int> buffer = new(maximumCapacity: 2);
        using CancellationTokenSource source = new();
        int completed = 0;

        for (int i = 0; i < 2; i++)
        {
            buffer.Enqueue(
                    Task.Run(async () =>
                    {
                        await Task.Delay(100).ConfigureAwait(false);

                        return Interlocked.Increment(ref completed);
                    }),
                    i);
        }

        Assert.False(buffer.IsEmpty);
        Assert.True(buffer.IsFull);

        source.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                {
                    await foreach ((int result, int meta) in
                            buffer.FlushAsync(cancellationToken: source.Token).ConfigureAwait(false))
                    {
                    }
                })
                .ConfigureAwait(false);

        Assert.True(completed < 2);

        Assert.True(buffer.IsEmpty);
        Assert.False(buffer.IsFull);
    }

    [Fact]
    public async Task FlushAsync_FailedTask_FlushesAll()
    {
        TaskBuffer<int, int> buffer = new(maximumCapacity: 2);

        for (int i = 0; i < 2; i++)
        {
            bool fail = i == 0;

            buffer.Enqueue(
                Task.Run(() =>
                {
                    if (fail)
                    {
                        throw new ArgumentException();
                    }

                    return 0;
                }),
                0);
        }

        Assert.False(buffer.IsEmpty);
        Assert.True(buffer.IsFull);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
                {
                    await foreach ((int result, int meta) in
                            buffer.FlushAsync().ConfigureAwait(false))
                    {
                    }
                })
                .ConfigureAwait(false);

        Assert.True(buffer.IsEmpty);
        Assert.False(buffer.IsFull);
    }

    [Fact]
    public async Task FlushAsync_CancelledTask_FlushesAll()
    {
        TaskBuffer<int, int> buffer = new(maximumCapacity: 2);
        using CancellationTokenSource source = new();

        source.Cancel();

        for (int i = 0; i < 2; i++)
        {
            buffer.Enqueue(
                Task.Run(() =>
                {
                    source.Token.ThrowIfCancellationRequested();

                    return 0;
                }),
                0);
        }

        Assert.False(buffer.IsEmpty);
        Assert.True(buffer.IsFull);

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await foreach ((int result, int meta) in
                    buffer.FlushAsync().ConfigureAwait(false))
            {
            }
        })
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
        TaskBuffer<int, int> buffer = new(
                maximumCapacity: 2,
                fullCallCount: rounds);
        int completed = 0;
        int expected = 0;

        for (int i = 0; i < rounds; i++)
        {
            int number = i + 1;

            if (buffer.Enqueue(
                    Task.Run(() =>
                    {
                        _ = Interlocked.Increment(ref completed);
                        return number;
                    }),
                    number))
            {
                await foreach ((int result, int meta) in
                        buffer.FlushAsync().ConfigureAwait(false))
                {
                    expected++;

                    Assert.Equal(expected, result);
                    Assert.Equal(expected, meta);
                }
            }
        }

        Assert.Equal(rounds, completed);
        Assert.Equal(rounds, expected);
        Assert.True(buffer.IsEmpty);
    }
}
