namespace Radicle.Common;

using System;

/// <summary>
/// Interface of a service which can contribute
/// to the progress indicator concurrently.
/// </summary>
/// <typeparam name="T">Type of the counter.</typeparam>
public interface IProgressContributor<T>
    where T : struct
{
    /// <summary>
    /// Increment reported count by given value.
    /// </summary>
    /// <param name="increment">Increment to
    ///     add to current count.</param>
    /// <returns>New value of <see cref="ITransparentProgress{T}.Count"/>.</returns>
    /// <exception cref="NotSupportedException">Thrown
    ///     if <typeparamref name="T"/> is not yet supported
    ///     numeric type.</exception>
    T IncrementCount(T increment);

    /// <summary>
    /// Increment reported total count by given value.
    /// </summary>
    /// <param name="increment">Increment to add
    ///     to current total amount, if total
    ///     amount is undefined it is initialized
    ///     to zero.</param>
    /// <returns>New value of <see cref="ITransparentProgress{T}.Total"/>.</returns>
    /// <exception cref="NotSupportedException">Thrown
    ///     if <typeparamref name="T"/> is not yet supported
    ///     numeric type.</exception>
    T IncrementTotal(T increment);
}
