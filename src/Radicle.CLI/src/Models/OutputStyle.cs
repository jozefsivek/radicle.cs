namespace Radicle.CLI.Models;

/// <summary>
/// Enumeration of output styles.
/// </summary>
public enum OutputStyle
{
    /// <summary>
    /// Normal style of output for general text.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// Bold style.
    /// </summary>
    Bold = 1,

    /// <summary>
    /// Less or no-bold, emphasized style.
    /// </summary>
    Emphasized = 2,

    /// <summary>
    /// Comment style of output for any coments.
    /// </summary>
    Comment = 3,
}
