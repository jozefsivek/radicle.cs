namespace Radicle.Sample.CLI.Evaluators;

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Radicle.CLI.Evaluators;
using Radicle.CLI.Evaluators.Generic;
using Radicle.CLI.Models;
using Radicle.Common;
using Radicle.Common.Tokenization.Models;
using Radicle.Sample.CLI.Evaluators.Base;

/// <summary>
/// "FOO" command evaluator.
/// </summary>
internal sealed class FooEvaluator : SampleCommandEvaluator
{
    /// <summary>
    /// Main verb of this command evaluator.
    /// </summary>
    public static readonly CommandVerb MainVerb = new("FOO");

    /// <summary>
    /// Initializes a new instance of the <see cref="FooEvaluator"/> class.
    /// </summary>
    public FooEvaluator()
    {
        this.Subs = new ICommandEvaluator<EvaluationOutput, SampleREPLEvaluator>[]
        {
            new FooBarEvaluator(),
        }.ToImmutableArray();
    }

    /// <inheritdoc/>
    public override ICommandGroup Group => SampleGroup.Instance;

    /// <inheritdoc/>
    public override CommandVerb Verb => MainVerb;

    /// <inheritdoc/>
    public override string ArgumentsDocTemplate =>
            "[BAR]";

    /// <inheritdoc/>
    public override string Summary =>
            "example of command nesting";

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
        return Task.FromResult(EvaluationOutput.SuccessFrom($"OK foo [{arguments.Length}]"));
    }

    /// <inheritdoc/>
    protected override Task<EvaluationOutput> EvaluateEmptyAsync(
            SampleREPLEvaluator context,
            TransparentProgress<long>? progress,
            CancellationToken cancellationToken = default)
    {
        return Task.FromResult(EvaluationOutput.SuccessFrom("OK foo"));
    }
}
