namespace Radicle.Common.Profiling;

using Radicle.Common.Profiling.Models;

/// <summary>
/// Interface defining an instance with metadata for profiling.
/// </summary>
/// <remarks>There is nothing magic about this, it is just
/// a good convention to keep services named in case
/// they need to be logged and or profiled.</remarks>
public interface INamedForProfiling
{
    /// <summary>
    /// Gets event source name for this instance
    /// for profiling use.
    /// </summary>
    public EventSourceName EventSourceName { get; }
}
