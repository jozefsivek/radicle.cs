namespace Radicle.Common.Compression;

/// <summary>
/// Enumeration of possible compression formats.
/// </summary>
public enum CompressionFormat
{
    /// <summary>
    /// None compression.
    /// </summary>
    None = 0,

    /// <summary>
    /// Gzip compression: https://en.wikipedia.org/wiki/Gzip .
    /// </summary>
    Gzip = 1,
}
