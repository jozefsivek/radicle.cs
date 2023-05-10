namespace Radicle.Common.Visual;

using System;
using Radicle.Common.Visual.Models;
using Xunit;

public class HorizontalBarPlotFormatterTest
{
    [Fact]
    public void ToLine_NaNValue_Throws()
    {
        HorizontalBarPlotFormatter formatter = new();

        Assert.Throws<ArgumentException>(() => formatter.ToLine(double.NaN));
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("[", 1)]
    [InlineData("[]", 2)]
    [InlineData("[ ]", 3)]
    [InlineData("[  ]", 4)]
    public void ToLine_SmallWidthBoundaries_ReturnsExpectedString(
            string expected,
            ushort width)
    {
        HorizontalBarPlotFormatter formatter = new()
        {
            StyleName = BarPlotStyle.ASCII,
            Width = width,
            ShowLeftBoundary = true,
            ShowRightBoundary = true,
        };

        Assert.Equal(expected, formatter.ToLine(0.0));
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("[", 1)]
    [InlineData("[ ", 2)]
    [InlineData("[  ", 3)]
    public void ToLine_SmallWidthLeftBoundary_ReturnsExpectedString(
            string expected,
            ushort width)
    {
        HorizontalBarPlotFormatter formatter = new()
        {
            StyleName = BarPlotStyle.ASCII,
            Width = width,
            ShowLeftBoundary = true,
        };

        Assert.Equal(expected, formatter.ToLine(0.0));
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("]", 1)]
    [InlineData(" ]", 2)]
    [InlineData("  ]", 3)]
    public void ToLine_SmallWidthRightBoundary_ReturnsExpectedString(
            string expected,
            ushort width)
    {
        HorizontalBarPlotFormatter formatter = new()
        {
            StyleName = BarPlotStyle.ASCII,
            Width = width,
            ShowRightBoundary = true,
        };

        Assert.Equal(expected, formatter.ToLine(0.0));
    }

    [Theory]
    [InlineData("[          ]", -50)]
    [InlineData("[          ]", 0)]
    [InlineData("[:         ]", 2)]
    [InlineData("[|         ]", 4)]
    [InlineData("[I         ]", 7)]
    [InlineData("[IIIIIIIIII]", 100)]
    [InlineData("[IIIIIIIIII]", 200)]
    public void ToLine_WithBoundary_ReturnsExpectedString(
            string expected,
            double value)
    {
        HorizontalBarPlotFormatter formatter = new()
        {
            StyleName = BarPlotStyle.ASCII,
            Width = 12,
            ShowLeftBoundary = true,
            ShowRightBoundary = true,
            Interval = new DoubleInterval(0.0, 100.0),
        };

        Assert.Equal(expected, formatter.ToLine(value));
    }

    [Theory]
    [InlineData("            ", -50)]
    [InlineData(":           ", 2)]
    [InlineData("|           ", 4)]
    [InlineData("I           ", 7)]
    [InlineData("IIIIIIIIIIII", 150)]
    public void ToLine_NoBoundary_ReturnsExpectedString(
            string expected,
            double value)
    {
        HorizontalBarPlotFormatter formatter = new()
        {
            StyleName = BarPlotStyle.ASCII,
            Width = 12,
            Interval = new DoubleInterval(0.0, 100.0),
        };

        Assert.Equal(expected, formatter.ToLine(value));
    }

    [Theory]
    [InlineData("", -50)]
    [InlineData("", 0)]
    [InlineData(":", 2)]
    [InlineData("|", 4)]
    [InlineData("I", 7)]
    [InlineData("III|", 30)]
    [InlineData("IIIIIIIIIIII", 150)]
    public void ToLine_TrimmedPlot_ReturnsExpectedString(
            string expected,
            double value)
    {
        HorizontalBarPlotFormatter formatter = new()
        {
            StyleName = BarPlotStyle.ASCII,
            Width = 12,
            Interval = new DoubleInterval(0.0, 100.0),
            TrimPlot = true,
        };

        Assert.Equal(expected, formatter.ToLine(value));
    }

    [Theory]
    [InlineData("[*         ]", 6, BarPlotStyle.ASCIIStars)]
    [InlineData("[*****     ]", 50, BarPlotStyle.ASCIIStars)]
    [InlineData("[.......   ]", 70, BarPlotStyle.ASCIIDots)]
    [InlineData("[I|        ]", 16, BarPlotStyle.ASCII)]
    [InlineData("\u2595\u2588\u2588\u258B       \u258F", 26, BarPlotStyle.FullBlock)]
    [InlineData("\u2595\u2588\u2588\u2588\u2599      \u258F", 36, BarPlotStyle.PartialBlock)]
    [InlineData("\u28B8\u28FF\u28FF\u28FF\u28FF\u284F     \u2847", 46, BarPlotStyle.BlockDots)]
    [InlineData("(\u2026\u2026\u2026\u2026\u2026\u2025    )", 56, BarPlotStyle.Dots)]
    public void ToLine_VaroiusStyles_ReturnsExpectedString(
            string expected,
            double value,
            BarPlotStyle style)
    {
        HorizontalBarPlotFormatter formatter = new()
        {
            ShowLeftBoundary = true,
            ShowRightBoundary = true,
            StyleName = style,
            Width = 12,
            Interval = new DoubleInterval(0.0, 100.0),
        };

        Assert.Equal(expected, formatter.ToLine(value));
    }
}
