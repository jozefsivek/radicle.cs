namespace Radicle.Common.Check.Models;

using System;
using System.Globalization;
using Xunit;

public class CultureInfoParamTest
{
    [Fact]
    public void Param_CultureInfoInput_ReturnsICultureInfoParam()
    {
        Assert.IsAssignableFrom<ICultureInfoParam>(Ensure.Param(CultureInfo.InvariantCulture));
    }

    [Fact]
    public void Optional_stringInput_ReturnsIStringArgument()
    {
        Assert.IsAssignableFrom<ICultureInfoParam>(Ensure.Optional((CultureInfo?)null));
    }

    [Fact]
    public void That_CultureInfoInput_PassesCultureInfo()
    {
        bool isCultureInfo = false;

        Ensure.Param(CultureInfo.InvariantCulture)
                .That(s => isCultureInfo = s is not null);

        Assert.True(isCultureInfo);
    }

    [Fact]
    public void OptionalThat_CultureInfoInput_PassesCultureInfo()
    {
        bool isCultureInfo = false;

        Ensure.Optional(CultureInfo.InvariantCulture)
                .That(s => isCultureInfo = s is not null);

        Assert.True(isCultureInfo);
    }

    [Fact]
    public void Optional_NullInput_Works()
    {
        Ensure.Optional((CultureInfo?)null).NotInvariant();
    }

    [Fact]
    public void NotInvariant_NotInvariantInput_Works()
    {
        Ensure.Param(CultureInfo.GetCultureInfo("nl-BE")).NotInvariant();
    }

    [Fact]
    public void NotInvariant_InvariantInput_Throws()
    {
        ArgumentException exc = Assert.Throws<ArgumentException>(
                () => Ensure.Param(CultureInfo.InvariantCulture).NotInvariant());

        Assert.StartsWith(
                "Parameter 'CultureInfo.InvariantCulture' cannot be invariant culture.",
                exc.Message,
                StringComparison.Ordinal);
    }
}
