namespace Radicle.Common.Visual.Models;

using System;

/// <summary>
/// Immutable representation of numerical interval.
/// </summary>
/// <typeparam name="T">Type of the numerical value.</typeparam>
public abstract class NumericInterval<T>
    where T : struct, IComparable<T>, IEquatable<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NumericInterval{T}"/> class.
    /// </summary>
    /// <param name="lower">Lower interval bound.</param>
    /// <param name="upper">Upper inteval bound.</param>
    /// <param name="includeLower">Include <paramref name="lower"/>
    ///     into the interval.</param>
    /// <param name="includeUpper">Include <paramref name="upper"/>
    ///     into the interval.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="lower"/> is greater than
    ///     <paramref name="upper"/>; or if they are equal
    ///     while some of the boundary is not included in the interval.</exception>
    protected NumericInterval(
            T lower,
            T upper,
            bool includeLower = true,
            bool includeUpper = true)
    {
        int cmp = lower.CompareTo(upper);

        if (cmp > 0)
        {
            throw new ArgumentOutOfRangeException(
                    nameof(lower),
                    $"Lower bound {lower} can not be greater the upper {upper}.");
        }
        else if (cmp == 0 && (!includeLower || !includeUpper))
        {
            throw new ArgumentOutOfRangeException(
                    nameof(lower),
                    $"Lower bound {lower} can not equal to the upper {upper} if not all are included.");
        }

        this.Lower = lower;
        this.Upper = upper;
        this.IncludeLower = includeLower;
        this.IncludeUpper = includeUpper;
    }

    /// <summary>
    /// Gets lower bound of the interval.
    /// </summary>
    public T Lower { get; }

    /// <summary>
    /// Gets upper bound of interval.
    /// </summary>
    public T Upper { get; }

    /// <summary>
    /// Gets a value indicating whether to include
    /// <see cref="Lower"/> value into this interval.
    /// </summary>
    public bool IncludeLower { get; }

    /// <summary>
    /// Gets a value indicating whether to include
    /// <see cref="Upper"/> value into this interval.
    /// </summary>
    public bool IncludeUpper { get; }

    /// <summary>
    /// Determines if this interval contains the given value.
    /// </summary>
    /// <param name="value">Value to probe.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/>
    ///     is within this interval; <see langword="false"/> otherwise.</returns>
    public bool Contains(T value)
    {
        return this.CompareValue(value) == 0;
    }

    /// <summary>
    /// Compare this interval to the given value.
    /// Akin to CompareTo on <see cref="IComparable{T}"/>.
    /// </summary>
    /// <param name="value">Value to probe.</param>
    /// <returns>Value less than zero means the <paramref name="value"/>
    ///     is bellow this interval, value equal to zero means
    ///     the <paramref name="value"/> is in the interval,
    ///     value greater than zero means <paramref name="value"/>
    ///     is above this interval. Inclusion of interval boundaries
    ///     are taken into the consideration.</returns>
    public int CompareValue(T value)
    {
        int upperCmp = value.CompareTo(this.Upper);

        if (upperCmp > 0)
        {
            return upperCmp;
        }
        else if (upperCmp == 0)
        {
            return this.IncludeUpper ? 0 : 1;
        }

        int lowerCmp = value.CompareTo(this.Lower);

        if (lowerCmp < 0)
        {
            return lowerCmp;
        }
        else if (lowerCmp == 0)
        {
            return this.IncludeLower ? 0 : -1;
        }

        return 0;
    }
}
