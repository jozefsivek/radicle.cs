namespace Radicle.Common.Text.Base;

using System;

/// <summary>
/// Base implementation of <see cref="IShellExpander"/>.
/// </summary>
public abstract class ShellExpander : IShellExpander
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShellExpander"/> class.
    /// </summary>
    /// <param name="previous">Optional previous expander.</param>
    protected ShellExpander(IShellExpander? previous = null)
    {
        this.Previous = previous;
    }

    /// <inheritdoc />
    public IShellExpander? Previous { get; }

    /// <inheritdoc />
    public abstract string Summary { get; }

    /// <inheritdoc />
    public string Expand(string value)
    {
        string expanded = this.Previous?.Expand(value) ?? value;

        return this.ExpandThis(expanded);
    }

    /// <summary>
    /// Perform only this class expansion.
    /// The <see cref="Previous"/> <see cref="IShellExpander.Expand(string)"/>
    /// is call automatically by <see cref="Expand(string)"/> before
    /// and its result is passed to <paramref name="value"/>.
    /// </summary>
    /// <param name="value">Value to expand.</param>
    /// <returns>Expanded value from the <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if the <paramref name="value"/> exceeds supported
    ///     length.</exception>
    protected abstract string ExpandThis(string value);
}
