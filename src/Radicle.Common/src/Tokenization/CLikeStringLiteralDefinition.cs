namespace Radicle.Common.Tokenization;

using System;
using System.Collections.Generic;
using System.Text;
using Radicle.Common.Check;
using Radicle.Common.Tokenization.Models;

/// <summary>
/// Implementation of <see cref="IStringLiteralDefinition"/>
/// which follows escape rules of C: https://en.wikipedia.org/wiki/Escape_sequences_in_C ,
/// https://redis.io/docs/manual/cli/#string-quoting-and-escaping ,
/// https://docs.microsoft.com/en-us/cpp/c-language/escape-sequences?view=msvc-170
/// additional configuration is possible.
/// </summary>
public sealed class CLikeStringLiteralDefinition : IStringLiteralDefinition
{
    /// <summary>
    /// Constant of the C like escape character.
    /// </summary>
    public const char EscapeMark = '\\';

    /// <summary>
    /// Constant of the C like double quotation mark.
    /// </summary>
    public const char DoubleQuotationMark = '"';

    /// <summary>
    /// Constant of the C like single quotation mark.
    /// </summary>
    public const char SingleQuotationMark = '\'';

    /// <summary>
    /// Constant of the C like hex prefix in escape sequence.
    /// </summary>
    public const char HexPrefix = 'x';

    /// <summary>
    /// Minimal definition, escaping just quotation (double quotes),
    /// and escape character itself.
    /// </summary>
    public static readonly CLikeStringLiteralDefinition Minimal = new();

    /// <summary>
    /// Normal definition, escaping common characters
    /// leaving unicode letters and digits intact.
    /// </summary>
    public static readonly CLikeStringLiteralDefinition Normal = new(
            preferCommonEscapeSequences: true);

    /// <summary>
    /// Conservative definition, escaping everything
    /// except common ASCII characters.
    /// </summary>
    public static readonly CLikeStringLiteralDefinition Conservative = new(
            preferCommonEscapeSequences: true,
            escapeNonAlphanumericAsBinary: true);

    private readonly ParsedTokenStopWord quotationMarkStopWord;

    private readonly ParsedTokenStopWord escapeTokenStopWord;

    private readonly char quotationMarkCharacter;

    private readonly char escapeTokenCharacter;

    private readonly Dictionary<char, char> controlToCommonEscapeCharacters = new()
    {
        { 'n', '\n' },
        { 'r', '\r' },
        { 't', '\t' },
        { 'b', '\b' },
        { 'a', '\a' },
        { 'f', '\f' },
        { 'v', '\v' },
    };

    private readonly Dictionary<char, byte> controlToCommonEscapeCode = new()
    {
        { 'n', (byte)'\n' },
        { 'r', (byte)'\r' },
        { 't', (byte)'\t' },
        { 'b', (byte)'\b' },
        { 'a', (byte)'\a' },
        { 'f', (byte)'\f' },
        { 'v', (byte)'\v' },
    };

    private readonly Dictionary<char, char> commonEscapeCharacters = new()
    {
        { '\n', 'n' },
        { '\r', 'r' },
        { '\t', 't' },
        { '\b', 'b' },
        { '\a', 'a' },
        { '\f', 'f' },
        { '\v', 'v' },
    };

    private readonly Dictionary<byte, char> commonEscapeCodes = new()
    {
        { (byte)'\n', 'n' },
        { (byte)'\r', 'r' },
        { (byte)'\t', 't' },
        { (byte)'\b', 'b' },
        { (byte)'\a', 'a' },
        { (byte)'\f', 'f' },
        { (byte)'\v', 'v' },
    };

    private readonly StopWordStringParser stopWordParser;

    /// <summary>
    /// Initializes a new instance of the <see cref="CLikeStringLiteralDefinition"/> class.
    /// </summary>
    /// <param name="preferCommonEscapeSequences">Value indicating whether
    ///     common escape sequences are used, like for tab etc.</param>
    /// <param name="escapeNonAlphanumericAsBinary">Value indicating whether
    ///     non alpha-numerical (plus common characters) characters
    ///     are escaped as binary data. Binary tokens have this
    ///     option implicitly enabled.</param>
    public CLikeStringLiteralDefinition(
            bool preferCommonEscapeSequences = false,
            bool escapeNonAlphanumericAsBinary = false)
    {
        this.QuotationMarkStart = new string(DoubleQuotationMark, 1);
        this.QuotationMarkEnd = this.QuotationMarkStart;
        this.EscapeTokenStart = new string(EscapeMark, 1);
        this.EscapeTokenEnd = string.Empty;
        this.PreferCommonEscapeSequences = preferCommonEscapeSequences;
        this.EscapeNonAlphanumericAsBinary = escapeNonAlphanumericAsBinary;
        this.quotationMarkStopWord = DoubleQuotationMark;
        this.escapeTokenStopWord = EscapeMark;
        this.quotationMarkCharacter = DoubleQuotationMark;
        this.escapeTokenCharacter = EscapeMark;
        this.stopWordParser = this.ConstructStopWordParser();
    }

