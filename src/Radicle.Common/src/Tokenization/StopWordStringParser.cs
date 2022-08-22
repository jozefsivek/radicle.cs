namespace Radicle.Common.Tokenization;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Radicle.Common.Check;
using Radicle.Common.Tokenization.Models;

/// <summary>
/// Parser for the string containing stop characters or words.
/// </summary>
public sealed class StopWordStringParser
{
    private readonly ImmutableHashSet<ParsedTokenStopWord> stopWords;

    private readonly int shortestStopWord;

    private readonly Func<ParsedReadState, ParseAction>? controlTokenAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="StopWordStringParser"/> class.
    /// </summary>
    /// <param name="stopWords">Optional stop chars.</param>
    /// <param name="controlTokenAction">Optional action
    ///     resolver for control tokens after stop words.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if <paramref name="stopWords"/> contains <see langword="null"/>
    ///     values.</exception>
    public StopWordStringParser(
            ISet<ParsedTokenStopWord>? stopWords = null,
            Func<ParsedReadState, ParseAction>? controlTokenAction = null)
    {
        this.stopWords = Ensure.OptionalCollection(stopWords)
                .AllNotNull()
                .ToImmutableHashSet();
        this.shortestStopWord = this.stopWords.Count > 0
                ? this.stopWords.Min(w => w.Length)
                : 0;
        this.controlTokenAction = controlTokenAction;
    }

    /// <summary>
    /// Parse given <paramref name="input"/>
    /// according to this parser rules to tokens.
    /// </summary>
    /// <param name="input">Input to parse.</param>
    /// <param name="startAt">Start position of the parsing.
    ///     if equal to length of <paramref name="input"/>
    ///     empty enumeration is returned.</param>
    /// <returns>Collection of parsed tokens.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="startAt"/> is out of bounds of
    ///     <paramref name="input"/> [0, length_of_string].</exception>
    /// <exception cref="NotSupportedException">Thrown in case of bug.</exception>
    public IEnumerable<ParsedToken> Parse(
            string input,
            int startAt = 0)
    {
        Ensure.Param(input).Done();
        Ensure.Param(startAt).InRange(0, input.Length).Done();

        /* TODO: use spans */

        List<char> buffer = new();
        bool inControlSequence = false;
        byte numberOfCharactersToRead = 0;
        ParsedTokenStopWord? stopWord = null;

        for (int position = startAt; position < input.Length; position++)
        {
            char current = input[position];
            buffer.Add(current);

            // read only control token which does not contain stop words
            if (inControlSequence)
            {
                // ContinueWithControllUntil
                if (numberOfCharactersToRead > 0)
                {
                    if (buffer.Count >= numberOfCharactersToRead)
                    {
                        yield return new ParsedTokenControl(new string(buffer.ToArray()));
                        inControlSequence = false;
                        stopWord = null;
                        numberOfCharactersToRead = 0;
                        buffer.Clear();
                    }

                    /* else continue */
                }
                else
                {
                    // ReadMoreControl
                    ParseAction action = this.controlTokenAction!(new ParsedReadState(buffer, stopWord!));

                    switch (action.Type)
                    {
                        case ParseActionType.Continue:
                            break;
                        case ParseActionType.ReadMoreControl:
                            continue;
                        case ParseActionType.YieldControl:
                            yield return new ParsedTokenControl(new string(buffer.ToArray()));
                            buffer.Clear();
                            break;
                        case ParseActionType.YieldFreeText:
                            yield return new ParsedTokenFreeText(new string(buffer.ToArray()));
                            buffer.Clear();
                            break;
                        case ParseActionType.ContinueWithControllUntil:
                            numberOfCharactersToRead = action.NumberOfCharactersToRead;

                            if (buffer.Count == numberOfCharactersToRead)
                            {
                                yield return new ParsedTokenControl(new string(buffer.ToArray()));
                                numberOfCharactersToRead = 0;
                                buffer.Clear();
                            }
                            else if (buffer.Count > numberOfCharactersToRead)
                            {
                                throw new NotSupportedException(
                                        $"Control token action function requested to read {numberOfCharactersToRead} "
                                        + $"characters of control token while buffer has already {buffer.Count} characters. "
                                        + "This is bug in the control token action function!");
                            }

                            continue;
                        default:
                            throw new NotSupportedException(
                                    $"BUG: Parse action type {action.Type} is not implemented.");
                    }

                    // all except ReadMoreControl and ContinueWithControllUntil
                    inControlSequence = false;
                    stopWord = null;

                    // buffer is not cleared in case of ContinueWithFreeText
                }
            }
            else if (this.TryGetStopWord(buffer, out stopWord))
            {
                int headLength = buffer.Count - stopWord.Length;

                // yield part of the buffer which is not stop word
                if (headLength > 0)
                {
                    string head = new(buffer.Take(headLength).ToArray());
                    yield return new ParsedTokenFreeText(head);
                }

                // clear buffer as all was used
                buffer.Clear();

                yield return stopWord;

                if (this.controlTokenAction is not null)
                {
                    ParseAction action = this.controlTokenAction(new ParsedReadState(buffer, stopWord));

                    switch (action.Type)
                    {
                        case ParseActionType.Continue:
                            inControlSequence = false;
                            stopWord = null;
                            break;
                        case ParseActionType.ReadMoreControl:
                            inControlSequence = true;
                            break;
                        case ParseActionType.YieldControl:
                            yield return new ParsedTokenControl(string.Empty);
                            break;
                        case ParseActionType.YieldFreeText:
                            yield return new ParsedTokenFreeText(string.Empty);
                            break;
                        case ParseActionType.ContinueWithControllUntil:
                            inControlSequence = true;
                            numberOfCharactersToRead = action.NumberOfCharactersToRead;
                            break;
                        default:
                            throw new NotSupportedException(
                                    $"BUG: Parse action type {action.Type} is not implemented.");
                    }
                }
            }
        }

        // flush remaining buffer
        if (buffer.Count > 0)
        {
            string tokenValue = new(buffer.ToArray());
            buffer.Clear();

            if (inControlSequence)
            {
                yield return new ParsedTokenControl(
                        tokenValue,
                        incompleteRead: true);
            }
            else
            {
                yield return new ParsedTokenFreeText(tokenValue);
            }
        }
        else if (inControlSequence)
        {
            yield return new ParsedTokenControl(
                    string.Empty,
                    incompleteRead: true);
        }
    }

    /// <summary>
    /// Tries to extract this instance defined stop word
    /// from the end of <paramref name="buffer"/>.
    /// </summary>
    /// <param name="buffer">Buffer to use.</param>
    /// <param name="stopWord">Extracted stop word.</param>
    /// <returns><see langword="true"/> if stop word was present
    ///     at the end of <paramref name="buffer"/>;
    ///     <see langword="false"/> otherwise.</returns>
    private bool TryGetStopWord(
            IReadOnlyList<char> buffer,
            [NotNullWhen(returnValue: true)] out ParsedTokenStopWord? stopWord)
    {
        stopWord = default;

        // return fast if not enough data
        if (buffer.Count < this.shortestStopWord || buffer.Count == 0)
        {
            return false;
        }

        string lastSample = string.Empty;

        foreach (ParsedTokenStopWord word in this.stopWords)
        {
            int take = word.Length;

            if (buffer.Count < take)
            {
                continue;
            }

            string sample = lastSample.Length == take
                    ? lastSample
                    : new(buffer.Skip(buffer.Count - take).ToArray());

            if (word.Value == sample)
            {
                stopWord = word;

                return true;
            }

            lastSample = sample;
        }

        return false;
    }
}
