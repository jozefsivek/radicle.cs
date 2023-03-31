namespace Radicle.Alike.Redis.Models;

using System;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of RESP boolean value.
/// </summary>
public sealed class RESPBoolean : RESPValue
{
    /// <summary>
    /// Gets human redable name.
    /// </summary>
    public const string HumanName = "boolean";

    /// <summary>
    /// Gets true value.
    /// </summary>
    public static readonly RESPBoolean True = new(true);

    /// <summary>
    /// Gets false value.
    /// </summary>
    public static readonly RESPBoolean False = new(false);

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPBoolean"/> class.
    /// </summary>
    /// <param name="value">Boolean value.</param>
    public RESPBoolean(bool value)
    {
        this.Value = value;
    }

    /// <inheritdoc/>
    public override RESPDataType Type => RESPDataType.Boolean;

    /// <summary>
    /// Gets a value indicating whether this RESP value is true or false.
    /// </summary>
    public bool Value { get; }

    /// <inheritdoc/>
    public override TResult Accept<TResult>(IRESPValueVisitor<TResult> visitor)
    {
        Ensure.Param(visitor).Done();

        return visitor.Visit(this);
    }

    /// <inheritdoc/>
    public override bool Equals(RESPValue? other)
    {
        return other is RESPBoolean boolean
                && boolean.Value == this.Value
                && other.Attribs == this.Attribs;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.Value, this.Attribs);
    }
}
