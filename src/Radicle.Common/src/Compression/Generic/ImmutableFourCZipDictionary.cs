namespace Radicle.Common.Compression.Generic;

using System;
using System.Collections.Generic;

/// <summary>
/// Immutable dictionary implementation storing string items
/// as linear list and compressed, for memory efficiency with
/// small dictionary lengths (~256).
/// </summary>
/// <typeparam name="TKey">Type of the key.</typeparam>
internal class ImmutableFourCZipDictionary<TKey> : ImmutableFourZipDictionary<TKey, string>
        where TKey : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImmutableFourCZipDictionary{TKey}"/> class.
    /// </summary>
    /// <param name="keyValue0">Key-value pair 0.</param>
    /// <param name="keyValue1">Key-value pair 1.</param>
    /// <param name="keyValue2">Key-value pair 2.</param>
    /// <param name="keyValue3">Key-value pair 3.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public ImmutableFourCZipDictionary(
            KeyValuePair<TKey, string> keyValue0,
            KeyValuePair<TKey, string> keyValue1,
            KeyValuePair<TKey, string> keyValue2,
            KeyValuePair<TKey, string> keyValue3)
            : base(keyValue0, keyValue1, keyValue2, keyValue3)
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
