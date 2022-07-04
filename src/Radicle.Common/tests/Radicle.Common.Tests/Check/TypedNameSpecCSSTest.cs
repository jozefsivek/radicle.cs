namespace Radicle.Common.Check;

using Xunit;

public class TypedNameSpecCSSTest
{
    [Fact]
    public void Description_HasExpectedValue()
    {
        Assert.Equal(
                "CSS class name: mixed case alphanumerical, '_' and '-', non number first character, '--' or '-' followed by number with length range [1, 256], case sensitive comparison",
                TypedNameSpec.CSS.Description);
    }

    [Fact]
    public void IsValid_LongestValidValue_ReturnsTrue()
    {
        Assert.True(TypedNameSpec.CSS.IsValid(new string('v', 256)));
    }

    [Theory]
    [InlineData("a")]
    [InlineData("A")]
    [InlineData("_")]
    [InlineData("__var__")]
    [InlineData("__VAR__")]
    [InlineData("a24")]
    [InlineData("A24")]
    [InlineData("-")]
    [InlineData("a-b")]
    [InlineData("-a")]
    [InlineData("-A")]
    public void IsValid_ValidValue_ReturnsTrue(string validValue)
    {
        Assert.True(TypedNameSpec.CSS.IsValid(validValue));
    }

    [Fact]
    public void IsValid_NullValue_ReturnsFalse()
    {
        Assert.False(TypedNameSpec.CSS.IsValid(null!));
    }

    [Fact]
    public void IsValid_LongValue_ReturnsFalse()
    {
        Assert.False(TypedNameSpec.CSS.IsValid(new string('v', 257)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("1")]
    [InlineData("a.b")]
    [InlineData("A.B")]
    [InlineData(".")]
    [InlineData("\n")]
    [InlineData("a\n")]
    [InlineData("a\r")]
    [InlineData("a\r\n")]
    [InlineData("\na")]
    [InlineData("-9a")]
    [InlineData("--a")]
    [InlineData("--A")]
    public void IsValid_InvalidValue_ReturnsFalse(string invalidValue)
    {
        Assert.False(TypedNameSpec.CSS.IsValid(invalidValue));
    }
}
