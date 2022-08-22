namespace Radicle.Common.Tokenization.Base;

using System;
using System.Collections.Generic;
using Radicle.Common.Check;
using Radicle.Common.Tokenization.Models;

/// <summary>
/// Base implementation of <see cref="IStringTokenizer"/>.
/// </summary>
/// <remarks>Note binary decoded strings are only parsed
/// ones, i.e. if the parent <see cref="IStringTokenizer"/> returns
/// binary decoded string it is not parsed further by this tokenizer.</remarks>
public abstract class StringTokenizer : IStringTokenizer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringTokenizer"/> class.
    /// </summary>
    /// <param name="parent">Optional parent tokenizer.</param>
    internal StringTokenizer(IStringTokenizer? parent = null)
    {
        this.Parent = parent;
    }

    /// <inheritdoc />
    public IStringTokenizer? Parent { get; }

    /// <inheritdoc/>
    public IEnumerable<TokenWithValue> Parse(
            string input,
            int startAt = 0)
    {
        Ensure.Param(input).Done();
        Ensure.Param(startAt).InRange(0, input.Length).Done();

        IEnumerable<TokenWithValue> parentTokens;

        if (this.Parent is null)
        {
            if (startAt == input.Length)
            {
                // nothing to parse
                parentTokens = Array.Empty<TokenWithValue>();
            }
            else
            {
                parentTokens = new[] { TokenString.GetPassThrough(input, startAt) };
            }
        }
        else
        {
            parentTokens = this.Parent.Parse(input, startAt: startAt);
        }

        foreach (TokenWithValue partialInput in parentTokens)
        {
            if (partialInput is TokenString tokenString)
            {
                int continueAt = 0;

                while (true)
                {
                    TokenMatch decoded = this.TryReadToken(
                            tokenString,
                            startAt: continueAt);

                    if (decoded is TokenFailure tokenFailure)
                    {
                        throw new FormatException(
                                $"Input format error at {tokenFailure.RootEndAt}: {tokenFailure.FormatErrorMessage}");
                    }
                    else if (decoded is TokenWith token)
                    {
                        continueAt = token.ContinueAt;

                        if (token is TokenWithValue tokenWithValue)
                        {
                            yield return tokenWithValue;
                        }

                        if (token.ContinueAt == tokenString.StringValue.Length)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                yield return partialInput;
            }
        }
    }

    /// <summary>
    /// Read the next token starting at <paramref name="startAt"/>.
    /// </summary>
    /// <remarks>This method does not take <see cref="Parent"/>
    /// into consideration.</remarks>
    /// <param name="input">String bearing input to parse.</param>
    /// <param name="startAt">Start position for parsing,
    ///     it is in inclusive range [0, input length].</param>
    /// <returns><see langword="true"/> if token was read;
    ///     <see langword="fase"/> if no token is present
    ///     in <paramref name="input"/> starting at position
    ///     <paramref name="startAt"/> (e.g. the input was exhausted).</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="startAt"/> is out of bounds
    ///     of <paramref name="input"/>.</exception>
    protected abstract TokenMatch TryReadToken(
            TokenString input,
            int startAt = 0);
}
