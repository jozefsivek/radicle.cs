namespace Radicle.Alike.Redis.Models;

using System;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of RESP null value.
/// </summary>
public sealed class RESPNull : RESPValue
{
    /// <summary>
    /// Gets human readable name.
    /// </summary>
    public const string HumanName = "null";

    /// <summary>
    /// Instance of this class.
    /// </summary>
    public static readonly RESPNull Instance = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPNull"/> class.
    /// </summary>
    public RESPNull()
    {
    }

    /// <inheritdoc/>
    public override RESPDataType Type => RESPDataType.Null;

    /// <inheritdoc/>
    public override TResult Accept<TResult>(IRESPValueVisitor<TResult> visitor)
    {
        Ensure.Param(visitor).Done();

        return visitor.Visit(this);
    }

    /// <inheritdoc/>
    public override bool Equals(RESPValue? other)
    {
        return other is RESPNull && other.Attribs == this.Attribs;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.Attribs);
    }
}
