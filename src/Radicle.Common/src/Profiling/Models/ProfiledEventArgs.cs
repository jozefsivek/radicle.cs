namespace Radicle.Common.Profiling.Models;

using System;
using System.Collections.Immutable;
using Radicle.Common.Check;

/// <summary>
/// Base of profiled event for use in events.
/// </summary>
public class ProfiledEventArgs : EventArgs, IProfiledEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProfiledEventArgs"/> class.
    /// </summary>
    /// <param name="sourceName">Source name.</param>
    /// <param name="categoryName">Category name.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    internal ProfiledEventArgs(
            EventSourceName sourceName,
            EventCategoryName categoryName)
    {
        this.CreatedOn = DateTime.UtcNow;
        this.SourceName = Ensure.Param(sourceName).Value;
        this.CategoryName = Ensure.Param(categoryName).Value;
    }

    /// <inheritdoc />
    public DateTime CreatedOn { get; }

    /// <inheritdoc />
    public IProfiledEvent First => this.Previous?.First ?? this;

    /// <inheritdoc />
    public TimeSpan Elapsed
    {
        get
        {
            if (this.Previous is null)
            {
                return TimeSpan.Zero;
            }

            return this.CreatedOn - this.First.CreatedOn;
        }
    }

    /// <inheritdoc />
    public virtual IProfiledEvent? Previous { get; init; }

    /// <inheritdoc />
    public virtual EventSeverity Severity { get; init; }

    /// <inheritdoc />
    public virtual EventSourceName SourceName { get; }

    /// <inheritdoc />
    public virtual EventCategoryName CategoryName { get; }

    /// <inheritdoc />
    public virtual string? Message { get; init; }

    /// <inheritdoc />
    public virtual EventContinuity Continuity { get; init; }

    /// <inheritdoc />
    public virtual ImmutableArray<object?> Arguments { get; init; } = ImmutableArray<object?>.Empty;

    /// <inheritdoc />
    public Exception? Exception { get; init; }

    /// <summary>
    /// Construct new stand alone instance of <see cref="ProfiledEventArgs"/>.
    /// </summary>
    /// <param name="instance">Instance of <see cref="INamedForProfiling"/>.</param>
    /// <param name="severity">Severity of event.</param>
    /// <param name="exception">Optional exception.</param>
    /// <param name="category">Category of event.</param>
    /// <param name="message">Optional message or message template.</param>
    /// <param name="args">Optional arguments for <paramref name="message"/> template.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <returns>Instance of <see cref="ProfiledEventArgs"/>.</returns>
    public static ProfiledEventArgs From(
            INamedForProfiling instance,
            EventSeverity severity,
            Exception? exception,
            EventCategoryName category,
            string? message,
            params object?[] args)
    {
        Ensure.Param(instance).Done();

        return new ProfiledEventArgs(
                instance.EventSourceName,
                category)
        {
            Arguments = args?.Length > 0 ? args.ToImmutableArray() : ImmutableArray<object?>.Empty,
            Severity = severity,
            Continuity = EventContinuity.StandAlone,
            Exception = exception,
            Message = message,
        };
    }

    /// <summary>
    /// Construct new fisrt instance of <see cref="ProfiledEventArgs"/>
    /// in a chain of events.
    /// </summary>
    /// <param name="instance">Instance of <see cref="INamedForProfiling"/>.</param>
    /// <param name="severity">Severity of event.</param>
    /// <param name="exception">Optional exception.</param>
    /// <param name="category">Category of event.</param>
    /// <param name="message">Optional message or message template.</param>
    /// <param name="args">Optional arguments for <paramref name="message"/> template.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <returns>Instance of <see cref="ProfiledEventArgs"/>.</returns>
    public static ProfiledEventArgs StartFrom(
            INamedForProfiling instance,
            EventSeverity severity,
            Exception? exception,
            EventCategoryName category,
            string? message,
            params object?[] args)
    {
        Ensure.Param(instance).Done();

        return new ProfiledEventArgs(
                instance.EventSourceName,
                category)
        {
            Arguments = args?.Length > 0 ? args.ToImmutableArray() : ImmutableArray<object?>.Empty,
            Severity = severity,
            Continuity = EventContinuity.First,
            Exception = exception,
            Message = message,
        };
    }

    /// <summary>
    /// Construct continuation instance of <see cref="ProfiledEventArgs"/>
    /// in a chain of events.
    /// </summary>
    /// <param name="previous">Event to continue from.</param>
    /// <param name="exception">Optional exception.</param>
    /// <param name="message">Optional message or message template,
    ///     if left <see langword="null"/> the previous even message is used.</param>
    /// <param name="args">Optional arguments for <paramref name="message"/> template,
    ///     if left empty the previous even message is used.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <returns>Instance of <see cref="ProfiledEventArgs"/>.</returns>
    public static ProfiledEventArgs Continue(
            IProfiledEvent previous,
            Exception? exception,
            string? message,
            params object?[] args)
    {
        Ensure.Param(previous).Done();

        return new ProfiledEventArgs(
                previous.SourceName,
                previous.CategoryName)
        {
            Arguments = args?.Length > 0 ? args.ToImmutableArray() : previous.Arguments,
            Severity = previous.Severity,
            Continuity = EventContinuity.Intermediate,
            Exception = exception,
            Message = message ?? previous.Message,
            Previous = previous,
        };
    }

    /// <summary>
    /// Construct continuation instance of <see cref="ProfiledEventArgs"/>
    /// in a chain of events.
    /// </summary>
    /// <param name="previous">Event to continue from.</param>
    /// <param name="exception">Optional exception.</param>
    /// <param name="message">Optional message or message template,
    ///     if left <see langword="null"/> the previous even message is used.</param>
    /// <param name="args">Optional arguments for <paramref name="message"/> template,
    ///     if left empty the previous even message is used.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <returns>Instance of <see cref="ProfiledEventArgs"/>.</returns>
    public static ProfiledEventArgs End(
            IProfiledEvent previous,
            Exception? exception,
            string? message,
            params object?[] args)
    {
        Ensure.Param(previous).Done();

        return new ProfiledEventArgs(
                previous.SourceName,
                previous.CategoryName)
        {
            Arguments = args?.Length > 0 ? args.ToImmutableArray() : previous.Arguments,
            Severity = previous.Severity,
            Continuity = EventContinuity.Last,
            Exception = exception,
            Message = message ?? previous.Message,
            Previous = previous,
        };
    }

    /// <inheritdoc />
    public bool Contains(IProfiledEvent? other)
    {
        return ReferenceEquals(this, other) || (this.Previous?.Contains(other) ?? false);
    }
}
