namespace Radicle.Common;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class TaskStashTest
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(65_536)]
    public void Constructor_InvalidMaxCapacity_Throws(int maxCapacity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                new TaskStash<int>(maximumCapacity: maxCapacity));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(5)]
    public void Capacity_InvalidValue_Throws(int capacity)
    {
        TaskStash<int> stash = new(maximumCapacity: 4);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
                stash.Capacity = capacity);
    }

    [Fact]
    public void IsFull_CapacityReached_ReturnsTrue()
    {
        TaskStash<int> stash = new(maximumCapacity: 2);

        Assert.False(stash.IsFull);

        bool isFull = stash.Enqueue(Task.CompletedTask, 0);

        Assert.False(stash.IsFull);
        Assert.False(isFull);

        isFull = stash.Enqueue(Task.CompletedTask, 0);

        Assert.True(stash.IsFull);
        Assert.True(isFull);
    }

    [Fact]
    public void IsFull_CappedCapacityReached_ReturnsTrue()
    {
        TaskStash<int> stash = new(maximumCapacity: 4);

        Assert.False(stash.IsFull);

        bool isFull = stash.Enqueue(Task.CompletedTask, 0);

        Assert.False(stash.IsFull);
        Assert.False(isFull);

        isFull = stash.Enqueue(Task.CompletedTask, 0);

        Assert.False(stash.IsFull);
        Assert.False(isFull);

        stash.Capacity = 2;

        Assert.True(stash.IsFull);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(13)]
    public void IsFull_CallCountReached_ReturnsTrue(int fullCallCount)
    {
        TaskStash<int> stash = new(maximumCapacity: 4, fullCallCount: fullCallCount);

        Assert.False(stash.IsFull);

        for (int i = 0; i < fullCallCount; i++)
        {
            bool isFull = stash.Enqueue(Task.CompletedTask, 0);

            if (
                    (((i + 1) % 4) == 0)
                    || (i == (fullCallCount - 1)))
            {
                Assert.True(stash.IsFull);
                Assert.True(isFull);

                stash.FlushAll();
            }
            else
            {
                Assert.False(stash.IsFull);
                Assert.False(isFull);
            }
        }
    }

    [Fact]
    public void IsEmpty_EmptyInstance_ReturnsTrue()
    {
        TaskStash<int> stash = new(maximumCapacity: 4);

        Assert.True(stash.IsEmpty);

        _ = stash.Enqueue(Task.CompletedTask, 0);

        Assert.False(stash.IsEmpty);
    }

    [Fact]
    public void Enqueue_NullTask_Throws()
    {
        TaskStash<int> stash = new(maximumCapacity: 4);

        Assert.Throws<ArgumentNullException>(() => stash.Enqueue(null!, 0));
    }

    [Fact]
    public void Enqueue_NullMetaData_Throws()
    {
        TaskStash<object> stash = new(maximumCapacity: 4);

        Assert.Throws<ArgumentNullException>(() => stash.Enqueue(Task.CompletedTask, null!));
    }

    [Fact]
    public void Enqueue_AboveCapacity_Throws()
    {
        TaskStash<int> stash = new(maximumCapacity: 2);

        bool isFull = stash.Enqueue(Task.CompletedTask, 0);

        Assert.False(isFull);

        isFull = stash.Enqueue(Task.CompletedTask, 0);

        Assert.True(isFull);

        Assert.Throws<InvalidOperationException>(() =>
                stash.Enqueue(Task.CompletedTask, 0));
    }

    [Fact]
    public void Enqueue_AboveCappedCapacity_Throws()
    {
        TaskStash<int> stash = new(maximumCapacity: 4);

        bool isFull = stash.Enqueue(Task.CompletedTask, 0);

        Assert.False(isFull);

        isFull = stash.Enqueue(Task.CompletedTask, 0);

        Assert.False(isFull);

        stash.Capacity = 2;

        Assert.Throws<InvalidOperationException>(() =>
                stash.Enqueue(Task.CompletedTask, 0));
    }

    [Fact]
    public void Enqueue_FullCallCount_ReturnsTrue()
    {
        TaskStash<int> stash = new(maximumCapacity: 4, fullCallCount: 2);

        bool isFull = stash.Enqueue(Task.CompletedTask, 0);

        Assert.False(isFull);

        Assert.True(stash.Enqueue(Task.CompletedTask, 0));
    }

    [Fact]
    public void CurrentTasks_ReturnsEnqueuedTasks()
    {
        TaskStash<int> stash = new(maximumCapacity: 4);

        Task t1 = new(() => Math.Sqrt(1));
        Task t2 = new(() => Math.Sqrt(1));

        _ = stash.Enqueue(t1, 1);
        _ = stash.Enqueue(t2, 2);

        (Task Task, int Meta)[] result = stash.CurrentTasks().ToArray();

        Assert.Equal(2, result.Length);

        Assert.Equal(t1, result[0].Task);
        Assert.Equal(1, result[0].Meta);

        Assert.Equal(t2, result[1].Task);
        Assert.Equal(2, result[1].Meta);
    }

    [Fact]
    public void CurrentPureTasks_ReturnsEnqueuedTasks()
    {
        TaskStash<int> stash = new(maximumCapacity: 4);

        Task t1 = new(() => Math.Sqrt(1));
        Task t2 = new(() => Math.Sqrt(1));

        _ = stash.Enqueue(t1, 1);
        _ = stash.Enqueue(t2, 2);

        Task[] result = stash.CurrentPureTasks().ToArray();

        Assert.Equal(2, result.Length);

        Assert.Equal(t1, result[0]);
        Assert.Equal(t2, result[1]);
    }

    [Fact]
    public void FlushCompleted_NoCompleted_ReturnsEmptyCollection()
    {
        TaskStash<int> stash = new(maximumCapacity: 4);
        using SemaphoreSlim semaphore = new(0, 2);

        Task t1 = semaphore.WaitAsync();
        Task t2 = semaphore.WaitAsync();

        _ = stash.Enqueue(t1, 1);
        _ = stash.Enqueue(t2, 2);

        Assert.Empty(stash.FlushCompleted());

        semaphore.Release(2);
    }

    [Fact]
    public void FlushCompleted_Empty_ReturnsEmptyCollection()
    {
        TaskStash<int> stash = new(maximumCapacity: 4);

        Assert.Empty(stash.FlushCompleted());
    }

    [Fact]
    public void FlushCompleted_SomeCompleted_ReturnsCompleted()
    {
        TaskStash<int> stash = new(maximumCapacity: 4);
        using SemaphoreSlim semaphore = new(0, 2);

        Task t1 = semaphore.WaitAsync();
        Task t2 = semaphore.WaitAsync();

        _ = stash.Enqueue(t1, 1);
        _ = stash.Enqueue(t2, 2);

        semaphore.Release(1);

        Assert.Single(stash.FlushCompleted());

        semaphore.Release(2);
    }

    [Fact]
    public void FlushAll_NoCompleted_Flushes()
    {
        TaskStash<int> stash = new(maximumCapacity: 4);
        using SemaphoreSlim semaphore = new(0, 2);

        Task t1 = semaphore.WaitAsync();
        Task t2 = semaphore.WaitAsync();

        _ = stash.Enqueue(t1, 1);
        _ = stash.Enqueue(t2, 2);

        stash.FlushAll();

        Assert.True(stash.IsEmpty);

        semaphore.Release(2);
    }

    [Fact]
    public void FlushAll_Empty_Flushes()
    {
        TaskStash<int> stash = new(maximumCapacity: 4);

        stash.FlushAll();

        Assert.True(stash.IsEmpty);
    }

    [Fact]
    public void FlushAll_SomeCompleted_Flushes()
    {
        TaskStash<int> stash = new(maximumCapacity: 4);
        using SemaphoreSlim semaphore = new(0, 2);

        Task t1 = semaphore.WaitAsync();
        Task t2 = semaphore.WaitAsync();

        _ = stash.Enqueue(t1, 1);
        _ = stash.Enqueue(t2, 2);

        semaphore.Release(1);

        stash.FlushAll();

        Assert.True(stash.IsEmpty);

        semaphore.Release(2);
    }
}
