namespace Radicle.Common.Tokenization.Models;

using System;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of parsed token from stop word string parser,
/// which can be either stop word, free text or control sequence.
/// Note this data model has no failure handling, only incomplete read handling.
/// </summary>
public abstract class ParsedToken : IEquatable<ParsedToken>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParsedToken"/> class.
    /// </summary>
    /// <param name="token">String value of this stope token.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    internal ParsedToken(string token)
    {
        this.Value = Ensure.Param(token).Value;
    }

    /// <summary>
    /// Gets type of this token.
    /// </summary>
    public abstract ParsedTokenType Type { get; }

    /// <summary>
    /// Gets string value of this token.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets length of this token value.
    /// </summary>
    public int Length => this.Value.Length;

    /// <summary>
    /// Gets a value indicating whether this stop token is empty.
    /// </summary>
    public bool IsEmpty => this.Value.Length == 0;

    /// <summary>
    /// Equals.
    /// </summary>
    /// <param name="one">Left operand.</param>
    /// <param name="other">Right operand.</param>
    /// <returns><see langword="true"/> if equal.</returns>
    public static bool operator ==(ParsedToken? one, ParsedToken? other)
    {
        return one is null ? other is null : one.Equals(other);
    }

    /// <summary>
    /// Non equals.
    /// </summary>
    /// <param name="one">Left operand.</param>
    /// <param name="other">Right operand.</param>
    /// <returns><see langword="true"/> if not equal.</returns>
    public static bool operator !=(ParsedToken? one, ParsedToken? other)
    {
        return !(one == other);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is ParsedToken token && this.Equals(token);
    }

    /// <inheritdoc />
    public virtual bool Equals(ParsedToken? other)
    {
        return other is not null
                && this.Value.Equals(other.Value, StringComparison.Ordinal);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.Value);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Dump.Literal(this.Value)}";
    }
}
