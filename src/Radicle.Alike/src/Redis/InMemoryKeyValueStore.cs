namespace Radicle.Alike.Redis;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Radicle.Alike.Redis.Models;
using Radicle.Common.Check;
using Radicle.Common.Extensions;

/// <summary>
/// In-memory thread safe key-value store with the support
/// of expiration akin to redis main dictionary.
/// </summary>
/// <remarks>For an idea about enforcing TTL see
/// https://www.youtube.com/watch?v=SyQTG0hXPxY .
/// There are 2 forms of expiration: first on active operation on the key
/// and the second one happens no matter the key access.
/// The second form can be done by random sampling, which stops
/// when we are finding less than e.g. 25% keys to expire (from all
/// keys with the expiration) in case of looped search. See also
/// https://www.youtube.com/watch?v=W8IEzoxRMz4 . Here we stick with constant
/// amount of randomly sampled keys which are checked for eviction.</remarks>
/// <typeparam name="TKey">Type of the keys.</typeparam>
/// <typeparam name="TValue">Type of the values.</typeparam>
public sealed class InMemoryKeyValueStore<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    private readonly object dataLock = new();

    // TODO: replace with unlimitted size map: gcAllowVeryLargeObjects
    private readonly Dictionary<TKey, InMemoryValueWrapper<TValue>> dict = new();

    private readonly Dictionary<TKey, bool> ttlDict = new();

    private readonly byte ttlEvictionSamples;

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryKeyValueStore{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="ttlEvictionSamples">Amount of samples to use for
    ///     on the so eviction of keys with expired time to live.
    ///     The value 0 will effectivelly disables key scanning
    ///     and only evicts expired keys if they are accessed directly.</param>
    public InMemoryKeyValueStore(
            byte ttlEvictionSamples = 5)
    {
        this.ttlEvictionSamples = ttlEvictionSamples;
    }

    /// <summary>
    /// Gets the length of this store, amount of key-values
    /// including those this were not expired to this moment.
    /// </summary>
    public ulong ContemporaryLength => (ulong)this.dict.Count;

    /// <summary>
    /// Check if the <paramref name="key"/> exist in this store.
    /// </summary>
    /// <param name="key">Key to check.</param>
    /// <returns><see langword="true"/> if there is a non-expired
    ///     key-value; <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public bool Contains(TKey key)
    {
        return this.TryGetValue(Ensure.Param(key).Value, out _);
    }

    /// <summary>
    /// Try to retrieve the value stored under the given <paramref name="key"/>.
    /// </summary>
    /// <param name="key">Key to use.</param>
    /// <param name="value">Stored value.</param>
    /// <returns><see langword="true"/> if there is a non-expired
    ///     key-value; <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public bool TryGetValue(
            TKey key,
            [NotNullWhen(returnValue: true)] out TValue? value)
    {
        lock (this.dataLock)
        {
            this.EvictRandomExpiredUnsafe();

            return this.TryGetOrExpireUnsafe(Ensure.Param(key).Value, out value);
        }
    }

    /// <summary>
    /// Try to retrieve value stored under the given <paramref name="key"/>
    /// or, if not present add the value constructed by call to <paramref name="valueFactory"/>
    /// and return the value under the <paramref name="key"/>.
    /// </summary>
    /// <param name="key">Key to check.</param>
    /// <param name="valueFactory">Factory to create value,
    ///     if it is not present under the given <paramref name="key"/>.</param>
    /// <param name="expiration">Expiration of the value
    ///     measured from the time of key-value creation.
    ///     Defaults to no expiration.</param>
    /// <returns>Value stored under the <paramref name="key"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>
    ///     or value produced by the <paramref name="valueFactory"/>
    ///     is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="expiration"/> is zero or negative.</exception>
    public TValue GetOrAdd(
            TKey key,
            Func<TKey, TValue> valueFactory,
            TimeSpan? expiration = null)
    {
        Ensure.Param(key).Done();
        Ensure.Param(valueFactory).Done();
        Ensure.Optional(expiration).StrictlyPositive().Done();

        lock (this.dataLock)
        {
            this.EvictRandomExpiredUnsafe();

            if (this.TryGetOrExpireUnsafe(key, out TValue? result))
            {
                return result;
            }
        }

        // in this way the time for factory run will not block the lock
        TValue value = valueFactory(key) ?? throw new ArgumentNullException(
                    nameof(valueFactory), "Value factory produced null value");

        lock (this.dataLock)
        {
            if (this.TryGetOrExpireUnsafe(key, out TValue? result))
            {
                return result;
            }

            this.SetUnsafe(key, value, expiration: expiration);
            return value;
        }
    }

    /// <summary>
    /// Set given <paramref name="value"/> under the given <paramref name="key"/>
    /// overriding any potential past value.
    /// </summary>
    /// <param name="key">Key to use.</param>
    /// <param name="value">Value to use.</param>
    /// <param name="expiration">Expiration of the value
    ///     measured from the time of key-value creation.
    ///     Defaults to no expiration.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="expiration"/> is zero or negative.</exception>
    public void Set(
            TKey key,
            TValue value,
            TimeSpan? expiration = null)
    {
        Ensure.Param(key).Done();
        Ensure.Param(value).Done();
        Ensure.Optional(expiration).StrictlyPositive().Done();

        lock (this.dataLock)
        {
            this.EvictRandomExpiredUnsafe();

            this.SetUnsafe(key, value, expiration: expiration);
        }
    }

    /// <summary>
    /// Try to remove value stored under given <paramref name="key"/>.
    /// </summary>
    /// <param name="key">Key to remove.</param>
    /// <param name="value">Stored value, if any.</param>
    /// <returns><see langword="true"/> if there was a non-expired
    ///     value under given <paramref name="key"/>
    ///     and was removed; <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public bool TryRemove(
            TKey key,
            [NotNullWhen(returnValue: true)] out TValue? value)
    {
        Ensure.Param(key).Done();

        lock (this.dataLock)
        {
            this.EvictRandomExpiredUnsafe();

            if (this.TryGetOrExpireUnsafe(Ensure.Param(key).Value, out value))
            {
                return this.RemoveUnsafe(key);
            }
        }

        return false;
    }

    /// <summary>
    /// Set given <paramref name="value"/> under the given <paramref name="key"/>
    /// overriding any potential past value.
    /// </summary>
    /// <param name="key">Key to use.</param>
    /// <param name="value">Value to use.</param>
    /// <param name="expiration">Expiration of the value
    ///     measured from the time of key-value creation.
    ///     Defaults to no expiration.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="expiration"/> is zero or negative.</exception>
    private void SetUnsafe(
            TKey key,
            TValue value,
            TimeSpan? expiration = null)
    {
        this.dict[key] = new InMemoryValueWrapper<TValue>(
                value,
                expiration: expiration);

        if (expiration.HasValue)
        {
            this.ttlDict[key] = true;
        }
    }

    /// <summary>
    /// Try to retrieve the value stored under the given <paramref name="key"/>
    /// if not expired.
    /// </summary>
    /// <param name="key">Key to use.</param>
    /// <param name="value">Stored value.</param>
    /// <returns><see langword="true"/> if there is a non-expired
    ///     key-value; <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    private bool TryGetOrExpireUnsafe(
            TKey key,
            [NotNullWhen(returnValue: true)] out TValue? value)
    {
        value = default;
        bool result = false;

        if (this.dict.TryGetValue(key, out InMemoryValueWrapper<TValue> v))
        {
            if (!v.Evict)
            {
                value = v.Value;
                result = true;
            }
            else
            {
                _ = this.RemoveUnsafe(key);
            }
        }

        return result;
    }

    /// <summary>
    /// Remove key-value.
    /// </summary>
    /// <param name="key">Key to remove.</param>
    /// <returns><see langword="true"/> if removed; <see langword="false"/>
    ///     otherwise.</returns>
    private bool RemoveUnsafe(
            TKey key)
    {
        if (this.dict.Remove(key))
        {
            _ = this.ttlDict.Remove(key);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Evict random expired key-values.
    /// </summary>
    private void EvictRandomExpiredUnsafe()
    {
        if (this.ttlEvictionSamples <= 0)
        {
            return;
        }

        byte amount = this.ContemporaryLength > byte.MaxValue
                ? this.ttlEvictionSamples
                : Math.Min(this.ttlEvictionSamples, (byte)this.ContemporaryLength);

        foreach (KeyValuePair<TKey, bool> item in this.ttlDict.RandomPick(amount).ToArray())
        {
            _ = this.TryGetOrExpireUnsafe(item.Key, out _);
        }
    }
}
