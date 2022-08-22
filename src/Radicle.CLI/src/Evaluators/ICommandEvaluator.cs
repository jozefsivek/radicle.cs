namespace Radicle.CLI.Evaluators;

using System.Collections.Generic;
using System.Collections.Immutable;
using Radicle.Common.Tokenization.Models;

/// <summary>
/// Baee interface of single command evaluator.
/// </summary>
public interface ICommandEvaluator
{
    /// <summary>
    /// Gets group this command evaluator belongs to.
    /// </summary>
    ICommandGroup Group { get; }

    /// <summary>
    /// Gets a value indicating whether this evaluator
    /// is legacy and will be removed in future.
    /// </summary>
    bool IsLegacy { get; }

    /// <summary>
    /// Gets a value indicating whether this evaluator
    /// is experimental and can be changed without notice.
    /// </summary>
    bool IsExperimental { get; }

    /// <summary>
    /// Gets a value indicating whether to disable listing
    /// of this command in the help when listing all commands
    /// or commands by group. Defaults to <see langword="fase"/>.
    /// </summary>
    bool DisableListing { get; }

    /// <summary>
    /// Gets main verb which this instance can match.
    /// </summary>
    /// <remarks>Verbs as reserved words are allways in capital case
    ///     see <see cref="CommandVerb.DocName"/>.</remarks>
    CommandVerb Verb { get; }

    /// <summary>
    /// Gets alternative verbs which this instance can match.
    /// Synonyms have always lower precedence than verbs.
    /// </summary>
    /// <remarks>Verbs as reserved words are allways in capital case
    ///     see <see cref="CommandVerbSynonym.DocName"/>.</remarks>
    ImmutableHashSet<CommandVerbSynonym> VerbSynonyms { get; }

    /// <summary>
    /// Gets documentation template for arguments following
    /// <see cref="Verb"/>.
    /// </summary>
    /// <remarks>Optional groups are placed in square brackets "[" and "]",
    /// groups can be nested, alternative options are denoted by pipe "|",
    /// reserved words are allways in capital case,
    /// user defined argument placeholders are in lower case as single words
    /// or kebab-case, variable count arguments are denoted by "...",
    /// empty template should be marked with "-". See https://redis.io/commands/ .</remarks>
    string ArgumentsDocTemplate { get; }

    /// <summary>
    /// Gets human readable one line explanation of
    /// this command evaluator time complexity. Use expressions like:
    /// "O(N) where N stands for ..." or "O(1)" etc.
    /// Do NOT use trailing period ".".
    /// </summary>
    string TimeComplexityDoc { get; }

    /// <summary>
    /// Gets human readable one line summary this command evaluator
    /// with NO trailing period ".".
    /// </summary>
    string Summary { get; }

    /// <summary>
    /// Gets human readable full description for this command evaluator
    /// functionality as formatted text.
    /// </summary>
    /// <remarks>The collection is to aid separation to paragraphs.</remarks>
    ImmutableArray<SimpleMarkdownText> Doc { get; }

    /// <summary>
    /// Gets the version from which the command evaluator is available from.
    /// </summary>
    string Since { get; }

    /// <summary>
    /// Gets the change log of this evaluator, version
    /// keyd summaries.
    /// </summary>
    ImmutableSortedDictionary<string, SimpleMarkdownText> ChangeLog { get; }

    /// <summary>
    /// Gets collection of subcommands. When called by user they
    /// are preceded by this <see cref="Verb"/> (e.g. subcommand
    /// with main verb 'SUB' of the command with the main verb 'BASE'
    /// is visible to user as 'BASE SUB ...').
    /// </summary>
    ImmutableArray<ICommandEvaluator> Subs { get; }

    /// <summary>
    /// Enumerate all the aplicable verbs for this
    /// evaluator, including any sub-evaluators.
    /// E.g. ['BASE'], ['BASE', 'SUB'].
    /// </summary>
    /// <returns>Collection of command evaluator verbs. For simple
    /// evaluator this is a collection of 1 item of <see cref="Verb"/>.
    /// For composite evaluators this is a collection of multiple
    /// items with items as arrays with <see cref="Verb"/> as the first
    /// element followed by the verb of subcommands etc.</returns>
    IEnumerable<CommandVerb[]> GetAllApplicableVerbs();
}
