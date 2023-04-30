namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of RESP map (dictionary).
/// </summary>
public sealed class RESPMap : RESPAggregate, IEnumerable<KeyValuePair<RESPValue, RESPValue>>
{
    /// <summary>
    /// Gets human readable name.
    /// </summary>
    public const string HumanName = "map";

    /// <summary>
    /// Instance of empty <see cref="RESPMap"/>.
    /// </summary>
    public static readonly RESPMap Empty = new(new Dictionary<RESPValue, RESPValue>());

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPMap"/> class.
    /// </summary>
    /// <param name="items">Items.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public RESPMap(IEnumerable<KeyValuePair<RESPValue, RESPValue>> items)
    {
        this.Items = Ensure.Param(items)
                .AllNotNull()
                .ToImmutableDictionary();
    }

    /// <inheritdoc/>
    public override RESPDataType Type => RESPDataType.Map;

    /// <summary>
    /// Gets collection of the items, it may be empty.
    /// </summary>
    public ImmutableDictionary<RESPValue, RESPValue> Items { get; }

    /// <inheritdoc/>
    public override uint Length => (uint)this.Items.Count;

    /// <summary>
    /// Explicitly convert <see cref="RESPMap"/> to <see cref="RESPArray"/>.
    /// </summary>
    /// <param name="map">Instance of map to convert.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static explicit operator RESPArray(RESPMap map)
    {
        return ToRESPArray(map);
    }

    /// <summary>
    /// Explicitly convert <see cref="RESPArray"/> to <see cref="RESPMap"/>.
    /// </summary>
    /// <param name="array">Instance of array to convert.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <see cref="Length"/> is not even amount.</exception>
    public static explicit operator RESPMap(RESPArray array)
    {
        return FromRESPArray(array);
    }

    /// <summary>
    /// Transform given <paramref name="map"/>
    /// to <see cref="RESPArray"/> with items corresponding to
    /// key1, value1, ke2, value2, etc.
    /// </summary>
    /// <param name="map">Map to transform.</param>
    /// <returns>Instance of <see cref="RESPArray"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static RESPArray ToRESPArray(RESPMap map)
    {
        return new RESPArray(Ensure.Param(map).Value.GetLinearKeyValues());
    }

    /// <summary>
    /// Transform given <paramref name="array"/>
    /// to <see cref="RESPMap"/> from items corresponding to
    /// key1, value1, ke2, value2, etc.
    /// </summary>
    /// <param name="array">Array to transform.</param>
    /// <returns>Instance of <see cref="RESPMap"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="array"/> length is not even.</exception>
    public static RESPMap FromRESPArray(RESPArray array)
    {
        return new RESPMap(GetKeyValues(Ensure.Param(array).Value));
    }

    /// <summary>
    /// Transform this map
    /// to <see cref="RESPArray"/> with items corresponding to
    /// key1, value1, ke2, value2, etc.
    /// See <see cref="ToRESPArray(RESPMap)"/>.
    /// </summary>
    /// <returns>Instance of <see cref="RESPArray"/>.</returns>
    public RESPArray ToRESPArray()
    {
        return ToRESPArray(this);
    }

    /// <inheritdoc/>
    public override TResult Accept<TResult>(IRESPValueVisitor<TResult> visitor)
    {
        Ensure.Param(visitor).Done();

        return visitor.Visit(this);
    }

    /// <inheritdoc/>
    public override bool Equals(RESPValue? other)
    {
        if (other is RESPMap map && this.Length == map.Length)
        {
            foreach (KeyValuePair<RESPValue, RESPValue> pair in this)
            {
                if (!map.Items.ContainsKey(pair.Key) || map.Items[pair.Key] != pair.Value)
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
    protected override IEnumerable<KeyValuePair<RESPValue, RESPValue>> GetKeyValues()
    {
        return this;
    }
}
