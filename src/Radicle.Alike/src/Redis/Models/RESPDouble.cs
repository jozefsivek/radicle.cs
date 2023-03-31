namespace Radicle.Alike.Redis.Models;

using System;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of RESP double, floating point number.
/// </summary>
public sealed class RESPDouble : RESPValue
{
    /// <summary>
    /// Gets Human readable name.
    /// </summary>
    public const string HumanName = "double";

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPDouble"/> class.
    /// </summary>
    /// <param name="value">Floating point number with
    ///     allowed NaN or infinity. Note unlike IEEE 754
    ///     ( https://en.wikipedia.org/wiki/IEEE_754 ) two NaN
    ///     values are treated as equal because this model describes
    ///     protocol values which are, if NaN, indistiguishable.</param>
    public RESPDouble(double value)
    {
        this.Value = value;
    }

    /// <inheritdoc/>
    public override RESPDataType Type => RESPDataType.FloatingPointNumber;

    /// <summary>
    /// Gets double number value.
    /// </summary>
    public double Value { get; }

    /// <inheritdoc/>
    public override TResult Accept<TResult>(IRESPValueVisitor<TResult> visitor)
    {
        Ensure.Param(visitor).Done();

        return visitor.Visit(this);
    }

    /// <inheritdoc/>
    public override bool Equals(RESPValue? other)
    {
        return other is RESPDouble number
                && (number.Value == this.Value
                    || (double.IsNaN(number.Value) && double.IsNaN(this.Value)))
                && other.Attribs == this.Attribs;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.Value, this.Attribs);
    }
}
