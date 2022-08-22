namespace Radicle.CLI.Models;

using System;
using System.Threading.Tasks;
using Radicle.Common;
using Radicle.Common.Check;

/// <summary>
/// Promise of <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Type of the evaluation output payload.</typeparam>
public sealed class EvaluationRawOutputPromise<T>
    where T : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EvaluationRawOutputPromise{T}"/> class.
    /// </summary>
    /// <param name="promise">Awaitable task
    ///     yielding <typeparamref name="T"/>.</param>
    /// <param name="progress">Optiona progress instance.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public EvaluationRawOutputPromise(
            Task<T> promise,
            TransparentProgress<long>? progress = null)
    {
        this.Promise = Ensure.Param(promise).Value;
        this.Progress = progress ?? new TransparentProgress<long>();
    }

    /// <summary>
    /// Gets progress of the promise.
    /// </summary>
    public TransparentProgress<long> Progress { get; }

    /// <summary>
    /// Gets promise, awaitable task, yielding <typeparamref name="T"/>.
    /// </summary>
    public Task<T> Promise { get; }

#pragma warning disable CA1000 // Do not declare static members on generic types
    /// <summary>
    /// Implicit conversion from the instance of <see cref="EvaluationRawOutputPromise{T}"/>.
    /// </summary>
    /// <param name="payload">Execution output to convert to fulfilled promisse.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static implicit operator EvaluationRawOutputPromise<T>(T payload)
    {
        return ToEvaluationRawOutputPromise(payload);
    }

    /// <summary>
    /// Conversion from the instance of <see cref="EvaluationRawOutputPromise{T}"/>.
    /// </summary>
    /// <param name="payload">Output to convert to fulfilled promisse.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <returns>Instance of <see cref="EvaluationRawOutputPromise{T}"/>.</returns>
    public static EvaluationRawOutputPromise<T> ToEvaluationRawOutputPromise(
            T payload)
    {
        return new EvaluationRawOutputPromise<T>(Task.FromResult(Ensure.Param(payload).Value));
    }
#pragma warning restore CA1000 // Do not declare static members on generic types
}
