namespace Radicle.Common.Tokenization.Models;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Radicle.Common.Check;

/// <summary>
/// Read state of the parser which can be probed by custom action.
/// </summary>
public sealed class ParsedReadState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParsedReadState"/> class.
    /// </summary>
    /// <param name="readBuffer">Bufferof characters read after
    ///     <paramref name="triggeringStopWord"/>.</param>
    /// <param name="triggeringStopWord">Stop word which
    ///     triggerred this state.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public ParsedReadState(
            IEnumerable<char> readBuffer,
            ParsedTokenStopWord triggeringStopWord)
    {
        this.ReadBuffer = Ensure.Param(readBuffer).ToImmutableArray();
        this.TriggeringStopWord = Ensure.Param(triggeringStopWord).Value;
    }

    /// <summary>
    /// Gets buffer of read characters after <see cref="TriggeringStopWord"/>.
    /// May be empty if the source input was exhausted.
    /// This buffer size depends on the last <see cref="ParseAction"/>.
    /// </summary>
    public ImmutableArray<char> ReadBuffer { get; }

    /// <summary>
    /// Gets triggering stop word.
    /// </summary>
    public ParsedTokenStopWord TriggeringStopWord { get; }
}
