namespace Radicle.Sample.CLI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Radicle.CLI.Evaluators;
using Radicle.CLI.Evaluators.Generic;
using Radicle.CLI.IO;
using Radicle.CLI.Models;
using Radicle.CLI.REPL;
using Radicle.Common;
using Radicle.Common.Check;
using Radicle.Common.Tokenization;
using Radicle.Common.Tokenization.Models;
using Radicle.Sample.CLI.Evaluators;

/// <summary>
/// Implementation of <see cref="IREPLEvaluator"/>.
/// </summary>
public sealed class SampleREPLEvaluator : IREPLEvaluator, IEvaluationContext
{
    private readonly CLILikeArgumentsTokenizer argumentTokenizer = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="SampleREPLEvaluator"/> class.
    /// </summary>
    /// <param name="stylingProvider">Styling provider.</param>
    /// <param name="history">History.</param>
    /// <param name="readerWriter">Reader and writer.</param>
    /// <param name="bundle">Bundle of evaluators.</param>
    private SampleREPLEvaluator(
            IREPLStylingProvider stylingProvider,
            IREPLInputHistory history,
            IREPLReaderWriter readerWriter,
            CommandEvaluatorBundle<EvaluationOutput, SampleREPLEvaluator> bundle)
    {
        this.StylingProvider = stylingProvider;
        this.History = history;
        this.ReaderWriter = readerWriter;
        this.Bundle = bundle;
    }

    /// <inheritdoc/>
    public IREPLStylingProvider StylingProvider { get; }

    /// <inheritdoc/>
    public IREPLInputHistory History { get; }

    /// <summary>
    /// Gets used reader/writer.
    /// </summary>
    public IREPLReaderWriter ReaderWriter { get; }

    /// <summary>
    /// Gets bundle with individual evaluators.
    /// </summary>
    public CommandEvaluatorBundle<EvaluationOutput, SampleREPLEvaluator> Bundle { get; }

    /// <inheritdoc/>
    public CommandEvaluatorBundle Commands => this.Bundle;

    /// <summary>
    /// Create instance of <see cref="SampleREPLEvaluator"/>.
    /// </summary>
    /// <returns>Instance of <see cref="SampleREPLEvaluator"/>.</returns>
    public static async Task<SampleREPLEvaluator> CreateAsync()
    {
        CommandEvaluatorBundle<EvaluationOutput, SampleREPLEvaluator> bundle = new()
        {
            new EchoEvaluator(),
            new HelpEvaluator(),
            new HistoryEvaluator(),
            new WaitEvaluator(),
            new QuitEvaluator(),
            new FooEvaluator(),
        };
        IREPLStylingProvider stylingProvider = new SampleREPLStylingProvider(bundle);
        IREPLInputHistory history = await ConstructHistoryAsync().ConfigureAwait(false);
        IREPLReaderWriter readerWriter = new REPLConsoleReaderWriter(
                stylingProvider,
                history);

        return new SampleREPLEvaluator(
                stylingProvider,
                history,
                readerWriter,
                bundle);
    }

    /// <inheritdoc/>
    public EvaluationOutputPromise Evaluate(
            string input,
            CancellationToken cancellationToken = default)
    {
        Ensure.Param(input).Done();

        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(input))
        {
            return EvaluationOutputPromise.Completed(EvaluationOutput.NoOp);
        }

        TokenWithValue[] arguments = this.argumentTokenizer.Parse(input).ToArray();

        if (arguments.Length == 0)
        {
            return EvaluationOutputPromise.Completed(EvaluationOutput.NoOp);
        }

        TransparentProgress<long> progress = new();

        return new EvaluationOutputPromise(
                this.EvaluateInternalAsync(
                    arguments,
                    progress: progress,
                    cancellationToken: cancellationToken),
                progress: progress);
    }

    /// <inheritdoc/>
    public async Task AuditEvaluationAsync(
            string input,
            TimeSpan elapsed,
            CancellationToken cancellationToken = default)
    {
        if (this.History.Push(input))
        {
            string filePath = HistoryReader.GetFilePath(".radicle_sample_cli_history");
            _ = await HistoryReader.TryAppendHistoryAsync(
                    filePath,
                    new[] { input }).ConfigureAwait(false);
        }
    }

    private static async Task<REPLInputHistory> ConstructHistoryAsync()
    {
        string filePath = HistoryReader.GetFilePath(".radicle_sample_cli_history");
        ICollection<string> rawHistory = await HistoryReader
                .TryReadHistoryAsync(filePath)
                .ConfigureAwait(false);
        REPLInputHistory history = new(
                initialRecords: rawHistory,
                ignoreRepeatedInputs: true);

        // trim stored long history
        if (rawHistory.Count > 2042)
        {
            _ = await HistoryReader.TryWriteHistoryAsync(
                    filePath,
                    rawHistory.Skip(1024)).ConfigureAwait(false);
        }

        return history;
    }

    private async Task<EvaluationOutput> EvaluateInternalAsync(
            TokenWithValue[] arguments,
            TransparentProgress<long>? progress = null,
            CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (this.Bundle.TryMapByVerbOrSynonym(
                arguments,
                out ICommandEvaluator<EvaluationOutput, SampleREPLEvaluator>? evaluator,
                out TokenWithValue[]? remainingArguments))
        {
            try
            {
                return await evaluator
                        .EvaluateAsync(
                            remainingArguments,
                            this,
                            progress: progress,
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return EvaluationOutput.ErrorFrom("BUG:", e);
            }
        }
        else
        {
            string argumentsPrintout = string.Join(
                    ' ',
                    arguments.Select(a => Dump.Literal(a)));

            return EvaluationOutput.ErrorFrom($"Unknown verb: {argumentsPrintout}");
        }
    }
}
