namespace Radicle.Common;

using System;

/// <summary>
/// Interface of all stash/collections for tasks completion.
/// </summary>
public interface ITaskStash
{
    /// <summary>
    /// Gets maximum capacity this instance can reach.
    /// </summary>
    int MaximumCapacity { get; }

    /// <summary>
    /// Gets or sets capacity of this instance,
    /// greater than zero. Capacity is amount
    /// of concurrently running tasks and or maximum
    /// number of enqueued tasks. Defaults to <see cref="MaximumCapacity"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if set to value equal to zero, negative
    ///     or greater than <see cref="MaximumCapacity"/>.</exception>
    int Capacity { get; set; }

    /// <summary>
    /// Gets amount of enqueue calls
    /// after which this instance will always indicate full capacity.
    /// Defaults to 0, which disables the feature.
    /// </summary>
    int CallCountToIndicateFull { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is
    /// at full capacity (<see cref="Capacity"/>) and or number of calls equals
    /// or exceeds the <see cref="CallCountToIndicateFull"/>.
    /// I.e. one or more of the enqueues tasks needs to be completed
    /// before enqueuing more.
    /// </summary>
    bool IsFull { get; }

    /// <summary>
    /// Gets a value indicating whether this instance
    /// is empty and tasks can be enqueued.
    /// </summary>
    bool IsEmpty { get; }
}
