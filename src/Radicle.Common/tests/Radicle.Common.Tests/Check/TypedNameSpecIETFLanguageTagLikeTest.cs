namespace Radicle.Common.Check;

using Radicle.Common.Extensions;
using Xunit;

public class TypedNameSpecIETFLanguageTagLikeTest
{
    [Fact]
    public void Description_HasExpectedValue()
    {
        Assert.Equal(
                "IETF language tag: mixed case alphanumerical sub-tags 1 to 8 characters long separated by '-', first sub-tag can not contain numbers and has to be at least 2 characters long with length range [2, 256], case sensitive comparison",
                TypedNameSpec.IETFLanguageTagLike.Description);
    }

    [Fact]
    public void IsValid_LongestValidValue_ReturnsTrue()
    {
        string value = string.Join("-", new[] { "abcdefgh" }.Mul(28)) + "-abcd";

        Assert.True(TypedNameSpec.IETFLanguageTagLike
                .IsValid(value));
    }

    [Theory]
    [InlineData("en")]
    [InlineData("sjn")]
    [InlineData("en-US")]
    [InlineData("nl-BE")]
    [InlineData("NL-be")]
    public void IsValid_ValidValue_ReturnsTrue(string validValue)
    {
        Assert.True(TypedNameSpec.IETFLanguageTagLike.IsValid(validValue));
    }

    [Fact]
    public void IsValid_NullValue_ReturnsFalse()
    {
        Assert.False(TypedNameSpec.IETFLanguageTagLike.IsValid(null!));
    }

    [Fact]
    public void IsValid_LongValue_ReturnsFalse()
    {
        Assert.False(TypedNameSpec.IETFLanguageTagLike
                .IsValid(string.Join("-", "abcdefgh".Mul(28)) + "-abcde"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("\n")]
    [InlineData("en\n")]
    [InlineData("en\r")]
    [InlineData("en\r\r")]
    [InlineData("\nen")]
    [InlineData("9")]
    [InlineData("toolongsubtag")]
    [InlineData("i")]
    [InlineData("not_allowed characters")]
    public void IsValid_InvalidValue_ReturnsFalse(string invalidValue)
    {
        Assert.False(TypedNameSpec.IETFLanguageTagLike.IsValid(invalidValue));
    }
}
