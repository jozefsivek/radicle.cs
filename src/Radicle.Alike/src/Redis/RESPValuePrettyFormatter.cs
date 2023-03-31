namespace Radicle.Alike.Redis;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Radicle.Alike.Redis.Models;
using Radicle.Common;
using Radicle.Common.Check;
using Radicle.Common.Tokenization;
using Radicle.Common.Tokenization.Models;

/// <summary>
/// Visitor for <see cref="RESPValue"/>
/// for pretty printing.
/// </summary>
/// <remarks>
/// Return pretty formatted string representation
/// as collection of lines not including any new line characters.
/// </remarks>
public sealed class RESPValuePrettyFormatter : IRESPValueVisitor<IEnumerable<string>>
{
    private string indentPrefix = string.Empty;

    /// <summary>
    /// Enumeration of possible <see cref="RESPValue.Attribs"/>
    /// handling.
    /// </summary>
    public enum AttribsHandling
    {
        /// <summary>
        /// Denote values with the attribute with note character.
        /// </summary>
        Denote = 0,

        /// <summary>
        /// Hide attributes and do not format them.
        /// </summary>
        Hide = 1,
    }

    /// <summary>
    /// Gets possible <see cref="RESPValue.Attribs"/> handling
    /// of this formatter. Defaults to <see cref="AttribsHandling.Denote"/>.
    /// </summary>
    public AttribsHandling ValueAttribsHandling { get; init; } = AttribsHandling.Denote;

    /// <summary>
    /// Gets attribute denote character.
    /// </summary>
    public string AttribsDenoteCharacter { get; init; } = "*";

