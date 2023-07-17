namespace Radicle.Common;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Radicle.Common.Check;

/// <summary>
/// Raw stash of the tasks for buffer or similar.
/// This class is NOT thread safe.
/// </summary>
/// <remarks>Making this a base class will only
/// complicate the API surface of the derived implementations.
/// Use as internal field and implement <see cref="ITaskStash"/>.</remarks>
/// <typeparam name="TMetaData">Type of the meta-data stored
///     alongside each task.</typeparam>
internal sealed class TaskStash<TMetaData> : ITaskStash
        where TMetaData : notnull
{
    private readonly (Task Task, TMetaData Meta)[] buffer;

    private int currentLength;

    private int capaacityValue;

    private int callCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskStash{TMetaData}"/> class.
    /// </summary>
    /// <param name="maximumCapacity">Cpacity of this buffer.
    ///     Defaults to half of the processor count.</param>
    /// <param name="fullCallCount">Optional value for
    ///     <see cref="CallCountToIndicateFull"/>, you can use init as well.
    ///     Zero or negative value will disable call count based full indication.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="maximumCapacity"/> is zero, negative
    ///     or greater than 65 535.</exception>
    internal TaskStash(
            int? maximumCapacity = null,
            int? fullCallCount = null)
    {
        this.MaximumCapacity = Ensure
                .Optional(maximumCapacity)
                .InRange(1, 65535)
                .ValueOr(Math.Max(Environment.ProcessorCount / 2, 1));
        this.Capacity = this.MaximumCapacity;

        if (fullCallCount.HasValue)
        {
            this.CallCountToIndicateFull = fullCallCount.Value;
        }

        this.buffer = new (Task Task, TMetaData Meta)[this.MaximumCapacity];
    }

    /// <inheritdoc/>
    public int MaximumCapacity { get; }

    /// <inheritdoc/>
    public int Capacity
    {
        get => this.capaacityValue;
        set => this.capaacityValue = Ensure
                .Param(value, nameof(this.Capacity))
                .InRange(1, this.MaximumCapacity)
                .Value;
    }

    /// <inheritdoc/>
    public int CallCountToIndicateFull { get; init; }

    /// <inheritdoc/>
    public bool IsFull =>
            (this.currentLength >= this.Capacity)
            || this.IsFullCallCountReached;

    /// <summary>
    /// Gets a value indicating whether <see cref="CallCountToIndicateFull"/>
    /// is enabled and the call count reached its value.
    /// </summary>
    public bool IsFullCallCountReached => (this.CallCountToIndicateFull > 0)
                && (this.callCount >= this.CallCountToIndicateFull);

    /// <inheritdoc/>
    public bool IsEmpty => this.currentLength == 0;

    /// <summary>
    /// Enqueue given <paramref name="task"/> if capacity was not reached.
    /// </summary>
    /// <param name="task">Task to enqueue.</param>
    /// <param name="metaData">Meta-data of <paramref name="task"/>.</param>
    /// <returns><see langword="true"/> if addition of <paramref name="task"/>
    ///     caused reach of capacity; <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown if required parameter
    ///     is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown
    ///     if the stash is at full capacity (i.e. <see cref="Capacity"/>)
    ///     and needs to be flushed.</exception>
    public bool Enqueue(
            Task task,
            TMetaData metaData)
    {
        if (this.currentLength < this.Capacity)
        {
            this.buffer[this.currentLength++] = (
                    Ensure.Param(task).Value,
                    Ensure.Param(metaData).Value);
            this.callCount++;
        }
        else
        {
            throw new InvalidOperationException("Task buffer capacity was reached.");
        }

        return this.IsFull;
    }

    /// <summary>
    /// Enumerate currently stored list of tasks.
    /// </summary>
    /// <returns>Collection of current tasks.</returns>
    public IEnumerable<(Task Task, TMetaData Meta)> CurrentTasks()
    {
        for (int i = 0; i < this.currentLength; i++)
        {
            yield return this.buffer[i];
        }
    }

    /// <summary>
    /// Enumerate currently stored list of tasks.
    /// </summary>
    /// <returns>Collection of current tasks.</returns>
    public IEnumerable<Task> CurrentPureTasks()
    {
        for (int i = 0; i < this.currentLength; i++)
        {
            yield return this.buffer[i].Task;
        }
    }

    /// <summary>
    /// Enumerate current completed (faulted, canceled
    /// or successfully completed) tasks.
    /// </summary>
    /// <returns>Collection of completed tasks
    /// (faulted, canceled or successfully completed).</returns>
    public IEnumerable<(Task Task, TMetaData Meta)> FlushCompleted()
    {
        if (!this.IsEmpty)
        {
            int freeIndex = 0;

            for (int i = 0; i < this.currentLength; i++)
            {
                // https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.iscompleted?view=net-7.0
                if (this.buffer[i].Task.IsCompleted)
                {
                    yield return this.buffer[i];

                    this.buffer[i] = default!;
                }

                // skip on no-op
                else if (freeIndex != i)
                {
                    // move non completed on available
                    // free index position
                    this.buffer[freeIndex++] = this.buffer[i];
                    this.buffer[i] = default!;
                }
                else
                {
                    freeIndex++;
                }
            }

            this.currentLength = freeIndex;
        }
    }

    /// <summary>
    /// Flush the complete buffer, effectivelly resetting the initial state.
    /// </summary>
    public void FlushAll()
    {
        // clean up any references for GC
        // we can not use weak reference
        // as the buffer may be the only
        // place the tasks are stored
        for (int i = 0; i < this.Capacity; i++)
        {
            this.buffer[i] = default!;
        }

        this.currentLength = 0;
    }
}
