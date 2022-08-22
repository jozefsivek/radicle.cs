namespace Radicle.CLI.REPL;

using System;
using System.Collections.Generic;
using Radicle.CLI.Models;

/// <summary>
/// Provider of the styling meta-data for REPL reader and writer.
/// </summary>
/// <remarks>This meta data is read by the reader/writer whenever
/// there is a change in UI. So it is best to provide up to date
/// information whenever values are requested.</remarks>
public interface IREPLStylingProvider
{
    /// <summary>
    /// Gets the prompt string one (prefix) of the input line which is displayed
    /// before the te user accessible input. Think of PS1
    /// (Prompt String One) from bash
    /// https://wiki.archlinux.org/title/Bash/Prompt_customization ,
    /// https://www.gnu.org/savannah-checkouts/gnu/bash/manual/bash.html .
    /// </summary>
    string PromptStringOne { get; }

    /// <summary>
    /// Gets a value indicating whether to use any color in the UI
    /// if this is supported.
    /// </summary>
    bool EnableColor { get; }

    /// <summary>
    /// Gets preferred type of spinner.
    /// </summary>
    SpinnerType SpinnerType { get; }

    /// <summary>
    /// Gets preferred type of progress bars.
    /// </summary>
    ProgressBarsType ProgressBarsType { get; }

    /// <summary>
    /// Return suggestion tail of the given user <paramref name="input"/>.
    /// E.g. for input 'foo' return ' bar ...' to be displayed as:
    /// 'foo bar ...'. This is displayed interactivelly as assisting
    /// visual only suggestion and it is displayed in different style
    /// to diferentiate from user written input.
    /// </summary>
    /// <param name="input">Input to complete.</param>
    /// <returns>Completion tail of the <paramref name="input"/>
    ///     or empty string. Do not return <see langword="null"/>
    ///     return empty string to avoid completion.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    string GetSuggestionTail(string input);

    /// <summary>
    /// Returns collection of auto-complete replacements
    /// of given <paramref name="input"/> which can replace
    /// entire user input on if user actively selects them.
    /// For xample think of [tab] completion in shells.
    /// E.g. for 'f' return 'foo', 'faa' etc.,
    /// the original <paramref name="input"/>
    /// MUST NOT not be included, e.g. 'f'.
    /// </summary>
    /// <param name="input">Input to find auto-complete
    ///     replacement for.</param>
    /// <returns>Collection of auto-completion replacements.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    IEnumerable<string> GetAutocomplete(string input);
}
