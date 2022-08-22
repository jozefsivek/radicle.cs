namespace Radicle.Common;

using System;
using Radicle.Common.Tokenization;
using Radicle.Common.Tokenization.Models;

/// <summary>
/// Collection of (text) dump related utilities akin to those in Python.
/// </summary>
public static class Dump
{
    /// <summary>
    /// Returns string representation of the range
    /// in form: '['|'(' + lowerBound.ToString() + ', ' + upperBound.ToString() + ')'|']',
    /// where '[' or ']' denotes inclusion of boundaries and
    /// '(' or ')' exclusion of boundaries. The <see langword="null"/>
    /// values are represented as '?'.
    /// </summary>
    /// <param name="lowerBound">Lowed bound.</param>
    /// <param name="upperBound">Upper bound.</param>
    /// <param name="includeLower">Lower bound is included.</param>
    /// <param name="includeUpper">Upper bound is included.</param>
    /// <returns>String representation of the range.</returns>
    public static string Range(
            object? lowerBound,
            object? upperBound,
            bool includeLower = true,
            bool includeUpper = true)
    {
        string? bra = includeLower ? "[" : "(";
        string? ket = includeUpper ? "]" : ")";
        string? low = lowerBound?.ToString() ?? "?";
        string? up = upperBound?.ToString() ?? "?";

        return $"{bra}{low}, {up}{ket}";
    }

    /// <summary>
    /// Dump string value as string literal.
    /// </summary>
    /// <param name="value">Value to dump.</param>
    /// <param name="literalDefinition">Definition
    ///     of the string literal to use,
    ///     defaults to conservative C like string literal.</param>
    /// <returns>Encoded string with quotation marks.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static string Literal(
            string value,
            IStringLiteralDefinition? literalDefinition = null)
    {
        IStringLiteralDefinition def = literalDefinition
                ?? CLikeStringLiteralDefinition.Conservative;

        return def.GetLiteral(value);
    }

    /// <summary>
    /// Dump character as string literal.
    /// </summary>
    /// <param name="value">Value to dump.</param>
    /// <param name="literalDefinition">Definition
    ///     of the string literal to use,
    ///     defaults to conservative C like string literal.</param>
    /// <returns>Encoded string with quotation marks.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static string Literal(
            char value,
            IStringLiteralDefinition? literalDefinition = null)
    {
        IStringLiteralDefinition def = literalDefinition
                ?? CLikeStringLiteralDefinition.Conservative;

        return def.GetLiteral(new string(value, 1));
    }

    /// <summary>
    /// Dump value as string literal.
    /// </summary>
    /// <param name="value">Value to dump.</param>
    /// <param name="literalDefinition">Definition
    ///     of the string literal to use,
    ///     defaults to conservative C like string literal.</param>
    /// <returns>Encoded string with quotation marks.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static string Literal(
            TokenWithValue value,
            IStringLiteralDefinition? literalDefinition = null)
    {
        IStringLiteralDefinition def = literalDefinition
                ?? CLikeStringLiteralDefinition.Conservative;

        return def.GetLiteral(value);
    }
}
