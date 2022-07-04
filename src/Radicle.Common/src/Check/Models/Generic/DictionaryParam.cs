namespace Radicle.Common.Check.Models.Generic;

using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Structure of the Typed dictionary parameter.
/// </summary>
/// <typeparam name="TDict">Type of the
///     dictionary parameter.</typeparam>
/// <typeparam name="TItemKey">Type of the collection
///     parameter item key.</typeparam>
/// <typeparam name="TItemValue">Type of the collection
///     parameter item value.</typeparam>
internal readonly struct DictionaryParam<TDict, TItemKey, TItemValue> : IDictionaryParam<TDict, TItemKey, TItemValue>
    where TDict : notnull, IEnumerable<KeyValuePair<TItemKey, TItemValue>>
{
    private readonly TDict valueValue;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="DictionaryParam{TDictionary, TItemKey, TItemValue}"/> struct.
    /// </summary>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="name">Name of the parameter if any, defaults to "n/a".</param>
    internal DictionaryParam(
            TDict value,
            string? name = null)
    {
        Ensure.NotNull(value, parameterName: name ?? "n/a");

        this.Name = name ?? "n/a";
        this.valueValue = value;
        this.IsSpecified = true;
    }

    /// <inheritdoc/>
    public string? Name { get; }

    /// <inheritdoc/>
    public TDict Value
    {
        get
        {
            if (this.IsSpecified)
            {
                return this.valueValue;
            }

            throw new InvalidOperationException(
                    "Can not retrieve value from unspecified parameter.");
        }
    }

    /// <inheritdoc/>
    public IParam<TDict> InnerParam => default(Param<TDict>);

    /// <inheritdoc/>
    public bool IsSpecified { get; }

    /// <inheritdoc/>
    IDictionaryParam<TDict, TItemKey, TItemValue> IDictionaryParam<TDict, TItemKey, TItemValue>.AllNotNull(
            Action<KeyValuePair<TItemKey, TItemValue>>? itemEnsureAction)
    {
        if (this.IsSpecified)
        {
            foreach (KeyValuePair<TItemKey, TItemValue> v in this.Value)
            {
                if (v.Value is null)
                {
                    throw new ArgumentNullException(
                            this.Name,
                            $"Dictionary parameter '{this.Name}' contains null value at key [{v.Key}].");
                }

                try
                {
                    itemEnsureAction?.Invoke(v);
                }
                catch (ArgumentOutOfRangeException rangeExc)
                {
                    throw new ArgumentOutOfRangeException(
                            $"Dictionary parameter '{this.Name}' at key [{v.Key}]: {rangeExc.Message}",
                            rangeExc);
                }
                catch (ArgumentNullException nullExc)
                {
                    throw new ArgumentNullException(
                            $"Dictionary parameter '{this.Name}' at key [{v.Key}]: {nullExc.Message}",
                            nullExc);
                }
                catch (ArgumentException argExc)
                {
                    throw new ArgumentException(
                            $"Dictionary parameter '{this.Name}' at key [{v.Key}]: {argExc.Message}",
                            argExc);
                }
            }
        }

        return this;
    }

    /// <inheritdoc/>
    public ICollectionParam<TDict, KeyValuePair<TItemKey, TItemValue>> AllNotNull(
            Action<KeyValuePair<TItemKey, TItemValue>>? itemEnsureAction = null)
    {
        return ((IDictionaryParam<TDict, TItemKey, TItemValue>)this)
                .AllNotNull(itemEnsureAction: itemEnsureAction);
    }

    /// <inheritdoc/>
    public IDictionaryParam<TDict, TItemKey, TItemValue> AllValuesNotNull(
            Action<TItemValue>? itemValueEnsureAction = null)
    {
        if (this.IsSpecified)
        {
            foreach (KeyValuePair<TItemKey, TItemValue> v in this.Value)
            {
                if (v.Value is null)
                {
                    throw new ArgumentNullException(
                            this.Name,
                            $"Dictionary parameter '{this.Name}' contains null value at key [{v.Key}].");
                }

                try
                {
                    itemValueEnsureAction?.Invoke(v.Value);
                }
                catch (ArgumentOutOfRangeException rangeExc)
                {
                    throw new ArgumentOutOfRangeException(
                            $"Dictionary parameter '{this.Name}' at key [{v.Key}], value error: {rangeExc.Message}",
                            rangeExc);
                }
                catch (ArgumentNullException nullExc)
                {
                    throw new ArgumentNullException(
                            $"Dictionary parameter '{this.Name}' at key [{v.Key}], value error: {nullExc.Message}",
                            nullExc);
                }
                catch (ArgumentException argExc)
                {
                    throw new ArgumentException(
                            $"Dictionary parameter '{this.Name}' at key [{v.Key}], value error: {argExc.Message}",
                            argExc);
                }
            }
        }

        return this;
    }

    /// <inheritdoc/>
    public IDictionaryParam<TDict, TItemKey, TItemValue> AllKeys(
            Action<TItemKey>? itemKeyEnsureAction = null)
    {
        if (this.IsSpecified)
        {
            foreach (KeyValuePair<TItemKey, TItemValue> v in this.Value)
            {
                try
                {
                    itemKeyEnsureAction?.Invoke(v.Key);
                }
                catch (ArgumentOutOfRangeException rangeExc)
                {
                    throw new ArgumentOutOfRangeException(
                            $"Dictionary parameter '{this.Name}' at key [{v.Key}], key error: {rangeExc.Message}",
                            rangeExc);
                }
                catch (ArgumentNullException nullExc)
                {
                    throw new ArgumentNullException(
                            $"Dictionary parameter '{this.Name}' at key [{v.Key}], key error: {nullExc.Message}",
                            nullExc);
                }
                catch (ArgumentException argExc)
                {
                    throw new ArgumentException(
                            $"Dictionary parameter '{this.Name}' at key [{v.Key}], key error: {argExc.Message}",
                            argExc);
                }
            }
        }

        return this;
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TItemKey, TItemValue>> GetEnumerator()
    {
        IEnumerable<KeyValuePair<TItemKey, TItemValue>> e = this.IsSpecified
                ? this.Value
                : Array.Empty<KeyValuePair<TItemKey, TItemValue>>();

        return e.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
