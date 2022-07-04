namespace Radicle.Common.Check.Base;

internal class ConcreteLowerCaseName : TypedName<ConcreteLowerCaseName>
{
    public ConcreteLowerCaseName(string name)
        : base(name)
    {
    }

    public static TypedNameSpec Specification { get; } = TypedNameSpec.Programming
            .With(ignoreCaseInPattern: false);

    public override TypedNameSpec Spec { get; } = Specification;

    public override string ToString()
    {
        return $"Test {base.ToString()}";
    }
}
