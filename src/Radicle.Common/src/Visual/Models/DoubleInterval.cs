namespace Radicle.Common.Visual.Models;

using System;
using Radicle.Common.Check;

/// <summary>
/// Implementation of <see cref="NumericInterval{T}"/>
/// for <see cref="double"/> values.
/// </summary>
public sealed class DoubleInterval : NumericInterval<double>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleInterval"/> class.
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
    /// <exception cref="ArgumentException">Thrown if
    ///     boundary is not a number.</exception>
    public DoubleInterval(
            double lower,
            double upper,
            bool includeLower = true,
            bool includeUpper = true)
        : base(
            Ensure.Param(lower).That(v => !double.IsNaN(v)).Value,
            Ensure.Param(upper).That(v => !double.IsNaN(v)).Value,
            includeLower: includeLower,
            includeUpper: includeUpper)
    {
    }
}
