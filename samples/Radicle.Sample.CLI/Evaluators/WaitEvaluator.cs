namespace Radicle.Sample.CLI.Evaluators;

using System;
using System.Threading;
using System.Threading.Tasks;
using Radicle.CLI.Evaluators;
using Radicle.CLI.Models;
using Radicle.Common;
using Radicle.Common.Check;
using Radicle.Common.Extensions;
using Radicle.Common.Tokenization.Models;
using Radicle.Sample.CLI.Evaluators.Base;

/// <summary>
/// "WAIT" command evaluator.
/// </summary>
internal sealed class WaitEvaluator : SampleCommandEvaluator
{
    /// <summary>
    /// Main verb of this command evaluator.
    /// </summary>
    public static readonly CommandVerb MainVerb = new("WAIT");

    /// <summary>
    /// Initializes a new instance of the <see cref="WaitEvaluator"/> class.
    /// </summary>
    public WaitEvaluator()
    {
    }

    /// <inheritdoc/>
    public override ICommandGroup Group => SampleGroup.Instance;

    /// <inheritdoc/>
    public override CommandVerb Verb => MainVerb;

    /// <inheritdoc/>
    public override string ArgumentsDocTemplate =>
            "seconds";

    /// <inheritdoc/>
    public override string Summary =>
            "Blocks/waits specified amound of seconds";

    /// <inheritdoc/>
    public override string Since => "0.3.0";

    /// <inheritdoc/>
    public override string TimeComplexityDoc => "O(N) where N is wait time";

    /// <inheritdoc/>
    protected override Task<EvaluationOutput> EvaluateEmptyAsync(
            SampleREPLEvaluator context,
            TransparentProgress<long>? progress = null,
            CancellationToken cancellationToken = default)
    {
        return Task.FromResult(EvaluationOutput.ErrorFrom(
                $"Wrong number of arguments for '{this.Verb.DocName}' command."));
    }

    /// <inheritdoc/>
    protected override async Task<EvaluationOutput> EvaluateArgsAsync(
            SampleREPLEvaluator context,
            TokenWithValue[] arguments,
            TransparentProgress<long>? progress,
            CancellationToken cancellationToken = default)
    {
        Ensure.Param(progress).Done();

        if (arguments.Length != 1)
        {
            return EvaluationOutput.ErrorFrom($"Wrong number of arguments for '{this.Verb.DocName}' command.");
        }
        else if (TryParseInt64(arguments[0], out long seconds))
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan delay = seconds <= 0
                    ? TimeSpan.Zero
                    : TimeSpan.FromSeconds(seconds);
            TimeSpan reportSpan = TimeSpan.FromSeconds(0.2);

            if (progress is not null)
            {
                progress.Total = (long)Math.Round(delay / reportSpan);
            }

            long counter = 0;

            while (!now.IsOlderThan(delay))
            {
                progress?.Report(++counter);

                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(
                        reportSpan,
                        cancellationToken).ConfigureAwait(false);
            }

            return EvaluationOutput.NoOp;
        }
        else
        {
            return EvaluationOutput.ErrorFrom(
                    $"Invalid option for '{this.Verb.DocName}'. Try HELP '{this.Verb.DocName}'.");
        }
    }
}
