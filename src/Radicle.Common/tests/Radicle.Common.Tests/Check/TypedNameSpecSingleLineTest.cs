namespace Radicle.Common.Check;

using Xunit;

public class TypedNameSpecSingleLineTest
{
    [Fact]
    public void Description_HasExpectedValue()
    {
        Assert.Equal(
                "Single line: mixed case any character except '\\n' or '\\r' with length range [1, 16384], case sensitive comparison",
                TypedNameSpec.SingleLine.Description);
    }

    [Fact]
    public void IsValid_LongestValidValue_ReturnsTrue()
    {
        Assert.True(TypedNameSpec.SingleLine
                .IsValid(new string(' ', 256)));
    }

    [Theory]
    [InlineData("any string will do")]
    [InlineData("A")]
    [InlineData("\ud83e\udd5a\ud83e\udd9c")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData(".")]
    [InlineData("%")]
    [InlineData("&")]
    [InlineData("^")]
    [InlineData("+")]
    [InlineData("*")]
    [InlineData("{")]
    [InlineData("}")]
    [InlineData("/")]
    [InlineData("\\")]
    public void IsValid_ValidValue_ReturnsTrue(string validValue)
    {
        Assert.True(TypedNameSpec.SingleLine.IsValid(validValue));
    }

    [Fact]
    public void IsValid_NullValue_ReturnsFalse()
    {
        Assert.False(TypedNameSpec.SingleLine.IsValid(null!));
    }

    [Fact]
    public void IsValid_LongValue_ReturnsFalse()
    {
        Assert.False(TypedNameSpec.SingleLine
                .IsValid(new string(' ', 16385)));
    }

    [Theory]
    [InlineData("\n")]
    [InlineData("a\n")]
    [InlineData("a\r")]
    [InlineData("a\r\n")]
    [InlineData("\na")]
    [InlineData("\n\n\n")]
    [InlineData(" \n")]
    [InlineData(" \n\n\n")]
    [InlineData("\r")]
    [InlineData("\n\r")]
    [InlineData("\r\n")]
    [InlineData("a\na")]
    [InlineData("A\r")]
    [InlineData("b\n\r")]
    [InlineData("B\r\n")]
    [InlineData("\n ")]
    [InlineData("\r ")]
    [InlineData("\n\r ")]
    [InlineData("\r\n ")]
    [InlineData("\n \r")]
    [InlineData("\r \n")]
    [InlineData("&\r+\n*")]
    public void IsValid_InvalidValue_ReturnsFalse(string invalidValue)
    {
        Assert.False(TypedNameSpec.SingleLine.IsValid(invalidValue));
    }
}
