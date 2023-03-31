namespace Radicle.Alike.Redis;

using System;

/// <summary>
/// Enumeration of RESP protocol versions.
/// See https://github.com/redis/redis-specifications/tree/master/protocol .
/// </summary>
[Flags]
public enum RESPVersions
{
    /// <summary>
    /// None version.
    /// </summary>
    None = 0,

    /// <summary>
    /// RESP2 version.
    /// </summary>
    RESP2 = 1 << 0,

    /// <summary>
    /// RESP2 version.
    /// </summary>
    RESP3 = 1 << 1,
}
