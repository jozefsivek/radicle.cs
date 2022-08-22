namespace Radicle.Sample.CLI;

using System;
using System.Threading;
using System.Threading.Tasks;
using Radicle.CLI.REPL;
using Radicle.Common.Check;

/// <summary>
/// Main entry point of REPL CLI sample project.
/// </summary>
public static class Program
{
    /// <summary>
    /// Main entry point.
    /// </summary>
    /// <param name="args">CLI arguments.</param>
    /// <returns>Awaitable task.</returns>
    public static async Task Main(string[] args)
    {
        Ensure.Param(args).AllNotNull().Done();

        // Allow fast quiting
        Console.TreatControlCAsInput = false;

        using CancellationTokenSource source = new();

        SampleREPLEvaluator evaluator = await SampleREPLEvaluator.CreateAsync()
                .ConfigureAwait(false);

        CancellationToken token = source.Token;

        Console.CancelKeyPress += (sender, cancelArgs) =>
        {
            Console.WriteLine();
            Console.WriteLine("SIGINT was received. Canceling now.");
            source.Cancel();
        };

        WriteHi();
        WriteVersionInfo();

        REPLEventLoop loop = new(evaluator.ReaderWriter, evaluator);

        try
        {
            await loop.RunAsync(token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            WriteGoodbye(true);

            // http://www.tldp.org/LDP/abs/html/exitcodes.html
            Environment.Exit(130);
        }

        WriteGoodbye();

        Environment.Exit(0);
    }

#pragma warning disable CA1303 // Do not pass literals as localized parameters
    private static void WriteHi()
    {
        Console.WriteLine("Welcome to Radicle sample CLI, type command or 'help'|'?'");
    }

    private static void WriteGoodbye(bool forced = false)
    {
        if (forced)
        {
            Console.WriteLine();
            Console.WriteLine("Forced exit, quitting");
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine("Good bye ...");
        }
    }

    private static void WriteVersionInfo()
    {
        Console.WriteLine($"`- {ThisAssembly.Git.Tag} ({ThisAssembly.Git.CommitDate} {ThisAssembly.Git.Commit} [+{ThisAssembly.Git.Commits}])");
    }

#pragma warning restore CA1303 // Do not pass literals as localized parameters
}
