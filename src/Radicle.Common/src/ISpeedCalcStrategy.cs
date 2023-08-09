namespace Radicle.Common;

using System;
using Radicle.Common.MetaData;

/// <summary>
/// Interface of speed calculation strategy algorithm.
/// </summary>
/// <remarks>
/// The implementations of this insterface should be thread
/// safe as they are use in potentially non-thread safe
/// cases with progress measurements.
/// </remarks>
[Experimental("May change")]
public interface ISpeedCalcStrategy
{
    /// <summary>
    /// Gets UTC based start date.
    /// See <see cref="Init(DateTime)"/>.
    /// </summary>
    DateTime StartDate { get; }

    /// <summary>
    /// Gets UTC based last date for <see cref="LastCount"/>.
    /// See <see cref="Report(DateTime, long)"/>.
    /// </summary>
    DateTime LastDate { get; }

    /// <summary>
    /// Gets current count.
    /// See <see cref="Report(DateTime, long)"/>.
    /// </summary>
    long LastCount { get; }

    /// <summary>
    /// Report first sample with implicit count equal to zero.
    /// </summary>
    /// <param name="startDate">UTC start date used for speed calculation.</param>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="startDate"/> is not UTC.</exception>
    public void Init(DateTime startDate);

    /// <summary>
    /// Report sample. Reported sample with date in past
    /// relative to <see cref="LastDate"/> will be discarted.
    /// </summary>
    /// <param name="sampleDate">UTC Date of the sample.</param>
    /// <param name="count">Reported count.</param>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="sampleDate"/> is not UTC.</exception>
    public void Report(DateTime sampleDate, long count);

    /// <summary>
    /// Gets or compute the current calculated speed (count/s)
    /// according to the implementation and reported
    /// samples.
    /// </summary>
    /// <returns>Speed measured as count per second.</returns>
    public double GetSpeed();
}
