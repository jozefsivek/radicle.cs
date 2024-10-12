namespace Radicle.Common.Compression.Generic;

using System;
using System.Collections.Generic;
using Radicle.Common.Check;
using Radicle.Common.Extensions;

/// <summary>
/// Immutable dictionary implementation storing items
/// as linear list for memory efficiency with small dictionary lengths (~256).
/// </summary>
/// <typeparam name="TKey">Type of the key.</typeparam>
/// <typeparam name="TValue">Type of the value.</typeparam>
internal class ImmutableZipDictionary<TKey, TValue> : ImmutableZipDictionaryBase<TKey, TValue>
        where TKey : notnull
{
    public static readonly ImmutableZipDictionary<TKey, TValue> Empty = new(Array.Empty<KeyValuePair<TKey, TValue>>());

    /// <summary>
    /// Store of the keys (0, 2, ...) and values (1, 3, ...).
    /// </summary>
    protected readonly object[] items;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImmutableZipDictionary{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="keyValues">Collection of key-values.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public ImmutableZipDictionary(
            IEnumerable<KeyValuePair<TKey, TValue>> keyValues)
    {
        int length = Ensure.Param(keyValues).Value.NativeLength();

        object[] items = new object[length * 2];
        int index = 0;

        foreach (KeyValuePair<TKey, TValue> kv in keyValues)
        {
            items[index++] = Ensure.Param(kv.Key).Value;
            items[index++] = this.EncodeValue(kv.Value);
        }

        this.items = items;
    }

    /// <inheritdoc />
    public override IEnumerable<TKey> Keys
    {
        get
        {
            for (int i = 0; i < this.items.Length; i += 2)
            {
                yield return (TKey)this.items[i];
            }
        }
    }

    /// <inheritdoc />
    public override IEnumerable<TValue> Values
    {
        get
        {
            for (int i = 1; i < this.items.Length; i += 2)
            {
                yield return this.DecodeValue(this.items[i]);
            }
        }
    }

    /// <inheritdoc />
    public override int Count => this.items.Length / 2;

    /// <inheritdoc />
    public override bool TryGetValue(
            TKey key,
            out TValue value)
    {
        Ensure.Param(key).Done();

        value = default!;

        for (int i = 0; i < this.items.Length; i += 2)
        {
            if (Equals(key, this.items[i]))
            {
                value = this.DecodeValue(this.items[i + 1]);
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc />
    public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < this.items.Length; i += 2)
        {
            yield return new KeyValuePair<TKey, TValue>(
                    (TKey)this.items[i],
                    this.DecodeValue(this.items[i + 1]));
        }
    }
}
