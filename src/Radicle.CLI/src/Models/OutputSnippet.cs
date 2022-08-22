namespace Radicle.CLI.Models;

using System;
using Radicle.Common.Check;

/// <summary>
/// Immutable model of elementary output snippet, ususally part of line.
/// </summary>
public sealed class OutputSnippet
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OutputSnippet"/> class.
    /// </summary>
    /// <param name="style">Style of the <paramref name="value"/>.</param>
    /// <param name="value">Text new line free value of the snippet.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="value"/> contains new lines.</exception>
    public OutputSnippet(
            OutputStyle style,
            string value)
    {
        this.Style = style;
        this.Value = Ensure.Param(value).NoNewLines().Value;
    }

    /// <summary>
    /// Gets style of the snippet.
    /// </summary>
    public OutputStyle Style { get; }

    /// <summary>
    /// Gets value of the snippet, can be empty string.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Implicit conversion of raw string to snippet.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="value"/> contains new lines.</exception>
    public static implicit operator OutputSnippet(string value)
    {
        return ToOutputSnippet(value);
    }

    /// <summary>
    /// Conversion of raw string to snippet.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="value"/> contains new lines.</exception>
    /// <returns>Instance of <see cref="OutputSnippet"/>.</returns>
    public static OutputSnippet ToOutputSnippet(string value)
    {
        return new OutputSnippet(OutputStyle.Normal, value);
    }
}
