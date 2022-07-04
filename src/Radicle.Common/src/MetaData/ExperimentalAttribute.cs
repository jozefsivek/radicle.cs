namespace Radicle.Common.MetaData;

using System;
using Radicle.Common.Check;

/// <summary>
/// Attibute marking API as experimental. Such API
/// is not stable and can be changed abruptly.
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public sealed class ExperimentalAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExperimentalAttribute"/> class.
    /// </summary>
    /// <param name="message">Message explaining the reason.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="message"/> is white space only.</exception>
    public ExperimentalAttribute(string message)
    {
        this.Message = Ensure.Param(message).NotWhiteSpace().Value;
    }

    /// <summary>
    /// Gets human readable message explaining the reason.
    /// </summary>
    public string? Message { get; }
}
