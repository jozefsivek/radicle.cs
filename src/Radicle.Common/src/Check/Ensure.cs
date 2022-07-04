namespace Radicle.Common.Check;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Radicle.Common.Check.Models;
using Radicle.Common.Check.Models.Generic;

/// <summary>
/// Collection of parameter ensure methods.
/// </summary>
public static class Ensure
{
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

    /// <summary>
    /// Ensure typed parameter.
    /// </summary>
    /// <param name="parameterValue">Value of the parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns>Instance of <see cref="IParam{TValue}"/>
    ///     which can be further restricted.</returns>
    /// <typeparam name="T">Type of the parameter value.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown when the specified
    ///     <paramref name="parameterValue"/> is <see langword="null"/>.</exception>
    public static IParam<T> Param<T>(
            T parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return new Param<T>(parameterValue, name: parameterName);
    }

    /// <summary>
    /// Ensure typed optional parameter.
    /// </summary>
    /// <param name="parameterValue">Value of the parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns>Instance of <see cref="IParam{TValue}"/>
    ///     which can be further restricted.</returns>
    /// <typeparam name="T">Type of the parameter value.</typeparam>
    public static IParam<T> Optional<T>(
            T? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
        where T : class
    {
        if (parameterValue is null)
        {
            return default(Param<T>);
        }

        return Param(parameterValue, parameterName: parameterName);
    }

    /// <summary>
    /// Ensure typed optional parameter.
    /// </summary>
    /// <param name="parameterValue">Value of the parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns>Instance of <see cref="IParam{TValue}"/>
    ///     which can be further restricted.</returns>
    /// <typeparam name="T">Type of the parameter value.</typeparam>
    public static IParam<T> Optional<T>(
            T? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
        where T : struct
    {
        if (parameterValue is null)
        {
            return default(Param<T>);
        }

        return Param(parameterValue.Value, parameterName: parameterName);
    }

#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

    /// <summary>
    /// Ensure string parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="IStringParam"/>
    ///     which can be further restricted.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the specified
    ///     <paramref name="parameterValue"/> is <see langword="null"/>.</exception>
    public static IStringParam Param(
            string parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return new StringParam(new Param<string>(parameterValue, name: parameterName));
    }

    /// <summary>
    /// Ensure optional string parameter.
    /// </summary>
    /// <param name="parameterValue">Value of the parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns>Instance of <see cref="IStringParam"/>
    ///     which can be further restricted.</returns>
    public static IStringParam Optional(
            string? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return new StringParam(Optional<string>(parameterValue, parameterName: parameterName));
    }

    /// <summary>
    /// Ensure culture info parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="ICultureInfoParam"/>
    ///     which can be further restricted.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the specified
    ///     <paramref name="parameterValue"/> is <see langword="null"/>.</exception>
    public static ICultureInfoParam Param(
            CultureInfo parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return new CultureInfoParam(
                new Param<CultureInfo>(parameterValue, name: parameterName));
    }

    /// <summary>
    /// Ensure optional string parameter.
    /// </summary>
    /// <param name="parameterValue">Value of the parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns>Instance of <see cref="ICultureInfoParam"/>
    ///     which can be further restricted.</returns>
    public static ICultureInfoParam Optional(
            CultureInfo? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return new CultureInfoParam(
                Optional<CultureInfo>(parameterValue, parameterName: parameterName));
    }

    /// <summary>
    /// Ensure numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<long> Param(
            long parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(new Param<long>(parameterValue, name: parameterName));
    }

    /// <summary>
    /// Ensure optional numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<long> Optional(
            long? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                Optional<long>(parameterValue, parameterName: parameterName));
    }

    /// <summary>
    /// Ensure numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<ulong> Param(
            ulong parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                new Param<ulong>(parameterValue, name: parameterName));
    }

    /// <summary>
    /// Ensure optional numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<ulong> Optional(
            ulong? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                Optional<ulong>(parameterValue, parameterName: parameterName));
    }

    /// <summary>
    /// Ensure numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<int> Param(
            int parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                new Param<int>(parameterValue, name: parameterName));
    }

    /// <summary>
    /// Ensure optional numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<int> Optional(
            int? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                Optional<int>(parameterValue, parameterName: parameterName));
    }

    /// <summary>
    /// Ensure numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<uint> Param(
            uint parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                new Param<uint>(parameterValue, name: parameterName));
    }

    /// <summary>
    /// Ensure optional numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<uint> Optional(
            uint? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                Optional<uint>(parameterValue, parameterName: parameterName));
    }

    /// <summary>
    /// Ensure numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<short> Param(
            short parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(new Param<short>(parameterValue, name: parameterName));
    }

    /// <summary>
    /// Ensure optional numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<short> Optional(
            short? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                Optional<short>(parameterValue, parameterName: parameterName));
    }

    /// <summary>
    /// Ensure numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<ushort> Param(
            ushort parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                new Param<ushort>(parameterValue, name: parameterName));
    }

    /// <summary>
    /// Ensure optional numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<ushort> Optional(
            ushort? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                Optional<ushort>(parameterValue, parameterName: parameterName));
    }

    /// <summary>
    /// Ensure numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<byte> Param(
            byte parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                new Param<byte>(parameterValue, name: parameterName));
    }

    /// <summary>
    /// Ensure optional numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<byte> Optional(
            byte? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                Optional<byte>(parameterValue, parameterName: parameterName));
    }

    /// <summary>
    /// Ensure numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<sbyte> Param(
            sbyte parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                new Param<sbyte>(parameterValue, name: parameterName));
    }

    /// <summary>
    /// Ensure optional numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<sbyte> Optional(
            sbyte? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                Optional<sbyte>(parameterValue, parameterName: parameterName));
    }

    /// <summary>
    /// Ensure numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<decimal> Param(
            decimal parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                new Param<decimal>(parameterValue, name: parameterName));
    }

    /// <summary>
    /// Ensure optional numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<decimal> Optional(
            decimal? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                Optional<decimal>(parameterValue, parameterName: parameterName));
    }

    /// <summary>
    /// Ensure numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<double> Param(
            double parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                new Param<double>(parameterValue, name: parameterName));
    }

    /// <summary>
    /// Ensure optional numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<double> Optional(
            double? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                Optional<double>(parameterValue, parameterName: parameterName));
    }

    /// <summary>
    /// Ensure numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<float> Param(
            float parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                new Param<float>(parameterValue, name: parameterName));
    }

    /// <summary>
    /// Ensure optional numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<float> Optional(
            float? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                Optional<float>(parameterValue, parameterName: parameterName));
    }

    /// <summary>
    /// Ensure numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<TimeSpan> Param(
            TimeSpan parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                new Param<TimeSpan>(parameterValue, name: parameterName));
    }

    /// <summary>
    /// Ensure optional numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<TimeSpan> Optional(
            TimeSpan? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                Optional<TimeSpan>(parameterValue, parameterName: parameterName));
    }

    /// <summary>
    /// Ensure numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<DateTime> Param(
            DateTime parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                new Param<DateTime>(parameterValue, name: parameterName));
    }

    /// <summary>
    /// Ensure optional numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<DateTime> Optional(
            DateTime? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                Optional<DateTime>(parameterValue, parameterName: parameterName));
    }

    /// <summary>
    /// Ensure numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<DateTimeOffset> Param(
            DateTimeOffset parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(new Param<DateTimeOffset>(parameterValue, name: parameterName));
    }

    /// <summary>
    /// Ensure optional numeric parameter.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>Instance of <see cref="INumberLikeParam{TValue}"/>
    ///     which can be further restricted.</returns>
    public static INumberLikeParam<DateTimeOffset> Optional(
            DateTimeOffset? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateNumeric(
                Optional<DateTimeOffset>(parameterValue, parameterName));
    }

    /// <summary>
    /// Ensure typed collection parameter.
    /// </summary>
    /// <param name="parameterValue">Value of the parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns>Instance of <see cref="IParam{TValue}"/>
    ///     which can be further restricted.</returns>
    /// <typeparam name="TItem">Type of the parameter item.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown when the specified
    ///     <paramref name="parameterValue"/> is <see langword="null"/>.</exception>
    public static ICollectionParam<IEnumerable<TItem>, TItem> Collection<TItem>(
            IEnumerable<TItem> parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateCollection(parameterValue, parameterName: parameterName);
    }

    /// <summary>
    /// Ensure typed optional collection parameter.
    /// </summary>
    /// <param name="parameterValue">Value of the parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns>Instance of <see cref="IParam{TValue}"/>
    ///     which can be further restricted.</returns>
    /// <typeparam name="TItem">Type of the parameter item.</typeparam>
    public static ICollectionParam<IEnumerable<TItem>, TItem> OptionalCollection<TItem>(
            IEnumerable<TItem>? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        if (parameterValue is null)
        {
            return default(CollectionParam<IEnumerable<TItem>, TItem>);
        }

        return Collection(parameterValue, parameterName: parameterName);
    }

    /// <summary>
    /// Ensure typed collection parameter.
    /// </summary>
    /// <param name="parameterValue">Value of the parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns>Instance of <see cref="IParam{TValue}"/>
    ///     which can be further restricted.</returns>
    /// <typeparam name="TValue">Type of the parameter value item.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown when the specified
    ///     <paramref name="parameterValue"/> is <see langword="null"/>.</exception>
    public static ICollectionParam<IEnumerable<TValue>, TValue> Param<TValue>(
            IEnumerable<TValue> parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return Collection(parameterValue, parameterName: parameterName);
    }

    /// <summary>
    /// Ensure typed optional collection parameter.
    /// </summary>
    /// <param name="parameterValue">Value of the parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns>Instance of <see cref="IParam{TValue}"/>
    ///     which can be further restricted.</returns>
    /// <typeparam name="TValue">Type of the parameter value item.</typeparam>
    public static ICollectionParam<IEnumerable<TValue>, TValue> Optional<TValue>(
            IEnumerable<TValue>? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return OptionalCollection(parameterValue, parameterName: parameterName);
    }

    /// <summary>
    /// Ensure typed collection parameter.
    /// </summary>
    /// <param name="parameterValue">Value of the parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns>Instance of <see cref="IParam{TValue}"/>
    ///     which can be further restricted.</returns>
    /// <typeparam name="TValue">Type of the parameter value item.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown when the specified
    ///     <paramref name="parameterValue"/> is <see langword="null"/>.</exception>
    public static ICollectionParam<IEnumerable<TValue>, TValue> Param<TValue>(
            TValue[] parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return Collection(parameterValue, parameterName: parameterName);
    }

    /// <summary>
    /// Ensure typed collection parameter.
    /// </summary>
    /// <param name="parameterValue">Value of the parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns>Instance of <see cref="IParam{TValue}"/>
    ///     which can be further restricted.</returns>
    /// <typeparam name="TItem">Type of the parameter item value.</typeparam>
    public static ICollectionParam<IEnumerable<TItem>, TItem> Optional<TItem>(
            TItem[]? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return OptionalCollection(parameterValue, parameterName: parameterName);
    }

    /// <summary>
    /// Ensure typed dictionary parameter.
    /// </summary>
    /// <param name="parameterValue">Value of the parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns>Instance of <see cref="IParam{TValue}"/>
    ///     which can be further restricted.</returns>
    /// <typeparam name="TVK">Type of the parameter item key.</typeparam>
    /// <typeparam name="TVV">Type of the parameter item value.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown when the specified
    ///     <paramref name="parameterValue"/> is <see langword="null"/>.</exception>
    public static IDictionaryParam<IEnumerable<KeyValuePair<TVK, TVV>>, TVK, TVV> Dictionary<TVK, TVV>(
            IEnumerable<KeyValuePair<TVK, TVV>> parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return CreateDictionary(parameterValue, parameterName: parameterName);
    }

    /// <summary>
    /// Ensure typed dictionary parameter.
    /// </summary>
    /// <param name="parameterValue">Value of the parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns>Instance of <see cref="IParam{TValue}"/>
    ///     which can be further restricted.</returns>
    /// <typeparam name="TVK">Type of the parameter item key.</typeparam>
    /// <typeparam name="TVV">Type of the parameter item value.</typeparam>
    public static IDictionaryParam<IEnumerable<KeyValuePair<TVK, TVV>>, TVK, TVV> OptionalDictionary<TVK, TVV>(
            IEnumerable<KeyValuePair<TVK, TVV>>? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        if (parameterValue is null)
        {
            return default(DictionaryParam<IEnumerable<KeyValuePair<TVK, TVV>>, TVK, TVV>);
        }

        return Dictionary(parameterValue, parameterName: parameterName);
    }

    /// <summary>
    /// Ensure typed dictionary parameter.
    /// </summary>
    /// <param name="parameterValue">Value of the parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns>Instance of <see cref="IParam{TValue}"/>
    ///     which can be further restricted.</returns>
    /// <typeparam name="TVK">Type of the parameter item key.</typeparam>
    /// <typeparam name="TVV">Type of the parameter item value.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown when the specified
    ///     <paramref name="parameterValue"/> is <see langword="null"/>.</exception>
    public static IDictionaryParam<IEnumerable<KeyValuePair<TVK, TVV>>, TVK, TVV> Param<TVK, TVV>(
            IEnumerable<KeyValuePair<TVK, TVV>> parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return Dictionary(parameterValue, parameterName: parameterName);
    }

    /// <summary>
    /// Ensure typed dictionary parameter.
    /// </summary>
    /// <param name="parameterValue">Value of the parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns>Instance of <see cref="IParam{TValue}"/>
    ///     which can be further restricted.</returns>
    /// <typeparam name="TVK">Type of the parameter item key.</typeparam>
    /// <typeparam name="TVV">Type of the parameter item value.</typeparam>
    public static IDictionaryParam<IEnumerable<KeyValuePair<TVK, TVV>>, TVK, TVV> Optional<TVK, TVV>(
            IEnumerable<KeyValuePair<TVK, TVV>>? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        return OptionalDictionary(parameterValue, parameterName: parameterName);
    }

    /// <summary>
    /// Throws if the specified <paramref name="parameterValue"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <exception cref="ArgumentNullException">Thrown when the specified
    ///     <paramref name="parameterValue"/> is <see langword="null"/>.</exception>
    internal static void NotNull(
            object? parameterValue,
            [CallerArgumentExpression("parameterValue")] string? parameterName = null)
    {
        if (parameterValue is null)
        {
            throw new ArgumentNullException(
                    parameterName ?? "n/a", // stupid order
                    $"Parameter '{parameterName}' cannot be null.");
        }
    }

    private static NumberLikeParam<long> CreateNumeric(IParam<long> inneParam)
    {
        return new NumberLikeParam<long>(
                inneParam,
                Comparer<long>.Default,
                0L,
                _ => true,
                _ => true);
    }

    private static NumberLikeParam<ulong> CreateNumeric(IParam<ulong> innerParam)
    {
        return new NumberLikeParam<ulong>(
                innerParam,
                Comparer<ulong>.Default,
                0uL,
                _ => true,
                _ => true);
    }

    private static NumberLikeParam<int> CreateNumeric(IParam<int> innerParam)
    {
        return new NumberLikeParam<int>(
                innerParam,
                Comparer<int>.Default,
                0,
                _ => true,
                _ => true);
    }

    private static NumberLikeParam<uint> CreateNumeric(IParam<uint> innerParam)
    {
        return new NumberLikeParam<uint>(
                innerParam,
                Comparer<uint>.Default,
                0u,
                _ => true,
                _ => true);
    }

    private static NumberLikeParam<short> CreateNumeric(IParam<short> innerParam)
    {
        return new NumberLikeParam<short>(
                innerParam,
                Comparer<short>.Default,
                0,
                _ => true,
                _ => true);
    }

    private static NumberLikeParam<ushort> CreateNumeric(IParam<ushort> innerParam)
    {
        return new NumberLikeParam<ushort>(
                innerParam,
                Comparer<ushort>.Default,
                0,
                _ => true,
                _ => true);
    }

    private static NumberLikeParam<sbyte> CreateNumeric(IParam<sbyte> innerParam)
    {
        return new NumberLikeParam<sbyte>(
                innerParam,
                Comparer<sbyte>.Default,
                0,
                _ => true,
                _ => true);
    }

    private static NumberLikeParam<byte> CreateNumeric(IParam<byte> innerParam)
    {
        return new NumberLikeParam<byte>(
                innerParam,
                Comparer<byte>.Default,
                0,
                _ => true,
                _ => true);
    }

    private static NumberLikeParam<decimal> CreateNumeric(IParam<decimal> innerParam)
    {
        return new NumberLikeParam<decimal>(
                innerParam,
                Comparer<decimal>.Default,
                decimal.Zero,
                _ => true,
                _ => true);
    }

    private static NumberLikeParam<double> CreateNumeric(IParam<double> innerParam)
    {
        return new NumberLikeParam<double>(
                innerParam,
                Comparer<double>.Default,
                0.0d,
                d => !double.IsNaN(d) && !double.IsInfinity(d),
                _ => true);
    }

    private static NumberLikeParam<float> CreateNumeric(IParam<float> innerParam)
    {
        return new NumberLikeParam<float>(
                innerParam,
                Comparer<float>.Default,
                0f,
                f => !float.IsNaN(f) && !float.IsInfinity(f),
                _ => true);
    }

    private static NumberLikeParam<TimeSpan> CreateNumeric(IParam<TimeSpan> innerParam)
    {
        return new NumberLikeParam<TimeSpan>(
                innerParam,
                Comparer<TimeSpan>.Default,
                TimeSpan.Zero,
                _ => true,
                _ => true);
    }

    private static NumberLikeParam<DateTime> CreateNumeric(IParam<DateTime> innerParam)
    {
        return new NumberLikeParam<DateTime>(
                innerParam,
                Comparer<DateTime>.Default,
                DateTime.UnixEpoch,
                _ => true,
                i => i.Kind == DateTimeKind.Utc);
    }

    private static NumberLikeParam<DateTimeOffset> CreateNumeric(IParam<DateTimeOffset> innerParam)
    {
        return new NumberLikeParam<DateTimeOffset>(
                innerParam,
                Comparer<DateTimeOffset>.Default,
                DateTimeOffset.UnixEpoch,
                _ => true,
                i => i.Offset == TimeSpan.Zero);
    }

    private static ICollectionParam<IEnumerable<TItem>, TItem> CreateCollection<TItem>(
            IEnumerable<TItem> value,
            string? parameterName = null)
    {
        return new CollectionParam<IEnumerable<TItem>, TItem>(
                value,
                name: parameterName);
    }

    private static IDictionaryParam<IEnumerable<KeyValuePair<TVK, TVV>>, TVK, TVV> CreateDictionary<TVK, TVV>(
            IEnumerable<KeyValuePair<TVK, TVV>> value,
            string? parameterName = null)
    {
        return new DictionaryParam<IEnumerable<KeyValuePair<TVK, TVV>>, TVK, TVV>(
                value,
                name: parameterName);
    }
}
