namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of RESP blob string.
/// </summary>
public sealed class RESPBlobString : RESPStringLikeValue
{
    /// <summary>
    /// Get human readable name.
    /// </summary>
    public const string HumanName = "blob string";

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPBlobString"/> class.
    /// </summary>
    /// <param name="stringValue">Value of this string
    ///     which will be serialized in UTF-8 encoding.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public RESPBlobString(string stringValue)
        : base(stringValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPBlobString"/> class.
    /// </summary>
    /// <param name="value">Binary value of this string.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public RESPBlobString(IEnumerable<byte> value)
        : base(value)
    {
    }

    /// <inheritdoc/>
    public override RESPDataType Type => RESPDataType.BlobString;

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
    /// to instance of <see cref="RESPBlobString"/>.
    /// </summary>
    /// <param name="stringValue">Value to cast.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static implicit operator RESPBlobString(string stringValue)
    {
        return new RESPBlobString(stringValue);
    }

    /// <summary>
    /// Convert <paramref name="stringValue"/>
    /// to instance of <see cref="RESPBlobString"/>.
    /// </summary>
    /// <param name="stringValue">Value to cast.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <returns>Instance of <see cref="RESPBlobString"/>.</returns>
    public static RESPBlobString FromString(string stringValue)
    {
        return new RESPBlobString(stringValue);
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
        return other is RESPBlobString blobString
                && this.Value.SequenceEqual(blobString.Value)
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
