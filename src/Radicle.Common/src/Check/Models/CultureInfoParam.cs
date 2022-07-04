namespace Radicle.Common.Check.Models;
using System;
using System.Globalization;
using Radicle.Common.Check.Models.Generic;

/// <summary>
/// Structure of culture info value parameter.
/// </summary>
internal readonly struct CultureInfoParam : ICultureInfoParam
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CultureInfoParam"/> struct.
    /// </summary>
    /// <param name="innerParam">The parameter.</param>
    public CultureInfoParam(IParam<CultureInfo> innerParam)
    {
        this.InnerParam = innerParam;
    }

    /// <inheritdoc/>
    public IParam<CultureInfo> InnerParam { get; }

    /// <inheritdoc/>
    public ICultureInfoParam NotInvariant()
    {
        if (
                this.InnerParam.IsSpecified
                && CultureInfo.InvariantCulture.Equals(this.InnerParam.Value))
        {
            throw new ArgumentException(
                    $"{this.InnerParam.Description} cannot be invariant culture.",
                    this.InnerParam.Name);
        }

        return this;
    }
}
