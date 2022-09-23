namespace Radicle.CLI.REPL;

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Radicle.CLI.Models;
using Radicle.Common.Check;

/// <summary>
/// REPL (Read-eval-print loop) loop is construct actually
/// listening and evaluating events. Create and call this loop
/// from your CLI program.
/// </summary>
public sealed class REPLEventLoop
{
    /// <summary>
    /// Regular expression matching the integer field name.
    /// </summary>
    private static readonly Regex MultiInputRegex = new(
            "\\A([1-9][0-9]{0,3}\\s+)([^\\s].+)\\z",
            RegexOptions.Compiled | RegexOptions.Singleline);

    private readonly IREPLReaderWriter readerWriter;

    private readonly IREPLEvaluator evaluator;

    private readonly bool allowMultiInput = true;

    private readonly bool allowRepeats = true;

    private readonly TimeSpan slowThreshold = TimeSpan.FromSeconds(2);

    /// <summary>
    /// Initializes a new instance of the <see cref="REPLEventLoop"/> class.
    /// </summary>
    /// <param name="readerWriter">REPL reader writer.</param>
    /// <param name="evaluator">Evaluator of the user input.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public REPLEventLoop(
            IREPLReaderWriter readerWriter,
            IREPLEvaluator evaluator)
    {
        this.readerWriter = Ensure.Param(readerWriter).Value;
        this.evaluator = Ensure.Param(evaluator).Value;
    }

    /// <summary>
    /// Decompose given <paramref name="input"/> to multi-input prefix
    /// and clean input.
    /// </summary>
    /// <param name="input">Input to decompose.</param>
    /// <returns>Decomposed input. Prefix can be empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public (string MultiInputPrefix, string Input) DecomposeInput(string input)
    {
        Ensure.Param(input).Done();

        if (this.allowMultiInput)
        {
            Match match = MultiInputRegex.Match(input);

            if (match.Success && match.Groups[1].Success && match.Groups[2].Success)
            {
                return (match.Groups[1].Value, match.Groups[2].Value);
            }
        }

        return (string.Empty, input);
    }

    /// <summary>
    /// Runs the loop.
    /// </summary>
    /// <param name="cancellationToken">Optional cancelation token
    ///     used for cancelling loop.</param>
    /// <exception cref="OperationCanceledException">Thrown
    ///     if operation is cancelled with <paramref name="cancellationToken"/>.</exception>
    /// <returns>Awaitable task.</returns>
    public async Task RunAsync(
            CancellationToken cancellationToken = default)
    {
        while (true)
        {
            string input = await this.readerWriter.ReadAsync(cancellationToken).ConfigureAwait(false);

            (ushort repeats, input) = this.ExtractRepeats(input);

            for (ushort i = 0; i < repeats; i++)
            {
                if (i > 0)
                {
                    await this.readerWriter.WriteRepeatEvaluationOutputSeparatorAsync(
                            (ushort)(i + 1),
                            repeats,
                            input).ConfigureAwait(false);
                }

                Stopwatch sw = Stopwatch.StartNew();
                EvaluationOutputPromise outputPromise = this.evaluator.Evaluate(
                        input,
                        cancellationToken: cancellationToken);
                EvaluationOutput? output = default;

                await this.readerWriter.WriteLinesAsync(
                        outputPromise.Progress,
                        async () =>
                        {
                            output = await outputPromise.Promise
                                    .ConfigureAwait(false);

                            return output.OutputLines;
                        }).ConfigureAwait(false);

                sw.Stop();

#pragma warning disable CA1508 // Avoid dead conditional code
                if (output?.State == EvaluationOutputState.Exit)
                {
                    return;
                }
                else if (sw.Elapsed > this.slowThreshold)
                {
                    await this.readerWriter.WriteElapsedMarkAsync(sw.Elapsed).ConfigureAwait(false);
                }
#pragma warning restore CA1508 // Avoid dead conditional code

                await this.evaluator.AuditEvaluationAsync(
                        input,
                        sw.Elapsed,
                        cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private (ushort Repeats, string Input) ExtractRepeats(string input)
    {
        if (this.allowRepeats)
        {
            Match match = MultiInputRegex.Match(input);

            if (match.Success
                    && match.Groups[2].Success
                    && ushort.TryParse(input.Split(' ', 2)[0], out ushort r)
                    && r > 1)
            {
                return (r, match.Groups[2].Value);
            }
        }

        return (1, input);
    }
}
