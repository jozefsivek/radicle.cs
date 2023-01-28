namespace Radicle.Common.Profiling;

using System;
using Radicle.Common.Profiling.Models;

/// <summary>
/// Interface defining service which allows on-demand profiling by session.
/// </summary>
public interface IProfilableBySession
{
    /// <summary>
    /// Set session factory.
    /// </summary>
    /// <param name="factory">Factory for <see cref="ProfilingSession"/>
    ///     which should be used to record events, if factory
    ///     produces <see langword="null"/> the events will not be recorded.</param>
    /// <exception cref="ArgumentException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    void SetProfiler(Func<ProfilingSession?> factory);

    /// <summary>
    /// Unset any set session factory for profiling.
    /// </summary>
    /// <returns><see langword="true"/> if factory was unset;
    ///     <see langword="false"/> if no factory was set.</returns>
    bool UnsetProfiler();
}
