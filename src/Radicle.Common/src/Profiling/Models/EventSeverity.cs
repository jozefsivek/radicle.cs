namespace Radicle.Common.Profiling.Models;

/// <summary>
/// Enumeration of severities. Values of this
/// enumeration can be numerically compared with higher
/// numerical value corresponding to higher severity.
/// Numerical values fit into the <see cref="byte"/> range.
/// </summary>
public enum EventSeverity
{
    /// <summary>
    /// Lowest level of severity. Use for most frequent profileable event.
    /// </summary>
    Trace = 0,

    /// <summary>
    /// Debug severity for debugging, still rather verbose.
    /// </summary>
    Debug = 1 << 2,

    /// <summary>
    /// Info severity for all essential events or events with no well defined severity.
    /// </summary>
    Info = (1 << 3) + (1 << 2),

    /// <summary>
    /// Warning severity for events pointing to potential issues.
    /// </summary>
    Warning = 1 << 5,

    /// <summary>
    /// Error severity for error events directly or inderectly
    /// influencig proper workings.
    /// </summary>
    Error = 1 << 7,

    /// <summary>
    /// Fatal severity level for ususally unrecoverable events.
    /// </summary>
    Fatal = (1 << 8) + (1 << 7),
}
