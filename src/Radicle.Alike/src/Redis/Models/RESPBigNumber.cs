namespace Radicle.Alike.Redis.Models;

using System;
using System.Numerics;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of RESP big number,
/// a signed integer above 64 bit range.
/// </summary>
public sealed class RESPBigNumber : RESPValue
{
    /// <summary>
    /// Gets human readable name.
    /// </summary>
    public const string HumanName = "big integer";

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPBigNumber"/> class.
    /// </summary>
    /// <param name="value">Number value, 64 bit signed integer.</param>
    public RESPBigNumber(BigInteger value)
    {
        this.Value = value;
    }

    /// <inheritdoc/>
    public override RESPDataType Type => RESPDataType.BigNumber;

    /// <summary>
    /// Gets integer number value.
    /// </summary>
    public BigInteger Value { get; }

    /// <inheritdoc/>
    public override bool Equals(RESPValue? other)
    {
        return other is RESPBigNumber number
                && number.Value == this.Value
                && other.Attribs == this.Attribs;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.Value, this.Attribs);
    }

    /// <inheritdoc/>
    public override TResult Accept<TResult>(IRESPValueVisitor<TResult> visitor)
    {
        Ensure.Param(visitor).Done();

        return visitor.Visit(this);
    }
}
