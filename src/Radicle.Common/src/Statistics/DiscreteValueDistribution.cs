namespace Radicle.Common.Statistics;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Radicle.Common.Check;
using Radicle.Common.Statistics.Models;

/// <summary>
/// Immutable implementation of inverse transform sampling
/// ( https://en.m.wikipedia.org/wiki/Inverse_transform_sampling )
/// for discrete weighted values.
/// </summary>
/// <typeparam name="TValue">Type of the discrete value.</typeparam>
/// <remarks>
/// <para>The distribution is given as (histogram of discrete values):</para>
/// <code>
///       A                     *
///       |                     *
/// weigt |      *     *     *  *     *
///       |*  *  *  *  *  *  *  *  *  *  *
///       +-------------------------------->
///        v1,v2, ...           ^        vn
///                   discrete values
/// </code>
/// <para>From this histogram a random series of values can be
/// produced following the given distribution. This is done
/// by constructing the cumulative distribution function
/// from the provided weights:</para>
/// <code>
///       A
///   1.0)|                              * }- bin for vn value
///       |                           *  *
///       |                           *  *
///       |                        *  *  *
///       |                     *  *  *  * \
///       |                     *  *  *  *  |_ bin for ^ value
///       |                     *  *  *  *  |
///       |                     *  *  *  * /
///       |                  *  *  *  *  *
///       |                  *  *  *  *  *
///       |               *  *  *  *  *  *
///       |            *  *  *  *  *  *  *
///       |            *  *  *  *  *  *  *
///       |         *  *  *  *  *  *  *  *
///       |      *  *  *  *  *  *  *  *  *
///       |      *  *  *  *  *  *  *  *  *
///       |   *  *  *  *  *  *  *  *  *  *
///       |*  *  *  *  *  *  *  *  *  *  * }- bin for v1 value
///  [0.0 +-------------------------------->
///        v1,v2, ...           ^        vn
///                 discrete values
/// </code>
/// <para>
/// Where on y axis we can generate uniformly
/// distributed random values and map them to discrete
/// user defined values (v1, v2, ...)
/// </para>
/// </remarks>
public class DiscreteValueDistribution<TValue> : IEnumerable<WeightedValue<TValue>>
    where TValue : notnull
{
    private readonly ImmutableArray<(DoubleInterval Bin, WeightedValue<TValue> Val)> values;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteValueDistribution{TValue}"/> class.
    /// </summary>
    /// <param name="values">Discrete values,
    ///     in principle they can repeat,
    ///     this instance will not impose restriction on that.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="values"/> is empty collection.</exception>
    public DiscreteValueDistribution(IEnumerable<WeightedValue<TValue>> values)
    {
        WeightedValue<TValue>[] orderedWeights = Ensure.Param(values)
                .NotEmpty()
                .AllNotNull()
                .OrderByDescending(w => w.Weight)
                .ToArray();

        double weightSum = orderedWeights.Sum(w => w.Weight);
        double runningSum = 0.0;

        ImmutableArray<(DoubleInterval Bin, WeightedValue<TValue> Val)>.Builder builder =
                ImmutableArray
                    .CreateBuilder<(DoubleInterval Bin, WeightedValue<TValue> Val)>();

        // normalize intervals to fit within [0, 1)
        for (int i = 0; i < orderedWeights.Length; i++)
        {
            WeightedValue<TValue> w = orderedWeights[i];
            double lower = runningSum;
            runningSum += w.Weight / weightSum;
            double upper = (i == (orderedWeights.Length - 1))
                    ? 1.0
                    : runningSum;

            builder.Add((new DoubleInterval(lower, upper, includeUpper: false), w));
        }

        this.values = builder.ToImmutable();
    }

    /// <summary>
    /// Gets amount of values stored in this instance.
    /// </summary>
    public int Count => this.values.Length;

    /// <summary>
    /// Try to create new instance of
    /// <see cref="DiscreteValueDistribution{TValue}"/>
    /// by removing first occurence of <paramref name="value"/>
    /// (by reference).
    /// </summary>
    /// <param name="value">Value to remove.</param>
    /// <param name="distribution">New distribution or
    ///     this distribution if this instance does not
    ///     contain <paramref name="value"/>.</param>
    /// <returns><see langword="true"/> if the new instance can be created;
    ///     <see langword="false"/> if this instance contains only
    ///     <paramref name="value"/> and removal will create
    ///     invalid distribution.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if requered parameter is <see langword="null"/>.</exception>
    public bool TryRemove(
            WeightedValue<TValue> value,
            [NotNullWhen(returnValue: true)] out DiscreteValueDistribution<TValue>? distribution)
    {
        Ensure.Param(value).Done();

        distribution = default;
        bool check = true;
        List<WeightedValue<TValue>> newValues = new(this.Count);

        foreach ((_, WeightedValue<TValue> w) in this.values)
        {
            if (check && ReferenceEquals(w, value))
            {
                check = false;
                continue;
            }

            newValues.Add(w);
        }

        if (newValues.Count <= 1)
        {
            return false;
        }

        if (check)
        {
            distribution = this;
        }
        else
        {
            distribution = new DiscreteValueDistribution<TValue>(newValues);
        }

        return true;
    }

    /// <summary>
    /// Determine if given <paramref name="value"/>
    /// is in this instance (by reference).
    /// </summary>
    /// <param name="value">Value to search for.</param>
    /// <returns><see langword="true"/> if found;
    ///     <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public bool Contains(WeightedValue<TValue> value)
    {
        Ensure.Param(value).Done();

        return this.values.Any(i => ReferenceEquals(i.Val, value));
    }

    /// <inheritdoc/>
    public IEnumerator<WeightedValue<TValue>> GetEnumerator()
    {
        foreach ((_, WeightedValue<TValue> val) in this.values)
        {
            yield return val;
        }
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    /// <summary>
    /// Gets next value following this instacnce distribution.
    /// </summary>
    /// <returns>One value.</returns>
    public WeightedValue<TValue> Next()
    {
        double rnd = ThreadSafeRandom.NextDouble();

        if (this.values.Length == 1)
        {
            return this.values[0].Val;
        }

        return this.values.First(i => i.Bin.Contains(rnd)).Val;
    }
}
