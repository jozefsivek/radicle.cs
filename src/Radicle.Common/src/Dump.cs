namespace Radicle.Common;

/// <summary>
/// Collection of (text) dump related utilities akin to those in Python.
/// </summary>
public static class Dump
{
    /// <summary>
    /// Returns string representation of the range
    /// in form: '['|'(' + lowerBound.ToString() + ', ' + upperBound.ToString() + ')'|']',
    /// where '[' or ']' denotes inclusion of boundaries and
    /// '(' or ')' exclusion of boundaries. The <see langword="null"/>
    /// values are represented as '?'.
    /// </summary>
    /// <param name="lowerBound">Lowed bound.</param>
    /// <param name="upperBound">Upper bound.</param>
    /// <param name="includeLower">Lower bound is included.</param>
    /// <param name="includeUpper">Upper bound is included.</param>
    /// <returns>String representation of the range.</returns>
    public static string Range(
            object? lowerBound,
            object? upperBound,
            bool includeLower = true,
            bool includeUpper = true)
    {
        string? bra = includeLower ? "[" : "(";
        string? ket = includeUpper ? "]" : ")";
        string? low = lowerBound?.ToString() ?? "?";
        string? up = upperBound?.ToString() ?? "?";

        return $"{bra}{low}, {up}{ket}";
    }
}
