namespace Radicle.Common.Check.Models;

using System;
using Xunit;

public class NumberLikeParamTest
{
    [Fact]
    public void Param_NumberLikeInput_ReturnsINumberLikeParam()
    {
        Assert.IsAssignableFrom<INumberLikeParam<long>>(Ensure.Param(0L));
        Assert.IsAssignableFrom<INumberLikeParam<ulong>>(Ensure.Param(0uL));
        Assert.IsAssignableFrom<INumberLikeParam<int>>(Ensure.Param(0));
        Assert.IsAssignableFrom<INumberLikeParam<uint>>(Ensure.Param(0u));
        Assert.IsAssignableFrom<INumberLikeParam<short>>(Ensure.Param((short)0));
        Assert.IsAssignableFrom<INumberLikeParam<ushort>>(Ensure.Param((ushort)0));
        Assert.IsAssignableFrom<INumberLikeParam<sbyte>>(Ensure.Param((sbyte)0));
        Assert.IsAssignableFrom<INumberLikeParam<byte>>(Ensure.Param((byte)0));
        Assert.IsAssignableFrom<INumberLikeParam<decimal>>(Ensure.Param(0m));
        Assert.IsAssignableFrom<INumberLikeParam<double>>(Ensure.Param(0d));
        Assert.IsAssignableFrom<INumberLikeParam<float>>(Ensure.Param(0f));
        Assert.IsAssignableFrom<INumberLikeParam<TimeSpan>>(Ensure.Param(TimeSpan.Zero));
        Assert.IsAssignableFrom<INumberLikeParam<DateTime>>(Ensure.Param(DateTime.UnixEpoch));
        Assert.IsAssignableFrom<INumberLikeParam<DateTimeOffset>>(Ensure.Param(DateTimeOffset.UnixEpoch));
    }

    [Fact]
    public void Optional_NumberLikeInput_ReturnsINumberLikeParam()
    {
        Assert.IsAssignableFrom<INumberLikeParam<long>>(Ensure.Optional((long?)null));
        Assert.IsAssignableFrom<INumberLikeParam<ulong>>(Ensure.Optional((ulong?)null));
        Assert.IsAssignableFrom<INumberLikeParam<int>>(Ensure.Optional((int?)null));
        Assert.IsAssignableFrom<INumberLikeParam<uint>>(Ensure.Optional((uint?)null));
        Assert.IsAssignableFrom<INumberLikeParam<short>>(Ensure.Optional((short?)null));
        Assert.IsAssignableFrom<INumberLikeParam<ushort>>(Ensure.Optional((ushort?)null));
        Assert.IsAssignableFrom<INumberLikeParam<sbyte>>(Ensure.Optional((sbyte?)null));
        Assert.IsAssignableFrom<INumberLikeParam<byte>>(Ensure.Optional((byte?)null));
        Assert.IsAssignableFrom<INumberLikeParam<decimal>>(Ensure.Optional((decimal?)null));
        Assert.IsAssignableFrom<INumberLikeParam<double>>(Ensure.Optional((double?)null));
        Assert.IsAssignableFrom<INumberLikeParam<float>>(Ensure.Optional((float?)null));
        Assert.IsAssignableFrom<INumberLikeParam<TimeSpan>>(Ensure.Optional((TimeSpan?)null));
        Assert.IsAssignableFrom<INumberLikeParam<DateTime>>(Ensure.Optional((DateTime?)null));
        Assert.IsAssignableFrom<INumberLikeParam<DateTimeOffset>>(Ensure.Optional((DateTimeOffset?)null));
    }

    /*
     * TODO: Smoke tests, not all numeric types are tested
     */

    [Fact]
    public void GreaterThan_ValidParam_Works()
    {
        Ensure.Param(14).GreaterThan(3);
    }

