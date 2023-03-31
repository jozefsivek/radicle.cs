namespace Radicle.Alike.Redis.Models;

using System;

/// <summary>
/// Collection of flags used in the <see cref="RESPDataType"/>.
/// </summary>
[Flags]
internal enum RESPDataTypeFlags
{
    /// <summary>
    /// None flag.
    /// </summary>
    None = 0,

    /// <summary>
    /// Error flag.
    /// </summary>
    ErrorLike = 1,

    /// <summary>
    /// Collection like structure flag.
    /// </summary>
    CollectionLike = 1 << 1,

    /// <summary>
    /// Structure with even length
    /// if other flag: <see cref="CollectionLike"/>, is also set.
    /// </summary>
    ConditionalEvenLength = 1 << 4,

    /// <summary>
    /// Map like structure flag,
    /// i.e. collection with even total number
    /// of key-value in-pair items.
    /// </summary>
    MapLike = CollectionLike | ConditionalEvenLength,

    /// <summary>
    /// Meta information flag.
    /// </summary>
    Meta = 1 << 7,
}
