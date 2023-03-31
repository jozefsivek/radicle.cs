namespace Radicle.Alike.Redis.Models;

/// <summary>
/// Enumeration of Redis RESP data types.
/// Currently for RESP3, see https://github.com/redis/redis-specifications/blob/master/protocol/RESP3.md
/// for reference.
/// </summary>
public enum RESPDataType
{
    /*
          ,- RESP3
          |     ,- RESP2
          |    /
        ,--. ,--.
        0000 0000
        |      |`- error flag
        |      `- collection flag
        `- meta flag


         */

    /// <summary>
    /// RESP2: Simple string: a space efficient non binary safe string.
    /// </summary>
    SimpleString = 0,

    /// <summary>
    /// RESP2: Simple error: a space efficient non binary safe error code and message.
    /// </summary>
    SimpleError = RESPDataTypeFlags.ErrorLike,

    /// <summary>
    /// RESP2: Array: an ordered collection of N other types.
    /// </summary>
    Array = RESPDataTypeFlags.CollectionLike,

    /// <summary>
    /// RESP2: Number: an integer in the signed 64 bit range.
    /// </summary>
    Number = 4,

    /// <summary>
    /// RESP2: Blob string: binary safe strings.
    /// </summary>
    BlobString = 8,

    /// <summary>
    /// RESP3 (RESP2): Null: a single null value replacing RESP2 v2 *-1 and $-1 null values.
    /// </summary>
    Null = 8 + 4,

    /// <summary>
    /// RESP3: Double: a floating point number.
    /// </summary>
    FloatingPointNumber = 16,

    /// <summary>
    /// RESP3: Blob error: binary safe error code and message.
    /// </summary>
    BlobError = 16 + RESPDataTypeFlags.ErrorLike,

    /// <summary>
    /// RESP3: Map: an unordered collection of key-value pairs.
    /// Keys and values can be any other RESP3 type.
    /// </summary>
    Map = RESPDataTypeFlags.MapLike,

    /// <summary>
    /// RESP3: Boolean: true or false.
    /// </summary>
    Boolean = 32,

    /// <summary>
    /// RESP3: Set: an unordered collection of N other types.
    /// </summary>
    Set = 32 + RESPDataTypeFlags.CollectionLike,

    /// <summary>
    /// RESP3: Verbatim string: a binary safe string that should be displayed
    /// to humans without any escaping or filtering.
    /// For instance the output of LATENCY DOCTOR in Redis.
    /// </summary>
    VerbatimString = 32 + 16,

    /// <summary>
    /// RESP3: Big number: a large number non representable by the Number type.
    /// </summary>
    BigNumber = 64,

    /// <summary>
    /// RESP3: Push: Out of band data. The format is like the Array type,
    /// but the client should just check the first string element,
    /// stating the type of the out of band data, a call a callback
    /// if there is one registered for this specific type of push information.
    /// Push types are not related to replies, since they are information
    /// that the server may push at any time in the connection,
    /// so the client should keep reading if it is reading the reply of a command.
    /// </summary>
    Push = 64 + RESPDataTypeFlags.CollectionLike,

    /// <summary>
    /// RESP3: Attribute: Like the Map type, but the client should keep
    /// reading the reply ignoring the attribute type,
    /// and return it to the client as additional information.
    /// </summary>
    Attribute = RESPDataTypeFlags.Meta + RESPDataTypeFlags.MapLike,

    /* not used as this type is literally just a map returned on hello command in Redis now
    /// <summary>
    /// RESP3: Hello: Like the Map type, but is sent only when the connection
    /// between the client and the server is established, in order to welcome
    /// the client with different information like the name of the server,
    /// its version, and so forth.
    /// </summary>
    Hello = RESPDataTypeFlags.Meta + 32 + Map,
    */
}
