namespace Radicle.Common.Tokenization.Models;

using System;
using Radicle.Common.Check;

/// <summary>
/// Action to perform after stop word was encountered.
/// </summary>
public sealed class ParseAction
{
    /// <summary>
    /// Action value for <see cref="ParseActionType.Continue"/>.
    /// I.e. after yielding stop word continue parsing as ususal.
    /// </summary>
    public static readonly ParseAction Continue = new(ParseActionType.Continue);

    /// <summary>
    /// Action value for <see cref="ParseActionType.ReadMoreControl"/>.
    /// I.e. after yielding stop word return with more control read
    /// in <see cref="ParsedReadState.ReadBuffer"/>.
    /// </summary>
    public static readonly ParseAction ReadMoreControl = new(ParseActionType.ReadMoreControl);

    /// <summary>
    /// Action value for <see cref="ParseActionType.Continue"/>.
    /// I.e. after yielding stop word yield control
    /// from contents of <see cref="ParsedReadState.ReadBuffer"/>.
    /// </summary>
    public static readonly ParseAction YieldControl = new(ParseActionType.YieldControl);

    /// <summary>
    /// Action value for <see cref="ParseActionType.Continue"/>.
    /// I.e. after yielding stop word yield free text
    /// from contents of <see cref="ParsedReadState.ReadBuffer"/>.
    /// </summary>
    public static readonly ParseAction YieldFreeText = new(ParseActionType.YieldFreeText);

    /// <summary>
    /// Initializes a new instance of the <see cref="ParseAction"/> class.
    /// </summary>
    /// <param name="type">Type of the action.</param>
    private ParseAction(ParseActionType type)
    {
        this.Type = type;
    }

    /// <summary>
    /// Gets type of this action.
    /// </summary>
    public ParseActionType Type { get; }

    /// <summary>
    /// Gets amount of characters to read before yielding.
    /// Defaults to zero if not applicable. Check <see cref="Type"/>
    /// if this parameter has a meaning for given type.
    /// </summary>
    public byte NumberOfCharactersToRead { get; private init; }

    /// <summary>
    /// Return <see cref="ParseAction"/> with
    /// <see cref="ParseActionType.ContinueWithControllUntil"/>
    /// with given amount of characters to accumulate in buffer
    /// until yielding control token.
    /// </summary>
    /// <param name="numberOfCharactersToRead">Number of
    ///     characters to accumulate before yielding
    ///     control token. If given amount of characters
    ///     is not available in the input, incomplete read control
    ///     token is returned.</param>
    /// <returns>Instance of <see cref="ParseAction"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     is <paramref name="numberOfCharactersToRead"/> is zero.</exception>
    public static ParseAction ContinueWithControllUntil(byte numberOfCharactersToRead)
    {
        return new ParseAction(ParseActionType.ContinueWithControllUntil)
        {
            NumberOfCharactersToRead = Ensure
                    .Param(numberOfCharactersToRead)
                    .StrictlyPositive()
                    .Value,
        };
    }
}
