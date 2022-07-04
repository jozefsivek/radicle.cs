namespace Radicle.Common.Check.Models.Generic;

using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Structure of the typed collection parameter.
/// </summary>
/// <typeparam name="TCollection">Type of the collection
///     parameter value.</typeparam>
/// <typeparam name="TItem">Type of the collection
///     parameter item value.</typeparam>
internal readonly struct CollectionParam<TCollection, TItem> : ICollectionParam<TCollection, TItem>
        where TCollection : notnull, IEnumerable<TItem>
{
    private readonly TCollection valueValue;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="CollectionParam{TCollection, TItem}"/> struct.
    /// </summary>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="name">Name of the parameter if any, defaults to "n/a".</param>
    internal CollectionParam(
            TCollection value,
            string? name = "n/a")
    {
        Ensure.NotNull(value, parameterName: name ?? "n/a");

        this.Name = name ?? "n/a";
        this.valueValue = value;
        this.IsSpecified = true;
    }

    /// <inheritdoc/>
    public string? Name { get; }

    /// <inheritdoc/>
    public TCollection Value
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
    public IParam<TCollection> InnerParam => default(Param<TCollection>);

    /// <inheritdoc/>
    public bool IsSpecified { get; }

    /// <inheritdoc/>
    public ICollectionParam<TCollection, TItem> AllNotNull(Action<TItem>? itemEnsureAction = null)
    {
        if (this.IsSpecified)
        {
            int index = 0;

            foreach (TItem v in this.Value)
            {
                if (v is null)
                {
                    throw new ArgumentNullException(
                            this.Name,
                            $"Collection parameter '{this.Name}' contains null value at index [{index}].");
                }

                try
                {
                    itemEnsureAction?.Invoke(v);
                }
                catch (ArgumentOutOfRangeException rangeExc)
                {
                    throw new ArgumentOutOfRangeException(
                            $"Collection parameter '{this.Name}' at index [{index}]: {rangeExc.Message}",
                            rangeExc);
                }
                catch (ArgumentNullException nullExc)
                {
                    throw new ArgumentNullException(
                            $"Collection parameter '{this.Name}' at index [{index}]: {nullExc.Message}",
                            nullExc);
                }
                catch (ArgumentException argExc)
                {
                    throw new ArgumentException(
                            $"Collection parameter '{this.Name}' at index [{index}]: {argExc.Message}",
                            argExc);
                }

                index++;
            }
        }

        return this;
    }

    /// <inheritdoc/>
    public IEnumerator<TItem> GetEnumerator()
    {
        IEnumerable<TItem> e = this.IsSpecified ? this.Value : Array.Empty<TItem>();

        return e.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
