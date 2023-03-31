﻿namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of the RESP simple string.
/// </summary>
public sealed class RESPSimpleString : RESPValue
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
    {
        Ensure.Param(stringValue).NoNewLines().Done();

        this.Value = RESPNames.DefaultEncoding.GetBytes(stringValue)
                .ToImmutableArray();
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
    {
        this.Value = Ensure.Param(value)
                .AllNotNull(b => Ensure.Param(b)
                    .That(b => !RESPNames.AllNewLineBytes.Contains(b)).Done())
                .ToImmutableArray();
    }

    /// <inheritdoc/>
    public override RESPDataType Type => RESPDataType.SimpleString;

    /// <summary>
    /// Gets value as byte array (by definition does not contain new lines).
    /// </summary>
    public ImmutableArray<byte> Value { get; }

    /// <summary>
    /// Gets string value from <see cref="Value"/>.
    /// </summary>
    /// <exception cref="ArgumentException">The byte array contains invalid Unicode code points.</exception>
    /// <exception cref="DecoderFallbackException">A fallback occurred.</exception>
    public string StringValue => RESPNames.DefaultEncoding.GetString(this.Value.ToArray());

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
