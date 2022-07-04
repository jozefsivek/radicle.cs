namespace Radicle.Common.Check.Base;

internal class ConcreteCaseInsensitiveName : TypedName<ConcreteCaseInsensitiveName>
{
    public ConcreteCaseInsensitiveName(string name)
        : base(name)
    {
    }

    public static TypedNameSpec Specification { get; } = TypedNameSpec.Programming
            .With(ignoreCaseWhenCompared: true);

    public override TypedNameSpec Spec { get; } = Specification;

    public override string ToString()
    {
        return $"Test {base.ToString()}";
    }
}
