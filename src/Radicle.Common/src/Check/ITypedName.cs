namespace Radicle.Common.Check;

/// <summary>
/// Typed immutable string name.
/// </summary>
public interface ITypedName
{
    /// <summary>
    /// Gets returns specification of this
    /// identifier name.
    /// </summary>
    /// <returns>Specification.</returns>
    TypedNameSpec Spec { get; }

    /// <summary>
    /// Gets valid value of the name
    /// which was used to create this instance.
    /// For long term storage purposes use
    /// <see cref="InvariantValue"/>.
    /// </summary>
    string Value { get; }

    /// <summary>
    /// Gets valid value of the name which
    /// was adjusted, i.e. lowercased for case
    /// insensitive names (see
    /// <see cref="TypedNameSpec.IgnoreCaseWhenCompared"/>).
    /// Use for long term storage purpose.
    /// </summary>
    /// <remarks>This value benefits storage
    /// when value needs to be compared
    /// and thus case sensitivity works as expected.</remarks>
    string InvariantValue { get; }
}
