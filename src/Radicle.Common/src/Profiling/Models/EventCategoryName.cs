namespace Radicle.Common.Profiling.Models;

using System;
using Radicle.Common.Check;
using Radicle.Common.Check.Base;

/// <summary>
/// Immutable representation of an event category name.
/// </summary>
public sealed class EventCategoryName : TypedName<EventCategoryName>
{
    /// <summary>
    /// Specification.
    /// </summary>
    public static readonly TypedNameSpec Specification = TypedNameSpec.SingleLine.With(maxLength: 256);

    /// <summary>
    /// Initializes a new instance of the <see cref="EventCategoryName"/> class.
    /// </summary>
    /// <param name="name">Arbitrary name with maximum length of 256 characters.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the
    ///     input exceeds allowed ranges.</exception>
    /// <exception cref="ArgumentException">Thrown if the input does not
    ///     conform to expected form.</exception>
    public EventCategoryName(string name)
        : base(name)
    {
    }

    /// <inheritdoc />
    public override TypedNameSpec Spec => Specification;

    /// <summary>
    /// Implicit conversion.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the
    ///     input exceeds allowed ranges.</exception>
    /// <exception cref="ArgumentException">Thrown if the input does not
    ///     conform to expected form.</exception>
    public static implicit operator EventCategoryName(string value)
    {
        return ToEventCategoryName(value);
    }

    /// <summary>
    /// Conversion.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the
    ///     input exceeds allowed ranges.</exception>
    /// <exception cref="ArgumentException">Thrown if the input does not
    ///     conform to expected form.</exception>
    /// <returns>Instance of <see cref="EventCategoryName"/>.</returns>
    public static EventCategoryName ToEventCategoryName(string value)
    {
        return new EventCategoryName(value);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"event category {base.ToString()}";
    }
}
