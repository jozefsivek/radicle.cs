namespace Radicle.Common;

using System;
using Radicle.Common.Check;

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
    private string statusValue = string.Empty;

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

    /// <summary>
    /// Gets or sets small descriptive human readable
    /// one-line status text of no more than 1024 characters.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if set value contains new lines.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if set value is longer that 1024 characters.</exception>
    public string Status
    {
        get => this.statusValue;
        set => this.statusValue = Ensure.Param(value)
                .NoNewLines()
                .InRange(0, 1024)
                .Value;
    }

    /// <inheritdoc/>
    public void Report(T value)
    {
        this.Count = value;
    }
}
