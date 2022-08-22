namespace Radicle.Common.Extensions;

using System;
using Xunit;

public class DateTimeExtensionsTest
{
    [Fact]
    public void IsOlderThan_ZeroDuration_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
                () => DateTime.UtcNow.IsOlderThan(TimeSpan.Zero));
    }

    [Fact]
    public void IsOlderThan_NonUTC_Throws()
    {
        Assert.Throws<ArgumentException>(
                () => new DateTime(1000_000, DateTimeKind.Local).IsOlderThan(TimeSpan.Zero));
    }

    [Fact]
    public void IsOlderThan_NegativeDuration_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
                () => DateTime.UtcNow.IsOlderThan(TimeSpan.FromMinutes(-1.0)));
    }

    [Fact]
    public void IsOlderThan_ZeroDurationOffset_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
                () => DateTimeOffset.UtcNow.IsOlderThan(TimeSpan.Zero));
    }

    [Fact]
    public void IsOlderThan_NegativeDurationOffset_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
                () => DateTimeOffset.UtcNow.IsOlderThan(TimeSpan.FromMinutes(-1.0)));
    }

    [Fact]
    public void IsOlderThan_OldDateTime_ReturnsTrue()
    {
        DateTime old = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1));

        Assert.True(old.IsOlderThan(TimeSpan.FromMinutes(1)));
    }

    [Fact]
    public void IsOlderThan_RecentDateTime_ReturnsFalse()
    {
        DateTime old = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(1));

        Assert.False(old.IsOlderThan(TimeSpan.FromDays(1)));
    }

    [Fact]
    public void IsOlderThan_OldDateTimeOffset_ReturnsTrue()
    {
        DateTimeOffset old = DateTimeOffset.Now.Subtract(TimeSpan.FromDays(1));

        Assert.True(old.IsOlderThan(TimeSpan.FromMinutes(1)));
    }

    [Fact]
    public void IsOlderThan_RecentDateTimeOffset_ReturnsFalse()
    {
        DateTimeOffset old = DateTimeOffset.Now.Subtract(TimeSpan.FromMinutes(1));

        Assert.False(old.IsOlderThan(TimeSpan.FromDays(1)));
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(+1.0)]
    [InlineData(+4.5)]
    [InlineData(9)]
    [InlineData(-10)]
    [InlineData(-13.0)]
    [InlineData(-14.0)]
    [InlineData(14.0)]
    public void IsOlderThan_DateTimeOffsetWithZone_Works(double offset)
    {
        DateTimeOffset date = DateTimeOffset.UtcNow
                .Subtract(TimeSpan.FromMinutes(5))
                .ToOffset(TimeSpan.FromHours(offset));

        Assert.True(date.IsOlderThan(TimeSpan.FromSeconds(3)));
        Assert.False(date.IsOlderThan(TimeSpan.FromHours(3)));

        date = date.AddDays(1); // test also future dates

        Assert.False(date.IsOlderThan(TimeSpan.FromSeconds(3)));
        Assert.False(date.IsOlderThan(TimeSpan.FromHours(3)));
    }
}
