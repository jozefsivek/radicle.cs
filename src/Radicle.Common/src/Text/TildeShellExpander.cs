namespace Radicle.Common.Text;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Radicle.Common.Check;
using Radicle.Common.Text.Base;

/// <summary>
/// Tilde expander as in https://tldp.org/LDP/Bash-Beginners-Guide/html/sect_03_04.html .
/// </summary>
/// <remarks>
/// <para>Tilde ('~') character at the beginning of the value can not be escaped.</para>
/// <para>Windows platform allows both slashes
/// to be directory separators while in *nix systems
/// backslash is allowed file name character
/// ( https://en.wikipedia.org/wiki/Filename ).</para>
/// </remarks>
public sealed class TildeShellExpander : ShellExpander
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TildeShellExpander"/> class.
    /// </summary>
    /// <param name="previous">Optional previous expander.</param>
    /// <param name="tildePrefixes">Optional collection of tilde prefixes
    ///     to expanded value mapping.
    ///     Read https://tldp.org/LDP/Bash-Beginners-Guide/html/sect_03_04.html ,
    ///     allows override of default expansion value of '~',
    ///     e.g. '~+/path' ('+' is prefix) or '~-' ('-' is prefix).
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if
    ///     <paramref name="tildePrefixes"/> contains
    ///     empty tilde prefixes (keys).</exception>
    public TildeShellExpander(
            IShellExpander? previous = null,
            IEnumerable<KeyValuePair<string, string>>? tildePrefixes = null)
        : base(previous)
    {
        this.Prefixes = Ensure.Optional(tildePrefixes)
                .AllKeys(k => Ensure.Param(k).NotEmpty().Done())
                .ToImmutableDictionary();
    }

    /// <summary>
    /// Gets home expanded value.
    /// </summary>
    public static string Home => Environment.GetEnvironmentVariable("USERPROFILE")
            ?? Environment.GetEnvironmentVariable("HOME");

    /// <inheritdoc />
    public override string Summary =>
            "Expands leading tilde character \"~\" to home directory if no prefix "
            + "(characters between tilde and directory separator) is present. "
            + "No escaping is supported. Available tilde-prefixes: "
            + this.TildePrefixesSummary();

    /// <summary>
    /// Gets available tilde prefixes. Can be empty.
    /// </summary>
    public IImmutableDictionary<string, string> Prefixes { get; }

    /// <inheritdoc/>
    protected override string ExpandThis(string value)
    {
        // https://learn.microsoft.com/en-us/dotnet/api/system.io.pathtoolongexception?view=net-6.0
        Ensure.Param(value).InRange(0, 32_767).Done();

        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        StringBuilder result = new();
        int pos = 0;
        string? tildePrefix = null;

        while (pos < value.Length)
        {
            char current = value[pos];

            if (pos == 0)
            {
                if (current != '~')
                {
                    return value; // no expansion
                }
            }
            else if (
                    current == System.IO.Path.DirectorySeparatorChar
                    || current == System.IO.Path.AltDirectorySeparatorChar)
            {
                tildePrefix = value[1..pos];
                break;
            }
            else if (pos == (value.Length - 1))
            {
                tildePrefix = value[1..];
            }

            pos++;
        }

        if (string.IsNullOrEmpty(tildePrefix))
        {
            _ = result.Append(Home);
        }
        else if (this.Prefixes.TryGetValue(tildePrefix, out string expandedValue))
        {
            _ = result.Append(expandedValue);
        }
        else
        {
            return value; // fall back to the original value
        }

        return result.Append(value[pos..]).ToString();
    }

    /// <summary>
    /// Return human readable collection of the tilde-prefixed
    /// in this instance.
    /// </summary>
    /// <returns>Human readable collection of available tilde-prefixes.</returns>
    private string TildePrefixesSummary()
    {
        return string.Join(", ", this.Prefixes.Keys.Select(k => Dump.Literal(k)));
    }
}
