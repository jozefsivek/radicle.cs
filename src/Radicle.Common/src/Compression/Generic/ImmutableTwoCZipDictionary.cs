namespace Radicle.Common.Compression.Generic;

using System;
using System.Collections.Generic;

/// <summary>
/// Immutable dictionary implementation storing string items
/// as linear list and compressed, for memory efficiency with
/// small dictionary lengths (~256).
/// </summary>
/// <typeparam name="TKey">Type of the key.</typeparam>
internal class ImmutableTwoCZipDictionary<TKey> : ImmutableTwoZipDictionary<TKey, string>
        where TKey : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImmutableTwoCZipDictionary{TKey}"/> class.
    /// </summary>
    /// <param name="keyValue0">Key-value pair 0.</param>
    /// <param name="keyValue1">Key-value pair 1.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public ImmutableTwoCZipDictionary(
            KeyValuePair<TKey, string> keyValue0,
            KeyValuePair<TKey, string> keyValue1)
            : base(keyValue0, keyValue1)
    {
    }

    protected override string DecodeValue(object rawValue)
    {
        return StringPacking.DecodeValue(rawValue);
    }

    protected override object EncodeValue(string value)
    {
        return StringPacking.EncodeValue(value);
    }
}
