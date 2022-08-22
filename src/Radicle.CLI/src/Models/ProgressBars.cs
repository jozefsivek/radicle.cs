namespace Radicle.CLI.Models;

using System;
using System.Collections.Immutable;

/// <summary>
/// Collection of various progress bars for showing
/// bound progress. Note the edge characters
/// are also included on first and last positions.
/// </summary>
internal static class ProgressBars
{
    /// <summary>
    /// ASCII progress bars.
    /// </summary>
    public static readonly ImmutableArray<char> ASCII = new[]
    {
        '[',
        ' ',
        ':',
        '|',
        'I',
        ']',
    }.ToImmutableArray();

    /// <summary>
    /// Classic full vertical block progress bars.
    /// </summary>
    public static readonly ImmutableArray<char> FullVerticalBlock = new[]
    {
        '\u2595',
        ' ',
        '\u258F',
        '\u258E',
        '\u258D',
        '\u258C',
        '\u258B',
        '\u258A',
        '\u2589',
        '\u2588', // full block
        '\u258F',
    }.ToImmutableArray();

    /// <summary>
    /// Partial block progress bars.
    /// </summary>
    public static readonly ImmutableArray<char> PartialBlock = new[]
    {
        '\u2595',
        ' ',
        '\u2596',
        '\u258C',
        '\u2599',
        '\u2588', // full block
        '\u258F',
    }.ToImmutableArray();

    /// <summary>
    /// Braille dots progress bars.
    /// </summary>
    public static readonly ImmutableArray<char> BlockDots = new[]
    {
        '\u28B8',
        ' ',
        '\u2801',
        '\u2803',
        '\u2807',
        '\u2847',
        '\u284F',
        '\u285F',
        '\u287F',
        '\u28FF',
        '\u2847',
    }.ToImmutableArray();

    /// <summary>
    /// Horizontal ots progress bars.
    /// </summary>
    public static readonly ImmutableArray<char> Dots = new[]
    {
        '(',
        ' ',
        '.',
        '\u2025', // two dot leader
        '\u2026', // ellipsis
        ')',
    }.ToImmutableArray();

    /// <summary>
    /// Return bars for given <paramref name="type"/>.
    /// </summary>
    /// <param name="type">Type of the bars.</param>
    /// <returns>Progress bars characters, first and last
    ///     character are bounding characters.</returns>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    public static ImmutableArray<char> GetBars(
            ProgressBarsType type)
    {
        return type switch
        {
            ProgressBarsType.ASCII => ASCII,
            ProgressBarsType.Graphic => FullVerticalBlock,
            ProgressBarsType.PartialBlock => PartialBlock,
            ProgressBarsType.BlockDots => BlockDots,
            ProgressBarsType.Dots => Dots,
            _ => throw new NotSupportedException($"BUG: progress type {type} is unknown."),
        };
    }
}
