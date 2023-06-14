namespace Radicle.Common.Profiling;

using System;
using System.Diagnostics;
using Radicle.Common.Check;

/// <summary>
/// Auxiliarly class for measuring a series of time counters
/// in style of the <see cref="Stopwatch"/>. This class is not thread safe.
/// </summary>
public sealed class TimeThread
{
    private readonly CountersDictionary countersCollection;

    private readonly Stopwatch stopwatch;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeThread"/> class.
    /// </summary>
    /// <param name="countersCollection">Counters collection
    ///     to modify.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public TimeThread(
            CountersDictionary countersCollection)
    {
        this.countersCollection = Ensure.Param(countersCollection).Value;
        this.stopwatch = Stopwatch.StartNew();
    }

    /// <summary>
    /// Measure elapsed time span till this point in time from
    /// the creation of this counting thread and or last call
    /// to <see cref="Measure(string)"/> or <see cref="Restart"/>.
    /// </summary>
    /// <param name="key">Counter key to measure for.</param>
    /// <returns>Total elapsed time for counter of given <paramref name="key"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public TimeSpan Measure(
            string key)
    {
        TimeSpan result = this.countersCollection
                .Incr(key, this.stopwatch.Elapsed);

        this.stopwatch.Restart();

        return result;
    }

    /// <summary>
    /// Restart measurement, e.g. after part which you
    /// want to exclude from the measurement.
    /// </summary>
    public void Restart()
    {
        this.stopwatch.Restart();
    }
}
