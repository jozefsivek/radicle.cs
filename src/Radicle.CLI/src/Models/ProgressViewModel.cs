namespace Radicle.CLI.Models;

using System;
using System.Collections.Generic;
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

    private readonly TimeSpan maximumETA = TimeSpan.FromDays(365 * 100);

    private double? speedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressViewModel"/> class.
    /// </summary>
    /// <param name="count">Current count.</param>
    /// <param name="spinner">Spinner containing consecutively displayed characters.</param>
    /// <param name="plotFormatter">Plot formatter.</param>
    /// <param name="totalCount">Optional total amount of items.</param>
    /// <param name="duration">Elapsed time from start of the progress.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="spinner"/> is out of [2, 42] range.</exception>
    public ProgressViewModel(
            ulong count,
            IEnumerable<char> spinner,
            HorizontalBarPlotFormatter plotFormatter,
            ulong? totalCount = null,
            TimeSpan? duration = null)
    {
        this.Count = count;
        this.Total = totalCount.HasValue ? Math.Max(count, totalCount.Value) : totalCount;
        this.Elapsed = Ensure.Optional(duration)
                .NonNegative()
                .ValueOr(TimeSpan.Zero);
        this.spinner = Ensure.Collection(spinner).InRange(2, 42).ToImmutableArray();
        this.plotFormatter = Ensure.Param(plotFormatter).Value;
        this.LastReports = ImmutableArray<ProgressReport<long>>.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressViewModel"/> class.
    /// </summary>
    /// <param name="progress">Progress source, values are clipped if required.</param>
    /// <param name="spinner">Spinner containing consecutively displayed characters.</param>
    /// <param name="plotFormatter">Plot formatter.</param>
    /// <param name="lastReports">Last reports, if available.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="spinner"/> is out of [2, 42] range.</exception>
    public ProgressViewModel(
            TransparentProgress<long> progress,
            ImmutableArray<char> spinner,
            HorizontalBarPlotFormatter plotFormatter,
            IEnumerable<ProgressReport<long>>? lastReports = null)
    {
        Ensure.Param(progress).Done();

        this.Count = progress.Count < 0L
                ? 0uL
                : (ulong)progress.Count;
        this.Total = (progress.Total >= 0L)
                ? Math.Max(this.Count, (ulong)progress.Total)
                : 0uL;
        this.Elapsed = (DateTimeOffset.UtcNow - progress.StartDate).UseIfLongerOr(TimeSpan.Zero);
        this.Status = progress.Status;
        this.spinner = Ensure.Collection(spinner).InRange(2, 42).ToImmutableArray();
        this.plotFormatter = Ensure.Param(plotFormatter).Value;
        this.LastReports = Ensure.OptionalCollection(lastReports).ToImmutableArray();
    }

    /// <summary>
    /// Gets a value indicating whether to include runtime performance counters.
    /// </summary>
    public bool IncludePerformanceCounters { get; init; }

    /// <summary>
    /// Gets current count.
    /// </summary>
    public ulong Count { get; }

    /// <summary>
    /// Gets current known total, if known.
    /// The amount is never smaller than <see cref="Count"/>.
    /// </summary>
    public ulong? Total { get; }

    /// <summary>
    /// Gets elapsed time from the start.
    /// Can be zero.
    /// </summary>
    public TimeSpan Elapsed { get; }

    /// <summary>
    /// Gets history reports. Can be empty.
    /// </summary>
    public ImmutableArray<ProgressReport<long>> LastReports { get; }

    /// <summary>
    /// Gets current status.
    /// </summary>
    public string Status { get; } = string.Empty;

    /// <summary>
    /// Gets estimated time of arrival computed as average across whole time.
    /// </summary>
    public TimeSpan AverageETA => this.ComputeETA(this.AverageSpeed);

    /// <summary>
    /// Gets average speed, operations per second,
    /// computed as average across whole time.
    /// </summary>
    public double AverageSpeed
    {
        get
        {
            if (this.Elapsed == TimeSpan.Zero)
            {
                return 0.0;
            }

            return this.Count / this.Elapsed.TotalSeconds;
        }
    }

    /// <summary>
    /// Gets estimated time of arrival computed from
    /// the <see cref="LastReports"/> if possible or
    /// fallback to <see cref="AverageETA"/>.
    /// </summary>
    public TimeSpan ETA => this.ComputeETA(this.Speed);

    /// <summary>
    /// Gets speed, operations per second,
    /// computed from the contemporary observation.
    /// </summary>
    public double Speed
    {
        get
        {
            if (!this.speedValue.HasValue)
            {
                if (this.LastReports.Length < 2 || !this.Total.HasValue)
                {
                    this.speedValue = this.AverageSpeed;
                }
                else
                {
                    double weights = 0.0;
                    double speed = 0.0;

                    if (this.LastReports.Length < 4 && this.Elapsed != TimeSpan.Zero)
                    {
                        speed += this.Count / this.Elapsed.TotalSeconds;
                        weights++;
                    }

                    ProgressReport<long> last = default;
                    bool first = true;
                    double weight = 2.0;

                    foreach (ProgressReport<long> report in this.LastReports)
                    {
                        if (!first)
                        {
                            weight *= 2.0;
                            TimeSpan timeIncrement = report.Date - last.Date;

                            if (timeIncrement != TimeSpan.Zero)
                            {
                                speed += weight * (report.Count - last.Count)
                                        / timeIncrement.TotalSeconds;
                                weights += weight;
                            }
                        }

                        last = report;
                        first = false;
                    }

                    this.speedValue = speed / weights;
                }
            }

            return this.speedValue.Value;
        }
    }

    /// <summary>
    /// Gets current progress ration.
    /// </summary>
    public double Ratio => this.Total.HasValue && this.Total != 0
            ? (double)this.Count / this.Total.Value
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
        if (this.Total.HasValue && this.Total.Value != 0)
        {
            return $"ETA {this.ETA.ToH()}";
        }

        return this.Elapsed.ToHuman();
    }

    private string GetProgressOrCurrentCount()
    {
        // progress in bars/percentage
        if (this.Total.HasValue && this.Total.Value != 0)
        {
            string total = $"{this.Total ?? '?'}";
            string count = $"{this.Count}".PadLeft(total.Length);
            string counts = $"{count}/{total}";

            return $"{counts} {this.plotFormatter.ToLine(this.Ratio)}";
        }

        // just count
        return $"{this.Count}";
    }

    private string SpeedOrEmpty()
    {
        if (this.Speed > 0.0)
        {
            return $"({this.Speed:f0} ops/s)";
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

    private TimeSpan ComputeETA(double opsPerSecond)
    {
        if (opsPerSecond == 0.0)
        {
            return this.maximumETA;
        }
        else if (this.Total.HasValue)
        {
            double eta = ((double)this.Total - this.Count) / opsPerSecond;

            // time span multiplication can overflow
            // with System.OverflowException:
            return Math.Abs(eta) >= this.maximumETA.TotalSeconds || double.IsNaN(eta)
                    ? this.maximumETA
                    : TimeSpan.FromSeconds(eta);
        }
        else
        {
            return TimeSpan.Zero;
        }
    }
}
