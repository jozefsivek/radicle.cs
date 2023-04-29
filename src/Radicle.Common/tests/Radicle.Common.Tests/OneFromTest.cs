namespace Radicle.Common;

using System;
using Xunit;

public class OneFromTest
{
    private readonly OneFrom<M0>.Builder b0 = OneFrom.CreateBuilder<M0>();
    private readonly OneFrom<M0, M1>.Builder b1 = OneFrom.CreateBuilder<M0, M1>();
    private readonly OneFrom<M0, M1, M2>.Builder b2 = OneFrom.CreateBuilder<M0, M1, M2>();
    private readonly OneFrom<M0, M1, M2, M3>.Builder b3 = OneFrom.CreateBuilder<M0, M1, M2, M3>();
    private readonly OneFrom<M0, M1, M2, M3, M4>.Builder b4 = OneFrom.CreateBuilder<M0, M1, M2, M3, M4>();
    private readonly OneFrom<M0, M1, M2, M3, M4, M5>.Builder b5 = OneFrom.CreateBuilder<M0, M1, M2, M3, M4, M5>();
    private readonly OneFrom<M0, M1, M2, M3, M4, M5, M6>.Builder b6 = OneFrom.CreateBuilder<M0, M1, M2, M3, M4, M5, M6>();
    private readonly OneFrom<M0, M1, M2, M3, M4, M5, M6, M7>.Builder b7 = OneFrom.CreateBuilder<M0, M1, M2, M3, M4, M5, M6, M7>();
    private readonly OneFrom<M0, M1, M2, M3, M4, M5, M6, M7, M8>.Builder b8 = OneFrom.CreateBuilder<M0, M1, M2, M3, M4, M5, M6, M7, M8>();
    private readonly OneFrom<M0, M1, M2, M3, M4, M5, M6, M7, M8, M9>.Builder b9 = OneFrom.CreateBuilder<M0, M1, M2, M3, M4, M5, M6, M7, M8, M9>();

    [Fact]
    public void FromTx_NullValue_Throws()
    {
        ThrowsNull(() => this.b0.FromT0(null!));

        ThrowsNull(() => this.b1.FromT0(null!));
        ThrowsNull(() => this.b1.FromT1(null!));

        ThrowsNull(() => this.b2.FromT0(null!));
        ThrowsNull(() => this.b2.FromT1(null!));
        ThrowsNull(() => this.b2.FromT2(null!));

        ThrowsNull(() => this.b3.FromT0(null!));
        ThrowsNull(() => this.b3.FromT1(null!));
        ThrowsNull(() => this.b3.FromT2(null!));
        ThrowsNull(() => this.b3.FromT3(null!));

        ThrowsNull(() => this.b4.FromT0(null!));
        ThrowsNull(() => this.b4.FromT1(null!));
        ThrowsNull(() => this.b4.FromT2(null!));
        ThrowsNull(() => this.b4.FromT3(null!));
        ThrowsNull(() => this.b4.FromT4(null!));

        ThrowsNull(() => this.b5.FromT0(null!));
        ThrowsNull(() => this.b5.FromT1(null!));
        ThrowsNull(() => this.b5.FromT2(null!));
        ThrowsNull(() => this.b5.FromT3(null!));
        ThrowsNull(() => this.b5.FromT4(null!));
        ThrowsNull(() => this.b5.FromT5(null!));

        ThrowsNull(() => this.b6.FromT0(null!));
        ThrowsNull(() => this.b6.FromT1(null!));
        ThrowsNull(() => this.b6.FromT2(null!));
        ThrowsNull(() => this.b6.FromT3(null!));
        ThrowsNull(() => this.b6.FromT4(null!));
        ThrowsNull(() => this.b6.FromT5(null!));
        ThrowsNull(() => this.b6.FromT6(null!));

        ThrowsNull(() => this.b7.FromT0(null!));
        ThrowsNull(() => this.b7.FromT1(null!));
        ThrowsNull(() => this.b7.FromT2(null!));
        ThrowsNull(() => this.b7.FromT3(null!));
        ThrowsNull(() => this.b7.FromT4(null!));
        ThrowsNull(() => this.b7.FromT5(null!));
        ThrowsNull(() => this.b7.FromT6(null!));
        ThrowsNull(() => this.b7.FromT7(null!));

        ThrowsNull(() => this.b8.FromT0(null!));
        ThrowsNull(() => this.b8.FromT1(null!));
        ThrowsNull(() => this.b8.FromT2(null!));
        ThrowsNull(() => this.b8.FromT3(null!));
        ThrowsNull(() => this.b8.FromT4(null!));
        ThrowsNull(() => this.b8.FromT5(null!));
        ThrowsNull(() => this.b8.FromT6(null!));
        ThrowsNull(() => this.b8.FromT7(null!));
        ThrowsNull(() => this.b8.FromT8(null!));

        ThrowsNull(() => this.b9.FromT0(null!));
        ThrowsNull(() => this.b9.FromT1(null!));
        ThrowsNull(() => this.b9.FromT2(null!));
        ThrowsNull(() => this.b9.FromT3(null!));
        ThrowsNull(() => this.b9.FromT4(null!));
        ThrowsNull(() => this.b9.FromT5(null!));
        ThrowsNull(() => this.b9.FromT6(null!));
        ThrowsNull(() => this.b9.FromT7(null!));
        ThrowsNull(() => this.b9.FromT8(null!));
        ThrowsNull(() => this.b9.FromT9(null!));
    }

