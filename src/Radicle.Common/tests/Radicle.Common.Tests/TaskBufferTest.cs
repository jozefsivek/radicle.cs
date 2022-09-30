namespace Radicle.Common;

using System;
using System.Threading.Tasks;
using Xunit;

public class TaskBufferTest
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Constructor_InvalidCpacity_Throws(int invalidCapacity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                new TaskBuffer(capacity: invalidCapacity));
    }

    [Fact]
    public void Enqueue_NullInput_Throws()
    {
        TaskBuffer buffer = new();

        Assert.Throws<ArgumentNullException>(() =>
                buffer.Enqueue(null!));
    }

    [Fact]
    public void Enqueue_FullCapacity_Throws()
    {
        TaskBuffer buffer = new(capacity: 2);

        buffer.Enqueue(Task.CompletedTask);
        buffer.Enqueue(Task.CompletedTask);

        Assert.Throws<InvalidOperationException>(() =>
                buffer.Enqueue(Task.CompletedTask));
    }

    [Fact]
    public void Enqueue_BellowCapacity_ReturnsFalse()
    {
        TaskBuffer buffer = new(capacity: 2);

        Assert.False(buffer.Enqueue(Task.CompletedTask));
    }

    [Fact]
    public void Enqueue_JustReachingCapacity_ReturnsTrue()
    {
        TaskBuffer buffer = new(capacity: 3);

        Assert.False(buffer.Enqueue(Task.CompletedTask));
        Assert.False(buffer.Enqueue(Task.CompletedTask));
        Assert.True(buffer.Enqueue(Task.CompletedTask));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(10)]
    [InlineData(13)]
    public async Task Enqueue_ReachOfCallCount_ReturnsTrue(int fullCallCount)
    {
        TaskBuffer buffer = new(capacity: 4, fullCallCount: fullCallCount);

        for (int i = 0; i < (fullCallCount - 1); i++)
        {
            if (buffer.Enqueue(Task.CompletedTask))
            {
                await buffer.FlushAsync().ConfigureAwait(false);
            }
        }

        Assert.True(buffer.Enqueue(Task.CompletedTask)); // n-th call
    }
}
