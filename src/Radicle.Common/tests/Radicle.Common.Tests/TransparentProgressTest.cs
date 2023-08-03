namespace Radicle.Common;

using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

public class TransparentProgressTest
{
    [Fact]
    public void ProgressChanged_TotalChange_Reports()
    {
        TransparentProgress<int> progress = new();
        object? sender = default;
        ProgressEventArgs<int> eventArgs = default!;

        progress.ProgressChanged += (object? s, ProgressEventArgs<int> e) =>
        {
            sender = s;
            eventArgs = e;
        };

        progress.SetTotal(100);

        Assert.Equal(progress, sender);
        Assert.Equal(0, eventArgs.Count);
        Assert.Equal(100, eventArgs.Total);
        Assert.Equal(progress.LastReport.Date, eventArgs.Date);
    }

    [Fact]
    public void ProgressChanged_CountChange_Reports()
    {
        TransparentProgress<int> progress = new();
        object? sender = default;
        ProgressEventArgs<int> eventArgs = default!;

        progress.SetTotal(100);

        progress.ProgressChanged += (object? s, ProgressEventArgs<int> e) =>
        {
            sender = s;
            eventArgs = e;
        };

        progress.Report(10);

        Assert.Equal(progress, sender);
        Assert.Equal(10, eventArgs.Count);
        Assert.Equal(100, eventArgs.Total);
        Assert.Equal(progress.LastReport.Date, eventArgs.Date);
    }

    [Fact]
    public void ProgressChanged_CountIncrement_Reports()
    {
        TransparentProgress<int> progress = new();
        object? sender = default;
        ProgressEventArgs<int> eventArgs = default!;

        progress.SetTotal(100);

        progress.ProgressChanged += (object? s, ProgressEventArgs<int> e) =>
        {
            sender = s;
            eventArgs = e;
        };

        progress.IncrementCount(10);

        Assert.Equal(progress, sender);
        Assert.Equal(10, eventArgs.Count);
        Assert.Equal(100, eventArgs.Total);
        Assert.Equal(progress.LastReport.Date, eventArgs.Date);
    }

    [Fact]
    public void LastReport_CountChange_Works()
    {
        TransparentProgress<int> progress = new();

        progress.Report(10);

        Assert.Equal(10, progress.LastReport.Count);
        Assert.True(progress.LastReport.Date <= DateTime.UtcNow);
    }

    [Fact]
    public void LastReport_CountIncrement_Works()
    {
        TransparentProgress<int> progress = new();

        progress.IncrementCount(10);

        Assert.Equal(10, progress.LastReport.Count);
        Assert.True(progress.LastReport.Date <= DateTime.UtcNow);
    }

    [Fact]
    public void Count_NewInstance_IsZero()
    {
        Assert.Equal(0, new TransparentProgress<int>().Count);
    }

    [Fact]
    public void Total_NewInstance_IsNull()
    {
        Assert.Null(new TransparentProgress<int>().Total);
    }

