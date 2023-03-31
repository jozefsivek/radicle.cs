namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of RESP set.
/// </summary>
public sealed class RESPSet : RESPAggregate, IEnumerable<RESPValue>
{
    /// <summary>
    /// Gets human readable name.
    /// </summary>
    public const string HumanName = "set";

    /// <summary>
    /// Instance of empty <see cref="RESPSet"/>.
    /// </summary>
    public static readonly RESPSet Empty = new(Array.Empty<RESPValue>());

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPSet"/> class.
    /// </summary>
    /// <param name="items">Items.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public RESPSet(IEnumerable<RESPValue> items)
    {
        this.Items = Ensure.Param(items)
                .AllNotNull()
                .ToImmutableHashSet();
    }

    /// <inheritdoc/>
    public override RESPDataType Type => RESPDataType.Set;

    /// <summary>
    /// Gets collection of the items, it may be empty.
    /// </summary>
    public ImmutableHashSet<RESPValue> Items { get; }

    /// <inheritdoc/>
    public override uint Length => (uint)this.Items.Count;

    /// <inheritdoc/>
    public override TResult Accept<TResult>(IRESPValueVisitor<TResult> visitor)
    {
        Ensure.Param(visitor).Done();

        return visitor.Visit(this);
    }

    /// <inheritdoc/>
    public override bool Equals(RESPValue? other)
    {
        if (other is RESPSet map && this.Length == map.Length)
        {
            foreach (RESPValue item in this)
            {
                if (!map.Items.Contains(item))
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
        return this.Items.GetEnumerator();
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

        // TODO: how to avoid this ... because Add will feel the order
        foreach (RESPValue item in this)
        {
            hc.Add(item);
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
