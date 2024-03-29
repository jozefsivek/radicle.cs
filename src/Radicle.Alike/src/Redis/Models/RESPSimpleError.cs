namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of RESP simple error.
/// </summary>
public sealed class RESPSimpleError : RESPStringLikeValue
{
    /// <summary>
    /// Gets human readable name of this type.
    /// </summary>
    public const string HumanName = "simple error";

    /// <summary>
    /// Gets general error code, this code is first word in the error value.
    /// </summary>
    public const string GeneralErrorCode = "ERR";

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPSimpleError"/> class.
    /// </summary>
    /// <param name="stringValue">Value of this simple string
    ///     which will be encoded as UTF-8 if required.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if
    ///     <paramref name="stringValue"/> contains new lines.</exception>
    public RESPSimpleError(string stringValue)
        : base(Ensure.Param(stringValue).NoNewLines().Value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPSimpleError"/> class.
    /// </summary>
    /// <param name="value">Binary value of this string.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if
    ///     <paramref name="value"/> contains new lines of any kind.</exception>
    public RESPSimpleError(IEnumerable<byte> value)
        : base(Ensure.Param(value)
                .AllNotNull(b => Ensure.Param(b)
                    .That(b => !RESPNames.AllNewLineBytes.Contains(b)).Done()))
    {
    }

    /// <inheritdoc/>
    public override RESPDataType Type => RESPDataType.SimpleError;

    /// <summary>
    /// Implicitly cast of <paramref name="stringValue"/>
    /// to instance of <see cref="RESPSimpleError"/>.
    /// </summary>
    /// <param name="stringValue">Value to cast.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if
    ///     <paramref name="stringValue"/> contains new lines.</exception>
    public static implicit operator RESPSimpleError(string stringValue)
    {
        return new RESPSimpleError(stringValue);
    }

    /// <summary>
    /// Convert <paramref name="stringValue"/>
    /// to instance of <see cref="RESPSimpleError"/>.
    /// </summary>
    /// <param name="stringValue">Value to cast.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if
    ///     <paramref name="stringValue"/> contains new lines.</exception>
    /// <returns>Instance of <see cref="RESPSimpleError"/>.</returns>
    public static RESPSimpleError FromString(string stringValue)
    {
        return new RESPSimpleError(stringValue);
    }

    /// <inheritdoc/>
    public override TResult Accept<TResult>(IRESPValueVisitor<TResult> visitor)
    {
        Ensure.Param(visitor).Done();

        return visitor.Visit(this);
    }

    /// <inheritdoc/>
    public override bool Equals(RESPValue? other)
    {
        return other is RESPSimpleError simpleError
                && this.Value.SequenceEqual(simpleError.Value)
                && other.Attribs == this.Attribs;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(
                GetHashCode(this.Value),
                this.Attribs);
    }
}
