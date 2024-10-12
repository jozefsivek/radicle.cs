namespace Radicle.Common.Compression;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

/// <summary>
/// Interface for the zip collections factory
/// for creating various memory efficient immutable collections.
/// </summary>
public interface IZipCollectionsFactory
{
    /// <summary>
    /// Create immutable dictionary from the given arbitrary key-value pairs.
    /// See <see cref="CreateCompressed{TKey}(IEnumerable{KeyValuePair{TKey, string}})"/>
    /// for gaining more memory savings for string values dictionaries.
    /// </summary>
    /// <param name="keyValues">Collection of key-value pairs.</param>
    /// <returns>Instance of <see cref="IImmutableDictionary{TKey, TValue}"/></returns>
    /// <typeparam name="TKey">Type of the key.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    IImmutableDictionary<TKey, TValue> Create<TKey, TValue>(
            IEnumerable<KeyValuePair<TKey, TValue>> keyValues)
        where TKey : notnull;

    /// <summary>
    /// Create immutable dictionary from the given key-string value pairs.
    /// Use string compression on values.
    /// </summary>
    /// <param name="keyValues">Collection of key-value pairs.</param>
    /// <returns>Instance of <see cref="IImmutableDictionary{TKey, TValue}"/></returns>
    /// <typeparam name="TKey">Type of the key.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    IImmutableDictionary<TKey, string> CreateCompressed<TKey>(
            IEnumerable<KeyValuePair<TKey, string>> keyValues)
        where TKey : notnull;

    /// <summary>
    /// Create immutable list from the given string values.
    /// Use string compression.
    /// </summary>
    /// <param name="values">Collection of string values.</param>
    /// <returns>Instance of <see cref="IImmutableList{T}"/></returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    IImmutableList<string> CreateCompressed(
            IEnumerable<string> values);
}
