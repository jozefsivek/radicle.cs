namespace Radicle.Common.Extensions;

using System;
using Radicle.Common.Check;

/// <summary>
/// Collection of date related extensions.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Determines if the given date is older than given
    /// <paramref name="duration"/> before the current time.
    /// </summary>
    /// <param name="date">Date value.</param>
    /// <param name="duration">Time duration to evaluate the "oldness".</param>
    /// <returns>Boolean value determining if the
    ///     <paramref name="date"/> if older than
    ///     <paramref name="duration"/> before the current time.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the
    ///     <paramref name="duration"/> is zero or negative.</exception>
    public static bool IsOlderThan(
            this DateTimeOffset date,
            TimeSpan duration)
    {
        TimeSpan interval = DateTimeOffset.UtcNow.Subtract(date);

        return interval > Ensure.Param(duration).StrictlyPositive().Value;
    }

    /// <summary>
    /// Determines if the given UTC date is older than given
    /// <paramref name="duration"/> before the current time.
    /// </summary>
    /// <param name="dateTime">Date value.</param>
    /// <param name="duration">Time duration to evaluate the "oldness".</param>
    /// <returns>Boolean value determining if the
    ///     <paramref name="dateTime"/> if older than
    ///     <paramref name="duration"/> before the current time.</returns>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="dateTime"/>
    ///     is not UTC <see cref="DateTime"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the
    ///     <paramref name="duration"/> is zero or negative.</exception>
    public static bool IsOlderThan(
            this DateTime dateTime,
            TimeSpan duration)
    {
        TimeSpan interval = DateTimeOffset.UtcNow.Subtract(
                Ensure.Param(dateTime).IsUTC().Value);

        return interval > Ensure.Param(duration).StrictlyPositive().Value;
    }
}