    [Fact]
    public void From_NullValue_Throws()
    {
        ThrowsNull(() => this.b0.From(null!));
        ThrowsNull(() => this.b1.From(null!));
        ThrowsNull(() => this.b2.From(null!));
        ThrowsNull(() => this.b3.From(null!));
        ThrowsNull(() => this.b4.From(null!));
        ThrowsNull(() => this.b5.From(null!));
        ThrowsNull(() => this.b6.From(null!));
        ThrowsNull(() => this.b7.From(null!));
        ThrowsNull(() => this.b8.From(null!));
        ThrowsNull(() => this.b9.From(null!));
    }

    [Fact]
    public void From_NonMatchingType_Throws()
    {
        Assert.Throws<ArgumentException>(() => this.b0.From(new object()));
        Assert.Throws<ArgumentException>(() => this.b1.From(new object()));
        Assert.Throws<ArgumentException>(() => this.b2.From(new object()));
        Assert.Throws<ArgumentException>(() => this.b3.From(new object()));
        Assert.Throws<ArgumentException>(() => this.b4.From(new object()));
        Assert.Throws<ArgumentException>(() => this.b5.From(new object()));
        Assert.Throws<ArgumentException>(() => this.b6.From(new object()));
        Assert.Throws<ArgumentException>(() => this.b7.From(new object()));
        Assert.Throws<ArgumentException>(() => this.b8.From(new object()));
        Assert.Throws<ArgumentException>(() => this.b9.From(new object()));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public void FromT0_ReturnsExpected(byte cardinality)
    {
        IOneFrom[] o = Array.Empty<IOneFrom>();
        M0 expected = new();

        switch (cardinality)
        {
            case 1:
                o = new[]
                {
                    this.b0.FromT0(expected),
                    this.b0.From(expected),
                };
                break;
            case 2:
                o = new[]
                {
                    this.b1.FromT0(expected),
                    this.b1.From(expected),
                };
                break;
            case 3:
                o = new[]
                {
                    this.b2.FromT0(expected),
                    this.b2.From(expected),
                };
                break;
            case 4:
                o = new[]
                {
                    this.b3.FromT0(expected),
                    this.b3.From(expected),
                };
                break;
            case 5:
                o = new[]
                {
                    this.b4.FromT0(expected),
                    this.b4.From(expected),
                };
                break;
            case 6:
                o = new[]
                {
                    this.b5.FromT0(expected),
                    this.b5.From(expected),
                };
                break;
            case 7:
                o = new[]
                {
                    this.b6.FromT0(expected),
                    this.b6.From(expected),
                };
                break;
            case 8:
                o = new[]
                {
                    this.b7.FromT0(expected),
                    this.b7.From(expected),
                };
                break;
            case 9:
                o = new[]
                {
                    this.b8.FromT0(expected),
                    this.b8.From(expected),
                };
                break;
            case 10:
                o = new[]
                {
                    this.b9.FromT0(expected),
                    this.b9.From(expected),
                };
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }

        Assert.Equal(2, o.Length);

        foreach (IOneFrom oi in o)
        {
            Assert.Equal(0, oi.SumTypeValueIndex);
            Assert.Equal(expected, oi.SumTypeValue);
        }
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public void FromT1_ReturnsExpected(byte cardinality)
    {
        IOneFrom[] o = Array.Empty<IOneFrom>();
        M1 expected = new();

        switch (cardinality)
        {
            case 2:
                o = new[]
                {
                    this.b1.FromT1(expected),
                    this.b1.From(expected),
                };
                break;
            case 3:
                o = new[]
                {
                    this.b2.FromT1(expected),
                    this.b2.From(expected),
                };
                break;
            case 4:
                o = new[]
                {
                    this.b3.FromT1(expected),
                    this.b3.From(expected),
                };
                break;
            case 5:
                o = new[]
                {
                    this.b4.FromT1(expected),
                    this.b4.From(expected),
                };
                break;
            case 6:
                o = new[]
                {
                    this.b5.FromT1(expected),
                    this.b5.From(expected),
                };
                break;
            case 7:
                o = new[]
                {
                    this.b6.FromT1(expected),
                    this.b6.From(expected),
                };
                break;
            case 8:
                o = new[]
                {
                    this.b7.FromT1(expected),
                    this.b7.From(expected),
                };
                break;
            case 9:
                o = new[]
                {
                    this.b8.FromT1(expected),
                    this.b8.From(expected),
                };
                break;
            case 10:
                o = new[]
                {
                    this.b9.FromT1(expected),
                    this.b9.From(expected),
                };
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }

        Assert.Equal(2, o.Length);

        foreach (IOneFrom oi in o)
        {
            Assert.Equal(1, oi.SumTypeValueIndex);
            Assert.Equal(expected, oi.SumTypeValue);
        }
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public void FromT2_ReturnsExpected(byte cardinality)
    {
        IOneFrom[] o = Array.Empty<IOneFrom>();
        M2 expected = new();

        switch (cardinality)
        {
            case 3:
                o = new[]
                {
                    this.b2.FromT2(expected),
                    this.b2.From(expected),
                };
                break;
            case 4:
                o = new[]
                {
                    this.b3.FromT2(expected),
                    this.b3.From(expected),
                };
                break;
            case 5:
                o = new[]
                {
                    this.b4.FromT2(expected),
                    this.b4.From(expected),
                };
                break;
            case 6:
                o = new[]
                {
                    this.b5.FromT2(expected),
                    this.b5.From(expected),
                };
                break;
            case 7:
                o = new[]
                {
                    this.b6.FromT2(expected),
                    this.b6.From(expected),
                };
                break;
            case 8:
                o = new[]
                {
                    this.b7.FromT2(expected),
                    this.b7.From(expected),
                };
                break;
            case 9:
                o = new[]
                {
                    this.b8.FromT2(expected),
                    this.b8.From(expected),
                };
                break;
            case 10:
                o = new[]
                {
                    this.b9.FromT2(expected),
                    this.b9.From(expected),
                };
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }

        Assert.Equal(2, o.Length);

        foreach (IOneFrom oi in o)
        {
            Assert.Equal(2, oi.SumTypeValueIndex);
            Assert.Equal(expected, oi.SumTypeValue);
        }
    }

    [Theory]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public void FromT3_ReturnsExpected(byte cardinality)
    {
        IOneFrom[] o = Array.Empty<IOneFrom>();
        M3 expected = new();

        switch (cardinality)
        {
            case 4:
                o = new[]
                {
                    this.b3.FromT3(expected),
                    this.b3.From(expected),
                };
                break;
            case 5:
                o = new[]
                {
                    this.b4.FromT3(expected),
                    this.b4.From(expected),
                };
                break;
            case 6:
                o = new[]
                {
                    this.b5.FromT3(expected),
                    this.b5.From(expected),
                };
                break;
            case 7:
                o = new[]
                {
                    this.b6.FromT3(expected),
                    this.b6.From(expected),
                };
                break;
            case 8:
                o = new[]
                {
                    this.b7.FromT3(expected),
                    this.b7.From(expected),
                };
                break;
            case 9:
                o = new[]
                {
                    this.b8.FromT3(expected),
                    this.b8.From(expected),
                };
                break;
            case 10:
                o = new[]
                {
                    this.b9.FromT3(expected),
                    this.b9.From(expected),
                };
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }

        Assert.Equal(2, o.Length);

        foreach (IOneFrom oi in o)
        {
            Assert.Equal(3, oi.SumTypeValueIndex);
            Assert.Equal(expected, oi.SumTypeValue);
        }
    }

    [Theory]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public void FromT4_ReturnsExpected(byte cardinality)
    {
        IOneFrom[] o = Array.Empty<IOneFrom>();
        M4 expected = new();

        switch (cardinality)
        {
            case 5:
                o = new[]
                {
                    this.b4.FromT4(expected),
                    this.b4.From(expected),
                };
                break;
            case 6:
                o = new[]
                {
                    this.b5.FromT4(expected),
                    this.b5.From(expected),
                };
                break;
            case 7:
                o = new[]
                {
                    this.b6.FromT4(expected),
                    this.b6.From(expected),
                };
                break;
            case 8:
                o = new[]
                {
                    this.b7.FromT4(expected),
                    this.b7.From(expected),
                };
                break;
            case 9:
                o = new[]
                {
                    this.b8.FromT4(expected),
                    this.b8.From(expected),
                };
                break;
            case 10:
                o = new[]
                {
                    this.b9.FromT4(expected),
                    this.b9.From(expected),
                };
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }

        Assert.Equal(2, o.Length);

        foreach (IOneFrom oi in o)
        {
            Assert.Equal(4, oi.SumTypeValueIndex);
            Assert.Equal(expected, oi.SumTypeValue);
        }
    }

    [Theory]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public void FromT5_ReturnsExpected(byte cardinality)
    {
        IOneFrom[] o = Array.Empty<IOneFrom>();
        M5 expected = new();

        switch (cardinality)
        {
            case 6:
                o = new[]
                {
                    this.b5.FromT5(expected),
                    this.b5.From(expected),
                };
                break;
            case 7:
                o = new[]
                {
                    this.b6.FromT5(expected),
                    this.b6.From(expected),
                };
                break;
            case 8:
                o = new[]
                {
                    this.b7.FromT5(expected),
                    this.b7.From(expected),
                };
                break;
            case 9:
                o = new[]
                {
                    this.b8.FromT5(expected),
                    this.b8.From(expected),
                };
                break;
            case 10:
                o = new[]
                {
                    this.b9.FromT5(expected),
                    this.b9.From(expected),
                };
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }

        Assert.Equal(2, o.Length);

        foreach (IOneFrom oi in o)
        {
            Assert.Equal(5, oi.SumTypeValueIndex);
            Assert.Equal(expected, oi.SumTypeValue);
        }
    }

    [Theory]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public void FromT6_ReturnsExpected(byte cardinality)
    {
        IOneFrom[] o = Array.Empty<IOneFrom>();
        M6 expected = new();

        switch (cardinality)
        {
            case 7:
                o = new[]
                {
                    this.b6.FromT6(expected),
                    this.b6.From(expected),
                };
                break;
            case 8:
                o = new[]
                {
                    this.b7.FromT6(expected),
                    this.b7.From(expected),
                };
                break;
            case 9:
                o = new[]
                {
                    this.b8.FromT6(expected),
                    this.b8.From(expected),
                };
                break;
            case 10:
                o = new[]
                {
                    this.b9.FromT6(expected),
                    this.b9.From(expected),
                };
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }

        Assert.Equal(2, o.Length);

        foreach (IOneFrom oi in o)
        {
            Assert.Equal(6, oi.SumTypeValueIndex);
            Assert.Equal(expected, oi.SumTypeValue);
        }
    }

    [Theory]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public void FromT7_ReturnsExpected(byte cardinality)
    {
        IOneFrom[] o = Array.Empty<IOneFrom>();
        M7 expected = new();

        switch (cardinality)
        {
            case 8:
                o = new[]
                {
                    this.b7.FromT7(expected),
                    this.b7.From(expected),
                };
                break;
            case 9:
                o = new[]
                {
                    this.b8.FromT7(expected),
                    this.b8.From(expected),
                };
                break;
            case 10:
                o = new[]
                {
                    this.b9.FromT7(expected),
                    this.b9.From(expected),
                };
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }

        Assert.Equal(2, o.Length);

        foreach (IOneFrom oi in o)
        {
            Assert.Equal(7, oi.SumTypeValueIndex);
            Assert.Equal(expected, oi.SumTypeValue);
        }
    }

    [Theory]
    [InlineData(9)]
    [InlineData(10)]
    public void FromT8_ReturnsExpected(byte cardinality)
    {
        IOneFrom[] o = Array.Empty<IOneFrom>();
        M8 expected = new();

        switch (cardinality)
        {
            case 9:
                o = new[]
                {
                    this.b8.FromT8(expected),
                    this.b8.From(expected),
                };
                break;
            case 10:
                o = new[]
                {
                    this.b9.FromT8(expected),
                    this.b9.From(expected),
                };
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }

        Assert.Equal(2, o.Length);

        foreach (IOneFrom oi in o)
        {
            Assert.Equal(8, oi.SumTypeValueIndex);
            Assert.Equal(expected, oi.SumTypeValue);
        }
    }

    [Theory]
    [InlineData(10)]
    public void FromT9_ReturnsExpected(byte cardinality)
    {
        IOneFrom[] o = Array.Empty<IOneFrom>();
        M9 expected = new();

        switch (cardinality)
        {
            case 10:
                o = new[]
                {
                    this.b9.FromT9(expected),
                    this.b9.From(expected),
                };
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }

        Assert.Equal(2, o.Length);

        foreach (IOneFrom oi in o)
        {
            Assert.Equal(9, oi.SumTypeValueIndex);
            Assert.Equal(expected, oi.SumTypeValue);
        }
    }

    private static ArgumentNullException ThrowsNull(Func<object?> predicate)
    {
        return Assert.Throws<ArgumentNullException>(predicate);
    }

    private class M0
    {
    }

    private class M1
    {
    }

    private class M2
    {
    }

    private class M3
    {
    }

    private class M4
    {
    }

    private class M5
    {
    }

    private class M6
    {
    }

    private class M7
    {
    }

    private class M8
    {
    }

    private class M9
    {
    }
}
