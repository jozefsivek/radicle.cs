namespace Radicle.Common.Check.Base;

using System;

/// <summary>
/// Base class of all immutable and typed names.
/// </summary>
/// <typeparam name="T">The type which derives from this class.</typeparam>
public abstract class TypedName<T> : ITypedName, IEquatable<T>, IComparable, IComparable<T>
    where T : TypedName<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypedName{T}"/> class.
    /// </summary>
    /// <param name="value">Identifier name which should comply with the
    ///     specification given by <see cref="Spec"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown if any input
    ///     is <see langword="null"/> or <see cref="Spec"/>
    ///     is not defined on construction time.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the
    ///     input exceeds allowed ranges.</exception>
    /// <exception cref="ArgumentException">Thrown if the input does not
    ///     conform to expected form.</exception>
    protected TypedName(string value)
    {
        _ = this.Spec.EnsureValid(value);
        this.Value = this.InternValues
                ? string.Intern(value)
                : value;

        string portableValue = this.Value;

        if (this.Spec.IgnoreCaseWhenCompared)
        {
#pragma warning disable CA1308 // Normalize strings to uppercase
            portableValue = this.Value.ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
        }

        this.InvariantValue = this.InternValues
                ? string.Intern(portableValue)
                : portableValue;
    }

    /// <inheritdoc />
    public string Value { get; }

    /// <inheritdoc />
    public string InvariantValue { get; }

    /// <inheritdoc />
    public abstract TypedNameSpec Spec { get; }

    /// <summary>
    /// Gets a value indicating whether to allow interning
    /// of the value to save memory. Defaults to <see langword="true"/>.
    /// </summary>
    protected virtual bool InternValues { get; } = true;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static bool operator ==(TypedName<T>? one, object? other)
    {
        return one is null ? other is null : one.Equals(other);
    }

    public static bool operator !=(TypedName<T>? one, object? other)
    {
        return one is null ? other is not null : !one.Equals(other);
    }

    public static bool operator ==(TypedName<T>? one, TypedName<T>? other)
    {
        return one is null ? other is null : one.Equals(other);
    }

    public static bool operator !=(TypedName<T>? one, TypedName<T>? other)
    {
        return one is null ? other is not null : !one.Equals(other);
    }

    public static bool operator ==(object? other, TypedName<T>? one)
    {
        return one is null ? other is null : one.Equals(other);
    }

    public static bool operator !=(object? other, TypedName<T>? one)
    {
        return one is null ? other is not null : !one.Equals(other);
    }

    public static bool operator >(TypedName<T>? one, TypedName<T>? other)
    {
        if (one is null)
        {
            return other?.CompareTo(one) < 0;
        }

        return one.CompareTo(other) > 0;
    }

    public static bool operator >=(TypedName<T>? one, TypedName<T>? other)
    {
        if (one is null)
        {
            return other is null || other.CompareTo(one) <= 0;
        }

        return one.CompareTo(other) >= 0;
    }

    public static bool operator <(TypedName<T>? one, TypedName<T>? other)
    {
        if (one is null)
        {
            return other?.CompareTo(one) > 0;
        }

        return one.CompareTo(other) < 0;
    }

    public static bool operator <=(TypedName<T>? one, TypedName<T>? other)
    {
        if (one is null)
        {
            return other is null || other.CompareTo(one) >= 0;
        }

        return one.CompareTo(other) <= 0;
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    /// <inheritdoc />
    public override string ToString()
    {
        return $"name: {Dump.Literal(this.InvariantValue)}";
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is T other)
        {
            return this.Equals(other);
        }

        return false;
    }

    /// <inheritdoc />
    public bool Equals(T? other)
    {
        if (other is null)
        {
            return false;
        }

        return this.InvariantValue.Equals(
                other.InvariantValue,
                StringComparison.Ordinal);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return this.InvariantValue.GetHashCode(StringComparison.Ordinal);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentException">Thrown if <paramref name="obj"/>
    ///     is not <typeparamref name="T"/>, <see langword="null"/>
    ///     values are allowed.</exception>
    public int CompareTo(object? obj)
    {
        // see https://docs.microsoft.com/en-us/dotnet/api/system.icomparable-1?view=netframework-4.7.2
        // If other is not a valid object reference, this instance is greater.
        if (obj is null)
        {
            return 1;
        }
        else if (obj is T other)
        {
            return this.CompareTo(other);
        }
        else
        {
            throw new ArgumentException(
                    $"Parameter {nameof(obj)} "
                        + $"to compare this instance to has to be of type {nameof(T)}",
                    nameof(obj));
        }
    }

    /// <inheritdoc />
    public int CompareTo(T? other)
    {
        // see https://docs.microsoft.com/en-us/dotnet/api/system.icomparable-1?view=netframework-4.7.2
        // If other is not a valid object reference, this instance is greater.
        if (other is null)
        {
            return 1;
        }

        return string.CompareOrdinal(
                this.InvariantValue,
                other.InvariantValue);
    }
}
