namespace Radicle.Common.Compression;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Radicle.Common.Check;
using Radicle.Common.Compression.Generic;
using Radicle.Common.Extensions;

/// <summary>
/// Factory class for creating various memory efficient immutable collections.
/// </summary>
public sealed class ZipCollectionsFactory : IZipCollectionsFactory
{
    /// <summary>
    /// Gets default factory with highest compression level.
    /// </summary>
    public static readonly IZipCollectionsFactory Default = new ZipCollectionsFactory();

    /// <summary>
    /// Initializes a new instance of the <see cref="ZipCollectionsFactory"/> class.
    /// Unless you test something, just use <see cref="Default"/> instance.
    /// </summary>
    /// <param name="level">Level of processing to use.</param>
    public ZipCollectionsFactory(
            ZipCollectionsLevel level = ZipCollectionsLevel.High)
    {
        this.Level = level;
    }

    /// <summary>
    /// Gets level this factory was created with.
    /// </summary>
    public ZipCollectionsLevel Level { get; }

    /// <inheritdoc />
    public IImmutableDictionary<TKey, TValue> Create<TKey, TValue>(
            IEnumerable<KeyValuePair<TKey, TValue>> keyValues)
        where TKey : notnull
    {
        Ensure.Param(keyValues).Done();

        switch (this.Level)
        {
            case ZipCollectionsLevel.High:
                int l = keyValues.NativeLength();

                if (l == 0)
                {
                    return ImmutableZipDictionary<TKey, TValue>.Empty;
                }
                else if (l <= 4)
                {
                    KeyValuePair<TKey, TValue> first = default;
                    KeyValuePair<TKey, TValue> second = default;
                    KeyValuePair<TKey, TValue> third = default;
                    KeyValuePair<TKey, TValue> fourth = default;
                    int index = 0;

                    foreach (KeyValuePair<TKey, TValue> kv in keyValues)
                    {
                        switch (index)
                        {
                            case 0: first = kv; break;
                            case 1: second = kv; break;
                            case 2: third = kv; break;
                            case 3: fourth = kv; break;
                            default: throw new NotSupportedException($"BUG: count overflow {index}");
                        }

                        index++;
                    }

                    return l switch
                    {
                        1 => new ImmutableOneZipDictionary<TKey, TValue>(first),
                        2 => new ImmutableTwoZipDictionary<TKey, TValue>(first, second),
                        3 => new ImmutableThreeZipDictionary<TKey, TValue>(first, second, third),
                        4 => new ImmutableFourZipDictionary<TKey, TValue>(first, second, third, fourth),
                        _ => throw new NotSupportedException($"BUG: length overflow {index}"),
                    };
                }
                else
                {
                    return new ImmutableZipDictionary<TKey, TValue>(keyValues);
                }

            case ZipCollectionsLevel.Basic:
                return new ImmutableZipDictionary<TKey, TValue>(keyValues);
            case ZipCollectionsLevel.Trivial:
                return keyValues.ToImmutableDictionary();
            default:
                throw new NotSupportedException($"BUG: Unknown level {this.Level}");
        }
    }

    /// <inheritdoc />
    public IImmutableDictionary<TKey, string> CreateCompressed<TKey>(
            IEnumerable<KeyValuePair<TKey, string>> keyValues)
        where TKey : notnull
    {
        Ensure.Param(keyValues).Done();

        switch (this.Level)
        {
            case ZipCollectionsLevel.High:
                int l = keyValues.NativeLength();

                if (l == 0)
                {
                    return ImmutableZipDictionary<TKey, string>.Empty;
                }
                else if (l <= 4)
                {
                    KeyValuePair<TKey, string> first = default;
                    KeyValuePair<TKey, string> second = default;
                    KeyValuePair<TKey, string> third = default;
                    KeyValuePair<TKey, string> fourth = default;
                    int index = 0;

                    foreach (KeyValuePair<TKey, string> kv in keyValues)
                    {
                        switch (index)
                        {
                            case 0: first = kv; break;
                            case 1: second = kv; break;
                            case 2: third = kv; break;
                            case 3: fourth = kv; break;
                            default: throw new NotSupportedException($"BUG: count overflow {index}");
                        }

                        index++;
                    }

                    return l switch
                    {
                        1 => new ImmutableOneCZipDictionary<TKey>(first),
                        2 => new ImmutableTwoCZipDictionary<TKey>(first, second),
                        3 => new ImmutableThreeCZipDictionary<TKey>(first, second, third),
                        4 => new ImmutableFourCZipDictionary<TKey>(first, second, third, fourth),
                        _ => throw new NotSupportedException($"BUG: length overflow {index}"),
                    };
                }
                else
                {
                    return new ImmutableCZipDictionary<TKey>(keyValues);
                }

            case ZipCollectionsLevel.Basic:
                return new ImmutableCZipDictionary<TKey>(keyValues);
            case ZipCollectionsLevel.Trivial:
                return keyValues.ToImmutableDictionary();
            default:
                throw new NotSupportedException($"BUG: Unknown level {this.Level}");
        }
    }

    /// <inheritdoc />
    public IImmutableList<string> CreateCompressed(
            IEnumerable<string> values)
    {
        return this.Level switch
        {
            ZipCollectionsLevel.High => new ImmutableCList(values),
            ZipCollectionsLevel.Basic => new ImmutableCList(values),
            ZipCollectionsLevel.Trivial => values.ToImmutableList(),
            _ => throw new NotSupportedException($"BUG: Unknown level {this.Level}"),
        };
    }
}
