namespace Radicle.Common.Check.Models;

using System;
using System.Collections.Generic;
using Radicle.Common.Check.Models.Generic;

/// <summary>
/// Interface or string method parameter.
/// </summary>
public interface IStringParam : IParam<string>, IEnumerable<char>
{
    /// <summary>
    /// Throws if the parameter value is empty string.
    /// </summary>
    /// <returns>This instance of <see cref="IStringParam"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the
    ///     parameter is an empty string.</exception>
    IStringParam NotEmpty();

    /// <summary>
    /// Checks if the parameter value is <see langword="null"/>,
    /// an empty string or white space string.
    /// </summary>
    /// <returns>This instance of <see cref="IStringParam"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the
    ///     parameter value is an empty string.</exception>
    /// <exception cref="ArgumentException">Thrown when the
    ///     parameter is whitespace string.</exception>
    IStringParam NotWhiteSpace();

    /// <summary>
    /// Checks if the parameter value length is inside given range, throws if not.
    /// By default the range is inclusive.
    /// </summary>
    /// <param name="lowerBound">The lower bound, included by default.</param>
    /// <param name="upperBound">The upper bound, included by default.</param>
    /// <param name="includeLower">Indicates if the <paramref name="lowerBound"/>
    ///     is included in allowed range, defaults to <see langword="true"/>.</param>
    /// <param name="includeUpper">Indicates if the <paramref name="upperBound"/>
    ///     is included in allowed range, defaults to <see langword="true"/>.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the
    ///     parameter length is outside of the given range
    ///     ( | [ <paramref name="lowerBound"/>, <paramref name="upperBound"/> ] | ).</exception>
    IStringParam InRange(
            int lowerBound,
            int upperBound,
            bool includeLower = true,
            bool includeUpper = true);

    /// <summary>
    /// Checks if the parameter value is regular expression.
    /// </summary>
    /// <returns>This instance of <see cref="IStringParam"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if the
    ///     parameter does not represents regular expression.</exception>
    IStringParam IsRegex();

    /// <summary>
    /// Checks if the parameter value conforms
    /// to the given <paramref name="specification"/>.
    /// </summary>
    /// <param name="specification">Specification the string argument
    ///     should conform to.</param>
    /// <returns>This instance of <see cref="IStringParam"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if
    ///     <paramref name="specification"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if parameter value length is out of allowed range.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if parameter value does not conform to given
    ///     <paramref name="specification"/>.</exception>
    IStringParam Conforms(ITypedNameSpec specification);
}
