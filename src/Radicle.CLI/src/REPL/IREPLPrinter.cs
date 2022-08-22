namespace Radicle.CLI.REPL;

using System;
using Radicle.CLI.Models;

/// <summary>
/// REPL view models printer.
/// </summary>
internal interface IREPLPrinter
{
    /// <summary>
    /// Print given model.
    /// </summary>
    /// <param name="model">Last pronted model.</param>
    /// <param name="request">Request counter for spinner.</param>
    /// <param name="color">Optionaly enable colors.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameters are missing.</exception>
    void Print(
            ProgressViewModel model,
            ulong request = 0,
            bool color = false);

    /// <summary>
    /// Print given model.
    /// </summary>
    /// <param name="model">Last pronted model.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameters are missing.</exception>
    void Clear(
            ProgressViewModel model);

    /// <summary>
    /// Print given model.
    /// </summary>
    /// <param name="lastModel">Last pronted model.</param>
    /// <param name="currentModel">Model to print.</param>
    /// <param name="request">Request counter for spinner.</param>
    /// <param name="color">Optionaly enable colors.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameters are missing.</exception>
    void Print(
            ProgressViewModel lastModel,
            ProgressViewModel currentModel,
            ulong request = 0,
            bool color = false);

    /// <summary>
    /// Print given model.
    /// </summary>
    /// <param name="model">Model to print.</param>
    /// <param name="color">Optionaly enable colors.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameters are missing.</exception>
    void Print(PromptViewModel model, bool color = false);

    /// <summary>
    /// Print given model if the <paramref name="lastModel"/>
    /// was printed before.
    /// </summary>
    /// <param name="lastModel">Last printed model.</param>
    /// <param name="currentModel">Model to print.</param>
    /// <param name="color">Optionaly enable colors.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameters are missing.</exception>
    void Print(
            PromptViewModel lastModel,
            PromptViewModel currentModel,
            bool color = false);
}
