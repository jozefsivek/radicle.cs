namespace Radicle.Common;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Radicle.Common.Check;

/// <summary>
/// ISO 8601:2004 dates parsing and serialization.
/// The support of the standard is not complete (e.g. week days support).
/// </summary>
public static class Iso8601
{
    /*
    see:
    - ISO 8601:2004 https://en.wikipedia.org/wiki/ISO_8601
    - https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings#the-hh-custom-format-specifier-1

    in Backus–Naur form:

    <digit>             ::= "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"

    ; Date ===================

    <sign>              :: = "+" | "-"

    ; "YYYY" or in .NET "yyyy"
    <year>              ::= <digit> <digit> <digit> <digit>

    ; "YYYYY" - not supported
    <long_year>         ::= <sign> <digit> <digit> <digit> <digit> <digit>

    ; in .NET "MM"
    <month>             ::= "0" <digit> | "10" | "11" | 12

    ; in .NET "DD"
    <day>               ::= "0" <digit> | "1" <digit> | "2" <digit> | "30" | "31"

    ; YYYY-MM-DD or in .NET "yyyy'-'MM'-'dd"
    ; "extended"
    <date>         ::= <year> "-" <month> "-" <day>

    ; YYYYMMDD or in .NET "yyyyMMdd"
    <basic_date> ::= <year> <month> <day>

    ; YYYY-MM or in .NET YYYY-MM
    ; note there is no compact version!
    <trimmed_date>      ::= <year> [ "-" <month> ]

    ; further not supported:
    ;   --MM-DD or --MMDD
    ;   week dates
    ;   ordinal dates

    ; Time ====================
    ; there is no 24th hour
    ; only seconds have fractions
    ; technically thre is no limit on decimal places in fractions

    <hour>             ::= "0" <digit> | "1" <digit> | "20" | "21" | "22" | "23"
    <minute>           ::= "0" <digit> | "1" <digit> | "2" <digit> | "3" <digit> |
                           "4" <digit> | "5" <digit>
    <whole_second>     ::= <minute>
    <second>           ::= <whole_second> | <whole_second> "." <digit>+

    ; hh:mm:ss.sss | hh:mm:ss | hh:mm | hh
    ; in .NET "HH':'mm':'ss'.'f[ffffff]" | "HH':'mm':'ss" | "HH':'mm" | "HH"
    ; "extended"
    <time>        ::= <hour> [ ":" <minute> [ ":" <second> ]]

    ; hhmmss.sss | hhmmss | hhmm
    ; in .NET "{BasicFractionSecondTime}f[ffffff]" | "HHmmss" | "HHmm"
    <basic_time>::= <hour> <minute> [ <second> ]

    ; Time zone designators ===
    ; missing time zone, e.g. "If no UTC relation information is given"
    ; then "the time is assumed to be in local time"

    ; Z | ±hh:mm | ±hh
    ; in .NET "Z" | "zzz" | "zz"
    <time_zone>        ::= "" | "Z" | <sign> <hour> ":" <minute> | <sign> <hour>
    ; compact representation (<time>±hhmm) - not supported

    ; Format ===================
    ; data and time part shoud match in use of basic or extended form
    ; "T" separator can not be omitted

    <iso_date>         ::= <trimmed_date> | <date> [ "T" <time> [ <time_zone> ]]
    <basic_iso_date>   ::= <basic_date> [ "T" <basic_time> [ <time_zone> ]]

    ; further not supported:
    ;   durations and time intervals

    */

    private const string Year = "yyyy";
    private const string TrimmedDate = "yyyy'-'MM";
    private const string Date = "yyyy'-'MM'-'dd";
    private const string BasicDate = "yyyyMMdd";

    private const string HourTime = "HH";
    private const string MinuteTime = "HH':'mm";
    private const string SecondTime = "HH':'mm':'ss";
    private const string FractionSecondTime = "HH':'mm':'ss'.'";

