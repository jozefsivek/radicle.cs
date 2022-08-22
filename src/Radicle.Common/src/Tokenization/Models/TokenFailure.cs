namespace Radicle.Common.Tokenization.Models;

using System;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of token decoding failure.
/// </summary>
public sealed class TokenFailure : TokenMatch
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenFailure"/> class.
    /// </summary>
    /// <param name="sourceString">Source string.</param>
    /// <param name="startAt">Inclusive position of start of decoding.</param>
    /// <param name="endAt">Inclusive position of end of decoding, i.e.
    ///     where the error was encountered.</param>
    /// <param name="formatErrorMessage">Error message in case of error.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="formatErrorMessage"/> is empty or blank
    ///     or positions are out of allowed range [0, input_length]
    ///     of <paramref name="startAt"/> is larger then <paramref name="endAt"/>.</exception>
    public TokenFailure(
            string sourceString,
            int startAt,
            int endAt,
            string formatErrorMessage)
        : base(sourceString, startAt, endAt)
    {
        this.FormatErrorMessage = Ensure
                .Param(formatErrorMessage)
                .NotWhiteSpace()
                .Value;
    }

    /// <summary>
    /// Gets short human readable message explaining format error.
    /// Can be empty in case of no error.
    /// </summary>
    public string FormatErrorMessage { get; }
}
