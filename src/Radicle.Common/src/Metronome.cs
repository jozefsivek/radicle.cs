namespace Radicle.Common;

using System;
using System.Threading.Tasks;
using Radicle.Common.Check;
using Radicle.Common.Extensions;

/// <summary>
/// Class for assisting with keeping a tempo of certain actions
/// by pulling method. If you need callbacks use official
/// <see cref="System.Threading.Timer"/>. This is not thread safe
/// class.
/// </summary>
/// <remarks>
/// <para>
/// The pulling is motivated by the way
/// how cancellation is handled and or how many times
/// user wants to do periodic actions in the loop.
/// </para>
/// <para>
/// The idea of use is the following. <see cref="Metronome"/>
/// keeps track of last "tick" and informs user if enough
/// time passed from the last "tick". When user is given green light,
/// the class registers it as "tick" and user can perform his action.
/// In this way the user polling should be a bit more frequent
/// than the anticipated tick interval. But no harm done if
/// done just as soon as possible, e.g. in a loop with work.
/// If you have a luxury to delay, use <see cref="DelayAsync"/> to wait
/// a reasonable time before polling <see cref="IsTime"/>.
/// </para>
/// </remarks>
public sealed class Metronome
{
    private static readonly TimeSpan MinDelay = TimeSpan.FromMilliseconds(1.0);

    private static readonly TimeSpan MaxDelay = TimeSpan.FromMinutes(1.0);

    private readonly IPeriodSource source;

    private DateTime lastTick;

    /// <summary>
    /// Initializes a new instance of the <see cref="Metronome"/> class.
    /// </summary>
    /// <param name="period">Positive period of this instance.</param>
    /// <param name="rampUp">Flag signaling that the first
    ///     couple of periods should ramp up from lower value
    ///     to given <paramref name="period"/>. This is usefull
    ///     when period is too long to wait the whole for the first time
    ///     as <see cref="IsTime"/> will return success only after
    ///     first <paramref name="period"/> is passed.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="period"/> is zero or negative.</exception>
    public Metronome(
            TimeSpan period,
            bool rampUp = false)
        : this(new SimplePeriodSource(period, rampUp: rampUp))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Metronome"/> class.
    /// </summary>
    /// <param name="source">Source of periods.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public Metronome(IPeriodSource source)
    {
        this.lastTick = DateTime.UtcNow;
        this.source = Ensure.Param(source).Value;
        _ = this.source.MoveNext();
    }

    /// <summary>
    /// Create instance of <see cref="Metronome"/> to be used
    /// to keep tempo of writing update into the logs.
    /// The period is 15min and ramp-up is used.
    /// </summary>
    /// <returns>Instance of <see cref="Metronome"/>.</returns>
    public static Metronome CreateForLogging()
    {
        return new Metronome(TimeSpan.FromMinutes(15), rampUp: true);
    }

    /// <summary>
    /// Create instance of <see cref="Metronome"/> to be used
    /// to keep tempo of updating responsive CLI interface.
    /// The period is 250ms and ramp-up is not used.
    /// </summary>
    /// <returns>Instance of <see cref="Metronome"/>.</returns>
    public static Metronome CreateForLiveUpdate()
    {
        return new Metronome(TimeSpan.FromMilliseconds(250));
    }

    /// <summary>
    /// Determine if it is time for user action,
    /// i.e. if the metronome ticked.
    /// </summary>
    /// <returns><see langword="true"/> if user action can be performed
    /// because period has passed from the last time or creation
    /// - or - <see langword="false"/> if period has not yet passed.</returns>
    public bool IsTime()
    {
        if (this.lastTick.IsOlderThan(this.source.Current))
        {
            _ = this.source.MoveNext();

            this.lastTick = DateTime.UtcNow;

            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets exact time span to the next occurence of
    /// metronome "tick".
    /// </summary>
    /// <returns>Time to next "tick", can be zero.</returns>
    public TimeSpan TimeToNext()
    {
        return (this.source.Current - (DateTime.UtcNow - this.lastTick))
                .UseIfLongerOr(TimeSpan.Zero);
    }

    /// <summary>
    /// Await a time interval reasonable for next polling
    /// of <see cref="IsTime"/>.
    /// </summary>
    /// <returns>Awaitable task.</returns>
    public async Task DelayAsync()
    {
        TimeSpan waitTime = this.source.Current / 3.0;

        await Task.Delay(waitTime.Clamp(MinDelay, MaxDelay))
                .ConfigureAwait(false);
    }

    /// <summary>
    /// Reset state of this instance,
    /// bringing it to the state as after creation.
    /// </summary>
    public void Reset()
    {
        this.lastTick = DateTime.UtcNow;

        this.source.Reset();

        _ = this.source.MoveNext();
    }
}
