namespace Radicle.Common;

using System;

/// <summary>
/// Mutable implementation of <see cref="IProgress{T}"/>
/// which allows external inspection so it is not closed
/// and can be used by other code and not only original owner.
/// </summary>
/// <remarks>This class is open of inheritance as it may
/// be usefull in APIs.</remarks>
/// <typeparam name="T">Type of the counter.</typeparam>
public class TransparentProgress<T> : IProgress<T>
    where T : struct
{
    /// <summary>
    /// Gets UTC based start date of this progress
    /// this is by default set to creation time of this object.
    /// </summary>
    public DateTime StartDate { get; } = DateTime.UtcNow;

    /// <summary>
    /// Gets current count, this is value
    /// normally set by <see cref="Report(T)"/>.
    /// </summary>
    public T Count { get; private set; }

    /// <summary>
    /// Gets or sets total count is available.
    /// </summary>
    public T? Total { get; set; }

    /// <inheritdoc/>
    public void Report(T value)
    {
        this.Count = value;
    }
}
