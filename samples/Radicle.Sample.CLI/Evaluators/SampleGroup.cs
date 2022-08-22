namespace Radicle.Sample.CLI.Evaluators;

using Radicle.CLI.Evaluators;

/// <summary>
/// Sample group.
/// </summary>
public sealed class SampleGroup : ICommandGroup
{
    /// <summary>
    /// Static singeton instance of this class.
    /// </summary>
    public static readonly SampleGroup Instance = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="SampleGroup"/> class.
    /// </summary>
    private SampleGroup()
    {
    }

    /// <inheritdoc/>
    public CommandGroupName Name { get; } = new CommandGroupName("sample");
}
