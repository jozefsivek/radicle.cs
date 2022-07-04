namespace Radicle.Common.Check.Models.Generic;

using System;

/// <summary>
/// Structure of the Typed parameter.
/// </summary>
/// <typeparam name="T">Type of the parameter value.</typeparam>
internal readonly struct Param<T> : IParam<T>
    where T : notnull
{
    private readonly T valueValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param{TValue}"/> struct.
    /// </summary>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="name">Name of the parameter if any, defaults to "n/a".</param>
    /// <exception cref="ArgumentNullException">Thrown when the specified
    ///     <paramref name="value"/> is <see langword="null"/>.</exception>
    public Param(
            T value,
            string? name = null)
    {
        Ensure.NotNull(value, parameterName: name ?? "n/a");

        this.Name = name ?? "n/a";
        this.valueValue = value;
        this.IsSpecified = true;
    }

    /// <inheritdoc/>
    public string? Name { get; }

    /// <inheritdoc/>
    public T Value
    {
        get
        {
            if (this.IsSpecified)
            {
                return this.valueValue;
            }

            throw new InvalidOperationException(
                    "Can not retrieve value from unspecified parameter.");
        }
    }

    /// <inheritdoc/>
    public bool IsSpecified { get; }

    /// <inheritdoc/>
    public IParam<T> InnerParam => default(Param<T>);

    /// <summary>
    /// Static general evaluation of validity according to given
    /// <paramref name="predicate"/>, if the <paramref name="predicate"/>
    /// returns <see langword="false"/> the parameter value
    /// is treated as invalid and exception is thrown.
    /// Whenever possible, use more specific guard function.
    /// </summary>
    /// <param name="self">Self parameter.</param>
    /// <param name="predicate">Predicate function which
    ///     determines if the parameter value is valid,
    ///     i.e. <paramref name="predicate"/> returns
    ///     <see langword="true"/>.</param>
    /// <param name="messageFactory">Optional message factory
    ///     which is used to construct message of <see cref="ArgumentException"/>
    ///     thrown if <paramref name="predicate"/> returns
    ///     <see langword="false"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown if
    ///     <paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="predicate"/>
    ///     returns <see langword="false"/>.</exception>
    public static void StaticThat(
            IParam<T> self,
            Func<T, bool> predicate,
            Func<IParam<T>, string>? messageFactory = null)
    {
        Ensure.NotNull(predicate);

        if (self.IsSpecified && !predicate(self.Value))
        {
            string msg;

            if (messageFactory is null)
            {
                string description = self.DescriptionWithValue;
                msg = $"{description} is not valid.";
            }
            else
            {
                msg = messageFactory(self);
            }

            throw new ArgumentException(msg, self.Name);
        }
    }

    /// <inheritdoc/>
    public IParam<T> That(
            Func<T, bool> predicate,
            Func<IParam<T>, string>? messageFactory = null)
    {
        Param<T>.StaticThat(this, predicate, messageFactory: messageFactory);

        return this;
    }
}
