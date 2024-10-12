namespace Radicle.Common.Compression.Generic;

using System;
using System.Collections.Immutable;
using System.Collections;
using System.Collections.Generic;
using Radicle.Common.Check;
using Radicle.Common.Extensions;
using System.Linq;

/// <summary>
/// Immutable array implementation storing string items
/// compressed.
/// </summary>
internal sealed class ImmutableCList : IImmutableList<string>
{
    public static readonly ImmutableCList Empty = new(Array.Empty<string>());

    /// <summary>
    /// Store of the keys (0, 2, ...) and values (1, 3, ...).
    /// </summary>
    private readonly object[] items;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImmutableCList"/> class.
    /// </summary>
    /// <param name="values">Collection of values.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public ImmutableCList(IEnumerable<string> values)
    {
        int length = Ensure.Param(values).Value.NativeLength();

        object[] items = new object[length];
        int index = 0;

        foreach (string v in values)
        {
            items[index++] = EncodeValue(v);
        }

        this.items = items;
    }

    /// <inheritdoc />
    public string this[int index] => DecodeValue(this.items[index]);

    /// <inheritdoc />
    public int Count => this.items.Length;

    /// <inheritdoc />
    public IImmutableList<string> Add(string value)
    {
        return AddRange(new string[] { value });
    }

    /// <inheritdoc />
    public IImmutableList<string> AddRange(IEnumerable<string> items)
    {
        return new ImmutableCList(this.Concat(items));
    }

    /// <inheritdoc />
    public IImmutableList<string> Clear()
    {
        return Empty;
    }

    /// <inheritdoc />
    public IEnumerator<string> GetEnumerator()
    {
        for (int i = 0; i < this.items.Length; i++)
        {
            yield return DecodeValue(this.items[i]);
        }
    }

    /// <inheritdoc />
    public int IndexOf(string item, int index, int count, IEqualityComparer<string> equalityComparer)
    {
        Ensure.Param(index).NonNegative().Done();
        Ensure.Param(equalityComparer).Done();

        for (int i = index; (i < index + count) && (i < this.items.Length); i++)
        {
            if (equalityComparer.Equals(DecodeValue(this.items[i]), item))
            {
                return i;
            }
        }

        return -1;
    }

    /// <inheritdoc />
    public IImmutableList<string> Insert(int index, string element)
    {
        Ensure.Param(element).Done();

        return this.InsertRange(index, new[] { element });
    }

    /// <inheritdoc />
    public IImmutableList<string> InsertRange(int index, IEnumerable<string> items)
    {
        List<string> newCollection = new(this.Count + items.NativeLength());
        int i = 0;
        bool added = false;

        foreach (string item in this)
        {
            if (i == index)
            {
                newCollection.AddRange(items);
                added = true;
            }

            newCollection.Add(item);

            i++;
        }
        if (!added)
        {
            newCollection.AddRange(items);
        }

        return ZipCollectionsFactory.Default.CreateCompressed(newCollection);
    }

    /// <inheritdoc />
    public int LastIndexOf(string item, int index, int count, IEqualityComparer<string> equalityComparer)
    {
        Ensure.Param(index).NonNegative().Done();
        Ensure.Param(equalityComparer).Done();

        int top = Math.Min(index + count, this.items.Length) - 1;

        if (top >= 0)
        {
            for (int i = top; i >= 0; i--)
            {
                if (equalityComparer.Equals(DecodeValue(this.items[i]), item))
                {
                    return i;
                }
            }
        }

        return -1;
    }

    /// <inheritdoc />
    public IImmutableList<string> Remove(
            string value,
            IEqualityComparer<string> equalityComparer)
    {
        Ensure.Param(value).Done();

        return this.RemoveRange(new[] { value }, equalityComparer);
    }

    /// <inheritdoc />
    public IImmutableList<string> RemoveAll(Predicate<string> match)
    {
        Ensure.Param(match).Done();

        List<string> newCollection = new(this.Count);
        bool removed = false;

        foreach (string item in this)
        {
            if (match(item))
            {
                removed = true;
            }
            else
            {
                newCollection.Add(item);
            }
        }

        if (removed)
        {
            return ZipCollectionsFactory.Default.CreateCompressed(newCollection);
        }

        return this;
    }

    /// <inheritdoc />
    public IImmutableList<string> RemoveAt(int index)
    {
        Ensure.Param(index).InRange(0, this.Count, includeUpper: false).Done();

        List<string> newCollection = new(this.Count);
        int i = 0;

        foreach (string item in this)
        {
            if (i != index)
            {
                newCollection.Add(item);
            }

            i++;
        }

        return ZipCollectionsFactory.Default.CreateCompressed(newCollection);
    }

    /// <inheritdoc />
    public IImmutableList<string> RemoveRange(
            IEnumerable<string> items,
            IEqualityComparer<string> equalityComparer)
    {
        Ensure.Param(items).Done();
        Ensure.Param(equalityComparer).Done();

        List<string> newCollection = new(this.Count);
        bool removed = false;

        foreach (string item in this)
        {
            bool match = false;

            foreach (string probeItem in items)
            {
                if (equalityComparer.Equals(probeItem, item))
                {
                    removed = true;
                    match = true;
                    break;
                }
            }

            if (!match)
            {
                newCollection.Add(item);
            }
        }

        if (removed)
        {
            return ZipCollectionsFactory.Default.CreateCompressed(newCollection);
        }

        return this;
    }

    /// <inheritdoc />
    public IImmutableList<string> RemoveRange(
            int index,
            int count)
    {
        Ensure.Param(index).InRange(0, this.Count, includeUpper: false).Done();
        Ensure.Param(count).NonNegative().Done();

        if (count == 0)
        {
            return this;
        }

        List<string> newCollection = new(this.Count);
        int i = 0;

        foreach (string item in this)
        {
            if ((i < index) || ((index + count - 1) < i))
            {
                newCollection.Add(item);
            }

            i++;
        }

        return ZipCollectionsFactory.Default.CreateCompressed(newCollection);
    }

    /// <inheritdoc />
    public IImmutableList<string> Replace(
            string oldValue,
            string newValue,
            IEqualityComparer<string> equalityComparer)
    {
        Ensure.Param(oldValue).Done();
        Ensure.Param(newValue).Done();
        Ensure.Param(equalityComparer).Done();

        List<string> newCollection = new(this.Count);
        bool replaced = false;

        foreach (string item in this)
        {
            if (equalityComparer.Equals(oldValue, item))
            {
                newCollection.Add(newValue);
                replaced = true;
            }
            else
            {
                newCollection.Add(item);
            }
        }

        if (replaced)
        {
            return ZipCollectionsFactory.Default.CreateCompressed(newCollection);
        }

        return this;
    }

    /// <inheritdoc />
    public IImmutableList<string> SetItem(
            int index,
            string value)
    {
        Ensure.Param(index).InRange(0, this.Count, includeUpper: false).Done();
        Ensure.Param(value).Done();

        List<string> newCollection = new(this.Count);
        int i = 0;

        foreach (string item in this)
        {
            if (i == index)
            {
                newCollection.Add(value);
            }
            else
            {
                newCollection.Add(item);
            }

            i++;
        }

        return ZipCollectionsFactory.Default.CreateCompressed(newCollection);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    private static string DecodeValue(object rawValue)
    {
        return StringPacking.DecodeValue(rawValue);
    }

    private static object EncodeValue(string value)
    {
        return StringPacking.EncodeValue(value);
    }
}
