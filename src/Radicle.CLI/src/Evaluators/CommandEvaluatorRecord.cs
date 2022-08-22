namespace Radicle.CLI.Evaluators;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Radicle.Common.Check;

/// <summary>
/// Immutable record of signle <see cref="ICommandEvaluator"/>.
/// </summary>
public sealed class CommandEvaluatorRecord
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandEvaluatorRecord"/> class.
    /// </summary>
    /// <param name="ancestors">Ancestors to this <paramref name="evaluator"/>.</param>
    /// <param name="evaluator">Evaluator.</param>
    /// <param name="subCollection">Subcollection for given
    ///     <paramref name="evaluator"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if reequired parameter is <see langword="null"/>.</exception>
    public CommandEvaluatorRecord(
            IEnumerable<CommandEvaluatorRecord> ancestors,
            ICommandEvaluator evaluator,
            CommandEvaluatorBundle subCollection)
    {
        this.Evaluator = Ensure.Param(evaluator).Value;
        this.Ancestors = Ensure.Param(ancestors)
                .AllNotNull()
                .ToImmutableArray();
        this.SubCollection = Ensure.Param(subCollection).Value;
    }

    /// <summary>
    /// Gets instance of the evaluator.
    /// </summary>
    public ICommandEvaluator Evaluator { get; }

    /// <summary>
    /// Gets collection of parents to this <see cref="Evaluator"/>
    /// in order from root to parent.
    /// </summary>
    public ImmutableArray<CommandEvaluatorRecord> Ancestors { get; }

    /// <summary>
    /// Gets sub collection for this <see cref="Evaluator"/>.
    /// See <see cref="ICommandEvaluator.Subs"/>.
    /// </summary>
    public CommandEvaluatorBundle SubCollection { get; }
}
