namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of RESP array.
/// </summary>
public sealed class RESPArray : RESPAggregate, IEnumerable<RESPValue>
{
    /// <summary>
    /// Gets human readable name.
    /// </summary>
    public const string HumanName = "array";

    /// <summary>
    /// Instance of empty <see cref="RESPArray"/>.
    /// </summary>
    public static readonly RESPArray Empty = new(Array.Empty<RESPValue>());

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPArray"/> class.
    /// </summary>
    /// <param name="items">Items.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public RESPArray(IEnumerable<RESPValue> items)
    {
        this.Items = Ensure.Param(items)
                .AllNotNull()
                .ToImmutableArray();
    }

    /// <inheritdoc/>
    public override RESPDataType Type => RESPDataType.Array;

    /// <summary>
    /// Gets collection of the items, it may be empty.
    /// </summary>
    public ImmutableArray<RESPValue> Items { get; }

    /// <inheritdoc/>
    public override uint Length => (uint)this.Items.Length;

    /// <summary>
    /// Transform this array to
    /// to <see cref="RESPMap"/> if possible
    /// with items corresponding to
    /// key1, value1, ke2, value2, etc.
    /// </summary>
    /// <returns>Instance of <see cref="RESPMap"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if this <see cref="Length"/> is not even.</exception>
    public RESPMap ToRESPMap()
    {
        return RESPMap.FromRESPArray(this);
    }

    /// <inheritdoc/>
    public override bool Equals(RESPValue? other)
    {
        if (other is RESPArray array && this.Length == array.Length)
        {
            for (int i = 0; i < this.Length; i++)
            {
                if (this.Items[i] != array.Items[i])
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
    public override TResult Accept<TResult>(IRESPValueVisitor<TResult> visitor)
    {
        Ensure.Param(visitor).Done();

        return visitor.Visit(this);
    }

    /// <inheritdoc/>
    protected override IEnumerable<RESPValue> GetValues()
    {
        return this;
    }
}
