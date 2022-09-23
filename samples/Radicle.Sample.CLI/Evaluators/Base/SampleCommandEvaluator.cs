namespace Radicle.Sample.CLI.Evaluators.Base;

using Radicle.CLI.Evaluators.Base;
using Radicle.CLI.Models;

/// <summary>
/// Base class of sample evaluators.
/// </summary>
internal abstract class SampleCommandEvaluator : CommandEvaluator<EvaluationOutput, SampleREPLEvaluator>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SampleCommandEvaluator"/> class.
    /// </summary>
    internal SampleCommandEvaluator()
    {
    }
}
