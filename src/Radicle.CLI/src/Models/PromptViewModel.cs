namespace Radicle.CLI.Models;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Radicle.Common.Check;

/// <summary>
/// View model of the user prompt.
/// </summary>
internal sealed class PromptViewModel
{
    /*
     Composition of the prompt:

     ps1_value > very long ][ user input [with auto suggestion tail]
     \_________/\_________/|'\__________/\_________________________/
          |         |      |      |                 `- interactive suggestion
          |         |      |      |                    (displayed with different style then input)
          |         |      |      `-right side of user written input
          |         |      `- cursor (of zero width in model) placed in user input
          |         `- left side of user written input
          `- prompt string one (fixed before user input)
     */

    /// <summary>
    /// Initializes a new instance of the <see cref="PromptViewModel"/> class.
    /// </summary>
    /// <param name="promptStringOne">Prompt string one.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public PromptViewModel(string promptStringOne)
    {
        this.PromptStringOne = Ensure.Param(promptStringOne).ToImmutableArray();
        this.InputHead = ImmutableStack<char>.Empty;
        this.InputTail = ImmutableStack<char>.Empty;
        this.SuggestionTail = ImmutableArray<char>.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PromptViewModel"/> class.
    /// </summary>
    /// <param name="promptStringOne">Prompt string one.</param>
    /// <param name="inputHead">Input head.</param>
    /// <param name="inputTail">Input tail.</param>
    /// <param name="suggestionTail">Suggestion tail.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    private PromptViewModel(
            ImmutableArray<char> promptStringOne,
            ImmutableStack<char> inputHead,
            ImmutableStack<char> inputTail,
            ImmutableArray<char> suggestionTail)
    {
        this.PromptStringOne = promptStringOne;
        this.InputHead = inputHead;
        this.InputTail = inputTail;
        this.SuggestionTail = suggestionTail;
    }

    /// <summary>
    /// Gets prompt string one characters.
    /// </summary>
    public ImmutableArray<char> PromptStringOne { get; }

    /// <summary>
    /// Gets inout head characters.
    /// </summary>
    public ImmutableStack<char> InputHead { get; }

    /// <summary>
    /// Gets inout tail characters.
    /// </summary>
    public ImmutableStack<char> InputTail { get; }

    /// <summary>
    /// Gets suggection tail characters.
    /// </summary>
    public ImmutableArray<char> SuggestionTail { get; }

    /// <summary>
    /// Gets length till head.
    /// </summary>
    public int TillHeadLength => this.PromptStringOne.Length + this.InputHead.Count();

    /// <summary>
    /// Gets length till tail.
    /// </summary>
    public int TillTailLength => this.TillHeadLength + this.InputTail.Count();

    /// <summary>
    /// Gets total length.
    /// </summary>
    public int TotalLength => this.TillTailLength + this.SuggestionTail.Length;

