namespace Radicle.Common.Generic;

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
/// <para>
/// Why there is no thing for <see cref="ValueTask"/>?
/// See https://devblogs.microsoft.com/dotnet/understanding-the-whys-whats-and-whens-of-valuetask/ .
/// It is just not more performant for the expected use cases
/// when the awaited things DO NOT complete synchronously
/// most of the time and are not directly awaited.
/// </para>
/// <para>
/// Why to have less than 2 paralel tasks running. In
/// the following scenarion a tasks are awaited:
/// - serially : 8.2s
/// - with thread buffer (capacity 2): 5.0s
/// - with Interweaver (level 0.0): 8.3s (same as serially)
/// - with Interweaver (level 0.2): 7.0s
/// - with Interweaver (level 0.4): 5.9s
/// - with Interweaver (level 0.6): 5.2s
/// - with Interweaver (level 0.8): 5.1s
/// - with Interweaver (level 1.0): 5.0s.
/// </para>
/// </remarks>
/// <typeparam name="TValue">Return type of the task.</typeparam>
public class TaskInterweaver<TValue>
{
    private readonly Action<TValue>? valueCallback;

    private readonly double level;

    private Task<TValue>? pending;

    private int remainingCallCount = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskInterweaver{TValue}"/> class.
    /// </summary>
    /// <param name="valueCallback">Optional callback for awaited value,
    ///     this callback is never called concurently.</param>
    /// <param name="autoFlushCallCount">Optional value for
    ///     auto flush.</param>
    /// <param name="level">Level of interweaving, 1.0 beeing full,
    ///     and 0.0 beeing none.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="level"/> is outside [0.0, 1.0] range
    ///     -or- <paramref name="autoFlushCallCount"/> is negative.</exception>
    public TaskInterweaver(
            Action<TValue>? valueCallback = null,
            int? autoFlushCallCount = null,
            double level = 1.0)
    {
        Ensure.Optional(autoFlushCallCount).NonNegative().Done();

        this.valueCallback = valueCallback;
        this.level = Ensure.Param(level).InRange(0.0, 1.0).Value;

        if (autoFlushCallCount.HasValue)
        {
            this.remainingCallCount = autoFlushCallCount.Value;
        }
    }

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
    public async Task EnqueueAsync(Task<TValue> task)
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

        this.valueCallback?.Invoke(await this.pending.ConfigureAwait(false));
        this.pending = default;
    }
}