    private const string BasicHourTime = "HH";
    private const string BasicMinuteTime = "HHmm";
    private const string BasicSecondTime = "HHmmss";
    private const string BasicFractionSecondTime = "HHmmss'.'";

    private static readonly string[] TimeZoneDesignators = new[] { string.Empty, "Z", "zz", "zzz" };
    private static readonly string[] SecondFractionDesignators = new[]
    {
        "f",
        "ff",
        "fff",
        "ffff",
        "fffff",
        "ffffff",
        "fffffff",
    };

    private static readonly string[] ExtendedFormats = ConstructDateTimeFormats(Iso8601Forms.Extended).ToArray();

    private static readonly string[] BasicFormats = ConstructDateTimeFormats(Iso8601Forms.Basic).ToArray();

    private static readonly string[] AllFormats = ConstructDateTimeFormats(Iso8601Forms.All).ToArray();

    /// <summary>
    /// Try to parse <paramref name="input"/> to date
    /// according to ISO 8601:2004. Only subset of the
    /// standard is supported, basic and extended forms
    /// with optional ommission of shortest time intervals.
    /// Year is only 4 digit one, and day with 24 hours.
    /// </summary>
    /// <param name="input">String to parse.</param>
    /// <param name="result">Parsed value if any.</param>
    /// <param name="assumeUniversal">By the standard the dates with the missing
    ///     time zone designation are interpreted in local time.
    ///     Set this flag to <see langword="true"/> to override this behavior
    ///     and interpret such dates as if they were in the UTC time zone.
    ///     Also consider use of this flag if parsing only dates
    ///     with no time information.</param>
    /// <param name="forms">Optional restriction on forms,
    ///     main practical usage is to disable basic forms.</param>
    /// <returns><see langword="true"/> if <paramref name="input"/>
    ///     was succesfully parsed; <see langword="false"/> otherwise.</returns>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    public static bool TryParse(
            string? input,
            out DateTimeOffset result,
            bool assumeUniversal = false,
            Iso8601Forms forms = Iso8601Forms.All)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        DateTimeStyles styles;

        // UTC 'Z' designation overrides
        if (!assumeUniversal && input.EndsWith("Z", StringComparison.Ordinal))
        {
            styles = DateTimeStyles.AssumeUniversal;
        }
        else
        {
            styles = assumeUniversal
                    ? DateTimeStyles.AssumeUniversal
                    : DateTimeStyles.AssumeLocal;
        }

        string[] formats = forms switch
        {
            Iso8601Forms.Extended => ExtendedFormats,
            Iso8601Forms.Basic => BasicFormats,
            Iso8601Forms.All => AllFormats,
            Iso8601Forms.None => Array.Empty<string>(),
            _ => throw new NotSupportedException($"BUG: {forms} is not supported."),
        };

        if (DateTimeOffset.TryParseExact(
                input,
                formats,
                CultureInfo.InvariantCulture,
                styles,
                out DateTimeOffset res))
        {
            result = res;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Parse <paramref name="input"/> to date
    /// according to ISO 8601:2004. Only subset of the
    /// standard is supported, basic and extended forms
    /// with optional ommission of shortest time intervals.
    /// Year is only 4 digit one, and day with 24 hours.
    /// </summary>
    /// <param name="input">String to parse.</param>
    /// <param name="assumeUniversal">By the standard the dates with the missing
    ///     time zone designation are interpreted in local time.
    ///     Set this flag to <see langword="true"/> to override this behavior
    ///     and interpret such dates as if they were in the UTC time zone.
    ///     Also consider use of this flag if parsing only dates
    ///     with no time information.</param>
    /// <param name="forms">Optional restriction on forms,
    ///     main practical usage is to disable basic forms.</param>
    /// <returns><see langword="true"/> if <paramref name="input"/>
    ///     was succesfully parsed; <see langword="false"/> otherwise.</returns>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    /// <returns>Instantce of <see cref="DateTimeOffset"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the input is
    ///     empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the input is
    ///     <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown is <paramref name="input"/>
    ///     is white space string.</exception>
    /// <exception cref="FormatException">Thrown if the input does not conform
    ///     to supported format subset of ISO 8601:2004.</exception>
    public static DateTimeOffset Parse(
            string input,
            bool assumeUniversal = false,
            Iso8601Forms forms = Iso8601Forms.All)
    {
        Ensure.Param(input).NotWhiteSpace().Done();

        DateTimeStyles styles;

        // UTC 'Z' designation overrides
        if (!assumeUniversal && input.EndsWith("Z", StringComparison.Ordinal))
        {
            styles = DateTimeStyles.AssumeUniversal;
        }
        else
        {
            styles = assumeUniversal
                    ? DateTimeStyles.AssumeUniversal
                    : DateTimeStyles.AssumeLocal;
        }

        string[] formats = forms switch
        {
            Iso8601Forms.Extended => ExtendedFormats,
            Iso8601Forms.Basic => BasicFormats,
            Iso8601Forms.All => AllFormats,
            Iso8601Forms.None => Array.Empty<string>(),
            _ => throw new NotSupportedException($"BUG: {forms} is not supported."),
        };

        return DateTimeOffset.ParseExact(
                input,
                formats,
                CultureInfo.InvariantCulture,
                styles);
    }

    /// <summary>
    /// Serialize given <paramref name="date"/> to string conforming
    /// ISO 8601:2004. See <see cref="TryParse(string?, out DateTimeOffset, bool, Iso8601Forms)"/>
    /// for inverse operation.
    /// </summary>
    /// <param name="date">Value to convert.</param>
    /// <returns>String represetnation of <paramref name="date"/>.</returns>
    public static string ToString(DateTimeOffset date)
    {
        return date.ToString("o", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Construct <see cref="DateTimeOffset"/> compatible formats
    /// of ISO 8601:2004. Default or <see cref="Iso8601Forms.Extended"/>
    /// will return year and year+month format,
    /// the <see cref="Iso8601Forms.Basic"/> will NOT.
    /// </summary>
    /// <param name="forms">Form to return.</param>
    /// <returns>Enumeration of formats.</returns>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    public static IEnumerable<string> ConstructDateTimeFormats(
            Iso8601Forms forms = Iso8601Forms.All)
    {
        List<Iso8601Forms> flags = new(2);
        string date;
        string htime;
        string mtime;
        string stime;
        string time;

        if (forms.HasFlag(Iso8601Forms.Extended))
        {
            yield return Year;
            yield return TrimmedDate;

            flags.Add(Iso8601Forms.Extended);
        }

        if (forms.HasFlag(Iso8601Forms.Basic))
        {
            flags.Add(Iso8601Forms.Basic);
        }

        foreach (Iso8601Forms flag in flags)
        {
            switch (flag)
            {
                case Iso8601Forms.Extended:
                    date = Date;
                    htime = HourTime;
                    mtime = MinuteTime;
                    stime = SecondTime;
                    time = FractionSecondTime;
                    break;
                case Iso8601Forms.Basic:
                    date = BasicDate;
                    htime = BasicHourTime;
                    mtime = BasicMinuteTime;
                    stime = BasicSecondTime;
                    time = BasicFractionSecondTime;
                    break;
                case Iso8601Forms.None:
                case Iso8601Forms.All:
                    throw new NotSupportedException($"BUG: {forms} should not be listed.");
                default:
                    throw new NotSupportedException($"BUG: the {forms} is not supported.");
            }

            yield return date;

            foreach (string zone in TimeZoneDesignators)
            {
                yield return $"{date}T{htime}{zone}";
                yield return $"{date}T{mtime}{zone}";
                yield return $"{date}T{stime}{zone}";

                foreach (string fraction in SecondFractionDesignators)
                {
                    yield return $"{date}T{time}{fraction}{zone}";
                }
            }
        }
    }
}
