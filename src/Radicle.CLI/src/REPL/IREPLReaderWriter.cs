namespace Radicle.CLI.REPL;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Radicle.CLI.Models;
using Radicle.Common;

/// <summary>
/// REPL (Read Eval Print Loop) console witer.
/// </summary>
public interface IREPLReaderWriter
{
    /// <summary>
    /// Write given <paramref name="value"/>
    /// to user output.
    /// </summary>
    /// <param name="value">Value to write if any.</param>
    /// <returns>Awaitable task.</returns>
    Task WriteAsync(OutputLine? value = null);

    /// <summary>
    /// Write given lines <paramref name="values"/>
    /// to user output.
    /// </summary>
    /// <param name="values">Collection of line values to write
    ///     if any.</param>
    /// <returns>Awaitable task.</returns>
    /// <exception cref="ArgumentNullException">Thrown if
    ///     required parameter is <see langword="null"/>.</exception>
    Task WriteLinesAsync(IEnumerable<OutputLine> values);

    /// <summary>
    /// Write given output lines <paramref name="outputFactory"/>
    /// to user output while providing progress while awaiting
    /// the output.
    /// </summary>
    /// <param name="progress">Progress to use while waiting for
    ///     <paramref name="outputFactory"/>.</param>
    /// <param name="outputFactory">Collection of line values to write
    ///     if any.</param>
    /// <returns>Awaitable task.</returns>
    /// <exception cref="ArgumentNullException">Thrown if
    ///     required parameter is <see langword="null"/>.</exception>
    Task WriteLinesAsync(
            TransparentProgress<long> progress,
            Func<Task<IEnumerable<OutputLine>>> outputFactory);

    /// <summary>
    /// Writes output separator between repeating
    /// evaluation outputs.
    /// </summary>
    /// <param name="repeat">Number of repeat, starting with 1.</param>
    /// <param name="total">Total number of repeats.</param>
    /// <param name="repeatingUserInput">User input which is being evaluated repeatedly,
    ///     this is usually output of <see cref="ReadAsync(CancellationToken)"/>
    ///     stripped of markings for repeat.</param>
    /// <returns>Awaitable task.</returns>
    /// <exception cref="ArgumentNullException">Thrown if
    ///     required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="repeat"/> or <paramref name="total"/>
    ///     anre zero of <paramref name="total"/> is smaller than <paramref name="repeat"/>.</exception>
    Task WriteRepeatEvaluationOutputSeparatorAsync(
            ushort repeat,
            ushort total,
            string repeatingUserInput);

    /// <summary>
    /// Writes elapsed time mark.
    /// </summary>
    /// <param name="elapsed">Amount of elapsed time.</param>
    /// <returns>Awaitable task.</returns>
    Task WriteElapsedMarkAsync(TimeSpan elapsed);

    /// <summary>
    /// Read the user input.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation
    ///     token to cancel read operation.</param>
    /// <returns>Input read from the console.</returns>
    /// <exception cref="OperationCanceledException">Thrown
    ///     if <paramref name="cancellationToken"/>
    ///     was cancelled.</exception>
    Task<string> ReadAsync(CancellationToken cancellationToken = default);
}
