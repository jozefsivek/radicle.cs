namespace Radicle.Common;

using System;
using Xunit;

public class Iso8601Test
{
    public static TheoryData<DateTimeOffset, string> LocalTimeAssumedCases => new()
    {
        { ConstructDateTimeOffset(42), "0042" },
        { ConstructDateTimeOffset(2042), "2042" },
        { ConstructDateTimeOffset(2042, 2), "2042-02" },
        { ConstructDateTimeOffset(2042, 2, 23), "2042-02-23" },
        { ConstructDateTimeOffset(2042, 2, 23), "20420223" },
        { ConstructDateTimeOffset(2042, 2, 23, 4), "2042-02-23T04:00:00" },
        { ConstructDateTimeOffset(2042, 1, 23, 4, 56), "2042-01-23T04:56:00" },
        { ConstructDateTimeOffset(2042, 1, 23, 4, 56, 57), "2042-01-23T04:56:57" },
        { ConstructDateTimeOffset(2042, 1, 23, 4, 56, 57, 0.123), "2042-01-23T04:56:57.123" },
        { ConstructDateTimeOffset(2042, 1, 23, 4, 56, 57, 0.123), "20420123T045657.123" },
    };

    public static TheoryData<DateTimeOffset, string> UTCAssumedCases => new()
    {
        { ConstructDateTimeOffset(42, minuteOffset: 0), "0042" },
        { ConstructDateTimeOffset(2042, minuteOffset: 0), "2042" },
        { ConstructDateTimeOffset(2042, 2, minuteOffset: 0), "2042-02" },
        { ConstructDateTimeOffset(2042, 2, 23, minuteOffset: 0), "2042-02-23" },
        { ConstructDateTimeOffset(2042, 2, 23, minuteOffset: 0), "20420223" },
        { ConstructDateTimeOffset(2042, 2, 23, 4, minuteOffset: 0), "2042-02-23T04:00:00" },
        { ConstructDateTimeOffset(2042, 1, 23, 4, 56, minuteOffset: 0), "2042-01-23T04:56:00" },
        { ConstructDateTimeOffset(2042, 1, 23, 4, 56, 57, minuteOffset: 0), "2042-01-23T04:56:57" },
        { ConstructDateTimeOffset(2042, 1, 23, 4, 56, 57, 0.1234, minuteOffset: 0), "2042-01-23T04:56:57.1234" },
        { ConstructDateTimeOffset(2042, 1, 23, 4, 56, 57, 0.1234, minuteOffset: 0), "20420123T045657.1234" },
    };

    public static TheoryData<DateTimeOffset, string> RegularCases => new()
    {
        { ConstructDateTimeOffset(42), "0042" },
        { ConstructDateTimeOffset(2042), "2042" },
        { ConstructDateTimeOffset(2042, 2), "2042-02" },
        { ConstructDateTimeOffset(2042, 2, 23), "2042-02-23" },
        { ConstructDateTimeOffset(2042, 2, 23), "20420223" },
        { ConstructDateTimeOffset(2042, 2, 23, 4, minuteOffset: 1 * 60), "2042-02-23T04:00:00+01" },
        { ConstructDateTimeOffset(2042, 1, 23, 4, 56, minuteOffset: -1 * 60), "2042-01-23T04:56:00-01" },
        { ConstructDateTimeOffset(2042, 1, 23, 4, 56, 57, minuteOffset: 14 * 60), "2042-01-23T04:56:57+14" },
        { ConstructDateTimeOffset(2042, 1, 23, 4, 56, 57, 0.12345, minuteOffset: -14 * 60), "2042-01-23T04:56:57.12345-14" },
        { ConstructDateTimeOffset(2042, 1, 23, 4, 56, 57, 0.12345, minuteOffset: (2 * 60) + 15), "20420123T045657.12345+02:15" },
    };

