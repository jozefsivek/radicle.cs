namespace Radicle.Common;

using System;
using Radicle.Common.MetaData;

/// <summary>
/// Immutable representation of ETA (Estimated Time of Arrival) status.
/// </summary>
[Experimental("May change.")]
public sealed class ETAStatus
{
    /// <summary>
    /// Gets calculated estimated time of arival (ETA), if possible.
    /// E.g. missing <see cref="Total"/> will lead to failue to compute ETA.
    /// The returned value is non negative. Impossible
    /// real world conditions will result in zero ETA (negative speed, etc.).
    /// Zero speed will result in ETA equal to <see cref="TimeSpan.MaxValue"/>
    /// (you can treat this value as infinite if you desire).
    /// </summary>
    public TimeSpan? ETA { get; init; }

    /// <summary>
    /// Gets UTC date this status and <see cref="ETA"/>
    /// was calculated relative to.
    /// </summary>
    public DateTime StatusDate { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets speed of counts per second.
    /// </summary>
    public double Speed { get; init; }

    /// <summary>
    /// Gets elapsed time from <see cref="StartDate"/>
    /// till <see cref="StatusDate"/>.
    /// </summary>
    public TimeSpan Elapsed => this.StatusDate - this.StartDate;

    /// <summary>
    /// Gets UTC based start date.
    /// </summary>
    public DateTime StartDate { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets UTC time when
    /// <see cref="LastCount"/> was reported.
    /// </summary>
    public DateTime LastDate { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets count.
    /// </summary>
    public long LastCount { get; init; }

    /// <summary>
    /// Gets total count if available.
    /// </summary>
    public long? Total { get; init; }
}
