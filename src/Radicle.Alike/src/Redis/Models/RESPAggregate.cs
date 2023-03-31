namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections.Generic;
using Radicle.Common.Check;

/// <summary>
/// Base class of all aggregate-like RESP values.
/// </summary>
/// <remarks>This includes <see cref="RESPArray"/>, <see cref="RESPAttributeValue"/>,
/// <see cref="RESPMap"/>, <see cref="RESPPush"/> and <see cref="RESPSet"/>.</remarks>
public abstract class RESPAggregate : RESPValue
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RESPAggregate"/> class.
    /// </summary>
    protected internal RESPAggregate()
    {
    }

    /// <summary>
    /// Gets a value indicating whether this aggregate is empty.
    /// </summary>
    public bool IsEmpty => this.Length == 0;

    /// <summary>
    /// Gets length of this aggregate type,
    /// items of pairs in a map.
    /// </summary>
    public abstract uint Length { get; }

    /// <summary>
    /// Gets key-value enumerator.
    /// </summary>
    /// <param name="linearKeyValues">Linear key values,
    ///     see <see cref="GetLinearKeyValues"/>.</param>
    /// <returns>Enumeration of key-value pairs.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if the required parameter is <see langwrod="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="linearKeyValues"/>
    ///     does not have even amount of items.</exception>
    protected static IEnumerable<KeyValuePair<RESPValue, RESPValue>> GetKeyValues(
            IEnumerable<RESPValue> linearKeyValues)
    {
        RESPValue? last = default;
        int length = 0;

        foreach (RESPValue item in Ensure.Param(linearKeyValues))
        {
            length++;

            if (last is not null)
            {
                yield return new KeyValuePair<RESPValue, RESPValue>(last, item);
                last = default;
            }
            else
            {
                last = item;
            }
        }

        if (last is not null)
        {
            throw new ArgumentOutOfRangeException(
                    nameof(linearKeyValues),
                    $"Got odd amount ({length}) items items, even expected.");
        }
    }

    /// <summary>
    /// Gets value enumerator.
    /// </summary>
    /// <returns>Enumeration of values.</returns>
    /// <exception cref="NotSupportedException">Thrown
    ///     if this enumeration is not supported because
    ///     values have keys.</exception>
    protected virtual IEnumerable<RESPValue> GetValues()
    {
        throw new NotSupportedException($"{this.GetType().Name} does not support values");
    }

    /// <summary>
    /// Gets key-value enumerator.
    /// </summary>
    /// <returns>Enumeration of key-value pairs.</returns>
    /// <exception cref="NotSupportedException">Thrown
    ///     if this enumeration is not supported because
    ///     values do not have keys.</exception>
    protected virtual IEnumerable<KeyValuePair<RESPValue, RESPValue>> GetKeyValues()
    {
        throw new NotSupportedException($"{this.GetType().Name} does not support key-values");
    }

    /// <summary>
    /// Gets linearized key-value enumerator.
    /// </summary>
    /// <returns>Enumeration of keys and values as key1, value1, key2, value2, etc.</returns>
    /// <exception cref="NotSupportedException">Thrown
    ///     if this enumeration is not supported because
    ///     values do not have keys.</exception>
    protected IEnumerable<RESPValue> GetLinearKeyValues()
    {
        foreach (KeyValuePair<RESPValue, RESPValue> item in this.GetKeyValues())
        {
            yield return item.Key;

            yield return item.Value;
        }
    }
}
