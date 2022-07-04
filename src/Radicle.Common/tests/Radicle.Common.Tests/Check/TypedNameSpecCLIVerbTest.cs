namespace Radicle.Common.Check;

using Xunit;

public class TypedNameSpecCLIVerbTest
{
    [Fact]
    public void Description_HasExpectedValue()
    {
        Assert.Equal(
                "CLI verb: mixed case alphanumerical non empty sub-tags separated by '-', no leading or trailing '-' with length range [1, 256], case sensitive comparison",
                TypedNameSpec.CLIVerb.Description);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("do")]
    [InlineData("do-this")]
    [InlineData("A")]
    [InlineData("Do")]
    [InlineData("DO-THIS")]
    public void IsValid_ValidValue_ReturnsTrue(string validValue)
    {
        Assert.True(TypedNameSpec.CLIVerb.IsValid(validValue));
    }

    [Fact]
    public void IsValid_NullValue_ReturnsFalse()
    {
        Assert.False(TypedNameSpec.CLIVerb.IsValid(null!));
    }

    [Theory]
    [InlineData("-")]
    [InlineData("\n")]
    [InlineData("a\n")]
    [InlineData("a\r")]
    [InlineData("a\r\n")]
    [InlineData("\na")]
    [InlineData("-foo")]
    [InlineData("--foo")]
    [InlineData("bar-")]
    [InlineData("foo--bar")]
    [InlineData("foo bar")]
    [InlineData("-Foo")]
    [InlineData("--Foo")]
    [InlineData("BAR-")]
    [InlineData("FOO--bar")]
    [InlineData("foo BAR")]
    public void IsValid_InvalidValue_ReturnsFalse(string invalidValue)
    {
        Assert.False(TypedNameSpec.CLIVerb.IsValid(invalidValue));
    }
}
