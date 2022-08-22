namespace Radicle.Common.Tokenization.Models;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Radicle.Common.Check;

/// <summary>
/// Immutable represetnation of token with binary value.
/// </summary>
public sealed class TokenBinary : TokenWithValue
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenBinary"/> class.
    /// </summary>
    /// <remarks>Note <paramref name="bytes"/> does not have
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
    /// <param name="bytes">Decoded bytes.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if positions are out of allowed range [0, input_length]
    ///     or <paramref name="startAt"/> is larger then <paramref name="endAt"/>
    ///     or <paramref name="continueAt"/> is smaller than <paramref name="endAt"/>.</exception>
    public TokenBinary(
            string sourceString,
            int startAt,
            int endAt,
            int continueAt,
            IEnumerable<byte> bytes)
        : base(sourceString, startAt, endAt, continueAt)
    {
        this.Bytes = Ensure
                .Collection(bytes)
                .ToImmutableArray();
    }

    /// <summary>
    /// Gets decoded bytes. Can be empty, as empty string token.
    /// </summary>
    public ImmutableArray<byte> Bytes { get; } = ImmutableArray<byte>.Empty;

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
    public static TokenBinary GetPassThrough(
            IEnumerable<byte> input,
            int startAt = 0)
    {
        Ensure.Param(input).Done();

        int inputLength = input.Count();

        Ensure.Param(startAt).InRange(0, inputLength).Done();

        IEnumerable<byte> value;

        if (startAt != 0)
        {
            if (startAt == inputLength)
            {
                value = Array.Empty<byte>();
            }
            else
            {
                value = input.Skip(startAt);
            }
        }
        else
        {
            value = input;
        }

        return new TokenBinary(
                string.Empty,
                startAt,
                endAt: startAt + Math.Max(inputLength - startAt - 1, 0),
                continueAt: startAt + inputLength - startAt,
                bytes: value);
    }
}
