namespace Radicle.Common.Extensions;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Radicle.Common.Check;

/// <summary>
/// Extensions for time durations.
/// </summary>
public static class TimeSpanExtensions
{
    private const string NanoSecondSI = "ns"; /* GREEK SMALL LETTER MU */
    private const string MicroSecondSI = "\u03bcs"; /* GREEK SMALL LETTER MU */
    private const string MilliSecondSI = "ms";
    private const string SecondSI = "s";
    private const string MinuteSI = "m";
    private const string HourSI = "h";
    private const string DaySI = "d";
    private const string YearSI = "y";
    private const string InfinitySI = "\u221E";
    private const string NegativeInfinitySI = "-\u221E";

    private const string NanoSecond = "nanosecond";
    private const string MicroSecond = "microsecond";
    private const string MilliSecond = "millisecond";
    private const string Second = "second";
    private const string Minute = "minute";
    private const string Hour = "hour";
    private const string Day = "day";
    private const string Year = "year";

    private const string NanoSeconds = "nanoseconds";
    private const string MicroSeconds = "microseconds";
    private const string MilliSeconds = "milliseconds";
    private const string Seconds = "seconds";
    private const string Minutes = "minutes";
    private const string Hours = "hours";
    private const string Days = "days";
    private const string Years = "years";

    private const string Infinity = "+\u221E";
    private const string NegativeInfinity = "-\u221E";

    private const string And = "and";

    private const int YearDuration = 365;

    /// <summary>
    /// Returns value clamped to the inclusive range of min and max
    /// like <see cref="Math.Clamp(int, int, int)"/>.
    /// </summary>
    /// <param name="span">The value to be clamped.</param>
    /// <param name="min">The lower bound of the result.</param>
    /// <param name="max">The upper bound of the result.</param>
    /// <returns><paramref name="span"/> if in inclusive range
    /// [<paramref name="min"/>, <paramref name="max"/>]
    /// -or- <paramref name="min"/> if shorter than <paramref name="min"/>
    /// -or- <paramref name="max"/> if longer than <paramref name="max"/>.</returns>
    public static TimeSpan Clamp(
            this TimeSpan span,
            TimeSpan min,
            TimeSpan max)
    {
        if (span >= max)
        {
            return max;
        }
        else if (span <= min)
        {
            return min;
        }

        return span;
    }

    /// <summary>
    /// Returns <paramref name="span"/> if it is shorter than
    /// <paramref name="alternative"/>, otherwise returns <paramref name="alternative"/>.
    /// </summary>
    /// <param name="span">Original time span.</param>
    /// <param name="alternative">Other time span to choose if <paramref name="span"/>
    ///     is longer than <paramref name="alternative"/>.</param>
    /// <returns><paramref name="span"/> if shorter
    ///     or equal than <paramref name="alternative"/>,
    ///     otherwise <paramref name="alternative"/>.</returns>
    public static TimeSpan UseIfShorterOr(
            this TimeSpan span,
            TimeSpan alternative)
    {
        if (span <= alternative)
        {
            return span;
        }

        return alternative;
    }

    /// <summary>
    /// Returns <paramref name="span"/> if it is longer than
    /// <paramref name="alternative"/>, otherwise returns <paramref name="alternative"/>.
    /// </summary>
    /// <param name="span">Original time span.</param>
    /// <param name="alternative">Other time span to choose if <paramref name="span"/>
    ///     is shorter than <paramref name="alternative"/>.</param>
    /// <returns><paramref name="span"/> if longer
    ///     or equal than <paramref name="alternative"/>,
    ///     otherwise <paramref name="alternative"/>.</returns>
    public static TimeSpan UseIfLongerOr(
            this TimeSpan span,
            TimeSpan alternative)
    {
        if (span >= alternative)
        {
            return span;
        }

        return alternative;
    }

    /// <summary>
    /// Perform modulo of <see cref="TimeSpan"/> by given time span
    /// returning division reminder with use of ticks.
    /// See https://en.wikipedia.org/wiki/Modulo_operation .
    /// </summary>
    /// <param name="dividend">Time span to perform modulo on.</param>
    /// <param name="divisor">Time span to perform modulo with.</param>
    /// <returns>Modulo of the time span division.</returns>
    /// <exception cref="DivideByZeroException">Thrown if
    ///     <paramref name="divisor"/> is of zero length.</exception>
    public static TimeSpan Mod(
            this TimeSpan dividend,
            TimeSpan divisor)
    {
        return TimeSpan.FromTicks(dividend.Ticks % divisor.Ticks);
    }