    /// <summary>
    /// Gets current input value.
    /// </summary>
    public string UserInputValue
    {
        get
        {
            StringBuilder sb = new();

            foreach (char c in this.InputHead.Reverse())
            {
                _ = sb.Append(c);
            }

            foreach (char c in this.InputTail)
            {
                _ = sb.Append(c);
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// Write given <paramref name="character"/>
    /// at current cursor position to user input.
    /// </summary>
    /// <param name="character">Character to write.</param>
    /// <returns>New instance of <see cref="PromptViewModel"/>.</returns>
    public PromptViewModel Write(char character)
    {
        return new PromptViewModel(
                this.PromptStringOne,
                this.InputHead.Push(character),
                this.InputTail,
                this.SuggestionTail);
    }

    /// <summary>
    /// Write given <paramref name="characters"/>
    /// at current cursor position to user input.
    /// </summary>
    /// <param name="characters">Characters to write.</param>
    /// <returns>New model.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public PromptViewModel Write(string characters)
    {
        Ensure.Param(characters).Done();

        if (characters.Length == 0)
        {
            return this;
        }

        ImmutableStack<char> head = this.InputHead;

        foreach (char c in characters)
        {
            head = head.Push(c);
        }

        return new PromptViewModel(
                this.PromptStringOne,
                head,
                this.InputTail,
                this.SuggestionTail);
    }

    /// <summary>
    /// Remove character on left from the cursor.
    /// </summary>
    /// <returns>New model.</returns>
    public PromptViewModel BackDelete()
    {
        if (this.InputHead.IsEmpty)
        {
            return this;
        }

        return new PromptViewModel(
                this.PromptStringOne,
                this.InputHead.Pop(),
                this.InputTail,
                this.SuggestionTail);
    }

    /// <summary>
    /// Remove character on right from the cursor.
    /// </summary>
    /// <returns>New model.</returns>
    public PromptViewModel ForwardDelete()
    {
        if (this.InputTail.IsEmpty)
        {
            return this;
        }

        return new PromptViewModel(
                this.PromptStringOne,
                this.InputHead,
                this.InputTail.Pop(),
                this.SuggestionTail);
    }

    /// <summary>
    /// Move cursor to left if possible.
    /// </summary>
    /// <returns>New instance of <see cref="PromptViewModel"/>
    ///     or existing if cursor reached edge of input.</returns>
    public PromptViewModel MoveCursorLeft()
    {
        if (this.InputHead.IsEmpty)
        {
            return this;
        }

        return new PromptViewModel(
                this.PromptStringOne,
                this.InputHead.Pop(out char c),
                this.InputTail.Push(c),
                this.SuggestionTail);
    }

    /// <summary>
    /// Move cursor to right if possible.
    /// </summary>
    /// <returns>New instance of <see cref="PromptViewModel"/>
    ///     or existing if cursor reached edge of input.</returns>
    public PromptViewModel MoveCursorRight()
    {
        if (this.InputTail.IsEmpty)
        {
            return this;
        }

        ImmutableStack<char> newTail = this.InputTail.Pop(out char c);

        return new PromptViewModel(
                this.PromptStringOne,
                this.InputHead.Push(c),
                newTail,
                this.SuggestionTail);
    }

    /// <summary>
    /// Set suggestion tail.
    /// </summary>
    /// <param name="suggestionTail">Suggestion tail.</param>
    /// <returns>New instance of <see cref="PromptViewModel"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public PromptViewModel WithSuggestionTail(string suggestionTail)
    {
        Ensure.Param(suggestionTail).Done();

        if (suggestionTail.Length == 0 && this.SuggestionTail.Length == 0)
        {
            return this;
        }

        return new PromptViewModel(
                this.PromptStringOne,
                this.InputHead,
                this.InputTail,
                suggestionTail.ToImmutableArray());
    }

    /// <summary>
    /// Get expected position of cursor after the prefix and head.
    /// </summary>
    /// <param name="maxWidthConstrain">Constrains on the width,
    ///     text will be wrapped if longer than constrain.</param>
    /// <returns>Coordinated and total amount of printable lines (at least 1).</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="maxWidthConstrain"/> non positive number.</exception>
    public (int Left, int Top, int Lines) GetCursorPosition(int maxWidthConstrain)
    {
        return GetPosition(this.TillHeadLength, maxWidthConstrain);
    }

    /// <summary>
    /// Get expected position of cursor after the total print out.
    /// </summary>
    /// <param name="maxWidthConstrain">Constrains on the width,
    ///     text will be wrapped if longer than constrain.</param>
    /// <returns>Coordinated and total amount of printable lines (at least 1).</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="maxWidthConstrain"/> non positive number.</exception>
    public (int Left, int Top, int Lines) GetPrintOutPosition(int maxWidthConstrain)
    {
        return GetPosition(this.TotalLength, maxWidthConstrain);
    }

    private static (int Left, int Top, int Lines) GetPosition(int length, int widthConstrain)
    {
        Ensure.Param(length).NonNegative().Done();
        Ensure.Param(widthConstrain).StrictlyPositive().Done();

        int lines = (length / widthConstrain)
                + ((length % widthConstrain) == 0 ? 0 : 1);

        if (lines == 0)
        {
            lines = 1;
        }

        int left = length % widthConstrain;
        int top = lines - 1;

        if (length == 0)
        {
            // special case of zero
            top = 0;
        }
        else if (left == 0)
        {
            // when text fits perfectly to constrain
            // we need to wrap around
            top = lines;
        }

        return (left, top, lines);
    }
}
