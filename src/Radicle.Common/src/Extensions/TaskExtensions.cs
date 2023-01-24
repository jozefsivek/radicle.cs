namespace Radicle.Common.Extensions;

using System;
using System.Threading.Tasks;
using Radicle.Common.Check;

/// <summary>
/// Collection of extensions for <see cref="Task"/>.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Wrap task awaiting in catch all try block and allow
    /// it to be not awaited on the calling thread.
    /// </summary>
    /// <param name="task">Task to safely fire and forget.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static void SafelyFireAndForgot(this Task task)
    {
        Ensure.Param(task).Done();

        _ = SafelyAwaitAsync(task).ConfigureAwait(false);
    }

    private static async Task SafelyAwaitAsync(Task task)
    {
#pragma warning disable CA1031, RCS1075 // Avoid empty catch clause that catches System.Exception.
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (Exception)
        {
            // pass
        }
#pragma warning restore RCS1075 // Avoid empty catch clause that catches System.Exception.
#pragma warning restore CA1031
    }
}
