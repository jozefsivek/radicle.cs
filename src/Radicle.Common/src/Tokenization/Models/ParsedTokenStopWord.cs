namespace Radicle.Common.Tokenization.Models;

using System;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of the parsed token stop word.
/// Such token is stop word defined by the parser.
/// </summary>
public sealed class ParsedTokenStopWord : ParsedToken
{
    /// <summary>
    /// Empty stop word.
    /// </summary>
    public static readonly ParsedTokenStopWord Empty = new(string.Empty);

    /// <summary>
    /// Initializes a new instance of the <see cref="ParsedTokenStopWord"/> class.
    /// </summary>
    /// <param name="value">Value of this stop word as read by parser.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="value"/> is empty.</exception>
    public ParsedTokenStopWord(string value)
        : base(Ensure.Param(value).NotEmpty().Value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParsedTokenStopWord"/> class.
    /// </summary>
    /// <param name="character">Value of this stop word as read by parser.</param>
    public ParsedTokenStopWord(char character)
        : base(new string(character, 1))
    {
    }

    /// <inheritdoc/>
    public override ParsedTokenType Type => ParsedTokenType.StopWord;

    /// <summary>
    /// Convert given <paramref name="character"/>
    /// to instance of <see cref="ParsedTokenStopWord"/>.
    /// </summary>
    /// <param name="character">Character to use.</param>
    /// <returns>Instance of <see cref="ParsedTokenStopWord"/>.</returns>
    public static implicit operator ParsedTokenStopWord(char character)
    {
        return ToParsedTokenStopWord(character);
    }

    /// <summary>
    /// Convert given <paramref name="character"/>
    /// to instance of <see cref="ParsedTokenStopWord"/>.
    /// </summary>
    /// <param name="character">Character to use.</param>
    /// <returns>Instance of <see cref="ParsedTokenStopWord"/>.</returns>
    public static ParsedTokenStopWord ToParsedTokenStopWord(char character)
    {
        return new ParsedTokenStopWord(character);
    }

    /// <inheritdoc/>
    public override bool Equals(ParsedToken? other)
    {
        return base.Equals(other)
                && other is ParsedTokenStopWord;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"[stop word] {base.ToString()}";
    }
}