    public static TheoryData<DateTimeOffset, string> SerializationCases => new()
    {
        { ConstructDateTimeOffset(42, minuteOffset: 0), "0042-01-01T00:00:00.0000000+00:00" },
        { ConstructDateTimeOffset(2042, minuteOffset: 0), "2042-01-01T00:00:00.0000000+00:00" },
        { ConstructDateTimeOffset(2042, 2, minuteOffset: 0), "2042-02-01T00:00:00.0000000+00:00" },
        { ConstructDateTimeOffset(2042, 2, 23, minuteOffset: 0), "2042-02-23T00:00:00.0000000+00:00" },
        { ConstructDateTimeOffset(2042, 2, 23, 4, minuteOffset: 0), "2042-02-23T04:00:00.0000000+00:00" },
        { ConstructDateTimeOffset(2042, 1, 23, 4, 56, minuteOffset: 0), "2042-01-23T04:56:00.0000000+00:00" },
        { ConstructDateTimeOffset(2042, 1, 23, 4, 56, 57, minuteOffset: 0), "2042-01-23T04:56:57.0000000+00:00" },
        { ConstructDateTimeOffset(2042, 1, 23, 4, 56, 57, 0.1234567, minuteOffset: 0), "2042-01-23T04:56:57.1234567+00:00" },
        { ConstructDateTimeOffset(42, minuteOffset: 1 * 60), "0042-01-01T00:00:00.0000000+01:00" },
        { ConstructDateTimeOffset(2042, minuteOffset: -1 * 60), "2042-01-01T00:00:00.0000000-01:00" },
        { ConstructDateTimeOffset(2042, 2, minuteOffset: (2 * 60) + 15), "2042-02-01T00:00:00.0000000+02:15" },
        { ConstructDateTimeOffset(2042, 2, 23, minuteOffset: -(2 * 60) - 15), "2042-02-23T00:00:00.0000000-02:15" },
        { ConstructDateTimeOffset(2042, 2, 23, 4, minuteOffset: 14 * 60), "2042-02-23T04:00:00.0000000+14:00" },
        { ConstructDateTimeOffset(2042, 1, 23, 4, 56, minuteOffset: -14 * 60), "2042-01-23T04:56:00.0000000-14:00" },
        { ConstructDateTimeOffset(2042, 1, 23, 4, 56, 57, minuteOffset: 6 * 60), "2042-01-23T04:56:57.0000000+06:00" },
        { ConstructDateTimeOffset(2042, 1, 23, 4, 56, 57, 0.1234567, minuteOffset: -8 * 60), "2042-01-23T04:56:57.1234567-08:00" },
    };

    [Fact]
    public void TryParse_NullInput_ReturnsFalse()
    {
        Assert.False(Iso8601.TryParse(null, out DateTimeOffset _));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("123")]
    [InlineData(" 123 ")]
    [InlineData("2042-01-02T03:04:05,678")]
    [InlineData("non date")]
    public void TryParse_InvalidInput_ReturnsFalse(
            string invalidInput)
    {
        Assert.False(Iso8601.TryParse(invalidInput, out DateTimeOffset _));
    }

    [Theory]
    [InlineData("2042-01-02T030405.678")]
    [InlineData("20420102T03:04:05.678")]
    public void TryParse_MixedBasicInput_ReturnsFalse(
            string invalidInput)
    {
        Assert.False(Iso8601.TryParse(invalidInput, out DateTimeOffset _));
    }

    [Theory]
    [InlineData("2042")]
    [InlineData("2042-01-02T03:04:05.678")]
    public void TryParse_ExtendedInput_ReturnsFalse(
            string invalidInput)
    {
        Assert.False(Iso8601.TryParse(invalidInput, out DateTimeOffset _, forms: Iso8601Forms.Basic));
    }

    [Theory]
    [InlineData("20420102")]
    [InlineData("20420102T03:04:05.678")]
    public void TryParse_BasicInput_ReturnsFalse(
            string invalidInput)
    {
        Assert.False(Iso8601.TryParse(invalidInput, out DateTimeOffset _, forms: Iso8601Forms.Extended));
    }

    [Fact]
    public void TryParse_NoForm_ReturnsFalse()
    {
        Assert.False(Iso8601.TryParse("2042-01-02", out DateTimeOffset _, forms: Iso8601Forms.None));
    }

    [Theory]
    [MemberData(nameof(LocalTimeAssumedCases))]
    public void TryParse_LocalTimeInput_Works(
            DateTimeOffset expected,
            string input)
    {
        Assert.True(Iso8601.TryParse(input, out DateTimeOffset value, assumeUniversal: false));

        Assert.Equal(expected, value);
    }

    [Theory]
    [MemberData(nameof(UTCAssumedCases))]
    public void TryParse_UTCTimeInput_Works(
            DateTimeOffset expected,
            string input)
    {
        Assert.True(Iso8601.TryParse(input, out DateTimeOffset value, assumeUniversal: true));

        Assert.Equal(expected, value);
    }

