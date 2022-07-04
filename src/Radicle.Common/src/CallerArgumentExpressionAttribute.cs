#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.Runtime.CompilerServices;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Remove ones in dotnet 6.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class CallerArgumentExpressionAttribute : Attribute
{
#pragma warning disable SA1600 // Elements should be documented
    public CallerArgumentExpressionAttribute(string parameterName)
    {
        this.ParameterName = parameterName;
    }

    public string ParameterName { get; }
#pragma warning restore SA1600 // Elements should be documented
}