    /// <summary>
    /// Return counter, human readable, representation of
    /// the <see cref="TimeSpan"/> instance. Examples: "[-]00:00:01.00" (h:m:s),
    /// "[-]1d 01:00:00.00", "[-]1y 1d 01:00:00.00".
    /// Year is defined as 365 days.
    /// </summary>
    /// <param name="span">Time span to represent.</param>
    /// <param name="secondsFloatingPrecission">Precission of seconds
    ///     floating part, allowed range is [0, 3].</param>
    /// <returns>String representation of the <paramref name="span"/>.</returns>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    public static string ToCounter(
            this TimeSpan span,
            byte secondsFloatingPrecission = 2)
    {
        string? sign = span.TotalHours < 0 ? "-" : string.Empty;
        int y = Abs(span.Days / YearDuration);
        int d = Abs(span.Days % YearDuration);
        int h = Abs(span.Hours);
        int m = Abs(span.Minutes);
        int s = Abs(span.Seconds);
        string millisecods = string.Empty;

        if (Ensure.Param(secondsFloatingPrecission).InRange(0, 3).Value > 0)
        {
            int ms = Abs(RoundToInt32(span.Milliseconds / Math.Pow(10.0, 3 - secondsFloatingPrecission)));

            millisecods = secondsFloatingPrecission switch
            {
                1 => $".{ms:D1}",
                2 => $".{ms:D2}",
                3 => $".{ms:D3}",
                _ => throw new NotSupportedException(
                        $"BUG: {nameof(secondsFloatingPrecission)} {secondsFloatingPrecission} is not supported."),
            };
        }
        else
        {
            // round secods
            s = Abs(RoundToInt32(span.Seconds + (span.Milliseconds / 1000.0)));
        }

        /*
         * see (':D2'):
         * https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#DFormatString
         */
        if (y > 0)
        {
            // non zero years
            return $"{sign}{y}{YearSI} {d}{DaySI} {h:D2}:{m:D2}:{s:D2}{millisecods}";
        }
        else if (d > 0)
        {
            // non zero days
            return $"{sign}{d}{DaySI} {h:D2}:{m:D2}:{s:D2}{millisecods}";
        }
        else
        {
            return $"{sign}{h:D2}:{m:D2}:{s:D2}{millisecods}";
        }
    }

    /// <summary>
    /// Proxy method for <see cref="ToHuman(TimeSpan, bool)"/>
    /// with brief parameter set to <see langword="true"/>.
    /// </summary>
    /// <param name="span">Time span to represent.</param>
    /// <returns>Short human string representation of <paramref name="span"/>.</returns>
    public static string ToH(this TimeSpan span)
    {
        return span.ToHuman(brief: true);
    }

    /// <summary>
    /// Proxy method for <see cref="ToHuman(TimeSpan, bool)"/>
    /// with brief parameter set to <see langword="true"/>.
    /// </summary>
    /// <param name="stopwatch"><see cref="Stopwatch"/> to extract elapsed time from.</param>
    /// <returns>Short human string representation of <paramref name="stopwatch"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static string ToH(this Stopwatch stopwatch)
    {
        return Ensure.Param(stopwatch).Value.Elapsed.ToHuman(brief: true);
    }

    /// <summary>
    /// Return <see cref="TimeSpan"/> instance representation in format
    /// easily readable for humans (e.g. good for showing estimates).
    /// Examples: "1 second", "1s" (brief), "4 days", "4d" (brief), etc.
    /// Year is defined as 365 days. Range is from years to nanosecods.
    /// </summary>
    /// <param name="stopwatch"><see cref="Stopwatch"/> to extract elapsed time from.</param>
    /// <param name="brief">define format of the time units, if set
    ///     to <see langword="true"/> then the common SI units
    ///     are used instead of full words.</param>
    /// <returns>Human string representation of <paramref name="stopwatch"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static string ToHuman(
            this Stopwatch stopwatch,
            bool brief = false)
    {
        Ensure.Param(stopwatch).Done();

        return stopwatch.Elapsed.ToHuman(brief: brief);
    }

    /// <summary>
    /// Return <see cref="TimeSpan"/> instance representation in format
    /// easily readable for humans (e.g. good for showing estimates).
    /// Examples: "1 second", "1s" (brief), "4 days", "4d" (brief), etc.
    /// Year is defined as 365 days. Range is from years to nanosecods.
    /// </summary>
    /// <param name="span">Time span to represent.</param>
    /// <param name="brief">define format of the time units, if set
    ///     to <see langword="true"/> then the common SI units
    ///     are used instead of full words.</param>
    /// <returns>Human string representation of <paramref name="span"/>.</returns>
    public static string ToHuman(
            this TimeSpan span,
            bool brief = false)
    {
        double s = span.TotalSeconds;
        double m = span.TotalMinutes;
        double h = span.TotalHours;
        double d = span.TotalDays;
        List<string> result = new();

        if (brief)
        {
            if (span == TimeSpan.MaxValue)
            {
                result.Add(InfinitySI);
            }
            else if (span == TimeSpan.MinValue)
            {
                result.Add(NegativeInfinitySI);
            }
            else if (Abs(s) < 0.000_0016)
            {
                result.Add($"{RoundToInt32(span.TotalMilliseconds * 1000_000)}{NanoSecondSI}");
            }
            else if (Abs(s) < 0.0016)
            {
                result.Add($"{RoundToInt32(span.TotalMilliseconds * 1000)}{MicroSecondSI}");
            }
            else if (Abs(s) < 1.6)
            {
                result.Add($"{RoundToInt32(span.TotalMilliseconds)}{MilliSecondSI}");
            }
            else if (Abs(s) < 60 * 1.6)
            {
                result.Add($"{RoundToInt32(s)}{SecondSI}");
            }
            else if (Abs(m) < 60 * 1.6)
            {
                result.Add($"{RoundToInt32(m)}{MinuteSI}");
            }
            else if (Abs(h) < 24 * 1.6)
            {
                result.Add($"{RoundToInt32(h)}{HourSI}");
            }
            else if (Abs(d) < (YearDuration * 1.6))
            {
                result.Add($"{RoundToInt32(d)}{DaySI}");
            }
            else
            {
                result.Add($"{RoundToInt32(d / YearDuration)}{YearSI}");
            }
        }
        else if (span == TimeSpan.MaxValue)
        {
            result.Add(Infinity);
        }
        else if (span == TimeSpan.MinValue)
        {
            result.Add(NegativeInfinity);
        }
        else if (Abs(s) < 0.000_0016)
        {
            int t = RoundToInt32(span.TotalMilliseconds * 1000_000);

            result.Add($"{t} {Plural(t, NanoSecond, NanoSeconds)}");
        }
        else if (Abs(s) < 0.0016)
        {
            int t = RoundToInt32(span.TotalMilliseconds * 1000);

            result.Add($"{t} {Plural(t, MicroSecond, MicroSeconds)}");
        }
        else if (Abs(s) < 1.6)
        {
            int t = RoundToInt32(span.TotalMilliseconds);

            result.Add($"{t} {Plural(t, MilliSecond, MilliSeconds)}");
        }
        else if (Abs(s) < 60 * 1.6)
        {
            int t = RoundToInt32(s);

            result.Add($"{t} {Plural(t, Second, Seconds)}");
        }
        else if (Abs(m) < 60 * 1.6)
        {
            int roundedSeconds = RoundToInt32(s);
            int tm = roundedSeconds / 60;
            int ts = Abs(roundedSeconds % 60);

            result.Add($"{tm} {Plural(tm, Minute, Minutes)}");

            if (ts > 0)
            {
                result.Add($"{ts} {Plural(ts, Second, Seconds)}");
            }
        }
        else if (Abs(h) < 24 * 1.6)
        {
            int roundedMinutes = RoundToInt32(m);
            int th = roundedMinutes / 60;
            int tm = roundedMinutes % 60;

            if (Abs(th) > 0)
            {
                result.Add($"{th} {Plural(th, Hour, Hours)}");
                tm = Abs(tm);

                if (tm > 0)
                {
                    result.Add($"{tm} {Plural(tm, Minute, Minutes)}");
                }
            }
            else if (Abs(tm) > 0)
            {
                result.Add($"{tm} {Plural(tm, Minute, Minutes)}");
            }
        }
        else if (Abs(d) < 7 * 1.6)
        {
            int roundedHours = RoundToInt32(h);
            int td = roundedHours / 24;
            int th = roundedHours % 24;

            if (Abs(td) > 0)
            {
                result.Add($"{td} {Plural(td, Day, Days)}");
                th = Abs(th);

                if (th > 0)
                {
                    result.Add($"{th} {Plural(th, Hour, Hours)}");
                }
            }
            else if (Abs(th) > 0)
            {
                result.Add($"{th} {Plural(th, Hour, Hours)}");
            }
        }
        else if (Abs(d) < (YearDuration * 160))
        {
            int roundedDays = RoundToInt32(d);
            int ty = roundedDays / 365;
            int td = roundedDays % 365;

            if (Abs(ty) > 0)
            {
                result.Add($"{ty} {Plural(ty, Year, Years)}");
                td = Abs(td);

                if (td > 0)
                {
                    result.Add($"{td} {Plural(td, Day, Days)}");
                }
            }
            else if (Abs(td) > 0)
            {
                result.Add($"{td} {Plural(td, Day, Days)}");
            }
        }
        else
        {
            result.Add($"{RoundToInt32(d / YearDuration)} {Years}");
        }

        return string.Join($" {And} ", result);
    }

    private static string Plural(
            int quantity,
            string singular,
            string plural)
    {
        return Abs(quantity) < 2 ? singular : plural;
    }

    private static int RoundToInt32(double input)
    {
        return (int)Math.Round(input, MidpointRounding.AwayFromZero);
    }

    private static double Abs(double input)
    {
        return Math.Abs(input);
    }

    private static int Abs(int input)
    {
        return Math.Abs(input);
    }
}
