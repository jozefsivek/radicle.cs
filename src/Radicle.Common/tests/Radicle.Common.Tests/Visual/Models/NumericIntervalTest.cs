namespace Radicle.Common.Visual.Models;

using System;
using Xunit;

public class NumericIntervalTest
{
    [Fact]
    public void Constructor_WrongBoundaryOrder_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Int32Interval(1, 0));
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public void Constructor_EqualBoundariesLowerExcluded_Throws(
            bool includeLower,
            bool includeUpper)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Int32Interval(
                0,
                0,
                includeLower: includeLower,
                includeUpper: includeUpper));
    }

    [Theory]
    [InlineData(0.0, 0)]
    [InlineData(0.5, 0)]
    [InlineData(1.0, 0)]
    [InlineData(-1.0, -1)]
    [InlineData(double.MinValue, -1)]
    [InlineData(1.5, 1)]
    [InlineData(double.MaxValue, 1)]
    public void CompareWith_InclusiveInterval_Works(
            double input,
            int expectedCmp)
    {
        Assert.Equal(
                expectedCmp,
                new DoubleInterval(0.0, 1.0).CompareValue(input));
    }

    [Theory]
    [InlineData(0.0, 0)]
    [InlineData(0.5, 0)]
    [InlineData(0.999, 0)]
    [InlineData(1.0, 1)]
    [InlineData(-1.0, -1)]
    [InlineData(double.MinValue, -1)]
    [InlineData(1.5, 1)]
    [InlineData(double.MaxValue, 1)]
    public void CompareWith_LowerInclusiveInterval_Works(
            double input,
            int expectedCmp)
    {
        Assert.Equal(
                expectedCmp,
                new DoubleInterval(0.0, 1.0, includeUpper: false).CompareValue(input));
    }

    [Theory]
    [InlineData(0.0, -1)]
    [InlineData(0.001, 0)]
    [InlineData(0.5, 0)]
    [InlineData(1.0, 0)]
    [InlineData(-1.0, -1)]
    [InlineData(double.MinValue, -1)]
    [InlineData(1.5, 1)]
    [InlineData(double.MaxValue, 1)]
    public void CompareWith_UpperInclusiveInterval_Works(
            double input,
            int expectedCmp)
    {
        Assert.Equal(
                expectedCmp,
                new DoubleInterval(0.0, 1.0, includeLower: false).CompareValue(input));
    }
}
