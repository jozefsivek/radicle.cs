namespace Radicle.Common.Check.Models;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Radicle.Common.Check.Models.Generic;

/// <summary>
/// Structure of string value parameter.
/// </summary>
internal readonly struct StringParam : IStringParam
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringParam"/> struct.
    /// </summary>
    /// <param name="innerParam">The parameter.</param>
    public StringParam(IParam<string> innerParam)
    {
        this.InnerParam = innerParam;
    }

    /// <inheritdoc/>
    public IParam<string> InnerParam { get; }

    /// <inheritdoc/>
    public IStringParam NotEmpty()
    {
        if (
                this.InnerParam.IsSpecified
                && this.InnerParam.Value.Length == 0)
        {
            throw new ArgumentOutOfRangeException(
                    this.InnerParam.Name,
                    this.InnerParam.Value,
                    $"{this.InnerParam.Description} cannot be an empty string.");
        }

        return this;
    }

    /// <inheritdoc/>
    public IStringParam NotWhiteSpace()
    {
        if (this.InnerParam.IsSpecified)
        {
            _ = this.NotEmpty();

            if (string.IsNullOrWhiteSpace(this.InnerParam.Value))
            {
                throw new ArgumentException(
                        $"{this.InnerParam.DescriptionWithValue} cannot be a white space string.",
                        this.InnerParam.Name);
            }
        }

        return this;
    }

    /// <inheritdoc/>
    public IStringParam NoNewLines()
    {
        if (this.InnerParam.IsSpecified
                && this.InnerParam.Value.Length != 0
                && !TypedNameSpec.SingleLine.IsValid(this.InnerParam.Value))
        {
            throw new ArgumentException(
                    $"{this.InnerParam.DescriptionWithValue} cannot be a string with new lines.",
                    this.InnerParam.Name);
        }

        return this;
    }

    /// <inheritdoc/>
    public IStringParam InRange(
            int lowerBound,
            int upperBound,
            bool includeLower = true,
            bool includeUpper = true)
    {
        if (this.InnerParam.IsSpecified)
        {
            int l = this.InnerParam.Value.Length;

            if (
                (includeLower ? l < lowerBound : l <= lowerBound)
                || (includeUpper ? upperBound < l : upperBound <= l))
            {
                string range = Dump.Range(lowerBound, upperBound, includeLower, includeUpper);

                throw new ArgumentOutOfRangeException(
                        this.InnerParam.Name,
                        this.InnerParam.Value,
                        $"{this.InnerParam.DescriptionWithValue} length must be in range {range}");
            }
        }

        return this;
    }

    /// <inheritdoc/>
    public IStringParam IsRegex()
    {
        if (this.InnerParam.IsSpecified)
        {
            // try to construct regular expression
            try
            {
                Regex r = new(this.InnerParam.Value);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(
                        $"{this.InnerParam.DescriptionWithValue} must be a valid regex.",
                        this.InnerParam.Name,
                        ex);
            }
        }

        return this;
    }

    /// <inheritdoc/>
    public IStringParam Conforms(ITypedNameSpec specification)
    {
        Ensure.NotNull(specification);

        if (this.InnerParam.IsSpecified)
        {
            _ = specification.EnsureValid(
                    this.InnerParam.Value!,
                    parameterName: this.InnerParam.Name);
        }

        return this;
    }

    /// <inheritdoc/>
    public IEnumerator<char> GetEnumerator()
    {
        IEnumerable<char> e = this.InnerParam.IsSpecified
                ? this.InnerParam.Value
                : Array.Empty<char>();

        return e.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
