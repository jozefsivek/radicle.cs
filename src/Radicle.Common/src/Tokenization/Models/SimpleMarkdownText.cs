namespace Radicle.Common.Tokenization.Models;

using System;
using System.Collections.Generic;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of simple markdown text.
/// This currently expects this limited text formatting:
/// '*' surrounded text for bold, '_' for emphasised.
/// </summary>
public class SimpleMarkdownText
{
    /// <summary>
    /// Stop stop word for bold formatting.
    /// </summary>
    public static readonly ParsedTokenStopWord BoldStopWord = new('*');

    /// <summary>
    /// Stop stop word for emphasised formatting.
    /// </summary>
    public static readonly ParsedTokenStopWord EmphasisedStopWord = new('_');

    /// <summary>
    /// Escape stop word.
    /// </summary>
    public static readonly ParsedTokenStopWord EscapeStopWord = new('\\');

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleMarkdownText"/> class.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public SimpleMarkdownText(string value)
    {
        this.Value = Ensure.Param(value).Value;
    }

    /// <summary>
    /// Gets value of this formatted text string.
    /// May be empty.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Implicitly convert <see cref="string"/>
    /// to instance of <see cref="SimpleMarkdownText"/>.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static implicit operator SimpleMarkdownText(string value)
    {
        return new SimpleMarkdownText(value);
    }

    /// <summary>
    /// Implicitly convert <see cref="string"/>
    /// to instance of <see cref="SimpleMarkdownText"/>.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <returns>Instance of <see cref="SimpleMarkdownText"/>.</returns>
    public static SimpleMarkdownText FromString(string value)
    {
        return new SimpleMarkdownText(value);
    }

    /// <summary>
    /// Implicitly convert <see cref="string"/>
    /// to instance of <see cref="SimpleMarkdownText"/>.
    /// </summary>
    /// <param name="values">Values to convert.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <returns>Instance of <see cref="SimpleMarkdownText"/>.</returns>
    public static IEnumerable<SimpleMarkdownText> FromStrings(
            IEnumerable<string> values)
    {
        foreach (string md in Ensure.Param(values).AllNotNull())
        {
            yield return new SimpleMarkdownText(md);
        }
    }

    /// <summary>
    /// Sanitize the <paramref name="input"/>
    /// to be displayed in the markdown text as is.
    /// </summary>
    /// <param name="input">Input to sanitize.</param>
    /// <returns>Sanitized text.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static SimpleMarkdownText Sanitize(string input)
    {
        string esc = EscapeStopWord.Value;

        return Ensure.Param(input).Value
            .Replace(esc, esc + esc, StringComparison.Ordinal)
            .Replace(EmphasisedStopWord.Value, esc + EmphasisedStopWord.Value, StringComparison.Ordinal)
            .Replace(BoldStopWord.Value, esc + BoldStopWord.Value, StringComparison.Ordinal);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return this.Value;
    }
}
