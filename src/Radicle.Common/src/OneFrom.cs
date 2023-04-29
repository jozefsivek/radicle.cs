namespace Radicle.Common;

using System;
using System.Linq;
using Radicle.Common.Check;
using Radicle.Common.MetaData;

/// <summary>
/// Base class and container for static convenience methods
/// for constructing ad-hoc instances of <see cref="IOneFrom"/>
/// of different cardinalities.
/// </summary>
/// <remarks>
/// <para>
/// In a nutshell there are 3 ways:
/// 1. construct your own class and implement one of the <see cref="IOneFrom"/>
/// interface of your choosen cardinality.
/// 2. Construct an ad-hoc instance with call to e.g. <see cref="OneFrom{T0, T1}.FromT0(T0)"/>.
/// 3. Construct an ad-hoc instance with call builder method,
/// get a builder like <see cref="CreateBuilder{T0, T1}()"/>.
/// </para>
/// <para>Why to have builder? So that you do not need to repeat
/// the lengthy generic types if you do not want if you want to
/// return <see cref="IOneFrom"/> on multiple places in your code:</para>
/// <code>
/// var builder = OneFrom.CreateBuilder&lt;T1, T2&gt;();
///
/// if (one)
/// {
///     return builder.From(oneType);
/// }
/// else
/// {
///     return builder.From(otherType);
/// }
/// </code>
/// <para>
/// Instead of:
/// </para>
/// <code>
/// if (one)
/// {
///     return OneFrom&lt;T1, T2&gt;.From(oneType);
/// }
/// else
/// {
///     return OneFrom&lt;T1, T2&gt;.From(otherType);
/// }
/// </code>
/// </remarks>
[Experimental("Experimental use now only.")]
public abstract class OneFrom : IOneFrom
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneFrom"/> class.
    /// </summary>
    /// <param name="typeIndex">Index of value.</param>
    /// <param name="value">Value.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    internal OneFrom(byte typeIndex, object value)
    {
        this.SumTypeValueIndex = typeIndex;
        this.SumTypeValue = Ensure.Param(value).Value;
    }

    /// <inheritdoc/>
    public object SumTypeValue { get; }

    /// <inheritdoc/>
    public byte SumTypeValueIndex { get; }

    /// <summary>
    /// Create builder for sum type of one value.
    /// </summary>
    /// <typeparam name="T0">Type of 1st argument.</typeparam>
    /// <returns>Builder class.</returns>
    public static OneFrom<T0>.Builder CreateBuilder<T0>()
        where T0 : notnull
    {
        return new OneFrom<T0>.Builder();
    }

    /// <summary>
    /// Create builder for sum type of two values.
    /// </summary>
    /// <typeparam name="T0">Type of 1st argument.</typeparam>
    /// <typeparam name="T1">Type of 2nd argument.</typeparam>
    /// <returns>Builder class.</returns>
    public static OneFrom<T0, T1>.Builder CreateBuilder<T0, T1>()
        where T0 : notnull
        where T1 : notnull
    {
        return new OneFrom<T0, T1>.Builder();
    }

    /// <summary>
    /// Create builder for sum type of three values.
    /// </summary>
    /// <typeparam name="T0">Type of 1st argument.</typeparam>
    /// <typeparam name="T1">Type of 2nd argument.</typeparam>
    /// <typeparam name="T2">Type of 3th argument.</typeparam>
    /// <returns>Builder class.</returns>
    public static OneFrom<T0, T1, T2>.Builder CreateBuilder<T0, T1, T2>()
        where T0 : notnull
        where T1 : notnull
        where T2 : notnull
    {
        return new OneFrom<T0, T1, T2>.Builder();
    }

    /// <summary>
    /// Create builder for sum type of four values.
    /// </summary>
    /// <typeparam name="T0">Type of 1st argument.</typeparam>
    /// <typeparam name="T1">Type of 2nd argument.</typeparam>
    /// <typeparam name="T2">Type of 3th argument.</typeparam>
    /// <typeparam name="T3">Type of 4th argument.</typeparam>
    /// <returns>Builder class.</returns>
    public static OneFrom<T0, T1, T2, T3>.Builder CreateBuilder<T0, T1, T2, T3>()
        where T0 : notnull
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
    {
        return new OneFrom<T0, T1, T2, T3>.Builder();
    }

    /// <summary>
    /// Create builder for sum type of five values.
    /// </summary>
    /// <typeparam name="T0">Type of 1st argument.</typeparam>
    /// <typeparam name="T1">Type of 2nd argument.</typeparam>
    /// <typeparam name="T2">Type of 3th argument.</typeparam>
    /// <typeparam name="T3">Type of 4th argument.</typeparam>
    /// <typeparam name="T4">Type of 5th argument.</typeparam>
    /// <returns>Builder class.</returns>
    public static OneFrom<T0, T1, T2, T3, T4>.Builder CreateBuilder<T0, T1, T2, T3, T4>()
        where T0 : notnull
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
    {
        return new OneFrom<T0, T1, T2, T3, T4>.Builder();
    }

    /// <summary>
    /// Create builder for sum type of six values.
    /// </summary>
    /// <typeparam name="T0">Type of 1st argument.</typeparam>
    /// <typeparam name="T1">Type of 2nd argument.</typeparam>
    /// <typeparam name="T2">Type of 3th argument.</typeparam>
    /// <typeparam name="T3">Type of 4th argument.</typeparam>
    /// <typeparam name="T4">Type of 5th argument.</typeparam>
    /// <typeparam name="T5">Type of 6th argument.</typeparam>
    /// <returns>Builder class.</returns>
    public static OneFrom<T0, T1, T2, T3, T4, T5>.Builder CreateBuilder<T0, T1, T2, T3, T4, T5>()
        where T0 : notnull
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
        where T5 : notnull
    {
        return new OneFrom<T0, T1, T2, T3, T4, T5>.Builder();
    }

    /// <summary>
    /// Create builder for sum type of seven values.
    /// </summary>
    /// <typeparam name="T0">Type of 1st argument.</typeparam>
    /// <typeparam name="T1">Type of 2nd argument.</typeparam>
    /// <typeparam name="T2">Type of 3th argument.</typeparam>
    /// <typeparam name="T3">Type of 4th argument.</typeparam>
    /// <typeparam name="T4">Type of 5th argument.</typeparam>
    /// <typeparam name="T5">Type of 6th argument.</typeparam>
    /// <typeparam name="T6">Type of 7th argument.</typeparam>
    /// <returns>Builder class.</returns>
    public static OneFrom<T0, T1, T2, T3, T4, T5, T6>.Builder CreateBuilder<T0, T1, T2, T3, T4, T5, T6>()
        where T0 : notnull
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
        where T5 : notnull
        where T6 : notnull
    {
        return new OneFrom<T0, T1, T2, T3, T4, T5, T6>.Builder();
    }

    /// <summary>
    /// Create builder for sum type of eight values.
    /// </summary>
    /// <typeparam name="T0">Type of 1st argument.</typeparam>
    /// <typeparam name="T1">Type of 2nd argument.</typeparam>
    /// <typeparam name="T2">Type of 3th argument.</typeparam>
    /// <typeparam name="T3">Type of 4th argument.</typeparam>
    /// <typeparam name="T4">Type of 5th argument.</typeparam>
    /// <typeparam name="T5">Type of 6th argument.</typeparam>
    /// <typeparam name="T6">Type of 7th argument.</typeparam>
    /// <typeparam name="T7">Type of 8th argument.</typeparam>
    /// <returns>Builder class.</returns>
    public static OneFrom<T0, T1, T2, T3, T4, T5, T6, T7>.Builder CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7>()
        where T0 : notnull
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
        where T5 : notnull
        where T6 : notnull
        where T7 : notnull
    {
        return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7>.Builder();
    }

    /// <summary>
    /// Create builder for sum type of nine values.
    /// </summary>
    /// <typeparam name="T0">Type of 1st argument.</typeparam>
    /// <typeparam name="T1">Type of 2nd argument.</typeparam>
    /// <typeparam name="T2">Type of 3th argument.</typeparam>
    /// <typeparam name="T3">Type of 4th argument.</typeparam>
    /// <typeparam name="T4">Type of 5th argument.</typeparam>
    /// <typeparam name="T5">Type of 6th argument.</typeparam>
    /// <typeparam name="T6">Type of 7th argument.</typeparam>
    /// <typeparam name="T7">Type of 8th argument.</typeparam>
    /// <typeparam name="T8">Type of 9th argument.</typeparam>
    /// <returns>Builder class.</returns>
    public static OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8>.Builder CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8>()
        where T0 : notnull
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
        where T5 : notnull
        where T6 : notnull
        where T7 : notnull
        where T8 : notnull
    {
        return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8>.Builder();
    }

    /// <summary>
    /// Create builder for sum type of ten values.
    /// </summary>
    /// <typeparam name="T0">Type of 1st argument.</typeparam>
    /// <typeparam name="T1">Type of 2nd argument.</typeparam>
    /// <typeparam name="T2">Type of 3th argument.</typeparam>
    /// <typeparam name="T3">Type of 4th argument.</typeparam>
    /// <typeparam name="T4">Type of 5th argument.</typeparam>
    /// <typeparam name="T5">Type of 6th argument.</typeparam>
    /// <typeparam name="T6">Type of 7th argument.</typeparam>
    /// <typeparam name="T7">Type of 8th argument.</typeparam>
    /// <typeparam name="T8">Type of 9th argument.</typeparam>
    /// <typeparam name="T9">Type of 10th argument.</typeparam>
    /// <returns>Builder class.</returns>
    public static OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>.Builder CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>()
        where T0 : notnull
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
        where T5 : notnull
        where T6 : notnull
        where T7 : notnull
        where T8 : notnull
        where T9 : notnull
    {
        return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>.Builder();
    }

    /// <summary>
    /// Converts index to remainder sum type index.
    /// </summary>
    /// <param name="source">Source index.</param>
    /// <param name="picked">Index which was picked not equal to
    ///     <paramref name="source"/>.</param>
    /// <returns>Converted index.</returns>
    /// <exception cref="NotSupportedException">Thrown
    ///     in case of bug, <paramref name="picked"/>
    ///     is equal to <paramref name="source"/>.</exception>
    internal static byte ConvertIndex(byte source, byte picked)
    {
        if (source < picked)
        {
            return source;
        }
        else if (source > picked)
        {
            return (byte)(source - 1);
        }
        else
        {
            throw new NotSupportedException(
                    $"BUG: sum type remainder made with wrong picked index {source}");
        }
    }

    /// <summary>
    /// Throwns error if the <paramref name="index"/>
    /// if equal or larger than <paramref name="expectedCardinality"/>.
    /// </summary>
    /// <param name="index">Index to check.</param>
    /// <param name="expectedCardinality">Expected cardinality (1-based number)
    ///     of the sum type.</param>
    /// <exception cref="NotSupportedException">Thrown
    ///     in case of bug, i.e. if the index is outside cardinality.</exception>
    internal static void EnsureCardinality(byte index, byte expectedCardinality)
    {
        if (index >= expectedCardinality)
        {
            throw new NotSupportedException(
                    $"BUG: implementation of {nameof(IOneFrom)} claims value index "
                    + $"{index} outside expressed cardinality {expectedCardinality}");
        }
    }
}

