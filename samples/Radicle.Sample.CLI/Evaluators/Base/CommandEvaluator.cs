namespace Radicle.Sample.CLI.Evaluators.Base;

using Radicle.CLI.Evaluators.Base;
using Radicle.CLI.Models;

/// <summary>
/// Base class of sample evaluators.
/// </summary>
internal abstract class CommandEvaluator : CommandEvaluator<EvaluationOutput, SampleREPLEvaluator>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandEvaluator"/> class.
    /// </summary>
    internal CommandEvaluator()
    {
    }
}
