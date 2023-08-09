namespace Radicle.Common.Extensions;

using System;
using Xunit;

public class TimeSpanExtensionsTest
{
    [Theory]
    [InlineData(0.0)]
    [InlineData(0.2)]
    [InlineData(0.4)]
    [InlineData(1.0)]
    public void Clamp_ValueInRange_ReturnsValue(double seconds)
    {
        TimeSpan span = TimeSpan.FromSeconds(seconds);

        Assert.Equal(span, span.Clamp(TimeSpan.Zero, TimeSpan.FromSeconds(1.0)));
    }

    [Theory]
    [InlineData(-1.2, true)]
    [InlineData(-1.0, true)]
    [InlineData(1.0, false)]
    [InlineData(1.4, false)]
    public void Clamp_ValueOutsideRange_ReturnsClampedValue(double seconds, bool low)
    {
        TimeSpan span = TimeSpan.FromSeconds(seconds);
        TimeSpan min = TimeSpan.FromSeconds(-1.0);
        TimeSpan max = TimeSpan.FromSeconds(1.0);

        Assert.Equal(
                low ? min : max,
                span.Clamp(min, max));
    }

    [Theory]
    [InlineData(long.MinValue, true)]
    [InlineData(long.MaxValue, false)]
    public void Clamp_MaxValueOutsideRange_ReturnsClampedValue(long ticks, bool low)
    {
        TimeSpan span = new(ticks);
        TimeSpan min = TimeSpan.FromSeconds(-1.0);
        TimeSpan max = TimeSpan.FromSeconds(1.0);

        Assert.Equal(
                low ? min : max,
                span.Clamp(min, max));
    }

    [Fact]
    public void UseIfShorterOr_ShorterThis_ReturnsThis()
    {
        TimeSpan one = TimeSpan.FromSeconds(2);
        TimeSpan alternative = TimeSpan.FromSeconds(20);

        Assert.Equal(one, one.UseIfShorterOr(alternative));
    }

    [Fact]
    public void UseIfShorterOr_ShorterAlternative_ReturnsAlternative()
    {
        TimeSpan one = TimeSpan.FromSeconds(20);
        TimeSpan alternative = TimeSpan.FromSeconds(-2);

        Assert.Equal(alternative, one.UseIfShorterOr(alternative));
    }

    [Fact]
    public void UseIfLongerOr_LongerThis_ReturnsThis()
    {
        TimeSpan one = TimeSpan.FromSeconds(-2);
        TimeSpan alternative = TimeSpan.FromSeconds(-20);

        Assert.Equal(one, one.UseIfLongerOr(alternative));
    }

    [Fact]
    public void UseIfLongerOr_LongerAlternative_ReturnsAlternative()
    {
        TimeSpan one = TimeSpan.FromSeconds(-20);
        TimeSpan other = TimeSpan.FromSeconds(20);

        Assert.Equal(other, one.UseIfLongerOr(other));
    }

    [Theory]
    [InlineData("01:14:05.93", 0, 1, 14, 5, 929)]
    [InlineData("01:14:05.03", 0, 1, 14, 5, 26)]
    [InlineData("12d 01:14:15.93", 12, 1, 14, 15, 929)]
    [InlineData("12d 01:14:15.00", 12, 1, 14, 15, 0)]
    [InlineData("-01:14:05.93", 0, -1, -14, -5, -926)]
    [InlineData("-01:14:05.03", 0, -1, -14, -5, -26)]
    [InlineData("-12d 01:14:15.93", -12, -1, -14, -15, -929)]
    [InlineData("-12d 01:14:15.00", -12, -1, -14, -15, 0)]
    public void ToCounter_ValidInput_Works(
            string expected,
            int d,
            int h,
            int m,
            int s,
            int ms)
    {
        Assert.Equal(expected, new TimeSpan(d, h, m, s, ms).ToCounter());
    }

