namespace Radicle.CLI.Models;

/// <summary>
/// Enumeraion of all spinnner types.
/// </summary>
public enum SpinnerType
{
    /// <summary>
    /// ASCII based spinner.
    /// </summary>
    ASCII = 0,

    /// <summary>
    /// Graphic arc or platform based spinner.
    /// </summary>
    Graphic = 1,

    /// <summary>
    /// Braille based shuffle spinner.
    /// </summary>
    Shuffle = 2,

    /// <summary>
    /// Braile based snake like spinner.
    /// </summary>
    Snake = 3,

    /// <summary>
    /// Block based spinner.
    /// </summary>
    Corners = 4,

    /// <summary>
    /// Circle based spinner.
    /// </summary>
    Circle = 5,
}
