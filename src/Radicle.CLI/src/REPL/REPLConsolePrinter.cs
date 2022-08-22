namespace Radicle.CLI.REPL;

using System;
using System.Linq;
using Radicle.CLI.Models;
using Radicle.Common.Check;

/// <summary>
/// System Console implementation of <see cref="IREPLPrinter"/>.
/// </summary>
internal class REPLConsolePrinter : IREPLPrinter
{
    /// <inheritdoc/>
    public void Print(
            ProgressViewModel model,
            ulong request = 0,
            bool color = false)
    {
        int bufferWidth = Console.BufferWidth;
        (int currentLeft, int currentTop) = Console.GetCursorPosition();

        if (currentLeft != 0)
        {
            currentLeft = 0;
        }

        Console.SetCursorPosition(currentLeft, currentTop);

        string progressString = Ensure.Param(model).Value.ToString(request);

        if (progressString.Length >= bufferWidth)
        {
            progressString = progressString[..(bufferWidth - 1)];
        }

        Console.Write(progressString);

        Console.SetCursorPosition(currentLeft, currentTop);
    }

    /// <inheritdoc/>
    public void Clear(
            ProgressViewModel model)
    {
        Ensure.Param(model).Done();

        int bufferWidth = Console.BufferWidth;
        (int currentLeft, int currentTop) = Console.GetCursorPosition();

        if (currentLeft != 0)
        {
            currentLeft = 0;
        }

        Console.SetCursorPosition(currentLeft, currentTop);

        Console.Write(new string(' ', bufferWidth - 1));

        Console.SetCursorPosition(currentLeft, currentTop);
    }

    /// <inheritdoc/>
    public void Print(
            ProgressViewModel lastModel,
            ProgressViewModel currentModel,
            ulong request = 0,
            bool color = false)
    {
        Ensure.Param(lastModel).Done();
        Ensure.Param(currentModel).Done();

        if (ReferenceEquals(lastModel, currentModel))
        {
            return; // identity, nothing to do
        }

        this.Clear(lastModel);
        this.Print(currentModel, request, color: color);
    }

    /// <inheritdoc/>
    public void Print(
            PromptViewModel model,
            bool color = false)
    {
        Ensure.Param(model).Done();

        int bufferWidth = Console.BufferWidth;
        int bufferHeight = Console.BufferHeight;

        (int currentLeft, int currentTop) = Console.GetCursorPosition();

        if (currentLeft != 0)
        {
            currentLeft = 0;
        }

        Console.SetCursorPosition(currentLeft, currentTop);

        if (color)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
        }

        Console.Write(model.PromptStringOne.ToArray());

        if (color)
        {
            Console.ResetColor();
        }

        Console.Write(model.UserInputValue);

        if (color)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        Console.Write(model.SuggestionTail.ToArray());

        if (color)
        {
            Console.ResetColor();
        }

        (int left, int top, _) = model.GetCursorPosition(bufferWidth);

        int originLeft = currentLeft + left;
        int originTop = currentTop + top;

        if (originLeft < 0)
        {
            originLeft = 0;
        }
        else if (originLeft > bufferWidth)
        {
            originLeft = bufferWidth;
        }

        if (originTop < 0)
        {
            originTop = 0;
        }
        else if (originTop > bufferHeight)
        {
            originTop = bufferHeight;
        }

        Console.SetCursorPosition(originLeft, originTop);
    }

    /// <inheritdoc/>
    public void Print(
            PromptViewModel lastModel,
            PromptViewModel currentModel,
            bool color = false)
    {
        Ensure.Param(lastModel).Done();
        Ensure.Param(currentModel).Done();

        if (ReferenceEquals(lastModel, currentModel))
        {
            return; // identity
        }

        Clear(lastModel);

        this.Print(currentModel, color: color);
    }

    private static void Clear(PromptViewModel model)
    {
        int bufferWidth = Console.BufferWidth;
        int bufferHeight = Console.BufferHeight;
        int printOutLength = model.TotalLength;
        (int currentLeft, int currentTop) = Console.GetCursorPosition();

        (int left, int top, _) = model.GetCursorPosition(bufferWidth);
        (_, _, int totalLines) = model.GetPrintOutPosition(bufferWidth);
        int originLeft = currentLeft - left;
        int originTop = currentTop - top;

        if (originLeft < 0)
        {
            originLeft = 0;
        }
        else if (originLeft > bufferWidth)
        {
            originLeft = bufferWidth;
        }

        if (originTop < 0)
        {
            originTop = 0;
        }
        else if (originTop > bufferHeight)
        {
            originTop = bufferHeight;
        }

        // start from the beggining of the block
        Console.SetCursorPosition(originLeft, originTop);

        for (int i = 0; i < totalLines; i++)
        {
            if ((i + 1) == totalLines)
            {
                if (printOutLength != 0)
                {
                    if (printOutLength % bufferWidth == 0)
                    {
                        Console.Write(new string(' ', bufferWidth));
                    }
                    else
                    {
                        Console.Write(new string(' ', printOutLength % bufferWidth));
                    }
                }
            }
            else
            {
                Console.Write(new string(' ', bufferWidth));
            }
        }

        // reset to original position
        Console.SetCursorPosition(originLeft, originTop);
    }
}
