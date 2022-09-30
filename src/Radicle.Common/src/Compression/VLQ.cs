namespace Radicle.Common.Compression;

using System;
using System.Collections.Generic;
using Radicle.Common.Check;

/// <summary>
/// No hassle API for Variable-length quantity (VLQ),
/// an unsigned variable byte length big endian serialization of integer numbers
/// https://en.wikipedia.org/wiki/Variable-length_quantity .
/// </summary>
public static class VLQ
{
    private const byte EncodingPayloadMask = 0b_0111_1111;

    private const byte ContinuationFlag = 0b_1000_0000;

    private const byte TerminalFlag = 0b_0000_0000;

    /// <summary>
    /// Claculate encoded payload byte length
    /// for given <paramref name="value"/>.
    /// </summary>
    /// <param name="value">Value to compute
    ///     encoded payload length for.</param>
    /// <returns>Length of the payload in bytes larger or
    ///     equal to 1.</returns>
    public static byte GetEncodePayloadLength(
            ulong value)
    {
        if (value == 0uL)
        {
            return 1;
        }

        byte payloadLength = 0;
        ulong v = value;

        while (v != 0uL)
        {
            payloadLength++;
            v >>= 7;
        }

        return payloadLength;
    }

    /// <summary>
    /// Encode given <paramref name="value"/> to bytes.
    /// </summary>
    /// <param name="value">Value to encode.</param>
    /// <returns>Enumeration of the encoded payload bytes
    ///     in given order.</returns>
    public static IEnumerable<byte> Encode(
            ulong value)
    {
        if (value == 0uL)
        {
            yield return TerminalFlag;
        }
        else
        {
            byte encodeLength = GetEncodePayloadLength(value);

            for (int i = encodeLength - 1; i >= 0; i--)
            {
                byte flag = i == 0
                        ? TerminalFlag
                        : ContinuationFlag;

                yield return (byte)(
                        ((value >> (i * 7)) & EncodingPayloadMask) | flag);
            }
        }
    }

    /// <summary>
    /// Encode given <paramref name="value"/> to given
    /// <paramref name="destination"/> starting at given
    /// <paramref name="destinationIndex"/>.
    /// </summary>
    /// <param name="destination">Destination array to write to.</param>
    /// <param name="value">Value to encode.</param>
    /// <param name="destinationIndex">Position
    ///     where to write the encoded value to.</param>
    /// <returns>Destination index pointing to byte after the encoded value.
    ///     I.e. if <paramref name="destinationIndex"/> was 0,
    ///     the encoded value required 2 B, the result
    ///     value will be 2.</returns>
    /// <exception cref="ArgumentNullException">Thrown if required
    ///     value is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if
    ///     <paramref name="destinationIndex"/> is negative.</exception>
    /// <exception cref="IndexOutOfRangeException">Thrown if
    ///      <paramref name="destinationIndex"/> is equal to
    ///     or greater than <paramref name="destination"/> length;
    ///     or the <paramref name="value"/>
    ///     can not fit into the <paramref name="destination"/>.</exception>
    public static int EncodeTo(
            byte[] destination,
            ulong value,
            int destinationIndex = 0)
    {
        Ensure.Param(destination).Done();
        Ensure.Param(destinationIndex).NonNegative().Done();

        foreach (byte b in Encode(value))
        {
            destination[destinationIndex++] = b;
        }

        return destinationIndex;
    }

    /// <summary>
    /// Decode Variable-length quantity byte by byte until
    /// <see langword="false"/> is returned.
    /// </summary>
    /// <param name="byteValue">Current byte to decode. Bytes should be
    ///     read in order as they appear.</param>
    /// <param name="decoded">Current output decoded value.
    ///     Input which overflows <see cref="ulong.MaxValue"/>
    ///     will be wrapped (VLQ with more than 9 bytes).</param>
    /// <param name="lastDecoded">Decoded value from the last call
    ///     to this method. This value will be merged with the decoded
    ///     value read from the current <paramref name="byteValue"/>
    ///     while adjusting its size.</param>
    /// <returns><see langword="true"/> if the <paramref name="byteValue"/>
    ///     is continuation byte and next byte should be read
    ///     in order to proceed with the decoding;
    ///     <see langword="false"/> if the <paramref name="byteValue"/>
    ///     is last byte of VLQ and <paramref name="decoded"/>
    ///     thus holds the full decoded value.</returns>
    public static bool DecodeReadNext(
            byte byteValue,
            out ulong decoded,
            ulong lastDecoded = 0uL)
    {
        unchecked
        {
            decoded = (lastDecoded << 7)
                    + (ulong)(byteValue & EncodingPayloadMask);
        }

        return (byteValue & ContinuationFlag) != 0;
    }

    /// <summary>
    /// Decode the <paramref name="input"/> until
    /// one Variable-length quantity is retrieved.
    /// </summary>
    /// <param name="input">Input enumeration to decode,
    ///     note only as much data is read as needed to
    ///     decode first Variable-length quantity.</param>
    /// <returns>Decoded value. Note the input which overflows
    ///     <see cref="ulong.MaxValue"/> will be wrapped
    ///     (i.e. do not decode such large values,
    ///     i.e. VLQ with more than 9 bytes).</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if the <paramref name="input"/> is empty
    ///     or too short to decode any Variable-length quantity.</exception>
    /// <exception cref="ArgumentNullException">Thrown if required
    ///     parameter is <see langword="null"/>.</exception>
    public static ulong DecodeFirstFrom(IEnumerable<byte> input)
    {
        ulong lastValue = 0uL;

        foreach (byte b in Ensure.Param(input).Value)
        {
            if (!DecodeReadNext(b, out ulong value, lastDecoded: lastValue))
            {
                return value;
            }

            lastValue = value;
        }

        throw new ArgumentOutOfRangeException(
                nameof(input),
                "Insufficient input length.");
    }

    /// <summary>
    /// Decode the <paramref name="input"/> fully
    /// to retrieve exactly one Variable-length quantity.
    /// </summary>
    /// <param name="input">Input enumeration to decode in full.</param>
    /// <returns>Decoded value. Note the input which overflows
    ///     <see cref="ulong.MaxValue"/> will be wrapped
    ///     (i.e. do not decode such large values,
    ///     i.e. VLQ with more than 9 bytes).</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if the <paramref name="input"/> is empty
    ///     or too short to decode any Variable-length quantity.</exception>
    /// <exception cref="ArgumentException">Thrown if
    ///     <paramref name="input"/> contains more data
    ///     than needed to decode one VLQ.</exception>
    /// <exception cref="ArgumentNullException">Thrown if required
    ///     parameter is <see langword="null"/>.</exception>
    public static ulong Decode(IEnumerable<byte> input)
    {
        ulong lastValue = 0uL;
        bool decoded = false;

        foreach (byte b in Ensure.Param(input).Value)
        {
            if (decoded)
            {
                throw new ArgumentException(
                        "Excess input length.",
                        nameof(input));
            }

            if (!DecodeReadNext(b, out ulong value, lastDecoded: lastValue))
            {
                decoded = true;
            }

            lastValue = value;
        }

        if (decoded)
        {
            return lastValue;
        }

        throw new ArgumentOutOfRangeException(
                nameof(input),
                "Insufficient input length.");
    }
}
