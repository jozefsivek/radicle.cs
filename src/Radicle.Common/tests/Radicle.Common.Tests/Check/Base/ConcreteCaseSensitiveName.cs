namespace Radicle.Common.Check.Base;

internal class ConcreteCaseSensitiveName : TypedName<ConcreteCaseSensitiveName>
{
    public ConcreteCaseSensitiveName(string name)
        : base(name)
    {
    }

    public static TypedNameSpec Specification { get; } = TypedNameSpec.Programming;

    public override TypedNameSpec Spec { get; } = Specification;

    public override string ToString()
    {
        return $"Test {base.ToString()}";
    }
}
