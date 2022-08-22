namespace Radicle.Common.Tokenization;

using System;
using System.Linq;
using Radicle.Common.Tokenization.Models;
using Xunit;

public class CLILikeArgumentsTokenizerTest
{
    [Fact]
    public void Parse_WithNullInput_Fails()
    {
        CLILikeArgumentsTokenizer t = new();

        Assert.Throws<ArgumentNullException>(() => t.Parse(null!).ToArray());
    }

    [Fact]
    public void Parse_WithEmptyInput_YieldsEmpty()
    {
        CLILikeArgumentsTokenizer t = new();

        Assert.Empty(t.Parse(string.Empty));
    }

    [Fact]
    public void Parse_WithEmptyTokenInput_YieldsEmpty()
    {
        CLILikeArgumentsTokenizer t = new();

        Assert.Empty(t.Parse(" "));
    }

    [Theory]
    [InlineData("a", "a")]
    [InlineData("a\"\\\"\"b", "a\"b")]
    [InlineData("a\" \"b", "a b")]
    [InlineData("a\"\\t\"b", "a\tb")]
    public void Parse_WithSingleStringToken_YieldsInput(string input, string expected)
    {
        CLILikeArgumentsTokenizer t = new();

        TokenWithValue[] decoded = t.Parse(input).ToArray();

        Assert.Single(decoded);
        TokenString ds = Assert.IsType<TokenString>(decoded[0]);
        Assert.Equal(expected, ds.StringValue);
    }

    [Theory]
    [InlineData("a b", "a", "b")]
    [InlineData("a\tb", "a", "b")]
    [InlineData("a\nb", "a", "b")]
    [InlineData("a\rb", "a", "b")]
    [InlineData("a\fb", "a", "b")]
    [InlineData("a\vb", "a", "b")]
    [InlineData("foo   bar", "foo", "bar")]
    [InlineData("  foo bar", "foo", "bar")]
    [InlineData("foo bar  ", "foo", "bar")]
    [InlineData("a\"\\\"\"b \" \"", "a\"b", " ")]
    public void Parse_WithTwoStringTokens_YieldsInput(string input, string expected1, string expected2)
    {
        CLILikeArgumentsTokenizer t = new();

        TokenWithValue[] decoded = t.Parse(input).ToArray();

        Assert.Equal(2, decoded.Length);

        TokenString ds1 = Assert.IsType<TokenString>(decoded[0]);
        Assert.Equal(expected1, ds1.StringValue);

        TokenString ds2 = Assert.IsType<TokenString>(decoded[1]);
        Assert.Equal(expected2, ds2.StringValue);
    }

    [Theory]
    [InlineData(" foo\n", 1)]
    [InlineData("a\"\\\"\"b", 1)]
    [InlineData("a b c d e", 5)]
    [InlineData(" a \"b c d\" e ", 3)]
    [InlineData(" \"\" \"\"\"\" \"\" ", 3)]
    public void Parse_WithMultipleStringTokens_YieldsInput(string input, int tokenCount)
    {
        CLILikeArgumentsTokenizer t = new();

        TokenWithValue[] decoded = t.Parse(input).ToArray();

        Assert.Equal(tokenCount, decoded.Length);
    }

    [Fact]
    public void Parse_WithSingleBinaryToken_YieldsInput()
    {
        CLILikeArgumentsTokenizer t = new();

        TokenWithValue[] decoded = t.Parse("\"\\x00\"").ToArray();

        Assert.Single(decoded);
        TokenBinary tokenWithBinary = Assert.IsType<TokenBinary>(decoded[0]);
        Assert.Equal(new byte[] { 0 }, tokenWithBinary.Bytes.ToArray());
    }
}
