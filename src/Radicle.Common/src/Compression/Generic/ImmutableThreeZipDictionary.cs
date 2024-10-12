namespace Radicle.Common.Compression.Generic;

using System;
using System.Collections.Generic;
using Radicle.Common.Check;

/// <summary>
/// Immutable dictionary implementation storing items
/// as linear list for memory efficiency with small dictionary lengths (~256).
/// </summary>
/// <typeparam name="TKey">Type of the key.</typeparam>
/// <typeparam name="TValue">Type of the value.</typeparam>
internal class ImmutableThreeZipDictionary<TKey, TValue> : ImmutableTwoZipDictionary<TKey, TValue>
        where TKey : notnull
{
    protected readonly TKey key2;

    protected readonly object value2;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImmutableThreeZipDictionary{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="keyValue0">Key-value pair 0.</param>
    /// <param name="keyValue1">Key-value pair 1.</param>
    /// <param name="keyValue2">Key-value pair 2.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public ImmutableThreeZipDictionary(
            KeyValuePair<TKey, TValue> keyValue0,
            KeyValuePair<TKey, TValue> keyValue1,
            KeyValuePair<TKey, TValue> keyValue2)
        : base(keyValue0, keyValue1)
    {
        this.key2 = Ensure.Param(keyValue2.Key).Value;
        this.value2 = this.EncodeValue(keyValue2.Value);
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

            yield return this.key2;
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

            yield return this.DecodeValue(this.value2);
        }
    }

    /// <inheritdoc />
    public override int Count => 3;

    /// <inheritdoc />
    public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        yield return new KeyValuePair<TKey, TValue>(
                this.key0,
                this.DecodeValue(this.value0));

        yield return new KeyValuePair<TKey, TValue>(
                this.key1,
                this.DecodeValue(this.value1));

        yield return new KeyValuePair<TKey, TValue>(
                this.key2,
                this.DecodeValue(this.value2));
    }

    /// <inheritdoc />
    public override bool TryGetValue(TKey key, out TValue value)
    {
        if (Equals(key, this.key2))
        {
            value = this.DecodeValue(this.value2);
            return true;
        }

        return base.TryGetValue(key, out value);
    }
}