    [Fact]
    public void StageName_NullValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                new TransparentProgress<int>() { StageName = null! });
    }

    [Theory]
    [InlineData("\n")]
    [InlineData("\n\r")]
    [InlineData("\r\n")]
    [InlineData("\r")]
    public void StageName_MultiLineValue_Throws(string invalidName)
    {
        Assert.Throws<ArgumentException>(() =>
                new TransparentProgress<int>() { StageName = invalidName });
    }

    [Fact]
    public void StageName_LongValue_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                new TransparentProgress<int>() { StageName = new string('a', 65) });
    }

    [Fact]
    public void Status_NullValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                new TransparentProgress<int>() { Status = null! });
    }

    [Theory]
    [InlineData("\n")]
    [InlineData("\n\r")]
    [InlineData("\r\n")]
    [InlineData("\r")]
    public void Status_MultiLineValue_Throws(string invalidStatus)
    {
        Assert.Throws<ArgumentException>(() =>
                new TransparentProgress<int>() { Status = invalidStatus });
    }

    [Fact]
    public void Status_LongValue_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                new TransparentProgress<int>() { Status = new string('a', 1025) });
    }

    [Fact]
    public void From_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                TransparentProgress<int>.From(null!));
    }

    [Fact]
    public void From_GenericProgress_Wraps()
    {
        Progress<int> progress = new();
        int calledValue = default;
        using SemaphoreSlim semaphore = new(0, 1);

        progress.ProgressChanged += (object? _, int e) =>
        {
            calledValue = e;
            semaphore.Release();
        };

        ITransparentProgress<int> tp = TransparentProgress<int>.From(progress);

        tp.Report(12);

        // the callbacks are happening asynchronously
        semaphore.Wait();
        Assert.Equal(12, calledValue);
    }

    [Fact]
    public void From_TransparentProgress_ReturnsInput()
    {
        TransparentProgress<int> expected = new();

        ITransparentProgress<int> tp = TransparentProgress<int>.From(expected);

        Assert.Equal(expected, tp);
    }

    [Fact]
    public void FromOrDefault_NullInput_ReturnsNull()
    {
        Assert.Null(TransparentProgress<int>.FromOrDefault(null));
    }

    [Fact]
    public void FromOrDefault_GenericProgress_Wraps()
    {
        Progress<int> progress = new();
        int calledValue = default;
        using SemaphoreSlim semaphore = new(0, 1);

        progress.ProgressChanged += (object? _, int e) =>
        {
            calledValue = e;
            semaphore.Release();
        };

        ITransparentProgress<int>? tp = TransparentProgress<int>.FromOrDefault(progress);

        tp?.Report(12);

        // the callbacks are happening asynchronously
        semaphore.Wait();
        Assert.Equal(12, calledValue);
    }

    [Fact]
    public void IncrementCount_SupportedTypes_DoesNotThrow()
    {
        new TransparentProgress<int>().IncrementCount(1);
        new TransparentProgress<long>().IncrementCount(1L);
        new TransparentProgress<double>().IncrementCount(1.0);
    }

    [Fact]
    public void IncrementTotal_SupportedTypes_DoesNotThrow()
    {
        new TransparentProgress<int>().IncrementTotal(1);
        new TransparentProgress<long>().IncrementTotal(1L);
        new TransparentProgress<double>().IncrementTotal(1.0);
    }

    [Fact]
    public void IncrementCount_UnsupportedTypes_Throws()
    {
        Assert.Throws<NotSupportedException>(() => new TransparentProgress<byte>().IncrementCount(1));
    }

    [Fact]
    public void IncrementTotal_UnsupportedTypes_Throws()
    {
        Assert.Throws<NotSupportedException>(() => new TransparentProgress<byte>().IncrementTotal(1));
    }

    [Fact]
    public void IncrementCount_ValidValue_IncrementsAndInvokes()
    {
        TransparentProgress<long> tp = new();
        List<long> reports = new();

        tp.ProgressChanged += (object? sender, ProgressEventArgs<long> e) =>
                reports.Add(e.Count);

        Assert.Equal(0, tp.Count);
        Assert.Null(tp.Total);

        Assert.Equal(10, tp.IncrementCount(10));

        Assert.Equal(10, tp.Count);

        Assert.Equal(-10, tp.IncrementCount(-20));

        Assert.Equal(-10, tp.Count);

        Assert.Equal(90, tp.IncrementCount(100));

        Assert.Equal(90, tp.Count);
        Assert.Equal(new long[] { 10, -10, 90 }, reports);
    }

    [Fact]
    public void IncrementTotal_ValidValue_Increments()
    {
        TransparentProgress<long> tp = new();

        Assert.Null(tp.Total);

        Assert.Equal(10, tp.IncrementTotal(10));

        Assert.Equal(10, tp.Total);

        Assert.Equal(-10, tp.IncrementTotal(-20));

        Assert.Equal(-10, tp.Total);

        Assert.Equal(90, tp.IncrementTotal(100));

        Assert.Equal(90, tp.Total);
    }

    [Fact]
    public void SetCount_ValidValue_SetsAndInvokes()
    {
        TransparentProgress<short> tp = new();
        List<short> reports = new();

        tp.ProgressChanged += (object? sender, ProgressEventArgs<short> e) =>
                reports.Add(e.Count);

        Assert.Equal(0, tp.Count);
        Assert.Null(tp.Total);

        tp.SetCount(10);

        Assert.Equal(10, tp.Count);

        tp.SetCount(-20);

        Assert.Equal(-20, tp.Count);

        tp.SetCount(100);

        Assert.Equal(100, tp.Count);
        Assert.Equal(new short[] { 10, -20, 100 }, reports);
    }

    [Fact]
    public void SetTotal_ValidValue_Sets()
    {
        TransparentProgress<short> tp = new();

        Assert.Null(tp.Total);

        tp.SetTotal(10);

        Assert.Equal((short)10, tp.Total);

        tp.SetTotal(-20);

        Assert.Equal((short)-20, tp.Total);

        tp.SetTotal(100);

        Assert.Equal((short)100, tp.Total);
    }

    [Fact]
    public void GetContributor_ReturnsItself()
    {
        TransparentProgress<long> expected = new();

        Assert.Equal(expected, expected.GetContributor());
    }

    [Fact]
    public void CreateChildStage_NullName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                new TransparentProgress<int>().CreateChildStage(null!));
    }

    [Fact]
    public void CreateChildStage_InvalidName_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
                new TransparentProgress<int>().CreateChildStage("new\nline"));
    }

    [Fact]
    public void CreateChildStage_TooLongName_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                new TransparentProgress<int>().CreateChildStage(new string('a', 65)));
    }

    [Fact]
    public void CreateChildStage_OneChild_PropagatesChanges()
    {
        TransparentProgress<long> tp = new()
        {
            Total = 100,
        };

        tp.Report(10);

        TransparentProgress<long> child = tp.CreateChildStage("child");

        child.Total = 100;
        child.Count = 10;

        Assert.Equal(200, tp.Total);
        Assert.Equal(20, tp.Count);

        Assert.Equal(100, child.Total);
        Assert.Equal(10, child.Count);

        child.Total = 300;
        child.Count = 30;

        Assert.Equal(400, tp.Total);
        Assert.Equal(40, tp.Count);

        Assert.Equal(300, child.Total);
        Assert.Equal(30, child.Count);

        child.Total = null;
        child.IncrementCount(1);

        Assert.Equal(100, tp.Total);
        Assert.Equal(41, tp.Count);

        Assert.Null(child.Total);
        Assert.Equal(31, child.Count);
    }

    [Fact]
    public void CreateChildStage_SecondChild_LinksPrevious()
    {
        TransparentProgress<long> tp = new();

        TransparentProgress<long> first = tp.CreateChildStage("first");
        TransparentProgress<long> second = tp.CreateChildStage("second");

        Assert.Null(first.PreviousStage);
        Assert.Equal(first, second.PreviousStage);
    }

    [Fact]
    public void CreateChildStage_DeepNesting_PropagatesChanges()
    {
        TransparentProgress<double> tp = new();

        tp.Report(10);

        TransparentProgress<double> first = tp.CreateChildStage("first");

        first.IncrementTotal(100);

        TransparentProgress<double> second = tp.CreateChildStage("second");

        second.IncrementCount(10);

        TransparentProgress<double> subsecond = second.CreateChildStage("subsecond");

        subsecond.SetTotal(100);
        subsecond.SetCount(20);

        Assert.Equal(200.0, tp.Total);
        Assert.Equal(40.0, tp.Count);
    }

    [Theory]
    [InlineData("", "", "")]
    [InlineData(" ", "", "")]
    [InlineData("", " ", "")]
    [InlineData("foo", "", "foo")]
    [InlineData("", "bar", "bar")]
    [InlineData("foo", "bar", "foo: bar")]
    public void GetDescription_Progress_ReturnsDescription(
            string stage,
            string status,
            string expected)
    {
        TransparentProgress<long> tp = new()
        {
            StageName = stage,
            Status = status,
        };

        Assert.Equal(expected, tp.GetDescription());
    }

    [Fact]
    public void GetDescription_HierarchicalProgress_ReturnsDescription()
    {
        TransparentProgress<long> tp = new()
        {
            StageName = "a",
            Status = "foo",
        };

        TransparentProgress<long> child = tp.CreateChildStage("b");
        child.Status = "bar";

        Assert.Equal("a > b: bar", tp.GetDescription());
    }

    [Fact]
    public void GetDescription_SubHierarchicalProgress_ReturnsDescription()
    {
        TransparentProgress<long> tp = new()
        {
            Status = "foo",
        };

        TransparentProgress<long> child = tp.CreateChildStage("b");
        child.Status = "Bar";

        TransparentProgress<long> child2 = child.CreateChildStage("c");
        child2.Status = "baR";

        TransparentProgress<long> child3 = child2.CreateChildStage(string.Empty);
        child3.Status = "bar";

        Assert.Equal("b > c: bar", tp.GetDescription());
    }
}
