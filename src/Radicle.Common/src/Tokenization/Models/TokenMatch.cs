namespace Radicle.Common.Tokenization.Models;

using System;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of token match.
/// This can be successfull or not.
/// </summary>
public abstract class TokenMatch : TokenDecoding
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenMatch"/> class.
    /// </summary>
    /// <param name="sourceString">Source string.</param>
    /// <param name="startAt">Inclusive position of start of decoding.</param>
    /// <param name="endAt">Inclusive position of end of decoding.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if positions are out of allowed range [0, input_length]
    ///     or <paramref name="startAt"/> is larger then <paramref name="endAt"/>.</exception>
    internal TokenMatch(
            string sourceString,
            int startAt,
            int endAt)
        : base(sourceString, startAt)
    {
        this.EndAt = Ensure.Param(endAt).GreaterThanOrEqual(startAt).Value;
    }

    /// <summary>
    /// Gets position in <see cref="TokenDecoding.SourceString"/> where literal was ended (inclusive).
    /// In case of <see cref="TokenFailure"/> it is last read position
    /// with formatting error.
    /// </summary>
    public int EndAt { get; }

    /// <summary>
    /// Gets position in root parent <see cref="TokenDecoding.SourceString"/> where literal was ended (inclusive).
    /// In case of <see cref="TokenFailure"/> it is last read position
    /// with formatting error.
    /// </summary>
    public int RootEndAt
    {
        get
        {
            if (this.Parent is null)
            {
                return this.EndAt;
            }

            return this.EndAt + this.Parent.StartAt;
        }
    }
}
