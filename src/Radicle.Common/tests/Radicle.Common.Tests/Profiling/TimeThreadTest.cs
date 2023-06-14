namespace Radicle.Common.Profiling;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

public class TimeThreadTest
{
    [Fact]
    public void Constructor_NullCollectio_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new TimeThread(null!));
    }

    [Fact]
    public void Measure_NullKey_Throws()
    {
        CountersDictionary collection = new();

        Assert.Throws<ArgumentNullException>(() =>
                new TimeThread(collection).Measure(null!));
    }

    [Fact]
    public void Measure_ValidKey_ModifiesCollection()
    {
        CountersDictionary collection = new();

        Assert.True(collection.IsEmpty);

        new TimeThread(collection).Measure("foo");

        Assert.False(collection.IsEmpty);
        Assert.True(collection.ContainsKey("foo"));
    }

    [Fact]
    public async Task Restart_RestartsCounter()
    {
        CountersDictionary collection = new();

        Stopwatch sw = Stopwatch.StartNew();
        TimeThread thread = new(collection);

        thread.Measure("foo");

        await Task.Delay(1).ConfigureAwait(false);

        thread.Restart();

        sw.Stop();
        thread.Measure("foo");

        Assert.True(sw.Elapsed > collection.GetElapsed("foo"));
    }
}
