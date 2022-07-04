namespace Radicle.Common.Check;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

/// <summary>
/// Immutable class holding specification of the name.
/// </summary>
public sealed class TypedNameSpec : ITypedNameSpec
{
    /*
     * Important: these specs are constants
     */

    /// <summary>
    /// Usual programming language identifier like
    /// in https://docs.python.org/2.5/ref/identifiers.html .
    /// Alphanumerical (ASCII) plus underscore "_",
    /// with non number as first character.
    /// Default length range is [1, 256] characters.
    /// Pattern is lowercase only and case is ignored in pattern by default.
    /// </summary>
    public static readonly TypedNameSpec Programming = new(
            "Programming name",
            "\\A[a-z_][a-z0-9_]{0,255}\\z",
            ignoreCaseInPattern: true,
            description: "alphanumerical and '_', non number first character");

    /// <summary>
    /// Ordinary programming language identifier like
    /// in https://docs.python.org/3/reference/lexical_analysis.html#keywords .
    /// I.e. it is <see cref="Programming"/> and
    /// it is not equal to general set of reserved keywords
    /// in the programming languages (see this
    /// <see cref="DisallowedValues"/>).
    /// Default length range is [1, 256] characters.
    /// Pattern is lowercase only and case is ignored in pattern by default.
    /// </summary>
    public static readonly TypedNameSpec ProgrammingOrdinary = Programming.With(
            name: "Ordinary programming name",
            additionalDisallowedValues: new[]
            {
                "true",
                "false",
                "none",
                "null",
                "NaN",
                "NaV",
                "undefined",
                "default",
                "as",
                "not",
                "and",
                "or",
                "xor",
                "is",
                "begins",
                "beginswith",
                "ends",
                "endswith",
                "superset",
                "subset",
                "del",
                "delete",
                "in",
                "ibegins",
                "ibeginswith",
                "iends",
                "iendswith",
                "isuperset",
                "isubset",
                "iin",

                "for",
                "foreach",
                "while",
                "try",
                "except",
                "catch",
                "finally",
                "with",
                "do",
                "case",
                "switch",
                "begin",
                "end",

                "global",
                "local",
                "nonlocal",
                "var",
                "let",
                "lambda",
                "class",
                "def",
                "new",
                "enum",
                "const",
                "pass",
                "raise",
                "throw",
                "return",
                "yield",
                "assert",
                "break",
                "continue",
                "import",
                "from",
                "export",
                "if",
                "else",
                "elif",
                "end",
                "fi",
            },
            description: "alphanumerical and '_', non number first character, except programming keywords");

    /// <summary>
    /// CSS class friendly and readable identifier: alphanumerical (ASCII)
    /// plus underscore "_" and or hyphen "-", with first non digit character.
    /// It can also not start with two hyphens "--" or number followed
    /// by hyphen e.g. "-9".Much like https://www.w3.org/TR/CSS21/syndata.html#characters
    /// with more restrictions on character set.
    /// Maximal length of the identifier is 256 characters, minimum 1.
    /// Pattern is lowercase only and case is ignored in pattern by default.
    /// </summary>
    public static readonly TypedNameSpec CSS = new(
            "CSS class name",
            @"\A(([a-z_][a-z0-9\-_]{0,255})|((\-[a-z_][a-z0-9_]{0,254})|(\-)))\z",
            ignoreCaseInPattern: true,
            description: "alphanumerical, '_' and '-', non number first character, '--' or '-' followed by number");

    /// <summary>
    /// Identifier which closely resembles IETF language tag composed
    /// of one or more alphanumerical sub-tags, up to 8 characters long
    /// separated by hyphen ("-", [Unicode] U+002D). The first tag is
    /// only composed of alphabet characters and denotes language code.
    /// This format is weaker than BCP-47 specification as full
    /// specification conformance will be overkill.
    /// Note that BCP-47 expects case insensitivity.
    /// Maximal length of the identifier is 256 characters, minimum 2.
    /// Pattern is lowercase only and case is ignored in pattern by default.
    /// </summary>
    public static readonly TypedNameSpec IETFLanguageTagLike = new(
            "IETF language tag",
            @"\A[a-z]{2,8}(\-[a-z0-9]{1,8})*\z",
            ignoreCaseInPattern: true,
            description: "alphanumerical sub-tags 1 to 8 characters long separated by '-', first sub-tag can not contain numbers and has to be at least 2 characters long",
            minLength: 2);

    /// <summary>
    /// Identifier of CLI (command line interface) verb, it is composed
    /// from alphanumerical non empty sub-tags separated by hyphen
    /// ("-", [Unicode] U+002D). Separator can not be present at the beginning
    /// or at the end of the identifier.
    /// Pattern is lowercase only and case is ignored in pattern by default.
    /// </summary>
    public static readonly TypedNameSpec CLIVerb = new(
            "CLI verb",
            @"\A[a-z0-9]+(\-[a-z0-9]+)*\z",
            ignoreCaseInPattern: true,
            description: "alphanumerical non empty sub-tags separated by '-', no leading or trailing '-'");

