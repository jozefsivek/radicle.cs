namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Radicle.Common.Check;

/// <summary>
/// Base class for string-like RESP values.
/// </summary>
public abstract class RESPStringLikeValue : RESPValue
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RESPStringLikeValue"/> class.
    /// </summary>
    /// <param name="stringValue">Value of this string
    ///     which will be serialized in UTF-8 encoding.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    protected internal RESPStringLikeValue(string stringValue)
    {
        this.Value = RESPNames.DefaultEncoding
                .GetBytes(Ensure.Param(stringValue).Value)
                .ToImmutableArray();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPStringLikeValue"/> class.
    /// </summary>
    /// <param name="value">Binary value of this string.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    protected internal RESPStringLikeValue(IEnumerable<byte> value)
    {
        this.Value = Ensure.Param(value).ToImmutableArray();
    }

    /// <summary>
    /// Gets value as byte array.
    /// </summary>
    public ImmutableArray<byte> Value { get; }

    /// <summary>
    /// Gets string value from <see cref="Value"/>.
    /// </summary>
    /// <remarks>See https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding.getstring?view=net-8.0#system-text-encoding-getstring(system-byte()).</remarks>
    /// <exception cref="ArgumentException">The byte array contains invalid Unicode code points.</exception>
    /// <exception cref="DecoderFallbackException">A fallback occurred.</exception>
    public string StringValue => RESPNames.DefaultEncoding.GetString(this.Value.ToArray());
}
