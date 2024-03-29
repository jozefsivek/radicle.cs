﻿namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of RESP blob string.
/// </summary>
public sealed class RESPBlobError : RESPStringLikeValue
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
        : base(stringValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPBlobError"/> class.
    /// </summary>
    /// <param name="value">Binary value of this error.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public RESPBlobError(IEnumerable<byte> value)
        : base(value)
    {
    }

    /// <inheritdoc/>
    public override RESPDataType Type => RESPDataType.BlobError;

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
    /// to instance of <see cref="RESPBlobError"/>.
    /// </summary>
    /// <param name="stringValue">Value to cast.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static implicit operator RESPBlobError(string stringValue)
    {
        return new RESPBlobError(stringValue);
    }

    /// <summary>
    /// Convert <paramref name="stringValue"/>
    /// to instance of <see cref="RESPBlobError"/>.
    /// </summary>
    /// <param name="stringValue">Value to cast.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <returns>Instance of <see cref="RESPBlobError"/>.</returns>
    public static RESPBlobError FromString(string stringValue)
    {
        return new RESPBlobError(stringValue);
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
