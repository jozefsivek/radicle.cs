namespace Radicle.Common;

using System;
using System.Threading.Tasks;
using Radicle.Common.Check;

/// <summary>
/// Non thread safe utility to interweave incomming tasks.
/// This allows to perform work on processing one result while
/// waiting on the other to complete. It is for low overhead
/// and low paralelization (even lower than 2) cases
/// unlike <see cref="TaskBuffer"/>.
/// </summary>
/// <remarks>
/// See <see cref="Generic.TaskInterweaver{TValue, TMetaData}"/>.
/// </remarks>
public class TaskInterweaver
{
    private readonly double level;

    private Task? pending;

    /* for memory reasons we combined here call count
     * and the counter itself unlike in task stash */
    private int remainingCallCount = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskInterweaver"/> class.
    /// </summary>
    /// <param name="autoFlushCallCount">Optional value for
    ///     auto flush. Zero or negative value will disable call count
    ///     based flush. Defaults to zero.
    /// </param>
    /// <param name="level">Level of interweaving, 1.0 beeing full,
    ///     and 0.0 beeing none.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="level"/> is outside [0.0, 1.0] range
    ///     -or- <paramref name="autoFlushCallCount"/> is negative.</exception>
    public TaskInterweaver(
            int? autoFlushCallCount = null,
            double level = 1.0)
    {
        this.level = Ensure.Param(level).InRange(0.0, 1.0).Value;

        if (autoFlushCallCount > 0)
        {
            this.remainingCallCount = autoFlushCallCount.Value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance has pending tasks to flush.
    /// </summary>
    public bool IsFull => this.pending is not null;

    /// <summary>
    /// Gets a value indicating whether this instance has no pending tasks to flush.
    /// </summary>
    public bool IsEmpty => this.pending is null;

    /// <summary>
    /// Enqueue given <paramref name="task"/>
    /// and call value callback if necessary.
    /// </summary>
    /// <param name="task">Task to store.</param>
    /// <returns>Flag determining if there is any pending value
    ///     and the pending value itself from the previously
    ///     enqueued task if any.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public async Task EnqueueAsync(
            Task task)
    {
        Ensure.Param(task).Done();

        await this.FlushAsync().ConfigureAwait(false);

        this.pending = task;

        bool forceFlush;

        if (this.remainingCallCount == -1)
        {
            forceFlush = false;
        }
        else
        {
            this.remainingCallCount--;

            if (this.remainingCallCount < 0)
            {
                this.remainingCallCount = 0;
            }

            forceFlush = this.remainingCallCount == 0;
        }

        if (forceFlush || (this.level < 1.0 && ThreadSafeRandom.NextDouble() >= this.level))
        {
            await this.FlushAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Return any awaitable value, if any.
    /// </summary>
    /// <returns>Flag determining if there is any pending value
    ///     and the pending value itself from the previously
    ///     enqueued task if any.</returns>
    public async Task FlushAsync()
    {
        if (this.pending is null)
        {
            return;
        }

        try
        {
            await this.pending.ConfigureAwait(false);
        }
        finally
        {
            this.pending = default;
        }
    }
}
