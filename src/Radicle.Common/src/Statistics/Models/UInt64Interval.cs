namespace Radicle.Common.Statistics.Models;

using System;

/// <summary>
/// Implementation of <see cref="NumericInterval{T}"/>
/// for <see cref="ulong"/> values.
/// </summary>
public sealed class UInt64Interval : NumericInterval<ulong>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UInt64Interval"/> class.
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
    public UInt64Interval(
            ulong lower,
            ulong upper,
            bool includeLower = true,
            bool includeUpper = true)
        : base(
            lower,
            upper,
            includeLower: includeLower,
            includeUpper: includeUpper)
    {
    }
}
