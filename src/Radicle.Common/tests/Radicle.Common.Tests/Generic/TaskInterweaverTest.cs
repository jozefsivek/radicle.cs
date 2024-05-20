namespace Radicle.Common.Generic;

using System;
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
        TaskInterweaver<int, int> ti = new(
                item =>
                {
                    Assert.Equal(counter, item.Result);
                    Assert.Equal(counter, item.Meta);

                    counter++;

                    return ValueTask.CompletedTask;
                },
                autoFlushCallCount: 10,
                level: level);

        static async Task<int> Foo(int number)
        {
            await Task.Yield();
            return number;
        }

        Assert.Equal(0, counter);

        for (int i = 0; i < 10; i++)
        {
            await ti.EnqueueAsync(Foo(i), i);
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
        TaskInterweaver<int, int> ti = new(
                item =>
                {
                    Assert.Equal(counter, item.Result);
                    Assert.Equal(counter, item.Meta);

                    counter++;

                    return ValueTask.CompletedTask;
                },
                level: level);

        static async Task<int> Foo(int number)
        {
            await Task.Yield();
            return number;
        }

        Assert.Equal(0, counter);

        for (int i = 0; i < 10; i++)
        {
            await ti.EnqueueAsync(Foo(i), i);
        }

        await ti.FlushAsync();

        Assert.True(ti.IsEmpty);
        Assert.Equal(10, counter);
    }

    [Fact]
    public void Constructor_NullCallBack_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                new TaskInterweaver<int, int>(null!));
    }

    [Theory]
    [InlineData(-1.0)]
    [InlineData(2.0)]
    public void Constructor_OutOfBoundsLevel_Throws(double level)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                new TaskInterweaver<int, int>(
                    _ => ValueTask.CompletedTask,
                    level: level));
    }

    [Fact]
    public async Task EnqueueAsync_NullTask_Throws()
    {
        TaskInterweaver<object, int> ti = new(_ => ValueTask.CompletedTask);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await ti.EnqueueAsync(null!, 0).ConfigureAwait(false));
    }

    [Fact]
    public async Task EnqueueAsync_AutoFlushCallCount_Flushes()
    {
        int counter = 0;
        TaskInterweaver<int, int> ti = new(
                _ =>
                {
                    counter++;

                    return ValueTask.CompletedTask;
                },
                autoFlushCallCount: 3);

        static async Task<int> Foo(int number)
        {
            await Task.Yield();
            return number;
        }

        Assert.Equal(0, counter);

        await ti.EnqueueAsync(Foo(0), 0);

        Assert.True(ti.IsFull);
        Assert.Equal(0, counter);

        await ti.EnqueueAsync(Foo(1), 1);

        Assert.Equal(1, counter);

        await ti.EnqueueAsync(Foo(2), 2);

        Assert.False(ti.IsFull);
        Assert.Equal(3, counter);
    }

    [Fact]
    public async Task EnqueueAsync_ZeroLevel_AwaitsDirectly()
    {
        int counter = 0;
        TaskInterweaver<int, int> ti = new(
                _ =>
                {
                    counter++;

                    return ValueTask.CompletedTask;
                },
                level: 0.0);

        static async Task<int> Foo(int number)
        {
            await Task.Yield();
            return number;
        }

        Assert.Equal(0, counter);

        await ti.EnqueueAsync(Foo(0), 0);

        Assert.False(ti.IsFull);
        Assert.Equal(1, counter);
    }

    [Fact]
    public async Task EnqueueAsync_DefaultLevel_AwaitsLater()
    {
        int counter = 0;
        TaskInterweaver<int, int> ti = new(
                _ =>
                {
                    counter++;

                    return ValueTask.CompletedTask;
                });

        static async Task<int> Foo(int number)
        {
            await Task.Yield();
            return number;
        }

        Assert.Equal(0, counter);
        Assert.True(ti.IsEmpty);

        await ti.EnqueueAsync(Foo(0), 0);

        Assert.True(ti.IsFull);
        Assert.False(ti.IsEmpty);
        Assert.Equal(0, counter);

        await ti.EnqueueAsync(Foo(1), 1);

        Assert.True(ti.IsFull);
        Assert.Equal(1, counter);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.33)]
    [InlineData(0.66)]
    [InlineData(1.0)]
    public async Task FlushAsync_FailedTask_FlushesAll(double level)
    {
        TaskInterweaver<int, int> ti = new(
                _ => ValueTask.CompletedTask,
                level: level);

        static async Task<int> Foo()
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
                await ti.EnqueueAsync(Foo(), 0).ConfigureAwait(false);
            }
        });

        Assert.True(ti.IsEmpty);
        Assert.False(ti.IsFull);
    }
}
