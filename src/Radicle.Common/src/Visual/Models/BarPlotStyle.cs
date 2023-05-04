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
    /// ASCII style.
    /// </summary>
    ASCII = 1,

    /// <summary>
    /// Style using solid blocks.
    /// </summary>
    FullBlock = 2,

    /// <summary>
    /// Style using partial grid blocks.
    /// </summary>
    PartialBlock = 3,

    /// <summary>
    /// Style using dots in a grid.
    /// </summary>
    BlockDots = 4,

    /// <summary>
    /// Style using simple dots.
    /// </summary>
    Dots = 5,
}
