namespace Radicle.CLI.REPL;

using System;
using System.Threading;
using System.Threading.Tasks;
using Radicle.CLI.Models;

/// <summary>
/// Interface of the REPL evaluation service.
/// </summary>
public interface IREPLEvaluator
{
    /// <summary>
    /// Evaluate given <paramref name="input"/> and produce promise
    /// of the output.
    /// </summary>
    /// <param name="input">Raw user input to process.</param>
    /// <param name="cancellationToken">Optional cancelation token.</param>
    /// <returns>Instance of <see cref="EvaluationOutputPromise"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown
    ///     if operation is canceled with <paramref name="cancellationToken"/>.</exception>
    EvaluationOutputPromise Evaluate(
            string input,
            CancellationToken cancellationToken = default);

    /// <summary>
    /// Perform auditing of the previous evaluation,
    /// e.g. push <paramref name="input"/> to history etc.
    /// </summary>
    /// <param name="input">Evaluated user input.</param>
    /// <param name="elapsed">Elapsed time from start till end
    ///     of the <paramref name="input"/> evaluation.</param>
    /// <param name="cancellationToken">Optional cancelation token.</param>
    /// <returns>Awaitable task.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown
    ///     if operation is canceled with <paramref name="cancellationToken"/>.</exception>
    Task AuditEvaluation(
            string input,
            TimeSpan elapsed,
            CancellationToken cancellationToken = default);
}
