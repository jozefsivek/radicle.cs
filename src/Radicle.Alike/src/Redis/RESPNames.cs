namespace Radicle.Alike.Redis;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Radicle.Alike.Redis.Models;

/// <summary>
/// Collection of RESP names and constants.
/// </summary>
internal static class RESPNames
{
    /// <summary>
    /// Initial byte of array.
    /// </summary>
    public const char Array = '*';

    /// <summary>
    /// Initial byte of attribute map.
    /// </summary>
    public const char Attribute = '|';

    /// <summary>
    /// Initial byte of big number.
    /// </summary>
    public const char BigNumber = '(';

    /// <summary>
    /// Initial byte of blob error.
    /// </summary>
    public const char BlobError = '!';

    /// <summary>
    /// Initial byte of blob string.
    /// </summary>
    public const char BlobString = '$';

    /// <summary>
    /// Initial byte of boolean.
    /// </summary>
    public const char Boolean = '#';

    /// <summary>
    /// Initial byte of double.
    /// </summary>
    public const char FloatingPointNumber = ',';

    /// <summary>
    /// Initial byte of map.
    /// </summary>
    public const char Map = '%';

    /// <summary>
    /// Initial byte of null.
    /// </summary>
    /// <remarks>RESP v2 *-1 and $-1 null values.</remarks>
    public const char Null = '_';

    /// <summary>
    /// Initial byte of number.
    /// </summary>
    public const char Number = ':';

    /// <summary>
    /// Initial byte of push array.
    /// </summary>
    public const char Push = '>';

    /// <summary>
    /// Initial byte of set.
    /// </summary>
    public const char Set = '~';

    /// <summary>
    /// Initial byte of simple error.
    /// </summary>
    public const char SimpleError = '-';

    /// <summary>
    /// Initial byte of simple string.
    /// </summary>
    public const char SimpleString = '+';

    /// <summary>
    /// Initial byte of verbatim string.
    /// </summary>
    public const char VerbatimString = '=';

    /// <summary>
    /// Type separator for verbatim string.
    /// </summary>
    public const char VerbatimStringTypeSeparator = ':';

    /// <summary>
    /// Prefix of text verbatim string.
    /// </summary>
    public const string VerbatimTextStringPrefix = "txt";

    /// <summary>
    /// Prefix of markdown verbatim string.
    /// </summary>
    public const string VerbatimMarkdownStringPrefix = "mkd";

    /// <summary>
    /// Double NaN (not a number).
    /// </summary>
    /// <remarks>
    /// Note that Redis prior to version 7.2 may return any representation
    /// of NaN produced by libc, such as "-nan", "NAN" or "nan(char-sequence)".
    /// </remarks>
    public const string DoubleNaN = "nan";

    /// <summary>
    /// Double positive infinity.
    /// </summary>
    public const string DoublePositiveInfinity = "inf";

    /// <summary>
    /// Double negative infinity.
    /// </summary>
    public const string DoubleNegativeInfinity = "-inf";

    /// <summary>
    /// Boolean true value.
    /// </summary>
    public const char BooleanTrue = 't';

    /// <summary>
    /// Boolean false value.
    /// </summary>
    public const char BooleanFalse = 'f';

    /// <summary>
    /// New line delimiter for AOF file format as string.
    /// </summary>
    public const string NewLineString = "\r\n";

    /// <summary>
    /// Collection of init bytes for RESP3.
    /// </summary>
    public static readonly ImmutableDictionary<int, RESPDataType> InitBytesRESP3 = new Dictionary<int, RESPDataType>()
    {
        { RESPNames.Null, RESPDataType.Null },
        { RESPNames.FloatingPointNumber, RESPDataType.FloatingPointNumber },
        { RESPNames.BlobError, RESPDataType.BlobError },
        { RESPNames.Map, RESPDataType.Map },
        { RESPNames.Boolean, RESPDataType.Boolean },
        { RESPNames.Set, RESPDataType.Set },
        { RESPNames.VerbatimString, RESPDataType.VerbatimString },
        { RESPNames.BigNumber, RESPDataType.BigNumber },
        { RESPNames.Push, RESPDataType.Push },
        { RESPNames.Attribute, RESPDataType.Attribute },
    }.ToImmutableDictionary();

    /// <summary>
    /// Gets default encoding used in models, i.e. UTF-8
    /// with enabled detection of errors.
    /// </summary>
    public static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);

    /// <summary>
    /// Null in RESP2 as array.
    /// </summary>
    public static readonly byte[] RESP2NullAltArray = new byte[] { (byte)'*', (byte)'-', (byte)'1' };

    /// <summary>
    /// Null in RESP2 as blob string.
    /// </summary>
    public static readonly byte[] RESP2NullAltBlobString = new byte[] { (byte)'$', (byte)'-', (byte)'1' };

    /// <summary>
    /// Null in RESP2 as '-1' length string.
    /// </summary>
    public static readonly byte[] RESP2NullLength = new byte[] { (byte)'-', (byte)'1' };

    /// <summary>
    /// New line delimiter for AOF file format.
    /// </summary>
    public static readonly byte[] NewLine = new byte[] { 0x0d, 0x0a }; // i.e. '\r' '\n'

    /// <summary>
    /// All bytes of new line delimiter for AOF file format.
    /// </summary>
    public static readonly ImmutableHashSet<byte> AllNewLineBytes = NewLine.ToImmutableHashSet();
}
