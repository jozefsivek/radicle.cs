namespace Radicle.Common.Extensions;

using System;
using Radicle.Common.Check;

/// <summary>
/// Extensions for <see cref="string"/>.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Convert given <paramref name="str"/>
    /// to value of limited in length, with optional suffix
    /// string, by trimming the original string from right.
    /// </summary>
    /// <param name="str">Value of the parameter.</param>
    /// <param name="trim">Defines maximum length of
    ///     the returned value together with <paramref name="fuzziness"/>,
    ///     i.e. the returned value length will not be longer than
    ///     <paramref name="trim"/> plus <paramref name="fuzziness"/>.</param>
    /// <param name="fuzziness">Fuzziness factor to use in order
    ///     not to trim.</param>
    /// <param name="suffix">Optional suffix to use to indicate
    ///     trimmed part, if set to default <see langword="null"/>
    ///     the ellipsis "..." will be used.</param>
    /// <returns>String representation of shortened <paramref name="str"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if required arguments
    ///     are <see langword="null" />.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="trim"/> is zero,
    ///     or if suffix is longer than maximum length
    ///     of the returned value.</exception>
    public static string Ellipsis(
            this string str,
            ushort trim = 24,
            ushort fuzziness = 6,
            string? suffix = null)
    {
        Ensure.Param<string>(str).Done();

        Ensure.Param(trim)
                .StrictlyPositive().Done();

        int max = trim + fuzziness;

        suffix = Ensure.Optional(suffix)
                .InRange(0, max)
                .ValueOr("...");

        if (str.Length <= max)
        {
            return str;
        }
        else if (trim > suffix.Length)
        {
            // suffix fits in trim length
            return str[..(trim - suffix.Length)] + suffix;
        }
        else if (max >= suffix.Length)
        {
            // suffix is long compared to trim length
            return str[..(max - suffix.Length)] + suffix;
        }
        else
        {
            // default suffix is way too long compared to trim length
            suffix = new string('.', max - 1);

            return str[..1] + suffix;
        }
    }
}