    [Theory]
    [MemberData(nameof(RegularCases))]
    public void TryParse_ValidTimeInput_Works(
            DateTimeOffset expected,
            string input)
    {
        Assert.True(Iso8601.TryParse(input, out DateTimeOffset value));

        Assert.Equal(expected, value);
    }

    [Fact]
    public void Parse_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Iso8601.Parse(null!));
    }

    [Theory]
    [InlineData("")]
    public void Parse_OutOfRangeInput_Throws(
            string invalidInput)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Iso8601.Parse(invalidInput));
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("\n")]
    [InlineData("\r")]
    [InlineData("\t")]
    public void Parse_WhiteSpaceInput_Throws(string invalidInput)
    {
        Assert.Throws<ArgumentException>(() => Iso8601.Parse(invalidInput));
    }

    [Theory]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("123")]
    [InlineData(" 123 ")]
    [InlineData("2042-01-02T03:04:05,678")]
    [InlineData("non date")]
    public void Parse_InvalidInput_Throws(
            string invalidInput)
    {
        Assert.Throws<FormatException>(() => Iso8601.Parse(invalidInput));
    }

    [Theory]
    [InlineData("2042-01-02T030405.678")]
    [InlineData("20420102T03:04:05.678")]
    public void Parse_MixedBasicInput_Throws(
            string invalidInput)
    {
        Assert.Throws<FormatException>(() => Iso8601.Parse(invalidInput));
    }

    [Theory]
    [InlineData("2042")]
    [InlineData("2042-01-02T03:04:05.678")]
    public void Parse_ExtendedInput_Throws(
            string invalidInput)
    {
        Assert.Throws<FormatException>(() => Iso8601.Parse(invalidInput, forms: Iso8601Forms.Basic));
    }

    [Theory]
    [InlineData("20420102")]
    [InlineData("20420102T03:04:05.678")]
    public void Parse_BasicInput_Throws(
            string invalidInput)
    {
        Assert.Throws<FormatException>(() => Iso8601.Parse(invalidInput, forms: Iso8601Forms.Extended));
    }

    [Fact]
    public void Parse_NoForm_Throws()
    {
        Assert.Throws<FormatException>(() => Iso8601.Parse("2042-01-02", forms: Iso8601Forms.None));
    }

    [Theory]
    [MemberData(nameof(LocalTimeAssumedCases))]
    public void Parse_LocalTimeInput_Works(
            DateTimeOffset expected,
            string input)
    {
        Assert.Equal(expected, Iso8601.Parse(input, assumeUniversal: false));
    }

    [Theory]
    [MemberData(nameof(UTCAssumedCases))]
    public void Parse_UTCTimeInput_Works(
            DateTimeOffset expected,
            string input)
    {
        Assert.Equal(expected, Iso8601.Parse(input, assumeUniversal: true));
    }

    [Theory]
    [MemberData(nameof(RegularCases))]
    public void Parse_ValidTimeInput_Works(
            DateTimeOffset expected,
            string input)
    {
        DateTimeOffset value = Iso8601.Parse(input);

        Assert.Equal(expected, value);
    }

    [Theory]
    [MemberData(nameof(SerializationCases))]
    public void ToString_ValidInput_Works(
            DateTimeOffset input,
            string expected)
    {
        string value = Iso8601.ToString(input);

        Assert.Equal(expected, value);
    }

    private static DateTimeOffset ConstructDateTimeOffset(
            int year,
            int month = 1,
            int day = 1,
            int hour = 0,
            int minute = 0,
            int second = 0,
            double secondFractionPart = 0.0,
            int? minuteOffset = null)
    {
        DateTimeOffset result;

        if (minuteOffset.HasValue)
        {
            result = new DateTimeOffset(
                    year,
                    month,
                    day,
                    hour,
                    minute,
                    second,
                    TimeSpan.FromMinutes(minuteOffset.Value));
        }
        else
        {
            DateTime date = new(
                    year,
                    month,
                    day,
                    hour,
                    minute,
                    second,
                    DateTimeKind.Local);
            result = new DateTimeOffset(date);
        }

        if (secondFractionPart != 0)
        {
            result += TimeSpan.FromTicks((long)(
                    secondFractionPart * TimeSpan.TicksPerSecond));
        }

        return result;
    }
}
