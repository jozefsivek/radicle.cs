namespace Radicle.Common.Check;

using Xunit;

public class TypedNameSpecProgrammingOrdinaryTest
{
    [Fact]
    public void Description_HasExpectedValue()
    {
        Assert.Equal(
                "Ordinary programming name: mixed case alphanumerical and '_', non number first character, except programming keywords with length range [1, 256], case sensitive comparison",
                TypedNameSpec.ProgrammingOrdinary.Description);
    }

    [Fact]
    public void IsValid_LongestValidValue_ReturnsTrue()
    {
        Assert.True(TypedNameSpec.ProgrammingOrdinary
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
        Assert.True(TypedNameSpec.ProgrammingOrdinary.IsValid(validValue));
    }

    [Fact]
    public void IsValid_NullValue_ReturnsFalse()
    {
        Assert.False(TypedNameSpec.ProgrammingOrdinary.IsValid(null!));
    }

    [Fact]
    public void IsValid_LongValue_ReturnsFalse()
    {
        Assert.False(TypedNameSpec.ProgrammingOrdinary
                .IsValid(new string('v', 257)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("1")]
    [InlineData("\n")]
    [InlineData("a\n")]
    [InlineData("a\r")]
    [InlineData("a\r\n")]
    [InlineData("\na")]
    [InlineData("a-b")]
    [InlineData("a-B")]
    [InlineData("a.b")]
    [InlineData(".")]
    [InlineData("is")]
    [InlineData("for")]
    [InlineData("TRUE")]
    [InlineData("def")]
    [InlineData("Def")]
    public void IsValid_InvalidValue_ReturnsFalse(string invalidValue)
    {
        Assert.False(TypedNameSpec.ProgrammingOrdinary.IsValid(invalidValue));
    }
}
