namespace Radicle.Common.Compression.Generic;

using System;
using System.Collections.Generic;

/// <summary>
/// Immutable dictionary implementation storing string items
/// as linear list and compressed, for memory efficiency with
/// small dictionary lengths (~256).
/// </summary>
/// <typeparam name="TKey">Type of the key.</typeparam>
internal class ImmutableThreeCZipDictionary<TKey> : ImmutableThreeZipDictionary<TKey, string>
        where TKey : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImmutableThreeCZipDictionary{TKey}"/> class.
    /// </summary>
    /// <param name="keyValue0">Key-value pair 0.</param>
    /// <param name="keyValue1">Key-value pair 1.</param>
    /// <param name="keyValue2">Key-value pair 2.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public ImmutableThreeCZipDictionary(
            KeyValuePair<TKey, string> keyValue0,
            KeyValuePair<TKey, string> keyValue1,
            KeyValuePair<TKey, string> keyValue2)
            : base(keyValue0, keyValue1, keyValue2)
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
