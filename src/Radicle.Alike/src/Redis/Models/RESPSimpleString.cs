namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of the RESP simple string.
/// </summary>
public sealed class RESPSimpleString : RESPStringLikeValue
{
    /// <summary>
    /// Gets human name of this value.
    /// </summary>
    public const string HumanName = "simple string";

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPSimpleString"/> class.
    /// </summary>
    /// <param name="stringValue">Value of this simple string
    ///     which will be encoded as UTF-8 if required.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if
    ///     <paramref name="stringValue"/> contains new lines.</exception>
    public RESPSimpleString(string stringValue)
        : base(Ensure.Param(stringValue).NoNewLines().Value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPSimpleString"/> class.
    /// </summary>
    /// <param name="value">Binary value of this string.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if
    ///     <paramref name="value"/> contains new lines of any kind.</exception>
    public RESPSimpleString(IEnumerable<byte> value)
        : base(Ensure.Param(value)
                .AllNotNull(b => Ensure.Param(b)
                    .That(b => !RESPNames.AllNewLineBytes.Contains(b)).Done()))
    {
    }

    /// <inheritdoc/>
    public override RESPDataType Type => RESPDataType.SimpleString;

    /// <summary>
    /// Implicitly cast of <paramref name="stringValue"/>
    /// to instance of <see cref="RESPSimpleString"/>.
    /// </summary>
    /// <param name="stringValue">Value to cast.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if
    ///     <paramref name="stringValue"/> contains new lines.</exception>
    public static implicit operator RESPSimpleString(string stringValue)
    {
        return new RESPSimpleString(stringValue);
    }

    /// <summary>
    /// Convert <paramref name="stringValue"/>
    /// to instance of <see cref="RESPSimpleString"/>.
    /// </summary>
    /// <param name="stringValue">Value to cast.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if
    ///     <paramref name="stringValue"/> contains new lines.</exception>
    /// <returns>Instance of <see cref="RESPSimpleString"/>.</returns>
    public static RESPSimpleString FromString(string stringValue)
    {
        return new RESPSimpleString(stringValue);
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
        return other is RESPSimpleString simpleString
                && this.Value.SequenceEqual(simpleString.Value)
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
