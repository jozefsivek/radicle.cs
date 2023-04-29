namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of RESP blob string.
/// </summary>
public sealed class RESPVerbatimString : RESPValue
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
    {
        Ensure.Param(stringValue).Done();

        this.StringType = stringType;
        this.Value = RESPNames.DefaultEncoding.GetBytes(stringValue)
                .ToImmutableArray();
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
    {
        this.StringType = stringType;
        this.Value = Ensure.Param(value).ToImmutableArray();
    }

    /// <inheritdoc/>
    public override RESPDataType Type => RESPDataType.VerbatimString;

    /// <summary>
    /// Gets type of the string content.
    /// </summary>
    public VerbatimStringType StringType { get; }

    /// <summary>
    /// Gets value as byte array.
    /// </summary>
    public ImmutableArray<byte> Value { get; }

    /// <summary>
    /// Gets string value from <see cref="Value"/>.
    /// </summary>
    /// <remarks>See https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding.getstring?view=net-8.0#system-text-encoding-getstring(system-byte()).</remarks>
    /// <exception cref="ArgumentException">The byte array contains invalid Unicode code points.</exception>
    /// <exception cref="DecoderFallbackException">A fallback occurred.</exception>
    public string StringValue => RESPNames.DefaultEncoding.GetString(this.Value.ToArray());

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
