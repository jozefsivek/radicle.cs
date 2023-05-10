namespace Radicle.Common.Visual.Models;

/// <summary>
/// Enumeration of pre-defined bar plot styles.
/// </summary>
public enum BarPlotStyle
{
    /// <summary>
    /// ASCII style with '*'.
    /// </summary>
    ASCIIStars = 0,

    /// <summary>
    /// ASCII style with '.'.
    /// </summary>
    ASCIIDots = 1,

    /// <summary>
    /// ASCII style.
    /// </summary>
    ASCII = 9,

    /// <summary>
    /// Style using solid blocks.
    /// </summary>
    FullBlock = 16,

    /// <summary>
    /// Style using partial grid blocks.
    /// </summary>
    PartialBlock = 20,

    /// <summary>
    /// Style using dots in a grid.
    /// </summary>
    BlockDots = 21,

    /// <summary>
    /// Style using simple dots.
    /// </summary>
    Dots = 32,
}
