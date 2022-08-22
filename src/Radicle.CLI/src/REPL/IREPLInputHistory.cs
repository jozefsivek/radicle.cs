namespace Radicle.CLI.REPL;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// History component REPL reader/writer.
/// </summary>
public interface IREPLInputHistory : IEnumerable<REPLInputHistoryRecord>
{
    /// <summary>
    /// Push given <paramref name="input"/>
    /// into history. Note that this input may be ignored
    /// if this instance is configured so. The push also resets
    /// the current peek window.
    /// </summary>
    /// <param name="input">Input to push to history.</param>
    /// <returns><see langword="true"/> if <paramref name="input"/>
    ///     was pushed to history stack; <see langword="false"/>
    ///     otherwise (se details of implementation).</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public bool Push(string input);

    /*
    How peek window works, changes the history and works with push:

    history stack:

        a                      \   (oldest)
        b                       }- history stack (may be empty)
        c                      /   (newest)
        [<current live input empty placeholder>] -->  window

     move up will change stack to:

        a
        b
        [c]                    --> window
        <current live value>

     consecutive move down with current value "ca" will change stack to
     (note the value will not change from "c" to "ca" if instead
     of move down just push was made):

        a
        b
        ca
        [<current live value>] --> window

     consecutive move up by 2 will change stack to:

        a
        [b]                    --> window
        ca
        <current live value>

     consecutive push of "b" will change stack to
     (note the peek window is reset as well <current live value>):

        a
        b
        ca
        b
        [<current live input empty placeholder>] --> window
     */

    /// <summary>
    /// Moves peek window upwards returns the value.
    /// </summary>
    /// <param name="currentValue">Current value to push.</param>
    /// <param name="value">Value under the peek window.</param>
    /// <returns><see langword="true"/> if moved successfully;
    ///     <see langword="false"/> if there is no value up (in history).</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    bool TryMovePeekWindowUp(
            string currentValue,
            [NotNullWhen(returnValue: true)] out string? value);

    /// <summary>
    /// Moves downwards in the history and returns the value.
    /// </summary>
    /// <param name="currentValue">Current value to push.</param>
    /// <param name="value">Value under the peek window.</param>
    /// <returns><see langword="true"/> if moved successfully;
    ///     <see langword="false"/> if there is no value down.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    bool TryMovePeekWindowDown(
            string currentValue,
            [NotNullWhen(returnValue: true)] out string? value);
}
