namespace Radicle.Common.Visual.Models;

using System;
using Radicle.Common.Check;

/// <summary>
/// Immutable struct representing a single line in the label.
/// </summary>
public readonly struct LabelLine : IEquatable<LabelLine>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LabelLine"/> struct.
    /// </summary>
    /// <param name="line">Line string, can be empty.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="line"/>
    ///     contains new lines.</exception>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public LabelLine(string line)
    {
        this.Value = Ensure.Param(line).NoNewLines().Value;
    }

    /// <summary>
    /// Gets value of the line.
    /// </summary>
    public string Value { get; } = string.Empty;

    /// <summary>
    /// Creates instance from string value.
    /// </summary>
    /// <param name="line">Line string, can be empty.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="line"/>
    ///     contains new lines.</exception>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <returns>Instance of <see cref="LabelLine"/>.</returns>
    public static explicit operator LabelLine(string line)
    {
        return FromString(line);
    }

    /// <summary>
    /// Creates instance from string value.
    /// </summary>
    /// <param name="line">Line string, can be empty.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="line"/>
    ///     contains new lines.</exception>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <returns>Instance of <see cref="LabelLine"/>.</returns>
    public static implicit operator string(LabelLine line)
    {
        return line.Value;
    }

    /// <summary>
    /// Compare two instances of <see cref="LabelLine"/>
    /// in ordinary case sensitive fashion for equality.
    /// </summary>
    /// <param name="left">Left.</param>
    /// <param name="right">Right.</param>
    /// <returns>Result of comparison.</returns>
    public static bool operator ==(LabelLine left, LabelLine right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Compare two instances of <see cref="LabelLine"/>
    /// in ordinary case sensitive fashion for inequality.
    /// </summary>
    /// <param name="left">Left.</param>
    /// <param name="right">Right.</param>
    /// <returns>Result of comparison.</returns>
    public static bool operator !=(LabelLine left, LabelLine right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Creates instance from string value.
    /// </summary>
    /// <param name="line">Line string, can be empty.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="line"/>
    ///     contains new lines.</exception>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <returns>Instance of <see cref="LabelLine"/>.</returns>
    public static LabelLine FromString(string line)
    {
        return new LabelLine(line);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return this.Value;
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        return obj is LabelLine ll && this.Equals(ll);
    }

    /// <inheritdoc/>
    public bool Equals(LabelLine other)
    {
        return this.Value == other.Value;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return this.Value.GetHashCode(StringComparison.Ordinal);
    }
}
