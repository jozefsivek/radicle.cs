namespace Radicle.Common.Check.Models;

using System;
using Moq;
using Radicle.Common.Extensions;
using Xunit;

public class StringParamTest
{
    [Fact]
    public void Param_StringInput_ReturnsIStringParam()
    {
        Assert.IsAssignableFrom<IStringParam>(Ensure.Param(string.Empty));
    }

    [Fact]
    public void Optional_StringInput_ReturnsIStringParam()
    {
        Assert.IsAssignableFrom<IStringParam>(Ensure.Optional((string?)null));
    }

    [Fact]
    public void That_StringInput_PassesString()
    {
        bool isString = false;

        Ensure.Param(string.Empty).That(s => isString = s is not null && string.IsNullOrEmpty(s));

        Assert.True(isString);
    }

    [Fact]
    public void OptionalThat_StringInput_PassesString()
    {
        bool isString = false;

        Ensure.Optional(string.Empty).That(s => isString = s is not null && string.IsNullOrEmpty(s));

        Assert.True(isString);
    }

    [Fact]
    public void Optional_NullInput_Works()
    {
        Ensure.Optional((string?)null).NotEmpty();
        Ensure.Optional((string?)null).NotWhiteSpace();
        Ensure.Optional((string?)null).InRange(0, 256);
        Ensure.Optional((string?)null).IsRegex();
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("a")]
    [InlineData("\n")]
    public void Empty_ValidParam_Works(string input)
    {
        Ensure.Param(input).NotEmpty();
    }

    [Theory]
    [InlineData("")]
    public void Empty_InvalidParam_Throws(string input)
    {
        ArgumentOutOfRangeException exc = Assert.Throws<ArgumentOutOfRangeException>(
                () => Ensure.Param(input).NotEmpty());

        Assert.StartsWith(
                "Parameter 'input' cannot be an empty string.",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("name")]
    [InlineData("A sentence!")]
    public void NotWhiteSpace_ValidParam_Works(string input)
    {
        Ensure.Param(input).NotWhiteSpace();
    }

    [Theory]
    [InlineData("")]
    public void NotWhiteSpace_EmptyArgument_Throws(string input)
    {
        ArgumentOutOfRangeException exc = Assert.Throws<ArgumentOutOfRangeException>(
                () => Ensure.Param(input).NotWhiteSpace());

        Assert.StartsWith(
                "Parameter 'input' cannot be an empty string.",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("\n")]
    [InlineData("\r")]
    [InlineData("\t")]
    public void NotWhiteSpace_InvalidParam_Throws(string input)
    {
        ArgumentException exc = Assert.Throws<ArgumentException>(
                () => Ensure.Param(input).NotWhiteSpace());

        Assert.StartsWith(
                $"Parameter 'input' with value: {Dump.Literal(input)} cannot be a white space string.",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("foo")]
    [InlineData("")]
    [InlineData("foo bar")]
    [InlineData("foo\tbar")]
    [InlineData(" foo\tbar ")]
    public void NoNewLines_ValidParam_Works(string input)
    {
        Ensure.Param(input).NoNewLines();
    }

    [Fact]
    public void NoNewLines_LongParam_Works()
    {
        Ensure.Param(new string('x', 17_000)).NoNewLines();
    }

    [Theory]
    [InlineData("\n")]
    [InlineData("\r")]
    [InlineData("\n\r")]
    [InlineData("\r\n")]
    [InlineData("foo\n")]
    [InlineData("foo\r")]
    [InlineData("foo\n\r")]
    [InlineData("foo\r\n")]
    [InlineData("\nbar")]
    [InlineData("\rbar")]
    [InlineData("\n\rbar")]
    [InlineData("\r\nbar")]
    public void NoNewLines_InvalidParam_Throws(string input)
    {
        ArgumentException exc = Assert.Throws<ArgumentException>(
                () => Ensure.Param(input).NoNewLines());

        Assert.StartsWith(
                $"Parameter 'input' with value: {Dump.Literal(input)} cannot be a string with new lines.",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("ham", 2, 4, true, true)]
    [InlineData("em", 2, 4, true, false)]
    [InlineData("buzz", 2, 4, false, true)]
    [InlineData("egg", 2, 4, false, false)]
    public void InRange_ValidParam_Works(
            string input,
            ushort min,
            ushort max,
            bool lower,
            bool upper)
    {
        Ensure.Param(input).InRange(min, max, lower, upper);
    }

    [Theory]
    [InlineData("a", 2, 3, true, true)]
    [InlineData("em", 2, 3, false, true)]
    [InlineData("ham", 2, 3, true, false)]
    [InlineData("parrot", 2, 3, false, false)]
    public void InRange_InvalidParam_Throws(
            string input,
            ushort min,
            ushort max,
            bool lower,
            bool upper)
    {
        ArgumentOutOfRangeException exc = Assert.Throws<ArgumentOutOfRangeException>(
                () => Ensure.Param(input).InRange(min, max, lower, upper));

        string range = Dump.Range(min, max, includeLower: lower, includeUpper: upper);

        Assert.StartsWith(
                $"Parameter 'input' with value: {Dump.Literal(input)} length must be in range {range}",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Fact]
    public void InRange_InvalidLongParam_Throws()
    {
        string input = new('a', 160);
        ArgumentOutOfRangeException exc = Assert.Throws<ArgumentOutOfRangeException>(
                () => Ensure.Param(input).InRange(1, 24));

        string range = Dump.Range(1, 24);

        Assert.StartsWith(
                $"Parameter 'input' with value: {input.SnippetLiteral()} length must be in range {range}",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(".*")]
    [InlineData("")]
    [InlineData("(?:([0-9]{2,}))")]
    public void IsRegex_ValidParam_Works(string input)
    {
        Ensure.Param(input).IsRegex();
    }

    [Theory]
    [InlineData("(()")]
    [InlineData("{****}}")]
    public void IsRegex_InvalidParam_Throws(string input)
    {
        ArgumentException exc = Assert.Throws<ArgumentException>(
                () => Ensure.Param(input).IsRegex());

        Assert.StartsWith(
                $"Parameter 'input' with value: {Dump.Literal(input)} must be a valid regex.",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Fact]
    public void Conforms_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Ensure.Param("foo").Conforms(null!));
    }

    [Fact]
    public void Conforms_InvalidInput_Throws()
    {
        Mock<ITypedNameSpec> specification = new();
        string calledValue = default!;

        specification
                .Setup(f => f.EnsureValid(It.IsAny<string>(), It.IsAny<string?>()))
                .Callback((string value, string? _) => calledValue = value)
                .Returns(() => throw new ArgumentException());

        Assert.Throws<ArgumentException>(() =>
                Ensure.Param("foo").Conforms(specification.Object));

        Assert.Equal("foo", calledValue);
    }

    [Fact]
    public void Conforms_ValidInput_Works()
    {
        Mock<ITypedNameSpec> specification = new();
        string calledValue = default!;

        specification
                .Setup(f => f.EnsureValid(It.IsAny<string>(), It.IsAny<string?>()))
                .Callback((string value, string? _) => calledValue = value)
                .Returns(() => calledValue);

        Ensure.Param("foo").Conforms(specification.Object);
    }
}
