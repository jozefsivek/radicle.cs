namespace Radicle.Common;

using System;
using System.Threading.Tasks;
using Radicle.Common.Base;

/// <summary>
/// Utility for buffering tasks and awaiting multiple of them.
/// This class is NOT thread safe.
/// </summary>
/// <remarks>The reaon for using <see cref="int"/> is easier
/// handling when used with collections, they notoriously have
/// <see cref="int"/> lengths.</remarks>
public sealed class TaskBuffer : TaskBufferBase<int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TaskBuffer"/> class.
    /// </summary>
    /// <param name="capacity">Capacity of this buffer.
    ///     Defaults to half of the processor count.</param>
    /// <param name="fullCallCount">Optional value for
    ///     <see cref="TaskBufferBase{TMetaData}.CallCountToIndicateFull"/>, you can use init as well.
    ///     Zero or negative value will disable call count based full state indication.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="capacity"/> is zero or negative.</exception>
    public TaskBuffer(
            int? capacity = null,
            int? fullCallCount = null)
        : base(capacity: capacity, fullCallCount: fullCallCount)
    {
    }

    /// <summary>
    /// Enqueue given <paramref name="task"/> if capacity was not reached.
    /// </summary>
    /// <param name="task">Task to enqueue.</param>
    /// <returns><see langword="true"/> if addition of <paramref name="task"/>
    ///     caused reach of capacity; <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown if required parameter
    ///     is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown
    ///     if the buffer is at full capacity and needs to be awaited.</exception>
    public bool Enqueue(Task task)
    {
        return this.Enqueue(task, 0);
    }

    /// <summary>
    /// Await all stored tasks serially and clear
    /// the buffer. Call before or at the time
    /// <see cref="Enqueue(Task)"/> returns <see langword="true"/>.
    /// </summary>
    /// <returns>Awaitable task.</returns>
    /// <exception cref="Exception">Thrown from the awaited
    ///     stored task.</exception>
    public async Task FlushAsync()
    {
        if (this.IsEmpty)
        {
            return;
        }

        try
        {
            foreach ((Task task, int _) in this.CurrentTasks())
            {
                await task.ConfigureAwait(false);
            }
        }
        finally
        {
            this.Flush();
        }
    }
}
