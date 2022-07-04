namespace Radicle.Common.Check.Models;

using System;
using System.Globalization;
using Radicle.Common.Check.Models.Generic;

/// <summary>
/// Interface or string method parameter.
/// </summary>
public interface ICultureInfoParam : IParam<CultureInfo>
{
    /// <summary>
    /// Throws if the paramter value is <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    /// <returns>This instance of <see cref="ICultureInfoParam"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when the argument
    ///      value is <see cref="CultureInfo.InvariantCulture"/>.</exception>
    ICultureInfoParam NotInvariant();
}