#pragma warning disable CA1000 // Do not declare static members on generic types
#pragma warning disable SA1402 // File may only contain a single type

/// <summary>
/// Internal implementation of sum type.
/// </summary>
/// <typeparam name="T0">Type of 1st argument.</typeparam>
[Experimental("Experimental use now only.")]
public class OneFrom<T0> : OneFrom, IOneFrom<T0>
    where T0 : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneFrom{T0}"/> class.
    /// </summary>
    /// <param name="typeIndex">Index of value.</param>
    /// <param name="value">Value.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    internal OneFrom(byte typeIndex, object value)
        : base(typeIndex, value)
    {
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0> FromT0(T0 value)
    {
        return CreateBuilder<T0>().FromT0(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type by probing its type.
    /// </summary>
    /// <remarks>Naturally, do not use derived types
    /// as the check may produce unexpected results.</remarks>
    /// <param name="value">Value to probe.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
    ///     is none of the expected type.</exception>
    public static IOneFrom<T0> From(object value)
    {
        return CreateBuilder<T0>().From(value);
    }

    /// <summary>
    /// Builder class.
    /// </summary>
    public sealed class Builder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Builder"/> class.
        /// </summary>
        internal Builder()
        {
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0> FromT0(T0 value)
        {
            return new OneFrom<T0>(0, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type by probing its type.
        /// </summary>
        /// <remarks>Naturally, do not use derived types
        /// as the check may produce unexpected results.</remarks>
        /// <param name="value">Value to probe.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
        ///     is none of the expected type.</exception>
        public IOneFrom<T0> From(object value)
        {
            Ensure.Param(value).Done();

            if (value is T0 t0)
            {
                return this.FromT0(t0);
            }

            string allowedTypes = string.Join(
                    ", ",
                    new[] { typeof(T0) }.Select(t => t.Name));

            throw new ArgumentException(
                    $"Parameter is none of the expected type: {allowedTypes}",
                    nameof(value));
        }
    }
}

/// <summary>
/// Internal implementation of sum type.
/// </summary>
/// <typeparam name="T0">Type of 1st argument.</typeparam>
/// <typeparam name="T1">Type of 2nd argument.</typeparam>
[Experimental("Experimental use now only.")]
public class OneFrom<T0, T1> : OneFrom, IOneFrom<T0, T1>
    where T0 : notnull
    where T1 : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneFrom{T0, T1}"/> class.
    /// </summary>
    /// <param name="typeIndex">Index of value.</param>
    /// <param name="value">Value.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    internal OneFrom(byte typeIndex, object value)
        : base(typeIndex, value)
    {
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1> FromT0(T0 value)
    {
        return CreateBuilder<T0, T1>().FromT0(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1> FromT1(T1 value)
    {
        return CreateBuilder<T0, T1>().FromT1(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type by probing its type.
    /// </summary>
    /// <remarks>Naturally, do not use derived types
    /// as the check may produce unexpected results.</remarks>
    /// <param name="value">Value to probe.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
    ///     is none of the expected type.</exception>
    public static IOneFrom<T0, T1> From(object value)
    {
        return CreateBuilder<T0, T1>().From(value);
    }

    /// <summary>
    /// Builder class.
    /// </summary>
    public sealed class Builder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Builder"/> class.
        /// </summary>
        internal Builder()
        {
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1> FromT0(T0 value)
        {
            return new OneFrom<T0, T1>(0, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1> FromT1(T1 value)
        {
            return new OneFrom<T0, T1>(1, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type by probing its type.
        /// </summary>
        /// <remarks>Naturally, do not use derived types
        /// as the check may produce unexpected results.</remarks>
        /// <param name="value">Value to probe.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
        ///     is none of the expected type.</exception>
        public IOneFrom<T0, T1> From(object value)
        {
            Ensure.Param(value).Done();

            if (value is T0 t0)
            {
                return this.FromT0(t0);
            }
            else if (value is T1 t1)
            {
                return this.FromT1(t1);
            }
            else
            {
                string allowedTypes = string.Join(
                        ", ",
                        new[] { typeof(T0), typeof(T1) }.Select(t => t.Name));
                throw new ArgumentException(
                        $"Parameter is none of the expected type: {allowedTypes}",
                        nameof(value));
            }
        }
    }
}

/// <summary>
/// Internal implementation of sum type.
/// </summary>
/// <typeparam name="T0">Type of 1st argument.</typeparam>
/// <typeparam name="T1">Type of 2nd argument.</typeparam>
/// <typeparam name="T2">Type of 3th argument.</typeparam>
[Experimental("Experimental use now only.")]
public class OneFrom<T0, T1, T2> : OneFrom, IOneFrom<T0, T1, T2>
    where T0 : notnull
    where T1 : notnull
    where T2 : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneFrom{T0, T1, T2}"/> class.
    /// </summary>
    /// <param name="typeIndex">Index of value.</param>
    /// <param name="value">Value.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    internal OneFrom(byte typeIndex, object value)
        : base(typeIndex, value)
    {
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2> FromT0(T0 value)
    {
        return CreateBuilder<T0, T1, T2>().FromT0(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2> FromT1(T1 value)
    {
        return CreateBuilder<T0, T1, T2>().FromT1(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2> FromT2(T2 value)
    {
        return CreateBuilder<T0, T1, T2>().FromT2(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type by probing its type.
    /// </summary>
    /// <remarks>Naturally, do not use derived types
    /// as the check may produce unexpected results.</remarks>
    /// <param name="value">Value to probe.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
    ///     is none of the expected type.</exception>
    public static IOneFrom<T0, T1, T2> From(object value)
    {
        return CreateBuilder<T0, T1, T2>().From(value);
    }

    /// <summary>
    /// Builder class.
    /// </summary>
    public sealed class Builder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Builder"/> class.
        /// </summary>
        internal Builder()
        {
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2> FromT0(T0 value)
        {
            return new OneFrom<T0, T1, T2>(0, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2> FromT1(T1 value)
        {
            return new OneFrom<T0, T1, T2>(1, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2> FromT2(T2 value)
        {
            return new OneFrom<T0, T1, T2>(2, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type by probing its type.
        /// </summary>
        /// <remarks>Naturally, do not use derived types
        /// as the check may produce unexpected results.</remarks>
        /// <param name="value">Value to probe.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
        ///     is none of the expected type.</exception>
        public IOneFrom<T0, T1, T2> From(object value)
        {
            Ensure.Param(value).Done();

            if (value is T0 t0)
            {
                return this.FromT0(t0);
            }
            else if (value is T1 t1)
            {
                return this.FromT1(t1);
            }
            else if (value is T2 t2)
            {
                return this.FromT2(t2);
            }
            else
            {
                string allowedTypes = string.Join(
                        ", ",
                        new[] { typeof(T0), typeof(T1), typeof(T2) }.Select(t => t.Name));
                throw new ArgumentException(
                        $"Parameter is none of the expected type: {allowedTypes}",
                        nameof(value));
            }
        }
    }
}

/// <summary>
/// Internal implementation of sum type.
/// </summary>
/// <typeparam name="T0">Type of 1st argument.</typeparam>
/// <typeparam name="T1">Type of 2nd argument.</typeparam>
/// <typeparam name="T2">Type of 3th argument.</typeparam>
/// <typeparam name="T3">Type of 4th argument.</typeparam>
[Experimental("Experimental use now only.")]
public class OneFrom<T0, T1, T2, T3> : OneFrom, IOneFrom<T0, T1, T2, T3>
    where T0 : notnull
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneFrom{T0, T1, T2, T3}"/> class.
    /// </summary>
    /// <param name="typeIndex">Index of value.</param>
    /// <param name="value">Value.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    internal OneFrom(byte typeIndex, object value)
        : base(typeIndex, value)
    {
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3> FromT0(T0 value)
    {
        return CreateBuilder<T0, T1, T2, T3>().FromT0(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3> FromT1(T1 value)
    {
        return CreateBuilder<T0, T1, T2, T3>().FromT1(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3> FromT2(T2 value)
    {
        return CreateBuilder<T0, T1, T2, T3>().FromT2(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3> FromT3(T3 value)
    {
        return CreateBuilder<T0, T1, T2, T3>().FromT3(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type by probing its type.
    /// </summary>
    /// <remarks>Naturally, do not use derived types
    /// as the check may produce unexpected results.</remarks>
    /// <param name="value">Value to probe.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
    ///     is none of the expected type.</exception>
    public static IOneFrom<T0, T1, T2, T3> From(object value)
    {
        return CreateBuilder<T0, T1, T2, T3>().From(value);
    }

    /// <summary>
    /// Builder class.
    /// </summary>
    public sealed class Builder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Builder"/> class.
        /// </summary>
        internal Builder()
        {
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3> FromT0(T0 value)
        {
            return new OneFrom<T0, T1, T2, T3>(0, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3> FromT1(T1 value)
        {
            return new OneFrom<T0, T1, T2, T3>(1, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3> FromT2(T2 value)
        {
            return new OneFrom<T0, T1, T2, T3>(2, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3> FromT3(T3 value)
        {
            return new OneFrom<T0, T1, T2, T3>(3, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type by probing its type.
        /// </summary>
        /// <remarks>Naturally, do not use derived types
        /// as the check may produce unexpected results.</remarks>
        /// <param name="value">Value to probe.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
        ///     is none of the expected type.</exception>
        public IOneFrom<T0, T1, T2, T3> From(object value)
        {
            Ensure.Param(value).Done();

            if (value is T0 t0)
            {
                return this.FromT0(t0);
            }
            else if (value is T1 t1)
            {
                return this.FromT1(t1);
            }
            else if (value is T2 t2)
            {
                return this.FromT2(t2);
            }
            else if (value is T3 t3)
            {
                return this.FromT3(t3);
            }
            else
            {
                string allowedTypes = string.Join(
                        ", ",
                        new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3) }.Select(t => t.Name));
                throw new ArgumentException(
                        $"Parameter is none of the expected type: {allowedTypes}",
                        nameof(value));
            }
        }
    }
}

/// <summary>
/// Internal implementation of sum type.
/// </summary>
/// <typeparam name="T0">Type of 1st argument.</typeparam>
/// <typeparam name="T1">Type of 2nd argument.</typeparam>
/// <typeparam name="T2">Type of 3th argument.</typeparam>
/// <typeparam name="T3">Type of 4th argument.</typeparam>
/// <typeparam name="T4">Type of 5th argument.</typeparam>
[Experimental("Experimental use now only.")]
public class OneFrom<T0, T1, T2, T3, T4> : OneFrom, IOneFrom<T0, T1, T2, T3, T4>
    where T0 : notnull
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where T4 : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneFrom{T0, T1, T2, T3, T4}"/> class.
    /// </summary>
    /// <param name="typeIndex">Index of value.</param>
    /// <param name="value">Value.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    internal OneFrom(byte typeIndex, object value)
        : base(typeIndex, value)
    {
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4> FromT0(T0 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4>().FromT0(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4> FromT1(T1 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4>().FromT1(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4> FromT2(T2 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4>().FromT2(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4> FromT3(T3 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4>().FromT3(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4> FromT4(T4 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4>().FromT4(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type by probing its type.
    /// </summary>
    /// <remarks>Naturally, do not use derived types
    /// as the check may produce unexpected results.</remarks>
    /// <param name="value">Value to probe.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
    ///     is none of the expected type.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4> From(object value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4>().From(value);
    }

    /// <summary>
    /// Builder class.
    /// </summary>
    public sealed class Builder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Builder"/> class.
        /// </summary>
        internal Builder()
        {
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4> FromT0(T0 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4>(0, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4> FromT1(T1 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4>(1, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4> FromT2(T2 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4>(2, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4> FromT3(T3 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4>(3, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4> FromT4(T4 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4>(4, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type by probing its type.
        /// </summary>
        /// <remarks>Naturally, do not use derived types
        /// as the check may produce unexpected results.</remarks>
        /// <param name="value">Value to probe.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
        ///     is none of the expected type.</exception>
        public IOneFrom<T0, T1, T2, T3, T4> From(object value)
        {
            Ensure.Param(value).Done();

            if (value is T0 t0)
            {
                return this.FromT0(t0);
            }
            else if (value is T1 t1)
            {
                return this.FromT1(t1);
            }
            else if (value is T2 t2)
            {
                return this.FromT2(t2);
            }
            else if (value is T3 t3)
            {
                return this.FromT3(t3);
            }
            else if (value is T4 t4)
            {
                return this.FromT4(t4);
            }
            else
            {
                string allowedTypes = string.Join(
                        ", ",
                        new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4) }.Select(t => t.Name));
                throw new ArgumentException(
                        $"Parameter is none of the expected type: {allowedTypes}",
                        nameof(value));
            }
        }
    }
}

/// <summary>
/// Internal implementation of sum type.
/// </summary>
/// <typeparam name="T0">Type of 1st argument.</typeparam>
/// <typeparam name="T1">Type of 2nd argument.</typeparam>
/// <typeparam name="T2">Type of 3th argument.</typeparam>
/// <typeparam name="T3">Type of 4th argument.</typeparam>
/// <typeparam name="T4">Type of 5th argument.</typeparam>
/// <typeparam name="T5">Type of 6th argument.</typeparam>
[Experimental("Experimental use now only.")]
public class OneFrom<T0, T1, T2, T3, T4, T5> : OneFrom, IOneFrom<T0, T1, T2, T3, T4, T5>
    where T0 : notnull
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where T4 : notnull
    where T5 : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneFrom{T0, T1, T2, T3, T4, T5}"/> class.
    /// </summary>
    /// <param name="typeIndex">Index of value.</param>
    /// <param name="value">Value.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    internal OneFrom(byte typeIndex, object value)
        : base(typeIndex, value)
    {
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5> FromT0(T0 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5>().FromT0(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5> FromT1(T1 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5>().FromT1(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5> FromT2(T2 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5>().FromT2(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5> FromT3(T3 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5>().FromT3(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5> FromT4(T4 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5>().FromT4(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5> FromT5(T5 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5>().FromT5(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type by probing its type.
    /// </summary>
    /// <remarks>Naturally, do not use derived types
    /// as the check may produce unexpected results.</remarks>
    /// <param name="value">Value to probe.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
    ///     is none of the expected type.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5> From(object value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5>().From(value);
    }

    /// <summary>
    /// Builder class.
    /// </summary>
    public sealed class Builder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Builder"/> class.
        /// </summary>
        internal Builder()
        {
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5> FromT0(T0 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5>(0, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5> FromT1(T1 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5>(1, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5> FromT2(T2 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5>(2, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5> FromT3(T3 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5>(3, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5> FromT4(T4 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5>(4, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5> FromT5(T5 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5>(5, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type by probing its type.
        /// </summary>
        /// <remarks>Naturally, do not use derived types
        /// as the check may produce unexpected results.</remarks>
        /// <param name="value">Value to probe.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
        ///     is none of the expected type.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5> From(object value)
        {
            Ensure.Param(value).Done();

            if (value is T0 t0)
            {
                return this.FromT0(t0);
            }
            else if (value is T1 t1)
            {
                return this.FromT1(t1);
            }
            else if (value is T2 t2)
            {
                return this.FromT2(t2);
            }
            else if (value is T3 t3)
            {
                return this.FromT3(t3);
            }
            else if (value is T4 t4)
            {
                return this.FromT4(t4);
            }
            else if (value is T5 t5)
            {
                return this.FromT5(t5);
            }
            else
            {
                string allowedTypes = string.Join(
                        ", ",
                        new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }.Select(t => t.Name));
                throw new ArgumentException(
                        $"Parameter is none of the expected type: {allowedTypes}",
                        nameof(value));
            }
        }
    }
}

/// <summary>
/// Internal implementation of sum type.
/// </summary>
/// <typeparam name="T0">Type of 1st argument.</typeparam>
/// <typeparam name="T1">Type of 2nd argument.</typeparam>
/// <typeparam name="T2">Type of 3th argument.</typeparam>
/// <typeparam name="T3">Type of 4th argument.</typeparam>
/// <typeparam name="T4">Type of 5th argument.</typeparam>
/// <typeparam name="T5">Type of 6th argument.</typeparam>
/// <typeparam name="T6">Type of 7th argument.</typeparam>
[Experimental("Experimental use now only.")]
public class OneFrom<T0, T1, T2, T3, T4, T5, T6> : OneFrom, IOneFrom<T0, T1, T2, T3, T4, T5, T6>
    where T0 : notnull
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where T4 : notnull
    where T5 : notnull
    where T6 : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneFrom{T0, T1, T2, T3, T4, T5, T6}"/> class.
    /// </summary>
    /// <param name="typeIndex">Index of value.</param>
    /// <param name="value">Value.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    internal OneFrom(byte typeIndex, object value)
        : base(typeIndex, value)
    {
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6> FromT0(T0 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6>().FromT0(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6> FromT1(T1 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6>().FromT1(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6> FromT2(T2 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6>().FromT2(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6> FromT3(T3 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6>().FromT3(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6> FromT4(T4 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6>().FromT4(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6> FromT5(T5 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6>().FromT5(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6> FromT6(T6 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6>().FromT6(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type by probing its type.
    /// </summary>
    /// <remarks>Naturally, do not use derived types
    /// as the check may produce unexpected results.</remarks>
    /// <param name="value">Value to probe.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
    ///     is none of the expected type.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6> From(object value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6>().From(value);
    }

    /// <summary>
    /// Builder class.
    /// </summary>
    public sealed class Builder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Builder"/> class.
        /// </summary>
        internal Builder()
        {
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6> FromT0(T0 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6>(0, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6> FromT1(T1 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6>(1, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6> FromT2(T2 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6>(2, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6> FromT3(T3 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6>(3, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6> FromT4(T4 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6>(4, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6> FromT5(T5 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6>(5, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6> FromT6(T6 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6>(6, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type by probing its type.
        /// </summary>
        /// <remarks>Naturally, do not use derived types
        /// as the check may produce unexpected results.</remarks>
        /// <param name="value">Value to probe.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
        ///     is none of the expected type.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6> From(object value)
        {
            Ensure.Param(value).Done();

            if (value is T0 t0)
            {
                return this.FromT0(t0);
            }
            else if (value is T1 t1)
            {
                return this.FromT1(t1);
            }
            else if (value is T2 t2)
            {
                return this.FromT2(t2);
            }
            else if (value is T3 t3)
            {
                return this.FromT3(t3);
            }
            else if (value is T4 t4)
            {
                return this.FromT4(t4);
            }
            else if (value is T5 t5)
            {
                return this.FromT5(t5);
            }
            else if (value is T6 t6)
            {
                return this.FromT6(t6);
            }
            else
            {
                string allowedTypes = string.Join(
                        ", ",
                        new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) }.Select(t => t.Name));
                throw new ArgumentException(
                        $"Parameter is none of the expected type: {allowedTypes}",
                        nameof(value));
            }
        }
    }
}

/// <summary>
/// Internal implementation of sum type.
/// </summary>
/// <typeparam name="T0">Type of 1st argument.</typeparam>
/// <typeparam name="T1">Type of 2nd argument.</typeparam>
/// <typeparam name="T2">Type of 3th argument.</typeparam>
/// <typeparam name="T3">Type of 4th argument.</typeparam>
/// <typeparam name="T4">Type of 5th argument.</typeparam>
/// <typeparam name="T5">Type of 6th argument.</typeparam>
/// <typeparam name="T6">Type of 7th argument.</typeparam>
/// <typeparam name="T7">Type of 8th argument.</typeparam>
[Experimental("Experimental use now only.")]
public class OneFrom<T0, T1, T2, T3, T4, T5, T6, T7> : OneFrom, IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7>
    where T0 : notnull
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where T4 : notnull
    where T5 : notnull
    where T6 : notnull
    where T7 : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneFrom{T0, T1, T2, T3, T4, T5, T6, T7}"/> class.
    /// </summary>
    /// <param name="typeIndex">Index of value.</param>
    /// <param name="value">Value.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    internal OneFrom(byte typeIndex, object value)
        : base(typeIndex, value)
    {
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7> FromT0(T0 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7>().FromT0(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7> FromT1(T1 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7>().FromT1(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7> FromT2(T2 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7>().FromT2(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7> FromT3(T3 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7>().FromT3(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7> FromT4(T4 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7>().FromT4(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7> FromT5(T5 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7>().FromT5(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7> FromT6(T6 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7>().FromT6(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7> FromT7(T7 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7>().FromT7(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type by probing its type.
    /// </summary>
    /// <remarks>Naturally, do not use derived types
    /// as the check may produce unexpected results.</remarks>
    /// <param name="value">Value to probe.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
    ///     is none of the expected type.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7> From(object value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7>().From(value);
    }

    /// <summary>
    /// Builder class.
    /// </summary>
    public sealed class Builder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Builder"/> class.
        /// </summary>
        internal Builder()
        {
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7> FromT0(T0 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7>(0, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7> FromT1(T1 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7>(1, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7> FromT2(T2 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7>(2, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7> FromT3(T3 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7>(3, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7> FromT4(T4 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7>(4, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7> FromT5(T5 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7>(5, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7> FromT6(T6 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7>(6, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7> FromT7(T7 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7>(7, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type by probing its type.
        /// </summary>
        /// <remarks>Naturally, do not use derived types
        /// as the check may produce unexpected results.</remarks>
        /// <param name="value">Value to probe.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
        ///     is none of the expected type.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7> From(object value)
        {
            Ensure.Param(value).Done();

            if (value is T0 t0)
            {
                return this.FromT0(t0);
            }
            else if (value is T1 t1)
            {
                return this.FromT1(t1);
            }
            else if (value is T2 t2)
            {
                return this.FromT2(t2);
            }
            else if (value is T3 t3)
            {
                return this.FromT3(t3);
            }
            else if (value is T4 t4)
            {
                return this.FromT4(t4);
            }
            else if (value is T5 t5)
            {
                return this.FromT5(t5);
            }
            else if (value is T6 t6)
            {
                return this.FromT6(t6);
            }
            else if (value is T7 t7)
            {
                return this.FromT7(t7);
            }
            else
            {
                string allowedTypes = string.Join(
                        ", ",
                        new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) }.Select(t => t.Name));
                throw new ArgumentException(
                        $"Parameter is none of the expected type: {allowedTypes}",
                        nameof(value));
            }
        }
    }
}

/// <summary>
/// Internal implementation of sum type.
/// </summary>
/// <typeparam name="T0">Type of 1st argument.</typeparam>
/// <typeparam name="T1">Type of 2nd argument.</typeparam>
/// <typeparam name="T2">Type of 3th argument.</typeparam>
/// <typeparam name="T3">Type of 4th argument.</typeparam>
/// <typeparam name="T4">Type of 5th argument.</typeparam>
/// <typeparam name="T5">Type of 6th argument.</typeparam>
/// <typeparam name="T6">Type of 7th argument.</typeparam>
/// <typeparam name="T7">Type of 8th argument.</typeparam>
/// <typeparam name="T8">Type of 9th argument.</typeparam>
[Experimental("Experimental use now only.")]
public class OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> : OneFrom, IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8>
    where T0 : notnull
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where T4 : notnull
    where T5 : notnull
    where T6 : notnull
    where T7 : notnull
    where T8 : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneFrom{T0, T1, T2, T3, T4, T5, T6, T7, T8}"/> class.
    /// </summary>
    /// <param name="typeIndex">Index of value.</param>
    /// <param name="value">Value.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    internal OneFrom(byte typeIndex, object value)
        : base(typeIndex, value)
    {
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> FromT0(T0 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8>().FromT0(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> FromT1(T1 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8>().FromT1(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> FromT2(T2 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8>().FromT2(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> FromT3(T3 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8>().FromT3(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> FromT4(T4 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8>().FromT4(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> FromT5(T5 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8>().FromT5(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> FromT6(T6 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8>().FromT6(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> FromT7(T7 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8>().FromT7(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> FromT8(T8 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8>().FromT8(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type by probing its type.
    /// </summary>
    /// <remarks>Naturally, do not use derived types
    /// as the check may produce unexpected results.</remarks>
    /// <param name="value">Value to probe.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
    ///     is none of the expected type.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> From(object value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8>().From(value);
    }

    /// <summary>
    /// Builder class.
    /// </summary>
    public sealed class Builder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Builder"/> class.
        /// </summary>
        internal Builder()
        {
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> FromT0(T0 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8>(0, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> FromT1(T1 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8>(1, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> FromT2(T2 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8>(2, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> FromT3(T3 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8>(3, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> FromT4(T4 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8>(4, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> FromT5(T5 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8>(5, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> FromT6(T6 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8>(6, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> FromT7(T7 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8>(7, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> FromT8(T8 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8>(8, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type by probing its type.
        /// </summary>
        /// <remarks>Naturally, do not use derived types
        /// as the check may produce unexpected results.</remarks>
        /// <param name="value">Value to probe.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
        ///     is none of the expected type.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8> From(object value)
        {
            Ensure.Param(value).Done();

            if (value is T0 t0)
            {
                return this.FromT0(t0);
            }
            else if (value is T1 t1)
            {
                return this.FromT1(t1);
            }
            else if (value is T2 t2)
            {
                return this.FromT2(t2);
            }
            else if (value is T3 t3)
            {
                return this.FromT3(t3);
            }
            else if (value is T4 t4)
            {
                return this.FromT4(t4);
            }
            else if (value is T5 t5)
            {
                return this.FromT5(t5);
            }
            else if (value is T6 t6)
            {
                return this.FromT6(t6);
            }
            else if (value is T7 t7)
            {
                return this.FromT7(t7);
            }
            else if (value is T8 t8)
            {
                return this.FromT8(t8);
            }
            else
            {
                string allowedTypes = string.Join(
                        ", ",
                        new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) }.Select(t => t.Name));
                throw new ArgumentException(
                        $"Parameter is none of the expected type: {allowedTypes}",
                        nameof(value));
            }
        }
    }
}

/// <summary>
/// Internal implementation of sum type.
/// </summary>
/// <typeparam name="T0">Type of 1st argument.</typeparam>
/// <typeparam name="T1">Type of 2nd argument.</typeparam>
/// <typeparam name="T2">Type of 3th argument.</typeparam>
/// <typeparam name="T3">Type of 4th argument.</typeparam>
/// <typeparam name="T4">Type of 5th argument.</typeparam>
/// <typeparam name="T5">Type of 6th argument.</typeparam>
/// <typeparam name="T6">Type of 7th argument.</typeparam>
/// <typeparam name="T7">Type of 8th argument.</typeparam>
/// <typeparam name="T8">Type of 9th argument.</typeparam>
/// <typeparam name="T9">Type of 10th argument.</typeparam>
[Experimental("Experimental use now only.")]
public class OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> : OneFrom, IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>
    where T0 : notnull
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where T4 : notnull
    where T5 : notnull
    where T6 : notnull
    where T7 : notnull
    where T8 : notnull
    where T9 : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneFrom{T0, T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> class.
    /// </summary>
    /// <param name="typeIndex">Index of value.</param>
    /// <param name="value">Value.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    internal OneFrom(byte typeIndex, object value)
        : base(typeIndex, value)
    {
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> FromT0(T0 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>().FromT0(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> FromT1(T1 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>().FromT1(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> FromT2(T2 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>().FromT2(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> FromT3(T3 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>().FromT3(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> FromT4(T4 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>().FromT4(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> FromT5(T5 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>().FromT5(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> FromT6(T6 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>().FromT6(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> FromT7(T7 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>().FromT7(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> FromT8(T8 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>().FromT8(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> FromT9(T9 value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>().FromT9(value);
    }

    /// <summary>
    /// Convert <paramref name="value"/>
    /// to sum type by probing its type.
    /// </summary>
    /// <remarks>Naturally, do not use derived types
    /// as the check may produce unexpected results.</remarks>
    /// <param name="value">Value to probe.</param>
    /// <returns>Sum type.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
    ///     is none of the expected type.</exception>
    public static IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> From(object value)
    {
        return CreateBuilder<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>().From(value);
    }

    /// <summary>
    /// Builder class.
    /// </summary>
    public sealed class Builder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Builder"/> class.
        /// </summary>
        internal Builder()
        {
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> FromT0(T0 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(0, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> FromT1(T1 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(1, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> FromT2(T2 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(2, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> FromT3(T3 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(3, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> FromT4(T4 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(4, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> FromT5(T5 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(5, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> FromT6(T6 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(6, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> FromT7(T7 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(7, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> FromT8(T8 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(8, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type.
        /// </summary>
        /// <param name="value">Value to use.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> FromT9(T9 value)
        {
            return new OneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(9, value);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// to sum type by probing its type.
        /// </summary>
        /// <remarks>Naturally, do not use derived types
        /// as the check may produce unexpected results.</remarks>
        /// <param name="value">Value to probe.</param>
        /// <returns>Sum type.</returns>
        /// <exception cref="ArgumentNullException">Thrown
        ///     if required parameter is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
        ///     is none of the expected type.</exception>
        public IOneFrom<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> From(object value)
        {
            Ensure.Param(value).Done();

            if (value is T0 t0)
            {
                return this.FromT0(t0);
            }
            else if (value is T1 t1)
            {
                return this.FromT1(t1);
            }
            else if (value is T2 t2)
            {
                return this.FromT2(t2);
            }
            else if (value is T3 t3)
            {
                return this.FromT3(t3);
            }
            else if (value is T4 t4)
            {
                return this.FromT4(t4);
            }
            else if (value is T5 t5)
            {
                return this.FromT5(t5);
            }
            else if (value is T6 t6)
            {
                return this.FromT6(t6);
            }
            else if (value is T7 t7)
            {
                return this.FromT7(t7);
            }
            else if (value is T8 t8)
            {
                return this.FromT8(t8);
            }
            else if (value is T9 t9)
            {
                return this.FromT9(t9);
            }
            else
            {
                string allowedTypes = string.Join(
                        ", ",
                        new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) }.Select(t => t.Name));
                throw new ArgumentException(
                        $"Parameter is none of the expected type: {allowedTypes}",
                        nameof(value));
            }
        }
    }
}

#pragma warning restore CA1000 // Do not declare static members
#pragma warning restore SA1402 // File may only contain a single typeon generic types
