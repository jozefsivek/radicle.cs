namespace Radicle.Common.Profiling.Models;

using System;
using System.Collections.Immutable;

/// <summary>
/// Interface with a definition of base properties of any profiling event.
/// Note these events are intended to be immutable and can represent variety of things
/// (command, event, query, etc.).
/// </summary>
public interface IProfilingEvent
{
    /// <summary>
    /// Gets UTC date time of event creation.
    /// </summary>
    public DateTime CreatedOn { get; }

    /// <summary>
    /// Gets the previous event if any, this is the case for continuation events.
    /// Previous events share set of common properties (see description
    /// of other properties refering to event chain).
    /// </summary>
    public IProfilingEvent? Previous { get; }

    /// <summary>
    /// Gets the first event in this event chain or this event.
    /// </summary>
    public IProfilingEvent First { get; }

    /// <summary>
    /// Gets elapsed time from the first event in the event chain.
    /// </summary>
    public TimeSpan Elapsed { get; }

    /// <summary>
    /// Gets this event intended continuity.
    /// </summary>
    public EventContinuity Continuity { get; }

    /// <summary>
    /// Gets severity of this event or an event chain.
    /// </summary>
    public EventSeverity Severity { get; }

    /// <summary>
    /// Gets event or event chain source name.
    /// </summary>
    public EventSourceName SourceName { get; }

    /// <summary>
    /// Gets the event or event chain category name.
    /// It can be used to group events.
    /// </summary>
    public EventCategoryName CategoryName { get; }

    /// <summary>
    /// Gets human readable message and or message template if any.
    /// See https://messagetemplates.org/ and https://github.com/NLog/NLog/wiki/How-to-use-structured-logging.
    /// The user of this profiling framework is in charge of using appropriate template.
    /// </summary>
    public string? Message { get; }

    /// <summary>
    /// Gets collection of arguments for template in <see cref="Message"/>.
    /// Follow common sense and best practices of structured logging,
    /// when filling them, use simple types etc. Empty if
    /// message is plain.
    /// </summary>
    public ImmutableArray<object?> Arguments { get; }

    /// <summary>
    /// Gets optional exception associated with this event.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Determines if the given <paramref name="other"/> instance
    /// is this event or is contained in the event chain
    /// by reference comparison.
    /// </summary>
    /// <param name="other">Event to search for.</param>
    /// <returns><see langword="true"/> if this event is <paramref name="other"/>
    ///     or it is one of the previous events; <see langword="false"/>
    ///     otherwise.</returns>
    public bool Contains(IProfilingEvent? other);
}
