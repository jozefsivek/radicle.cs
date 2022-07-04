namespace Radicle.Common.Check.Models.Generic;

using System;
using System.Collections.Generic;

/// <summary>
/// Interface of generic dictionary parameter.
/// </summary>
/// <typeparam name="T">Type of the
///     dictionary parameter.</typeparam>
/// <typeparam name="TItemKey">Type of the collection
///     parameter key value.</typeparam>
/// <typeparam name="TItemValue">Type of the collection
///     parameter value.</typeparam>
public interface IDictionaryParam<T, TItemKey, TItemValue> : ICollectionParam<T, KeyValuePair<TItemKey, TItemValue>>
    where T : notnull, IEnumerable<KeyValuePair<TItemKey, TItemValue>>
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
    new IDictionaryParam<T, TItemKey, TItemValue> That(
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
    new IDictionaryParam<T, TItemKey, TItemValue> NotEmpty()
    {
        _ = ((ICollectionParam<T, KeyValuePair<TItemKey, TItemValue>>)this).NotEmpty();

        return this;
    }

    /// <summary>
    /// Checks if the collection contains null values.
    /// Additionaly, the <paramref name="itemEnsureAction"/>
    /// can be used to further restrict values.
    /// Whenever possible, use more specific guard function.
    /// </summary>
    /// <param name="itemEnsureAction">Optional callback function which
    ///     which can be used for further check.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when
    ///     the dictionary contains
    ///     <see langword="null"/> value.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     by <paramref name="itemEnsureAction"/> if item ensure action fails
    ///     with <see cref="ArgumentException"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     by <paramref name="itemEnsureAction"/> if item ensure action fails
    ///     with <see cref="ArgumentOutOfRangeException"/>.</exception>
    new IDictionaryParam<T, TItemKey, TItemValue> AllNotNull(
            Action<KeyValuePair<TItemKey, TItemValue>>? itemEnsureAction = null);

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
    new IDictionaryParam<T, TItemKey, TItemValue> InRange(
            int lowerBound,
            int upperBound,
            bool includeLower = true,
            bool includeUpper = true)
    {
        _ = ((ICollectionParam<T, KeyValuePair<TItemKey, TItemValue>>)this)
                .InRange(
                    lowerBound,
                    upperBound,
                    includeLower: includeLower,
                    includeUpper: includeUpper);

        return this;
    }

    /// <summary>
    /// Checks if the dictionary contains null values.
    /// Additionaly, the <paramref name="itemValueEnsureAction"/>
    /// can be used to further restrict values.
    /// Whenever possible, use more specific guard function.
    /// </summary>
    /// <param name="itemValueEnsureAction">Optional callback function which
    ///     which can be used for further ensure collection items.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when
    ///     the dictionary contains
    ///     <see langword="null"/> value.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     by <paramref name="itemValueEnsureAction"/> if item value ensure action fails
    ///     with <see cref="ArgumentException"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     by <paramref name="itemValueEnsureAction"/> if item value ensure action fails
    ///     with <see cref="ArgumentOutOfRangeException"/>.</exception>
    IDictionaryParam<T, TItemKey, TItemValue> AllValuesNotNull(
            Action<TItemValue>? itemValueEnsureAction = null);

    /// <summary>
    /// Perform chek on the keys with <paramref name="itemKeyEnsureAction"/>.
    /// Note keys can not be <see langword="null"/> by definition.
    /// Whenever possible, use more specific guard function.
    /// </summary>
    /// <param name="itemKeyEnsureAction">Optional callback function which
    ///     which can be used for further ensuring collection items.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     by <paramref name="itemKeyEnsureAction"/> if item key ensure action fails
    ///     with <see cref="ArgumentNullException"/>.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     by <paramref name="itemKeyEnsureAction"/> if item key ensure action fails
    ///     with <see cref="ArgumentException"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     by <paramref name="itemKeyEnsureAction"/> if item key ensure action fails
    ///     with <see cref="ArgumentOutOfRangeException"/>.</exception>
    IDictionaryParam<T, TItemKey, TItemValue> AllKeys(
            Action<TItemKey>? itemKeyEnsureAction = null);
}
