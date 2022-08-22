namespace Radicle.CLI.Evaluators;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Radicle.Common.Check;
using Radicle.Common.Tokenization.Models;

/// <summary>
/// Bundle of command evaluators of specific type.
/// </summary>
public abstract class CommandEvaluatorBundle : IEnumerable<CommandEvaluatorRecord>
{
    private readonly Dictionary<CommandVerb, CommandEvaluatorRecord> byVerb = new();

    private readonly Dictionary<CommandVerbSynonym, CommandVerb> bySynonym = new();

    private readonly Dictionary<CommandGroupName, ImmutableArray<CommandEvaluatorRecord>> byGroupAndListed = new();

    private readonly HashSet<CommandEvaluatorRecord> allListed = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandEvaluatorBundle"/> class.
    /// </summary>
    internal CommandEvaluatorBundle()
    {
    }

    /// <summary>
    /// Gets a value indicating whether this collection is empty.
    /// </summary>
    public bool IsEmpty => this.byVerb.Count == 0;

    /// <inheritdoc/>
    public IEnumerator<CommandEvaluatorRecord> GetEnumerator()
    {
        return this.byVerb.Values.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    /// <summary>
    /// Gets all instances of <see cref="CommandEvaluatorRecord"/>
    /// with matching <paramref name="name"/>.
    /// </summary>
    /// <param name="name">Group name.</param>
    /// <param name="recursive">Flag determining if to perform recursive listing.</param>
    /// <returns>Collection of first level command evaluator
    ///     with matching <paramref name="name"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public IEnumerable<CommandEvaluatorRecord> GetByGroup(
            CommandGroupName name,
            bool recursive = false)
    {
        Ensure.Param(name).Done();

        if (this.byGroupAndListed.TryGetValue(name, out ImmutableArray<CommandEvaluatorRecord> list))
        {
            foreach (CommandEvaluatorRecord item in list)
            {
                yield return item;

                if (recursive)
                {
                    foreach (CommandEvaluatorRecord sub in
                        item.SubCollection.GetByGroup(name, recursive: true))
                    {
                        yield return sub;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Tries to complete verb with first level instances verbs of contained
    /// <see cref="CommandEvaluatorRecord"/> with verb or as a fallback synonym.
    /// </summary>
    /// <param name="value">Value to complete case insensitivelly
    ///     to the full verb or synonym, note verbs have higher precedence.</param>
    /// <param name="completion">Returned completion of the
    ///     <paramref name="value"/> if any. E.g. "FOO" for "F",
    ///     or "foo" for "f".</param>
    /// <returns><see langword="true"/> if verb or synonym was found,
    ///     <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public bool TryCompleteVerbOrSynonym(
            string value,
            [NotNullWhen(returnValue: true)] out string? completion)
    {
        Ensure.Param(value).Done();

        completion = default;

        if (CommandVerb.Specification.IsValid(value))
        {
            CommandVerb? foundVerb = default;

            foreach (CommandVerb verb in this.byVerb.Keys.Where(v =>
                        v.Value.StartsWith(value, StringComparison.OrdinalIgnoreCase)))
            {
                if (foundVerb is null)
                {
                    foundVerb = verb;
                }
                else
                {
                    foundVerb = default;
                    break;
                }
            }

            if (foundVerb?.Value.Length > value.Length)
            {
                completion = char.IsLower(value[0])
                        ? foundVerb.InvariantValue[value.Length..]
                        : foundVerb.Value[value.Length..];

                return true;
            }
        }

        if (CommandVerbSynonym.Specification.IsValid(value))
        {
            CommandVerbSynonym? foundVerbSynonym = default;

            foreach (CommandVerbSynonym verbSynonym in this.bySynonym.Keys.Where(v =>
                        v.Value.StartsWith(value, StringComparison.OrdinalIgnoreCase)))
            {
                if (foundVerbSynonym is null)
                {
                    foundVerbSynonym = verbSynonym;
                }
                else
                {
                    foundVerbSynonym = default;
                    break;
                }
            }

            if (foundVerbSynonym?.Value.Length > value.Length)
            {
                completion = char.IsLower(value[0])
                        ? foundVerbSynonym.InvariantValue[value.Length..]
                        : foundVerbSynonym.Value[value.Length..];

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets all instances of <see cref="CommandEvaluatorRecord"/>
    /// with matching verbs or as fallback synonyms.
    /// Unlike the <see cref="TryGetByVerbOrSynonym(string, out CommandEvaluatorRecord?)"/>
    /// this method traverses collection recursively.
    /// </summary>
    /// <param name="values">Values to compare case insensitivelly
    ///     to verbs or synonyms, note verbs have higher precedence
    ///     and all values of <paramref name="values"/> need to
    ///     match existing evaluator.</param>
    /// <param name="completion">Returned evaluator if any.</param>
    /// <returns><see langword="true"/> if evaluator was found,
    ///     <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public bool TryCompleteVerbOrSynonym(
            string[] values,
            [NotNullWhen(returnValue: true)] out string? completion)
    {
        Ensure.Param(values).AllNotNull().Done();

        completion = default;

        if (values.Length == 1
                && this.TryCompleteVerbOrSynonym(values[0], out completion))
        {
            return true;
        }
        else if (values.Length > 1
                && this.TryGetByVerbOrSynonym(values[0], out CommandEvaluatorRecord? recorn))
        {
            return recorn.SubCollection.TryCompleteVerbOrSynonym(
                        values.Skip(1).ToArray(),
                        out completion);
        }

        return false;
    }

    /// <summary>
    /// Gets all first level instances of <see cref="CommandEvaluatorRecord"/>
    /// which can be listed, for all evaluators enumerate this instance.
    /// </summary>
    /// <param name="recursive">Flag determining recursive listing.</param>
    /// <returns>Collection of listable first level
    ///     command evaluators.</returns>
    public IEnumerable<CommandEvaluatorRecord> GetAllListed(
            bool recursive = false)
    {
        foreach (CommandEvaluatorRecord item in this.allListed)
        {
            yield return item;

            if (recursive)
            {
                foreach (CommandEvaluatorRecord sub in
                        item.SubCollection.GetAllListed(recursive: true))
                {
                    yield return sub;
                }
            }
        }
    }

    /// <summary>
    /// Gets all first level instances of <see cref="CommandEvaluatorRecord"/>
    /// with matching given verb or as fallback synonym.
    /// </summary>
    /// <param name="value">Value to compare case insensitivelly
    ///     to verb or synonym, note verbs have higher precedence.</param>
    /// <param name="record">Returned evaluator record if any.</param>
    /// <returns><see langword="true"/> if evaluator was found,
    ///     <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public bool TryGetByVerbOrSynonym(
            string value,
            [NotNullWhen(returnValue: true)] out CommandEvaluatorRecord? record)
    {
        Ensure.Param(value).Done();

        record = default;

        if (CommandVerb.Specification.IsValid(value)
                && this.byVerb.TryGetValue(new CommandVerb(value), out record))
        {
            return true;
        }

        if (CommandVerbSynonym.Specification.IsValid(value)
                && this.bySynonym.TryGetValue(new CommandVerbSynonym(value), out CommandVerb? verb)
                && this.byVerb.TryGetValue(verb, out record))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets all instance of <see cref="CommandEvaluatorRecord"/>
    /// with matching verbs or as fallback synonyms. Unlike
    /// <see cref="TryGetByVerbOrSynonym(string, out CommandEvaluatorRecord?)"/>
    /// this method traverses collection rreecursively.
    /// </summary>
    /// <param name="values">Values to compare case insensitivelly
    ///     to verbs or synonyms, note verbs have higher precedence
    ///     and all values of <paramref name="values"/> need to
    ///     match existing evaluator.</param>
    /// <param name="record">Returned evaluator if any.</param>
    /// <returns><see langword="true"/> if evaluator was found,
    ///     <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public bool TryGetByVerbOrSynonym(
            string[] values,
            [NotNullWhen(returnValue: true)] out CommandEvaluatorRecord? record)
    {
        Ensure.Param(values).AllNotNull().Done();

        record = default;

        if (values.Length > 0
                && this.TryGetByVerbOrSynonym(values[0], out record))
        {
            if (values.Length > 1)
            {
                return record.SubCollection.TryGetByVerbOrSynonym(
                        values.Skip(1).ToArray(),
                        out record);
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets all instance of <see cref="CommandEvaluatorRecord"/>
    /// with matching verbs or synonyms (as fallback). Unlike
    /// <see cref="TryGetByVerbOrSynonym(string, out CommandEvaluatorRecord?)"/>
    /// this method traverses collection recursively.
    /// </summary>
    /// <param name="arguments">Values to compare case insensitivelly
    ///     to verbs or synonyms, note verbs have higher precedence.</param>
    /// <param name="record">Returned evaluator if any.</param>
    /// <param name="remainingArguments">Returned remaining arguments
    ///     from <paramref name="arguments"/> which were left after
    ///     mapping verbs to evaluator.</param>
    /// <returns><see langword="true"/> if evaluator was found,
    ///     <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    protected bool TryMapByVerbOrSynonym(
            TokenWithValue[] arguments,
            [NotNullWhen(returnValue: true)] out CommandEvaluatorRecord? record,
            [NotNullWhen(returnValue: true)] out TokenWithValue[]? remainingArguments)
    {
        Ensure.Param(arguments).AllNotNull().Done();

        record = default;
        remainingArguments = default;

        if (arguments.Length > 0
                && arguments[0] is TokenString ts
                && this.TryGetByVerbOrSynonym(ts.StringValue, out record))
        {
            if (arguments.Length > 1)
            {
                TokenWithValue[] tail = arguments.Skip(1).ToArray();

                if (record.SubCollection.TryMapByVerbOrSynonym(
                        tail,
                        out CommandEvaluatorRecord? subRecord,
                        out TokenWithValue[]? subRemainingArguments))
                {
                    record = subRecord;
                    remainingArguments = subRemainingArguments;
                }
                else
                {
                    remainingArguments = tail;
                }

                return true;
            }

            remainingArguments = Array.Empty<TokenWithValue>();

            return true;
        }

        return false;
    }

    /// <summary>
    /// Add new instance of <see cref="ICommandEvaluator"/>
    /// to the bundle collection.
    /// </summary>
    /// <param name="evaluator">Evaluator instance to add.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if given
    ///     <paramref name="evaluator"/> was already added.</exception>
    protected void Add(ICommandEvaluator evaluator)
    {
        this.AddInternal(ImmutableList<CommandEvaluatorRecord>.Empty, evaluator);
    }

    /// <summary>
    /// Create new instance of the derived class.
    /// </summary>
    /// <returns>Instance of <see cref="CommandEvaluatorBundle"/>.</returns>
    protected abstract CommandEvaluatorBundle CreateNewInstance();

    /// <summary>
    /// Add new instance of <see cref="ICommandEvaluator"/>
    /// to collection.
    /// </summary>
    /// <param name="ancestors">Ancestors.</param>
    /// <param name="evaluator">Evaluator instance to add.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if given
    ///     <paramref name="evaluator"/> was already added.</exception>
    private void AddInternal(
            ImmutableList<CommandEvaluatorRecord> ancestors,
            ICommandEvaluator evaluator)
    {
        Ensure.Param(evaluator).Done();

        if (this.byVerb.ContainsKey(evaluator.Verb))
        {
            throw new ArgumentException(
                    $"Duplicate evaluator verb '{evaluator.Verb.DocName}' added: {evaluator.GetType()}",
                    nameof(evaluator));
        }

        if (evaluator.VerbSynonyms.Any(s => this.bySynonym.ContainsKey(s)))
        {
            string synonyms = string.Join(
                    ", ",
                    evaluator.VerbSynonyms.Select(s => $"'{s.DocName}'"));

            throw new ArgumentException(
                    $"Duplicate evaluator verb synonym {synonyms} added: {evaluator.GetType()}",
                    nameof(evaluator));
        }

        CommandEvaluatorBundle subCollection = this.CreateNewInstance();
        CommandEvaluatorRecord record = new(ancestors, evaluator, subCollection);

        foreach (ICommandEvaluator sub in evaluator.Subs)
        {
            ImmutableList<CommandEvaluatorRecord> subAncestors = ancestors.Add(record);
            subCollection.AddInternal(subAncestors, sub);
        }

        if (!evaluator.DisableListing)
        {
            CommandGroupName groupName = evaluator.Group.Name;

            if (!this.byGroupAndListed.ContainsKey(groupName))
            {
                this.byGroupAndListed[groupName] = ImmutableArray<CommandEvaluatorRecord>.Empty;
            }

            this.byGroupAndListed[groupName] = this.byGroupAndListed[groupName].Add(record);
            _ = this.allListed.Add(record);
        }

        this.byVerb[evaluator.Verb] = record;

        foreach (CommandVerbSynonym synonym in evaluator.VerbSynonyms)
        {
            this.bySynonym[synonym] = evaluator.Verb;
        }
    }
}
