namespace Radicle.Common.Profiling;

using System;
using System.Threading;
using Radicle.Common.Extensions;
using Radicle.Common.MetaData;
using Radicle.Common.Profiling.Models;

/// <summary>
/// Utility to retrieve performance counters in more
/// understandable form. Inspired by
/// https://github.com/StackExchange/StackExchange.Redis/blob/ae6419a164600b4b54dd7ce8a699efe8e98d8f1c/src/StackExchange.Redis/PerfCounterHelper.cs .
/// </summary>
[Experimental("Experimental use only")]
public static class PerformanceCounters
{
    private static readonly TimeSpan MaxAge = TimeSpan.FromMilliseconds(500);

    private static ThreadCounters completionPortValue = ThreadCounters.Empty;

    private static ThreadCounters workerValue = ThreadCounters.Empty;

    private static DateTime lastSample = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1));

    /// <summary>
    /// Gets thread counters for completion port -
    /// asynchronous I/O threads in the thread pool.
    /// </summary>
    public static ThreadCounters CompletionPort
    {
        get
        {
            Sample();

            return completionPortValue;
        }
    }

    /// <summary>
    /// Gets thread counters for worker threads in the thread pool.
    /// </summary>
    public static ThreadCounters Worker
    {
        get
        {
            Sample();

            return workerValue;
        }
    }

    private static void Sample()
    {
        if (lastSample.IsOlderThan(MaxAge))
        {
            lastSample = DateTime.UtcNow;

            ThreadPool.GetAvailableThreads(out int availableWorkerThreads, out int availableCompletionPortThreads);
            ThreadPool.GetMinThreads(out int minWorkerThreads, out int minCompletionPortThreads);
            ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxCompletionPortThreads);

            completionPortValue = new ThreadCounters(
                    (uint)availableCompletionPortThreads,
                    (uint)minCompletionPortThreads,
                    (uint)maxCompletionPortThreads);

            workerValue = new ThreadCounters(
                    (uint)availableWorkerThreads,
                    (uint)minWorkerThreads,
                    (uint)maxWorkerThreads);
        }
    }
}
