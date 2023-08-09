namespace Radicle.Common;

using System;
using Radicle.Common.Check;
using Radicle.Common.Extensions;

/// <summary>
/// Simple implementation of <see cref="IPeriodSource"/>
/// with ramp up option.
/// </summary>
public sealed class SimplePeriodSource : IPeriodSource
{
    private static readonly TimeSpan MaxStartingPeriod = TimeSpan.FromMinutes(2.0);

    private long counter;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimplePeriodSource"/> class.
    /// </summary>
    /// <param name="period">Positive period of this instance.</param>
    /// <param name="rampUp">Flag signaling that the first
    ///     couple of periods should ramp up from a lower value
    ///     to given <paramref name="period"/>. This is usefull
    ///     when period is too long to wait the whole for the first time
    ///     as <see cref="Metronome.IsTime"/> will return success only after
    ///     first <paramref name="period"/> is passed.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="period"/> is zero or negative.</exception>
    public SimplePeriodSource(
            TimeSpan period,
            bool rampUp = false)
    {
        this.Period = Ensure.Param(period).StrictlyPositive().Value;
        this.RampUp = rampUp;

        this.Reset();
    }

    /// <summary>
    /// Gets time period of this instance.
    /// </summary>
    public TimeSpan Period { get; }

    /// <summary>
    /// Gets a value indicating whether the first initial periods
    /// ramp up towards <see cref="Period"/> instead of just beeing <see cref="Period"/>.
    /// </summary>
    public bool RampUp { get; }

    /// <inheritdoc/>
    public TimeSpan Current { get; private set; }

    /// <inheritdoc/>
    public TimeSpan MoveNext()
    {
        this.counter++;

        // only ramp up for few first values
        if (this.counter <= 6)
        {
            if (this.counter == 1)
            {
                this.Current = this.RampUp
                        ? (this.Period / 4.0).UseIfShorterOr(MaxStartingPeriod)
                        : this.Period;
            }
            else if (this.counter == 6)
            {
                this.Current = this.Period;
            }
            else
            {
                this.Current = (this.Current * 2.0)
                        .UseIfShorterOr(this.Period);
            }
        }

        return this.Current;
    }

    /// <inheritdoc/>
    public void Reset()
    {
        this.counter = 0;
        this.Current = default;
    }
}
