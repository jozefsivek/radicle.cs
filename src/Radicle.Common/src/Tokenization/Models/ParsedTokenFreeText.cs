namespace Radicle.Common.Tokenization.Models;

using System;

/// <summary>
/// Immutable representaion of free text token between other tokens.
/// </summary>
public sealed class ParsedTokenFreeText : ParsedToken
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParsedTokenFreeText"/> class.
    /// </summary>
    /// <param name="token">Free text token.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public ParsedTokenFreeText(string token)
            : base(token)
    {
    }

    /// <inheritdoc/>
    public override ParsedTokenType Type => ParsedTokenType.FreeText;

    /// <inheritdoc/>
    public override bool Equals(ParsedToken? other)
    {
        return base.Equals(other)
                && other is ParsedTokenFreeText;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"[free text] {base.ToString()}";
    }
}
