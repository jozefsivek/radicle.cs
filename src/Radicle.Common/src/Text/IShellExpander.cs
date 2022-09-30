namespace Radicle.Common.Text;

using System;

/// <summary>
/// General interface of shell like expander: https://tldp.org/LDP/Bash-Beginners-Guide/html/sect_03_04.html .
/// </summary>
public interface IShellExpander
{
    /// <summary>
    /// Gets linked instance of <see cref="IShellExpander"/>
    /// which is called first to expand values.
    /// </summary>
    IShellExpander? Previous { get; }

    /// <summary>
    /// Gets human readable one line summary of this expander.
    /// </summary>
    string Summary { get; }

    /// <summary>
    /// Expand given <paramref name="value"/>.
    /// Non expandable tokens will be left as they are
    /// and will not cause exceptions.
    /// </summary>
    /// <param name="value">Value to be expanded.</param>
    /// <returns>Expanded value from <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if the <paramref name="value"/> exceeds supported length.</exception>
    string Expand(string value);
}