    /// <summary>
    /// Special token identifier used for inter system identification,
    /// global identification and similar. Alphanumerical (ASCII)
    /// plus underscore "_", hyphen "-" and or period ".", with
    /// no restrictions on the character placement.
    /// Maximal length of the identifier is 256 characters, minimum 1.
    /// Pattern is lowercase only and case is ignored in pattern by default.
    /// </summary>
    /// <remarks>This identifier should be URL safe
    /// see https://en.wikipedia.org/wiki/Percent-encoding#Percent-encoding_in_a_URI
    /// thus potential extension can include tilde character "~", any current
    /// use should count with this potential extension.</remarks>
    public static readonly TypedNameSpec URLToken = new(
            "URL token",
            @"\A[a-z0-9\.\-_]{1,256}\z",
            ignoreCaseInPattern: true,
            description: "alphanumerical, '_', '-' and '.'");

    /// <summary>
    /// Specifies the most general form of a name with any character
    /// and no new lines (either line feed, carriage return or any combination of them).
    /// Maximal length of the name is 16384 characters, minimum 1.
    /// </summary>
    public static readonly TypedNameSpec SingleLine = new(
            "Single line",
            @"\A[^\r\n]+\z",
            ignoreCaseInPattern: true,
            description: "any character except '\\n' or '\\r'",
            maxLength: 16384);

    private readonly Regex regex;

