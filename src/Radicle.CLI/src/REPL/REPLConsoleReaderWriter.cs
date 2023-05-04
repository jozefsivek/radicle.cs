namespace Radicle.CLI.REPL;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Radicle.CLI.Models;
using Radicle.Common;
using Radicle.Common.Check;
using Radicle.Common.Extensions;
using Radicle.Common.Visual;
using Radicle.Common.Visual.Models;

/// <summary>
/// System Console implementation of <see cref="IREPLReaderWriter"/>.
/// This implementation is not thread safe.
/// </summary>
public class REPLConsoleReaderWriter : IREPLReaderWriter
{
    private readonly IREPLStylingProvider stylingProvider;

    private readonly IREPLPrinter printer = new REPLConsolePrinter();

    private readonly IREPLInputHistory history;

    /// <summary>
    /// Initializes a new instance of the <see cref="REPLConsoleReaderWriter"/> class.
    /// </summary>
    /// <param name="stylingProvider">Meta data provider.</param>
    /// <param name="history">History. Note this is used only to read,
    ///     the caller is responsible for pushing new records to history.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public REPLConsoleReaderWriter(
            IREPLStylingProvider stylingProvider,
            IREPLInputHistory history)
    {
        this.stylingProvider = Ensure.Param(stylingProvider).Value;
        this.history = Ensure.Param(history).Value;
    }

    /// <inheritdoc/>
    public Task WriteAsync(OutputLine? value = null)
    {
        if (value is not null)
        {
            WriteLine(value, color: !this.stylingProvider.EnableColor);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task WriteLinesAsync(IEnumerable<OutputLine> values)
    {
        Ensure.Param(values).Done();

        foreach (OutputLine value in values)
        {
            Ensure.Param(value).Done();

            WriteLine(value, color: this.stylingProvider.EnableColor);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task WriteLinesAsync(
            TransparentProgress<long> progress,
            Func<Task<IEnumerable<OutputLine>>> outputFactory)
    {
        Task<IEnumerable<OutputLine>> task = Ensure.Param(outputFactory).Value();
        DateTime lastPrintout = DateTime.UtcNow;
        ProgressViewModel? lastProgress = default;
        ulong request = 0;

        while (!task.IsCompleted)
        {
            if (lastPrintout.IsOlderThan(TimeSpan.FromMilliseconds(250)))
            {
                ProgressViewModel currentProgress = new(
                        progress,
                        spinner: Spinners.GetSpinner(this.stylingProvider.SpinnerType),
                        plotFormatter: ConstructFormatter(this.stylingProvider.ProgressBarsType));

                if (lastProgress is null)
                {
                    this.printer.Print(
                            currentProgress,
                            request++,
                            color: this.stylingProvider.EnableColor);
                }
                else
                {
                    this.printer.Print(
                            lastProgress,
                            currentProgress,
                            request++,
                            color: this.stylingProvider.EnableColor);
                }

                lastProgress = currentProgress;
                lastPrintout = DateTime.UtcNow;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false);
        }

        if (lastProgress is not null)
        {
            this.printer.Clear(lastProgress);
        }

        IEnumerable<OutputLine> lines = await task.ConfigureAwait(false);

        await this.WriteLinesAsync(lines).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task WriteRepeatEvaluationOutputSeparatorAsync(
            ushort repeat,
            ushort total,
            string repeatingUserInput)
    {
        Ensure.Param(repeat).StrictlyPositive().Done();
        Ensure.Param(total).StrictlyPositive().LowerThanOrEqual(repeat).Done();

        await this.WriteAsync(new OutputLine(new[]
        {
            new OutputSnippet(OutputStyle.Emphasized, $"[  {repeat}/{total}  ]> "),
            new OutputSnippet(OutputStyle.Normal, Ensure.Param(repeatingUserInput).Value),
        })).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task WriteElapsedMarkAsync(TimeSpan elapsed)
    {
        await this.WriteAsync(new OutputLine(new[]
        {
            new OutputSnippet(OutputStyle.Emphasized, $"[  elapsed: {elapsed.ToHuman()}  ]"),
        })).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<string> ReadAsync(
            CancellationToken cancellationToken = default)
    {
        PromptViewModel cleanModel = new(this.stylingProvider.PromptStringOne);
        PromptViewModel lastModel = cleanModel;
        LoopCollection<PromptViewModel>? rotatingAutocompleteModels = null;
        PromptViewModel currentModel = lastModel;

        ConsoleKeyInfo keyInfo;

        this.printer.Print(currentModel, color: this.stylingProvider.EnableColor);

        do
        {
            keyInfo = Console.ReadKey(true);
            bool done = false;

            if (
                    (keyInfo.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt
                    || (keyInfo.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)
            {
                // Ignore Alt or Ctrl combinations for now
                continue;
            }
            else if (keyInfo.Key == ConsoleKey.Tab)
            {
                if (rotatingAutocompleteModels is not null)
                {
                    currentModel = rotatingAutocompleteModels.Next();
                }
                else
                {
                    // Ignore tab key.
                    string[] replacements = this.stylingProvider
                            .GetAutocomplete(currentModel.UserInputValue)
                            .ToArray();

                    if (replacements.Length > 0)
                    {
                        rotatingAutocompleteModels = new LoopCollection<PromptViewModel>(
                                new[] { currentModel }
                                    .Concat(replacements.Select(s => cleanModel.Write(s))));
                        currentModel = rotatingAutocompleteModels.Next();
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                rotatingAutocompleteModels = null;
                done = true;
            }
            else if (keyInfo.Key == ConsoleKey.LeftArrow)
            {
                currentModel = currentModel.MoveCursorLeft();
            }
            else if (keyInfo.Key == ConsoleKey.RightArrow)
            {
                currentModel = currentModel.MoveCursorRight();
            }
            else if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                if (this.history.TryMovePeekWindowUp(currentModel.UserInputValue, out string? windowValue))
                {
                    currentModel = cleanModel.Write(windowValue);
                }
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                if (this.history.TryMovePeekWindowDown(currentModel.UserInputValue, out string? windowValue))
                {
                    currentModel = cleanModel.Write(windowValue);
                }
            }
            else if (keyInfo.Key == ConsoleKey.Backspace)
            {
                rotatingAutocompleteModels = null;
                currentModel = currentModel.BackDelete();
            }
            else if (keyInfo.Key == ConsoleKey.Delete)
            {
                rotatingAutocompleteModels = null;
                currentModel = currentModel.ForwardDelete();
            }
            else if (keyInfo.KeyChar == '\u0000' || keyInfo.Key == ConsoleKey.Escape)
            {
                // Ignore if KeyChar value is \u0000 - special character for non printable keys
                // or escape
                continue;
            }
            else
            {
                rotatingAutocompleteModels = null;

                // add character
                currentModel = currentModel.Write(keyInfo.KeyChar);
            }

            // Check completion
            int bufferWidth = Console.BufferWidth;

            if (!done && ReferenceEquals(lastModel, currentModel))
            {
                // pass, keep current model completion intact
            }
            else if (
                    !done
                    && currentModel.TillTailLength < bufferWidth)
            {
                string suggestion = this.stylingProvider.GetSuggestionTail(
                        currentModel.UserInputValue);

                if (suggestion.Length > 0
                        && (currentModel.TillTailLength + suggestion.Length) < bufferWidth)
                {
                    currentModel = currentModel.WithSuggestionTail(suggestion);
                }
                else
                {
                    currentModel = currentModel.WithSuggestionTail(string.Empty);
                }
            }
            else
            {
                currentModel = currentModel.WithSuggestionTail(string.Empty);
            }

            this.printer.Print(
                    lastModel,
                    currentModel,
                    color: this.stylingProvider.EnableColor);

            lastModel = currentModel;
        }
        while (keyInfo.Key != ConsoleKey.Enter);

        Console.WriteLine();

        return Task.FromResult(currentModel.UserInputValue);
    }

    private static HorizontalBarPlotFormatter ConstructFormatter(
            ProgressBarsType type)
    {
        BarPlotStyle styleName = type switch
        {
            ProgressBarsType.ASCII => BarPlotStyle.ASCII,
            ProgressBarsType.Graphic => BarPlotStyle.FullBlock,
            ProgressBarsType.PartialBlock => BarPlotStyle.PartialBlock,
            ProgressBarsType.BlockDots => BarPlotStyle.BlockDots,
            ProgressBarsType.Dots => BarPlotStyle.Dots,
            _ => throw new NotSupportedException(
                    $"BUG: progress bar type {type} is not recognized."),
        };

        return new HorizontalBarPlotFormatter()
        {
            Interval = new DoubleInterval(0.0, 1.0),
            ShowLeftBoundary = true,
            ShowRightBoundary = true,
            StyleName = styleName,
            Width = 14,
        };
    }

    private static void WriteLine(
            OutputLine line,
            bool color = false)
    {
        bool notFirst = false;

        foreach (OutputSnippet snippet in line)
        {
            if (notFirst && line.Separator == OutputSnippetSeparator.Space)
            {
                Console.Write(' ');
            }

            notFirst = true;

            if (color)
            {
                switch (snippet.Style)
                {
                    case OutputStyle.Bold:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    case OutputStyle.Normal:
                        Console.ResetColor();
                        break;
                    case OutputStyle.Emphasized:
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                    case OutputStyle.Comment:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    default:
                        throw new NotSupportedException($"BUG: Unknown style {snippet.Style}");
                }
            }

            Console.Write(snippet.Value);
        }

        if (color)
        {
            Console.ResetColor();
        }

        Console.WriteLine();
    }
}