    /// <summary>
    /// Gets a value indicating whether common escape sequences are used,
    /// like for tab etc.
    /// </summary>
    /// <remarks>Complete set of escape sequences:
    /// '\"', '\'' (depending on <see cref="QuotationMarkStart"/> so it does not clash),
    /// '\n', '\r', '\t', '\b', '\a', '\\', '\xnn' (hex notation of single byte),
    /// '\f' (Formfeed Page Break \x0C), '\v' (vertical tab '\x0B'),
    /// '\?' (not used with this setting).</remarks>
    public bool PreferCommonEscapeSequences { get; }

    /// <summary>
    /// Gets a value indicating whether non alphanumerical
    /// (plus common characters) characters
    /// are escaped as binary data (in hex notation one byte wide,
    /// e.g. '\xnn' where n stands for single hex number).
    /// Binary tokens have this option enabled by default.
    /// This is to ensure maximal safety of copied token.
    /// </summary>
    public bool EscapeNonAlphanumericAsBinary { get; }

    /// <inheritdoc/>
    public string QuotationMarkStart { get; }

    /// <inheritdoc/>
    public string QuotationMarkEnd { get; }

    /// <inheritdoc/>
    public string EscapeTokenStart { get; }

    /// <inheritdoc/>
    public string EscapeTokenEnd { get; }

    /// <inheritdoc/>
    public string Encode(TokenWithValue value)
    {
        Ensure.Param(value).Done();

        if (value is TokenString ts)
        {
            return this.Encode(ts.StringValue);
        }
        else if (value is TokenBinary tb)
        {
            return this.Encode(tb.Bytes);
        }

        string type = value?.GetType().ToString() ?? "null";

        throw new NotSupportedException($"BUG: not supported token with value {type}");
    }

    /// <inheritdoc/>
    public string GetLiteral(TokenWithValue value)
    {
        return new StringBuilder()
                .Append(this.QuotationMarkStart)
                .Append(this.Encode(value))
                .Append(this.QuotationMarkEnd)
                .ToString();
    }

    /// <inheritdoc/>
    public TokenMatch Decode(string input)
    {
        return (TokenMatch)this.TryReadStringLiteralInternal(
                input,
                startAt: 0,
                disallowQuotation: true);
    }

    /// <inheritdoc/>
    public TokenDecoding TryReadStringLiteral(
            string input,
            int startAt = 0)
    {
        return this.TryReadStringLiteralInternal(
                input,
                startAt: startAt);
    }

    /// <inheritdoc/>
    public TokenDecoding TryReadStringLiteral(
            TokenString input,
            int startAt = 0)
    {
        Ensure.Param(input).Done();

        return this.TryReadStringLiteralInternal(
                input.StringValue,
                startAt: startAt,
                parent: input);
    }

    private static IEnumerable<string> EncodeToHex(char character)
    {
        foreach (byte code in Encoding.UTF8.GetBytes(new[] { character }))
        {
#pragma warning disable CA1308 // Normalize strings to uppercase
            yield return EncodeToHex(code).ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
        }
    }

    private static string EncodeToHex(byte characterCode)
    {
#pragma warning disable CA1308 // Normalize strings to uppercase
        return BitConverter.ToString(new[] { characterCode }).ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
    }

