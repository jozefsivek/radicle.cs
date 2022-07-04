namespace Radicle.Common.Check.Models;

using System;
using Radicle.Common.Check.Models.Generic;

/// <summary>
/// Interface or long method parameter.
/// </summary>
/// <typeparam name="TValue">Type of the numerical value.</typeparam>
public interface INumberLikeParam<TValue> : IParam<TValue>
        where TValue : struct
{
    /// <summary>
    /// Checks if the parameter value is greater than a given
    /// <paramref name="minimum"/>, throws if not.
    /// </summary>
    /// <param name="minimum">Minimum, non inclusive, value to compare parameter value to.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when
    ///     the parameter value is smaller or equal to the
    ///     <paramref name="minimum"/>.</exception>
    INumberLikeParam<TValue> GreaterThan(TValue minimum);

    /// <summary>
    /// Checks if the parameter value is greater than or equal to given
    /// <paramref name="minimum"/>, throws if not.
    /// </summary>
    /// <param name="minimum">Minimum, inclusive, value to compare parameter value to.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when
    ///     the parameter value is smaller than the
    ///     <paramref name="minimum"/>.</exception>
    INumberLikeParam<TValue> GreaterThanOrEqual(TValue minimum);

    /// <summary>
    /// Checks if the parameter value is lower than a given
    /// <paramref name="maximum"/>, throws if not.
    /// </summary>
    /// <param name="maximum">Maximum, non inclusive, value to compare parameter value to.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when
    ///     the parameter value is larger or equal to the
    ///     <paramref name="maximum"/>.</exception>
    INumberLikeParam<TValue> LowerThan(TValue maximum);

    /// <summary>
    /// Checks if the parameter value is lower or equal to given
    /// <paramref name="maximum"/>, throws if not.
    /// </summary>
    /// <param name="maximum">Maximum, inclusive, value to compare parameter value to.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when
    ///     the parameter value is larger than the
    ///     <paramref name="maximum"/>.</exception>
    [Obsolete("Use LowerThanOrEqual")]
    INumberLikeParam<TValue> LowerThenOrEqual(TValue maximum);

    /// <summary>
    /// Checks if the parameter value is lower or equal to given
    /// <paramref name="maximum"/>, throws if not.
    /// </summary>
    /// <param name="maximum">Maximum, inclusive, value to compare parameter value to.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when
    ///     the parameter value is larger than the
    ///     <paramref name="maximum"/>.</exception>
    INumberLikeParam<TValue> LowerThanOrEqual(TValue maximum);

    /// <summary>
    /// Checks if the parameter value is non negative, throws if negative.
    /// </summary>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when
    ///     the parameter value is negative.</exception>
    INumberLikeParam<TValue> NonNegative();

    /// <summary>
    /// Checks if the parameter value is non zero, throws if zero.
    /// </summary>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when
    ///     the parameter value is zero.</exception>
    INumberLikeParam<TValue> NonZero();

    /// <summary>
    /// Checks if the parameter value is strictly positive (non zero), throws if not.
    /// </summary>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when
    ///     the parameter value is negative.</exception>
    INumberLikeParam<TValue> StrictlyPositive();

    /// <summary>
    /// Checks if the parameter value represents actual finite value, throws if not.
    /// </summary>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the
    ///     parameter is not a "number" or is infinite.</exception>
    INumberLikeParam<TValue> HasActualValue();

    /// <summary>
    /// Checks if the parameter value is inside given range, throws if not.
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
    ///     parameter is outside of the given range
    ///     ( | [ <paramref name="lowerBound"/>, <paramref name="upperBound"/> ] | ).</exception>
    INumberLikeParam<TValue> InRange(
            TValue lowerBound,
            TValue upperBound,
            bool includeLower = true,
            bool includeUpper = true);

    /// <summary>
    /// Checks if the parameter value is in UTC time zone.
    /// All scalar arguments are by default.
    /// </summary>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentException">Thrown if the
    ///     parameter is not in UTC time zone.</exception>
    INumberLikeParam<TValue> IsUTC();

    /// <summary>
    /// Return value of this parameter or
    /// default value for the type if this argument
    /// was optional and lacks value. May not work
    /// as you expect for value types.
    /// </summary>
    /// <remarks>This is because of https://www.debugcn.com/en/article/49095841.html
    /// "When you say T? in T? and in (T? t), they both refer
    /// to nullable reference types, not to the special
    /// Nullable{T} struct. There's no way that you can
    /// specify a generic parameter such that you can
    /// treat it as a class and a nullable value type.".</remarks>
    /// <returns><see cref="IParam{TValue}.Value"/> if present,
    ///     default value for the type otherwise.</returns>
    new TValue? ValueOrDefault()
    {
        if (this.IsSpecified)
        {
            return this.Value;
        }

        return default;
    }
}
