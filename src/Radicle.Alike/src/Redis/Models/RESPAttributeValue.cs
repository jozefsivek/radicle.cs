namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of RESP attribute (dictionary).
/// </summary>
/// <remarks>This value is special because it does not
///     count into the lengths of containing aggregate values.
///     I.e. an array of 3 elements where one element is preceeded
///     by the RESP attribute will actually have 4 elements in total:
///     "When a client reads a reply and encounters an attribute type,
///     it should read the attribute, and continue reading the reply.
///     The attribute reply should be accumulated separately ...
///     ... Attributes can appear anywhere before a valid part of the protocol
///     identifying a given type, and will inform only the part of
///     the reply that immediately follows" ( https://github.com/redis/redis-specifications/blob/master/protocol/RESP3.md#attribute-type ).</remarks>
public sealed class RESPAttributeValue : RESPAggregate, IEnumerable<KeyValuePair<RESPValue, RESPValue>>
{
    /// <summary>
    /// Gets human readable name.
    /// </summary>
    public const string HumanName = "attribute map";

    /// <summary>
    /// Instance of empty <see cref="RESPAttributeValue"/>.
    /// </summary>
    public static readonly RESPAttributeValue Empty = new(new Dictionary<RESPValue, RESPValue>());

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPAttributeValue"/> class.
    /// </summary>
    /// <param name="items">Items.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public RESPAttributeValue(IEnumerable<KeyValuePair<RESPValue, RESPValue>> items)
    {
        this.Items = Ensure.Param(items)
                .AllNotNull()
                .ToImmutableDictionary();
    }

    /// <inheritdoc/>
    public override RESPDataType Type => RESPDataType.Attribute;

    /// <summary>
    /// Gets collection of the items, it may be empty.
    /// </summary>
    public ImmutableDictionary<RESPValue, RESPValue> Items { get; }

    /// <inheritdoc/>
    public override uint Length => (uint)this.Items.Count;

    /// <inheritdoc/>
    public override bool Equals(RESPValue? other)
    {
        if (other is RESPAttributeValue map && this.Length == map.Length)
        {
            foreach (KeyValuePair<RESPValue, RESPValue> pair in this)
            {
                bool contains = map.Items.ContainsKey(pair.Key);

                if (!contains || map.Items[pair.Key] != pair.Value)
                {
                    return false;
                }
            }

            return other.Attribs == this.Attribs;
        }

        return false;
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<RESPValue, RESPValue>> GetEnumerator()
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
        foreach (KeyValuePair<RESPValue, RESPValue> pair in this)
        {
            hc.Add(pair.Key);
            hc.Add(pair.Value);
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
    protected override IEnumerable<KeyValuePair<RESPValue, RESPValue>> GetKeyValues()
    {
        return this;
    }
}