    private static bool TryDecodeHex(string input, out byte b)
    {
        b = default;

        try
        {
            b = Convert.ToByte(input, fromBase: 16);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
        catch (FormatException)
        {
            return false;
        }
        catch (OverflowException)
        {
            return false;
        }
    }

    private string Encode(string rawValue)
    {
        StringBuilder sb = new();

        foreach (char character in Ensure.Param(rawValue).Value)
        {
            if (this.IsSafeCharacter(character))
            {
                sb = sb.Append(character);
            }
            else
            {
                foreach (string str in this.EncodeToEscapCodes(character))
                {
                    sb = sb
                            .Append(this.EscapeTokenStart)
                            .Append(str)
                            .Append(this.EscapeTokenEnd);
                }
            }
        }

        return sb.ToString();
    }

    private string Encode(IEnumerable<byte> rawBytes)
    {
        StringBuilder sb = new();

        foreach (byte characterCode in Ensure.Param(rawBytes).Value)
        {
            if (this.IsSafeCharacterCode(characterCode))
            {
                sb = sb.Append((char)characterCode);
            }
            else
            {
                sb = sb
                        .Append(this.EscapeTokenStart)
                        .Append(this.EncodeToEscapeCode(characterCode))
                        .Append(this.EscapeTokenEnd);
            }
        }

        return sb.ToString();
    }

    private TokenDecoding TryReadStringLiteralInternal(
            string input,
            int startAt = 0,
            bool disallowQuotation = false,
            TokenString? parent = null)
    {
        Ensure.Param(input).Done();
        Ensure.Param(startAt).InRange(0, input.Length).Done();

        // empty source
        if (string.IsNullOrEmpty(input) || startAt == input.Length)
        {
            if (disallowQuotation)
            {
                return new TokenString(
                        input,
                        startAt: startAt,
                        endAt: startAt,
                        continueAt: startAt,
                        stringValue: string.Empty)
                {
                    Parent = parent,
                };
            }

            // for required quotation this is failure
            return new TokenFailure(
                    input,
                    startAt: startAt,
                    endAt: startAt,
                    formatErrorMessage: $"Missing literal boundary: {Dump.Literal(this.QuotationMarkStart)}")
            {
                Parent = parent,
            };
        }

        // check for leading quotation mark
        if (!disallowQuotation && input[startAt] != this.quotationMarkCharacter)
        {
            return new TokenNoMatch(input, startAt: startAt);
        }
        else if (disallowQuotation && input[startAt] == this.quotationMarkCharacter)
        {
            return new TokenFailure(
                    input,
                    startAt: startAt,
                    endAt: startAt,
                    formatErrorMessage: $"Disallowed character in encoded string literal content: {Dump.Literal(this.QuotationMarkStart)}")
            {
                Parent = parent,
            };
        }

        bool inQuotation = false;
        bool incompleteRead = false;
        StringBuilder sb = new();
        List<byte>? bytes = null;
        int pos = -1 + startAt;
        ParsedToken? lastReadToken = null;

        foreach (ParsedToken token in this.stopWordParser.Parse(input, startAt: startAt))
        {
            lastReadToken = token;
            pos += token.Length;

            if (token == this.quotationMarkStopWord)
            {
                if (disallowQuotation)
                {
                    return new TokenFailure(
                        input,
                        startAt: startAt,
                        endAt: pos,
                        formatErrorMessage: $"Disallowed character in encoded string literal: {Dump.Literal(this.QuotationMarkStart)}")
                    {
                        Parent = parent,
                    };
                }

                if (inQuotation)
                {
                    inQuotation = false;
                    break;
                }

                inQuotation = true;
                continue;
            }

            if (token is ParsedTokenControl incompleteControl && incompleteControl.IncompleteRead)
            {
                incompleteRead = true;
                break;
            }

            // continue if ParsedTokenStopWord
            if (token is ParsedTokenControl control && control.Length > 0)
            {
                if (this.controlToCommonEscapeCharacters.ContainsKey(control.Value[0]))
                {
                    if (bytes is null)
                    {
                        sb = sb.Append(this.controlToCommonEscapeCharacters[control.Value[0]]);
                    }
                    else
                    {
                        bytes.Add(this.controlToCommonEscapeCode[control.Value[0]]);
                    }
                }
                else if (control.Value[0] == HexPrefix)
                {
                    if (!(control.Value.Length == 3 && TryDecodeHex(control.Value[1..], out byte b)))
                    {
                        return new TokenFailure(
                            input,
                            startAt: startAt,
                            endAt: pos,
                            formatErrorMessage: $"Invalid escape sequence: {Dump.Literal(control.Value)}")
                        {
                            Parent = parent,
                        };
                    }

                    if (bytes is null)
                    {
                        bytes = new List<byte>(Encoding.UTF8.GetBytes(sb.ToString()));
                        sb = sb.Clear();
                    }

                    bytes.Add(b);
                }
                else if (bytes is null)
                {
                    sb = sb.Append(control.Value);
                }
                else
                {
                    bytes.AddRange(Encoding.UTF8.GetBytes(control.Value));
                }
            }
            else if (token is ParsedTokenFreeText txt)
            {
                if (bytes is null)
                {
                    sb = sb.Append(txt.Value);
                }
                else
                {
                    bytes.AddRange(Encoding.UTF8.GetBytes(txt.Value));
                }
            }
        }

        // incomplete reads
        if (inQuotation || incompleteRead || lastReadToken == this.escapeTokenStopWord)
        {
            return new TokenFailure(
                    input,
                    startAt: startAt,
                    endAt: pos,
                    formatErrorMessage: "Unfinished string literal")
            {
                Parent = parent,
            };
        }

        if (bytes is null)
        {
            return new TokenString(
                    input,
                    startAt: startAt,
                    endAt: pos,
                    continueAt: pos + 1,
                    stringValue: sb.ToString())
            {
                Parent = parent,
            };
        }

        return new TokenBinary(
                input,
                startAt: startAt,
                endAt: pos,
                continueAt: pos + 1,
                bytes: bytes)
        {
            Parent = parent,
        };
    }

    private StopWordStringParser ConstructStopWordParser()
    {
        return new StopWordStringParser(
                new HashSet<ParsedTokenStopWord>(new[] { this.quotationMarkStopWord, this.escapeTokenStopWord }),
                state =>
                {
                    if (state.TriggeringStopWord == this.escapeTokenStopWord)
                    {
                        if (state.ReadBuffer.Length == 0)
                        {
                            return ParseAction.ReadMoreControl;
                        }
                        else if (state.ReadBuffer.Length == 1)
                        {
                            if (state.ReadBuffer[0] == HexPrefix)
                            {
                                return ParseAction.ContinueWithControllUntil(3);
                            }

                            return ParseAction.YieldControl;
                        }
                    }

                    return ParseAction.Continue;
                });
    }

    private bool IsSafeCharacter(char character)
    {
        if (character == this.quotationMarkCharacter)
        {
            return false;
        }
        else if (character == this.escapeTokenCharacter)
        {
            return false;
        }
        else if (character is >= ' ' and <= '~')
        {
            /* printable region */
            return true;
        }
        else if (this.commonEscapeCharacters.ContainsKey(character))
        {
            return !this.PreferCommonEscapeSequences;
        }
        else if (this.EscapeNonAlphanumericAsBinary)
        {
            return false;
        }
        else
        {
            return char.IsLetterOrDigit(character)
                    || char.IsPunctuation(character);
        }
    }

    private bool IsSafeCharacterCode(byte characterCode)
    {
        if (characterCode == (byte)this.quotationMarkCharacter)
        {
            return false;
        }
        else if (characterCode == (byte)this.escapeTokenCharacter)
        {
            return false;
        }
        else if (characterCode is >= (byte)' ' and <= (byte)'~')
        {
            /* printable region */
            return true;
        }
        else if (this.commonEscapeCodes.ContainsKey(characterCode))
        {
            return !this.PreferCommonEscapeSequences;
        }
        else
        {
            return false;
        }
    }

    private IEnumerable<string> EncodeToEscapCodes(char character)
    {
        if (character == this.quotationMarkCharacter)
        {
            yield return this.quotationMarkStopWord.Value;
        }
        else if (character == this.escapeTokenCharacter)
        {
            yield return this.escapeTokenStopWord.Value;
        }
        else if (this.commonEscapeCharacters.TryGetValue(character, out char escapeValue))
        {
            yield return new string(escapeValue, 1);
        }
        else
        {
            foreach (string str in EncodeToHex(character))
            {
                yield return $"{HexPrefix}{str}";
            }
        }
    }

    private string EncodeToEscapeCode(byte characterCode)
    {
        if (characterCode == this.quotationMarkCharacter)
        {
            return this.quotationMarkStopWord.Value;
        }
        else if (characterCode == this.escapeTokenCharacter)
        {
            return this.escapeTokenStopWord.Value;
        }
        else if (this.commonEscapeCodes.TryGetValue(characterCode, out char code))
        {
            return new string(code, 1);
        }
        else
        {
            return $"{HexPrefix}{EncodeToHex(characterCode)}";
        }
    }
}
