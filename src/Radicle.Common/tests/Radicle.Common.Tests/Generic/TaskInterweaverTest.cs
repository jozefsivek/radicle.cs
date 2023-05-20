namespace Radicle.Common.Generic;

using System;
using System.Threading.Tasks;
using Xunit;

public class TaskInterweaverTest
{
    [Theory]
    [InlineData(-1.0)]
    [InlineData(2.0)]
    public void Constructor_OutOfBoundsLevel_Throws(double level)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                new TaskInterweaver<int>(level: level));
    }

    [Fact]
    public void Constructor_NegativeAutoFlushCallCount_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                new TaskInterweaver<int>(autoFlushCallCount: -1));
    }

    [Fact]
    public async Task EnqueueAsync_NullableValue_Works()
    {
        int counter = 0;
        int nulls = 3;
        TaskInterweaver<object?> ti = new(
                obj =>
                {
                    if (obj is null)
                    {
                        counter++;
                    }
                });

        async Task<object?> Foo()
        {
            await Task.Yield();
            return nulls-- > 0 ? null : new object();
        }

        Assert.Equal(0, counter);

        for (int i = 0; i < 10; i++)
        {
            await ti.EnqueueAsync(Foo()).ConfigureAwait(false);
        }

        await ti.FlushAsync().ConfigureAwait(false);

        Assert.Equal(3, counter);
    }

    [Fact]
    public void EnqueueAsync_NullTask_Throws()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await new TaskInterweaver<int>().EnqueueAsync(null!).ConfigureAwait(false));
    }

    [Fact]
    public async Task EnqueueAsync_AutoFlushCallCount_Flushes()
    {
        int counter = 0;
        TaskInterweaver<int> ti = new(_ => counter++, autoFlushCallCount: 3);

        async Task<int> Foo()
        {
            await Task.Yield();
            return counter;
        }

        Assert.Equal(0, counter);

        await ti.EnqueueAsync(Foo()).ConfigureAwait(false);

        Assert.Equal(0, counter);

        await ti.EnqueueAsync(Foo()).ConfigureAwait(false);

        Assert.Equal(1, counter);

        await ti.EnqueueAsync(Foo()).ConfigureAwait(false);

        Assert.Equal(3, counter);
    }

    [Fact]
    public async Task EnqueueAsync_ZeroLevel_AwaitsDirectly()
    {
        int counter = 0;
        TaskInterweaver<int> ti = new(_ => counter++, level: 0.0);

        async Task<int> Foo()
        {
            await Task.Yield();
            return counter;
        }

        Assert.Equal(0, counter);

        await ti.EnqueueAsync(Foo()).ConfigureAwait(false);

        Assert.Equal(1, counter);
    }

    [Fact]
    public async Task EnqueueAsync_DefaultLevel_AwaitsLater()
    {
        int counter = 0;
        TaskInterweaver<int> ti = new(_ => counter++);

        async Task<int> Foo()
        {
            await Task.Yield();
            return counter;
        }

        Assert.Equal(0, counter);

        await ti.EnqueueAsync(Foo()).ConfigureAwait(false);

        Assert.Equal(0, counter);

        await ti.EnqueueAsync(Foo()).ConfigureAwait(false);

        Assert.Equal(1, counter);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.33)]
    [InlineData(0.66)]
    [InlineData(1.0)]
    public async Task FlushAsync_Completes(double level)
    {
        int counter = 0;
        TaskInterweaver<int> ti = new(_ => counter++, level: level);

        async Task<int> Foo()
        {
            await Task.Yield();
            return counter;
        }

        Assert.Equal(0, counter);

        for (int i = 0; i < 10; i++)
        {
            await ti.EnqueueAsync(Foo()).ConfigureAwait(false);
        }

        await ti.FlushAsync().ConfigureAwait(false);

        Assert.Equal(10, counter);
    }
}
