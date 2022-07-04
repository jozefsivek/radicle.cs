namespace Radicle.Common.Check;

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

/// <summary>
/// Interface of the <see cref="TypedNameSpec"/>
/// for testing and extension purposes.
/// </summary>
public interface ITypedNameSpec
{
    /// <summary>
    /// Gets short single or multi word, single line,
    /// name of this specification. It does not need
    /// to be uniqueue it is just shorthand for pattern.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets single line description of this specification.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets regular expression pattern
    /// which matches expected name value in addition
    /// to other restrictions in this specification.
    /// The pattern uses lowercase characters by default.
    /// Use of '\A' and '\z' to frame expression pattern is advised.
    /// The default regular expression options used are:
    /// <see cref="RegexOptions.Compiled"/>, <see cref="RegexOptions.IgnoreCase"/>
    /// (if <see cref="IgnoreCaseInPattern"/> is <see langword="true"/>),
    /// <see cref="RegexOptions.CultureInvariant"/> and <see cref="RegexOptions.Singleline"/>.
    /// </summary>
    string Pattern { get; }

    /// <summary>
    /// Gets explicit list of disallowed values. Although
    /// values could be part of the <see cref="Pattern"/>
    /// this could be inpractical making the string too long.
    /// </summary>
    IImmutableSet<string> DisallowedValues { get; }

    /// <summary>
    /// Gets minimum allowed length of the name.
    /// </summary>
    ushort MinLength { get; }

    /// <summary>
    /// Gets maximum allowed length of the name.
    /// </summary>
    ushort MaxLength { get; }

    /// <summary>
    /// Gets a value indicating whether to ignore case
    /// in <see cref="Pattern"/> and <see cref="DisallowedValues"/>.
    /// </summary>
    bool IgnoreCaseInPattern { get; }

    /// <summary>
    /// Gets a value indicating whether to ignore case
    /// when comparing 2 values conforming to this specification.
    /// </summary>
    bool IgnoreCaseWhenCompared { get; }

    /// <summary>
    /// Asses validity of the given <paramref name="value"/>
    /// against this specification.
    /// </summary>
    /// <param name="value">Name to test.</param>
    /// <returns><see langword="true"/> if the given <paramref name="value"/>
    ///     is valid against this specification; <see langword="false"/>
    ///     otherwise.</returns>
    bool IsValid(string value);

    /// <summary>
    /// Ensures validity of the given <paramref name="value"/>
    /// against this specification.
    /// </summary>
    /// <param name="value">Name to test.</param>
    /// <param name="parameterName">Original parameter name,
    ///     defaults to <paramref name="value"/> expression.</param>
    /// <returns><paramref name="value"/> if valid.</returns>
    /// <exception cref="ArgumentNullException">Thrown if
    ///     <paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="value"/> length is out of allowed range.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="value"/> does not conform to this
    ///     specification.</exception>
    string EnsureValid(
            string value,
            [CallerArgumentExpression("value")] string? parameterName = null);
}
