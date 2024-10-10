namespace Radicle.Sample.CLI.Evaluators;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Radicle.CLI.Evaluators;
using Radicle.CLI.Models;
using Radicle.CLI.REPL;
using Radicle.Common;
using Radicle.Common.Tokenization.Models;
using Radicle.Sample.CLI.Evaluators.Base;

/// <summary>
/// "HISTORY" command evaluator.
/// </summary>
internal sealed class HistoryEvaluator : SampleCommandEvaluator
{
    /// <summary>
    /// Main verb of this command evaluator.
    /// </summary>
    public static readonly CommandVerb MainVerb = new("HISTORY");

    /// <inheritdoc/>
    public override ICommandGroup Group => SampleGroup.Instance;

    /// <inheritdoc/>
    public override CommandVerb Verb => MainVerb;

    /// <inheritdoc/>
    public override string ArgumentsDocTemplate =>
            "-";

    /// <inheritdoc/>
    public override string Summary =>
            "Prints input history stack";

    /// <inheritdoc/>
    public override string Since => "0.3.0";

    /// <inheritdoc/>
    public override string TimeComplexityDoc =>
            "O(N) where N is length of history stack";

    /// <inheritdoc/>
    protected override Task<EvaluationOutput> EvaluateEmptyAsync(
            SampleREPLEvaluator context,
            TransparentProgress<long>? progress,
            CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
                EvaluationOutput.SuccessFrom(
                    OutputLine.ToOutputLines(
                        context.History.Select(r => RecordToString(r)))));
    }

    /// <inheritdoc/>
    protected override Task<EvaluationOutput> EvaluateArgsAsync(
            SampleREPLEvaluator context,
            TokenWithValue[] arguments,
            TransparentProgress<long>? progress,
            CancellationToken cancellationToken = default)
    {
        return Task.FromResult(EvaluationOutput.ErrorFrom(
                $"Wrong number of arguments for '{this.Verb.DocName}' command."));
    }

    private static string RecordToString(REPLInputHistoryRecord record)
    {
        return (record.IsModified ? "*" : " ") + record.Value;
    }
}
