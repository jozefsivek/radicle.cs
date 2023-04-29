namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of RESP blob string.
/// </summary>
public sealed class RESPVerbatimString : RESPStringLikeValue
{
    /// <summary>
    /// Gets human readable name.
    /// </summary>
    public const string HumanName = "verbatim string";

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPVerbatimString"/> class.
    /// </summary>
    /// <param name="stringType">Type of the string.</param>
    /// <param name="stringValue">Value of this string
    ///     which will be serialized in UTF-8 encoding.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public RESPVerbatimString(
            VerbatimStringType stringType,
            string stringValue)
        : base(stringValue)
    {
        this.StringType = stringType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPVerbatimString"/> class.
    /// </summary>
    /// <param name="stringType">Type of the string.</param>
    /// <param name="value">Binary value of this string.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public RESPVerbatimString(
            VerbatimStringType stringType,
            IEnumerable<byte> value)
        : base(value)
    {
        this.StringType = stringType;
    }

    /// <inheritdoc/>
    public override RESPDataType Type => RESPDataType.VerbatimString;

    /// <summary>
    /// Gets type of the string content.
    /// </summary>
    public VerbatimStringType StringType { get; }

    /// <summary>
    /// Gets a value indicating whether this value is empty.
    /// </summary>
    public bool IsEmpty => this.Value.Length == 0;

    /// <summary>
    /// Gets byte length of this blob string value.
    /// </summary>
    public ulong Length => (ulong)this.Value.Length;

    /// <summary>
    /// Implicitly cast of <paramref name="stringValue"/>
    /// to instance of plain text <see cref="RESPVerbatimString"/>.
    /// </summary>
    /// <param name="stringValue">Value to cast.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static implicit operator RESPVerbatimString(string stringValue)
    {
        return new RESPVerbatimString(VerbatimStringType.Text, stringValue);
    }

    /// <summary>
    /// Convert <paramref name="stringValue"/>
    /// to instance of plain text <see cref="RESPVerbatimString"/>.
    /// </summary>
    /// <param name="stringValue">Value to cast.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <returns>Instance of <see cref="RESPVerbatimString"/>.</returns>
    public static RESPVerbatimString FromString(string stringValue)
    {
        return new RESPVerbatimString(VerbatimStringType.Text, stringValue);
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
        return other is RESPVerbatimString verbatimString
                && this.StringType == verbatimString.StringType
                && this.Value.SequenceEqual(verbatimString.Value)
                && other.Attribs == this.Attribs;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.StringType, GetHashCode(this.Value), this.Attribs);
    }
}
