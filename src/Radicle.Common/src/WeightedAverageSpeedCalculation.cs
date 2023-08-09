namespace Radicle.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Radicle.Common.Check;
using Radicle.Common.MetaData;

/// <summary>
/// Average speed over select sample window of event
/// with weights, technically moving weighted average.
/// </summary>
/// <remarks>
/// <para>
/// A nice series of samples will produce good results
/// in any way. A very jagged plot will also works
/// as here we average speed over selected window at the end.
/// And because we use only tail of samples to calculate speed
/// the speed is always the "current" one in place of
/// value stained by the whole history.
/// </para>
/// <para>
/// However consider the following example, where there is
/// an actual need for form of low-pass filter.
/// </para>
/// <code>
/// Consider the following situation (why such weird one?
/// think of a parallel execution when multiple threads
/// "dump" count increments at ones):
///
///       A                                     * -> 7
///       |                                    * -> 6
///       |                   *  -> 5
/// count |                  *  -> 4
///       |    2            *  -> 3
///       |  1 *
///       |  *
///       *--+-|------------+|+----------------++|---->
///          1 2           10-12             20-21  time (s)
///
/// The overal actual speed is around 0.33 count/s.
///
/// If we calculate the speed in each segment
/// (between two adjacent samples). We get the following data:
///
/// - 1 count/s
/// - 1 count/s
/// - 0.1 count/s
/// - 1 count/s
/// - 1 count/s
/// - 0.125 count/s
/// - 1 count/s (last)
///
/// Let's say we take window to calculate running average:
///
/// - 1 count/s (window of 1 last speed segment)
/// - 0.56 count/s (2)
/// - 0.71 count/s (3)
/// - 0.78 count/s (4)
/// - 0.65 count/s (5)
/// - 0.70 count/s (6)
/// - 0.75 count/s (window of last 6 speed segments)
///
/// Pretty unstable, and nowhere near the "real" value.
/// Let's take a very simple low-pass filter in this form:
/// take a sample for speed calculation only ones around maximum
/// distance between original samples (~1.3x that):
///
///       A                                     * -> 8
///       |
///       |
///       |
/// count |                  *  -> 4
///       |
///       |   1
///       |   *
///       *---|--------------|------------------|------>
///           1              11                 22  time (s)
///
///            `------v------'
///            frequency threshold
///        (~ 1.3x maximum samples delta)
///
/// With this low pass filter wa can calculate speeds:
///
/// - 1 count/s (the beginning will be allways fiddly)
/// - 0.30 count/s
/// - 0.36 count/s
///
/// Even withouth averaging the proximity to observed speed
/// is much better, and in order to smooth the remaining
/// oscilations we can take moving average.
///
/// This is:
///
/// - simple as we are not using expensive curve fitting
/// - simple as we do not need to average values over certain
///   window
///
/// And the frequency threshold can be chosen automatically
/// by observing deltas between samples and picking the maximum one
/// over last period.
/// </code>
/// </remarks>
[Experimental("May change")]
public sealed class WeightedAverageSpeedCalculation : ISpeedCalcStrategy
{
    private readonly object operationLock = new();

    private readonly double[] forwardWeigths;

    private readonly RollingCollection<double> speedSegments;

    private readonly bool useFrequencyThreshold;

    private readonly bool autoFrequencyThreshold;

    private DateTime thresholdLastDate = DateTime.UtcNow;

    private long thresholdLastCount;

    private TimeSpan maximumSamplesDelta = TimeSpan.Zero;

    private TimeSpan frequencyThreshold;