    [Fact]
    public void ToCounter_OutOfBoudsPrecission_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                TimeSpan.FromMilliseconds(1234).ToCounter(secondsFloatingPrecission: 4));
    }

    [Theory]
    [InlineData("00:00:01", 929, 0)]
    [InlineData("00:00:00.9", 929, 1)]
    [InlineData("00:00:00.93", 929, 2)]
    [InlineData("00:00:00.929", 929, 3)]
    public void ToCounter_VariablePrecission_Works(
            string expected,
            int ms,
            byte precission)
    {
        Assert.Equal(
                expected,
                TimeSpan.FromMilliseconds(ms).ToCounter(secondsFloatingPrecission: precission));
    }

    [Fact]
    public void ToH_MaxInput_Returns()
    {
        Assert.Equal("\u221E", TimeSpan.MaxValue.ToH());
    }

    [Fact]
    public void ToH_MinInput_Returns()
    {
        Assert.Equal("-\u221E", TimeSpan.MinValue.ToH());
    }

    [Theory]
    [InlineData("100ns", 0, 0, 0, 0.000_000_1)]
    [InlineData("2\u03bcs", 0, 0, 0, 0.000_002)]
    [InlineData("2s", 0, 0, 0, 2.0)]
    [InlineData("52s", 0, 0, 0, 51.500)]
    [InlineData("5m", 0, 0, 5, 14.0)]
    [InlineData("90s", 0, 0, 1, 30.0)]
    [InlineData("14m", 0, 0, 14, 15.0)]
    [InlineData("5h", 0, 5, 14, 15.0)]
    [InlineData("90m", 0, 1, 30, 0.0)]
    [InlineData("7h", 0, 7, 14, 15.0)]
    [InlineData("5d", 4, 14, 45, 9.0)]
    [InlineData("37h", 1, 12, 45, 9.0)]
    [InlineData("514d", 514, 0, 0, 0.0)]
    [InlineData("2y", 780, 23, 0, 0.0)]
    [InlineData("416d", 415, 23, 0, 0.0)]
    [InlineData("10y", 3650, 0, 0, 0.0)]
    [InlineData("-100ns", 0, 0, 0, -0.000_000_1)]
    [InlineData("-2\u03bcs", 0, 0, 0, -0.000_002)]
    [InlineData("-2s", 0, 0, 0, -2.0)]
    [InlineData("-32s", 0, 0, 0, -31.500)]
    [InlineData("-5m", 0, 0, -5, -14.0)]
    [InlineData("-90s", 0, 0, -1, -30.0)]
    [InlineData("-14m", 0, 0, -14, -15.0)]
    [InlineData("-90m", 0, -1, -30, 0.0)]
    [InlineData("-7h", 0, -7, -14, -15.0)]
    [InlineData("-5d", -4, -14, -45, -9.0)]
    [InlineData("-37h", -1, -12, -45, -9.0)]
    [InlineData("-314d", -314, 0, 0, 0.0)]
    [InlineData("-2y", -780, -23, 0, 0.0)]
    [InlineData("-416d", -415, -23, 0, 0.0)]
    [InlineData("-2y", -730, -23, 0, 0.0)]
    [InlineData("-10y", -3650, 0, 0, 0.0)]
    public void ToH_ValidInput_Works(
            string expected,
            int d,
            int h,
            int m,
            double s)
    {
        long ticks = (TimeSpan.TicksPerDay * d)
                + (TimeSpan.TicksPerHour * h)
                + (TimeSpan.TicksPerMinute * m)
                + ((long)Math.Round(TimeSpan.TicksPerSecond * s, MidpointRounding.AwayFromZero));

        Assert.Equal(expected, new TimeSpan(ticks).ToH());
    }

    [Theory]
    [InlineData("100 nanoseconds", 0, 0, 0, 0.000_000_1)]
    [InlineData("2 microseconds", 0, 0, 0, 0.000_002)]
    [InlineData("2 milliseconds", 0, 0, 0, 0.002)]
    [InlineData("500 microseconds", 0, 0, 0, 0.000_500)]
    [InlineData("515 milliseconds", 0, 0, 0, 0.514_5)]
    [InlineData("2 seconds", 0, 0, 0, 2.0)]
    [InlineData("52 seconds", 0, 0, 0, 51.500)]
    [InlineData("5 minutes and 14 seconds", 0, 0, 5, 14.0)]
    [InlineData("2 minutes and 30 seconds", 0, 0, 2, 30.0)]
    [InlineData("5 minutes and 1 second", 0, 0, 5, 1.0)]
    [InlineData("14 minutes and 15 seconds", 0, 0, 14, 15.0)]
    [InlineData("5 hours and 14 minutes", 0, 5, 14, 15.0)]
    [InlineData("90 minutes", 0, 1, 30, 0.0)]
    [InlineData("5 hours and 1 minute", 0, 5, 1, 0.0)]
    [InlineData("7 hours and 14 minutes", 0, 7, 14, 15.0)]
    [InlineData("5 days and 15 hours", 5, 14, 45, 9.0)]
    [InlineData("1 day and 15 hours", 1, 14, 45, 9.0)]
    [InlineData("5 days and 1 hour", 5, 0, 45, 9.0)]
    [InlineData("1 year and 149 days", 514, 0, 0, 0.0)]
    [InlineData("2 years and 51 days", 780, 23, 0, 0.0)]
    [InlineData("1 year and 51 days", 415, 23, 0, 0.0)]
    [InlineData("2 years and 1 day", 730, 24, 0, 0.0)]
    [InlineData("10 years", 3650, 0, 0, 0.0)]
    [InlineData("-100 nanoseconds", 0, 0, 0, -0.000_000_1)]
    [InlineData("-2 microseconds", 0, 0, 0, -0.000_002)]
    [InlineData("-2 milliseconds", 0, 0, 0, -0.002)]
    [InlineData("-500 microseconds", 0, 0, 0, -0.000_500)]
    [InlineData("-515 milliseconds", 0, 0, 0, -0.514_5)]
    [InlineData("-2 seconds", 0, 0, 0, -2.0)]
    [InlineData("-32 seconds", 0, 0, 0, -31.500)]
    [InlineData("-5 minutes and 14 seconds", 0, 0, -5, -14.0)]
    [InlineData("-90 seconds", 0, 0, -1, -30.0)]
    [InlineData("-5 minutes and 1 second", 0, 0, -5, -1.0)]
    [InlineData("-14 minutes and 15 seconds", 0, 0, -14, -15.0)]
    [InlineData("-5 hours and 14 minutes", 0, -5, -14, -15.0)]
    [InlineData("-90 minutes", 0, -1, -30, 0.0)]
    [InlineData("-5 hours and 1 minute", 0, -5, -1, 0.0)]
    [InlineData("-7 hours and 14 minutes", 0, -7, -14, -15.0)]
    [InlineData("-5 days and 15 hours", -5, -14, -45, -9.0)]
    [InlineData("-1 day and 15 hours", -1, -14, -45, -9.0)]
    [InlineData("-5 days and 1 hour", -5, 0, -45, -9.0)]
    [InlineData("-314 days", -314, 0, 0, 0.0)]
    [InlineData("-2 years and 51 days", -780, -23, 0, 0.0)]
    [InlineData("-1 year and 51 days", -415, -23, 0, 0.0)]
    [InlineData("-2 years and 1 day", -730, -24, 0, 0.0)]
    [InlineData("-10 years", -3650, 0, 0, 0.0)]
    public void ToHuman_ValidInput_Works(
            string expected,
            int d,
            int h,
            int m,
            double s)
    {
        long ticks = (TimeSpan.TicksPerDay * d)
                + (TimeSpan.TicksPerHour * h)
                + (TimeSpan.TicksPerMinute * m)
                + ((long)Math.Round(TimeSpan.TicksPerSecond * s, MidpointRounding.AwayFromZero));

        Assert.Equal(expected, new TimeSpan(ticks).ToHuman());
    }

    [Fact]
    public void ToHuman_MaxInput_Returns()
    {
        Assert.Equal("+\u221E", TimeSpan.MaxValue.ToHuman());
    }

    [Fact]
    public void ToHuman_MinInput_Returns()
    {
        Assert.Equal("-\u221E", TimeSpan.MinValue.ToHuman());
    }
}
