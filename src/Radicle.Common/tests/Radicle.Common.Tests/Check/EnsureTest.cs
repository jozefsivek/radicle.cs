namespace Radicle.Common.Check;

using System;
using Xunit;

public class EnsureTest
{
    [Fact]
    public void NotNull_NullParam_Throws()
    {
        object? param = null;

        ArgumentNullException exc = Assert.Throws<ArgumentNullException>(
                () => Ensure.NotNull(param!));

        Assert.Equal(
                "Parameter 'param' cannot be null. (Parameter 'param')",
                exc.Message);
    }

    [Fact]
    public void NotNull_NonNullParam_DoesNotThrow()
    {
        Ensure.NotNull(new object());
    }

    [Fact]
    public void NotNull_NullParam_Works()
    {
        Ensure.NotNull(new object(), parameterName: null);
    }
}
