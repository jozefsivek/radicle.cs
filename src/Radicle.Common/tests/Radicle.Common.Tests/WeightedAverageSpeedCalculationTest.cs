namespace Radicle.Common;

using System;
using Xunit;

public class WeightedAverageSpeedCalculationTest
{
    [Fact]
    public void Constructor_ZeroWindowLength_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new WeightedAverageSpeedCalculation(0));
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.NegativeInfinity)]
    [InlineData(double.PositiveInfinity)]
    public void Constructor_WeigthWithNoActualValue_Throws(double value)
    {
        Assert.Throws<ArgumentException>(() =>
                new WeightedAverageSpeedCalculation(1, new double[] { value }));
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.NegativeInfinity)]
    [InlineData(double.PositiveInfinity)]
    public void Constructor_DefaultWeigthWithNoActualValue_Throws(double value)
    {
        Assert.Throws<ArgumentException>(() =>
                new WeightedAverageSpeedCalculation(1, defaultWeight: value));
    }

    [Fact]
    public void Constructor_OverflowingWeights_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
                new WeightedAverageSpeedCalculation(
                    2,
                    weights: new double[] { double.MaxValue, double.MaxValue }));
    }

    [Fact]
    public void StartDate_CallToInit_IsSet()
    {
        DateTime date = DateTime.UtcNow;

        WeightedAverageSpeedCalculation calc = new();

        calc.Init(date);

        Assert.Equal(date, calc.StartDate);
    }

    [Fact]
    public void LastDate_CallToReport_IsSet()
    {
        DateTime date = DateTime.UtcNow;
        DateTime date2 = date.AddDays(1);

        WeightedAverageSpeedCalculation calc = new();

        calc.Report(date, 1);
        calc.Report(date2, 2);

        Assert.Equal(date2, calc.LastDate);
    }

    [Fact]
    public void LastCount_CallToReport_IsSet()
    {
        DateTime date = DateTime.UtcNow;
        DateTime date2 = date.AddDays(1);

        WeightedAverageSpeedCalculation calc = new();

        calc.Report(date, 1);
        calc.Report(date2, 2);

        Assert.Equal(2, calc.LastCount);
    }

    [Fact]
    public void GetSpeed_NewInstance_ReturnsZero()
    {
        WeightedAverageSpeedCalculation calc = new();

        Assert.Equal(0.0, calc.GetSpeed());
    }

    [Fact]
    public void GetSpeed_NewInitializedInstance_ReturnsZero()
    {
        WeightedAverageSpeedCalculation calc = new();

        calc.Init(DateTime.UtcNow);

        Assert.Equal(0.0, calc.GetSpeed());
    }

    [Fact]
    public void GetSpeed_NegativeCount_ReturnsZero()
    {
        WeightedAverageSpeedCalculation calc = new();

        calc.Init(DateTime.UtcNow);
        calc.Report(DateTime.UtcNow, -1);

        Assert.Equal(0.0, calc.GetSpeed());
    }

    [Fact]
    public void GetSpeed_NegativeTimeIncrement_ReturnsZero()
    {
        WeightedAverageSpeedCalculation calc = new();

        calc.Init(DateTime.UtcNow);
        calc.Report(DateTime.UtcNow.AddDays(-1), 1);

        Assert.Equal(0.0, calc.GetSpeed());
    }

    [Fact]
    public void GetSpeed_ValidReport_ReturnsSpeed()
    {
        WeightedAverageSpeedCalculation calc = new();
        DateTime date = DateTime.UtcNow;
        DateTime date2 = date.AddSeconds(2);

        calc.Init(date);
        calc.Report(date2, 12);

        Assert.Equal(12.0 / 2.0, calc.GetSpeed());
    }

    [Theory]
    [InlineData(
            new double[] { 7, 6, 5, 4, 3, 2, 1 },
            0.0,
            new long[] { 1, 2, 3 },
            ((1 * 7) + (1 * 6) + (1 * 5)) / (double)(7 + 6 + 5))]
    [InlineData(
            new double[] { 7, 6 },
            0.0,
            new long[] { 1, 2, 3, 4 },
            ((1 * 7) + (1 * 6)) / (double)(7 + 6))]
    [InlineData(
            new double[] { 7, 6 },
            3.0,
            new long[] { 1, 2, 3, 4 },
            ((1 * 7) + (1 * 6) + (1 * 3) + (1 * 3)) / (double)(7 + 6 + 3 + 3))]
    public void GetSpeed_ValidConsecutiveReport_ReturnsSpeed(
            double[] weights,
            double defaultWeigth,
            long[] counts,
            double expected)
    {
        WeightedAverageSpeedCalculation calc = new(
                5,
                weights,
                defaultWeight: defaultWeigth,
                frequencyThreshold: TimeSpan.Zero);
        DateTime date = DateTime.UtcNow;
        calc.Init(date);

        DateTime nextDate = date;

        foreach (long count in counts)
        {
            nextDate = nextDate.AddSeconds(1);
            calc.Report(nextDate, count);
        }

        Assert.Equal(expected, calc.GetSpeed());
    }

    [Fact]
    public void GetSpeed_MixedFrequencySamplesDefaultSetup_ReturnsStableResult()
    {
        WeightedAverageSpeedCalculation calc = new();
        DateTime date = DateTime.UtcNow;
        RollingCollection<double> speeds = new(16);
        int count = 0;
        calc.Init(date);

        DateTime nextDate = date;

        for (int lf = 0; lf < 20; lf++)
        {
            nextDate = nextDate.AddSeconds(10);

            for (int hf = 0; hf < 3; hf++)
            {
                nextDate = nextDate.AddSeconds(1);
                calc.Report(nextDate, ++count);

                speeds.Add(calc.GetSpeed());
            }
        }

        double averageSpeed = count / (nextDate - date).TotalSeconds;
        int index = 1;

        // we expect numbers around 0.23 count/s
        foreach (double speed in speeds)
        {
            Assert.Equal(averageSpeed, speed, 0.1 / index++);
        }
    }

    [Fact]
    public void Init_NonUTCDate_Throws()
    {
        Assert.Throws<ArgumentException>(() => new WeightedAverageSpeedCalculation().Init(DateTime.Now));
    }

    [Fact]
    public void Init_PastDate_DoesNotThrow()
    {
        new WeightedAverageSpeedCalculation().Init(DateTime.UtcNow.AddDays(-1));
    }

    [Fact]
    public void Report_NonUTCDate_Throws()
    {
        Assert.Throws<ArgumentException>(() => new WeightedAverageSpeedCalculation().Report(DateTime.Now, 0));
    }

    [Fact]
    public void Report_PastDate_DoesNotThrow()
    {
        new WeightedAverageSpeedCalculation().Report(DateTime.UtcNow.AddDays(-1), 0);
    }

    [Fact]
    public void Report_PastDateRelativeToLast_DoesRecord()
    {
        WeightedAverageSpeedCalculation calc = new();
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
