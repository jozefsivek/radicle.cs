namespace Radicle.Common.Compression;

/// <summary>
/// Levels of the compression the zip collection factory can provide.
/// </summary>
public enum ZipCollectionsLevel
{
    /// <summary>
    /// High (default) level with all optimization used.
    /// </summary>
    High = 0,

    /// <summary>
    /// Basic level with exclusion of micro-optimizations
    /// for very short collections. Use for testing
    /// or if working collections are long and micro–optimizations
    /// do not provide good tradeoff if this was determined by testing.
    /// </summary>
    Basic = 3,

    /// <summary>
    /// Trivial level will use detnet immutable
    /// primitives. Use for reference comparison or testing.
    /// </summary>
    Trivial = 4,
}
