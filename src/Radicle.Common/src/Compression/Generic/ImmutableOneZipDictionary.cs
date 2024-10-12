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
internal class ImmutableOneZipDictionary<TKey, TValue> : ImmutableZipDictionaryBase<TKey, TValue>
        where TKey : notnull
{
    protected readonly TKey key0;

    protected readonly object value0;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImmutableOneZipDictionary{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="keyValue0">Key-value pair.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public ImmutableOneZipDictionary(
            KeyValuePair<TKey, TValue> keyValue0)
    {
        this.key0 = Ensure.Param(keyValue0.Key).Value;
        this.value0 = this.EncodeValue(keyValue0.Value);
    }

    /// <inheritdoc />
    public override IEnumerable<TKey> Keys
    {
        get
        {
            yield return this.key0;
        }
    }

    /// <inheritdoc />
    public override IEnumerable<TValue> Values
    {
        get
        {
            yield return this.DecodeValue(this.value0);
        }
    }

    /// <inheritdoc />
    public override int Count => 1;

    /// <inheritdoc />
    public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        yield return new KeyValuePair<TKey, TValue>(
                this.key0,
                this.DecodeValue(this.value0));
    }

    /// <inheritdoc />
    public override bool TryGetValue(TKey key, out TValue value)
    {
        value = default!;

        if (Equals(key, this.key0))
        {
            value = this.DecodeValue(this.value0);
            return true;
        }

        return false;
    }
}
