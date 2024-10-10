namespace Radicle.Common.Profiling.Models;

using System;
using Xunit;

public class ProfilingEventArgsTest
{
    [Fact]
    public void From_NullInstance_Throws()
    {
        EventCategoryName category = new("test_category");

        Assert.Throws<ArgumentNullException>(() => ProfilingEventArgs.From(
                null!,
                severity: EventSeverity.Info,
                exception: null,
                category: category,
                message: null));
    }

    [Fact]
    public void From_NullCategory_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ProfilingEventArgs.From(
                new TestService(),
                severity: EventSeverity.Info,
                exception: null,
                category: null!,
                message: null));
    }

    [Fact]
    public void Contains_NullInput_ReturnsFalse()
    {
        (_, _, _, _, ProfilingEventArgs e) = CreateFirstEvent(EventSeverity.Info);

        Assert.False(e.Contains(null));
    }

    [Fact]
    public void Contains_Self_ReturnsTrue()
    {
        (_, _, _, _, ProfilingEventArgs e) = CreateFirstEvent(EventSeverity.Info);

        Assert.True(e.Contains(e));
    }

    [Fact]
    public void Contains_OtherLookAlikeInput_ReturnsFalse()
    {
        (_, _, _, _, ProfilingEventArgs e) = CreateFirstEvent(EventSeverity.Info);
        (_, _, _, _, ProfilingEventArgs e2) = CreateFirstEvent(EventSeverity.Info);

        Assert.False(e.Contains(e2));
    }

    [Fact]
    public void Contains_PreviousInput_ReturnsTrue()
    {
        (_, _, _, _, ProfilingEventArgs previous) = CreateFirstEvent(EventSeverity.Info);
        ProfilingEventArgs next = ProfilingEventArgs.Continue(previous, null, null);

        Assert.True(next.Contains(previous));
    }

    [Fact]
    public void Contains_PreviousPreviousInput_ReturnsTrue()
    {
        (_, _, _, _, ProfilingEventArgs previous2) = CreateFirstEvent(EventSeverity.Info);
        ProfilingEventArgs previous = ProfilingEventArgs.Continue(previous2, null, null);
        ProfilingEventArgs self = ProfilingEventArgs.End(previous, null, null);

        Assert.True(self.Contains(previous2));
    }

    [Theory]
    [InlineData(EventSeverity.Debug)]
    [InlineData(EventSeverity.Error)]
    [InlineData(EventSeverity.Fatal)]
    [InlineData(EventSeverity.Info)]
    [InlineData(EventSeverity.Trace)]
    [InlineData(EventSeverity.Warning)]
    public void From_ValidInput_CreatesStandAloneEvent(EventSeverity severity)
    {
        EventCategoryName category = new("test_category");
        TestService service = new();
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
        ArgumentNullException exception = new("param", "exc");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
        const string message = "{foo} is {bar}";

        ProfilingEventArgs e = ProfilingEventArgs.From(
                service,
                severity: severity,
                exception: exception,
                category: category,
                message: message,
                "foo",
                2);

        Assert.Equal(new object[] { "foo", 2 }, e.Arguments);
        Assert.Equal(category, e.CategoryName);
        Assert.Equal(EventContinuity.StandAlone, e.Continuity);
        Assert.Equal(DateTime.UtcNow, e.CreatedOn, TimeSpan.FromSeconds(8));
        Assert.Equal(TimeSpan.Zero, e.Elapsed);
        Assert.Equal(exception, e.Exception);
        Assert.Equal(e, e.First);
        Assert.Equal(message, e.Message);
        Assert.Null(e.Previous);
        Assert.Equal(severity, e.Severity);
        Assert.Equal(service.EventSourceName, e.SourceName);
    }

    [Fact]
    public void StartFrom_NullInstance_Throws()
    {
        EventCategoryName category = new("test_category");

        Assert.Throws<ArgumentNullException>(() => ProfilingEventArgs.StartFrom(
                null!,
                severity: EventSeverity.Info,
                exception: null,
                category: category,
                message: null));
    }

    [Fact]
    public void StartFrom_NullCategory_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ProfilingEventArgs.StartFrom(
                new TestService(),
                severity: EventSeverity.Info,
                exception: null,
                category: null!,
                message: null));
    }

    [Theory]
    [InlineData(EventSeverity.Debug)]
    [InlineData(EventSeverity.Error)]
    [InlineData(EventSeverity.Fatal)]
    [InlineData(EventSeverity.Info)]
    [InlineData(EventSeverity.Trace)]
    [InlineData(EventSeverity.Warning)]
    public void StartFrom_ValidInput_CreatesFirstEvent(EventSeverity severity)
    {
        (EventCategoryName category, Exception exception, string message, INamedForProfiling service, ProfilingEventArgs e) =
                CreateFirstEvent(severity);

        Assert.Equal(new object[] { "foo", 2 }, e.Arguments);
        Assert.Equal(category, e.CategoryName);
        Assert.Equal(EventContinuity.First, e.Continuity);
        Assert.Equal(DateTime.UtcNow, e.CreatedOn, TimeSpan.FromSeconds(8));
        Assert.Equal(TimeSpan.Zero, e.Elapsed);
        Assert.Equal(exception, e.Exception);
        Assert.Equal(e, e.First);
        Assert.Equal(message, e.Message);
        Assert.Null(e.Previous);
        Assert.Equal(severity, e.Severity);
        Assert.Equal(service.EventSourceName, e.SourceName);
    }

    [Fact]
    public void Continue_NullPrevious_Throws()
    {
        EventCategoryName category = new("test_category");

        Assert.Throws<ArgumentNullException>(() => ProfilingEventArgs.Continue(
                null!,
                exception: null,
                message: null));
    }

    [Theory]
    [InlineData(EventSeverity.Debug)]
    [InlineData(EventSeverity.Error)]
    [InlineData(EventSeverity.Fatal)]
    [InlineData(EventSeverity.Info)]
    [InlineData(EventSeverity.Trace)]
    [InlineData(EventSeverity.Warning)]
    public void Continue_ValidInput_CreatesIntermediateEvent(EventSeverity severity)
    {
        (EventCategoryName category, _, string message, INamedForProfiling service, ProfilingEventArgs first) =
                CreateFirstEvent(severity);

        ProfilingEventArgs e = ProfilingEventArgs.Continue(
                first,
                exception: null,
                message: null);

        Assert.Equal(new object[] { "foo", 2 }, e.Arguments);
        Assert.Equal(category, e.CategoryName);
        Assert.Equal(EventContinuity.Intermediate, e.Continuity);
        Assert.Equal(DateTime.UtcNow, e.CreatedOn, TimeSpan.FromSeconds(8));
        Assert.Equal(e.CreatedOn - first.CreatedOn, e.Elapsed);
        Assert.Null(e.Exception);
        Assert.Equal(first, e.First);
        Assert.Equal(message, e.Message);
        Assert.Equal(first, e.Previous);
        Assert.Equal(severity, e.Severity);
        Assert.Equal(service.EventSourceName, e.SourceName);
    }

    [Fact]
    public void Continue_ValidInputWithOverrides_CreatesIntermediateEvent()
    {
        const EventSeverity severity = EventSeverity.Info;
        (EventCategoryName category, _, _, INamedForProfiling service, ProfilingEventArgs first) =
                CreateFirstEvent(severity);

        const string messageOverride = "{foo} is {bar} 2";

        ProfilingEventArgs e = ProfilingEventArgs.Continue(
                first,
                exception: null,
                message: messageOverride,
                "bar",
                4);

        Assert.Equal(new object[] { "bar", 4 }, e.Arguments);
        Assert.Equal(category, e.CategoryName);
        Assert.Equal(EventContinuity.Intermediate, e.Continuity);
        Assert.Equal(DateTime.UtcNow, e.CreatedOn, TimeSpan.FromSeconds(8));
        Assert.Equal(e.CreatedOn - first.CreatedOn, e.Elapsed);
        Assert.Null(e.Exception);
        Assert.Equal(first, e.First);
        Assert.Equal(messageOverride, e.Message);
        Assert.Equal(first, e.Previous);
        Assert.Equal(severity, e.Severity);
        Assert.Equal(service.EventSourceName, e.SourceName);
    }

    [Fact]
    public void End_NullPrevious_Throws()
    {
        EventCategoryName category = new("test_category");

        Assert.Throws<ArgumentNullException>(() => ProfilingEventArgs.End(
                null!,
                exception: null,
                message: null));
    }

    [Theory]
    [InlineData(EventSeverity.Debug)]
    [InlineData(EventSeverity.Error)]
    [InlineData(EventSeverity.Fatal)]
    [InlineData(EventSeverity.Info)]
    [InlineData(EventSeverity.Trace)]
    [InlineData(EventSeverity.Warning)]
    public void End_ValidInput_CreatesLastEvent(EventSeverity severity)
    {
        (EventCategoryName category, _, string message, INamedForProfiling service, ProfilingEventArgs first) =
                CreateFirstEvent(severity);

        ProfilingEventArgs e = ProfilingEventArgs.End(
                first,
                exception: null,
                message: null);

        Assert.Equal(new object[] { "foo", 2 }, e.Arguments);
        Assert.Equal(category, e.CategoryName);
        Assert.Equal(EventContinuity.Last, e.Continuity);
        Assert.Equal(DateTime.UtcNow, e.CreatedOn, TimeSpan.FromSeconds(8));
        Assert.Equal(e.CreatedOn - first.CreatedOn, e.Elapsed);
        Assert.Null(e.Exception);
        Assert.Equal(first, e.First);
        Assert.Equal(message, e.Message);
        Assert.Equal(first, e.Previous);
        Assert.Equal(severity, e.Severity);
        Assert.Equal(service.EventSourceName, e.SourceName);
    }

    [Fact]
    public void End_ValidInputWithOverrides_CreatesLastEvent()
    {
        const EventSeverity severity = EventSeverity.Info;
        (EventCategoryName category, _, _, INamedForProfiling service, ProfilingEventArgs first) =
                CreateFirstEvent(severity);

        const string messageOverride = "{foo} is {bar} 2";

        ProfilingEventArgs e = ProfilingEventArgs.End(
                first,
                exception: null,
                message: messageOverride,
                "bar",
                4);

        Assert.Equal(new object[] { "bar", 4 }, e.Arguments);
        Assert.Equal(category, e.CategoryName);
        Assert.Equal(EventContinuity.Last, e.Continuity);
        Assert.Equal(DateTime.UtcNow, e.CreatedOn, TimeSpan.FromSeconds(8));
        Assert.Equal(e.CreatedOn - first.CreatedOn, e.Elapsed);
        Assert.Null(e.Exception);
        Assert.Equal(first, e.First);
        Assert.Equal(messageOverride, e.Message);
        Assert.Equal(first, e.Previous);
        Assert.Equal(severity, e.Severity);
        Assert.Equal(service.EventSourceName, e.SourceName);
    }

    private static (EventCategoryName Category, Exception Exc, string Message, INamedForProfiling Service, ProfilingEventArgs E) CreateFirstEvent(
            EventSeverity severity)
    {
        EventCategoryName category = new("test_category");
        TestService service = new();
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
        ArgumentNullException exception = new("param", "exc");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
        const string message = "{foo} is {bar}";
        ProfilingEventArgs e = ProfilingEventArgs.StartFrom(
                service,
                severity: severity,
                exception: exception,
                category: category,
                message: message,
                "foo",
                2);

        return (category, exception, message, service, e);
    }

    private class TestService : INamedForProfiling
    {
        public EventSourceName EventSourceName => new("test");
    }
}
