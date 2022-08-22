namespace Radicle.Common.Tokenization;

using System;
using System.Collections.Generic;
using System.Linq;
using Radicle.Common.Tokenization.Models;
using Xunit;

public class StopWordStringParserTest
{
    private static readonly ParsedTokenStopWord Em = new('_');

    private static readonly ParsedTokenStopWord Bold = new('*');

    private static readonly ParsedTokenStopWord Escape = new('\\');

    [Fact]
    public void Constructor_NullStopWords_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                new StopWordStringParser(stopWords: new HashSet<ParsedTokenStopWord>() { null! }));
    }

    [Fact]
    public void Parse_NullInput_Throws()
    {
        StopWordStringParser p = new();

        Assert.Throws<ArgumentNullException>(() => p.Parse(null!).ToArray());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    public void Parse_OutOfRangeStart_Throws(int startAt)
    {
        StopWordStringParser p = new();

        Assert.Throws<ArgumentOutOfRangeException>(() => p.Parse("a", startAt: startAt).ToArray());
    }

    [Fact]
    public void Parse_EmptyInput_YieldsEmptyCollection()
    {
        StopWordStringParser p = new();

        Assert.Empty(p.Parse(string.Empty));
    }

    [Fact]
    public void Parse_EndOfStringStart_YieldsEmptyCollection()
    {
        StopWordStringParser p = new();

        Assert.Empty(p.Parse("a", startAt: 1));
    }

    [Theory]
    [InlineData("foo")]
    [InlineData("foo bar")]
    [InlineData("foo bar\n\tbar foo\n")]
    public void Parse_InputFreeText_Works(string input)
    {
        StopWordStringParser p = ConstructSimpleMarkdownTokenParser();

        ParsedToken[] tokens = p.Parse(input).ToArray();

        ParsedToken token = Assert.Single(tokens);
        Assert.Equal(new ParsedTokenFreeText(input), token);
    }

    [Fact]
    public void Parse_InputWithOneLeadingControl_Works()
    {
        StopWordStringParser p = ConstructSimpleMarkdownTokenParser();

        Assert.Equal(
            new ParsedToken[]
            {
                new ParsedTokenStopWord("\\"),
                new ParsedTokenControl("*"),
                new ParsedTokenFreeText("foo bar"),
            },
            p.Parse("\\*foo bar"));
    }

    [Fact]
    public void Parse_InputWithOneControl_Works()
    {
        StopWordStringParser p = ConstructSimpleMarkdownTokenParser();

        Assert.Equal(
            new ParsedToken[]
            {
                new ParsedTokenFreeText("foo"),
                new ParsedTokenStopWord("\\"),
                new ParsedTokenControl("*"),
                new ParsedTokenFreeText("bar"),
            },
            p.Parse("foo\\*bar"));
    }

    [Fact]
    public void Parse_InputWithOneTrailingControl_Works()
    {
        StopWordStringParser p = ConstructSimpleMarkdownTokenParser();

        Assert.Equal(
            new ParsedToken[]
            {
                new ParsedTokenFreeText("foo bar"),
                new ParsedTokenStopWord("\\"),
                new ParsedTokenControl("*"),
            },
            p.Parse("foo bar\\*"));
    }

    [Fact]
    public void Parse_InputWithTwoControl_Works()
    {
        StopWordStringParser p = ConstructSimpleMarkdownTokenParser();

        Assert.Equal(
            new ParsedToken[]
            {
                new ParsedTokenFreeText("foo"),
                new ParsedTokenStopWord("\\"),
                new ParsedTokenControl("*"),
                new ParsedTokenFreeText("bar"),
                new ParsedTokenStopWord("\\"),
                new ParsedTokenControl("n"),
            },
            p.Parse("foo\\*bar\\n"));
    }

    [Theory]
    [InlineData("\\")]
    [InlineData("1")]
    [InlineData("?")]
    [InlineData("#")]
    [InlineData("{")]
    public void Parse_InputWithOneVariousControl_Works(string controlSequence)
    {
        StopWordStringParser p = ConstructSimpleMarkdownTokenParser();

        Assert.Equal(
            new ParsedToken[]
            {
                new ParsedTokenFreeText("foo"),
                new ParsedTokenStopWord("\\"),
                new ParsedTokenControl(controlSequence),
                new ParsedTokenFreeText("bar"),
            },
            p.Parse($"foo\\{controlSequence}bar"));
    }

    [Fact]
    public void Parse_IncompleteControl_Works()
    {
        StopWordStringParser p = ConstructSimpleMarkdownTokenParser();

        Assert.Equal(
            new ParsedToken[]
            {
                new ParsedTokenFreeText("foo"),
                new ParsedTokenStopWord("\\"),
                new ParsedTokenControl("*"),
                new ParsedTokenFreeText("bar"),
                new ParsedTokenStopWord("\\"),
                new ParsedTokenControl(string.Empty, incompleteRead: true),
            },
            p.Parse("foo\\*bar\\"));
    }

    [Theory]
    [InlineData("*")]
    [InlineData("_")]
    public void Parse_WithMarkdownStopWordPairInput_works(
            string stopWord)
    {
        StopWordStringParser p = ConstructSimpleMarkdownTokenParser();

        Assert.Equal(
            new ParsedToken[]
            {
                new ParsedTokenFreeText("foo"),
                new ParsedTokenStopWord(stopWord),
                new ParsedTokenStopWord("\\"),
                new ParsedTokenControl(stopWord),
                new ParsedTokenFreeText("bar"),
                new ParsedTokenStopWord(stopWord),
            },
            p.Parse($"foo{stopWord}\\{stopWord}bar{stopWord}"));
    }

    private static StopWordStringParser ConstructSimpleMarkdownTokenParser()
    {
        return new StopWordStringParser(
                stopWords: new HashSet<ParsedTokenStopWord>(new[]
                {
                    Em,
                    Bold,
                    Escape,
                }),
                controlTokenAction: (state) =>
                {
                    if (state.TriggeringStopWord == Escape)
                    {
                        return ParseAction.ContinueWithControllUntil(1);
                    }

                    return ParseAction.Continue;
                });
    }
}
