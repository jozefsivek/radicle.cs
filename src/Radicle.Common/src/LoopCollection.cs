namespace Radicle.Common;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Radicle.Common.Check;

/// <summary>
/// Looping immutable collection with mutable cursor to support looping, i.e.
/// wrapping around.
/// </summary>
/// <typeparam name="T">Type of the collcetion item.</typeparam>
public sealed class LoopCollection<T> : IEnumerable<T>
{
    private readonly ImmutableArray<T> collection;

    private int cursor;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoopCollection{T}"/> class.
    /// </summary>
    /// <param name="elements">Elements of collection
    ///     which will be looped over.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="elements"/> is empty.</exception>
    public LoopCollection(
            IEnumerable<T> elements)
    {
        this.collection = Ensure.Param(elements)
                .NotEmpty()
                .ToImmutableArray();
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)this.collection).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    /// <summary>
    /// Advance in position forward and return element on that position.
    /// </summary>
    /// <returns>Element on next position.</returns>
    public T Next()
    {
        this.cursor++;

        if (this.cursor >= this.collection.Length)
        {
            this.cursor = 0;
        }

        return this.collection[this.cursor];
    }

    /// <summary>
    /// Advance in position backwards and return element on that position.
    /// </summary>
    /// <returns>Element on previous position.</returns>
    public T Prev()
    {
        this.cursor--;

        if (this.cursor < 0)
        {
            this.cursor = this.collection.Length - 1;
        }

        return this.collection[this.cursor];
    }
}
