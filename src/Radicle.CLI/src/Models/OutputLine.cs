namespace Radicle.CLI.Models;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Radicle.Common;
using Radicle.Common.Check;

/// <summary>
/// Immutable model of printable output line
/// ususally printed on some form of console.
/// </summary>
public sealed class OutputLine : IEnumerable<OutputSnippet>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OutputLine"/> class.
    /// </summary>
    /// <param name="snippets">Snippets.</param>
    /// <param name="separator">Seprator preference, defaults to none.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public OutputLine(
            IEnumerable<OutputSnippet> snippets,
            OutputSnippetSeparator separator = OutputSnippetSeparator.Empty)
    {
        this.Snippets = Ensure.Param(snippets)
                .AllNotNull()
                .ToImmutableArray();
        this.Separator = separator;
    }

    /// <summary>
    /// Gets snippets this line is made of.
    /// </summary>
    public ImmutableArray<OutputSnippet> Snippets { get; }

    /// <summary>
    /// Gets a value indicating what separator should be used.
    /// </summary>
    public OutputSnippetSeparator Separator { get; }

    /// <summary>
    /// Implicitly convert raw string to line as a single snippet.
    /// </summary>
    /// <param name="snippetString">Raw string.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="snippetString"/> contains new lines.</exception>
    public static implicit operator OutputLine(string snippetString)
    {
        return ToOutputLine(snippetString);
    }

    /// <summary>
    /// Convert raw string to line.
    /// </summary>
    /// <param name="snippetString">Raw string.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="snippetString"/> contains new lines.</exception>
    /// <returns>Instance of <see cref="OutputLine"/>.</returns>
    public static OutputLine ToOutputLine(string snippetString)
    {
        return new OutputLine(new[] { (OutputSnippet)snippetString });
    }

    /// <summary>
    /// Convert raw strings to line.
    /// </summary>
    /// <param name="snippetStrings">Raw strings.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="snippetStrings"/> contains new lines.</exception>
    /// <returns>Instance of <see cref="OutputLine"/>.</returns>
    public static IEnumerable<OutputLine> ToOutputLines(IEnumerable<string> snippetStrings)
    {
        return Ensure.Param(snippetStrings)
                .AllNotNull()
                .Select(s => new OutputLine(new[] { (OutputSnippet)s }));
    }

    /// <summary>
    /// Convert raw strings to line.
    /// </summary>
    /// <param name="multilineString">Raw strings.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="multilineString"/> contains new lines.</exception>
    /// <returns>Instance of <see cref="OutputLine"/>.</returns>
    public static IEnumerable<OutputLine> ToOutputLines(string multilineString)
    {
        Ensure.Param(multilineString).Done();

        return ToOutputLines(
                multilineString.Split(
                    new[] { "\n", "\r" },
                    StringSplitOptions.RemoveEmptyEntries));
    }

    /// <inheritdoc/>
    public IEnumerator<OutputSnippet> GetEnumerator()
    {
        return ((IEnumerable<OutputSnippet>)this.Snippets).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        StringBuilder sb = new();

        foreach (OutputSnippet snipet in this)
        {
            sb = sb.Append('[')
                    .Append(snipet.Style)
                    .Append("] ")
                    .Append(Dump.Literal(snipet.Value));
        }

        return sb.ToString();
    }
}
