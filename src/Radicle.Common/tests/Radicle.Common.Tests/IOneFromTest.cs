namespace Radicle.Common;

using System.Collections.Generic;
using System.Linq;
using Xunit;

public class IOneFromTest
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
    public void TryPick_ReturnsFirst(byte cardinality)
    {
        M0 expected = new(new object());
        M0? actual = default;
        bool result = false;

        switch (cardinality)
        {
            case 1:
                actual = this.b0.FromT0(expected).Pick();
                result = true;
                break;
            case 2:
                result = this.b1.FromT0(expected).TryPick(out actual, out _);
                break;
            case 3:
                result = this.b2.FromT0(expected).TryPick(out actual, out _);
                break;
            case 4:
                result = this.b3.FromT0(expected).TryPick(out actual, out _);
                break;
            case 5:
                result = this.b4.FromT0(expected).TryPick(out actual, out _);
                break;
            case 6:
                result = this.b5.FromT0(expected).TryPick(out actual, out _);
                break;
            case 7:
                result = this.b6.FromT0(expected).TryPick(out actual, out _);
                break;
            case 8:
                result = this.b7.FromT0(expected).TryPick(out actual, out _);
                break;
            case 9:
                result = this.b8.FromT0(expected).TryPick(out actual, out _);
                break;
            case 10:
                result = this.b9.FromT0(expected).TryPick(out actual, out _);
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }

        Assert.Equal(expected, actual);
        Assert.True(result);
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
    public void TryPickT0_ReturnsFirst(byte cardinality)
    {
        M0 expected = new(new object());
        M0? actual = default;
        bool result = false;

        switch (cardinality)
        {
            case 2:
                result = this.b1.FromT0(expected).TryPickT0(out actual, out _);
                break;
            case 3:
                result = this.b2.FromT0(expected).TryPickT0(out actual, out _);
                break;
            case 4:
                result = this.b3.FromT0(expected).TryPickT0(out actual, out _);
                break;
            case 5:
                result = this.b4.FromT0(expected).TryPickT0(out actual, out _);
                break;
            case 6:
                result = this.b5.FromT0(expected).TryPickT0(out actual, out _);
                break;
            case 7:
                result = this.b6.FromT0(expected).TryPickT0(out actual, out _);
                break;
            case 8:
                result = this.b7.FromT0(expected).TryPickT0(out actual, out _);
                break;
            case 9:
                result = this.b8.FromT0(expected).TryPickT0(out actual, out _);
                break;
            case 10:
                result = this.b9.FromT0(expected).TryPickT0(out actual, out _);
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }

        Assert.Equal(expected, actual);
        Assert.True(result);
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
    public void TryPickT1_ReturnsFirst(byte cardinality)
    {
        M1 expected = new(new object());
        M1? actual = default;
        bool result = false;

        switch (cardinality)
        {
            case 2:
                result = this.b1.FromT1(expected).TryPickT1(out actual, out _);
                break;
            case 3:
                result = this.b2.FromT1(expected).TryPickT1(out actual, out _);
                break;
            case 4:
                result = this.b3.FromT1(expected).TryPickT1(out actual, out _);
                break;
            case 5:
                result = this.b4.FromT1(expected).TryPickT1(out actual, out _);
                break;
            case 6:
                result = this.b5.FromT1(expected).TryPickT1(out actual, out _);
                break;
            case 7:
                result = this.b6.FromT1(expected).TryPickT1(out actual, out _);
                break;
            case 8:
                result = this.b7.FromT1(expected).TryPickT1(out actual, out _);
                break;
            case 9:
                result = this.b8.FromT1(expected).TryPickT1(out actual, out _);
                break;
            case 10:
                result = this.b9.FromT1(expected).TryPickT1(out actual, out _);
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }

        Assert.Equal(expected, actual);
        Assert.True(result);
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
    public void TryPickT2_ReturnsFirst(byte cardinality)
    {
        M2 expected = new(new object());
        M2? actual = default;
        bool result = false;

        switch (cardinality)
        {
            case 3:
                result = this.b2.FromT2(expected).TryPickT2(out actual, out _);
                break;
            case 4:
                result = this.b3.FromT2(expected).TryPickT2(out actual, out _);
                break;
            case 5:
                result = this.b4.FromT2(expected).TryPickT2(out actual, out _);
                break;
            case 6:
                result = this.b5.FromT2(expected).TryPickT2(out actual, out _);
                break;
            case 7:
                result = this.b6.FromT2(expected).TryPickT2(out actual, out _);
                break;
            case 8:
                result = this.b7.FromT2(expected).TryPickT2(out actual, out _);
                break;
            case 9:
                result = this.b8.FromT2(expected).TryPickT2(out actual, out _);
                break;
            case 10:
                result = this.b9.FromT2(expected).TryPickT2(out actual, out _);
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }

        Assert.Equal(expected, actual);
        Assert.True(result);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public void TryPickT3_ReturnsFirst(byte cardinality)
    {
        M3 expected = new(new object());
        M3? actual = default;
        bool result = false;

        switch (cardinality)
        {
            case 4:
                result = this.b3.FromT3(expected).TryPickT3(out actual, out _);
                break;
            case 5:
                result = this.b4.FromT3(expected).TryPickT3(out actual, out _);
                break;
            case 6:
                result = this.b5.FromT3(expected).TryPickT3(out actual, out _);
                break;
            case 7:
                result = this.b6.FromT3(expected).TryPickT3(out actual, out _);
                break;
            case 8:
                result = this.b7.FromT3(expected).TryPickT3(out actual, out _);
                break;
            case 9:
                result = this.b8.FromT3(expected).TryPickT3(out actual, out _);
                break;
            case 10:
                result = this.b9.FromT3(expected).TryPickT3(out actual, out _);
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }

        Assert.Equal(expected, actual);
        Assert.True(result);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public void TryPickT4_ReturnsFirst(byte cardinality)
    {
        M4 expected = new(new object());
        M4? actual = default;
        bool result = false;

        switch (cardinality)
        {
            case 5:
                result = this.b4.FromT4(expected).TryPickT4(out actual, out _);
                break;
            case 6:
                result = this.b5.FromT4(expected).TryPickT4(out actual, out _);
                break;
            case 7:
                result = this.b6.FromT4(expected).TryPickT4(out actual, out _);
                break;
            case 8:
                result = this.b7.FromT4(expected).TryPickT4(out actual, out _);
                break;
            case 9:
                result = this.b8.FromT4(expected).TryPickT4(out actual, out _);
                break;
            case 10:
                result = this.b9.FromT4(expected).TryPickT4(out actual, out _);
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }

        Assert.Equal(expected, actual);
        Assert.True(result);
    }

    [Theory]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public void TryPickT5_ReturnsFirst(byte cardinality)
    {
        M5 expected = new(new object());
        M5? actual = default;
        bool result = false;

        switch (cardinality)
        {
            case 6:
                result = this.b5.FromT5(expected).TryPickT5(out actual, out _);
                break;
            case 7:
                result = this.b6.FromT5(expected).TryPickT5(out actual, out _);
                break;
            case 8:
                result = this.b7.FromT5(expected).TryPickT5(out actual, out _);
                break;
            case 9:
                result = this.b8.FromT5(expected).TryPickT5(out actual, out _);
                break;
            case 10:
                result = this.b9.FromT5(expected).TryPickT5(out actual, out _);
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }

        Assert.Equal(expected, actual);
        Assert.True(result);
    }

    [Theory]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public void TryPickT6_ReturnsFirst(byte cardinality)
    {
        M6 expected = new(new object());
        M6? actual = default;
        bool result = false;

        switch (cardinality)
        {
            case 7:
                result = this.b6.FromT6(expected).TryPickT6(out actual, out _);
                break;
            case 8:
                result = this.b7.FromT6(expected).TryPickT6(out actual, out _);
                break;
            case 9:
                result = this.b8.FromT6(expected).TryPickT6(out actual, out _);
                break;
            case 10:
                result = this.b9.FromT6(expected).TryPickT6(out actual, out _);
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }

        Assert.Equal(expected, actual);
        Assert.True(result);
    }

    [Theory]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public void TryPickT7_ReturnsFirst(byte cardinality)
    {
        M7 expected = new(new object());
        M7? actual = default;
        bool result = false;

        switch (cardinality)
        {
            case 8:
                result = this.b7.FromT7(expected).TryPickT7(out actual, out _);
                break;
            case 9:
                result = this.b8.FromT7(expected).TryPickT7(out actual, out _);
                break;
            case 10:
                result = this.b9.FromT7(expected).TryPickT7(out actual, out _);
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }

        Assert.Equal(expected, actual);
        Assert.True(result);
    }

    [Theory]
    [InlineData(9)]
    [InlineData(10)]
    public void TryPickT8_ReturnsFirst(byte cardinality)
    {
        M8 expected = new(new object());
        M8? actual = default;
        bool result = false;

        switch (cardinality)
        {
            case 9:
                result = this.b8.FromT8(expected).TryPickT8(out actual, out _);
                break;
            case 10:
                result = this.b9.FromT8(expected).TryPickT8(out actual, out _);
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }

        Assert.Equal(expected, actual);
        Assert.True(result);
    }

    [Theory]
    [InlineData(10)]
    public void TryPickT9_ReturnsFirst(byte cardinality)
    {
        M9 expected = new(new object());
        M9? actual = default;
        bool result = false;

        switch (cardinality)
        {
            case 10:
                result = this.b9.FromT9(expected).TryPickT9(out actual, out _);
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }

        Assert.Equal(expected, actual);
        Assert.True(result);
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
    public void Switch_Works(byte cardinality)
    {
        MR expected = new(new object());
        HashSet<int> remainder = new(Enumerable.Range(0, cardinality));

        void Test0(M0 o)
        {
            Assert.Equal(expected.Value, o.Value);
            remainder.Remove(0);
        }

        void Test1(M1 o)
        {
            Assert.Equal(expected.Value, o.Value);
            remainder.Remove(1);
        }

        void Test2(M2 o)
        {
            Assert.Equal(expected.Value, o.Value);
            remainder.Remove(2);
        }

        void Test3(M3 o)
        {
            Assert.Equal(expected.Value, o.Value);
            remainder.Remove(3);
        }

        void Test4(M4 o)
        {
            Assert.Equal(expected.Value, o.Value);
            remainder.Remove(4);
        }

        void Test5(M5 o)
        {
            Assert.Equal(expected.Value, o.Value);
            remainder.Remove(5);
        }

        void Test6(M6 o)
        {
            Assert.Equal(expected.Value, o.Value);
            remainder.Remove(6);
        }

        void Test7(M7 o)
        {
            Assert.Equal(expected.Value, o.Value);
            remainder.Remove(7);
        }

        void Test8(M8 o)
        {
            Assert.Equal(expected.Value, o.Value);
            remainder.Remove(8);
        }

        void Test9(M9 o)
        {
            Assert.Equal(expected.Value, o.Value);
            remainder.Remove(9);
        }

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable SA1107 // Code should not contain multiple statements on one line
        switch (cardinality)
        {
            case 1:
                this.b0.FromT0(expected).Switch(Test0);
                break;
            case 2:
                _ = new[]
                {
                    this.b1.FromT0(expected),
                    this.b1.FromT1(expected),
                }.All(of => { of.Switch(Test0, Test1); return true; });
                break;
            case 3:
                _ = new[]
                {
                    this.b2.FromT0(expected),
                    this.b2.FromT1(expected),
                    this.b2.FromT2(expected),
                }.All(of => { of.Switch(Test0, Test1, Test2); return true; });
                break;
            case 4:
                _ = new[]
                {
                    this.b3.FromT0(expected),
                    this.b3.FromT1(expected),
                    this.b3.FromT2(expected),
                    this.b3.FromT3(expected),
                }.All(of => { of.Switch(Test0, Test1, Test2, Test3); return true; });
                break;
            case 5:
                _ = new[]
                {
                    this.b4.FromT0(expected),
                    this.b4.FromT1(expected),
                    this.b4.FromT2(expected),
                    this.b4.FromT3(expected),
                    this.b4.FromT4(expected),
                }.All(of => { of.Switch(Test0, Test1, Test2, Test3, Test4); return true; });
                break;
            case 6:
                _ = new[]
                {
                    this.b5.FromT0(expected),
                    this.b5.FromT1(expected),
                    this.b5.FromT2(expected),
                    this.b5.FromT3(expected),
                    this.b5.FromT4(expected),
                    this.b5.FromT5(expected),
                }.All(of => { of.Switch(Test0, Test1, Test2, Test3, Test4, Test5); return true; });
                break;
            case 7:
                _ = new[]
                {
                    this.b6.FromT0(expected),
                    this.b6.FromT1(expected),
                    this.b6.FromT2(expected),
                    this.b6.FromT3(expected),
                    this.b6.FromT4(expected),
                    this.b6.FromT5(expected),
                    this.b6.FromT6(expected),
                }.All(of => { of.Switch(Test0, Test1, Test2, Test3, Test4, Test5, Test6); return true; });
                break;
            case 8:
                _ = new[]
                {
                    this.b7.FromT0(expected),
                    this.b7.FromT1(expected),
                    this.b7.FromT2(expected),
                    this.b7.FromT3(expected),
                    this.b7.FromT4(expected),
                    this.b7.FromT5(expected),
                    this.b7.FromT6(expected),
                    this.b7.FromT7(expected),
                }.All(of => { of.Switch(Test0, Test1, Test2, Test3, Test4, Test5, Test6, Test7); return true; });
                break;
            case 9:
                _ = new[]
                {
                    this.b8.FromT0(expected),
                    this.b8.FromT1(expected),
                    this.b8.FromT2(expected),
                    this.b8.FromT3(expected),
                    this.b8.FromT4(expected),
                    this.b8.FromT5(expected),
                    this.b8.FromT6(expected),
                    this.b8.FromT7(expected),
                    this.b8.FromT8(expected),
                }.All(of => { of.Switch(Test0, Test1, Test2, Test3, Test4, Test5, Test6, Test7, Test8); return true; });
                break;
            case 10:
                _ = new[]
                {
                    this.b9.FromT0(expected),
                    this.b9.FromT1(expected),
                    this.b9.FromT2(expected),
                    this.b9.FromT3(expected),
                    this.b9.FromT4(expected),
                    this.b9.FromT5(expected),
                    this.b9.FromT6(expected),
                    this.b9.FromT7(expected),
                    this.b9.FromT8(expected),
                    this.b9.FromT9(expected),
                }.All(of => { of.Switch(Test0, Test1, Test2, Test3, Test4, Test5, Test6, Test7, Test8, Test9); return true; });
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }
#pragma warning restore SA1107 // Code should not contain multiple statements on one line
#pragma warning restore IDE0008 // Use explicit type

        Assert.Empty(remainder);
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
    public void Match_Returns(byte cardinality)
    {
        MR expected = new(new object());
        HashSet<int> remainder = new(Enumerable.Range(0, cardinality));
        bool result = false;

        bool Test0(M0 o)
        {
            Assert.Equal(expected.Value, o.Value);
            remainder.Remove(0);
            return true;
        }

        bool Test1(M1 o)
        {
            Assert.Equal(expected.Value, o.Value);
            remainder.Remove(1);
            return true;
        }

        bool Test2(M2 o)
        {
            Assert.Equal(expected.Value, o.Value);
            remainder.Remove(2);
            return true;
        }

        bool Test3(M3 o)
        {
            Assert.Equal(expected.Value, o.Value);
            remainder.Remove(3);
            return true;
        }

        bool Test4(M4 o)
        {
            Assert.Equal(expected.Value, o.Value);
            remainder.Remove(4);
            return true;
        }

        bool Test5(M5 o)
        {
            Assert.Equal(expected.Value, o.Value);
            remainder.Remove(5);
            return true;
        }

        bool Test6(M6 o)
        {
            Assert.Equal(expected.Value, o.Value);
            remainder.Remove(6);
            return true;
        }

        bool Test7(M7 o)
        {
            Assert.Equal(expected.Value, o.Value);
            remainder.Remove(7);
            return true;
        }

        bool Test8(M8 o)
        {
            Assert.Equal(expected.Value, o.Value);
            remainder.Remove(8);
            return true;
        }

        bool Test9(M9 o)
        {
            Assert.Equal(expected.Value, o.Value);
            remainder.Remove(9);
            return true;
        }

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable SA1107 // Code should not contain multiple statements on one line
        switch (cardinality)
        {
            case 1:
                result = this.b0.FromT0(expected).Match(Test0);
                break;
            case 2:
                result = new[]
                {
                    this.b1.FromT0(expected),
                    this.b1.FromT1(expected),
                }.All(of => of.Match(Test0, Test1));
                break;
            case 3:
                result = new[]
                {
                    this.b2.FromT0(expected),
                    this.b2.FromT1(expected),
                    this.b2.FromT2(expected),
                }.All(of => of.Match(Test0, Test1, Test2));
                break;
            case 4:
                result = new[]
                {
                    this.b3.FromT0(expected),
                    this.b3.FromT1(expected),
                    this.b3.FromT2(expected),
                    this.b3.FromT3(expected),
                }.All(of => of.Match(Test0, Test1, Test2, Test3));
                break;
            case 5:
                result = new[]
                {
                    this.b4.FromT0(expected),
                    this.b4.FromT1(expected),
                    this.b4.FromT2(expected),
                    this.b4.FromT3(expected),
                    this.b4.FromT4(expected),
                }.All(of => of.Match(Test0, Test1, Test2, Test3, Test4));
                break;
            case 6:
                result = new[]
                {
                    this.b5.FromT0(expected),
                    this.b5.FromT1(expected),
                    this.b5.FromT2(expected),
                    this.b5.FromT3(expected),
                    this.b5.FromT4(expected),
                    this.b5.FromT5(expected),
                }.All(of => of.Match(Test0, Test1, Test2, Test3, Test4, Test5));
                break;
            case 7:
                result = new[]
                {
                    this.b6.FromT0(expected),
                    this.b6.FromT1(expected),
                    this.b6.FromT2(expected),
                    this.b6.FromT3(expected),
                    this.b6.FromT4(expected),
                    this.b6.FromT5(expected),
                    this.b6.FromT6(expected),
                }.All(of => of.Match(Test0, Test1, Test2, Test3, Test4, Test5, Test6));
                break;
            case 8:
                result = new[]
                {
                    this.b7.FromT0(expected),
                    this.b7.FromT1(expected),
                    this.b7.FromT2(expected),
                    this.b7.FromT3(expected),
                    this.b7.FromT4(expected),
                    this.b7.FromT5(expected),
                    this.b7.FromT6(expected),
                    this.b7.FromT7(expected),
                }.All(of => of.Match(Test0, Test1, Test2, Test3, Test4, Test5, Test6, Test7));
                break;
            case 9:
                result = new[]
                {
                    this.b8.FromT0(expected),
                    this.b8.FromT1(expected),
                    this.b8.FromT2(expected),
                    this.b8.FromT3(expected),
                    this.b8.FromT4(expected),
                    this.b8.FromT5(expected),
                    this.b8.FromT6(expected),
                    this.b8.FromT7(expected),
                    this.b8.FromT8(expected),
                }.All(of => of.Match(Test0, Test1, Test2, Test3, Test4, Test5, Test6, Test7, Test8));
                break;
            case 10:
                result = new[]
                {
                    this.b9.FromT0(expected),
                    this.b9.FromT1(expected),
                    this.b9.FromT2(expected),
                    this.b9.FromT3(expected),
                    this.b9.FromT4(expected),
                    this.b9.FromT5(expected),
                    this.b9.FromT6(expected),
                    this.b9.FromT7(expected),
                    this.b9.FromT8(expected),
                    this.b9.FromT9(expected),
                }.All(of => of.Match(Test0, Test1, Test2, Test3, Test4, Test5, Test6, Test7, Test8, Test9));
                break;
            default:
                Assert.Fail("Wrong cardinality");
                break;
        }
#pragma warning restore SA1107 // Code should not contain multiple statements on one line
#pragma warning restore IDE0008 // Use explicit type

        Assert.True(result);
        Assert.Empty(remainder);
    }

    private class MR
    {
        public MR(object v)
        {
            this.Value = v;
        }

        public object Value { get; }
    }

    private class M
    {
        public M(object v)
        {
            this.Value = v;
        }

        public object Value { get; }
    }

    private class M0 : M
    {
        public M0(object v)
            : base(v)
        {
        }

        public static implicit operator M0(MR v)
        {
            return new M0(v.Value);
        }
    }

    private class M1 : M
    {
        public M1(object v)
            : base(v)
        {
        }

        public static implicit operator M1(MR v)
        {
            return new M1(v.Value);
        }
    }

    private class M2 : M
    {
        public M2(object v)
            : base(v)
        {
        }

        public static implicit operator M2(MR v)
        {
            return new M2(v.Value);
        }
    }

    private class M3 : M
    {
        public M3(object v)
            : base(v)
        {
        }

        public static implicit operator M3(MR v)
        {
            return new M3(v.Value);
        }
    }

    private class M4 : M
    {
        public M4(object v)
            : base(v)
        {
        }

        public static implicit operator M4(MR v)
        {
            return new M4(v.Value);
        }
    }

    private class M5 : M
    {
        public M5(object v)
            : base(v)
        {
        }

        public static implicit operator M5(MR v)
        {
            return new M5(v.Value);
        }
    }

    private class M6 : M
    {
        public M6(object v)
            : base(v)
        {
        }

        public static implicit operator M6(MR v)
        {
            return new M6(v.Value);
        }
    }

    private class M7 : M
    {
        public M7(object v)
            : base(v)
        {
        }

        public static implicit operator M7(MR v)
        {
            return new M7(v.Value);
        }
    }

    private class M8 : M
    {
        public M8(object v)
            : base(v)
        {
        }

        public static implicit operator M8(MR v)
        {
            return new M8(v.Value);
        }
    }

    private class M9 : M
    {
        public M9(object v)
            : base(v)
        {
        }

        public static implicit operator M9(MR v)
        {
            return new M9(v.Value);
        }
    }
}
