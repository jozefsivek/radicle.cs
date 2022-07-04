namespace Radicle.Common.Check.Models.Generic;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

/// <summary>
/// Interface of generic colection parameter.
/// </summary>
/// <typeparam name="T">Type of the collection
///     parameter value.</typeparam>
/// <typeparam name="TItem">Type of the collection
///     parameter item value.</typeparam>
public interface ICollectionParam<T, TItem> : IParam<T>, IEnumerable<TItem>
        where T : notnull, IEnumerable<TItem>
{
    /// <summary>
    /// General evaluation of validity according to given
    /// <paramref name="predicate"/>, if the <paramref name="predicate"/>
    /// returns <see langword="false"/> the parameter value
    /// is treated as invalid and exception is thrown.
    /// Whenever possible, use more specific guard function.
    /// </summary>
    /// <param name="predicate">Predicate function which
    ///     determines if the parameter value is valid,
    ///     i.e. <paramref name="predicate"/> returns
    ///     <see langword="true"/>.</param>
    /// <param name="messageFactory">Optional message factory
    ///     which is used to construct message of <see cref="ArgumentException"/>
    ///     thrown if <paramref name="predicate"/> returns
    ///     <see langword="false"/>.</param>
    /// <returns>This instance of <see cref="Param{TValue}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if
    ///     <paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="predicate"/>
    ///     returns <see langword="false"/>.</exception>
    new ICollectionParam<T, TItem> That(
            Func<T, bool> predicate,
            Func<IParam<T>, string>? messageFactory = null)
    {
        Ensure.NotNull(predicate);

        Param<T>.StaticThat(this, predicate, messageFactory: messageFactory);

        return this;
    }

    /// <summary>
    /// Checks if the specified collection parameter value is empty.
    /// </summary>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when
    ///     the collection is emoty.</exception>
    ICollectionParam<T, TItem> NotEmpty()
    {
        if (this.IsSpecified)
        {
            bool empty;

            if (this.Value is Array a)
            {
                empty = a.Length == 0;
            }
            else if (this.Value is IList<TItem> l)
            {
                empty = l.Count == 0;
            }
            else if (this.Value is ICollection<TItem> c)
            {
                empty = c.Count == 0;
            }
            else if (this.Value is IImmutableList<TItem> il)
            {
                empty = il.Count == 0;
            }
            else if (this.Value is IDictionary d)
            {
                empty = d.Count == 0;
            }
            else if (this.Value is IReadOnlyCollection<TItem> rc)
            {
                empty = rc.Count == 0;
            }
            else
            {
                empty = !this.Value.Any();
            }

            if (empty)
            {
                string description = this.Description;

                throw new ArgumentOutOfRangeException(
                        this.Name,
                        $"{description} cannot be an empty enumerable.");
            }
        }

        return this;
    }

    /// <summary>
    /// Checks that the collection dot NOT contain <see langword="null"/> values.
    /// Additionaly, the <paramref name="itemEnsureAction"/>
    /// can be used to further restrict non
    /// <see langword="null"/> values.
    /// Whenever possible, use more specific guard function.
    /// </summary>
    /// <param name="itemEnsureAction">Optional callback function which
    ///     which can be used for further ensuring of the collection items.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when
    ///     the collection contains
    ///     <see langword="null"/> value or if item check fails.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     by <paramref name="itemEnsureAction"/> if item check fails
    ///     with <see cref="ArgumentException"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     by <paramref name="itemEnsureAction"/> if item check fails
    ///     with <see cref="ArgumentOutOfRangeException"/>.</exception>
    ICollectionParam<T, TItem> AllNotNull(Action<TItem>? itemEnsureAction = null);

    /// <summary>
    /// Checks if the collection length is inside given range, throws if not.
    /// By default the range is inclusive.
    /// </summary>
    /// <param name="lowerBound">The lower bound, included by default.</param>
    /// <param name="upperBound">The upper bound, included by default.</param>
    /// <param name="includeLower">Indicates if the <paramref name="lowerBound"/>
    ///     is included in allowed range, defaults to <see langword="true"/>.</param>
    /// <param name="includeUpper">Indicates if the <paramref name="upperBound"/>
    ///     is included in allowed range, defaults to <see langword="true"/>.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the
    ///     parameter length is outside of the given range
    ///     ( | [ <paramref name="lowerBound"/>, <paramref name="upperBound"/> ] | ).</exception>
    ICollectionParam<T, TItem> InRange(
            int lowerBound,
            int upperBound,
            bool includeLower = true,
            bool includeUpper = true)
    {
        if (this.IsSpecified)
        {
            int length;

            if (this.Value is Array a)
            {
                length = a.Length;
            }
            else if (this.Value is IList<TItem> l)
            {
                length = l.Count;
            }
            else if (this.Value is IImmutableList<TItem> il)
            {
                length = il.Count;
            }
            else if (this.Value is IDictionary d)
            {
                length = d.Count;
            }
            else if (this.Value is IReadOnlyCollection<TItem> rc)
            {
                length = rc.Count;
            }
            else
            {
                length = this.Value.Count();
            }

            long lc = length - lowerBound;
            long uc = upperBound - length;

            if ((includeLower ? lc < 0 : lc <= 0) || (includeUpper ? uc < 0 : uc <= 0))
            {
                string description = this.Description;
                string range = Dump.Range(
                        lowerBound,
                        upperBound,
                        includeLower,
                        includeUpper);

                throw new ArgumentOutOfRangeException(
                        this.Name,
                        this.Value,
                        $"{description} length must be in range {range}.");
            }
        }

        return this;
    }
}
