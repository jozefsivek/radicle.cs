namespace Radicle.Alike.Redis.Models;

using System;

/// <summary>
/// Interface of RESP value visitor.
/// </summary>
/// <typeparam name="T">Return type.</typeparam>
public interface IRESPValueVisitor<T>
{
    /// <summary>
    /// Visit value.
    /// </summary>
    /// <param name="array">Array.</param>
    /// <returns>Visit result.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    T Visit(RESPArray array);

    /// <summary>
    /// Visit value.
    /// </summary>
    /// <param name="attribute">Attribute.</param>
    /// <returns>Visit result.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    T Visit(RESPAttributeValue attribute);

    /// <summary>
    /// Visit value.
    /// </summary>
    /// <param name="bigNumber">Big number.</param>
    /// <returns>Visit result.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    T Visit(RESPBigNumber bigNumber);

    /// <summary>
    /// Visit value.
    /// </summary>
    /// <param name="blobError">Blob error.</param>
    /// <returns>Visit result.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    T Visit(RESPBlobError blobError);

    /// <summary>
    /// Visit value.
    /// </summary>
    /// <param name="blobString">Blob String.</param>
    /// <returns>Visit result.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    T Visit(RESPBlobString blobString);

    /// <summary>
    /// Visit value.
    /// </summary>
    /// <param name="booleanValue">Boolean.</param>
    /// <returns>Visit result.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    T Visit(RESPBoolean booleanValue);

    /// <summary>
    /// Visit value.
    /// </summary>
    /// <param name="doubleValue">Double.</param>
    /// <returns>Visit result.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    T Visit(RESPDouble doubleValue);

    /// <summary>
    /// Visit value.
    /// </summary>
    /// <param name="map">Map.</param>
    /// <returns>Visit result.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    T Visit(RESPMap map);

    /// <summary>
    /// Visit value.
    /// </summary>
    /// <param name="nullValue">Null.</param>
    /// <returns>Visit result.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    T Visit(RESPNull nullValue);

    /// <summary>
    /// Visit value.
    /// </summary>
    /// <param name="number">Number.</param>
    /// <returns>Visit result.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    T Visit(RESPNumber number);

    /// <summary>
    /// Visit value.
    /// </summary>
    /// <param name="push">Push.</param>
    /// <returns>Visit result.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    T Visit(RESPPush push);

    /// <summary>
    /// Visit value.
    /// </summary>
    /// <param name="setValue">Set.</param>
    /// <returns>Visit result.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    T Visit(RESPSet setValue);

    /// <summary>
    /// Visit value.
    /// </summary>
    /// <param name="simpleError">Simple error.</param>
    /// <returns>Visit result.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    T Visit(RESPSimpleError simpleError);

    /// <summary>
    /// Visit value.
    /// </summary>
    /// <param name="simpleString">Simple string.</param>
    /// <returns>Visit result.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    T Visit(RESPSimpleString simpleString);

    /// <summary>
    /// Visit value.
    /// </summary>
    /// <param name="verbatimString">Verbatim string.</param>
    /// <returns>Visit result.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    T Visit(RESPVerbatimString verbatimString);
}
