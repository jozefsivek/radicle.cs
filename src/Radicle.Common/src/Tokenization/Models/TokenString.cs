namespace Radicle.Common.Tokenization.Models;

using System;
using Radicle.Common.Check;

/// <summary>
/// Structure definiting result of attempt to read string literal.
/// </summary>
public sealed class TokenString : TokenWithValue
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenString"/> class.
    /// </summary>
    /// <remarks>Note <paramref name="stringValue"/> does not have
    ///     clear connection with form and length of <paramref name="sourceString"/>.
    ///     It depends on decoder or tokenizer on how given string input
    ///     is translated to bytes.</remarks>
    /// <param name="sourceString">Source string.</param>
    /// <param name="startAt">Inclusive position of start of decoding.</param>
    /// <param name="endAt">Inclusive position of end of decoding.</param>
    /// <param name="continueAt">Position immidiatelly after
    ///     string token in <paramref name="sourceString"/> or equal to
    ///     length of <paramref name="sourceString"/>. If this is equal to
    ///     lenght of <paramref name="sourceString"/> then the input was exhausted.</param>
    /// <param name="stringValue">Decoded string.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if positions are out of allowed range [0, input_length]
    ///     or <paramref name="startAt"/> is larger then <paramref name="endAt"/>
    ///     or <paramref name="continueAt"/> is smaller than <paramref name="endAt"/>.</exception>
    public TokenString(
            string sourceString,
            int startAt,
            int endAt,
            int continueAt,
            string stringValue)
        : base(sourceString, startAt, endAt, continueAt)
    {
        this.StringValue = Ensure
                .Param(stringValue)
                .Value;
    }

    /// <summary>
    /// Gets decoded bytes. Can be empty, as empty string token.
    /// </summary>
    public string StringValue { get; } = string.Empty;

    /// <summary>
    /// Get pass through instance of <see cref="TokenString"/>
    /// with <paramref name="input"/> and value equal to substring of this
    /// <paramref name="input"/>.
    /// Bytes are obtaint assuming UTF-8 encoding.
    /// </summary>
    /// <param name="input">Original input string.</param>
    /// <param name="startAt">Inclusive oosition of start of decoding,
    ///     this can be start of literal or just encoded value.</param>
    /// <returns>Instance of <see cref="TokenBinary"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="startAt"/> is out of allowed
    ///     range [0, input_length].</exception>
    public static TokenString GetPassThrough(
            string input,
            int startAt = 0)
    {
        Ensure.Param(input).Done();
        Ensure.Param(startAt).InRange(0, input.Length).Done();

        string stringValue;

        if (startAt != 0)
        {
            if (startAt == input.Length)
            {
                stringValue = string.Empty;
            }
            else
            {
                stringValue = input[startAt..];
            }
        }
        else
        {
            stringValue = input;
        }

        return new TokenString(
                input,
                startAt,
                endAt: startAt + Math.Max(stringValue.Length - 1, 0),
                continueAt: startAt + stringValue.Length,
                stringValue: stringValue);
    }

    /// <summary>
    /// Get pass through instance of <see cref="TokenString"/>
    /// with <paramref name="input"/> and value equal to substring of this
    /// <paramref name="input"/>.
    /// Bytes are obtaint assuming UTF-8 encoding.
    /// </summary>
    /// <param name="input">Original input string.</param>
    /// <param name="startAt">Inclusive oosition of start of decoding,
    ///     this can be start of literal or just encoded value.</param>
    /// <returns>Instance of <see cref="TokenBinary"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="startAt"/> is out of allowed
    ///     range [0, input_length].</exception>
    public static TokenString GetPassThrough(
            TokenString input,
            int startAt = 0)
    {
        Ensure.Param(input).Done();
        Ensure.Param(startAt).InRange(0, input.StringValue.Length).Done();

        string stringValue;

        if (startAt != 0)
        {
            if (startAt == input.StringValue.Length)
            {
                stringValue = string.Empty;
            }
            else
            {
                stringValue = input.StringValue[startAt..];
            }
        }
        else
        {
            stringValue = input.StringValue;
        }

        return new TokenString(
                input.StringValue,
                startAt,
                endAt: startAt + Math.Max(stringValue.Length - 1, 0),
                continueAt: startAt + stringValue.Length,
                stringValue: stringValue)
        {
            Parent = input,
        };
    }
}
