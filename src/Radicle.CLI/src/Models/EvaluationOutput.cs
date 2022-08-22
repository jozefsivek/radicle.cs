namespace Radicle.CLI.Models;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Radicle.Common.Check;
using Radicle.Common.Tokenization;
using Radicle.Common.Tokenization.Models;

/// <summary>
/// Printable output of the evaluation.
/// </summary>
public sealed class EvaluationOutput
{
    /// <summary>
    /// No operation execution output.
    /// </summary>
    public static readonly EvaluationOutput NoOp = new(EvaluationOutputState.NoOp);

    /// <summary>
    /// Plain exit execution output.
    /// </summary>
    public static readonly EvaluationOutput Exit = new(EvaluationOutputState.Exit);

    /// <summary>
    /// No message (silent) error execution output
    /// for cases when errors should be hidden because
    /// they are expected due to nature of task (e.g. help
    /// about non existing command or similar).
    /// </summary>
    public static readonly EvaluationOutput SilentError = new(EvaluationOutputState.Error);

    private static readonly StopWordStringParser SimpleMarkdownStopWordStringParser = new(
            stopWords: new HashSet<ParsedTokenStopWord>(new[]
            {
                SimpleMarkdownText.EmphasisedStopWord,
                SimpleMarkdownText.BoldStopWord,
                SimpleMarkdownText.EscapeStopWord,
            }),
            controlTokenAction: (state) =>
            {
                if (state.TriggeringStopWord == SimpleMarkdownText.EscapeStopWord)
                {
                    return ParseAction.ContinueWithControllUntil(1);
                }

                return ParseAction.Continue;
            });

    /// <summary>
    /// Initializes a new instance of the <see cref="EvaluationOutput"/> class.
    /// </summary>
    /// <param name="state">Type of the output.</param>
    /// <param name="outputLines">Output lines data if any.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required argument is <see langword="null"/>.</exception>
    private EvaluationOutput(
            EvaluationOutputState state,
            IEnumerable<OutputLine>? outputLines = null)
    {
        this.State = state;
        this.OutputLines = Ensure.Optional(outputLines).AllNotNull().ToImmutableArray();
    }

    /// <summary>
    /// Gets the state of the execution output.
    /// </summary>
    public EvaluationOutputState State { get; }

    /// <summary>
    /// Gets execution output payload.
    /// </summary>
    public ImmutableArray<OutputLine> OutputLines { get; }

    /// <summary>
    /// Creates success execution output.
    /// </summary>
    /// <param name="outputLines">Output lines data if any.</param>
    /// <returns>instance of <see cref="EvaluationOutput"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required argument is <see langword="null"/>.</exception>
    public static EvaluationOutput SuccessFrom(
            IEnumerable<OutputLine>? outputLines = null)
    {
        return new EvaluationOutput(EvaluationOutputState.Success, outputLines);
    }

    /// <summary>
    /// Creates success execution output.
    /// </summary>
    /// <param name="mds">Simple markdown if any.</param>
    /// <returns>instance of <see cref="EvaluationOutput"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required argument is <see langword="null"/>.</exception>
    public static EvaluationOutput SuccessFrom(
            IEnumerable<SimpleMarkdownText> mds)
    {
        return new EvaluationOutput(
                EvaluationOutputState.Success,
                Ensure.Param(mds).AllNotNull().Select(md => Convert(md)));
    }

    /// <summary>
    /// Creates success execution output.
    /// </summary>
    /// <param name="md">Simple markdown.</param>
    /// <returns>instance of <see cref="EvaluationOutput"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required argument is <see langword="null"/>.</exception>
    public static EvaluationOutput SuccessFrom(
            SimpleMarkdownText md)
    {
        return SuccessFrom(new[] { md });
    }

    /// <summary>
    /// Creates error output with simple message.
    /// </summary>
    /// <param name="message">Message as it is.</param>
    /// <param name="exception">Optional exception.</param>
    /// <returns>instance of <see cref="EvaluationOutput"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required argument is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if parameters are empty.</exception>
    /// <exception cref="ArgumentException">Thrown if parameters
    ///     are white space or contain new lines.</exception>
    public static EvaluationOutput ErrorFrom(
            string message,
            Exception? exception = null)
    {
        Ensure.Param(message)
                .NotEmpty()
                .NotWhiteSpace()
                .NoNewLines().Done();

        List<OutputLine> lines = new()
        {
            message,
        };

        if (exception is not null)
        {
            lines.AddRange(OutputLine.ToOutputLines(exception.ToString()));
        }

        return new EvaluationOutput(
                EvaluationOutputState.Error,
                lines);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder($"{this.State}")
            .AppendLine();

        foreach (OutputLine line in this.OutputLines)
        {
            sb = sb.AppendLine(line.ToString());
        }

        return sb.ToString();
    }

    private static OutputLine Convert(SimpleMarkdownText md)
    {
        List<OutputSnippet> snippets = new();
        OutputStyle nextStyle = OutputStyle.Normal;

        foreach (ParsedToken token in SimpleMarkdownStopWordStringParser.Parse(md.Value))
        {
            if (token is ParsedTokenControl escape)
            {
                snippets.Add(new OutputSnippet(nextStyle, escape.Value));
            }
            else if (token is ParsedTokenStopWord stop)
            {
                if (nextStyle == OutputStyle.Normal)
                {
                    if (stop == SimpleMarkdownText.BoldStopWord)
                    {
                        nextStyle = OutputStyle.Bold;
                    }
                    else if (stop == SimpleMarkdownText.EmphasisedStopWord)
                    {
                        nextStyle = OutputStyle.Emphasized;
                    }
                }
                else if (
                        (stop == SimpleMarkdownText.BoldStopWord
                            && nextStyle == OutputStyle.Bold)
                        || (stop == SimpleMarkdownText.EmphasisedStopWord
                            && nextStyle == OutputStyle.Emphasized))
                {
                    nextStyle = OutputStyle.Normal;
                }
            }
            else
            {
                snippets.Add(new OutputSnippet(nextStyle, token.Value));
            }
        }

        return new OutputLine(snippets, OutputSnippetSeparator.Empty);
    }

    /* TODO: add other conversions */
}
