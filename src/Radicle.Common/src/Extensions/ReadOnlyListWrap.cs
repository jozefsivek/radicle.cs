namespace Radicle.Common.Extensions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Radicle.Common.Check;

/// <summary>
/// Wrapper for a list like <see cref="IList{T}"/>
/// or <see cref="IReadOnlyList{T}"/>.
/// </summary>
/// <typeparam name="T">Type of the items in the list.</typeparam>
internal sealed class ReadOnlyListWrap<T> : IReadOnlyList<T>
{
    private readonly bool readOnly;

    private readonly bool enumerable;

    private readonly IList<T> innerList = default!;

    private readonly IReadOnlyList<T> innerReadOnlyList = default!;

    private readonly IEnumerable<T> innerEnumerable = default!;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyListWrap{T}"/> class.
    /// </summary>
    /// <param name="inner">Inner list or enumerable. Note
    /// that pure enumerables will have worse performance
    /// when accessing items by index and the order
    /// is not guaranteed if the original enumerable does not
    /// gurantee it.</param>
    public ReadOnlyListWrap(IEnumerable<T> inner)
    {
        Ensure.Param(inner).Done();

        if (inner is IList<T> list)
        {
            this.innerList = list;
            this.readOnly = false;
        }
        else if (inner is IReadOnlyList<T> readOnlyList)
        {
            this.innerReadOnlyList = readOnlyList;
            this.readOnly = true;
        }
        else
        {
            this.enumerable = true;
        }

        this.innerEnumerable = inner;
    }

    /// <inheritdoc/>
    public int Count
    {
        get
        {
            if (this.enumerable)
            {
                return this.innerEnumerable.Count();
            }
            else if (this.readOnly)
            {
                return this.innerReadOnlyList.Count;
            }

            return this.innerList.Count;
        }
    }

    /// <inheritdoc/>
    public T this[int index]
    {
        get
        {
            if (index < 0)
            {
                Ensure.Param(index).NonNegative().Done();
            }

            if (this.enumerable)
            {
                int possition = 0;

                foreach (T item in this.innerEnumerable)
                {
                    if (possition == index)
                    {
                        return item;
                    }

                    possition++;
                }

                throw new ArgumentOutOfRangeException(
                        nameof(index),
                        index,
                        $"Index {index} was outside the bounds of the enumerable");
            }
            else if (this.readOnly)
            {
                try
                {
                    return this.innerReadOnlyList[index];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(index),
                        index,
                        $"Index {index} was outside the bounds of the read only list");
                }
            }

            try
            {
                return this.innerList[index];
            }
            catch (IndexOutOfRangeException)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(index),
                    index,
                    $"Index {index} was outside the bounds of the read only list");
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return this.innerEnumerable.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
