namespace Radicle.Common.Check.Models;

/// <summary>
/// Interface of the generic method parameter description.
/// </summary>
public interface IParam
{
    /// <summary>
    /// Gets name of the parameter.
    /// </summary>
    string? Name => this.InnerParam.Name;

    /// <summary>
    /// Gets the description of the parametr reflecting <see cref="Name"/>.
    /// </summary>
    string Description => $"Parameter '{this.Name}'";

    /// <summary>
    /// Gets inner parameter.
    /// </summary>
    IParam InnerParam { get; }

    /// <summary>
    /// Gets a value indicating whether this parameter is
    /// specified, if not the parameter is always
    /// valid and its value should be ignored.
    /// </summary>
    bool IsSpecified => this.InnerParam.IsSpecified;

    /// <summary>
    /// Do nothing and return nothing.
    /// </summary>
    void Done()
    {
    }
}
