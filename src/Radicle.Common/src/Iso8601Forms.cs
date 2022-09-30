namespace Radicle.Common;

using System;

/// <summary>
/// Flags of date and time forms in ISO 8601:2004.
/// </summary>
[Flags]
public enum Iso8601Forms
{
    /// <summary>
    /// None.
    /// </summary>
    None = 0,

    /// <summary>
    /// Dash separated parts: YYYY-MM-DD.
    /// </summary>
    Extended = 1,

    /// <summary>
    /// No parts separation: YYYYMMDD.
    /// </summary>
    Basic = 1 << 1,

    /// <summary>
    /// All forms.
    /// </summary>
    All = Extended | Basic,
}
