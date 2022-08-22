namespace Radicle.Sample.CLI.Evaluators;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Radicle.CLI.Evaluators;
using Radicle.CLI.Models;
using Radicle.Common;
using Radicle.Common.Tokenization.Models;
using Radicle.Sample.CLI.Evaluators.Base;

/// <summary>
/// "ECHO" command evaluator.
/// </summary>
internal sealed class EchoEvaluator : CommandEvaluator
{
    /// <summary>
    /// Main verb of this command evaluator.
    /// </summary>
    public static readonly CommandVerb MainVerb = new("ECHO");

    /// <summary>
    /// Initializes a new instance of the <see cref="EchoEvaluator"/> class.
    /// </summary>
    public EchoEvaluator()
    {
    }

    /// <inheritdoc/>
    public override ICommandGroup Group => SampleGroup.Instance;

    /// <inheritdoc/>
    public override CommandVerb Verb => MainVerb;

    /// <inheritdoc/>
    public override string ArgumentsDocTemplate =>
            "[argument ...]";

    /// <inheritdoc/>
    public override string Summary =>
            "Returns command evaluator arguments back as they were used in the call";

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
        return Task.FromResult(EvaluationOutput.SuccessFrom(OutputLine.ToOutputLines(arguments.Select(a => Dump.Literal(a)))));
    }

    /// <inheritdoc/>
    protected override Task<EvaluationOutput> EvaluateEmptyAsync(
            SampleREPLEvaluator context,
            TransparentProgress<long>? progress,
            CancellationToken cancellationToken = default)
    {
        return Task.FromResult(EvaluationOutput.SuccessFrom());
    }
}
