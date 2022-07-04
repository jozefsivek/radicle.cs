namespace Radicle.Common.MetaData;

using System;
using Radicle.Common.Check;

/// <summary>
/// Attribute marking API as internal-use-only. Any changes
/// to those API are not subject to SemVer of the library.
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public sealed class InternalUseOnlyAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InternalUseOnlyAttribute"/> class.
    /// </summary>
    /// <param name="message">Message explaining the reason.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="message"/> is white space only.</exception>
    public InternalUseOnlyAttribute(string message)
    {
        this.Message = Ensure.Param(message).NotWhiteSpace().Value;
    }

    /// <summary>
    /// Gets human readable message explaining the reason.
    /// </summary>
    public string? Message { get; }
}
