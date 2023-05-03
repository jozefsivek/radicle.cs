namespace Radicle.Common.Extensions;

using System;
using System.Globalization;
using Radicle.Common.Check;
using Radicle.Common.Tokenization;

/// <summary>
/// Extensions for <see cref="string"/>.
/// </summary>
public static class StringExtensions
{
    private const string OmissionStart = "[+";
    private const string OmissionEnd = " more]";
    private const string OmissionEndShort = "]";
    private const string OmissionEllipsisSuffix = "[+]";

    private static readonly (string Start, string Stop)[] OmissionAttempts = new[]
    {
        (OmissionStart, OmissionEnd),
        (OmissionStart, OmissionEndShort),
    };

    /// <summary>
    /// Convert given <paramref name="self"/>
    /// to value of limited in length, with optional suffix
    /// string, by trimming the original string from right.
    /// </summary>
    /// <param name="self">Value of the parameter.</param>
    /// <param name="trim">Defines maximum length of
    ///     the returned value together with <paramref name="fuzziness"/>,
    ///     i.e. the returned value length will not be longer than
    ///     <paramref name="trim"/> plus <paramref name="fuzziness"/>.</param>
    /// <param name="fuzziness">Fuzziness factor to use in order
    ///     not to trim.</param>
    /// <param name="suffix">Optional suffix to use to indicate
    ///     trimmed part, if set to default <see langword="null"/>
    ///     the ellipsis "..." will be used.</param>
    /// <returns>String representation of shortened <paramref name="self"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if required arguments
    ///     is <see langword="null" />.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="trim"/> is zero,
    ///     or if suffix is longer than maximum length
    ///     of the returned value.</exception>
    public static string Ellipsis(
            this string self,
            ushort trim = 24,
            ushort fuzziness = 6,
            string? suffix = null)
    {
        return self.EllipsisWithModifier(
                trim: trim,
                fuzziness: fuzziness,
                suffix: suffix);
    }

    /// <summary>
    /// Convert given <paramref name="self"/>
    /// to value of limited in length for purpose of trully representing
    /// the original value. Unlike <see cref="Ellipsis(string, ushort, ushort, string?)"/>
    /// this method shows omitted lenght and is less visually pleasing.
    /// Trading visual for authenticity.
    /// </summary>
    /// <param name="self">Value of the parameter.</param>
    /// <param name="trim">Defines maximum length of
    ///     the returned value together with <paramref name="fuzziness"/>,
    ///     i.e. the returned value length will not be longer than
    ///     <paramref name="trim"/> plus <paramref name="fuzziness"/>.</param>
    /// <param name="fuzziness">Fuzziness factor to use in order
    ///     not to trim.</param>
    /// <returns>String representation of shortened <paramref name="self"/>
    /// whoch is either original string if short or form like:
    /// "original string head[+N more]" where N stands for character count.</returns>
    /// <exception cref="ArgumentNullException">Thrown if required arguments
    ///     is <see langword="null" />.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="trim"/> is zero.</exception>
    public static string Snippet(
            this string self,
            ushort trim = 128,
            ushort fuzziness = 6)
    {
        return self.SnippetWithModifier(
                trim: trim,
                fuzziness: fuzziness);
    }

