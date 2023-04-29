namespace Radicle.Common.Tokenization.Models;

using System;
using System.Collections.Generic;
using Radicle.Common.MetaData;

/// <summary>
/// Immutable representation of token with value.
/// </summary>
public abstract class TokenWithValue : TokenWith, IOneFrom<TokenBinary, TokenString>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenWithValue"/> class.
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
    internal TokenWithValue(
            string sourceString,
            int startAt,
            int endAt,
            int continueAt)
        : base(sourceString, startAt, endAt, continueAt)
    {
    }

    /// <inheritdoc/>
    public new byte SumTypeValueIndex
    {
        get
        {
            if (this is TokenString)
            {
                return 1;
            }
            else if (this is TokenBinary)
            {
                return 0;
            }

            throw new NotSupportedException(
                    $"BUG: {nameof(TokenDecoding)} of type {this.GetType().Name} "
                    + "is not counted into the sum type");
        }
    }

    /// <summary>
    /// Convert given <paramref name="value"/>
    /// to instance of <see cref="TokenWithValue"/>.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Instance of <see cref="TokenWithValue"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static implicit operator TokenWithValue(string value)
    {
        return ToTokenWithValue(value);
    }

    /// <summary>
    /// Convert given <paramref name="value"/>
    /// to instance of <see cref="TokenWithValue"/>.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Instance of <see cref="TokenWithValue"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static implicit operator TokenWithValue(byte[] value)
    {
        return ToTokenWithValue(value);
    }

    /// <summary>
    /// Convert given <paramref name="value"/>
    /// to instance of <see cref="TokenWithValue"/>.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Instance of <see cref="TokenWithValue"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static TokenWithValue ToTokenWithValue(string value)
    {
        return TokenString.GetPassThrough(value);
    }

    /// <summary>
    /// Convert given <paramref name="value"/>
    /// to instance of <see cref="TokenWithValue"/>.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Instance of <see cref="TokenWithValue"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static TokenWithValue ToTokenWithValue(IEnumerable<byte> value)
    {
        return TokenBinary.GetPassThrough(value);
    }

    /// <summary>
    /// Return representation of this instance
    /// as one of the possible subtypes.
    /// </summary>
    /// <returns>Sum type of all possible values.</returns>
    [Experimental("Currently under experimental use.")]
    public new IOneFrom<TokenBinary, TokenString> AsOneFrom()
    {
        return this;
    }
}
