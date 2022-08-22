namespace Radicle.CLI.Evaluators;

using Radicle.CLI.REPL;

/// <summary>
/// Command evaluator evaluation context.
/// </summary>
public interface IEvaluationContext
{
    /// <summary>
    /// Gets current styling provider in use.
    /// </summary>
    IREPLStylingProvider StylingProvider { get; }

    /// <summary>
    /// Gets input history.
    /// </summary>
    IREPLInputHistory History { get; }

    /// <summary>
    /// Gets bundle of available commands.
    /// </summary>
    CommandEvaluatorBundle Commands { get; }
}
