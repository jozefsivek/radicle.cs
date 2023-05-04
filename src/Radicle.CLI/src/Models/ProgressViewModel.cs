namespace Radicle.CLI.Models;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Radicle.Common;
using Radicle.Common.Check;
using Radicle.Common.Extensions;
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
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressViewModel"/> class.
    /// </summary>
    /// <param name="progress">Progress source, values are clipped if required.</param>
    /// <param name="spinner">Spinner containing consecutively displayed characters.</param>
    /// <param name="plotFormatter">Plot formatter.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="spinner"/> is out of [2, 42] range.</exception>
    public ProgressViewModel(
            TransparentProgress<long> progress,
            ImmutableArray<char> spinner,
            HorizontalBarPlotFormatter plotFormatter)
    {
        Ensure.Param(progress).Done();

        this.Count = progress.Count < 0L
                ? 0uL
                : (ulong)progress.Count;
        this.Total = (progress.Total >= 0L)
                ? Math.Max(this.Count, (ulong)progress.Total)
                : 0uL;
        this.Elapsed = (DateTimeOffset.UtcNow - progress.StartDate).UseIfLongerOr(TimeSpan.Zero);
        this.spinner = Ensure.Collection(spinner).InRange(2, 42).ToImmutableArray();
        this.plotFormatter = Ensure.Param(plotFormatter).Value;
    }

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
    /// Gets estimated time of arrival.
    /// </summary>
    public TimeSpan ETA
    {
        get
        {
            double multiplier = (1.0 - this.Ratio) / this.Ratio;
            TimeSpan maximumETA = TimeSpan.FromDays(365 * 2);

            if (double.IsInfinity(multiplier))
            {
                return maximumETA;
            }

            // time span multiplication can overflow
            // with System.OverflowException unlike:
            double elapsed = this.Elapsed.TotalSeconds;
            double eta = elapsed * multiplier;

            if (maximumETA.TotalSeconds <= eta)
            {
                return maximumETA;
            }

            return TimeSpan.FromSeconds(eta);
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
                .Append(' ')
                .Append(this.SpeedOrEmpty())
                .Append("; ")
                .Append(this.GetETAOrElapsedTime())
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
        if (this.Elapsed != TimeSpan.Zero)
        {
            double speed = this.Count / this.Elapsed.TotalSeconds;

            return $"({speed:f0} ops/s)";
        }

        return string.Empty;
    }
}
