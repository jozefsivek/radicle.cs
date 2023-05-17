namespace Radicle.Common;

using System;

/// <summary>
/// Single progress report.
/// </summary>
/// <typeparam name="T">Type of the count value.</typeparam>
public readonly struct ProgressReport<T> : IEquatable<ProgressReport<T>>
    where T : struct
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressReport{T}"/> struct.
    /// </summary>
    /// <param name="count">Report count.</param>
    public ProgressReport(T count)
    {
        this.Date = DateTime.UtcNow;
        this.Count = count;
    }

    /// <summary>
    /// Gets UTC time of the report creation.
    /// </summary>
    public DateTime Date { get; }

    /// <summary>
    /// Gets count associated with the report.
    /// </summary>
    public T Count { get; }

    /// <summary>
    /// Implicitly convert value to instance of <see cref="ProgressReport{T}"/>.
    /// </summary>
    /// <param name="t">Value to convert.</param>
    public static implicit operator ProgressReport<T>(T t)
    {
        return ToProgressReport(t);
    }

    /// <summary>
    /// Compare instances.
    /// </summary>
    /// <param name="left">Left argument.</param>
    /// <param name="right">Right argument.</param>
    /// <returns><see langword="true"/> if equal.</returns>
    public static bool operator ==(ProgressReport<T> left, ProgressReport<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Compare instances.
    /// </summary>
    /// <param name="left">Left argument.</param>
    /// <param name="right">Right argument.</param>
    /// <returns><see langword="true"/> if not equal.</returns>
    public static bool operator !=(ProgressReport<T> left, ProgressReport<T> right)
    {
        return !(left == right);
    }

#pragma warning disable CA1000 // Do not declare static members on generic types
    /// <summary>
    /// Convert given value to instance of <see cref="ProgressReport{T}"/>.
    /// </summary>
    /// <param name="t">Value to convert.</param>
    /// <returns>Instance of <see cref="ProgressReport{T}"/>.</returns>
    public static ProgressReport<T> ToProgressReport(T t)
    {
        return new ProgressReport<T>(t);
    }
#pragma warning restore CA1000 // Do not declare static members on generic types

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is ProgressReport<T> report && this.Equals(report);
    }

    /// <inheritdoc/>
    public bool Equals(ProgressReport<T> other)
    {
        return this.Date == other.Date && Equals(this.Count, other.Count);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.Date, this.Count);
    }
}
