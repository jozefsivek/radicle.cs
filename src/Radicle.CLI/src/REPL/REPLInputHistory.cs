namespace Radicle.CLI.REPL;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Radicle.Common.Check;

/// <summary>
/// Default implementation of IREPLInputHistory input history stack.
/// </summary>
/// <remarks>This class is not thread safe.</remarks>
public sealed class REPLInputHistory : IREPLInputHistory
{
    /// <summary>
    /// Historical stack.
    /// </summary>
    private readonly Stack<REPLInputHistoryRecord> history = new();

    /// <summary>
    /// Future stack cleaned on any call to <see cref="Push(string)"/>
    /// used when using peek window, last value is always discarted on push.
    /// </summary>
    private readonly Stack<REPLInputHistoryRecord> future = new();

    /// <summary>
    /// Capacity overhead to avoid too frequent shrinks.
    /// </summary>
    private readonly ushort maxCapacityOverhead = 64;

    /// <summary>
    /// Initializes a new instance of the <see cref="REPLInputHistory"/> class.
    /// </summary>
    /// <param name="maxCapacity">Maximum capacity of the input history.</param>
    /// <param name="initialRecords">Initial records if any. First record
    ///     is oldest.</param>
    /// <param name="ignoreInputsWithPrefix">Prefix to use for ignoring
    ///     inputs when calling <see cref="Push(string)"/>.
    ///     Set to <see langword="null"/> to avoid ignoring,
    ///     defaults to shell commonly used space character.</param>
    /// <param name="ignoreRepeatedInputs">Flag determining whether to
    ///     ignore repeating commands when calling <see cref="Push(string)"/>.
    ///     Set to <see langword="true"/> for shell like behaviour.</param>
    /// <exception cref="ArgumentNullException">Thrown if
    ///     <paramref name="initialRecords"/> contains <see langword="null"/>
    ///     values.</exception>
    public REPLInputHistory(
            ushort maxCapacity = 2042,
            IEnumerable<string>? initialRecords = null,
            string? ignoreInputsWithPrefix = " ",
            bool ignoreRepeatedInputs = false)
    {
        this.MaxCapacity = maxCapacity;
        this.IgnoreInputsWithPrefix = ignoreInputsWithPrefix;
        this.IgnoreRepeatedInputs = ignoreRepeatedInputs;

        foreach (string recString in Ensure.Optional(initialRecords).AllNotNull())
        {
            this.history.Push(
                    new REPLInputHistoryRecord(
                        recString,
                        isInitRecord: true));
        }
    }

    /// <summary>
    /// Gets maximum capacity of the history stack exposed to user.
    /// </summary>
    public ushort MaxCapacity { get; }

    /// <summary>
    /// Gets prefix of inputs which will be ignored,
    /// or <see langword="null"/> if not applicable.
    /// </summary>
    public string? IgnoreInputsWithPrefix { get; }

    /// <summary>
    /// Gets a value indicating whether repeated
    /// inputs should be ignored.
    /// </summary>
    public bool IgnoreRepeatedInputs { get; }

    /// <inheritdoc/>
    public bool Push(string input)
    {
        Ensure.Param(input).Done();

        REPLInputHistoryRecord? lastF = null;

        while (this.future.TryPop(out REPLInputHistoryRecord f))
        {
            if (lastF.HasValue)
            {
                this.history.Push(lastF.Value);
            }

            // discard last record which is current input
            lastF = f;
        }

        if (this.TryPushToHistory(input))
        {
            this.Shrink();

            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public bool TryMovePeekWindowUp(
            string currentValue,
            [NotNullWhen(returnValue: true)] out string? value)
    {
        Ensure.Param(currentValue).Done();

        value = default;

        if (
                this.future.Count < this.MaxCapacity
                && this.history.TryPop(out REPLInputHistoryRecord v))
        {
            if (this.future.Count == 0)
            {
                this.future.Push(new REPLInputHistoryRecord(currentValue));
            }
            else if (this.future.TryPeek(out REPLInputHistoryRecord currentWidow)
                    && currentWidow.Value != currentValue)
            {
                _ = this.future.Pop();
                this.future.Push(new REPLInputHistoryRecord(
                        currentValue,
                        isInitRecord: currentWidow.IsInitRecord,
                        isModified: true));
            }

            this.future.Push(v);

            value = v.Value;

            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public bool TryMovePeekWindowDown(
            string currentValue,
            [NotNullWhen(returnValue: true)] out string? value)
    {
        Ensure.Param(currentValue).Done();

        value = default;

        if (this.future.Count <= 1)
        {
            return false;
        }
        else if (this.future.TryPop(out REPLInputHistoryRecord currentWidow))
        {
            if (currentWidow.Value != currentValue)
            {
                this.history.Push(
                        new REPLInputHistoryRecord(
                            currentValue,
                            isInitRecord: currentWidow.IsInitRecord,
                            isModified: true));
            }
            else
            {
                this.history.Push(currentWidow);
            }

            value = this.future.Peek().Value;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public IEnumerator<REPLInputHistoryRecord> GetEnumerator()
    {
        return this.history
                .Take(Math.Max(this.MaxCapacity - (this.future.Count - 1), 0))
                .Reverse()
                .Concat(this.future.Take(this.future.Count - 1))
                .GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    /// <summary>
    /// Push given <paramref name="input"/>
    /// to history in a clean way, ignoring empty, null,
    /// skipped or duplicit values.
    /// </summary>
    /// <param name="input">Input to push.</param>
    /// <returns><see langword="true"/> if pushed;
    ///     <see langword="false"/> otherwise.</returns>
    private bool TryPushToHistory(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }
        else if (this.IgnoreInputsWithPrefix is not null
                && input.StartsWith(this.IgnoreInputsWithPrefix, StringComparison.Ordinal))
        {
            return false;
        }
        else if (
                this.IgnoreRepeatedInputs
                && this.history.TryPeek(out REPLInputHistoryRecord last)
                && input.Equals(last.Value, StringComparison.Ordinal))
        {
            return false;
        }
        else
        {
            this.history.Push(new REPLInputHistoryRecord(input));

            return true;
        }
    }

    /// <summary>
    /// Shrink currrent history to maximum capacity if needed.
    /// </summary>
    private void Shrink()
    {
        if (this.history.Count > (this.MaxCapacity + this.maxCapacityOverhead))
        {
            Stack<REPLInputHistoryRecord> tmp = new(this.MaxCapacity);

            for (int i = 0; i < this.MaxCapacity; i++)
            {
                if (this.history.TryPop(out REPLInputHistoryRecord value))
                {
                    tmp.Push(value);
                }
            }

            this.history.Clear();

            while (tmp.TryPop(out REPLInputHistoryRecord value))
            {
                this.history.Push(value);
            }
        }
    }
}
