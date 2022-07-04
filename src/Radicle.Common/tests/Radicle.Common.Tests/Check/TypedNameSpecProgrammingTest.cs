namespace Radicle.Common.Check;

using Xunit;

public class TypedNameSpecProgrammingTest
{
    [Fact]
    public void Description_HasExpectedValue()
    {
        Assert.Equal(
                "Programming name: mixed case alphanumerical and '_', non number first character with length range [1, 256], case sensitive comparison",
                TypedNameSpec.Programming.Description);
    }

    [Fact]
    public void IsValid_LongestValidValue_ReturnsTrue()
    {
        Assert.True(TypedNameSpec.Programming
                .IsValid(new string('v', 256)));
    }

    [Theory]
    [InlineData("a")]
    [InlineData("A")]
    [InlineData("_")]
    [InlineData("__var__")]
    [InlineData("__VAR__")]
    [InlineData("a24")]
    [InlineData("A24")]
    public void IsValid_ValidValue_ReturnsTrue(string validValue)
    {
        Assert.True(TypedNameSpec.Programming.IsValid(validValue));
    }

    [Fact]
    public void IsValid_NullValue_ReturnsFalse()
    {
        Assert.False(TypedNameSpec.Programming.IsValid(null!));
    }

    [Fact]
    public void IsValid_LongValue_ReturnsFalse()
    {
        Assert.False(TypedNameSpec.Programming
                .IsValid(new string('v', 257)));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\n")]
    [InlineData("a\n")]
    [InlineData("a\r")]
    [InlineData("a\r\n")]
    [InlineData("\na")]
    [InlineData("1")]
    [InlineData("0x1")]
    [InlineData("0X1")]
    [InlineData("a-b")]
    [InlineData("A-B")]
    [InlineData("a.b")]
    [InlineData("a.B")]
    [InlineData(".")]
    public void IsValid_InvalidValue_ReturnsFalse(string invalidValue)
    {
        Assert.False(TypedNameSpec.Programming.IsValid(invalidValue));
    }
}
