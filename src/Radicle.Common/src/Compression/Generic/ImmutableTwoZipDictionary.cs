namespace Radicle.Common.Compression.Generic;

using System;
using System.Collections.Generic;
using Radicle.Common.Check;

/// <summary>
/// Immutable disctionary implementation storing items
/// as linear list for memory efficiency with small dictionary lengths (~256).
/// </summary>
/// <typeparam name="TKey">Type of the key.</typeparam>
/// <typeparam name="TValue">Type of the value.</typeparam>
internal class ImmutableTwoZipDictionary<TKey, TValue> : ImmutableOneZipDictionary<TKey, TValue>
        where TKey : notnull
{
    protected readonly TKey key1;

    protected readonly object value1;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImmutableTwoZipDictionary{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="keyValue0">Key-value pair 0.</param>
    /// <param name="keyValue1">Key-value pair 1.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public ImmutableTwoZipDictionary(
            KeyValuePair<TKey, TValue> keyValue0,
            KeyValuePair<TKey, TValue> keyValue1)
        : base(keyValue0)
    {
        this.key1 = Ensure.Param(keyValue1.Key).Value;
        this.value1 = this.EncodeValue(keyValue1.Value);
    }

    /// <inheritdoc />
    public override IEnumerable<TKey> Keys
    {
        get
        {
            foreach (TKey k in base.Keys)
            {
                yield return k;
            }

            yield return this.key1;
        }
    }

    /// <inheritdoc />
    public override IEnumerable<TValue> Values
    {
        get
        {
            foreach (TValue v in base.Values)
            {
                yield return v;
            }

            yield return this.DecodeValue(this.value1);
        }
    }

    /// <inheritdoc />
    public override int Count => 2;

    /// <inheritdoc />
    public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        yield return new KeyValuePair<TKey, TValue>(
                this.key0,
                this.DecodeValue(this.value0));

        yield return new KeyValuePair<TKey, TValue>(
                this.key1,
                this.DecodeValue(this.value1));
    }

    /// <inheritdoc />
    public override bool TryGetValue(TKey key, out TValue value)
    {
        if (Equals(key, this.key1))
        {
            value = this.DecodeValue(this.value1);
            return true;
        }

        return base.TryGetValue(key, out value);
    }
}
