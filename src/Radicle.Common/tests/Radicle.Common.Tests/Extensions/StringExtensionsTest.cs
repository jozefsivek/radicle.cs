namespace Radicle.Common.Extensions;

using System;
using Xunit;

public class StringExtensionsTest
{
    [Fact]
    public void Ellipsis_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ((string)null!).Ellipsis());
    }

    [Fact]
    public void Ellipsis_NonPositiveTrim_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => string.Empty.Ellipsis(trim: 0));
    }

    [Fact]
    public void Ellipsis_TooLongSuffix_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
                () => string.Empty.Ellipsis(
                    trim: 1,
                    fuzziness: 1,
                    suffix: "way too long suffix"));
    }

    /*
     "Red brown fox jumped"
      123456789_123456789_
     */
    [Theory]
    [InlineData("Red brown fox jumped", 18, 2)]
    [InlineData("Red brown fox j...", 18, 1)]
    [InlineData("Red brown fox j...", 18, 0)]
    [InlineData("Red brown fox jumped", 1, 19)]
    [InlineData("R", 1, 0)]
    [InlineData("R.", 1, 1)]
    [InlineData("...", 2, 1)]
    [InlineData("R...", 3, 1)]
    [InlineData("R...", 1, 3)]
    public void Ellipsis_Works(
            string expected,
            ushort trim,
            ushort fuziness)
    {
        const string str = "Red brown fox jumped";

        Assert.Equal(
                expected,
                str.Ellipsis(
                    trim: trim,
                    fuzziness: fuziness));
    }
}