    private double? speed;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeightedAverageSpeedCalculation"/> class.
    /// </summary>
    /// <param name="windowLength">Size of the window to average speeds.</param>
    /// <param name="weights">Weights, starting from last sample going backwards.
    ///     This enumeration can be shorter than <paramref name="windowLength"/>
    ///     in which case <paramref name="defaultWeight"/> is used to fill empty space.
    ///     The enumeration can be also longer, then the tail is not used.</param>
    /// <param name="defaultWeight">Default weight to use if <paramref name="weights"/>
    ///     does not contain value for specific sample.</param>
    /// <param name="frequencyThreshold">Optional frequency threshold,
    ///     defaults to auto, use zero or negative for switching it off.
    ///     Values reported below this threshold (i.e. too frequent)
    ///     will not be computed to speed for which
    ///     <paramref name="windowLength"/> and <paramref name="weights"/>
    ///     are applicable.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="windowLength"/> is zero.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="defaultWeight"/> is
    ///     or <paramref name="weights"/> contains
    ///     infinite or non numeric values.</exception>
    public WeightedAverageSpeedCalculation(
            byte windowLength,
            IEnumerable<double>? weights = null,
            double defaultWeight = 1.0,
            TimeSpan? frequencyThreshold = null)
    {
        Ensure.Param(windowLength).StrictlyPositive().Done();
        Ensure.Param(defaultWeight).HasActualValue().Done();
        Ensure.Optional(weights).AllNotNull(w => Ensure.Param(w).HasActualValue()).Done();

        double[] w = Enumerable.Range(0, windowLength)
                    .Select(_ => defaultWeight)
                    .ToArray();

        if (weights is not null)
        {
            int index = w.Length;

            foreach (double weight in weights)
            {
                index--;

                if (index < 0)
                {
                    break;
                }

                w[index] = weight;
            }
        }

        this.speedSegments = new RollingCollection<double>(windowLength);
        this.forwardWeigths = w;

        Ensure.Param(w.Sum(), nameof(weights)).HasActualValue().Done();

        if (frequencyThreshold.HasValue)
        {
            this.useFrequencyThreshold = frequencyThreshold > TimeSpan.Zero;
            this.frequencyThreshold = frequencyThreshold.Value;
        }
        else
        {
            this.useFrequencyThreshold = true;
            this.autoFrequencyThreshold = true;

            // seed thresholds with something, but not too long
            this.frequencyThreshold = TimeSpan.FromSeconds(1.3);
            this.maximumSamplesDelta = TimeSpan.FromSeconds(1.0);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeightedAverageSpeedCalculation"/> class.
    /// This is default reasonable setup with window of
    /// size 4 and exponential weights with multiplier of 2.
    /// </summary>
    public WeightedAverageSpeedCalculation()
        : this(
              16,
              new double[] { 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 2, 2, 2, 1, 1 },
              defaultWeight: 0.0)
    {
    }

    /// <inheritdoc/>
    public DateTime StartDate { get; private set; } = DateTime.UtcNow;

    /// <inheritdoc/>
    public DateTime LastDate { get; private set; } = DateTime.UtcNow;

    /// <inheritdoc/>
    public long LastCount { get; private set; }

    /// <inheritdoc/>
    public double GetSpeed()
    {
        double? speed = this.speed;

        if (speed.HasValue)
        {
            return speed.Value;
        }

        lock (this.operationLock)
        {
            if (this.speed.HasValue)
            {
                return this.speed.Value;
            }

            double s = 0.0;
            double norm = 0;
            int index = this.forwardWeigths.Length - this.speedSegments.Count;

            foreach (double speedSegment in this.speedSegments)
            {
                norm += this.forwardWeigths[index];
                s += speedSegment * this.forwardWeigths[index++];
            }

            if (norm != 0.0 && double.IsFinite(norm))
            {
                s /= norm;
            }

            this.speed = s;

            return this.speed.Value;
        }
    }

    /// <inheritdoc/>
    public void Init(DateTime startDate)
    {
        Ensure.Param(startDate).IsUTC().Done();

        lock (this.operationLock)
        {
            this.StartDate = startDate;

            if (this.speedSegments.Count == 0)
            {
                this.LastDate = startDate;
                this.thresholdLastDate = startDate;
            }
        }
    }

    /// <inheritdoc/>
    public void Report(DateTime sampleDate, long count)
    {
        Ensure.Param(sampleDate).IsUTC().Done();

        if (count == this.LastCount && sampleDate == this.LastDate)
        {
            return;
        }

        lock (this.operationLock)
        {
            // discard samples from past
            if (sampleDate <= this.LastDate)
            {
                return;
            }

            // Compute new maximum delta between samples
            // but only ones we have at least one speed segment
            // so that LastDate is not just StartDate
            if (this.useFrequencyThreshold
                    && this.autoFrequencyThreshold
                    && this.speedSegments.Count > 0)
            {
                TimeSpan delta = sampleDate - this.LastDate;

                if (delta > this.maximumSamplesDelta)
                {
                    this.frequencyThreshold = delta * 1.3;
                }
                else
                {
                    // slowly fade away old value to give chance to correct trends
                    this.maximumSamplesDelta *= 0.999;
                }
            }

            this.LastDate = sampleDate;
            this.LastCount = count;

            // low-pass filter
            if (this.useFrequencyThreshold
                    && ((sampleDate - this.thresholdLastDate) < this.frequencyThreshold))
            {
                return;
            }

            double speedSegment;
            long countSegment = count - this.thresholdLastCount;
            double elapsedSeconds = (sampleDate - this.thresholdLastDate).TotalSeconds;

            if (countSegment <= 0 || elapsedSeconds <= 0.0)
            {
                speedSegment = 0.0;
            }
            else
            {
                speedSegment = countSegment / elapsedSeconds;
            }

            this.speedSegments.Add(speedSegment);

            // reset speed
            this.speed = null;
            this.thresholdLastCount = count;
            this.thresholdLastDate = sampleDate;
        }
    }
}
