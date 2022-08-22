namespace Radicle.Sample.CLI.Evaluators;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Radicle.CLI.Evaluators;
using Radicle.CLI.Models;
using Radicle.Common;
using Radicle.Common.Tokenization.Models;
using Radicle.Sample.CLI.Evaluators.Base;

// TODO: see Redis "COMMAND DOCS"

/// <summary>
/// "HELP" command evaluator.
/// </summary>
internal sealed class HelpEvaluator : CommandEvaluator
{
    /// <summary>
    /// Main verb of this command evaluator.
    /// </summary>
    public static readonly CommandVerb MainVerb = new("HELP");

    /// <summary>
    /// Initializes a new instance of the <see cref="HelpEvaluator"/> class.
    /// </summary>
    public HelpEvaluator()
    {
        this.VerbSynonyms = new[] { new CommandVerbSynonym("?") }.ToImmutableHashSet();
    }

    /// <inheritdoc/>
    public override ICommandGroup Group => SampleGroup.Instance;

    /// <inheritdoc/>
    public override CommandVerb Verb => MainVerb;

    /// <inheritdoc/>
    public override string ArgumentsDocTemplate =>
            "[command [subcommand ...]|@<group_name>|@all]";

    /// <inheritdoc/>
    public override string Summary =>
            "Prints general help or help for specific command";

    /// <inheritdoc/>
    public override string Since => "0.3.0";

    /// <inheritdoc/>
    public override string TimeComplexityDoc =>
            "O(N) where N is amount of the commands matching input criteria";

    /// <inheritdoc/>
    protected override Task<EvaluationOutput> EvaluateEmptyAsync(
            SampleREPLEvaluator context,
            TransparentProgress<long>? progress,
            CancellationToken cancellationToken = default)
    {
        return Task.FromResult(EvaluationOutput.SuccessFrom(SimpleMarkdownText.FromStrings(new[]
        {
                "To get help about Radicle sample CLI commands, type the following:",
                "      _\"help|?\"_ show this help",
                "      _\"help|? <command>\"_ show help on <command>",
                "      _\"help @all\"_ list all available commands",
                "      _\"help @<group_name>\"_ list all available commands in the given group",
                "      _\"quit|q\"_ exit",
        })));
    }

    /// <inheritdoc/>
    protected override Task<EvaluationOutput> EvaluateArgsAsync(
            SampleREPLEvaluator context,
            TokenWithValue[] arguments,
            TransparentProgress<long>? progress,
            CancellationToken cancellationToken = default)
    {
        string[] args = new string[arguments.Length];

        for (int i = 0; i < arguments.Length; i++)
        {
            if (arguments[i] is TokenString ts)
            {
                args[i] = ts.StringValue;
            }
            else
            {
                return Task.FromResult(EvaluationOutput.ErrorFrom(
                        $"Not supported arguments for '{this.Verb.DocName}' command."));
            }
        }

        // check groups
        if (args[0].StartsWith('@'))
        {
            if (args.Length != 1)
            {
                return Task.FromResult(EvaluationOutput.ErrorFrom(
                        $"Wrong number of arguments for '{this.Verb.DocName}' command."));
            }

            string rawGroupName = args[0][1..];

            if (rawGroupName.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(EvaluationOutput.SuccessFrom(HelpOutput(
                            false,
                            context.Commands.GetAllListed(recursive: true))));
            }
            else if (CommandGroupName.Specification.IsValid(rawGroupName))
            {
                CommandGroupName groupName = new(rawGroupName);

                return Task.FromResult(EvaluationOutput.SuccessFrom(HelpOutput(
                            false,
                            context.Commands.GetByGroup(groupName, recursive: true))));
            }
            else
            {
                return Task.FromResult(EvaluationOutput.SuccessFrom());
            }
        }
        else if (context.Commands.TryGetByVerbOrSynonym(
                args, out CommandEvaluatorRecord? record))
        {
            return Task.FromResult(EvaluationOutput.SuccessFrom(HelpOutput(true, record)));
        }

        return Task.FromResult(EvaluationOutput.SilentError);
    }

    private static IEnumerable<SimpleMarkdownText> HelpOutput(
            bool extended,
            params CommandEvaluatorRecord[] records)
    {
        return HelpOutput(
                extended,
                records as IEnumerable<CommandEvaluatorRecord>);
    }

    private static IEnumerable<SimpleMarkdownText> HelpOutput(
            bool extended,
            IEnumerable<CommandEvaluatorRecord> records)
    {
        foreach (CommandEvaluatorRecord record in records)
        {
            yield return string.Empty;

            foreach (SimpleMarkdownText text in
                    SingleHelpOutput(extended, record))
            {
                yield return text;
            }
        }

        yield return string.Empty;
    }

    private static IEnumerable<SimpleMarkdownText> SingleHelpOutput(
            bool extended,
            CommandEvaluatorRecord record)
    {
        ICommandEvaluator evaluator = record.Evaluator;
        string sanitized = SimpleMarkdownText.Sanitize(evaluator.ArgumentsDocTemplate).Value;
        string compoundVerb = evaluator.Verb.DocName;

        if (record.Ancestors.Length > 0)
        {
            compoundVerb = new StringBuilder()
                    .AppendJoin(' ', record.Ancestors.Select(i => i.Evaluator.Verb.DocName))
                    .Append(' ')
                    .Append(compoundVerb)
                    .ToString();
        }

        yield return $"  {compoundVerb} _{sanitized}_";

        if (extended)
        {
            yield return $"  *complexity*: {SimpleMarkdownText.Sanitize(evaluator.TimeComplexityDoc)}";
        }

        yield return $"  *summary*: {SimpleMarkdownText.Sanitize(evaluator.Summary)}";
        yield return $"  *since*: {SimpleMarkdownText.Sanitize(evaluator.Since)}";

        if (extended)
        {
            if (evaluator.ChangeLog.Count > 0)
            {
                yield return "  *changelog*:";

                foreach (KeyValuePair<string, SimpleMarkdownText> item in evaluator.ChangeLog)
                {
                    yield return $"    _{SimpleMarkdownText.Sanitize(item.Key)}_: {item.Value}";
                }
            }

            if (evaluator.Doc.Length > 0)
            {
                yield return "  *documentation*:";

                foreach (SimpleMarkdownText text in evaluator.Doc)
                {
                    yield return $"  {text}";
                }
            }
        }

        yield return $"  *group*: {SimpleMarkdownText.Sanitize(evaluator.Group.Name.InvariantValue)}";

        if (evaluator.IsLegacy)
        {
            yield return "  *legacy*: this command will be removed in future releases";
        }

        if (evaluator.IsExperimental)
        {
            yield return "  *experimantal*: this command is experimental, use with caution, can change";
        }
    }
}
