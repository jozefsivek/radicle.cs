namespace Radicle.CLI.Evaluators.Generic;

using System;
using System.Diagnostics.CodeAnalysis;
using Radicle.Common.Tokenization.Models;

/// <summary>
/// Bundle of command evaluators of specific type.
/// </summary>
/// <typeparam name="TOut">Type of the execution raw output.</typeparam>
/// <typeparam name="TContext">Type of the evaluation context.</typeparam>
public sealed class CommandEvaluatorBundle<TOut, TContext> : CommandEvaluatorBundle
    where TOut : notnull
    where TContext : IEvaluationContext
{
    /// <summary>
    /// Add new instance of <see cref="ICommandEvaluator"/>
    /// to collection.
    /// </summary>
    /// <param name="evaluator">Evaluator instance to add.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if given
    ///     <paramref name="evaluator"/> was already added.</exception>
    public void Add(ICommandEvaluator<TOut, TContext> evaluator)
    {
        base.Add(evaluator);
    }

    /// <summary>
    /// Gets all instance of <see cref="ICommandEvaluator{TOut, TContext}"/>
    /// with matching verbs or synonyms (as fallback). This method
    /// traverses collection recursively.
    /// </summary>
    /// <param name="arguments">Values to compare case insensitivelly
    ///     to verbs or synonyms, note verbs have higher precedence.</param>
    /// <param name="evaluator">Returned evaluator if any.</param>
    /// <param name="remainingArguments">Returned remaining arguments
    ///     from <paramref name="arguments"/> which were left after
    ///     mapping verbs to evaluator.</param>
    /// <returns><see langword="true"/> if evaluator was found,
    ///     <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public bool TryMapByVerbOrSynonym(
            TokenWithValue[] arguments,
            [NotNullWhen(returnValue: true)] out ICommandEvaluator<TOut, TContext>? evaluator,
            [NotNullWhen(returnValue: true)] out TokenWithValue[]? remainingArguments)
    {
        evaluator = default;

        if (this.TryMapByVerbOrSynonym(arguments, out CommandEvaluatorRecord? record, out remainingArguments))
        {
            evaluator = (ICommandEvaluator<TOut, TContext>)record.Evaluator;

            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    protected override CommandEvaluatorBundle CreateNewInstance()
    {
        return new CommandEvaluatorBundle<TOut, TContext>();
    }
}
