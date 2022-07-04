namespace Radicle.Common.Check;

using Xunit;

public class TypedNameSpecURLTokenTest
{
    [Fact]
    public void Description_HasExpectedValue()
    {
        Assert.Equal(
                "URL token: mixed case alphanumerical, '_', '-' and '.' with length range [1, 256], case sensitive comparison",
                TypedNameSpec.URLToken.Description);
    }

    [Fact]
    public void IsValid_LongestValidValue_ReturnsTrue()
    {
        Assert.True(TypedNameSpec.URLToken
                .IsValid(new string('v', 256)));
    }

    [Theory]
    [InlineData("a")]
    [InlineData("A")]
    [InlineData("_")]
    [InlineData("__var__")]
    [InlineData("a24")]
    [InlineData("A24")]
    [InlineData("a-b")]
    [InlineData("a.b")]
    [InlineData("A.B")]
    [InlineData(".")]
    [InlineData(".a")]
    [InlineData(".a.")]
    [InlineData(".A.")]
    [InlineData("-9")]
    [InlineData("-")]
    public void IsValid_ValidValue_ReturnsTrue(string validValue)
    {
        Assert.True(TypedNameSpec.URLToken.IsValid(validValue));
    }

    [Fact]
    public void IsValid_NullValue_ReturnsFalse()
    {
        Assert.False(TypedNameSpec.URLToken.IsValid(null!));
    }

    [Fact]
    public void IsValid_LongValue_ReturnsFalse()
    {
        Assert.False(TypedNameSpec.URLToken
                .IsValid(new string('v', 257)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("\n")]
    [InlineData("a\n")]
    [InlineData("a\r")]
    [InlineData("a\r\n")]
    [InlineData("\na")]
    [InlineData("%")]
    public void IsValid_InvalidValue_ReturnsFalse(string invalidValue)
    {
        Assert.False(TypedNameSpec.URLToken.IsValid(invalidValue));
    }
}
