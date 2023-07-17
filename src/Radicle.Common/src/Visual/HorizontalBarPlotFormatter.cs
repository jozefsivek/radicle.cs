namespace Radicle.Common.Visual;

using System;
using System.Text;
using Radicle.Common.Check;
using Radicle.Common.Statistics.Models;
using Radicle.Common.Visual.Models;

/* TODO: maybe implement inversion of the plot, which can be a way to display zero mark
 * and or plot in reverse (mirrored image) */

/// <summary>
/// Utility class for plotting a single value in-line.
/// </summary>
public sealed class HorizontalBarPlotFormatter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HorizontalBarPlotFormatter"/> class.
    /// </summary>
    public HorizontalBarPlotFormatter()
    {
    }

    /// <summary>
    /// Gets a value indicating whether left boundary should be shown.
    /// </summary>
    public bool ShowLeftBoundary { get; init; }

    /// <summary>
    /// Gets a value indicating whether right boundary should be shown.
    /// </summary>
    public bool ShowRightBoundary { get; init; }

    /// <summary>
    /// Gets a value indicating whether the plot
    /// should be padded up to <see cref="Width"/>
    /// or trimmed.
    /// </summary>
    public bool TrimPlot { get; init; }

    /// <summary>
    /// Gets width of the line plot, for best results use value
    /// large enough to incorporate boundaries, zeros etc.
    /// </summary>
    public ushort Width { get; init; } = 12;

    /// <summary>
    /// Gets interval this bar plot operates in.
    /// </summary>
    public DoubleInterval Interval { get; init; } = new DoubleInterval(0.0, 1.0);

    /// <summary>
    /// Gets the style of the plot.
    /// </summary>
    public BarPlotStyle StyleName { get; init; } = BarPlotStyle.ASCII;

    /// <summary>
    /// Gets a single line plot of the given <paramref name="value"/>.
    /// </summary>
    /// <param name="value">Value to use to draw the plot.</param>
    /// <returns>Single line with the plot.</returns>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="value"/> is not a number.</exception>
    public string ToLine(
            double value)
    {
        Ensure.Param(value).That(v => !double.IsNaN(v)).Done();

        if (this.Width <= 0)
        {
            return string.Empty;
        }

        int cmp = this.Interval.CompareValue(value);
        double v = value;
        HorizontalBarPlotStyle style = this.GetStyle();

        if (cmp < 0)
        {
            v = this.Interval.Lower;
        }
        else if (cmp > 0)
        {
            v = this.Interval.Upper;
        }

        StringBuilder result = new();
        int plotWidth = this.Width
                - (this.ShowLeftBoundary ? 1 : 0)
                - (this.ShowRightBoundary ? 1 : 0);

        // add opening boundary
        if (this.ShowLeftBoundary)
        {
            result = result.Append(style.LeftBracket);

            if (result.Length >= this.Width)
            {
                return result.ToString();
            }
        }

        if (plotWidth > 0)
        {
            double intervalWidth = this.Interval.Upper - this.Interval.Lower;

            // TODO: handle infinities
            double ratio = intervalWidth == 0.0
                    ? 1.0
                    : (v - this.Interval.Lower) / intervalWidth;
            int resolution = style.PositiveProgress.Length;
            int fill = (int)Math.Round(plotWidth * resolution * ratio, MidpointRounding.AwayFromZero);
            int full = fill / resolution;
            int partial = fill % resolution;
            int currentLength = 0;

            // fill with full characters
            for (int i = 0; i < plotWidth; i++)
            {
                if (i < full)
                {
                    result = result.Append(style.PositiveProgress[^1]);
                    currentLength++;
                }
            }

            // pad with partial fraction character
            if (currentLength < plotWidth && !(this.TrimPlot && partial == 0))
            {
                result = result.Append(style.PositiveProgress[partial]);
                currentLength++;
            }

            // pad with empty characters
            if (!this.TrimPlot)
            {
                while (currentLength < plotWidth)
                {
                    result = result.Append(style.PositiveProgress[0]);
                    currentLength++;
                }
            }
        }

        // add closing boundary
        if (this.ShowRightBoundary)
        {
            result = result.Append(style.RightBracket);
        }

        return result.ToString();
    }

    private HorizontalBarPlotStyle GetStyle()
    {
        return this.StyleName switch
        {
            BarPlotStyle.ASCIIStars => HorizontalBarPlotStyle.ASCIIStars,
            BarPlotStyle.ASCIIDots => HorizontalBarPlotStyle.ASCIIDots,
            BarPlotStyle.ASCII => HorizontalBarPlotStyle.ASCII,
            BarPlotStyle.FullBlock => HorizontalBarPlotStyle.FullVerticalBlock,
            BarPlotStyle.PartialBlock => HorizontalBarPlotStyle.PartialBlock,
            BarPlotStyle.BlockDots => HorizontalBarPlotStyle.BlockDots,
            BarPlotStyle.Dots => HorizontalBarPlotStyle.Dots,
            _ => throw new NotSupportedException($"BUG: Style name {this.StyleName} has no style."),
        };
    }
}
