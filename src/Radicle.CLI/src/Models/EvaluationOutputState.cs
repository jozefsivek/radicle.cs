namespace Radicle.CLI.Models;

/// <summary>
/// Enumeration of possible session command outputs.
/// </summary>
public enum EvaluationOutputState
{
    /// <summary>
    /// No operation output.
    /// </summary>
    NoOp = 0,

    /// <summary>
    /// Request for termination.
    /// </summary>
    Exit = 1,

    /// <summary>
    /// Error output.
    /// </summary>
    Error = 2,

    /// <summary>
    /// Success output.
    /// </summary>
    Success = 3,
}
