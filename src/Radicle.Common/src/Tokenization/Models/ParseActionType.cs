namespace Radicle.Common.Tokenization.Models;

/// <summary>
/// Enumeration of possible actions after yielding stop word.
/// </summary>
/// <remarks>Note there is no stop option here because the parser
/// returns enumeration which can be stopped externally.
/// This makes this a bit simplier.</remarks>
public enum ParseActionType
{
    /// <summary>
    /// After stop word there is no control, so just continue
    /// as ususal in parsing.
    /// </summary>
    Continue = 0,

    /// <summary>
    /// Read further and retrigger parse action.
    /// If input is exhausted incomplete control
    /// token is returned immidiatelly.
    /// </summary>
    ReadMoreControl = 1,

    /// <summary>
    /// Yield control token with contents
    /// of <see cref="ParsedReadState.ReadBuffer"/>
    /// read so far as part of control sequence read.
    /// </summary>
    YieldControl = 2,

    /// <summary>
    /// Yield free text token with contents
    /// of <see cref="ParsedReadState.ReadBuffer"/>
    /// read so far as part of control sequence read.
    /// </summary>
    YieldFreeText = 3,

    /// <summary>
    /// Continue acumulating control token until given condition
    /// yield that.
    /// </summary>
    ContinueWithControllUntil = 4,
}
