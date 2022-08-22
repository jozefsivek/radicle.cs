namespace Radicle.Common.Tokenization.Models;

/// <summary>
/// enumeration of all parsed token types.
/// </summary>
public enum ParsedTokenType
{
    /// <summary>
    /// Stop word.
    /// </summary>
    StopWord = 0,

    /// <summary>
    /// Control sequence.
    /// </summary>
    Control = 1,

    /// <summary>
    /// Free text.
    /// </summary>
    FreeText = 2,
}
