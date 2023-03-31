namespace Radicle.Alike.Redis.Models;

/// <summary>
/// Types of the verbatim string.
/// </summary>
public enum VerbatimStringType
{
    /// <summary>
    /// Plain text.
    /// </summary>
    Text = 0,

    /// <summary>
    /// Markdown text.
    /// </summary>
    Markdown = 1,
}
