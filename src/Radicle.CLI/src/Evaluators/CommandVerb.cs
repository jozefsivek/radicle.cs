namespace Radicle.CLI.Evaluators;

using System;
using Radicle.Common.Check;
using Radicle.Common.Check.Base;

/// <summary>
/// Immutable representation of the command verb.
/// </summary>
public sealed class CommandVerb : TypedName<CommandVerb>
{
    /// <summary>
    /// Specification.
    /// </summary>
    public static readonly TypedNameSpec Specification = new(
            "Command evaluator verb",
            @"\A[a-z]+\z",
            ignoreCaseInPattern: true,
            ignoreCaseWhenCompared: true,
            description: "alpha characters",
            maxLength: 32);

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandVerb"/> class.
    /// </summary>
    /// <param name="name">Case insensitive command verb,
    ///     with maximum length of 42 characters.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the
    ///     input exceeds allowed ranges.</exception>
    /// <exception cref="ArgumentException">Thrown if the input does not
    ///     conform to expected form.</exception>
    public CommandVerb(string name)
        : base(name)
    {
    }

    /// <inheritdoc />
    public override TypedNameSpec Spec => Specification;

    /// <summary>
    /// Gets documentation name version which is capitalized.
    /// </summary>
    public string DocName => this.Value.ToUpperInvariant();

    /// <inheritdoc />
    public override string ToString()
    {
        return $"command evaluator verb {base.ToString()}";
    }
}
