namespace Radicle.CLI.Evaluators;

using System;
using Radicle.Common.Check;
using Radicle.Common.Check.Base;

/// <summary>
/// Immutable command group name.
/// </summary>
public sealed class CommandGroupName : TypedName<CommandGroupName>
{
    /// <summary>
    /// Specification.
    /// </summary>
    public static readonly TypedNameSpec Specification = TypedNameSpec.Programming
            .With(maxLength: 32, ignoreCaseWhenCompared: true);

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandGroupName"/> class.
    /// </summary>
    /// <param name="name">Case insensitive programming name of the
    ///     command group, with maximum length of 32 characters.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the
    ///     input exceeds allowed ranges.</exception>
    /// <exception cref="ArgumentException">Thrown if the input does not
    ///     conform to expected form.</exception>
    public CommandGroupName(string name)
        : base(name)
    {
    }

    /// <inheritdoc />
    public override TypedNameSpec Spec => Specification;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"command group {base.ToString()}";
    }
}
