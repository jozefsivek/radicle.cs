namespace Radicle.CLI.Models;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Radicle.Common;
using Radicle.Common.Check;
using Radicle.Common.Extensions;

/// <summary>
/// Immutable progress model used for REPL
/// with string representations.
/// </summary>
internal sealed class ProgressViewModel
{
    private readonly ImmutableArray<char> spinner;

    private readonly ImmutableArray<char> progressBars;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressViewModel"/> class.
    /// </summary>
    /// <param name="count">Current count.</param>
    /// <param name="spinner">Spinner containing consecutively displayed characters.</param>
    /// <param name="progressBars">Progress bars characters, first and last
    ///     items are treated as bounding characters. Inside items
    ///     are treated as characters representing empty to full state (thus minimu 2).
    ///     E.g.: ['(', ' ', '.', '_', ')'] will result in 50% progress displayed as:
    ///     "(___.   )".</param>
    /// <param name="totalCount">Optional total amount of items.</param>
    /// <param name="duration">Elapsed time from start of the progress.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="spinner"/> is out of [2, 42] range;
    ///     or <paramref name="progressBars"/> is out of [4, 42] range.</exception>
    public ProgressViewModel(
            ulong count,
            IEnumerable<char> spinner,
            IEnumerable<char> progressBars,
            ulong? totalCount = null,
            TimeSpan? duration = null)
    {
        this.Count = count;
        this.Total = totalCount.HasValue ? Math.Max(count, totalCount.Value) : totalCount;
        this.Elapsed = Ensure.Optional(duration)
                .NonNegative()
                .ValueOr(TimeSpan.Zero);
        this.spinner = Ensure.Collection(spinner).InRange(2, 42).ToImmutableArray();
        this.progressBars = Ensure.Collection(progressBars).InRange(4, 42).ToImmutableArray();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressViewModel"/> class.
    /// </summary>
    /// <param name="progress">Progress source, values are clipped if required.</param>
    /// <param name="spinner">Spinner containing consecutively displayed characters.</param>
    /// <param name="progressBars">Progress bars characters, first and last
    ///     items are treated as bounding characters. Inside items
    ///     are treated as characters representing empty to full state (thus minimu 2).
    ///     E.g.: ['(', ' ', '.', '_', ')'] will result in 50% progress displayed as:
    ///     "(____.   )".</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="spinner"/> is out of [2, 42] range;
    ///     or <paramref name="progressBars"/> is out of [4, 42] range.</exception>
    public ProgressViewModel(
            TransparentProgress<long> progress,
            ImmutableArray<char> spinner,
            ImmutableArray<char> progressBars)
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
        this.progressBars = Ensure.Collection(progressBars).InRange(4, 42).ToImmutableArray();
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

            return $"{counts} {this.GetProgressBars(this.Ratio, 12)}";
        }

        // just count
        return $"{this.Count}";
    }

    private string GetProgressBars(
            double ratio,
            ushort barWidth)
    {
        ratio = ratio < 0.0 ? 0.0 : ratio;
        ratio = ratio > 1.0 ? 1.0 : ratio;
        int effectiveLength = this.progressBars.Length - 2;
        int fill = (int)Math.Round(barWidth * effectiveLength * ratio, MidpointRounding.AwayFromZero);
        int full = fill / effectiveLength;
        int partial = fill % effectiveLength;

        // add opening boundary
        StringBuilder sb = new StringBuilder()
                .Append(this.progressBars[0]);

        // fill with full characters
        for (int i = 0; i < barWidth; i++)
        {
            if (i < full)
            {
                sb = sb.Append(this.progressBars[^2]);
            }
        }

        // pad with partial fraction character
        if (sb.Length < barWidth)
        {
            sb = sb.Append(this.progressBars[1 + partial]);
        }

        // pad with empty characters
        while (sb.Length < barWidth)
        {
            sb = sb.Append(this.progressBars[1]);
        }

        // add closing boundary
        sb = sb.Append(this.progressBars[^1]);

        return sb.ToString();
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
