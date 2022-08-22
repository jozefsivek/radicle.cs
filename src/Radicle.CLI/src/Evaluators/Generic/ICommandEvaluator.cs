namespace Radicle.CLI.Evaluators.Generic;

using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Radicle.CLI.Models;
using Radicle.Common;
using Radicle.Common.Tokenization.Models;

/// <summary>
/// Interface of single command evaluator.
/// </summary>
/// <typeparam name="TOut">Type of the execution raw output.</typeparam>
/// <typeparam name="TContext">Type of the evaluation context.</typeparam>
public interface ICommandEvaluator<TOut, TContext> : ICommandEvaluator
    where TOut : notnull
    where TContext : IEvaluationContext
{
    /// <summary>
    /// Gets collection of subcommands. When called by user they
    /// are preceded by this <see cref="ICommandEvaluator.Verb"/> (e.g. subcommand
    /// with main verb 'SUB' of the command with the main verb 'BASE'
    /// is visible to user as 'BASE SUB ...').
    /// </summary>
    new ImmutableArray<ICommandEvaluator<TOut, TContext>> Subs { get; }

    /// <summary>
    /// Evaluate given <paramref name="arguments"/>.
    /// <paramref name="arguments"/> do NOT contain
    /// leading command evaluator verb.
    /// </summary>
    /// <param name="arguments">Collection of arguments,
    ///     some of them can be empty.</param>
    /// <param name="context">Context to work in.</param>
    /// <param name="progress">Optional progress.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Instance of <see cref="EvaluationOutput"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown
    ///     if <paramref name="cancellationToken"/> was cancelled.</exception>
    Task<TOut> EvaluateAsync(
            TokenWithValue[] arguments,
            TContext context,
            TransparentProgress<long>? progress = null,
            CancellationToken cancellationToken = default);
}
