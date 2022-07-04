namespace Radicle.Common.Check;

using System;
using Xunit;

public class TypedNameSpecTest
{
    [Fact]
    public void TypedNameSpec_null_parameters_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new TypedNameSpec(null!, ".*"));
        Assert.Throws<ArgumentNullException>(() => new TypedNameSpec("name", null!));
        Assert.Throws<ArgumentNullException>(() => new TypedNameSpec(
                "name", ".*", disallowedValues: new string[] { null! }));

        Assert.Throws<ArgumentNullException>(() =>
                TypedNameSpec.Programming.With(
                    additionalDisallowedValues: new string[] { null! }));
    }

    [Fact]
    public void TypedNameSpec_empty_parameters_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new TypedNameSpec(string.Empty, ".*"));
        Assert.Throws<ArgumentOutOfRangeException>(() => new TypedNameSpec(
                "name", ".*", description: string.Empty));
        Assert.Throws<ArgumentOutOfRangeException>(() => new TypedNameSpec(
                "name", ".*", disallowedValues: new string[] { string.Empty }));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
                TypedNameSpec.Programming.With(name: string.Empty));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                TypedNameSpec.Programming.With(description: string.Empty));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                TypedNameSpec.Programming.With(
                additionalDisallowedValues: new string[] { string.Empty }));
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("\n")]
    [InlineData("\t")]
    [InlineData("\r")]
    public void Constructor_WithWhitespaceName_Throws(string whiteSpace)
    {
        Assert.Throws<ArgumentException>(() => new TypedNameSpec(
                whiteSpace, ".*"));

        Assert.Throws<ArgumentException>(() =>
                TypedNameSpec.Programming.With(name: whiteSpace));
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("\n")]
    [InlineData("\t")]
    [InlineData("\r")]
    public void Constructor_WithWhitespaceDescription_Throws(string whiteSpace)
    {
        Assert.Throws<ArgumentException>(() => new TypedNameSpec(
                "name", ".*", description: whiteSpace));

        Assert.Throws<ArgumentException>(() =>
                TypedNameSpec.Programming.With(description: whiteSpace));
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("\n")]
    [InlineData("\t")]
    [InlineData("\r")]
    public void Constructor_WithWhitespaceDisallowedValue_Throws(string whiteSpace)
    {
        Assert.Throws<ArgumentException>(() => new TypedNameSpec(
                "name", ".*", disallowedValues: new string[] { whiteSpace }));

        Assert.Throws<ArgumentException>(() =>
                TypedNameSpec.Programming.With(
                    additionalDisallowedValues: new string[] { whiteSpace }));
    }

    [Fact]
    public void TypedNameSpec_invalid_regexp_Throws()
    {
        Assert.Throws<ArgumentException>(() => new TypedNameSpec("name", "{****}}"));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 1)]
    [InlineData(10, 1)]
    [InlineData(1, 0)]
    [InlineData(4, 3)]
    public void Constructor_WithOutOfRangeLengths_Throws(ushort min, ushort max)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
                new TypedNameSpec("name", ".*", minLength: min, maxLength: max));
    }

    [Theory]
    [InlineData("a name", "\\A.*\\z", false)]
    [InlineData("Name", "\\A[a-z]+\\z", true)]
    [InlineData("Goo", "\\A[a-z]+\\z", true)]
    [InlineData("\n", "\\A.+\\z", true)]
    [InlineData("\r", "\\A.+\\z", true)]
    [InlineData("a\n", "\\A.+\\z", true)]
    [InlineData("a\r\n", "\\A.+\\z", true)]
    [InlineData("\r\na", "\\A.+\\z", true)]
    [InlineData("\ra", "\\A.+\\z", true)]
    public void IsValid_ValidRegExpValue_ReturnsTrue(
            string input,
            string regExp,
            bool ignoreCaseInPatter)
    {
        TypedNameSpec spec = new(
                "foo",
                regExp,
                ignoreCaseInPattern: ignoreCaseInPatter);

        Assert.True(spec.IsValid(input));

        Assert.Equal(input, spec.EnsureValid(input));
    }

    [Theory]
    [InlineData("a", "\\Ab\\z", false)]
    [InlineData("B", "\\Ab\\z", false)]
    [InlineData("Goo\n", "\\A[a-z]+\\z", true)]
    [InlineData("Goo", "\\A[a-z]+\\z", false)]
    [InlineData("too much space", "\\A[a-z]+\\z", false)]
    [InlineData("c", "\\A[a-b]+\\z", true)]
    public void IsValid_InvalidRegExpValue_ReturnsFalse(
            string input,
            string regExp,
            bool ignoreCaseInPatter)
    {
        TypedNameSpec spec = new(
                "foo",
                regExp,
                ignoreCaseInPattern: ignoreCaseInPatter);

        Assert.False(spec.IsValid(input));

        ArgumentException exc = Assert.Throws<ArgumentException>(
                () => spec.EnsureValid(input));

        Assert.Contains(
                "does not conform to specification of",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("a name", "foo,bar", false)]
    [InlineData("Foo", "foo,bar", false)]
    [InlineData("Name", "bar,ame", true)]
    public void IsValid_ValidDisallowedValue_ReturnsTrue(
            string input,
            string disallowed,
            bool ignoreCaseInPatter)
    {
        TypedNameSpec spec = new(
                "foo",
                ".*",
                ignoreCaseInPattern: ignoreCaseInPatter,
                disallowedValues: disallowed.Split(','));

        Assert.True(spec.IsValid(input));

        Assert.Equal(input, spec.EnsureValid(input));
    }

    [Theory]
    [InlineData("parrot", "parrot", false)]
    [InlineData("Parrot", "dead,parrot", true)]
    [InlineData("foo", "foo,bar", false)]
    public void IsValid_DisallowedValue_ReturnsFalse(
            string input,
            string disallowed,
            bool ignoreCaseInPatter)
    {
        TypedNameSpec spec = new(
                "foo",
                ".*",
                ignoreCaseInPattern: ignoreCaseInPatter,
                disallowedValues: disallowed.Split(','));

        Assert.False(spec.IsValid(input));

        ArgumentException exc = Assert.Throws<ArgumentException>(
                () => spec.EnsureValid(input));

        Assert.Contains(
                "is disallowed value for this specification of",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("a name", 1, 256)]
    [InlineData("Foo", 1, 3)]
    [InlineData("Name", 1, 4)]
    [InlineData("a", 1, 356)]
    public void IsValid_InRangeValue_ReturnsTrue(
            string input,
            ushort min,
            ushort max)
    {
        TypedNameSpec spec = new(
                "foo",
                ".*",
                minLength: min,
                maxLength: max);

        Assert.True(spec.IsValid(input));

        Assert.Equal(input, spec.EnsureValid(input));
    }

    [Theory]
    [InlineData("a way too long name", 1, 6)]
    [InlineData("o", 2, 3)]
    public void IsValid_OutOfRangeValue_ReturnsFalse(
            string input,
            ushort min,
            ushort max)
    {
        TypedNameSpec spec = new(
                "foo",
                ".*",
                minLength: min,
                maxLength: max);

        Assert.False(spec.IsValid(input));

        ArgumentException exc = Assert.Throws<ArgumentOutOfRangeException>(
                () => spec.EnsureValid(input));

        Assert.Contains(
                $"length must be in range [{min}, {max}]",
                exc.Message,
                StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(1, 8)]
    [InlineData(2, 9)]
    [InlineData(8, 2)]
    public void With_OutOfBoundsRange_Throws(ushort min, ushort max)
    {
        TypedNameSpec specification = new TypedNameSpec(
                "foo",
                ".*").With(
                    minLength: 2,
                    maxLength: 8);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
                specification.With(minLength: min, maxLength: max));
    }

    [Fact]
    public void Description_LowerCaseSpec_HasExpectedValue()
    {
        TypedNameSpec spec = new TypedNameSpec(
                "foo",
                "\\A[a-z]+\\z",
                ignoreCaseInPattern: true).With(ignoreCaseInPattern: false);

        string expected = "foo: mixed case /\\A[a-z]+\\z/i with length range [1, 256], case sensitive comparison"
                .Replace("mixed case", "lower case", StringComparison.Ordinal);

        Assert.Equal(
                expected,
                spec.Description);
    }

    [Fact]
    public void Description_CaseInsensitiveComparison_HasExpectedValue()
    {
        TypedNameSpec spec = new TypedNameSpec(
                "foo",
                "\\A[a-z]+\\z",
                ignoreCaseWhenCompared: false).With(ignoreCaseWhenCompared: true);

        Assert.Equal(
                "foo: lower case /\\A[a-z]+\\z/ with length range [1, 256], case sensitive comparison"
                    .Replace("case sensitive", "case insensitive", StringComparison.Ordinal),
                spec.Description);
    }

    /// <summary>
    /// Convert given string to uppercase and
    /// return <see langword="true"/> if resulting
    /// string is different, hence upper case.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <param name="upper">Upper form.</param>
    /// <returns>Non equality.</returns>
    internal static bool Upper(string value, out string upper)
    {
        Ensure.NotNull(nameof(value), value);

        upper = value.ToUpperInvariant();

        return upper != value;
    }
}
