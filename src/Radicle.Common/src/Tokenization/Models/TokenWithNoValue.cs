namespace Radicle.Common.Tokenization.Models;

using System;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of no-value token,
/// which was produced from the source but yield nothing.
/// </summary>
public sealed class TokenWithNoValue : TokenWith
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenWithNoValue"/> class.
    /// </summary>
    /// <param name="sourceString">Source string.</param>
    /// <param name="startAt">Inclusive position of start of decoding.</param>
    /// <param name="endAt">Inclusive position of end of decoding.</param>
    /// <param name="continueAt">Position immidiatelly after
    ///     string token in <paramref name="sourceString"/> or equal to
    ///     length of <paramref name="sourceString"/>. If this is equal to
    ///     lenght of <paramref name="sourceString"/> then the input was exhausted.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if positions are out of allowed range [0, input_length]
    ///     or <paramref name="startAt"/> is larger then <paramref name="endAt"/>
    ///     or <paramref name="continueAt"/> is smaller than <paramref name="endAt"/>.</exception>
    public TokenWithNoValue(
            string sourceString,
            int startAt,
            int endAt,
            int continueAt)
        : base(Ensure.Param(sourceString).Value, startAt, endAt, continueAt)
    {
    }
}
