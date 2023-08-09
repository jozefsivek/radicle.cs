namespace Radicle.Common;

using System;
using System.Linq;
using Xunit;

public class SimplePeriodSourceTest
{
    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    public void Constructor_NonPositivePeriod_Throws(double seconds)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SimplePeriodSource(TimeSpan.FromSeconds(seconds)));
    }

    [Fact]
    public void Current_NewInstance_ReturnsDefaultValue()
    {
        Assert.Equal(
                default,
                new SimplePeriodSource(TimeSpan.FromSeconds(1.0)).Current);
    }

    [Fact]
    public void MoveNext_NoRampUp_Works()
    {
        TimeSpan period = TimeSpan.FromSeconds(1.0);
        TimeSpan[] expected = new TimeSpan[]
        {
            period,
            period,
            period,
            period,
        };
        SimplePeriodSource source = new(period);

        Assert.Equal(
                expected,
                Enumerable.Range(0, 4).Select(_ => source.MoveNext()));
    }

    [Fact]
    public void MoveNext_RampUp_Works()
    {
        TimeSpan period = TimeSpan.FromSeconds(1.0);
        TimeSpan[] expected = new TimeSpan[]
        {
            period / 4,
            period / 2,
            period,
            period,
        };
        SimplePeriodSource source = new(period, rampUp: true);

        Assert.Equal(
                expected,
                Enumerable.Range(0, 4).Select(_ => source.MoveNext()));
    }

    [Fact]
    public void Reset_Resets()
    {
        TimeSpan period = TimeSpan.FromSeconds(1.0);
        TimeSpan expected = period / 4;
        SimplePeriodSource source = new(period, rampUp: true);

        Assert.Equal(expected, source.MoveNext());

        source.Reset();

        Assert.Equal(expected, source.MoveNext());
    }
}
