namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of RESP push.
/// Do not nest this value in other aggregate values.
/// </summary>
public sealed class RESPPush : RESPAggregate, IEnumerable<RESPValue>
{
    /// <summary>
    /// Gets human readable name.
    /// </summary>
    public const string HumanName = "push array";

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPPush"/> class.
    /// </summary>
    /// <param name="items">Items.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="items"/> is empty.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if first element of the collection is not
    ///     <see cref="RESPDataType.SimpleString"/>
    ///     (from protocol e.g. "pubsub" and "monitor").</exception>
    public RESPPush(IEnumerable<RESPValue> items)
    {
        this.Items = Ensure.Param(items)
                .AllNotNull()
                .InRange(1, int.MaxValue)
                .That(values => values.First().Type == RESPDataType.SimpleString)
                .ToImmutableArray();
    }

    /// <inheritdoc/>
    public override RESPDataType Type => RESPDataType.Push;

    /// <summary>
    /// Gets collection of the items, it may be empty.
    /// </summary>
    public ImmutableArray<RESPValue> Items { get; }

    /// <inheritdoc/>
    public override uint Length => (uint)this.Items.Length;

    /// <inheritdoc/>
    public override TResult Accept<TResult>(IRESPValueVisitor<TResult> visitor)
    {
        Ensure.Param(visitor).Done();

        return visitor.Visit(this);
    }

    /// <inheritdoc/>
    public override bool Equals(RESPValue? other)
    {
        if (other is RESPPush push && this.Length == push.Length)
        {
            for (int i = 0; i < this.Length; i++)
            {
                if (this.Items[i] != push.Items[i])
                {
                    return false;
                }
            }

            return other.Attribs == this.Attribs;
        }

        return false;
    }

    /// <inheritdoc/>
    public IEnumerator<RESPValue> GetEnumerator()
    {
        return ((IEnumerable<RESPValue>)this.Items).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        HashCode hc = default;

        foreach (RESPValue v in this)
        {
            hc.Add(v);
        }

        hc.Add(this.Attribs);

        return hc.ToHashCode();
    }

    /// <inheritdoc/>
    protected override IEnumerable<RESPValue> GetValues()
    {
        return this;
    }
}
