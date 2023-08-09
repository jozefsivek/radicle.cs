namespace Radicle.Common;

using System;
using System.Threading.Tasks;
using Xunit;

public class ETATest
{
    [Fact]
    public void Constructor_NonUTCDate_Throws()
    {
        Assert.Throws<ArgumentException>(() => new ETA(DateTime.Now));
    }

    [Fact]
    public void Report_NonUTCDate_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
                new ETA().Report(DateTime.Now, 1));
    }

    [Fact]
    public void FromProgress_ValidProgress_WiresInstances()
    {
        TransparentProgress<long> tp = new();
        ETA eta = ETA.FromProgress(tp, strategy: new AverageSpeedCalculation());

        tp.Total = 100;
        tp.Count = 10;

        Assert.Equal(100, eta.Total);
        Assert.Equal(10, eta.LastCount);

        tp.Total = 200;
        tp.Count = 20;

        Assert.Equal(200, eta.Total);
        Assert.Equal(20, eta.LastCount);
    }

    [Fact]
    public void GetETA_NoTotal_ReturnsTrivialStatus()
    {
        DateTime creation = DateTime.UtcNow;

        ETA eta = new();
        ETAStatus status = eta.GetETA();

        Assert.Equal(0, status.LastCount);
        Assert.Null(status.ETA);
        Assert.Equal(0.0, status.Speed);
        Assert.Null(status.Total);
        Assert.True(creation <= status.LastDate
                && status.LastDate <= DateTime.UtcNow);
        Assert.True(creation <= status.StatusDate
                && status.StatusDate <= DateTime.UtcNow);
    }

    [Fact]
    public void GetETA_WithTotalZeroSpeed_ReturnsMaxTimeSpan()
    {
        ETA eta = new(total: long.MaxValue);

        ETAStatus status = eta.GetETA();

        Assert.Equal(TimeSpan.MaxValue, status.ETA);
    }

    [Fact]
    public void GetETA_WithTotaTooLowSpeed_ReturnsMaxTimeSpan()
    {
        ETA eta = new(total: long.MaxValue);
        eta.Report(DateTime.UtcNow.AddYears(1), 1);

        ETAStatus status = eta.GetETA();

        Assert.Equal(TimeSpan.MaxValue, status.ETA);
    }

    [Fact]
    public void GetETA_WithTotal_ReturnsStatus()
    {
        DateTime date = DateTime.UtcNow.AddSeconds(-1);
        ETA eta = new(startDate: date, total: 100);

        // 2 to overcome the inbuild threshold
        eta.Report(date.AddSeconds(2), 2);

        ETAStatus status = eta.GetETA();

        Assert.Equal(date, status.StartDate);
        Assert.True(status.ETA.HasValue);
        Assert.Equal(98.0, status.ETA.Value.TotalSeconds, 3.0);
    }

    [Fact]
    public async Task GetETA_WithTotal_ReturnsStatusDependingOnCallTime()
    {
        DateTime date = DateTime.UtcNow.AddSeconds(-1);
        ETA eta = new(startDate: date, total: 100);
        eta.Report(DateTime.UtcNow.AddSeconds(1), 1);

        ETAStatus status = eta.GetETA();

        Assert.True(status.ETA.HasValue);

        TimeSpan eta1 = status.ETA.Value;

        await Task.Delay(1).ConfigureAwait(false);

        status = eta.GetETA();

        Assert.True(status.ETA.HasValue);

        TimeSpan eta2 = status.ETA.Value;

        Assert.True(eta2 < eta1);
    }
}
