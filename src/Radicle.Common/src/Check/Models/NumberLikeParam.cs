namespace Radicle.Common.Check.Models;

using System;
using System.Collections.Generic;
using Radicle.Common.Check.Models.Generic;

/// <summary>
/// Structure of numeric value parameter.
/// </summary>
/// <typeparam name="TValue">Type of the numerical value.</typeparam>
internal readonly struct NumberLikeParam<TValue> : INumberLikeParam<TValue>
        where TValue : struct
{
    private readonly IComparer<TValue> comparer;

    private readonly TValue zero;

    private readonly Func<TValue, bool> hasValue;

    private readonly Func<TValue, bool> isUTC;

    /// <summary>
    /// Initializes a new instance of the <see cref="NumberLikeParam{TValue}"/> struct.
    /// </summary>
    /// <param name="innerParam">The parameter.</param>
    /// <param name="comparer">Comparer for values.</param>
    /// <param name="zero">Zero value.</param>
    /// <param name="hasValue">Value detector.</param>
    /// <param name="isUTC">UTC detector.</param>
    public NumberLikeParam(
            IParam<TValue> innerParam,
            IComparer<TValue> comparer,
            TValue zero,
            Func<TValue, bool> hasValue,
            Func<TValue, bool> isUTC)
    {
        this.InnerParam = innerParam;
        this.comparer = comparer;
        this.zero = zero;
        this.hasValue = hasValue;
        this.isUTC = isUTC;
    }

    /// <inheritdoc/>
    public IParam<TValue> InnerParam { get; }

    /// <inheritdoc/>
    public INumberLikeParam<TValue> GreaterThan(TValue minimum)
    {
        if (this.InnerParam.IsSpecified)
        {
            TValue val = this.InnerParam.Value;

            if (this.comparer.Compare(val, minimum) <= 0)
            {
                throw new ArgumentOutOfRangeException(
                        this.InnerParam.Name,
                        this.InnerParam.Value,
                        $"{this.InnerParam.DescriptionWithValue} must be greater than {minimum}.");
            }
        }

        return this;
    }

    /// <inheritdoc/>
    public INumberLikeParam<TValue> GreaterThanOrEqual(TValue minimum)
    {
        if (this.InnerParam.IsSpecified)
        {
            TValue val = this.InnerParam.Value;

            if (this.comparer.Compare(val, minimum) < 0)
            {
                throw new ArgumentOutOfRangeException(
                        this.InnerParam.Name,
                        this.InnerParam.Value,
                        $"{this.InnerParam.DescriptionWithValue} must be greater or equal to {minimum}.");
            }
        }

        return this;
    }

    /// <inheritdoc/>
    public INumberLikeParam<TValue> LowerThan(TValue maximum)
    {
        if (this.InnerParam.IsSpecified)
        {
            TValue val = this.InnerParam.Value;

            if (this.comparer.Compare(val, maximum) >= 0)
            {
                throw new ArgumentOutOfRangeException(
                        this.InnerParam.Name,
                        this.InnerParam.Value,
                        $"{this.InnerParam.DescriptionWithValue} must be lesser than {maximum}.");
            }
        }

        return this;
    }

    /// <inheritdoc/>
    public INumberLikeParam<TValue> LowerThenOrEqual(TValue maximum)
    {
        return this.LowerThanOrEqual(maximum);
    }

    /// <inheritdoc/>
    public INumberLikeParam<TValue> LowerThanOrEqual(TValue maximum)
    {
        if (this.InnerParam.IsSpecified)
        {
            TValue val = this.InnerParam.Value;

            if (this.comparer.Compare(val, maximum) > 0)
            {
                throw new ArgumentOutOfRangeException(
                        this.InnerParam.Name,
                        this.InnerParam.Value,
                        $"{this.InnerParam.DescriptionWithValue} must be lesser or equal to {maximum}.");
            }
        }

        return this;
    }

    /// <inheritdoc/>
    public INumberLikeParam<TValue> NonNegative()
    {
        if (this.InnerParam.IsSpecified)
        {
            TValue val = this.InnerParam.Value;

            if (this.comparer.Compare(val, this.zero) < 0)
            {
                throw new ArgumentOutOfRangeException(
                        this.InnerParam.Name,
                        this.InnerParam.Value,
                        $"{this.InnerParam.DescriptionWithValue} can not be negative.");
            }
        }

        return this;
    }

    /// <inheritdoc/>
    public INumberLikeParam<TValue> StrictlyPositive()
    {
        if (this.InnerParam.IsSpecified)
        {
            TValue val = this.InnerParam.Value;

            if (this.comparer.Compare(val, this.zero) <= 0)
            {
                throw new ArgumentOutOfRangeException(
                        this.InnerParam.Name,
                        this.InnerParam.Value,
                        $"{this.InnerParam.DescriptionWithValue} must be strictly positive (greater than zero).");
            }
        }

        return this;
    }

    /// <inheritdoc/>
    public INumberLikeParam<TValue> NonZero()
    {
        if (this.InnerParam.IsSpecified)
        {
            TValue val = this.InnerParam.Value;

            if (this.comparer.Compare(val, this.zero) == 0)
            {
                throw new ArgumentOutOfRangeException(
                        this.InnerParam.Name,
                        this.InnerParam.Value,
                        $"{this.InnerParam.DescriptionWithValue} can not be zero.");
            }
        }

        return this;
    }

    /// <inheritdoc/>
    public INumberLikeParam<TValue> HasActualValue()
    {
        if (this.InnerParam.IsSpecified)
        {
            TValue val = this.InnerParam.Value;

            if (!this.hasValue(val))
            {
                //// float, double
                throw new ArgumentException(
                        $"{this.InnerParam.DescriptionWithValue} has to be an actual number.",
                        this.InnerParam.Name);
            }
        }

        return this;
    }

    /// <inheritdoc/>
    public INumberLikeParam<TValue> InRange(
            TValue lowerBound,
            TValue upperBound,
            bool includeLower = true,
            bool includeUpper = true)
    {
        if (this.InnerParam.IsSpecified)
        {
            TValue val = this.InnerParam.Value;

            int lc = this.comparer.Compare(val, lowerBound);
            int uc = this.comparer.Compare(upperBound, val);

            if ((includeLower ? lc < 0 : lc <= 0) || (includeUpper ? uc < 0 : uc <= 0))
            {
                string range = Dump.Range(
                        lowerBound,
                        upperBound,
                        includeLower,
                        includeUpper);

                throw new ArgumentOutOfRangeException(
                        this.InnerParam.Name,
                        this.InnerParam.Value,
                        $"{this.InnerParam.DescriptionWithValue} must be in range {range}.");
            }
        }

        return this;
    }

    /// <inheritdoc/>
    public INumberLikeParam<TValue> IsUTC()
    {
        if (this.InnerParam.IsSpecified)
        {
            TValue val = this.InnerParam.Value;

            if (!this.isUTC(val))
            {
                throw new ArgumentException(
                        $"{this.InnerParam.DescriptionWithValue} is not a UTC time.",
                        this.InnerParam.Name);
            }
        }

        return this;
    }
}
