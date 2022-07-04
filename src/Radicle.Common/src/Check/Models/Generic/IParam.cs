namespace Radicle.Common.Check.Models.Generic;

using System;
using Radicle.Common.Extensions;

/// <summary>
/// Interface of generic method parameter.
/// </summary>
/// <typeparam name="T">Type of the parameter value.</typeparam>
public interface IParam<T> : IParam
        where T : notnull
{
    /// <summary>
    /// Gets value of the parameter or fails if value
    /// is missing in optional parameter.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown
    ///     if <see cref="IParam.IsSpecified"/>
    ///     is <see langword="true"/>.</exception>
    T Value
    {
        get
        {
            if (this.InnerParam.IsSpecified)
            {
                return this.InnerParam.Value;
            }

            throw new InvalidOperationException(
                    "Can not retrieve value from unspecified parameter.");
        }
    }

    /// <summary>
    /// Gets the dectription of the parameter reflecting
    /// <see cref="IParam.Name"/>, <see cref="Value"/>.
    /// </summary>
    string DescriptionWithValue
    {
        get
        {
            string val = this.Value is string s
                    ? "'" + s.Ellipsis() + "'"
                    : this.Value.ToString();

            return $"Parameter '{this.Name}' with value: {val}";
        }
    }

    /// <summary>
    /// Gets inner parameter.
    /// </summary>
    new IParam<T> InnerParam { get; }

    /// <inheritdoc/>
    IParam IParam.InnerParam => this.InnerParam;

    /// <summary>
    /// General evaluation of validity according to given
    /// <paramref name="predicate"/>, if the <paramref name="predicate"/>
    /// returns <see langword="false"/> the parameter value
    /// is treated as invalid and exception is thrown.
    /// Whenever possible, use more specific guards.
    /// </summary>
    /// <param name="predicate">Predicate function which
    ///     determines if the parameter value is valid,
    ///     i.e. <paramref name="predicate"/> returns
    ///     <see langword="true"/>.</param>
    /// <param name="messageFactory">Optional message factory
    ///     which is used to construct message of <see cref="ArgumentException"/>
    ///     thrown if <paramref name="predicate"/> returns
    ///     <see langword="false"/>.</param>
    /// <returns>This instance of <see cref="Param{TValue}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if
    ///     <paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="predicate"/>
    ///     returns <see langword="false"/>.</exception>
    IParam<T> That(
            Func<T, bool> predicate,
            Func<IParam<T>, string>? messageFactory = null)
    {
        Ensure.NotNull(predicate);

        _ = this.InnerParam.That(
                predicate,
                messageFactory: messageFactory);

        return this;
    }

    /// <summary>
    /// Return value of this parameter or
    /// default value for the type if this argument
    /// was optional and lacks value. May not work
    /// as you expect for value types so avoid use for those.
    /// </summary>
    /// <returns><see cref="Value"/> if present,
    ///     default value for the type otherwise.</returns>
    T? ValueOrDefault()
    {
        if (this.IsSpecified)
        {
            return this.Value;
        }

        return default;
    }

    /// <summary>
    /// Return value of this parameter or
    /// <paramref name="defaultValue"/> if this argument
    /// was optional and lacks value.
    /// </summary>
    /// <param name="defaultValue">default value to return
    ///     if <see cref="Value"/> was not specified.</param>
    /// <returns><see cref="Value"/> if present,
    ///     <paramref name="defaultValue"/> otherwise.</returns>
    T ValueOr(T defaultValue)
    {
        if (this.IsSpecified)
        {
            return this.Value;
        }

        return defaultValue;
    }

    /// <summary>
    /// Return value of this parameter or value
    /// from <paramref name="defaultValueFactory"/> if this argument
    /// was optional and lacks value.
    /// </summary>
    /// <param name="defaultValueFactory">default value factory to return
    ///     value if <see cref="Value"/> was not specified.</param>
    /// <returns><see cref="Value"/> if present,
    ///     result of <paramref name="defaultValueFactory"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown if
    ///     <paramref name="defaultValueFactory"/> is <see langword="null"/>.</exception>
    T ValueOr(Func<T> defaultValueFactory)
    {
        Ensure.NotNull(defaultValueFactory);

        if (this.IsSpecified)
        {
            return this.Value;
        }

        return defaultValueFactory();
    }
}
