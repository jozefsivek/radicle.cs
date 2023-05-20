namespace Radicle.Common.Profiling.Models;

using System;
using Radicle.Common.Check;

/// <summary>
/// Immutable representation of dotnet thread counters.
/// </summary>
public class ThreadCounters
{
    /// <summary>
    /// Get empty (zero) counters.
    /// </summary>
    public static readonly ThreadCounters Empty = new(0, 0, 0);

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadCounters"/> class.
    /// </summary>
    /// <param name="available">Available (free) threads.</param>
    /// <param name="min">Minimum threads.</param>
    /// <param name="max">Maximum threads.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="max"/> is lower than <paramref name="min"/>,
    ///     or <paramref name="available"/> is greater than <paramref name="max"/>.</exception>
    public ThreadCounters(
            uint available,
            uint min,
            uint max)
    {
        this.Available = Ensure.Param(available).LowerThanOrEqual(max).Value;
        this.Min = min;
        this.Max = Ensure.Param(max).GreaterThanOrEqual(min).Value;
        this.Busy = this.Max - this.Available;
    }

    /// <summary>
    /// Gets amount of busy threads.
    /// </summary>
    public uint Busy { get; }

    /// <summary>
    /// Gets amount of available (free) threads.
    /// </summary>
    public uint Available { get; }

    /// <summary>
    /// Gets minimum amount of available threads.
    /// </summary>
    public uint Min { get; }

    /// <summary>
    /// Gets maximum amount of available threads.
    /// </summary>
    public uint Max { get; }
}