    /// <summary>
    /// Convert given <paramref name="self"/>
    /// to value of limited in length for purpose of trully representing
    /// the original value as literal. Unlike <see cref="Ellipsis(string, ushort, ushort, string?)"/>
    /// this method shows omitted lenght and is less visually pleasing.
    /// Trading visual for authenticity.
    /// </summary>
    /// <param name="self">Value of the parameter.</param>
    /// <param name="trim">Defines maximum length of
    ///     the returned value before converting to litera
    ///     together with <paramref name="fuzziness"/>,
    ///     i.e. the returned value character count length will not be longer than
    ///     <paramref name="trim"/> plus <paramref name="fuzziness"/>.</param>
    /// <param name="fuzziness">Fuzziness factor to use in order
    ///     not to trim.</param>
    /// <param name="literalDefinition">Definition
    ///     of the string literal to use,
    ///     defaults to conservative C like string literal.</param>
    /// <returns>String representation of shortened <paramref name="self"/>
    /// whoch is either original string if short or form like:
    /// '"original string head"[+N more]' where N stands for character count.</returns>
    /// <exception cref="ArgumentNullException">Thrown if required arguments
    ///     is <see langword="null" />.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="trim"/> is zero.</exception>
    public static string SnippetLiteral(
            this string self,
            ushort trim = 128,
            ushort fuzziness = 6,
            IStringLiteralDefinition? literalDefinition = null)
    {
        return self.SnippetWithModifier(
                trim: trim,
                fuzziness: fuzziness,
                modifier: h => Dump.Literal(h, literalDefinition: literalDefinition));
    }

    /// <summary>
    /// Split given <paramref name="self"/>
    /// to parts not containing any new line character.
    /// Composite line breaks like '\r\n' have precedence
    /// over single character line breaks.
    /// </summary>
    /// <param name="self">String to split.</param>
    /// <param name="options">Optional split options.</param>
    /// <returns>Enumeration of line break free lines.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static string[] ToLines(
            this string self,
            StringSplitOptions options = StringSplitOptions.None)
    {
        // The order here is important, we do not want to have
        // more splits than necessary
        return Ensure.Param(self).Value
                .Split(new[] { "\r\n", "\n\r", "\n", "\r" }, options: options);
    }

    private static string EllipsisWithModifier(
            this string self,
            ushort trim = 24,
            ushort fuzziness = 6,
            string? suffix = null,
            Func<string, string>? modifier = null)
    {
        Ensure.Param<string>(self).Done();

        Ensure.Param(trim)
                .StrictlyPositive().Done();

        int max = trim + fuzziness;
        modifier ??= h => h;

        suffix = Ensure.Optional(suffix)
                .InRange(0, max)
                .ValueOr("...");

        if (self.Length <= max)
        {
            return modifier(self);
        }
        else if (trim > suffix.Length)
        {
            // suffix fits in trim length
            return modifier(self[..(trim - suffix.Length)]) + suffix;
        }
        else if (max >= suffix.Length)
        {
            // suffix is long compared to trim length
            return modifier(self[..(max - suffix.Length)]) + suffix;
        }
        else
        {
            // default suffix is way too long compared to trim length
            suffix = new string('.', max - 1);

            return modifier(self[..1]) + suffix;
        }
    }

    private static string SnippetWithModifier(
            this string self,
            ushort trim = 128,
            ushort fuzziness = 6,
            Func<string, string>? modifier = null)
    {
        Ensure.Param<string>(self).Done();

        Ensure.Param(trim)
                .StrictlyPositive().Done();

        int max = trim + fuzziness;

        if (self.Length <= max)
        {
            if (modifier is null)
            {
                return self;
            }

            return modifier(self);
        }

        foreach ((string start, string stop) in OmissionAttempts)
        {
            int numbersLength = Math.Min(
                    (self.Length - trim).ToString(CultureInfo.InvariantCulture).Length,
                    self.Length.ToString(CultureInfo.InvariantCulture).Length);
            int tailLength = start.Length + numbersLength + stop.Length;

            if (tailLength <= trim)
            {
                int headLength = trim - tailLength;
                string head = self[..headLength];

                if (modifier is not null)
                {
                    head = modifier(head);
                }

                string tail = $"{start}{self.Length - headLength}{stop}";

                return head + tail;
            }
        }

        string suffix = OmissionEllipsisSuffix;

        if (suffix.Length > max)
        {
            suffix = suffix[..max];
        }

        return self.EllipsisWithModifier(
                trim: trim,
                fuzziness: fuzziness,
                suffix: suffix,
                modifier: modifier);
    }
}
