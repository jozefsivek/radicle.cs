namespace Radicle.Common;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Radicle.Common.Check;

/// <summary>
/// Mutable non-thread safe rolling collection which keeps specified maximum
/// amount of recent values. Any new addition after reaching
/// the capacity will cause eviction of the earlies values.
/// </summary>
/// <remarks>
/// <para>
/// Why this exists? Imagine you want to store n recent values,
/// but you do not want to have overhead of linked list or
/// CPU work required to trim some other list structure.
/// So this is it a fixed (maximum) sized collection which does that
/// aimed for low memory and CPU overhead.
/// </para>
/// <para>
/// The maximum capacity is set to be <see cref="ushort"/>,
/// because the array of internally stored values is set to this size
/// to avoid rescaling and prioritize high throughput.
/// </para>
/// <para>
/// Note the methods like <see cref="Remove(T)"/> or <see cref="Contains(T)"/>
/// are provided as nicety not as a performant feature.
/// </para>
/// </remarks>
/// <typeparam name="T">Type of the values.</typeparam>
public sealed class RollingCollection<T> : ICollection<T>
{
    /* How are the values stored:
     *
     * Case for count equal to 3:
     *
     *    used          free
     * ,---^----, ,------^------,
     *   _  _  _
     * [ 0, 1, 2, 3, 4, ....., n ] - where n  capacity
     *         A
     *         `- cursor pointing to last added value or -1 if empty
     *
     * - enumeration of values will yield
     *   (from earliest to latest added value):
     *   [0, 1, 2]
     *
     *
     * Case for count equal to capacity:
     *
     *             used
     * ,------------^-----------,
     *   _  _  _  _  _         _
     * [ 0, 1, 2, 3, 4, ....., n ] - where n  capacity
     *         A
     *         `- cursor pointing to last added value or -1 if empty
     *
     * - enumeration of values will yield:
     *   [3, 4, ..., n, 0, 1, 2]
     * - whenever an item is removed, the items are reorganized
     *   so that a gap of free places in the values is allways
     *   at the end after the cursor position.
     */

    private readonly T[] values;

    private int cursor = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="RollingCollection{T}"/> class.
    /// </summary>
    /// <param name="capacity">Desired capacity of the buffer.</param>
    public RollingCollection(ushort capacity)
    {
        this.values = new T[Ensure.Param(capacity).Value];
    }

    /// <inheritdoc/>
    public int Count { get; private set; }

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public void Add(T item)
    {
        if (this.values.Length == 0)
        {
            return;
        }

        this.cursor = (this.cursor + 1) % this.values.Length;

        if (this.cursor >= this.values.Length)
        {
            this.cursor = 0;
        }

        this.values[this.cursor] = item;
        this.Count = Math.Clamp(this.Count + 1, 0, this.values.Length);
    }

    /// <summary>
    /// Try to retrieve last value.
    /// </summary>
    /// <param name="item">Item to add.</param>
    /// <param name="t">Evicted value as a result of addition, if any.</param>
    /// <returns><see langword="true"/> if this instance
    ///     contains at least one value.</returns>
    public bool AddAndTryGetEvicted(T item, [MaybeNullWhen(returnValue: false)] out T t)
    {
        t = default;

        if (this.Count == this.values.Length)
        {
            t = this.values[(this.cursor + 1) % this.values.Length];

            return true;
        }

        this.Add(item);

        return false;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        this.cursor = -1;
        this.Count = 0;

        for (int i = 0; i < this.values.Length; i++)
        {
            this.values[i] = default!;
        }
    }

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        foreach (T? t in this)
        {
            if (Equals(t, item))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        Ensure.Param(array).Done();
        Ensure.Param(arrayIndex).NonNegative().Done();

        if (array.Rank > 1)
        {
            throw new ArgumentException("Can not copy to multidimensional array.");
        }

        if (this.Count > (array.Length - arrayIndex))
        {
            throw new ArgumentException("Can not copy to smaller space in array.");
        }

        foreach (T? t in this)
        {
            array[arrayIndex++] = t;
        }
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        if (this.Count > 0)
        {
            int start;

            if (this.Count == this.values.Length)
            {
                start = (this.cursor + 1) % this.values.Length;
            }
            else
            {
                start = 0;
            }

            for (int i = 0; i < this.Count; i++)
            {
                yield return this.values[start++ % this.values.Length];
            }
        }
    }

    /// <summary>
    /// Try to retrieve last value.
    /// </summary>
    /// <param name="t">Retrieved value, if any.</param>
    /// <returns><see langword="true"/> if this instance
    ///     contains at least one value.</returns>
    public bool TryGetLast([MaybeNullWhen(returnValue: false)] out T t)
    {
        t = default;

        if (this.cursor >= 0)
        {
            t = this.values[this.cursor];

            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        List<T> copy = new(this.Count);
        bool success = false;

        foreach (T? t in this)
        {
            if (Equals(t, item))
            {
                success = true;
            }
            else
            {
                copy.Add(t);
            }
        }

        if (success)
        {
            this.Clear();

            foreach (T t in copy)
            {
                this.Add(t);
            }
        }

        return success;
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
