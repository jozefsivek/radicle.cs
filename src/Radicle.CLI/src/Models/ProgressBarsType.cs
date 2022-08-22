namespace Radicle.CLI.Models;

/// <summary>
/// Enumeration of types of progress bars.
/// </summary>
public enum ProgressBarsType
{
    /// <summary>
    /// ASCII based progress bars.
    /// </summary>
    ASCII = 0,

    /// <summary>
    /// Graphic full vertical bars based progress bars
    /// or platform based progress bars.
    /// </summary>
    Graphic = 1,

    /// <summary>
    /// Block based progress bars.
    /// </summary>
    PartialBlock = 2,

    /// <summary>
    /// Braille based progress bars.
    /// </summary>
    BlockDots = 3,

    /// <summary>
    /// Dots/period progress bars.
    /// </summary>
    Dots = 4,
}
