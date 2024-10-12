namespace Radicle.Common.Compression.Generic;

using System;
using System.Text;
using Radicle.Common.Check;

/// <summary>
/// Packing primitives for the zompressed value collections.
/// </summary>
internal static class StringPacking
{
    public static string DecodeValue(object rawValue)
    {
        byte[] bytes = (byte[])rawValue;

        if (Gzip.IsProbablyCompressedPayload(bytes))
        {
            try
            {
                return Gzip.UncompressString(bytes);
            }
            catch (ArgumentException)
            {
                return Encoding.UTF8.GetString(bytes);
            }
        }

        return Encoding.UTF8.GetString(bytes);
    }

    public static object EncodeValue(string value)
    {
        Ensure.Param(value).Done();

        if (value.Length < Gzip.TextStringCompressionThreshold)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        return Gzip.Compress(value);
    }
}
