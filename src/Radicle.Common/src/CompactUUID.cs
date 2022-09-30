namespace Radicle.Common;

using System;
using System.Collections.Generic;
using Radicle.Common.Check;

/// <summary>
/// Utility for generating compact UUID.
/// </summary>
/// <remarks>For a better rcontext read
/// https://blog.stephencleary.com/2010/11/few-words-on-guids.html ,
/// https://docs.microsoft.com/en-us/dotnet/api/system.guid.newguid?view=net-5.0
/// and https://www.uuidtools.com/decode.</remarks>
public static class CompactUUID
{
    /// <summary>
    /// Generate raw big-endian UUID with 16 bytes.
    /// This UUID does not reveal private information
    /// about the host, it was generated on.
    /// </summary>
    /// <remarks>See
    /// https://en.wikipedia.org/wiki/Universally_unique_identifier
    /// version 4 variant 1 RFC 4122/DCE 1.1 or DCE 1.1, ISO/IEC 11578:1996
    /// (big-endian).</remarks>
    /// <returns>Big-endian UUID with 16 bytes.</returns>
    public static byte[] GenerateRawByteArray()
    {
        // Check the following source code for example of use
        // https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/Guid.cs
        if (BitConverter.IsLittleEndian)
        {
            byte[] result = Guid.NewGuid().ToByteArray();

            ShuffleFromToMSMixedEndianWith(result);

            return result;
        }

        return Guid.NewGuid().ToByteArray();
    }

    /// <summary>
    /// Generate compact ASCII Base64-based UUID
    /// sourced from <see cref="GenerateRawByteArray"/> with
    /// 22 mixed characters, with padding ("==") omitted.
    /// </summary>
    /// <returns>Base64-like UUID identifier.</returns>
    public static string Generate()
    {
        return Convert.ToBase64String(GenerateRawByteArray())[..22];
    }

    /// <summary>
    /// Generate URL-friendly compact UUID.
    /// See <see cref="Generate"/>.
    /// </summary>
    /// <returns>URL safe UUID identifier.</returns>
    public static string GenerateWebSafe()
    {
        // replace tricky base 64 characters
        return Generate()
                .Replace('/', '_')
                .Replace('+', '-');
    }

    /// <summary>
    /// Convert <paramref name="rawUuid"/> from or to Microsoft mixed-endian GUI variant 2
    /// encoding into or from big-endian encoding (variant 1).
    /// </summary>
    /// <param name="rawUuid">Collection of raw UUID bytes.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required argument is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if the <paramref name="rawUuid"/> has not 16 bytes.</exception>
    public static void ShuffleFromToMSMixedEndianWith(IList<byte> rawUuid)
    {
        Ensure.Collection(rawUuid).InRange(16, 16).Done();

        // big-endian encoded GUID <--> MS mixed-endian GUID
        // https://en.wikipedia.org/wiki/Universally_unique_identifier#Encoding
        // and https://en.wikipedia.org/wiki/Universally_unique_identifier#Variants

        // First 3 numbers have little-endian encoding in MS format
        (rawUuid[0], rawUuid[1], rawUuid[2], rawUuid[3]) = (rawUuid[3], rawUuid[2], rawUuid[1], rawUuid[0]);
        (rawUuid[4], rawUuid[5]) = (rawUuid[5], rawUuid[4]);
        (rawUuid[6], rawUuid[7]) = (rawUuid[7], rawUuid[6]);

        /* bytes 8 to 15 are unchanged */
    }
}
