namespace Radicle.Common.Statistics.Models;

using System;
using System.Globalization;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of user value
/// with given weight.
/// </summary>
/// <typeparam name="TValue">Type of the value.</typeparam>
public sealed class WeightedValue<TValue>
    where TValue : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WeightedValue{TValue}"/> class.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <param name="weight">Weight associated with <paramref name="value"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="weight"/> is negative or zero.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="weight"/> is not a number
    ///     with finite of defined value.</exception>
    public WeightedValue(TValue value, double weight)
    {
        this.Value = Ensure.Param(value).Value;
        this.Weight = Ensure.Param(weight).HasActualValue().StrictlyPositive().Value;
    }

    /// <summary>
    /// Gets user defined value.
    /// </summary>
    public TValue Value { get; }

    /// <summary>
    /// Gets non negative weight associated with
    /// the <see cref="Value"/>.
    /// </summary>
    public double Weight { get; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{this.Value} <{this.Weight.ToString(CultureInfo.InvariantCulture)}>";
    }
}
