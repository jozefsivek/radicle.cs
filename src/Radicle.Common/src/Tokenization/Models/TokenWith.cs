namespace Radicle.Common.Tokenization.Models;

using System;
using Radicle.Common.Check;

/// <summary>
/// Immutable base for no-value as well as value tokens.
/// </summary>
public abstract class TokenWith : TokenMatch
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenWith"/> class.
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
    internal TokenWith(
            string sourceString,
            int startAt,
            int endAt,
            int continueAt)
        : base(sourceString, startAt, endAt)
    {
        if (continueAt != sourceString.Length)
        {
            this.ContinueAt = Ensure.Param(continueAt).GreaterThan(endAt).Value;
        }
        else
        {
            this.ContinueAt = continueAt;
        }
    }

    /// <summary>
    /// Gets exit position to continue reading of input on.
    /// If it is equal to the input length the source was exhausted.
    /// </summary>
    public int ContinueAt { get; }
}
