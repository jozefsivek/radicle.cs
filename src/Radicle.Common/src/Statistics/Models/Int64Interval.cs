namespace Radicle.Common.Statistics.Models;

using System;

/// <summary>
/// Implementation of <see cref="NumericInterval{T}"/>
/// for <see cref="long"/> values.
/// </summary>
public sealed class Int64Interval : NumericInterval<long>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Int64Interval"/> class.
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
    public Int64Interval(
            long lower,
            long upper,
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
