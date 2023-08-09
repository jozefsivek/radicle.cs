namespace Radicle.Common;

using System;
using Radicle.Common.Check;
using Radicle.Common.MetaData;

/// <summary>
/// Average speed over total duration of event.
/// </summary>
[Experimental("May change")]
public sealed class AverageSpeedCalculation : ISpeedCalcStrategy
{
    private readonly object operationLock = new();

    private double? speed;

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

            long count = this.LastCount;
            double elapsedSeconds = (this.LastDate - this.StartDate).TotalSeconds;

            if (count < 0
                    || elapsedSeconds <= 0.0
                    || double.IsNaN(elapsedSeconds)
                    || double.IsInfinity(elapsedSeconds))
            {
                this.speed = 0.0;
            }
            else
            {
                this.speed = count / elapsedSeconds;
            }

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

            this.LastDate = sampleDate;
            this.LastCount = count;
            this.speed = null;
        }
    }
}
