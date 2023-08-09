namespace Radicle.CLI.Models;

using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using Radicle.Common;
using Radicle.Common.Check;
using Radicle.Common.Extensions;
using Radicle.Common.Profiling.Models;
using Radicle.Common.Visual;

/// <summary>
/// Immutable progress model used for REPL
/// with string representations.
/// </summary>
internal sealed class ProgressViewModel
{
    private readonly ImmutableArray<char> spinner;

    private readonly HorizontalBarPlotFormatter plotFormatter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressViewModel"/> class.
    /// </summary>
    /// <param name="etaStatus">Instance of ETA status to base data on.</param>
    /// <param name="spinner">Spinner containing consecutively displayed characters.</param>
    /// <param name="plotFormatter">Plot formatter.</param>
    /// <param name="status">Optional one-line status.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="spinner"/> is out of [2, 42] range.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="status"/> is multiline string.</exception>
    public ProgressViewModel(
            ETAStatus etaStatus,
            ImmutableArray<char> spinner,
            HorizontalBarPlotFormatter plotFormatter,
            string status = "")
    {
        this.ETAStatus = Ensure.Param(etaStatus).Value;
        this.Status = Ensure.Param(status).NoNewLines().Value;
        this.spinner = Ensure.Collection(spinner).InRange(2, 42).ToImmutableArray();
        this.plotFormatter = Ensure.Param(plotFormatter).Value;
    }

    /// <summary>
    /// Gets a value indicating whether to include runtime performance counters.
    /// </summary>
    public bool IncludePerformanceCounters { get; init; }

    /// <summary>
    /// Gets current one-line status.
    /// </summary>
    public string Status { get; }

    /// <summary>
    /// Gets ETA calculator.
    /// </summary>
    public ETAStatus ETAStatus { get; }

    /// <summary>
    /// Gets current progress ration.
    /// </summary>
    public double Ratio => this.ETAStatus.Total.HasValue && this.ETAStatus.Total != 0
            ? (double)this.ETAStatus.LastCount / this.ETAStatus.Total.Value
            : 1.0;

    /// <summary>
    /// Convert this instance to printable string.
    /// </summary>
    /// <param name="requestCounter">Request counter
    ///     to determine spinner position. Each time
    ///     you call this method increment the counter by 1.</param>
    /// <returns>String representation.</returns>
    public string ToString(ulong requestCounter = 0)
    {
        ImmutableArray<char> spinnerChars = this.spinner;
        char spinner = spinnerChars[(int)(requestCounter % (ulong)spinnerChars.Length)];

        return new StringBuilder(spinner.ToString())
                .Append(' ')
                .Append(this.GetProgressOrCurrentCount())
                .Append(this.CountersOrEmpty())
                .Append(' ')
                .Append(this.SpeedOrEmpty())
                .Append("; ")
                .Append(this.GetETAOrElapsedTime())
                .Append(this.StatusOrEmpty())
                .ToString();
    }

    private string GetETAOrElapsedTime()
    {
        if (this.ETAStatus.Speed == 0)
        {
            return "ETA ?"; // i.e. infinite
        }
        else if (this.ETAStatus.ETA.HasValue)
        {
            return $"ETA {this.ETAStatus.ETA.Value.ToH()}";
        }

        return this.ETAStatus.Elapsed.ToHuman();
    }

    private string GetProgressOrCurrentCount()
    {
        // progress in bars/percentage
        if (this.ETAStatus.Total.HasValue && this.ETAStatus.Total.Value != 0)
        {
            string total = $"{this.ETAStatus.Total ?? '?'}";
            string count = $"{this.ETAStatus.LastCount}".PadLeft(total.Length);
            string counts = $"{count}/{total}";

            return $"{counts} {this.plotFormatter.ToLine(this.Ratio)}";
        }

        // just count
        return $"{this.ETAStatus.LastCount}";
    }

    private string SpeedOrEmpty()
    {
        if (this.ETAStatus.Speed > 0.0)
        {
            return $"({this.ETAStatus.Speed:f0} ops/s)";
        }

        return string.Empty;
    }

    private string CountersOrEmpty()
    {
        if (this.IncludePerformanceCounters)
        {
            ThreadCounters w = Common.Profiling.PerformanceCounters.Worker;
            ThreadCounters io = Common.Profiling.PerformanceCounters.CompletionPort;
            StringBuilder sb = new("[");
            bool first = true;

            foreach ((string name, ThreadCounters t) in new[] { ("w", w), ("io", io) })
            {
                if (!first)
                {
                    sb = sb.Append('|');
                }

                first = false;
                string busy = t.Busy.ToString(CultureInfo.InvariantCulture).PadLeft(2);
                sb = sb.Append(name)
                        .Append(':')
                        .Append(busy)
                        .Append('/')
                        .Append(t.Available)
                        .Append('_')
                        .Append(t.Min);
            }

            return sb.Append(']').ToString();
        }

        return string.Empty;
    }

    private string StatusOrEmpty()
    {
        if (!string.IsNullOrWhiteSpace(this.Status))
        {
            // the trimming to buffer width will happen in printer
            // here we trim just to make it look better
            return $": {this.Status.Ellipsis(trim: 128)}";
        }

        return string.Empty;
    }
}
