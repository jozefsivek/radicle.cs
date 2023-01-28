namespace Radicle.Common.Profiling.Models;

using System;
using System.Linq;
using System.Threading.Tasks;
using Radicle.Common.Extensions;
using Xunit;

public class ProfilingSessionTest
{
    [Fact]
    public void Yield_EmptySession_YieldsEmpty()
    {
        Assert.Empty(new ProfilingSession().Yield());
    }

    [Fact]
    public async Task Yield_MultipleEvents_ReturnsAll()
    {
        ProfilingSession session = new();

        for (int i = 0; i < 4; i++)
        {
            await FeedSession(session, 4).ConfigureAwait(false);

            Assert.Equal(4, session.Yield().Count());
        }
    }

    [Fact]
    public void Yield_AsyncEvents_ReturnsAll()
    {
        ProfilingSession session = new();

        const ushort max = 68;
        int i = 0;
        ushort passes = 0;
        FeedSession(session, max, wait: true).SafelyFireAndForgot();

        while (i < max)
        {
            passes++;

            foreach (IProfiledEvent e in session.Yield())
            {
                Assert.Equal($"{i++}", e.Message);
            }
        }

        Assert.True(passes > 1);
    }

    [Fact]
    public void Add_NullEvent_Throws()
    {
        ProfilingSession session = new();

        Assert.Throws<ArgumentNullException>(() => session.Add(null!));
    }

    private static async Task FeedSession(
            ProfilingSession session,
            ushort count,
            bool wait = false)
    {
        for (int i = 0; i < count; i++)
        {
#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                session.Add(new ProfiledEventArgs(
                        "test",
                        "test_category")
                {
                    Message = $"{i}",
                });
            }
            catch (Exception)
            {
                break;
            }
#pragma warning restore CA1031 // Do not catch general exception types

            if (wait)
            {
                await Task.Delay(1).ConfigureAwait(false);
            }
        }
    }
}