    [Theory]
    [InlineData(3u, 3u)]
    [InlineData(3u, 14u)]
    [InlineData(uint.MinValue, uint.MaxValue)]
    public void GreaterThan_InvalidParam_Throws(uint value, uint min)
    {
        ArgumentOutOfRangeException exc = Assert.Throws<ArgumentOutOfRangeException>(
                () => Ensure.Param(value).GreaterThan(min));

        Assert.StartsWith(
                $"Parameter 'value' with value: {value} must be greater than {min}.",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Fact]
    public void GreaterOrEqualThan_ValidParam_Works()
    {
        Ensure.Param(14).GreaterThanOrEqual(14);
    }

    [Theory]
    [InlineData(3u, 4u)]
    [InlineData(3u, 14u)]
    [InlineData(uint.MinValue, uint.MaxValue)]
    public void GreaterOrEqualThan_InvalidParam_Throws(uint value, uint min)
    {
        ArgumentOutOfRangeException exc = Assert.Throws<ArgumentOutOfRangeException>(
                () => Ensure.Param(value).GreaterThanOrEqual(min));

        Assert.StartsWith(
                $"Parameter 'value' with value: {value} must be greater or equal to {min}.",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Fact]
    public void LowerThan_ValidParam_Works()
    {
        Ensure.Param(3L).LowerThan(14L);
    }

    [Theory]
    [InlineData(3uL, 3uL)]
    [InlineData(ulong.MaxValue, ulong.MinValue)]
    [InlineData(14uL, 3uL)]
    public void LowerThan_InvalidParam_Throws(ulong value, ulong max)
    {
        ArgumentOutOfRangeException exc = Assert.Throws<ArgumentOutOfRangeException>(
                () => Ensure.Param(value).LowerThan(max));

        Assert.StartsWith(
                $"Parameter 'value' with value: {value} must be lesser than {max}.",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Fact]
    public void LowerOrEqualThan_ValidParam_Works()
    {
        Ensure.Param(4L).LowerThanOrEqual(4L);
    }

    [Theory]
    [InlineData(3uL, 2uL)]
    [InlineData(ulong.MaxValue, ulong.MinValue)]
    [InlineData(14uL, 3uL)]
    public void LowerOrEqualThan_InvalidParam_Throws(ulong value, ulong max)
    {
        ArgumentOutOfRangeException exc = Assert.Throws<ArgumentOutOfRangeException>(
                () => Ensure.Param(value).LowerThanOrEqual(max));

        Assert.StartsWith(
                $"Parameter 'value' with value: {value} must be lesser or equal to {max}.",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Fact]
    public void NonNegative_ValidInput_Works()
    {
        Ensure.Param(0L).NonNegative();
        Ensure.Param(0uL).NonNegative();
        Ensure.Param(0).NonNegative();
        Ensure.Param(0u).NonNegative();
        Ensure.Param((short)0).NonNegative();
        Ensure.Param((ushort)0).NonNegative();
        Ensure.Param((sbyte)0).NonNegative();
        Ensure.Param((byte)0).NonNegative();
        Ensure.Param(0m).NonNegative();
        Ensure.Param(0d).NonNegative();
        Ensure.Param(0f).NonNegative();
        Ensure.Param(TimeSpan.Zero).NonNegative();
        Ensure.Param(DateTime.UnixEpoch).NonNegative();
        Ensure.Param(DateTimeOffset.UnixEpoch).NonNegative();

        Ensure.Param(1L).NonNegative();
        Ensure.Param(1uL).NonNegative();
        Ensure.Param(1).NonNegative();
        Ensure.Param(1u).NonNegative();
        Ensure.Param((short)1).NonNegative();
        Ensure.Param((ushort)1).NonNegative();
        Ensure.Param((sbyte)1).NonNegative();
        Ensure.Param((byte)1).NonNegative();
        Ensure.Param(.1m).NonNegative();
        Ensure.Param(.1d).NonNegative();
        Ensure.Param(.1f).NonNegative();
        Ensure.Param(TimeSpan.FromTicks(1)).NonNegative();
        Ensure.Param(DateTime.UnixEpoch + TimeSpan.FromTicks(1)).NonNegative();
        Ensure.Param(DateTimeOffset.UnixEpoch + TimeSpan.FromTicks(1)).NonNegative();
    }

    [Fact]
    public void NonNegative_InvalidInput_Throws()
    {
        ArgumentOutOfRangeException exc = Assert.Throws<ArgumentOutOfRangeException>(
                () => Ensure.Param(-1).NonNegative());

        Assert.StartsWith(
                "Parameter '-1' with value: -1 can not be negative.",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Fact]
    public void StrictlyPositive_ValidInput_Works()
    {
        Ensure.Param(1L).StrictlyPositive();
        Ensure.Param(1uL).StrictlyPositive();
        Ensure.Param(1).StrictlyPositive();
        Ensure.Param(1u).StrictlyPositive();
        Ensure.Param((short)1).StrictlyPositive();
        Ensure.Param((ushort)1).StrictlyPositive();
        Ensure.Param((sbyte)1).StrictlyPositive();
        Ensure.Param((byte)1).StrictlyPositive();
        Ensure.Param(.1m).StrictlyPositive();
        Ensure.Param(.1d).StrictlyPositive();
        Ensure.Param(.1f).StrictlyPositive();
        Ensure.Param(TimeSpan.FromTicks(1)).StrictlyPositive();
        Ensure.Param(DateTime.UnixEpoch + TimeSpan.FromTicks(1)).StrictlyPositive();
        Ensure.Param(DateTimeOffset.UnixEpoch + TimeSpan.FromTicks(1)).StrictlyPositive();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void StrictlyPositive_InvalidInput_Throws(int value)
    {
        ArgumentOutOfRangeException exc = Assert.Throws<ArgumentOutOfRangeException>(
                () => Ensure.Param(value).StrictlyPositive());

        Assert.StartsWith(
                $"Parameter 'value' with value: {value} must be strictly positive (greater than zero).",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Fact]
    public void NonZero_ValidInput_Works()
    {
        Ensure.Param(1L).NonZero();
        Ensure.Param(1uL).NonZero();
        Ensure.Param(-1).NonZero();
        Ensure.Param(1u).NonZero();
        Ensure.Param((short)-1).NonZero();
        Ensure.Param((ushort)1).NonZero();
        Ensure.Param((sbyte)1).NonZero();
        Ensure.Param((byte)1).NonZero();
        Ensure.Param(.1m).NonZero();
        Ensure.Param(.1d).NonZero();
        Ensure.Param(.1f).NonZero();
        Ensure.Param(TimeSpan.FromTicks(1)).NonZero();
        Ensure.Param(TimeSpan.FromTicks(-1)).NonZero();
        Ensure.Param(DateTime.UnixEpoch + TimeSpan.FromTicks(1)).NonZero();
        Ensure.Param(DateTimeOffset.UnixEpoch + TimeSpan.FromTicks(1)).NonZero();
    }

    [Theory]
    [InlineData(0)]
    public void NonZero_InvalidInput_Throws(int value)
    {
        ArgumentOutOfRangeException exc = Assert.Throws<ArgumentOutOfRangeException>(
                () => Ensure.Param(value).NonZero());

        Assert.StartsWith(
                $"Parameter 'value' with value: {value} can not be zero.",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Fact]
    public void HasActualValue_ValidInput_Works()
    {
        Ensure.Param(1L).HasActualValue();
        Ensure.Param(1uL).HasActualValue();
        Ensure.Param(1).HasActualValue();
        Ensure.Param(1u).HasActualValue();
        Ensure.Param((short)1).HasActualValue();
        Ensure.Param((ushort)1).HasActualValue();
        Ensure.Param((sbyte)1).HasActualValue();
        Ensure.Param((byte)1).HasActualValue();
        Ensure.Param(.1m).HasActualValue();
        Ensure.Param(.1d).HasActualValue();
        Ensure.Param(.1f).HasActualValue();
        Ensure.Param(TimeSpan.FromTicks(1)).HasActualValue();
        Ensure.Param(DateTime.UnixEpoch).HasActualValue();
        Ensure.Param(DateTimeOffset.UnixEpoch).HasActualValue();
    }

    [Fact]
    public void HasActualValue_InvalidInput_Throws()
    {
        ArgumentException exc = Assert.Throws<ArgumentException>(
                () => Ensure.Param(double.PositiveInfinity).HasActualValue());

        exc = Assert.Throws<ArgumentException>(
                () => Ensure.Param(double.NaN).HasActualValue());

        exc = Assert.Throws<ArgumentException>(
                () => Ensure.Param(double.NegativeInfinity).HasActualValue());

        exc = Assert.Throws<ArgumentException>(
                () => Ensure.Param(float.PositiveInfinity).HasActualValue());

        exc = Assert.Throws<ArgumentException>(
                () => Ensure.Param(float.NaN).HasActualValue());

        exc = Assert.Throws<ArgumentException>(
                () => Ensure.Param(float.NegativeInfinity).HasActualValue());

        Assert.StartsWith(
                $"Parameter 'float.NegativeInfinity' with value: {float.NegativeInfinity} has to be an actual number.",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(0L, -1L, 1L, true, true)]
    [InlineData(-1L, -1L, 1L, true, false)]
    [InlineData(1L, -1L, 1L, false, true)]
    [InlineData(0L, -1L, 1L, false, false)]
    public void InRange_ValidInput_Works(
            long value,
            long min,
            long max,
            bool lower,
            bool upper)
    {
        Ensure.Param(value).InRange(
                min,
                max,
                includeLower: lower,
                includeUpper: upper);
    }

    [Fact]
    public void InRange_ValidTimeParam_Works()
    {
        Ensure.Param(TimeSpan.Zero).InRange(
                TimeSpan.FromSeconds(-1.2),
                TimeSpan.FromSeconds(1.2),
                true,
                true);
        Ensure.Param(TimeSpan.FromSeconds(-1.2)).InRange(
                TimeSpan.FromSeconds(-1.2),
                TimeSpan.FromSeconds(1.2),
                true,
                false);
        Ensure.Param(TimeSpan.FromSeconds(1.2)).InRange(
                TimeSpan.FromSeconds(-1.2),
                TimeSpan.FromSeconds(1.2),
                false,
                true);
        Ensure.Param(TimeSpan.FromSeconds(0.1)).InRange(
                TimeSpan.FromSeconds(-1.2),
                TimeSpan.FromSeconds(1.2),
                false,
                false);

        Ensure.Param(DateTime.UnixEpoch).InRange(
                DateTime.MinValue,
                DateTime.MaxValue);

        Ensure.Param(DateTimeOffset.UnixEpoch).InRange(
                DateTimeOffset.MinValue,
                DateTimeOffset.MaxValue);
    }

    [Theory]
    [InlineData(-3.14, -1.2, 1.2, true, true)]
    [InlineData(-1.2, -1.2, 1.2, false, true)]
    [InlineData(1.2, -1.2, 1.2, true, false)]
    [InlineData(3.14, -1.2, 1.2, false, false)]
    public void InRange_InvalidData_Works(
            double value,
            double min,
            double max,
            bool lower,
            bool upper)
    {
        ArgumentOutOfRangeException exc = Assert.Throws<ArgumentOutOfRangeException>(
                () => Ensure.Param(value).InRange(
                    min,
                    max,
                    includeLower: lower,
                    includeUpper: upper));

        string range = Dump.Range(
                min,
                max,
                lower,
                upper);

        Assert.StartsWith(
                $"Parameter 'value' with value: {value} must be in range {range}.",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Fact]
    public void IsUTC_ValidInput_Works()
    {
        Ensure.Param(1L).IsUTC();
        Ensure.Param(1uL).IsUTC();
        Ensure.Param(1).IsUTC();
        Ensure.Param(1u).IsUTC();
        Ensure.Param((short)1).IsUTC();
        Ensure.Param((ushort)1).IsUTC();
        Ensure.Param((sbyte)1).IsUTC();
        Ensure.Param((byte)1).IsUTC();
        Ensure.Param(.1m).IsUTC();
        Ensure.Param(.1d).IsUTC();
        Ensure.Param(.1f).IsUTC();
        Ensure.Param(TimeSpan.FromTicks(1)).IsUTC();
        Ensure.Param(DateTime.UtcNow).IsUTC();
        Ensure.Param(DateTimeOffset.UtcNow).IsUTC();
    }

    [Fact]
    public void IsUTC_InvalidInput_Throws()
    {
        ArgumentException exc = Assert.Throws<ArgumentException>(
                () => Ensure.Param(new DateTime(314159265358979323, DateTimeKind.Local)).IsUTC());

        exc = Assert.Throws<ArgumentException>(
                () => Ensure.Param(new DateTime(314159265358979323, DateTimeKind.Unspecified)).IsUTC());

        exc = Assert.Throws<ArgumentException>(
                () => Ensure.Param(new DateTimeOffset(314159265358979323, TimeSpan.FromHours(1.0))).IsUTC());

        Assert.Contains("Parameter 'new DateTimeOffset(314159265358979323, TimeSpan.FromHours(1.0))'", exc.Message, StringComparison.Ordinal);
        Assert.Contains("is not a UTC time.", exc.Message, StringComparison.Ordinal);
    }
}
