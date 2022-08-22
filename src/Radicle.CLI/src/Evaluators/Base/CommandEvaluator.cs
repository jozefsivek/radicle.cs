namespace Radicle.CLI.Evaluators.Base;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Radicle.CLI.Evaluators.Generic;
using Radicle.Common;
using Radicle.Common.Check;
using Radicle.Common.Tokenization.Models;

/// <summary>
/// Base class for <see cref="ICommandEvaluator"/>
/// implementation.
/// </summary>
/// <typeparam name="TOut">Type of the execution raw output.</typeparam>
/// <typeparam name="TContext">Type of the evaluation context.</typeparam>
public abstract class CommandEvaluator<TOut, TContext> : ICommandEvaluator<TOut, TContext>
    where TOut : notnull
    where TContext : IEvaluationContext
{
    /// <summary>
    /// Allowed styles for parsing integer arguments.
    /// </summary>
    private const NumberStyles IntegerNumberStyles = NumberStyles.AllowLeadingSign;

    private static readonly Regex IntegerNumberStyleRegex =
            new("\\A(-)?[0-9]+\\z", RegexOptions.Compiled);

    private ImmutableArray<ICommandEvaluator>? untypedSubs;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandEvaluator{TOut, TContext}"/> class.
    /// </summary>
    protected CommandEvaluator()
    {
    }

    /// <inheritdoc/>
    public abstract ICommandGroup Group { get; }

    /// <inheritdoc/>
    public abstract CommandVerb Verb { get; }

    /// <inheritdoc/>
    public ImmutableHashSet<CommandVerbSynonym> VerbSynonyms { get; protected init; } =
            ImmutableHashSet<CommandVerbSynonym>.Empty;

    /// <inheritdoc/>
    public virtual bool DisableListing { get; }

    /// <inheritdoc/>
    public virtual bool IsLegacy { get; }

    /// <inheritdoc/>
    public virtual bool IsExperimental { get; }

    /// <inheritdoc/>
    public abstract string ArgumentsDocTemplate { get; }

    /// <inheritdoc/>
    public abstract string TimeComplexityDoc { get; }

    /// <inheritdoc/>
    public abstract string Summary { get; }

    /// <inheritdoc/>
    public ImmutableArray<SimpleMarkdownText> Doc { get; protected init; } =
            ImmutableArray<SimpleMarkdownText>.Empty;

    /// <inheritdoc/>
    public abstract string Since { get; }

    /// <inheritdoc/>
    public ImmutableSortedDictionary<string, SimpleMarkdownText> ChangeLog { get; protected init; } =
            ImmutableSortedDictionary<string, SimpleMarkdownText>.Empty;

    /// <inheritdoc/>
    public ImmutableArray<ICommandEvaluator<TOut, TContext>> Subs { get; protected init; } =
            ImmutableArray<ICommandEvaluator<TOut, TContext>>.Empty;

    /// <inheritdoc/>
    ImmutableArray<ICommandEvaluator> ICommandEvaluator.Subs =>
            this.untypedSubs ??= this.Subs.Cast<ICommandEvaluator>().ToImmutableArray();

    /// <inheritdoc/>
    public async Task<TOut> EvaluateAsync(
            TokenWithValue[] arguments,
            TContext context,
            TransparentProgress<long>? progress = null,
            CancellationToken cancellationToken = default)
    {
        Ensure.Param(arguments).AllNotNull().Done();
        Ensure.Param(context).Done();

        if (arguments.Length == 0)
        {
            return await this.EvaluateEmptyAsync(
                    context,
                    progress: progress,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        return await this.EvaluateArgsAsync(
                context,
                arguments,
                progress: progress,
                cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public IEnumerable<CommandVerb[]> GetAllApplicableVerbs()
    {
        CommandVerb[] head = new[] { this.Verb };

        yield return head;

        foreach (ICommandEvaluator<TOut, TContext> sub in this.Subs)
        {
            foreach (CommandVerb[] verbs in sub.GetAllApplicableVerbs())
            {
                yield return head.Concat(verbs).ToArray();
            }
        }
    }

    /// <summary>
    /// Try parse given <paramref name="argument"/>
    /// to int64 value according to common interpreter rules.
    /// </summary>
    /// <param name="argument">Argument to parse.</param>
    /// <param name="value">Parsed value.</param>
    /// <returns><see langword="true"/> if parsed successfully,
    ///     <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown if required value
    ///     is <see langword="null"/>.</exception>
    protected static bool TryParseInt64(
            TokenWithValue argument,
            out long value)
    {
        value = default;

        if (argument is TokenString ts
                && IntegerNumberStyleRegex.IsMatch(ts.StringValue)
                && long.TryParse(
                    ts.StringValue,
                    IntegerNumberStyles,
                    CultureInfo.InvariantCulture,
                    out long d))
        {
            value = d;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns "yes" or "no" string according to <paramref name="value"/>.
    /// This makes better UX than boolean values.
    /// </summary>
    /// <param name="value">Value to determine output on.</param>
    /// <returns>"yes" or "no" string.</returns>
    protected static string GetYesNo(bool value)
    {
        return value ? "yes" : "no";
    }

    /// <summary>
    /// Evaluator argument less input.
    /// </summary>
    /// <param name="context">Context to work in.</param>
    /// <param name="progress">Optional progress.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Instance of <typeparamref name="TOut"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown
    ///     if <paramref name="cancellationToken"/> was cancelled.</exception>
    protected abstract Task<TOut> EvaluateEmptyAsync(
            TContext context,
            TransparentProgress<long>? progress = null,
            CancellationToken cancellationToken = default);

    /// <summary>
    /// Evaluator input with arguments.
    /// </summary>
    /// <param name="context">Context to work in.</param>
    /// <param name="arguments">Non empty collection of arguments.</param>
    /// <param name="progress">Optional progress.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Instance of <typeparamref name="TOut"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown
    ///     if <paramref name="cancellationToken"/> was cancelled.</exception>
    protected abstract Task<TOut> EvaluateArgsAsync(
            TContext context,
            TokenWithValue[] arguments,
            TransparentProgress<long>? progress = null,
            CancellationToken cancellationToken = default);
}
