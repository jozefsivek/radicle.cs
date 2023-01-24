namespace Radicle.Common.Base;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Radicle.Common.Check;

/// <summary>
/// Base of task buffers.
/// This class is NOT thread safe.
/// </summary>
/// <typeparam name="TMetaData">Type of the meta-data stored with the task.</typeparam>
public abstract class TaskBufferBase<TMetaData>
        where TMetaData : notnull
{
    private readonly (Task Task, TMetaData Meta)[] buffer;

    private int currentLength;

    private int callCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskBufferBase{TMetaData}"/> class.
    /// </summary>
    /// <param name="capacity">Cpacity of this buffer.
    ///     Defaults to half of the processor count.</param>
    /// <param name="fullCallCount">Optional value for
    ///     <see cref="CallCountToIndicateFull"/>, you can use init as well.
    ///     Zero or negative value will disable call count based full indication.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="capacity"/> is zero or negative.</exception>
    internal TaskBufferBase(
            int? capacity = null,
            int? fullCallCount = null)
    {
        this.Capacity = Ensure
                .Optional(capacity)
                .StrictlyPositive()
                .ValueOr(Math.Max(Environment.ProcessorCount / 2, 1));

        if (fullCallCount.HasValue)
        {
            this.CallCountToIndicateFull = fullCallCount.Value;
        }

        this.buffer = new (Task Task, TMetaData Meta)[this.Capacity];
    }

    /// <summary>
    /// Gets capacity of this buffer,
    /// greater than zero.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Gets amount of enqueue calls
    /// after which the buffer will always indicate full capacity.
    /// Defaults to 0, which disables the indication.
    /// </summary>
    public int CallCountToIndicateFull { get; init; }

    /// <summary>
    /// Gets a value indicating whether this buffer is
    /// at full capacity and or number of calls equals
    /// or exeeds the <see cref="CallCountToIndicateFull"/>.
    /// </summary>
    public bool IsFull =>
            (this.currentLength == this.Capacity)
            || (
                (this.CallCountToIndicateFull > 0)
                && (this.callCount >= this.CallCountToIndicateFull));

    /// <summary>
    /// Gets a value indicating whether this buffer is empty.
    /// </summary>
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
    ///     if the slider is at full capacity and needs to be awaited.</exception>
    protected bool Enqueue(
            Task task,
            TMetaData metaData)
    {
        if (this.currentLength < this.Capacity)
        {
            this.buffer[this.currentLength++] = (Ensure.Param(task).Value, metaData);
            this.callCount++;
        }
        else
        {
            throw new InvalidOperationException("Task buffer capacity was reached.");
        }

        return this.IsFull;
    }

    /// <summary>
    /// Enumerate current list of tasks.
    /// </summary>
    /// <returns>Collection of current tasks.</returns>
    protected IEnumerable<(Task Task, TMetaData Meta)> CurrentTasks()
    {
        for (int i = 0; i < this.currentLength; i++)
        {
            yield return this.buffer[i];
        }
    }

    /// <summary>
    /// Flush the buffer.
    /// </summary>
    protected void Flush()
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
