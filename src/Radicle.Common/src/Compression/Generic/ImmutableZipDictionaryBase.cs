namespace Radicle.Common.Compression.Generic;

using System;
using System.Collections.Immutable;
using System.Collections;
using System.Collections.Generic;
using Radicle.Common.Check;
using System.Linq;

/// <summary>
/// Immutable zip dictionary implementation base storing items
/// as linear list, for memory efficiency with small dictionary lengths (up to ~256).
/// </summary>
/// <typeparam name="TKey">Type of the key.</typeparam>
/// <typeparam name="TValue">Type of the value.</typeparam>
internal abstract class ImmutableZipDictionaryBase<TKey, TValue> : IImmutableDictionary<TKey, TValue>
        where TKey : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImmutableZipDictionary{TKey, TValue}"/> class.
    /// </summary>
    internal ImmutableZipDictionaryBase()
    {
    }

    /// <inheritdoc />
    public abstract IEnumerable<TKey> Keys { get; }

    /// <inheritdoc />
    public abstract IEnumerable<TValue> Values { get; }

    /// <inheritdoc />
    public abstract int Count { get; }

    /// <inheritdoc />
    public TValue this[TKey key]
    {
        get
        {
            Ensure.Param(key).Done();

            if (this.TryGetValue(key, out TValue found))
            {
                return found;
            }

            throw new KeyNotFoundException($"Key {key} could not be found in dictionary");
        }
    }

    /// <inheritdoc />
    public bool ContainsKey(TKey key)
    {
        return this.TryGetValue(key, out _);
    }

    /// <inheritdoc />
    public abstract bool TryGetValue(
            TKey key,
            out TValue value);

    /// <inheritdoc />
    public abstract IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    /// <inheritdoc />
    public IImmutableDictionary<TKey, TValue> Clear()
    {
        return ImmutableZipDictionary<TKey, TValue>.Empty;
    }

    // TODO: add overrides for volatile methods in compressed derived class

    /// <inheritdoc />
    public virtual IImmutableDictionary<TKey, TValue> Add(
            TKey key,
            TValue value)
    {
        Ensure.Param(key).Done();

        KeyValuePair<TKey, TValue> newValue = new(key, value);

        if (this.Contains(newValue))
        {
            return this;
        }
        else if (this.ContainsKey(key))
        {
            throw new ArgumentException(
                    $"Key {key} is already present in the dictionary with different value",
                    nameof(key));
        }

        // ZipCollections. ...
        return ZipCollectionsFactory.Default.Create(
                this.Concat(new KeyValuePair<TKey, TValue>[] { newValue }));
    }

    /// <inheritdoc />
    public IImmutableDictionary<TKey, TValue> AddRange(
            IEnumerable<KeyValuePair<TKey, TValue>> pairs)
    {
        IImmutableDictionary<TKey, TValue> result = this;

        foreach (KeyValuePair<TKey, TValue> pair in pairs)
        {
            result = result.Add(pair.Key, pair.Value);
        }

        return result;
    }

    /// <inheritdoc />
    public IImmutableDictionary<TKey, TValue> SetItem(
            TKey key,
            TValue value)
    {
        if (this.Contains(new KeyValuePair<TKey, TValue>(key, value)))
        {
            return this;
        }

        List<KeyValuePair<TKey, TValue>> newCollection = new(this.Count);
        bool overriden = false;

        foreach (KeyValuePair<TKey, TValue> item in this)
        {
            if (Equals(item.Key, key))
            {
                newCollection.Add(new KeyValuePair<TKey, TValue>(key, value));
                overriden = true;
            }
            else
            {
                newCollection.Add(item);
            }
        }

        if (!overriden)
        {
            newCollection.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        return ZipCollectionsFactory.Default.Create(newCollection);
    }

    /// <inheritdoc />
    public IImmutableDictionary<TKey, TValue> SetItems(IEnumerable<KeyValuePair<TKey, TValue>> items)
    {
        IImmutableDictionary<TKey, TValue> result = this;

        foreach (KeyValuePair<TKey, TValue> item in items)
        {
            result = result.SetItem(item.Key, item.Value);
        }

        return result;
    }

    /// <inheritdoc />
    public IImmutableDictionary<TKey, TValue> Remove(TKey key)
    {
        if (this.ContainsKey(key))
        {
            List<KeyValuePair<TKey, TValue>> newCollection = new(this.Count);

            foreach (KeyValuePair<TKey, TValue> item in this)
            {
                if (!Equals(item.Key, key))
                {
                    newCollection.Add(item);
                }
            }

            return ZipCollectionsFactory.Default.Create(newCollection);
        }

        return this;
    }

    /// <inheritdoc />
    public IImmutableDictionary<TKey, TValue> RemoveRange(IEnumerable<TKey> keys)
    {
        IImmutableDictionary<TKey, TValue> result = this;

        foreach (TKey key in keys)
        {
            result = result.Remove(key);
        }

        return result;
    }

    /// <inheritdoc />
    public bool Contains(KeyValuePair<TKey, TValue> pair)
    {
        if (this.TryGetValue(pair.Key, out TValue value))
        {
            return Equals(value, pair.Value);
        }

        return false;
    }

    /// <inheritdoc />
    public bool TryGetKey(TKey equalKey, out TKey actualKey)
    {
        actualKey = default!;

        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            if (Equals(pair.Key, equalKey))
            {
                actualKey = pair.Key;

                return true;
            }
        }

        return false;
    }

    protected virtual TValue DecodeValue(object rawValue)
    {
        return (TValue)rawValue;
    }

    protected virtual object EncodeValue(TValue value)
    {
        return value!;
    }
}
