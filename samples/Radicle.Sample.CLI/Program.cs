namespace Radicle.Sample.CLI;

using System;
using System.Threading.Tasks;
using Radicle.Common.Check;

/// <summary>
/// Main entry point of Radicle CLI sample project.
/// </summary>
public static class Program
{
    /// <summary>
    /// Main entry point of app.
    /// </summary>
    /// <param name="args">CLI arguments.</param>
    /// <returns>Awaitable task.</returns>
    public static async Task Main(string[] args)
    {
        Ensure.Param(args).AllNotNull().Done();

#pragma warning disable CA1303 // Do not pass literals as localized parameters
        Console.WriteLine("OK");
#pragma warning restore CA1303 // Do not pass literals as localized parameters

        await Task.Delay(1).ConfigureAwait(false);
    }
}
