namespace Radicle.Sample.CLI.Evaluators;

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Radicle.CLI.Evaluators;
using Radicle.CLI.Models;
using Radicle.Common;
using Radicle.Common.Tokenization.Models;
using Radicle.Sample.CLI.Evaluators.Base;

/// <summary>
/// "QUIT" command evaluator.
/// </summary>
internal sealed class QuitEvaluator : SampleCommandEvaluator
{
    /// <summary>
    /// Main verb of this command evaluator.
    /// </summary>
    public static readonly CommandVerb MainVerb = new("QUIT");

    /// <summary>
    /// Initializes a new instance of the <see cref="QuitEvaluator"/> class.
    /// </summary>
    public QuitEvaluator()
    {
        this.VerbSynonyms = new[]
        {
            new CommandVerbSynonym("EXIT"),
            new CommandVerbSynonym("Q"),
        }.ToImmutableHashSet();
    }

    /// <inheritdoc/>
    public override ICommandGroup Group => SampleGroup.Instance;

    /// <inheritdoc/>
    public override CommandVerb Verb => MainVerb;

    /// <inheritdoc/>
    public override string ArgumentsDocTemplate => "-";

    /// <inheritdoc/>
    public override string Summary =>
            "Quits program";

    /// <inheritdoc/>
    public override string Since => "0.3.0";

    /// <inheritdoc/>
    public override string TimeComplexityDoc => "O(1)";

    /// <inheritdoc/>
    protected override Task<EvaluationOutput> EvaluateArgsAsync(
            SampleREPLEvaluator context,
            TokenWithValue[] arguments,
            TransparentProgress<long>? progress = null,
            CancellationToken cancellationToken = default)
    {
        return Task.FromResult(EvaluationOutput.Exit);
    }

    /// <inheritdoc/>
    protected override Task<EvaluationOutput> EvaluateEmptyAsync(
            SampleREPLEvaluator context,
            TransparentProgress<long>? progress,
            CancellationToken cancellationToken = default)
    {
        return Task.FromResult(EvaluationOutput.Exit);
    }
}
