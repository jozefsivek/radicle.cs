namespace Radicle.Common.Profiling.Models;

/// <summary>
/// Enumeraion of the event continuity.
/// </summary>
public enum EventContinuity
{
    /// <summary>
    /// Event with no preceeding of following event.
    /// </summary>
    StandAlone = 0,

    /// <summary>
    /// First event in the chain.
    /// </summary>
    First = 1,

    /// <summary>
    /// Intermediate event in the chain.
    /// </summary>
    Intermediate = 2,

    /// <summary>
    /// Last event in the chain.
    /// </summary>
    Last = 3,
}