    /// <inheritdoc/>
    public IEnumerable<string> Visit(RESPArray array)
    {
        Ensure.Param(array).Done();

        if (array.IsEmpty)
        {
            yield return $"(empty {RESPArray.HumanName}){this.AttribsNote(array)}";
        }
        else
        {
            int keySize = $"{array.Length + 1}".Length;
            const string separator = ") ";
            string indentIncr = new(' ', keySize + separator.Length);
            string currentIndentPrefix = this.indentPrefix;
            this.indentPrefix += indentIncr;
            bool firstLine = true;
            int key = 0;

            foreach (RESPValue item in array)
            {
                key++; // 1-based indexing
                string itemKey = $"{key}".PadLeft(keySize) + separator;

                bool firstSubLine = true;

                foreach (string subLine in item.Accept(this))
                {
                    if (firstSubLine)
                    {
                        if (firstLine)
                        {
                            yield return itemKey + subLine;
                            firstLine = false;
                        }
                        else
                        {
                            yield return currentIndentPrefix + itemKey + subLine;
                        }

                        firstSubLine = false;
                    }
                    else
                    {
                        yield return subLine;
                    }
                }
            }

            if (this.TryGetAttribsFootNote(array, out string? note))
            {
                yield return currentIndentPrefix + note;
            }

            this.indentPrefix = currentIndentPrefix;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<string> Visit(RESPAttributeValue attribute)
    {
        Ensure.Param(attribute).Done();

        if (attribute.IsEmpty)
        {
            yield return $"(empty {RESPAttributeValue.HumanName}){this.AttribsNote(attribute)}";
        }
        else
        {
            const string keyPrefix = "| ";
            const string separator = ": ";
            string indentIncr = new(' ', 2);
            string currentIndentPrefix = this.indentPrefix;
            this.indentPrefix += indentIncr;
            bool firstLine = true;

            foreach (KeyValuePair<RESPValue, RESPValue> item in attribute)
            {
                bool firstSubLine = true;
                string[] keyLines = item.Key.Accept(this).ToArray();

                foreach (string subKeyLine in keyLines[..^1])
                {
                    if (firstSubLine)
                    {
                        if (firstLine)
                        {
                            yield return keyPrefix + subKeyLine;
                            firstLine = false;
                        }
                        else
                        {
                            yield return currentIndentPrefix + keyPrefix + subKeyLine;
                        }

                        firstSubLine = false;
                    }
                    else
                    {
                        yield return subKeyLine;
                    }
                }

                string key = keyLines[^1];
                string itemKey = key + separator;

                foreach (string subLine in item.Value.Accept(this))
                {
                    if (firstSubLine)
                    {
                        if (firstLine)
                        {
                            yield return keyPrefix + itemKey + subLine;
                            firstLine = false;
                        }
                        else
                        {
                            yield return currentIndentPrefix + keyPrefix + itemKey + subLine;
                        }

                        firstSubLine = false;
                    }
                    else
                    {
                        yield return subLine;
                    }
                }
            }

            if (this.TryGetAttribsFootNote(attribute, out string? note))
            {
                yield return currentIndentPrefix + note;
            }

            this.indentPrefix = currentIndentPrefix;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<string> Visit(RESPBigNumber bigNumber)
    {
        Ensure.Param(bigNumber).Done();

        yield return $"({RESPBigNumber.HumanName}) {bigNumber.Value}{this.AttribsNote(bigNumber)}";
    }

    /// <inheritdoc/>
    public IEnumerable<string> Visit(RESPBlobError blobError)
    {
        Ensure.Param(blobError).Done();

        string? stringValue = default;

        try
        {
            stringValue = blobError.StringValue;
        }
        catch (ArgumentException)
        {
            // no UTF-8 string
        }

        if (stringValue is null)
        {
            yield return Dump.Literal(TokenWithValue.ToTokenWithValue(blobError.Value));
        }
        else
        {
            yield return Dump.Literal(stringValue);
        }

        if (this.TryGetAttribsFootNote(blobError, out string? note))
        {
            yield return this.indentPrefix + note;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<string> Visit(RESPBlobString blobString)
    {
        Ensure.Param(blobString).Done();

        string? stringValue = default;

        try
        {
            stringValue = blobString.StringValue;
        }
        catch (ArgumentException)
        {
            // no UTF-8 string
        }

        if (stringValue is null)
        {
            yield return Dump.Literal(TokenWithValue.ToTokenWithValue(blobString.Value));
        }
        else
        {
            yield return Dump.Literal(stringValue);
        }

        if (this.TryGetAttribsFootNote(blobString, out string? note))
        {
            yield return this.indentPrefix + note;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<string> Visit(RESPBoolean booleanValue)
    {
        yield return (Ensure.Param(booleanValue).Value.Value ? "(true)" : "(false)")
                + this.AttribsNote(booleanValue);
    }

    /// <inheritdoc/>
    public IEnumerable<string> Visit(RESPDouble doubleValue)
    {
        string numberString = GetNumberString(Ensure.Param(doubleValue).Value.Value);

        yield return $"({RESPDouble.HumanName}) {numberString}{this.AttribsNote(doubleValue)}";
    }

    /// <inheritdoc/>
    public IEnumerable<string> Visit(RESPMap map)
    {
        Ensure.Param(map).Done();

        if (map.IsEmpty)
        {
            yield return $"(empty {RESPMap.HumanName}){this.AttribsNote(map)}";
        }
        else
        {
            const string keyPrefix = "* ";
            const string separator = ": ";
            string indentIncr = new(' ', 2);
            string currentIndentPrefix = this.indentPrefix;
            this.indentPrefix += indentIncr;
            bool firstLine = true;

            foreach (KeyValuePair<RESPValue, RESPValue> item in map)
            {
                bool firstSubLine = true;
                string[] keyLines = item.Key.Accept(this).ToArray();

                foreach (string subKeyLine in keyLines[..^1])
                {
                    if (firstSubLine)
                    {
                        if (firstLine)
                        {
                            yield return keyPrefix + subKeyLine;
                            firstLine = false;
                        }
                        else
                        {
                            yield return currentIndentPrefix + keyPrefix + subKeyLine;
                        }

                        firstSubLine = false;
                    }
                    else
                    {
                        yield return subKeyLine;
                    }
                }

                string key = keyLines[^1];
                string itemKey = key + separator;

                foreach (string subLine in item.Value.Accept(this))
                {
                    if (firstSubLine)
                    {
                        if (firstLine)
                        {
                            yield return keyPrefix + itemKey + subLine;
                            firstLine = false;
                        }
                        else
                        {
                            yield return currentIndentPrefix + keyPrefix + itemKey + subLine;
                        }

                        firstSubLine = false;
                    }
                    else
                    {
                        yield return subLine;
                    }
                }
            }

            if (this.TryGetAttribsFootNote(map, out string? note))
            {
                yield return currentIndentPrefix + note;
            }

            this.indentPrefix = currentIndentPrefix;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<string> Visit(RESPNull nullValue)
    {
        Ensure.Param(nullValue).Done();

        yield return $"({RESPNull.HumanName}){this.AttribsNote(nullValue)}";
    }

    /// <inheritdoc/>
    public IEnumerable<string> Visit(RESPNumber number)
    {
        yield return $"({RESPNumber.HumanName}) {Ensure.Param(number).Value.Value}{this.AttribsNote(number)}";
    }

    /// <inheritdoc/>
    public IEnumerable<string> Visit(RESPPush push)
    {
        Ensure.Param(push).Done();

        if (push.IsEmpty)
        {
            throw new NotSupportedException("BUG: RESP push can not be empty");
        }

        int keySize = $"{push.Length + 1}".Length;
        const string separator = "} ";
        string indentIncr = new(' ', keySize + separator.Length);
        string currentIndentPrefix = this.indentPrefix;
        this.indentPrefix += indentIncr;
        bool firstLine = true;
        int key = 0;

        foreach (RESPValue item in push)
        {
            key++; // 1-based indexing
            string itemKey = $"{key}".PadLeft(keySize) + separator;

            bool firstSubLine = true;

            foreach (string subLine in item.Accept(this))
            {
                if (firstSubLine)
                {
                    if (firstLine)
                    {
                        yield return itemKey + subLine;
                        firstLine = false;
                    }
                    else
                    {
                        yield return currentIndentPrefix + itemKey + subLine;
                    }

                    firstSubLine = false;
                }
                else
                {
                    yield return subLine;
                }
            }
        }

        if (this.TryGetAttribsFootNote(push, out string? note))
        {
            yield return currentIndentPrefix + note;
        }

        this.indentPrefix = currentIndentPrefix;
    }

    /// <inheritdoc/>
    public IEnumerable<string> Visit(RESPSet setValue)
    {
        Ensure.Param(setValue).Done();

        if (setValue.IsEmpty)
        {
            yield return $"(empty {RESPSet.HumanName}){this.AttribsNote(setValue)}";
        }
        else
        {
            const string key = "~";
            const string separator = " ";
            string indentIncr = new(' ', key.Length + separator.Length);
            string currentIndentPrefix = this.indentPrefix;
            this.indentPrefix += indentIncr;
            bool firstLine = true;
            const string itemKey = key + separator;

            foreach (RESPValue item in setValue)
            {
                bool firstSubLine = true;

                foreach (string subLine in item.Accept(this))
                {
                    if (firstSubLine)
                    {
                        if (firstLine)
                        {
                            yield return itemKey + subLine;
                            firstLine = false;
                        }
                        else
                        {
                            yield return currentIndentPrefix + itemKey + subLine;
                        }

                        firstSubLine = false;
                    }
                    else
                    {
                        yield return subLine;
                    }
                }
            }

            if (this.TryGetAttribsFootNote(setValue, out string? note))
            {
                yield return currentIndentPrefix + note;
            }

            this.indentPrefix = currentIndentPrefix;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<string> Visit(RESPSimpleError simpleError)
    {
        Ensure.Param(simpleError).Done();

        string? stringValue = default;

        try
        {
            stringValue = simpleError.StringValue;
        }
        catch (ArgumentException)
        {
            // no UTF-8 string
        }

        if (stringValue is null)
        {
            yield return Dump.Literal(TokenWithValue.ToTokenWithValue(simpleError.Value))
                    + this.AttribsNote(simpleError);
        }
        else
        {
            yield return Dump.Literal(stringValue)
                    + this.AttribsNote(simpleError);
        }
    }

    /// <inheritdoc/>
    public IEnumerable<string> Visit(RESPSimpleString simpleString)
    {
        Ensure.Param(simpleString).Done();

        string? stringValue = default;

        try
        {
            stringValue = simpleString.StringValue;
        }
        catch (ArgumentException)
        {
            // no UTF-8 string
        }

        if (stringValue is null)
        {
            yield return Dump.Literal(TokenWithValue.ToTokenWithValue(simpleString.Value))
                    + this.AttribsNote(simpleString);
        }
        else
        {
            yield return Dump.Literal(stringValue)
                    + this.AttribsNote(simpleString);
        }
    }

    /// <inheritdoc/>
    public IEnumerable<string> Visit(RESPVerbatimString verbatimString)
    {
        Ensure.Param(verbatimString).Done();

        string? stringValue = default;
        string currentIndentPrefix = this.indentPrefix;

        try
        {
            stringValue = verbatimString.StringValue;
        }
        catch (ArgumentException)
        {
            // no UTF-8 string
        }

        switch (verbatimString.StringType)
        {
            case VerbatimStringType.Text:
                yield return $"({RESPNames.VerbatimTextStringPrefix} {RESPVerbatimString.HumanName})";
                break;
            case VerbatimStringType.Markdown:
                yield return $"({RESPNames.VerbatimMarkdownStringPrefix} {RESPVerbatimString.HumanName})";
                break;
            default:
                throw new NotSupportedException($"BUG: verbatim string type {verbatimString.StringType} is not supported.");
        }

        if (stringValue is not null)
        {
            foreach (string l in stringValue.Split(new[] { "\r\n", "\n\r", "\r", "\n" }, StringSplitOptions.None))
            {
                yield return currentIndentPrefix + l;
            }
        }
        else
        {
            yield return currentIndentPrefix + CLikeStringLiteralDefinition.Minimal.Encode(
                    TokenWithValue.ToTokenWithValue(verbatimString.Value));
        }

        if (this.TryGetAttribsFootNote(verbatimString, out string? note))
        {
            yield return currentIndentPrefix + note;
        }
    }

    private static string GetNumberString(double value)
    {
        /*
        > The number itself consists of an integral part, an optional fractional
        > part and an optional exponent part. The integral part consists of one
        > or more decimal digits. The optional fractional part consists of a dot (.)
        > followed by one or more decimal digits. The optional exponent part consists
        > of E or e, an optional + or - and one or more decimal digits.

        https://github.com/redis/redis-specifications/blob/master/protocol/RESP3.md
        */

        if (double.IsNaN(value))
        {
            // Note that Redis prior to version 7.2 may return any representation
            // of NaN produced by libc, such as "-nan", "NAN" or "nan(char-sequence)".
            return RESPNames.DoubleNaN;
        }
        else if (double.IsPositiveInfinity(value))
        {
            return RESPNames.DoublePositiveInfinity;
        }
        else if (double.IsNegativeInfinity(value))
        {
            return RESPNames.DoubleNegativeInfinity;
        }
        else
        {
            return value.ToString("g", CultureInfo.InvariantCulture);
        }
    }

    private string AttribsNote(RESPValue value)
    {
        return this.ValueAttribsHandling switch
        {
            AttribsHandling.Denote => value.Attribs is null ? string.Empty : this.AttribsDenoteCharacter,
            AttribsHandling.Hide => string.Empty,
            _ => throw new NotSupportedException($"BUG: value attribs handling {this.ValueAttribsHandling} is not supported"),
        };
    }

    private bool TryGetAttribsFootNote(
            RESPValue value,
            [NotNullWhen(returnValue: true)] out string? note)
    {
        note = $"[{this.AttribsNote(value)}]";

        return this.ValueAttribsHandling switch
        {
            AttribsHandling.Denote => value.Attribs is not null,
            AttribsHandling.Hide => false,
            _ => throw new NotSupportedException($"BUG: value attribs handling {this.ValueAttribsHandling} is not supported"),
        };
    }
}
