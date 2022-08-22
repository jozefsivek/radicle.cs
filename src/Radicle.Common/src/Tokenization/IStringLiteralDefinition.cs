namespace Radicle.Common.Tokenization;

using System;
using Radicle.Common.Tokenization.Models;

/// <summary>
/// Definition for encoding string literals.
/// </summary>
public interface IStringLiteralDefinition
{
    /// <summary>
    /// Gets Quotation mark start character.
    /// </summary>
    string QuotationMarkStart { get; }

    /// <summary>
    /// Gets Quotation mark end character.
    /// </summary>
    string QuotationMarkEnd { get; }

    /// <summary>
    /// Gets escape start character.
    /// </summary>
    string EscapeTokenStart { get; }

    /// <summary>
    /// Gets escape end character.
    /// </summary>
    string EscapeTokenEnd { get; }

    /// <summary>
    /// Encode given value to escaped string with no literal boundaries.
    /// See <see cref="GetLiteral(TokenWithValue)"/>.
    /// </summary>
    /// <param name="value">Value to encode.</param>
    /// <returns>Encoded string with no quotation marks.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public string Encode(TokenWithValue value);

    /// <summary>
    /// Encode given raw value to escaped string literal with boundaries.
    /// </summary>
    /// <param name="value">Value to encode.</param>
    /// <returns>Encoded string with quotation marks.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public string GetLiteral(TokenWithValue value);

    /// <summary>
    /// Decode given escaped string with no with no literal boundaries
    /// (quotation marks).
    /// </summary>
    /// <param name="input">Input to decode.</param>
    /// <returns>instance of <see cref="TokenBinary"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public TokenMatch Decode(string input);

    /// <summary>
    /// Try to read the string literal from <paramref name="input"/>
    /// with position starting at <paramref name="startAt"/>.
    /// </summary>
    /// <param name="input">Input to read.</param>
    /// <param name="startAt">Start position for parsing,
    ///     it is in inclusive range [0, input length]s.</param>
    /// <returns>instance of <see cref="TokenBinary"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="startAt"/> is out of bounds
    ///     of <paramref name="input"/> (i.e. outside [0, input_length]).</exception>
    public TokenDecoding TryReadStringLiteral(
            string input,
            int startAt = 0);

    /// <summary>
    /// Try to read the string literal from <paramref name="input"/>
    /// with position starting at <paramref name="startAt"/>.
    /// </summary>
    /// <param name="input">Input to read.</param>
    /// <param name="startAt">Start position for parsing,
    ///     it is in inclusive range [0, input length]s.</param>
    /// <returns>instance of <see cref="TokenBinary"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="startAt"/> is out of bounds
    ///     of <paramref name="input"/> (i.e. outside [0, input_length]).</exception>
    public TokenDecoding TryReadStringLiteral(
            TokenString input,
            int startAt = 0);
}
