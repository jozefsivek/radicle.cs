namespace Radicle.CLI.Models;

using System;
using System.Collections.Immutable;

/// <summary>
/// Collection of various spinners for showing
/// unbound progress or that something is not frozen.
/// </summary>
internal static class Spinners
{
    /// <summary>
    /// Classic tube spinner.
    /// </summary>
    public static readonly ImmutableArray<char> Tube = new[]
    {
        '|',
        '/',
        '-',
        '\\',
        '|',
        '/',
        '-',
        '\\',
    }.ToImmutableArray();

    /// <summary>
    /// Arc based spinner.
    /// </summary>
    public static readonly ImmutableArray<char> Arc = new[]
    {
        '\u25DC',
        '\u25DD',
        '\u25DE',
        '\u25DF',
    }.ToImmutableArray();

    /// <summary>
    /// Shuffle Braille based spinner.
    /// </summary>
    public static readonly ImmutableArray<char> Shuffle = new[]
    {
        '\u2813',
        '\u2815',
        '\u2807',
        '\u2815',
        '\u281C',
        '\u281A',
        '\u2819',
        '\u280B',
    }.ToImmutableArray();

    /// <summary>
    /// Snake Braille based spinner.
    /// </summary>
    public static readonly ImmutableArray<char> Snake = new[]
    {
        '\u2847',
        '\u28C6',
        '\u28E4',
        '\u28F0',
        '\u28B8',
        '\u2839',
        '\u281B',
        '\u280F',
    }.ToImmutableArray();

    /// <summary>
    /// Block corners spinner.
    /// </summary>
    public static readonly ImmutableArray<char> Corners = new[]
    {
        '\u2599',
        '\u259B',
        '\u259C',
        '\u259F',
    }.ToImmutableArray();

    /// <summary>
    /// Circle spinner.
    /// </summary>
    public static readonly ImmutableArray<char> Circle = new[]
    {
        '\u25F4',
        '\u25F7',
        '\u25F6',
        '\u25F5',
    }.ToImmutableArray();

    /// <summary>
    /// Return spinner for given <paramref name="type"/>.
    /// </summary>
    /// <param name="type">Type of the spinner.</param>
    /// <returns>Spinner characters..</returns>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    public static ImmutableArray<char> GetSpinner(
            SpinnerType type)
    {
        return type switch
        {
            SpinnerType.ASCII => Tube,
            SpinnerType.Graphic => Arc,
            SpinnerType.Circle => Circle,
            SpinnerType.Corners => Corners,
            SpinnerType.Shuffle => Shuffle,
            SpinnerType.Snake => Snake,
            _ => throw new NotSupportedException($"BUG: spiner {type} is unknown."),
        };
    }
}
