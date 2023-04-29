namespace Radicle.Common.Tokenization.Models;

using System;

/// <summary>
/// Immutable representation of no-match for token decoding.
/// E.g. if the probed input does not contain expected token form
/// of the token so it is not even possible to continue in searching for token,
/// then this will be the result of decoding.
/// </summary>
public sealed class TokenNoMatch : TokenDecoding
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenNoMatch"/> class.
    /// </summary>
    /// <param name="sourceString">Source string.</param>
    /// <param name="startAt">Inclusive position of start of decoding.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if positions are out of allowed range [0, input_length].</exception>
    public TokenNoMatch(string sourceString, int startAt)
            : base(sourceString, startAt)
    {
    }
}
