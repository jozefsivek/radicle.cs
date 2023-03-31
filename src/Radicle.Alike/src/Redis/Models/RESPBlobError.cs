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
public sealed class RESPBlobError : RESPValue
{
    /// <summary>
    /// Gets human readable name.
    /// </summary>
    public const string HumanName = "blob error";

    /// <summary>
    /// Gets syntax error code, this code is first word in the error value.
    /// </summary>
    public const string SyntaxErrorCode = "SYNTAX";

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPBlobError"/> class.
    /// </summary>
    /// <param name="stringValue">Value of this blob error
    ///     which will be serialized in UTF-8 encoding.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public RESPBlobError(string stringValue)
    {
        Ensure.Param(stringValue).Done();

        this.Value = RESPNames.DefaultEncoding.GetBytes(stringValue)
                .ToImmutableArray();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPBlobError"/> class.
    /// </summary>
    /// <param name="value">Binary value of this error.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public RESPBlobError(IEnumerable<byte> value)
    {
        this.Value = Ensure.Param(value).ToImmutableArray();
    }

    /// <inheritdoc/>
    public override RESPDataType Type => RESPDataType.BlobError;

    /// <summary>
    /// Gets value as byte array.
    /// </summary>
    public ImmutableArray<byte> Value { get; }

    /// <summary>
    /// Gets string value from <see cref="Value"/>.
    /// </summary>
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

    /// <inheritdoc/>
    public override TResult Accept<TResult>(IRESPValueVisitor<TResult> visitor)
    {
        Ensure.Param(visitor).Done();

        return visitor.Visit(this);
    }

    /// <inheritdoc/>
    public override bool Equals(RESPValue? other)
    {
        return other is RESPBlobError blobError
                && this.Value.SequenceEqual(blobError.Value)
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
