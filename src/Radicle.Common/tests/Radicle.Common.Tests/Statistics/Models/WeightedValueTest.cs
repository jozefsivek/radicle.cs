namespace Radicle.Common.Statistics.Models;

using System;
using Xunit;

public class WeightedValueTest
{
    [Fact]
    public void Constructor_NullValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                new WeightedValue<object>(null!, 0.0));
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    public void Constructor_OutOfBoundsWeight_Throws(double weight)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                new WeightedValue<int>(0, weight));
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.NegativeInfinity)]
    [InlineData(double.PositiveInfinity)]
    public void Constructor_InvalidWeight_Throws(double weight)
    {
        Assert.Throws<ArgumentException>(() =>
                new WeightedValue<int>(0, weight));
    }

    [Theory]
    [InlineData("", 0.1, " <0.1>")]
    [InlineData("foo", 10.0, "foo <10>")]
    public void ToString_ValidWeight_Returns(
            string value,
            double weight,
            string expected)
    {
        Assert.Equal(
                expected,
                new WeightedValue<string>(value, weight).ToString());
    }
}
