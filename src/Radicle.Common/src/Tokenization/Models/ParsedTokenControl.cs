namespace Radicle.Common.Tokenization.Models;

using System;

/// <summary>
/// Immutable representation of the control token.
/// This is a token which was preceeded by special stop word.
/// </summary>
public sealed class ParsedTokenControl : ParsedToken
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParsedTokenControl"/> class.
    /// </summary>
    /// <param name="value">Character of the escape token.</param>
    /// <param name="incompleteRead">Flag indicating incomplete read,
    ///     a potential error.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public ParsedTokenControl(
            string value,
            bool incompleteRead = false)
            : base(value)
    {
        this.IncompleteRead = incompleteRead;
    }

    /// <summary>
    /// Gets a value indicating whether this token is a result
    /// of incomplete read, i.e. missing escape character.
    /// </summary>
    public bool IncompleteRead { get; }

    /// <inheritdoc/>
    public override ParsedTokenType Type => ParsedTokenType.Control;

    /// <inheritdoc/>
    public override bool Equals(ParsedToken? other)
    {
        return base.Equals(other)
                && other is ParsedTokenControl c
                && this.IncompleteRead == c.IncompleteRead;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.Value, this.IncompleteRead);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        string read = this.IncompleteRead ? "*" : string.Empty;

        return $"[control] {base.ToString()}{read}";
    }
}
