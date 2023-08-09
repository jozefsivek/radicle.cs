namespace Radicle.Common;

using System;
using Xunit;

public class AverageSpeedCalculationTest
{
    [Fact]
    public void StartDate_CallToInit_IsSet()
    {
        DateTime date = DateTime.UtcNow;

        AverageSpeedCalculation calc = new();

        calc.Init(date);

        Assert.Equal(date, calc.StartDate);
    }

    [Fact]
    public void LastDate_CallToReport_IsSet()
    {
        DateTime date = DateTime.UtcNow;
        DateTime date2 = date.AddDays(1);

        AverageSpeedCalculation calc = new();

        calc.Report(date, 1);
        calc.Report(date2, 2);

        Assert.Equal(date2, calc.LastDate);
    }

    [Fact]
    public void LastCount_CallToReport_IsSet()
    {
        DateTime date = DateTime.UtcNow;
        DateTime date2 = date.AddDays(1);

        AverageSpeedCalculation calc = new();

        calc.Report(date, 1);
        calc.Report(date2, 2);

        Assert.Equal(2, calc.LastCount);
    }

    [Fact]
    public void GetSpeed_NewInstance_ReturnsZero()
    {
        AverageSpeedCalculation calc = new();

        Assert.Equal(0.0, calc.GetSpeed());
    }

    [Fact]
    public void GetSpeed_NewInitializedInstance_ReturnsZero()
    {
        AverageSpeedCalculation calc = new();

        calc.Init(DateTime.UtcNow);

        Assert.Equal(0.0, calc.GetSpeed());
    }

    [Fact]
    public void GetSpeed_NegativeCount_ReturnsZero()
    {
        AverageSpeedCalculation calc = new();

        calc.Init(DateTime.UtcNow);
        calc.Report(DateTime.UtcNow, -1);

        Assert.Equal(0.0, calc.GetSpeed());
    }

    [Fact]
    public void GetSpeed_NegativeTimeIncrement_ReturnsZero()
    {
        AverageSpeedCalculation calc = new();

        calc.Init(DateTime.UtcNow);
        calc.Report(DateTime.UtcNow.AddDays(-1), 1);

        Assert.Equal(0.0, calc.GetSpeed());
    }

    [Fact]
    public void GetSpeed_ValidReport_ReturnsSpeed()
    {
        AverageSpeedCalculation calc = new();
        DateTime date = DateTime.UtcNow;
        DateTime date2 = date.AddSeconds(2);

        calc.Init(date);
        calc.Report(date2, 12);

        Assert.Equal(12.0 / 2.0, calc.GetSpeed());
    }

    [Fact]
    public void GetSpeed_ValidConsecutiveReport_ReturnsSpeed()
    {
        AverageSpeedCalculation calc = new();
        DateTime date = DateTime.UtcNow;
        DateTime date2 = date.AddSeconds(1);
        DateTime date3 = date2.AddSeconds(1);

        calc.Init(date);
        calc.Report(date2, 12);
        calc.Report(date3, 16);

        Assert.Equal(16.0 / 2.0, calc.GetSpeed());
    }

    [Fact]
    public void Init_NonUTCDate_Throws()
    {
        Assert.Throws<ArgumentException>(() => new AverageSpeedCalculation().Init(DateTime.Now));
    }

    [Fact]
    public void Init_PastDate_DoesNotThrow()
    {
        new AverageSpeedCalculation().Init(DateTime.UtcNow.AddDays(-1));
    }

    [Fact]
    public void Report_NonUTCDate_Throws()
    {
        Assert.Throws<ArgumentException>(() => new AverageSpeedCalculation().Report(DateTime.Now, 0));
    }

    [Fact]
    public void Report_PastDate_DoesNotThrow()
    {
        new AverageSpeedCalculation().Report(DateTime.UtcNow.AddDays(-1), 0);
    }

    [Fact]
    public void Report_PastDateRelativeToLast_DoesRecord()
    {
        AverageSpeedCalculation calc = new();
        DateTime date = DateTime.UtcNow;
        DateTime date2 = date.AddDays(1);
        DateTime date3 = date.AddDays(-1);

        calc.Init(date);
        calc.Report(date2, 12);
        calc.Report(date3, 16); // ignored as it is in past
        calc.Report(date2, 27); // ignored as it is in past

        Assert.Equal(date2, calc.LastDate);
        Assert.Equal(12, calc.LastCount);
    }
}
