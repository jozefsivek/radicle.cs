namespace Radicle.Common;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

public class MetronomeTest
{
    [Fact]
    public void Constructor_NullSource_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                new Metronome(null!));
    }

    [Fact]
    public void IsTime_AfterCreation_ReturnsFalse()
    {
        Metronome metronome = Metronome.CreateForLiveUpdate();

        Assert.False(metronome.IsTime());
    }

    [Fact]
    public async Task IsTime_BeforePeriod_ReturnsFalse()
    {
        Metronome metronome = new(TimeSpan.FromMinutes(1.0));

        await Task.Delay(1).ConfigureAwait(false);

        Assert.False(metronome.IsTime());
    }

    [Fact]
    public async Task IsTime_AfterPeriod_ReturnsTrue()
    {
        Metronome metronome = new(TimeSpan.FromMilliseconds(0.1));

        await Task.Delay(1).ConfigureAwait(false);

        Assert.True(metronome.IsTime());
    }

    [Fact]
    public void TimeToNext_ReturnsTimeToNextTick()
    {
        Metronome metronome = new(TimeSpan.FromMinutes(1.0));

        TimeSpan actual = metronome.TimeToNext();

        Assert.Equal(1.0, actual.TotalMinutes, 0.1);
    }

    [Fact]
    public async Task DelayAsync_ShortPeriod_DelaysAtLeastOneMs()
    {
        Metronome metronome = new(TimeSpan.FromMilliseconds(1.0));

        Stopwatch sw = Stopwatch.StartNew();

        await metronome.DelayAsync().ConfigureAwait(false);

        sw.Stop();

        Assert.True(sw.Elapsed > TimeSpan.FromMilliseconds(1.0) && sw.Elapsed < TimeSpan.FromMilliseconds(10.0));
    }

    [Fact]
    public async Task Reset_ResetsSource()
    {
        Metronome metronome = new(TimeSpan.FromMilliseconds(2.0), rampUp: true);

        TimeSpan actual = metronome.TimeToNext();

        Assert.True(actual.TotalMilliseconds <= 0.5);

        await Task.Delay(1).ConfigureAwait(false);

        Assert.True(metronome.IsTime());

        metronome.Reset();

        actual = metronome.TimeToNext();

        Assert.True(actual.TotalMilliseconds <= 0.5);
    }
}
