namespace Radicle.Common.Profiling;

using System;
using Radicle.Common.Profiling.Models;

/// <summary>
/// Interface for a service which produces profiling events.
/// </summary>
public interface IProfilableByEvent
{
    /// <summary>
    /// Event handler for events.
    /// </summary>
    event EventHandler<ProfiledEventArgs>? ProfileEventOccured;
}
