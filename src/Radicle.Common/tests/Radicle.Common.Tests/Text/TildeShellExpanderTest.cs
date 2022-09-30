namespace Radicle.Common.Text;

using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

public class TildeShellExpanderTest
{
    public static readonly string Home = Environment.GetEnvironmentVariable("HOME")
            ?? Environment.GetEnvironmentVariable("USERPROFILE")
            ?? "missing";

    public static readonly HashSet<char> Separators = new(new[]
    {
        System.IO.Path.DirectorySeparatorChar,
        System.IO.Path.AltDirectorySeparatorChar,
    });

    private readonly ITestOutputHelper output;

    public TildeShellExpanderTest(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void Constructor_EmptyPrefixes_Throws()
    {
        Dictionary<string, string> invalidPrefixes = new()
        {
            { "a", "A" },
            { string.Empty, "B" },
        };

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new TildeShellExpander(tildePrefixes: invalidPrefixes));
    }

    [Fact]
    public void Expand_NullValue_Throws()
    {
        TildeShellExpander expander = new();

        Assert.Throws<ArgumentNullException>(() => expander.Expand(null!));
    }

    [Fact]
    public void Expant_OutOfBoundInput_Throws()
    {
        TildeShellExpander expander = new();
        string invalidValue = new(' ', 32_768);

        Assert.Throws<ArgumentOutOfRangeException>(() => expander.Expand(invalidValue));
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(" ~b", " ~b")]
    [InlineData("a~b", "a~b")]
    [InlineData("~a", "~a")]
    [InlineData("~a/", "~a/")]
    [InlineData("~a\\", "~a\\")]
    [InlineData("/path/to/", "/path/to/")]
    [InlineData("\\path\\to\\", "\\path\\to\\")]
    [InlineData("D:\\\\path\\to\\", "D:\\\\path\\to\\")]
    [InlineData("$", "$")]
    public void Expand_NoOp_Works(string input, string expected)
    {
        TildeShellExpander expander = new();

        string result = expander.Expand(input);

        Assert.Equal(expected, result);

        this.output.WriteLine($"{Dump.Literal(input)} expanded to {Dump.Literal(result)}");
    }

    [Fact]
    public void Expand_ValidExpandableInput_Works()
    {
        TildeShellExpander expander = new();

        Dictionary<string, string> values = new()
        {
            { "~", Home },
        };

        foreach (char separator in Separators)
        {
            values[$"~{separator}"] = $"{Home}{separator}";
            values[$"~{separator}path{separator}"] = $"{Home}{separator}path{separator}";
        }

        foreach (KeyValuePair<string, string> pair in values)
        {
            string result = expander.Expand(pair.Key);

            Assert.Equal(pair.Value, result);

            this.output.WriteLine($"{Dump.Literal(pair.Key)} expanded to {Dump.Literal(pair.Value)}");
        }
    }

    [Theory]
    [InlineData("~a", "ONE")]
    [InlineData("~-", "TWO")]
    [InlineData("~+", "~+")]
    [InlineData("~A", "~A")]
    public void Expand_ValidExpandableInputWithPrefix_Works(string input, string expected)
    {
        TildeShellExpander expander = new(tildePrefixes: new Dictionary<string, string>()
        {
            { "a", "ONE" },
            { "-", "TWO" },
        });

        string result = expander.Expand(input);

        Assert.Equal(expected, result);

        this.output.WriteLine($"{Dump.Literal(input)} expanded to {Dump.Literal(result)}");
    }

    [Theory]
    [InlineData("~a", "ONE")]
    [InlineData("~-", "TWO")]
    [InlineData("~+", "~+")]
    [InlineData("~A", "~A")]
    public void Expand_ValidExpandableInputWithPrefixAndPathDelimeter_Works(string input, string expected)
    {
        TildeShellExpander expander = new(tildePrefixes: new Dictionary<string, string>()
        {
            { "a", "ONE" },
            { "-", "TWO" },
        });

        string result = expander.Expand(input + System.IO.Path.DirectorySeparatorChar);

        Assert.Equal(expected + System.IO.Path.DirectorySeparatorChar, result);

        this.output.WriteLine($"{Dump.Literal(input)} expanded to {Dump.Literal(result)}");
    }

    [Theory]
    [InlineData("~a", "ONE")]
    [InlineData("~-", "TWO")]
    [InlineData("~+", "~+")]
    [InlineData("~A", "~A")]
    public void Expand_ValidExpandableInputWithPrefixAndAltPathDelimeter_Works(string input, string expected)
    {
        TildeShellExpander expander = new(tildePrefixes: new Dictionary<string, string>()
        {
            { "a", "ONE" },
            { "-", "TWO" },
        });

        string result = expander.Expand(input + System.IO.Path.AltDirectorySeparatorChar);

        Assert.Equal(expected + System.IO.Path.AltDirectorySeparatorChar, result);

        this.output.WriteLine($"{Dump.Literal(input)} expanded to {Dump.Literal(result)}");
    }
}
