namespace Radicle.CLI.REPL.Generic;

using System;
using System.Threading;
using Radicle.CLI.Models;

/// <summary>
/// Interface of the REPL evaluation service.
/// </summary>
/// <typeparam name="T">Type of the raw output payload.</typeparam>
public interface IREPLEvaluator<T> : IREPLEvaluator
    where T : notnull
{
    /// <summary>
    /// Evaluate given <paramref name="input"/> and produce promise
    /// of the output.
    /// </summary>
    /// <param name="input">Raw user input to process.</param>
    /// <param name="cancellationToken">Optional cancelation token.</param>
    /// <returns>Instance of <see cref="EvaluationRawOutputPromise{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown
    ///     if operation is canceled with <paramref name="cancellationToken"/>.</exception>
    EvaluationRawOutputPromise<T> EvaluateToRawOutput(
            string input,
            CancellationToken cancellationToken = default);

    /// <summary>
    /// Map given <paramref name="payload"/> to
    /// <see cref="EvaluationOutput"/>.
    /// </summary>
    /// <param name="payload">Raw execution output payload.</param>
    /// <returns>Instance of <see cref="EvaluationOutput"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    EvaluationOutput Map(T payload);
}
