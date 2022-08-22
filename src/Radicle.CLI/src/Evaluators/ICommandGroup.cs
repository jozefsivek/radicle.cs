namespace Radicle.CLI.Evaluators;

/// <summary>
/// Interface of the command evaluators group.
/// </summary>
public interface ICommandGroup
{
    /// <summary>
    /// Gets the name of the command group.
    /// </summary>
    CommandGroupName Name { get; }
}
