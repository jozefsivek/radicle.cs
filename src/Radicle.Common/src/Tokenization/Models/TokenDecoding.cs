namespace Radicle.Common.Tokenization.Models;

using System;
using Radicle.Common;
using Radicle.Common.Check;
using Radicle.Common.MetaData;

/// <summary>
/// Base of immutable token decodigns with failure handling.
/// </summary>
/// <remarks>
/// The full hierarchy is as follows:
/// <code>
/// Token decoding class hierarchy:
///
///            TokenDecoding
///           /              \
///    TokenNoMatch       TokenMatch
///                      /          \
///               TokenFailure     TokenWith
///                               /         \
///                   TokenWithNoValue     TokenWithValue
///                                        /             \
///                                  TokenBinary     TokenString
/// </code>
/// </remarks>
public abstract class TokenDecoding : IOneFrom<TokenNoMatch, TokenFailure, TokenWithNoValue, TokenBinary, TokenString>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenDecoding"/> class.
    /// </summary>
    /// <param name="sourceString">Source string.</param>
    /// <param name="startAt">Inclusive position of start of decoding.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if positions are out of allowed range [0, input_length].</exception>
    internal TokenDecoding(
            string sourceString,
            int startAt)
    {
        this.SourceString = Ensure.Param(sourceString).Value;
        this.StartAt = Ensure.Param(startAt).InRange(0, sourceString.Length).Value;
    }

    /// <summary>
    /// Gets parent of this decoded string if any.
    /// The parent has string representation and its
    /// value is equal to <see cref="SourceString"/>.
    /// </summary>
    /// <remarks>Parent can be used to make multistace tokenization
    /// where parent tokens are again tokeninzed by child tokenizer.</remarks>
    public TokenString? Parent { get; init; }

    /// <summary>
    /// Gets original value of the source this token was made from.
    /// Can be empty.
    /// </summary>
    public string SourceString { get; } = string.Empty;

    /// <summary>
    /// Gets position in <see cref="SourceString"/> where token
    /// decoding happened (inclusive). It is in range
    /// [0, source_string_length], with upper bound
    /// leading to empty token.
    /// </summary>
    public int StartAt { get; }

    /// <inheritdoc/>
    public object SumTypeValue => this;

    /// <inheritdoc/>
    public byte SumTypeValueIndex
    {
        get
        {
            if (this is TokenString)
            {
                return 4;
            }
            else if (this is TokenBinary)
            {
                return 3;
            }
            else if (this is TokenWithNoValue)
            {
                return 2;
            }
            else if (this is TokenFailure)
            {
                return 1;
            }
            else if (this is TokenNoMatch)
            {
                return 0;
            }

            throw new NotSupportedException(
                    $"BUG: {nameof(TokenDecoding)} of type {this.GetType().Name} "
                    + "is not counted into the sum type");
        }
    }

    /// <summary>
    /// Return representation of this instance
    /// as one of the possible subtypes.
    /// </summary>
    /// <returns>Sum type of all possible values.</returns>
    [Experimental("Currently under experimental use.")]
    public IOneFrom<TokenNoMatch, TokenFailure, TokenWithNoValue, TokenBinary, TokenString> AsOneFrom()
    {
        return this;
    }
}
