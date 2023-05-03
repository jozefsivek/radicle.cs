namespace Radicle.Common.Extensions;

using System;
using Radicle.Common.Tokenization;
using Xunit;

public class StringExtensionsTest
{
    [Fact]
    public void Ellipsis_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ((string)null!).Ellipsis());
    }

    [Fact]
    public void Ellipsis_NonPositiveTrim_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => string.Empty.Ellipsis(trim: 0));
    }

    [Fact]
    public void Ellipsis_TooLongSuffix_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
                () => string.Empty.Ellipsis(
                    trim: 1,
                    fuzziness: 1,
                    suffix: "way too long suffix"));
    }

    /*
     "Red brown fox jumped"
      123456789_123456789_
     */
    [Theory]
    [InlineData("Red brown fox jumped", 18, 2)]
    [InlineData("Red brown fox j...", 18, 1)]
    [InlineData("Red brown fox j...", 18, 0)]
    [InlineData("Red brown fox jumped", 1, 19)]
    [InlineData("R", 1, 0)]
    [InlineData("R.", 1, 1)]
    [InlineData("...", 2, 1)]
    [InlineData("R...", 3, 1)]
    [InlineData("R...", 1, 3)]
    public void Ellipsis_Works(
            string expected,
            ushort trim,
            ushort fuziness)
    {
        const string str = "Red brown fox jumped";

        string actual = str.Ellipsis(
                    trim: trim,
                    fuzziness: fuziness);

        Assert.Equal(
                expected,
                actual);
    }

    [Fact]
    public void Snippet_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ((string)null!).Snippet());
    }

    [Fact]
    public void Snippet_NonPositiveTrim_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => string.Empty.Snippet(trim: 0));
    }

    /*
     "Red brown fox jumped"
      123456789_123456789_
      _987654321_987654321
     */
    [Theory]
    [InlineData("Red brown fox jumped", 18, 2)]
    [InlineData("Red brown[+11 more]", 18, 1)]
    [InlineData("Red brown[+11 more]", 18, 0)]
    [InlineData("Red brown fox jumped", 1, 19)]
    [InlineData("Red[+17]", 8, 0)]
    [InlineData("[", 1, 0)]
    [InlineData("[+", 1, 1)]
    [InlineData("[+]", 2, 1)]
    [InlineData("R[+]", 1, 3)]
    [InlineData("Red[+]", 1, 5)]
    [InlineData("Red [+]", 1, 6)]
    [InlineData("R[+]", 3, 1)]
    [InlineData("[+20]", 5, 1)]
    [InlineData("R[+19]", 6, 1)]
    [InlineData("Re[+18]", 7, 1)]
    public void Snippet_Works(
            string expected,
            ushort trim,
            ushort fuziness)
    {
        const string str = "Red brown fox jumped";

        string actual = str.Snippet(
                    trim: trim,
                    fuzziness: fuziness);

        Assert.Equal(
                expected,
                actual);
    }

    [Fact]
    public void SnippetLiteral_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ((string)null!).SnippetLiteral());
    }

    [Fact]
    public void SnippetLiteral_NonPositiveTrim_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => string.Empty.SnippetLiteral(trim: 0));
    }

    /*
     "Red brown fox jumped"
      123456789_123456789_
      _987654321_987654321
     */
    [Theory]
    [InlineData("\"Red brown fox jumped\"", 18, 2)]
    [InlineData("\"Red brown\"[+11 more]", 18, 1)]
    [InlineData("\"Red brown\"[+11 more]", 18, 0)]
    [InlineData("\"Red brown fox jumped\"", 1, 19)]
    [InlineData("\"Red\"[+17]", 8, 0)]
    [InlineData("\"\"[", 1, 0)]
    [InlineData("\"\"[+", 1, 1)]
    [InlineData("\"\"[+]", 2, 1)]
    [InlineData("\"R\"[+]", 1, 3)]
    [InlineData("\"Red\"[+]", 1, 5)]
    [InlineData("\"Red \"[+]", 1, 6)]
    [InlineData("\"R\"[+]", 3, 1)]
    [InlineData("\"\"[+20]", 5, 1)]
    [InlineData("\"R\"[+19]", 6, 1)]
    [InlineData("\"Re\"[+18]", 7, 1)]
    public void SnippetLiteral_Works(
            string expected,
            ushort trim,
            ushort fuziness)
    {
        const string str = "Red brown fox jumped";

        string actual = str.SnippetLiteral(
                    trim: trim,
                    fuzziness: fuziness);

        Assert.Equal(
                expected,
                actual);
    }

    [Theory]
    [InlineData("\"R\\xc3\\xabd brow\\n\"[+11 more]", 18, 1)]
    [InlineData("\"R\\xc3\\xabd brow\\n fox'jumped\"", 1, 19)]
    public void SnippetLiteral_InputWithEscapeConservative_Works(
            string expected,
            ushort trim,
            ushort fuziness)
    {
        const string str = "R\u00EBd brow\n fox\'jumped";

        string actual = str.SnippetLiteral(
                    trim: trim,
                    fuzziness: fuziness);

        Assert.Equal(
                expected,
                actual);
    }

    [Theory]
    [InlineData("\"R\u00EBd brow\\n\"[+11 more]", 18, 1)]
    [InlineData("\"R\u00EBd brow\\n fox'jumped\"", 1, 19)]
    public void SnippetLiteral_InputWithEscapeNormal_Works(
            string expected,
            ushort trim,
            ushort fuziness)
    {
        const string str = "R\u00EBd brow\n fox\'jumped";

        string actual = str.SnippetLiteral(
                    trim: trim,
                    fuzziness: fuziness,
                    literalDefinition: CLikeStringLiteralDefinition.Normal);

        Assert.Equal(
                expected,
                actual);
    }

    [Theory]
    [InlineData("\"R\u00EBd brow\n\"[+11 more]", 18, 1)]
    [InlineData("\"R\u00EBd brow\n fox'jumped\"", 1, 19)]
    public void SnippetLiteral_InputWithEscapeMinimal_Works(
            string expected,
            ushort trim,
            ushort fuziness)
    {
        const string str = "R\u00EBd brow\n fox\'jumped";

        string actual = str.SnippetLiteral(
                    trim: trim,
                    fuzziness: fuziness,
                    literalDefinition: CLikeStringLiteralDefinition.Minimal);

        Assert.Equal(
                expected,
                actual);
    }

    [Fact]
    public void ToLines_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ((string)null!).ToLines());
    }

    [Theory]
    [InlineData("", new[] { "" })]
    [InlineData("\r\n", new[] { "", "" })]
    [InlineData("\n\r", new[] { "", "" })]
    [InlineData("\n", new[] { "", "" })]
    [InlineData("\n\n", new[] { "", "", "" })]
    [InlineData("\r", new[] { "", "" })]
    [InlineData("foo\r\nbar\n", new[] { "foo", "bar", "" })]
    [InlineData("foo\nbar", new[] { "foo", "bar" })]
    public void ToLines_ValidInput_Works(
            string input,
            string[] expected)
    {
        Assert.Equal(expected, input.ToLines());
    }

    [Theory]
    [InlineData("\r\n", StringSplitOptions.RemoveEmptyEntries, new string[] { })]
    [InlineData("\n", StringSplitOptions.RemoveEmptyEntries, new string[] { })]
    [InlineData("foo\n\nbar", StringSplitOptions.RemoveEmptyEntries, new string[] { "foo", "bar" })]
    [InlineData(" foo\n\n\tbar", StringSplitOptions.TrimEntries, new string[] { "foo", "", "bar" })]
    [InlineData("  \n\n\t", StringSplitOptions.TrimEntries, new string[] { "", "", "" })]
    public void ToLines_ComplexInput_Works(
            string input,
            StringSplitOptions options,
            string[] expected)
    {
        Assert.Equal(expected, input.ToLines(options: options));
    }
}
