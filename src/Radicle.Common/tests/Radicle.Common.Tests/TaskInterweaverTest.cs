namespace Radicle.Common;

using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class TaskInterweaverTest
{
    [Theory]
    [InlineData(0.0)]
    [InlineData(0.33)]
    [InlineData(0.66)]
    [InlineData(1.0)]
    public async Task TaskInterweaver_KnownCount_Works(double level)
    {
        int counter = 0;
        TaskInterweaver ti = new(
                autoFlushCallCount: 10,
                level: level);

        async Task Foo()
        {
            await Task.Yield();
            _ = Interlocked.Increment(ref counter);
        }

        Assert.Equal(0, counter);

        for (int i = 0; i < 10; i++)
        {
            await ti.EnqueueAsync(Foo()).ConfigureAwait(false);
        }

        Assert.True(ti.IsEmpty);
        Assert.Equal(10, counter);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.33)]
    [InlineData(0.66)]
    [InlineData(1.0)]
    public async Task TaskInterweaver_UnknownCount_Works(double level)
    {
        int counter = 0;
        TaskInterweaver ti = new(
                level: level);

        async Task Foo()
        {
            await Task.Yield();
            _ = Interlocked.Increment(ref counter);
        }

        Assert.Equal(0, counter);

        for (int i = 0; i < 10; i++)
        {
            await ti.EnqueueAsync(Foo()).ConfigureAwait(false);
        }

        await ti.FlushAsync().ConfigureAwait(false);

        Assert.True(ti.IsEmpty);
        Assert.Equal(10, counter);
    }

    [Theory]
    [InlineData(-1.0)]
    [InlineData(2.0)]
    public void Constructor_OutOfBoundsLevel_Throws(double level)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                new TaskInterweaver(
                    level: level));
    }

    [Fact]
    public void EnqueueAsync_NullTask_Throws()
    {
        TaskInterweaver ti = new();

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await ti.EnqueueAsync(null!).ConfigureAwait(false));
    }

    [Fact]
    public async Task EnqueueAsync_AutoFlushCallCount_Flushes()
    {
        int counter = 0;
        TaskInterweaver ti = new(
                autoFlushCallCount: 3);
        using SemaphoreSlim semaphore = new(0, 2);

        async Task Foo()
        {
            await semaphore.WaitAsync().ConfigureAwait(false);
            _ = Interlocked.Increment(ref counter);
        }

        Assert.Equal(0, counter);

        await ti.EnqueueAsync(Foo()).ConfigureAwait(false);

        Assert.True(ti.IsFull);
        Assert.Equal(0, counter);
        semaphore.Release(1);

        await ti.EnqueueAsync(Foo()).ConfigureAwait(false);

        Assert.Equal(1, counter);
        semaphore.Release(2);

        await ti.EnqueueAsync(Foo()).ConfigureAwait(false);

        Assert.False(ti.IsFull);
        Assert.Equal(3, counter);
    }

    [Fact]
    public async Task EnqueueAsync_ZeroLevel_AwaitsDirectly()
    {
        int counter = 0;
        TaskInterweaver ti = new(
                level: 0.0);

        async Task Foo()
        {
            await Task.Yield();
            _ = Interlocked.Increment(ref counter);
        }

        Assert.Equal(0, counter);

        await ti.EnqueueAsync(Foo()).ConfigureAwait(false);

        Assert.False(ti.IsFull);
        Assert.Equal(1, counter);
    }

    [Fact]
    public async Task EnqueueAsync_DefaultLevel_AwaitsLater()
    {
        int counter = 0;
        TaskInterweaver ti = new();
        using SemaphoreSlim semaphore = new(0, 2);

        async Task Foo()
        {
            await semaphore.WaitAsync().ConfigureAwait(false);
            _ = Interlocked.Increment(ref counter);
        }

        Assert.Equal(0, counter);
        Assert.True(ti.IsEmpty);

        await ti.EnqueueAsync(Foo()).ConfigureAwait(false);

        Assert.True(ti.IsFull);
        Assert.False(ti.IsEmpty);
        Assert.Equal(0, counter);
        semaphore.Release(1);

        await ti.EnqueueAsync(Foo()).ConfigureAwait(false);

        Assert.True(ti.IsFull);
        Assert.Equal(1, counter);
        semaphore.Release(1);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.33)]
    [InlineData(0.66)]
    [InlineData(1.0)]
    public async Task FlushAsync_FailedTask_FlushesAll(double level)
    {
        TaskInterweaver ti = new(
                level: level);

        static async Task Foo()
        {
            await Task.Yield();

            throw new ArgumentException("test");
        }

        Assert.True(ti.IsEmpty);
        Assert.False(ti.IsFull);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            for (int i = 0; i < 3; i++)
            {
                await ti.EnqueueAsync(Foo()).ConfigureAwait(false);
            }
        }).ConfigureAwait(false);

        Assert.True(ti.IsEmpty);
        Assert.False(ti.IsFull);
    }
}
