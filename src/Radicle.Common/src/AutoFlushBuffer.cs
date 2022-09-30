namespace Radicle.Common;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Radicle.Common.Check;

/// <summary>
/// Auto-flush buffer for tasks.
/// Aggregator is used to accumulate tasks into aggregate
/// and then, when capacity is reached, or explicitly,
/// the flush callback is called.
/// This class is NOT thread safe.
/// </summary>
/// <typeparam name="TAggregate">Type of aggregate.</typeparam>
/// <typeparam name="T">Type of the item beeing processed.</typeparam>
public sealed class AutoFlushBuffer<TAggregate, T>
        where TAggregate : notnull
        where T : notnull
{
    private readonly TAggregate aggregate;
    private readonly Func<TAggregate, T, ValueTask<bool>> aggregator;
    private readonly Func<TAggregate, ValueTask> flusher;
    private int currentLength;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoFlushBuffer{T, TAggregate}"/> class.
    /// </summary>
    /// <param name="capacity">Maximum capacity for auto-flush.</param>
    /// <param name="aggregate">Initial aggregate instance to use.</param>
    /// <param name="aggregator">Aggregator callback, returns <see langword="true"/>
    ///     if item should be counted for comparing with <see cref="Capacity"/>
    ///     or <see langword="false"/> if the item should be consideren no-op like.</param>
    /// <param name="flusher">Flush callback.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="capacity"/> is zero or negative.</exception>
    public AutoFlushBuffer(
            int capacity,
            TAggregate aggregate,
            Func<TAggregate, T, ValueTask<bool>> aggregator,
            Func<TAggregate, ValueTask> flusher)
    {
        this.Capacity = Ensure.Param(capacity).StrictlyPositive().Value;
        this.aggregate = Ensure.Param(aggregate).Value;
        this.aggregator = Ensure.Param(aggregator).Value;
        this.flusher = Ensure.Param(flusher).Value;
    }

    /// <summary>
    /// Gets capacity of this buffer,
    /// greater than zero.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Enqueue item and flush if capacity is reached.
    /// </summary>
    /// <param name="item">Item.</param>
    /// <returns>Awaitable value task.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public async ValueTask EnqueueAsync(T item)
    {
        Ensure.Param(item).Done();

        if (await this.aggregator(this.aggregate, item).ConfigureAwait(false))
        {
            this.currentLength++;
        }

        if (this.currentLength >= this.Capacity)
        {
            await this.FlushAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Enqueue all items and flush in any case.
    /// </summary>
    /// <param name="items">Items.</param>
    /// <returns>Awaitable value task.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public async ValueTask EnqueueAndFlushAsync(IEnumerable<T> items)
    {
        foreach (T item in Ensure.Param(items))
        {
            await this.EnqueueAsync(item).ConfigureAwait(false);
        }

        await this.FlushAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Flush manually.
    /// </summary>
    /// <returns>Awaitable value task.</returns>
    public async ValueTask FlushAsync()
    {
        await this.flusher(this.aggregate).ConfigureAwait(false);

        this.currentLength = 0;
    }
}