    private readonly string patternDescription;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedNameSpec"/> class.
    /// </summary>
    /// <param name="name">Short single or multi word,
    ///     single line, name.</param>
    /// <param name="regularExpressionPattern">Regular expression pattern which
    ///     needs to match the name value in addtion to other
    ///     restrictions in this specification. Use '\A' and '\z'
    ///     to frame expression pattern and use lower case characters in pattern.
    ///     The default regular expression options used are:
    ///     <see cref="RegexOptions.Compiled"/>, <see cref="RegexOptions.IgnoreCase"/>
    ///     (if <paramref name="ignoreCaseInPattern"/> is <see langword="true"/>),
    ///     <see cref="RegexOptions.CultureInvariant"/> and <see cref="RegexOptions.Singleline"/>.</param>
    /// <param name="disallowedValues">Explicitly disallowed values
    ///     in additon to given <paramref name="regularExpressionPattern"/>.</param>
    /// <param name="ignoreCaseInPattern">Ignore case in
    ///     <paramref name="regularExpressionPattern"/>.</param>
    /// <param name="ignoreCaseWhenCompared">Ignore case when comparing
    ///     2 values conforming to this specification.</param>
    /// <param name="description">Single line description
    ///     of the pattern. Defaults to pattern itself.
    ///     The final <see cref="Description"/> will be
    ///     extended with length limits, flags etc.</param>
    /// <param name="minLength">Additional restriction on minimum,
    ///     inclusive, name length.</param>
    /// <param name="maxLength">Additional restriction on maximum,
    ///     inclusive, name length.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if
    ///     <paramref name="name"/>, <paramref name="description"/>
    ///     or values in <paramref name="disallowedValues"/>
    ///     is empty, <paramref name="minLength"/> is zero
    ///     or <paramref name="maxLength"/>
    ///     is smaller than <paramref name="minLength"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if
    ///     <paramref name="name"/>, <paramref name="description"/>
    ///     or valus in <paramref name="disallowedValues"/>
    ///     is white space string
    ///     or <paramref name="regularExpressionPattern"/>
    ///     is not regular expression.</exception>
    /// <exception cref="ArgumentNullException">Thrown if
    ///     required parameter is <see langword="null"/>.</exception>
    public TypedNameSpec(
            string name,
            string regularExpressionPattern,
            IEnumerable<string>? disallowedValues = null,
            bool ignoreCaseInPattern = false,
            bool ignoreCaseWhenCompared = false,
            string? description = null,
            ushort minLength = 1,
            ushort maxLength = 256)
    {
        this.Name = Ensure
                .Param(name)
                .NotWhiteSpace()
                .Value;
        this.Pattern = Ensure
                .Param(regularExpressionPattern)
                .IsRegex()
                .Value;
        this.IgnoreCaseInPattern = ignoreCaseInPattern;
        this.IgnoreCaseWhenCompared = ignoreCaseWhenCompared;
        this.DisallowedValues = Ensure
                .Optional(disallowedValues)
                .AllNotNull(v => Ensure.Param(v).NotWhiteSpace())
                .ToImmutableHashSet(this.IgnoreCaseInPattern
                    ? StringComparer.OrdinalIgnoreCase
                    : StringComparer.Ordinal);
        this.patternDescription = Ensure
                .Optional(description)
                .NotWhiteSpace()
                .ValueOr($"/{this.Pattern}/"
                    + (this.IgnoreCaseInPattern ? "i" : string.Empty));
        this.MinLength = Ensure
                .Param(minLength)
                .StrictlyPositive()
                .Value;
        this.MaxLength = Ensure
                .Param(maxLength)
                .GreaterThanOrEqual(this.MinLength)
                .Value;

        RegexOptions options = RegexOptions.Compiled
                | RegexOptions.CultureInvariant
                | RegexOptions.Singleline;
        options = this.IgnoreCaseInPattern
                ? options | RegexOptions.IgnoreCase
                : options;

        this.regex = new Regex(this.Pattern, options);
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public string Description
    {
        get
        {
            string @case = this.IgnoreCaseInPattern
                    ? "mixed case"
                    : "lower case";
            string comparisonCase = this.IgnoreCaseWhenCompared
                    ? "case insensitive"
                    : "case sensitive";
            string r = Dump.Range(this.MinLength, this.MaxLength);

            return $"{this.Name}: {@case} {this.patternDescription} "
                    + $"with length range {r}, {comparisonCase} comparison";
        }
    }

    /// <inheritdoc/>
    public string Pattern { get; }

    /// <inheritdoc/>
    public IImmutableSet<string> DisallowedValues { get; }

    /// <inheritdoc/>
    public ushort MinLength { get; }

    /// <inheritdoc/>
    public ushort MaxLength { get; }

    /// <inheritdoc/>
    public bool IgnoreCaseInPattern { get; }

    /// <inheritdoc/>
    public bool IgnoreCaseWhenCompared { get; }

    /// <summary>
    /// Create new derived instance of <see cref="TypedNameSpec"/>
    /// from this instance by modifying some of the parameters.
    /// </summary>
    /// <param name="name">Optionally change the name of
    ///     the original specification.</param>
    /// <param name="additionalDisallowedValues">Additional
    ///     disallowed values in addtion to the original specification.</param>
    /// <param name="ignoreCaseInPattern">Optionally change case sensitivity
    ///     on the original pattern.</param>
    /// <param name="ignoreCaseWhenCompared">Optionally change original
    ///     iognore case when comparing 2 values conforming to this specification.</param>
    /// <param name="description">Optionally change the original
    ///     pattern description, this mainly makes sense if
    ///     the original one was plain pattern.</param>
    /// <param name="minLength">Optionally change the original
    ///     minimum length.</param>
    /// <param name="maxLength">Optionally change the original
    ///     maximum length.</param>
    /// <returns>New instance of <see cref="TypedNameSpec"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if
    ///     <paramref name="name"/>, <paramref name="description"/>
    ///     or values in <paramref name="additionalDisallowedValues"/>
    ///     is empty, <paramref name="minLength"/> is zero,
    ///     <paramref name="maxLength"/>
    ///     is smaller than <paramref name="minLength"/>
    ///     or <paramref name="minLength"/> and
    ///     <paramref name="maxLength"/> are outside
    ///     original name specification bounds.</exception>
    /// <exception cref="ArgumentException">Thrown if
    ///     <paramref name="name"/>, <paramref name="description"/>
    ///     or valus in <paramref name="additionalDisallowedValues"/>
    ///     is white space string.</exception>
    public TypedNameSpec With(
            string? name = null,
            IEnumerable<string>? additionalDisallowedValues = null,
            bool? ignoreCaseInPattern = null,
            bool? ignoreCaseWhenCompared = null,
            string? description = null,
            ushort? minLength = null,
            ushort? maxLength = null)
    {
        IEnumerable<string> disallowed = additionalDisallowedValues is null
                ? this.DisallowedValues
                : this.DisallowedValues.Concat(additionalDisallowedValues);
        ushort min = Ensure.Optional(minLength)
                .InRange(this.MinLength, this.MaxLength)
                .ValueOr(this.MinLength);
        ushort max = Ensure.Optional(maxLength)
                .InRange(this.MinLength, this.MaxLength)
                .ValueOr(this.MaxLength);

        return new TypedNameSpec(
                name ?? this.Name,
                this.Pattern,
                disallowedValues: disallowed,
                ignoreCaseInPattern: ignoreCaseInPattern ?? this.IgnoreCaseInPattern,
                ignoreCaseWhenCompared: ignoreCaseWhenCompared ?? this.IgnoreCaseWhenCompared,
                description: description ?? this.patternDescription,
                minLength: min,
                maxLength: max);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return this.Description;
    }

    /// <inheritdoc/>
    public bool IsValid(string value)
    {
        /*
        Match? match = this.regex.Match(value);
        _ = match.Name;
        */

        return value is not null
                && value.Length >= this.MinLength
                && value.Length <= this.MaxLength
                && this.regex.IsMatch(value)
                && !this.DisallowedValues.Contains(value);
    }

    /// <inheritdoc/>
    public string EnsureValid(
            string value,
            [CallerArgumentExpression("value")] string? parameterName = null)
    {
        return Ensure.Param(value, parameterName: parameterName)
                .InRange(this.MinLength, this.MaxLength)
                .That(
                    v => this.regex.IsMatch(v),
                    messageFactory: arg => $"{arg.DescriptionWithValue} "
                        + $"does not conform to specification of {this.Description}")
                .That(
                    v => !this.DisallowedValues.Contains(v),
                    messageFactory: arg => $"{arg.DescriptionWithValue} "
                        + $"is disallowed value for this specification of {this.Description}")
                .Value;
    }
}
