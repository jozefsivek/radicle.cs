namespace Radicle.Common.Tokenization;

using System;
using System.Linq;
using System.Text;
using Radicle.Common.Tokenization.Models;
using Xunit;

public class CLikeStringLiteralDefinitionTest
{
    public static TheoryData<byte[], string> BinaryRawValueCasesForMinimal => new()
    {
        { Array.Empty<byte>(), string.Empty },
        { new byte[] { (byte)'\n', (byte)'\r', (byte)'\t', (byte)'\b', (byte)'\a', (byte)'\f', (byte)'\v' }, "\n\r\t\b\a\f\v" },
        { new byte[] { (byte)'"', (byte)'\'', (byte)'\\' }, "\\\"'\\\\" },
        { new byte[] { 0xc3, 0xb4 }, "\\xc3\\xb4" }, // letter
    };

    public static TheoryData<byte[], string> BinaryRawValueCasesForNormal => new()
    {
        { Array.Empty<byte>(), string.Empty },
        { new byte[] { (byte)'\n', (byte)'\r', (byte)'\t', (byte)'\b', (byte)'\a', (byte)'\f', (byte)'\v' }, "\\n\\r\\t\\b\\a\\f\\v" },
        { new byte[] { (byte)'"', (byte)'\'', (byte)'\\' }, "\\\"'\\\\" },
        { new byte[] { 0xc3, 0xb4 }, "\\xc3\\xb4" }, // letter
    };

    public static TheoryData<byte[], string> BinaryRawValueCasesForConservative => new()
    {
        { Array.Empty<byte>(), string.Empty },
        { new byte[] { (byte)'\n', (byte)'\r', (byte)'\t', (byte)'\b', (byte)'\a', (byte)'\f', (byte)'\v' }, "\\n\\r\\t\\b\\a\\f\\v" },
        { new byte[] { (byte)'"', (byte)'\'', (byte)'\\' }, "\\\"'\\\\" },
        { new byte[] { 0xc3, 0xb4 }, "\\xc3\\xb4" }, // letter
    };

    [Fact]
    public void Encode_NullInput_Throws()
    {
        CLikeStringLiteralDefinition def = new();

        Assert.Throws<ArgumentNullException>(() => def.Encode(null!));
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("a", "a")]
    [InlineData("\n\r\t\b\a\f\v", "\n\r\t\b\a\f\v")]
    [InlineData("\"'\\", "\\\"'\\\\")]
    [InlineData("\u00f4", "\u00f4")] // letter
    [InlineData("\u00bb", "\u00bb")] // punctuation
    [InlineData("\u2601\uFE0F", "\\xe2\\x98\\x81\\xef\\xb8\\x8f")] // emoji
    public void Encode_StringInputWithMinimalSetup_Works(
            string rawInput,
            string expectedEncoded)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Minimal;

