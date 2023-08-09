namespace Radicle.Common;

using System;
using Radicle.Common.Check;
using Radicle.Common.Extensions;
using Radicle.Common.MetaData;

/// <summary>
/// ETA (Estimated Time of Arrival) calculator.
/// </summary>
[Experimental("May change.")]
public sealed class ETA
{
    private readonly object operationLock = new();

    private readonly ISpeedCalcStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="ETA"/> class.
    /// </summary>
    /// <param name="startDate">Start date for the estimated
    /// time of arrival calculation. Defaults to time of creation.</param>
    /// <param name="total">Optional total amount of items.</param>
    /// <param name="strategy">Optional strategy, defaults to weighted
    ///     moving average which should cover most cases
    ///     with focus on creating optimal prediction according
    ///     to the most recet data.</param>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="startDate"/> is not UTC.</exception>
    public ETA(
            DateTime? startDate = null,
            long? total = null,
            ISpeedCalcStrategy? strategy = null)
    {
        this.strategy = strategy ?? new WeightedAverageSpeedCalculation();

        this.strategy.Init(startDate ?? DateTime.UtcNow);

        this.Total = total;
    }

    /// <summary>
    /// Gets UTC based start date.
    /// </summary>
    public DateTime StartDate => this.strategy.StartDate;

    /// <summary>
    /// Gets total amount of items if any.
    /// </summary>
    public long? Total { get; private set; }

    /// <summary>
    /// Gets UTC based last date for <see cref="LastCount"/>.
    /// See <see cref="Report(DateTime, long)"/>.
    /// </summary>
    public DateTime LastDate => this.strategy.LastDate;

    /// <summary>
    /// Gets current count.
    /// See <see cref="Report(DateTime, long)"/>.
    /// </summary>
    public long LastCount => this.strategy.LastCount;

    /// <summary>
    /// Creates a new instance of the <see cref="ETA"/> class
    /// from transparnet progress instance.
    /// This initialization is easiest as it works out of the box with no
    /// additional calls required as it monitors directly
    /// the <paramref name="progress"/> via events.
    /// </summary>
    /// <param name="progress">Progress to monitor.</param>
    /// <param name="strategy">Optional strategy, defaults to weighted moving average.</param>
    /// <returns>New instance of <see cref="ETA"/>
    ///     wired to <paramref name="progress"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static ETA FromProgress(
            ITransparentProgress<long> progress,
            ISpeedCalcStrategy? strategy = null)
    {
        Ensure.Param(progress).Done();

        ETA result = new(
                progress.StartDate,
                progress.Total,
                strategy: strategy);

        progress.ProgressChanged += (object _, ProgressEventArgs<long> e) =>
        {
            result.Report(e.Date, e.Count);
            result.SetTotal(e.Total);
        };

        return result;
    }

    /// <summary>
    /// Set total value or clear one if <see langword="null"/>.
    /// </summary>
    /// <param name="total">New total value
    ///     overriding any previous one.</param>
    public void SetTotal(long? total)
    {
        if (this.Total == total)
        {
            return;
        }

        lock (this.operationLock)
        {
            this.Total = total;
        }
    }

    /// <summary>
    /// Report sample. Reported sample with date in past
    /// relative to <see cref="LastDate"/> will be discarted.
    /// </summary>
    /// <param name="sampleDate">UTC Date of the sample.</param>
    /// <param name="count">Reported count.</param>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="sampleDate"/> is not UTC.</exception>
    public void Report(DateTime sampleDate, long count)
    {
        this.strategy.Report(sampleDate, count);
    }

    /// <summary>
    /// Returns current eta calculated according to
    /// value of the <see cref="LastCount"/>,
    /// <see cref="Total"/>, <see cref="StartDate"/>,
    /// strategy and current time.
    /// </summary>
    /// <returns>Instance of <see cref="ETAStatus"/>.</returns>
    public ETAStatus GetETA()
    {
        if (this.Total is null)
        {
            return new ETAStatus()
            {
                LastCount = this.LastCount,
                LastDate = this.LastDate,
                Total = this.Total,
                StatusDate = DateTime.UtcNow,
                StartDate = this.StartDate,
            };
        }

        lock (this.operationLock)
        {
            if (this.Total is null)
            {
                return new ETAStatus()
                {
                    LastCount = this.LastCount,
                    LastDate = this.LastDate,
                    Total = this.Total,
                    StatusDate = DateTime.UtcNow,
                    StartDate = this.StartDate,
                };
            }

            long total = this.Total.Value;
            long count = this.LastCount;
            long pending = total - count;
            DateTime date = this.LastDate;
            double speedPerSecods = this.strategy.GetSpeed();
            DateTime now = DateTime.UtcNow;
            TimeSpan eta;

            if (pending == 0)
            {
                eta = TimeSpan.Zero;
            }
            else if (speedPerSecods == 0.0)
            {
                eta = TimeSpan.MaxValue;
            }
            else
            {
                double etaSeconds = pending / speedPerSecods;
                TimeSpan alreadyPassed = now - date;

                try
                {
                    eta = TimeSpan.FromSeconds(etaSeconds - alreadyPassed.TotalSeconds);
                    eta = eta.Clamp(TimeSpan.Zero, TimeSpan.MaxValue);
                }
                catch (OverflowException)
                {
                    eta = TimeSpan.MaxValue;
                }
            }

            return new ETAStatus()
            {
                LastCount = count,
                LastDate = date,
                Total = total,
                ETA = eta,
                Speed = speedPerSecods,
                StatusDate = now,
                StartDate = this.StartDate,
            };
        }
    }
}
