namespace Radicle.Sample.CLI;

using System;
using System.Collections.Generic;
using System.Linq;
using Radicle.CLI.Evaluators;
using Radicle.CLI.Models;
using Radicle.CLI.REPL;
using Radicle.Common.Check;

/// <summary>
/// Implementation of <see cref="IREPLStylingProvider"/>.
/// </summary>
internal class SampleREPLStylingProvider : IREPLStylingProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SampleREPLStylingProvider"/> class.
    /// </summary>
    /// <param name="bundle">Current bundle used.</param>
    public SampleREPLStylingProvider(CommandEvaluatorBundle bundle)
    {
        this.Bundle = Ensure.Param(bundle).Value;
    }

    /// <inheritdoc />
    public string PromptStringOne => "sample> ";

    /// <inheritdoc />
    public bool EnableColor => true;

    /// <inheritdoc />
    public SpinnerType SpinnerType => SpinnerType.Snake;

    /// <inheritdoc />
    public ProgressBarsType ProgressBarsType => ProgressBarsType.BlockDots;

    /// <summary>
    /// Gets currently used bundle.
    /// </summary>
    public CommandEvaluatorBundle Bundle { get; }

    /// <inheritdoc />
    public string GetSuggestionTail(string input)
    {
        Ensure.Param(input).Done();

        // TODO: recursive + extract
        string[] all = this.Bundle
                .GetAllListed()
                .Select(r => r.Evaluator.Verb.Value)
                .ToArray();

        foreach (string v in all)
        {
            if (v.StartsWith(input, StringComparison.OrdinalIgnoreCase))
            {
                return v[input.Length..];
            }
        }

        return string.Empty;
    }

    /// <inheritdoc />
    public IEnumerable<string> GetAutocomplete(string input)
    {
        Ensure.Param(input).Done();

        string[] all = this.Bundle
                .GetAllListed()
                .Select(r => r.Evaluator.Verb.Value)
                .ToArray();
        List<string> autocomplete = new();

        foreach (string v in all)
        {
            if (v.StartsWith(input, StringComparison.OrdinalIgnoreCase))
            {
                autocomplete.Add(v);
            }
        }

        return autocomplete;
    }
}
