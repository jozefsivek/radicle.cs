namespace Radicle.Alike.Redis.Models;

using System;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of RESP number,
/// a signed 64 bit integer.
/// </summary>
public sealed class RESPNumber : RESPValue
{
    /// <summary>
    /// Gets human readable name.
    /// </summary>
    public const string HumanName = "integer";

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPNumber"/> class.
    /// </summary>
    /// <param name="value">Number value, 64 bit signed integer.</param>
    public RESPNumber(long value)
    {
        this.Value = value;
    }

    /// <inheritdoc/>
    public override RESPDataType Type => RESPDataType.Number;

    /// <summary>
    /// Gets integer number value.
    /// </summary>
    public long Value { get; }

    /// <inheritdoc/>
    public override TResult Accept<TResult>(IRESPValueVisitor<TResult> visitor)
    {
        Ensure.Param(visitor).Done();

        return visitor.Visit(this);
    }

    /// <inheritdoc/>
    public override bool Equals(RESPValue? other)
    {
        return other is RESPNumber number
                && number.Value == this.Value
                && other.Attribs == this.Attribs;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.Value, this.Attribs);
    }
}
