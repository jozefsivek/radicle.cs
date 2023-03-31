namespace Radicle.Alike.Redis.Models;

using System;
using System.Collections.Immutable;
using System.Text;

/// <summary>
/// Base class for all immutable RESP values.
/// see https://redis.io/topics/protocol
/// and https://github.com/redis/redis-specifications/blob/master/protocol/RESP3.md
/// (formerly https://github.com/antirez/RESP3/blob/master/spec.md).
/// </summary>
/// <remarks>All implementations of the <see cref="RESPValue"/>
/// have human friendly ToString implementaion aimed for pretty printing.</remarks>
public abstract class RESPValue : IEquatable<RESPValue>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RESPValue"/> class.
    /// </summary>
    protected internal RESPValue()
    {
    }

    /// <summary>
    /// Gets the type of this data value.
    /// </summary>
    public abstract RESPDataType Type { get; }

    /// <summary>
    /// Gets optional attributes associated with this value.
    /// </summary>
    public RESPAttributeValue? Attribs { get; init; }

    /// <summary>
    /// Compare two instances of <see cref="RESPValue"/>.
    /// </summary>
    /// <param name="one">Left.</param>
    /// <param name="other">Rigt.</param>
    /// <returns><see langword="true"/> if equal; <see langword="false"/> otherwise.</returns>
    public static bool operator ==(RESPValue? one, RESPValue? other)
    {
        return one is null ? other is null : one.Equals(other);
    }

    /// <summary>
    /// Compare two instances of <see cref="RESPValue"/>.
    /// </summary>
    /// <param name="one">Left.</param>
    /// <param name="other">Rigt.</param>
    /// <returns><see langword="true"/> if not equal; <see langword="false"/> otherwise.</returns>
    public static bool operator !=(RESPValue? one, RESPValue? other)
    {
        return !(one == other);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is RESPValue value && this.Equals(value);
    }

    /// <inheritdoc/>
    public sealed override string ToString()
    {
        return new StringBuilder()
                .AppendJoin(Environment.NewLine, this.Accept(new RESPValuePrettyFormatter()))
                .ToString();
    }

    /// <inheritdoc />
    public abstract bool Equals(RESPValue? other);

    /// <inheritdoc/>
    public abstract override int GetHashCode();

    /// <summary>
    /// Accept given <paramref name="visitor"/>
    /// and return its result.
    /// </summary>
    /// <typeparam name="TResult">Type of result.</typeparam>
    /// <param name="visitor">Visitor.</param>
    /// <returns>Visitor result.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public abstract TResult Accept<TResult>(IRESPValueVisitor<TResult> visitor);

    /// <summary>
    /// Calculates fast hash code of the immutable byte array
    /// by sampling subset of values.
    /// </summary>
    /// <param name="value">Value to calculate hash code for.</param>
    /// <returns>Hash code.</returns>
    protected static int GetHashCode(ImmutableArray<byte> value)
    {
        int l = value.Length;

        if (l == 0)
        {
            return 0;
        }

        // TODO: ones available maybe
        // https://learn.microsoft.com/en-us/dotnet/api/system.hashcode.addbytes?view=net-8.0
        HashCode hc = default;
        const int N = 12;

        hc.Add(l); // explicitly count length

        for (int i = 0; i < l && i < N; i++)
        {
            hc.Add(value[i]);
        }

        if (l > N)
        {
            int incr = l / N;

            for (int i = N; i < l; i += incr)
            {
                hc.Add(value[i]);
            }
        }

        return hc.ToHashCode();
    }
}
