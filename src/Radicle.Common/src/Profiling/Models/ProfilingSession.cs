namespace Radicle.Common.Profiling.Models;

using System;
using System.Collections.Generic;
using Radicle.Common.Check;

/// <summary>
/// Profiling session used for recording profiling events.
/// </summary>
public sealed class ProfilingSession
{
    private readonly object operationLock = new();

    private Item? last;

    // TODO: use interlocking if faster than lock
    // https://github.com/StackExchange/StackExchange.Redis/blob/main/src/StackExchange.Redis/Profiling/ProfilingSession.cs
    // https://stackoverflow.com/questions/154551/volatile-vs-interlocked-vs-lock

    /// <summary>
    /// Yield enumeration of all recorded events and clear any stored events.
    /// </summary>
    /// <returns>Enumeration of events.</returns>
    public IEnumerable<IProfilingEvent> Yield()
    {
        Item? last = null;

        if (this.last is not null)
        {
            lock (this.operationLock)
            {
                if (this.last is not null)
                {
                    last = this.last;
                    this.last = null;
                }
            }
        }

        if (last is not null)
        {
            Item? yieldHead;

            while (true)
            {
                if (last.Previous is null)
                {
                    yieldHead = last;
                    break;
                }

                last = last.Previous;
            }

            while (yieldHead is not null)
            {
                yield return yieldHead.Value;

                yieldHead = yieldHead.Next;
            }
        }
    }

    /// <summary>
    /// Add instance of stand alone or linked event to this session.
    /// </summary>
    /// <param name="profileEvent">Event to add.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public void Add(IProfilingEvent profileEvent)
    {
        Item e = new(Ensure.Param(profileEvent).Value);

        lock (this.operationLock)
        {
            if (this.last is not null)
            {
                e.Previous = this.last;
                this.last.Next = e;
            }

            this.last = e;
        }
    }

    /// <summary>
    /// Single-linked items storing <see cref="IProfilingEvent"/> instances.
    /// </summary>
    private class Item
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="value">Event.</param>
        public Item(IProfilingEvent value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets event.
        /// </summary>
        public IProfilingEvent Value { get; }

        /// <summary>
        /// Gets or sets link to the previous item.
        /// </summary>
        public Item? Previous { get; set; }

        /// <summary>
        /// Gets or sets link to the next item.
        /// </summary>
        public Item? Next { get; set; }
    }
}
