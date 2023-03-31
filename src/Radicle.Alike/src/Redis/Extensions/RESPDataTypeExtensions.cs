namespace Radicle.Alike.Redis.Extensions;

using Radicle.Alike.Redis.Models;

/// <summary>
/// Extensions for <see cref="RESPDataType"/>.
/// </summary>
public static class RESPDataTypeExtensions
{
    /// <summary>
    /// Gets the version(s) this type can be used in.
    /// </summary>
    /// <param name="type">Type to probe.</param>
    /// <returns>Version flags this type is in.</returns>
    public static RESPVersions GetVersion(this RESPDataType type)
    {
        if ((int)type < 16)
        {
            return RESPVersions.RESP3 | RESPVersions.RESP2;
        }

        return RESPVersions.RESP3;
    }
}
