namespace Radicle.CLI.Models;

using System;
using System.Threading.Tasks;
using Radicle.Common;
using Radicle.Common.Check;

/// <summary>
/// Promise of <see cref="EvaluationOutput"/>.
/// </summary>
public sealed class EvaluationOutputPromise
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EvaluationOutputPromise"/> class.
    /// </summary>
    /// <param name="promise">Awaitable task
    ///     yielding <see cref="EvaluationOutput"/>.</param>
    /// <param name="progress">Optiona progress instance.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public EvaluationOutputPromise(
            Task<EvaluationOutput> promise,
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
    /// Gets promise, awaitable task, yielding <see cref="EvaluationOutput"/>.
    /// </summary>
    public Task<EvaluationOutput> Promise { get; }

    /// <summary>
    /// Return completed instance.
    /// </summary>
    /// <param name="output">Output of completed promise.</param>
    /// <returns>Instance of completed <see cref="EvaluationOutputPromise"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required paramter is <see langword="null"/>.</exception>
    public static EvaluationOutputPromise Completed(EvaluationOutput output)
    {
        Ensure.Param(output).Done();

        return new EvaluationOutputPromise(Task.FromResult(output));
    }
}
