namespace Radicle.Common.Generic;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Generic version of <see cref="TaskBuffer"/>.
/// This class is NOT thread safe.
/// </summary>
/// <typeparam name="TValue">Return type of the task.</typeparam>
/// <typeparam name="TMetaData">Type of the meta-data stored with the task.</typeparam>
public sealed class TaskBuffer<TValue, TMetaData> : ITaskStash
        where TMetaData : notnull
{
    private readonly TaskStash<TMetaData> stash;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskBuffer{TValue, TMetaData}"/> class.
    /// </summary>
    /// <param name="maximumCapacity">Maximum capacity of this buffer.
    ///     Defaults to half of the processor count.</param>
    /// <param name="fullCallCount">Optional value for
    ///     <see cref="TaskStash{TMetaData}.CallCountToIndicateFull"/>.
    ///     Zero or negative value will disable call count
    ///     based full state indication. Defaults to zero.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="maximumCapacity"/> is zero, negative
    ///     or greater than 65 535.</exception>
    public TaskBuffer(
            int? maximumCapacity = null,
            int? fullCallCount = null)
    {
        this.stash = new TaskStash<TMetaData>(
                maximumCapacity: maximumCapacity,
                fullCallCount: fullCallCount);
    }

    /// <inheritdoc/>
    public int MaximumCapacity => this.stash.MaximumCapacity;

    /// <inheritdoc/>
    public int Capacity
    {
        get => this.stash.Capacity;
        set => this.stash.Capacity = value;
    }

    /// <inheritdoc/>
    public int CallCountToIndicateFull => this.stash.CallCountToIndicateFull;

    /// <inheritdoc/>
    public bool IsFull => this.stash.IsFull;

    /// <inheritdoc/>
    public bool IsEmpty => this.stash.IsEmpty;

    /// <summary>
    /// Enqueue given <paramref name="task"/> if capacity was not reached.
    /// Order of enqueued tasks is preserved.
    /// </summary>
    /// <param name="task">Task to enqueue.</param>
    /// <param name="metaData">Meta-data of <paramref name="task"/>.</param>
    /// <returns><see langword="true"/> if addition of <paramref name="task"/>
    ///     caused reach of capacity; <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown if required parameter
    ///     is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown
    ///     if the buffer is at full capacity and needs to be awaited.</exception>
    public bool Enqueue(
            Task<TValue> task,
            TMetaData metaData)
    {
        return this.stash.Enqueue(task, metaData);
    }

    /*
    check http://blog.monstuff.com/archives/2019/03/async-enumerables-with-cancellation.html
    for tips
    */

    /// <summary>
    /// Await all stored tasks serially in order they were enqueued and clear
    /// the buffer. Call before or at the time
    /// <see cref="Enqueue(Task{TValue}, TMetaData)"/> returns <see langword="true"/>.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token
    ///     for the enumeration. See
    ///     http://blog.monstuff.com/archives/2019/03/async-enumerables-with-cancellation.html
    ///     for more info.</param>
    /// <returns>Enumeraion of task results and metadata.</returns>
    /// <exception cref="Exception">Thrown from the awaited
    ///     stored task.</exception>
    /// <exception cref="OperationCanceledException">Thrown
    ///     if operation was cancelled.</exception>
    public async IAsyncEnumerable<(TValue Result, TMetaData Meta)> FlushAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!this.IsEmpty)
        {
            try
            {
                foreach ((Task task, TMetaData meta) in this.stash.CurrentTasks())
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    TValue result = await ((Task<TValue>)task).ConfigureAwait(false);

                    yield return (result, meta);
                }
            }
            finally
            {
                this.stash.FlushAll();
            }
        }
    }
}
