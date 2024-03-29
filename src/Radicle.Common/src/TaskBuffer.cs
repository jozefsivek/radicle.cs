namespace Radicle.Common;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Utility for buffering tasks and awaiting multiple of them.
/// This class is NOT thread safe.
/// </summary>
/// <remarks>The reason for using <see cref="int"/> is easier
/// handling when used with collections, they notoriously have
/// <see cref="int"/> lengths.</remarks>
public sealed class TaskBuffer : ITaskStash
{
    private readonly TaskStash<int> stash;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskBuffer"/> class.
    /// </summary>
    /// <param name="maximumCapacity">Capacity of this buffer.
    ///     Defaults to half of the processor count.</param>
    /// <param name="fullCallCount">Optional value for
    ///     <see cref="TaskStash{TMetaData}.CallCountToIndicateFull"/>, you can use init as well.
    ///     Zero or negative value will disable call count based full state indication.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="maximumCapacity"/> is zero, negative
    ///     or greater than 65 535.</exception>
    public TaskBuffer(
            int? maximumCapacity = null,
            int? fullCallCount = null)
    {
        this.stash = new TaskStash<int>(
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
    /// </summary>
    /// <param name="task">Task to enqueue.</param>
    /// <returns><see langword="true"/> if addition of <paramref name="task"/>
    ///     caused reach of capacity; <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown if required parameter
    ///     is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown
    ///     if the buffer is at full capacity and needs to be awaited.</exception>
    public bool Enqueue(Task task)
    {
        return this.stash.Enqueue(task, 0);
    }

    /// <summary>
    /// Await all stored tasks serially and clear
    /// the buffer. Call before or at the time
    /// <see cref="Enqueue(Task)"/> returns <see langword="true"/>.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Awaitable task.</returns>
    /// <exception cref="Exception">Thrown from the awaited
    ///     stored task.</exception>
    /// <exception cref="OperationCanceledException">Thrown
    ///     if operation was cancelled.</exception>
    public async Task FlushAsync(
            CancellationToken cancellationToken = default)
    {
        if (this.IsEmpty)
        {
            return;
        }

        try
        {
            foreach ((Task task, int _) in this.stash.CurrentTasks())
            {
                cancellationToken.ThrowIfCancellationRequested();

                await task.ConfigureAwait(false);
            }
        }
        finally
        {
            this.stash.FlushAll();
        }
    }
}