        Assert.Equal(expectedEncoded, def.Encode(rawInput));
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("a", "a")]
    [InlineData("\n\r\t\b\a\f\v", "\\n\\r\\t\\b\\a\\f\\v")]
    [InlineData("\"'\\", "\\\"'\\\\")]
    [InlineData("\u00f4", "\u00f4")] // letter
    [InlineData("\u00bb", "\u00bb")] // punctuation
    [InlineData("\u2601\uFE0F", "\\xe2\\x98\\x81\\xef\\xb8\\x8f")] // emoji
    public void Encode_StringInputWithNormalSetup_Works(
            string rawInput,
            string expectedEncoded)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Normal;

        Assert.Equal(expectedEncoded, def.Encode(rawInput));
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("a", "a")]
    [InlineData("\n\r\t\b\a\f\v", "\\n\\r\\t\\b\\a\\f\\v")]
    [InlineData("\"'\\", "\\\"'\\\\")]
    [InlineData("\u00f4", "\\xc3\\xb4")] // letter
    [InlineData("\u00bb", "\\xc2\\xbb")] // punctuation
    [InlineData("\u2601\uFE0F", "\\xe2\\x98\\x81\\xef\\xb8\\x8f")] // emoji
    public void Encode_StringInputWithConservativeSetup_Works(
            string rawInput,
            string expectedEncoded)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;

        Assert.Equal(expectedEncoded, def.Encode(rawInput));
    }

    [Theory]
    [MemberData(nameof(BinaryRawValueCasesForMinimal))]
    public void Encode_BinaryInputWithMinimalSetup_Works(
            byte[] rawInput,
            string expectedEncoded)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Minimal;

        Assert.Equal(expectedEncoded, def.Encode(rawInput));
    }

    [Theory]
    [MemberData(nameof(BinaryRawValueCasesForNormal))]
    public void Encode_BinaryInputWithNormalSetup_Works(
            byte[] rawInput,
            string expectedEncoded)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Normal;

        Assert.Equal(expectedEncoded, def.Encode(rawInput));
    }

    [Theory]
    [MemberData(nameof(BinaryRawValueCasesForConservative))]
    public void Encode_BinaryInputWithConservativeSetup_Works(
            byte[] rawInput,
            string expectedEncoded)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;

        Assert.Equal(expectedEncoded, def.Encode(rawInput));
    }

    [Fact]
    public void GetLiteral_NullInput_Throws()
    {
        CLikeStringLiteralDefinition def = new();

        Assert.Throws<ArgumentNullException>(() => def.GetLiteral(null!));
    }

    [Theory]
    [InlineData("", "\"\"")]
    [InlineData("a", "\"a\"")]
    [InlineData("\n\r\t\b\a\f\v", "\"\n\r\t\b\a\f\v\"")]
    [InlineData("\"'\\", "\"\\\"'\\\\\"")]
    [InlineData("\u00f4", "\"\u00f4\"")] // letter
    [InlineData("\u00bb", "\"\u00bb\"")] // punctuation
    [InlineData("\u2601\uFE0F", "\"\\xe2\\x98\\x81\\xef\\xb8\\x8f\"")] // emoji
    public void GetLiteral_StringInputWithMinimalSetup_Works(
            string rawInput,
            string expectedEncoded)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Minimal;

        Assert.Equal(expectedEncoded, def.GetLiteral(rawInput));
    }

    [Theory]
    [InlineData("", "\"\"")]
    [InlineData("a", "\"a\"")]
    [InlineData("\n\r\t\b\a\f\v", "\"\\n\\r\\t\\b\\a\\f\\v\"")]
    [InlineData("\"'\\", "\"\\\"'\\\\\"")]
    [InlineData("\u00f4", "\"\u00f4\"")] // letter
    [InlineData("\u00bb", "\"\u00bb\"")] // punctuation
    [InlineData("\u2601\uFE0F", "\"\\xe2\\x98\\x81\\xef\\xb8\\x8f\"")] // emoji
    public void GetLiteral_StringInputWithNormalSetup_Works(
            string rawInput,
            string expectedEncoded)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Normal;

        Assert.Equal(expectedEncoded, def.GetLiteral(rawInput));
    }

    [Theory]
    [InlineData("", "\"\"")]
    [InlineData("a", "\"a\"")]
    [InlineData("\n\r\t\b\a\f\v", "\"\\n\\r\\t\\b\\a\\f\\v\"")]
    [InlineData("\"'\\", "\"\\\"'\\\\\"")]
    [InlineData("\u00f4", "\"\\xc3\\xb4\"")] // letter
    [InlineData("\u00bb", "\"\\xc2\\xbb\"")] // punctuation
    [InlineData("\u2601\uFE0F", "\"\\xe2\\x98\\x81\\xef\\xb8\\x8f\"")] // emoji
    public void GetLiteral_StringInputWithConservativeSetup_Works(
            string rawInput,
            string expectedEncoded)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;

        Assert.Equal(expectedEncoded, def.GetLiteral(rawInput));
    }

    [Theory]
    [MemberData(nameof(BinaryRawValueCasesForMinimal))]
    public void GetLiteral_BinaryInputWithMinimalSetup_Works(
            byte[] rawInput,
            string expectedEncoded)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Minimal;

        Assert.Equal($"\"{expectedEncoded}\"", def.GetLiteral(rawInput));
    }

    [Theory]
    [MemberData(nameof(BinaryRawValueCasesForNormal))]
    public void GetLiteral_BinaryInputWithNormalSetup_Works(
            byte[] rawInput,
            string expectedEncoded)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Normal;

        Assert.Equal($"\"{expectedEncoded}\"", def.GetLiteral(rawInput));
    }

    [Theory]
    [MemberData(nameof(BinaryRawValueCasesForConservative))]
    public void GetLiteral_BinaryInputWithConservativeSetup_Works(
            byte[] rawInput,
            string expectedEncoded)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;

        Assert.Equal($"\"{expectedEncoded}\"", def.GetLiteral(rawInput));
    }

    [Fact]
    public void Decode_NullInput_Throws()
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;

        Assert.Throws<ArgumentNullException>(() => def.Decode(null!));
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("a", "a")]
    [InlineData("\n\r\t\b\a\f\v", "\\n\\r\\t\\b\\a\\f\\v")]
    [InlineData("\"'\\", "\\\"'\\\\")]
    public void Decode_StringClearInput_Works(
            string expected,
            string encodedInput)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;

        TokenMatch decoded = def.Decode(encodedInput);

        TokenString decodedWithString =
                Assert.IsType<TokenString>(decoded);
        Assert.Equal(expected, decodedWithString.StringValue);
    }

    [Theory]
    [InlineData("\u00f4", "\\xc3\\xb4")] // letter
    [InlineData("\u00bb", "\\xc2\\xbb")] // punctuation
    [InlineData("\u2601\uFE0F", "\\xe2\\x98\\x81\\xef\\xb8\\x8f")] // emoji
    public void Decode_BinaryClearInput_Works(
            string expectedUTF8,
            string encodedInput)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;

        TokenMatch decoded = def.Decode(encodedInput);

        TokenBinary decodedWithBytes = Assert.IsType<TokenBinary>(decoded);
        Assert.Equal(Encoding.UTF8.GetBytes(expectedUTF8), decodedWithBytes.Bytes.ToArray());
    }

    [Fact]
    public void Decode_InputWithDoubleQuote_ReturnsFailure()
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;

        TokenMatch decoded = def.Decode("\"");

        TokenFailure failure = Assert.IsType<TokenFailure>(decoded);
        Assert.Equal("Disallowed character in encoded string literal content: \"\\\"\"", failure.FormatErrorMessage);
    }

    [Fact]
    public void Decode_InputWithInvalidHex_ReturnsFailure()
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;

        TokenMatch decoded = def.Decode("\\xgg");

        TokenFailure failure = Assert.IsType<TokenFailure>(decoded);
        Assert.Equal("Invalid escape sequence: \"xgg\"", failure.FormatErrorMessage);
    }

    [Theory]
    [InlineData("\\")]
    [InlineData("\\x")]
    [InlineData("\\x0")]
    public void Decode_IncompleteInput_ReturnsFailure(string encodedInput)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;

        TokenMatch decoded = def.Decode(encodedInput);

        TokenFailure failure = Assert.IsType<TokenFailure>(decoded);
        Assert.Equal("Unfinished string literal", failure.FormatErrorMessage);
    }

    [Fact]
    public void TryReadStringLiteral_NullStringInput_Throws()
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;

        Assert.Throws<ArgumentNullException>(() => def.TryReadStringLiteral((string)null!));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    public void TryReadStringLiteral_OutOfRangeInputForString_Throws(int startAt)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;

        Assert.Throws<ArgumentOutOfRangeException>(() => def.TryReadStringLiteral("a", startAt: startAt));
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("a", 1)]
    public void TryReadStringLiteral_StartAtEndOfStringInput_ReturnsFailure(
            string input,
            int startAt)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;

        TokenDecoding decoded = def.TryReadStringLiteral(input, startAt: startAt);

        TokenFailure failure = Assert.IsType<TokenFailure>(decoded);
        Assert.Equal("Missing literal boundary: \"\\\"\"", failure.FormatErrorMessage);
    }

    [Theory]
    [InlineData("", "\"\"", 0)]
    [InlineData("a", "\"a\"", 0)]
    [InlineData("\n\r\t\b\a\f\v", "\"\\n\\r\\t\\b\\a\\f\\v\"", 0)]
    [InlineData("\"'\\", "\"\\\"'\\\\\"", 0)]
    [InlineData("", "some\"\"", 4)]
    [InlineData("", "some\"\"\"", 4)]
    [InlineData("", "some\"\"suffix", 4)]
    [InlineData("", "some\"\"\\xgg", 4)]
    [InlineData("multiple", "\"multiple\"\"literals\"\"\"", 0)]
    [InlineData("literals", "\"multiple\"\"literals\"\"\"", 10)]
    [InlineData("", "\"multiple\"\"literals\"\"\"", 20)]
    public void TryReadStringLiteral_StringClearInput_Works(
            string expected,
            string encodedLiteral,
            int startAt)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;

        TokenDecoding decoded = def.TryReadStringLiteral(encodedLiteral, startAt: startAt);

        TokenString decodedWithString =
                Assert.IsType<TokenString>(decoded);
        Assert.Equal(expected, decodedWithString.StringValue);
    }

    [Theory]
    [InlineData("\u00f4", "\"\\xc3\\xb4\"", 0)] // letter
    [InlineData("\u00bb", "\"\\xc2\\xbb\"", 0)] // punctuation
    [InlineData("\u2601\uFE0F", "\"\\xe2\\x98\\x81\\xef\\xb8\\x8f\"", 0)] // emoji
    [InlineData("\u00f4", "  \"\\xc3\\xb4\"", 2)] // letter
    [InlineData("\u00f4", "  \"\\xc3\\xb4\"\"", 2)] // letter
    [InlineData("\u00f4", "  \"\\xc3\\xb4\"\"\\xgg", 2)] // letter
    public void TryReadStringLiteral_BinaryClearInput_Works(
            string expectedUTF8,
            string encodedLiteral,
            int startAt)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;

        TokenDecoding decoded = def.TryReadStringLiteral(encodedLiteral, startAt: startAt);

        TokenBinary decodedWithBytes = Assert.IsType<TokenBinary>(decoded);
        Assert.Equal(Encoding.UTF8.GetBytes(expectedUTF8), decodedWithBytes.Bytes.ToArray());
    }

    [Fact]
    public void TryReadStringLiteral_InputWithNoLiteral_ReturnsNoMathc()
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;

        TokenDecoding decoded = def.TryReadStringLiteral("this is not a literal \"\"");

        Assert.IsType<TokenNoMatch>(decoded);
    }

    [Theory]
    [InlineData("\"\"", 0, 2)]
    [InlineData("\"\"suffix", 0, 2)]
    [InlineData("\"foo\"", 0, 5)]
    [InlineData("\"foo\" \"bar\"", 6, 11)]
    public void TryReadStringLiteral_InputWithMultipleLiterals_ReturnsContinueAt(
            string inputLiteral,
            int startAt,
            int continueAt)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;

        TokenDecoding decoded = def.TryReadStringLiteral(inputLiteral, startAt: startAt);

        Assert.True(decoded is TokenWithValue);
        Assert.Equal(continueAt, ((TokenWithValue)decoded).ContinueAt);
    }

    [Fact]
    public void TryReadStringLiteral_InputWithInvalidHex_ReturnsFailure()
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;

        TokenDecoding decoded = def.TryReadStringLiteral("\"\\xgg\"");

        TokenFailure failure = Assert.IsType<TokenFailure>(decoded);
        Assert.Equal("Invalid escape sequence: \"xgg\"", failure.FormatErrorMessage);
    }

    [Theory]
    [InlineData("\"", 0)]
    [InlineData("\"\\", 0)]
    [InlineData("\"\\x", 0)]
    [InlineData("\"\\x0", 0)]
    [InlineData("\"\"", 1)]
    [InlineData("  \"\\x0", 2)]
    public void TryReadStringLiteral_IncompleteInput_ReturnsFailure(
            string encodedInput,
            int startAt)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;

        TokenDecoding decoded = def.TryReadStringLiteral(encodedInput, startAt: startAt);

        TokenFailure failure = Assert.IsType<TokenFailure>(decoded);
        Assert.Equal("Unfinished string literal", failure.FormatErrorMessage);
    }

    [Fact]
    public void TryReadStringLiteral_NullDecodedStringInput_Throws()
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;

        Assert.Throws<ArgumentNullException>(() => def.TryReadStringLiteral((TokenString)null!));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    public void TryReadStringLiteral_OutOfRangeInputForDecodedString_Throws(int startAt)
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;

        Assert.Throws<ArgumentOutOfRangeException>(() => def.TryReadStringLiteral(
                TokenString.GetPassThrough("a"),
                startAt: startAt));
    }

    [Fact]
    public void TryReadStringLiteral_DecodedStringInut_ReturnsCorrectParent()
    {
        CLikeStringLiteralDefinition def = CLikeStringLiteralDefinition.Conservative;
        TokenString parent = TokenString.GetPassThrough("\"foo\"");

        TokenDecoding decoded = def.TryReadStringLiteral(parent);

        Assert.True(ReferenceEquals(parent, decoded.Parent));
    }
}
