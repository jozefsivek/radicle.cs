namespace Radicle.Common;

using System;
using Radicle.Common.Check;

/// <summary>
/// Event arguments for progress reporting.
/// </summary>
/// <typeparam name="T">Type of the counter.</typeparam>
public sealed class ProgressEventArgs<T> : EventArgs
    where T : struct
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressEventArgs{T}"/> class.
    /// </summary>
    /// <param name="date">UTC date of the <paramref name="count"/> change,
    ///     a later change to <paramref name="total"/> will
    ///     maintain the same date.</param>
    /// <param name="count">Current count.</param>
    /// <param name="total">Total count if available.</param>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="date"/> is not UTC.</exception>
    public ProgressEventArgs(
            DateTime date,
            T count,
            T? total = null)
    {
        this.Date = Ensure.Param(date).IsUTC().Value;
        this.Count = count;
        this.Total = total;
    }

    /// <summary>
    /// Gets UTC time of the <see cref="Count"/>
    /// change.
    /// </summary>
    public DateTime Date { get; }

    /// <summary>
    /// Gets current progress count at given <see cref="Date"/>.
    /// </summary>
    public T Count { get; }

    /// <summary>
    /// Gets total count if available.
    /// </summary>
    public T? Total { get; }
}
