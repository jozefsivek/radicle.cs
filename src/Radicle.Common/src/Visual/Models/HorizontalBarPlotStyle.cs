namespace Radicle.Common.Visual.Models;

/// <summary>
/// Immutable class representing a style of line plot.
/// </summary>
internal sealed class HorizontalBarPlotStyle
{
    /// <summary>
    /// ASCII stars plot style.
    /// </summary>
    public static readonly HorizontalBarPlotStyle ASCIIStars = new()
    {
        PositiveProgress = new[]
        {
            ' ',
            '*',
        },
        NegativeProgress = new[]
        {
            ' ',
            '*',
        },
    };

    /// <summary>
    /// ASCII line plot style.
    /// </summary>
    public static readonly HorizontalBarPlotStyle ASCII = new();

    /// <summary>
    /// Line plot style from full vertical blocks.
    /// </summary>
    public static readonly HorizontalBarPlotStyle FullVerticalBlock = new()
    {
        LeftBracket = '\u2595',
        RightBracket = '\u258F',
        PositiveProgress = new[]
        {
            ' ',
            '\u258F',
            '\u258E',
            '\u258D',
            '\u258C',
            '\u258B',
            '\u258A',
            '\u2589',
            '\u2588', // full block
        },
        NegativeProgress = new[]
        {
            ' ',
            '\u2595',
            '\u2590',
            '\u2588', // full block
        },
        ZeroIndicator = '0',
        ZeroAxis = '\u2502', // BOX DRAWINGS LIGHT VERTICAL
        ZeroAxisTick = '\u253C', // BOX DRAWINGS LIGHT VERTICAL AND HORIZONTAL
    };

    /// <summary>
    /// Line plot style from full vertical blocks.
    /// </summary>
    public static readonly HorizontalBarPlotStyle PartialBlock = new()
    {
        LeftBracket = '\u2595',
        RightBracket = '\u258F',
        PositiveProgress = new[]
        {
            ' ',
            '\u2596',
            '\u258C',
            '\u2599',
            '\u2588', // full block
        },
        NegativeProgress = new[]
        {
            ' ',
            '\u2597', // QUADRANT LOWER RIGHT
            '\u2590', // RIGHT HALF BLOCK
            '\u259F', // QUADRANT UPPER RIGHT AND LOWER LEFT AND LOWER RIGHT
            '\u2588', // full block
        },
        ZeroIndicator = '0',
        ZeroAxis = '\u2502', // BOX DRAWINGS LIGHT VERTICAL
        ZeroAxisTick = '\u253C', // BOX DRAWINGS LIGHT VERTICAL AND HORIZONTAL
    };

    /// <summary>
    /// Line plot style from full vertical blocks.
    /// </summary>
    public static readonly HorizontalBarPlotStyle BlockDots = new()
    {
        LeftBracket = '\u28B8',
        RightBracket = '\u2847',
        PositiveProgress = new[]
        {
            ' ',
            '\u2801',
            '\u2803',
            '\u2807',
            '\u2847',
            '\u284F',
            '\u285F',
            '\u287F',
            '\u28FF',
        },
        NegativeProgress = new[]
        {
            ' ',
            '\u2808',
            '\u2818',
            '\u2838',
            '\u28B8',
            '\u28B9',
            '\u28BB',
            '\u28BF',
            '\u28FF',
        },
        ZeroIndicator = '0',
        ZeroAxis = '\u2502', // BOX DRAWINGS LIGHT VERTICAL
        ZeroAxisTick = '\u253C', // BOX DRAWINGS LIGHT VERTICAL AND HORIZONTAL
    };

    /// <summary>
    /// Line plot style from full vertical blocks.
    /// </summary>
    public static readonly HorizontalBarPlotStyle Dots = new()
    {
        LeftBracket = '(',
        RightBracket = ')',
        PositiveProgress = new[]
        {
            ' ',
            '.',
            '\u2025', // two dot leader
            '\u2026', // ellipsis
        },
        NegativeProgress = new[]
        {
            ' ',
            '.',
            '\u2025', // two dot leader
            '\u2026', // ellipsis
        },
    };

    /// <summary>
    /// Gets left boundary of the plot.
    /// </summary>
    public char LeftBracket { get; init; } = '[';

    /// <summary>
    /// Gets right boundary of the plot.
    /// </summary>
    public char RightBracket { get; init; } = ']';

    /// <summary>
    /// Gets characters for positive side of the plot
    /// relative to zero from no to full part.
    /// </summary>
    public char[] PositiveProgress { get; init; } = new[] { ' ', ':', '|', 'I' };

    /// <summary>
    /// Gets characters for negative side of the plot
    /// relative to zero from no to full part.
    /// </summary>
    public char[] NegativeProgress { get; init; } = new[] { ' ', ':', '|', 'I' };

    /// <summary>
    /// Gets zero indicator.
    /// </summary>
    public char ZeroIndicator { get; init; } = '0';

    /// <summary>
    /// Gets zero axis character.
    /// </summary>
    public char ZeroAxis { get; init; } = '|';

    /// <summary>
    /// Gets zero axis tick character.
    /// </summary>
    public char ZeroAxisTick { get; init; } = '+';
}
