namespace Radicle.Common.Extensions;

using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class TaskExtensionsTest
{
    [Fact]
    public void SafelyFireAndForgot_NullTask_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => TaskExtensions.SafelyFireAndForgot(null!));
    }

    [Fact]
    public void SafelyFireAndForgot_Task_IsCalled()
    {
        using Semaphore semaphore = new(0, 1);

        async Task SemaphoreTask()
        {
            await Task.Delay(1).ConfigureAwait(false);

            _ = semaphore.Release();
        }

        SemaphoreTask().SafelyFireAndForgot();

        Assert.True(semaphore.WaitOne(512));
    }

    [Fact]
    public void SafelyFireAndForgot_TgrowingTask_DoesNotThrow()
    {
        static async Task BadTask()
        {
            await Task.Delay(1).ConfigureAwait(false);

            throw new ArgumentNullException();
        }

        BadTask().SafelyFireAndForgot();
    }
}
