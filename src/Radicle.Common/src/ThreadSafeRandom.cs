namespace Radicle.Common;

using System;
using System.Threading;

/// <summary>
/// Implementation of thread safe random class
/// proxy for <see cref="Random"/>.
/// </summary>
/// <remarks>
/// See https://codeblog.jonskeet.uk/2009/11/04/revisiting-randomness/
/// and https://stackoverflow.com/questions/1667516/doesnt-passing-in-parameters-that-should-be-known-implicitly-violate-encapsulat/1667590#1667590 .
/// </remarks>
public static class ThreadSafeRandom
{
#pragma warning disable CA5394 // Random is an insecure random number generator

    /// <summary>
    /// Seed source.
    /// </summary>
    private static readonly Random Rnd = new();

    /// <summary>
    /// Lock for <see cref="Rnd"/>.
    /// </summary>
    private static readonly object RndLock = new();

    private static readonly ThreadLocal<Random> ThreadRnd = new(ConstructRandom);

    /// <summary>
    /// Gets thread local instance of
    /// <see cref="Random"/> (on its own is not thread safe).
    /// </summary>
    public static Random Instance => ThreadRnd.Value;

    /// <summary>
    /// Construct new instance of <see cref="Random"/>
    /// with random seed.
    /// </summary>
    /// <returns>Instance of <see cref="Random"/>.</returns>
    public static Random ConstructRandom()
    {
        lock (RndLock)
        {
            return new Random(Rnd.Next());
        }
    }

    // Proxy methods

    /// <summary>
    /// Thread safe proxy for <see cref="Random.Next()"/>.
    /// </summary>
    /// <returns>A 32-bit signed integer that is greater
    ///     than or equal to 0 and less than
    ///     <see cref="int.MaxValue"/>.</returns>
    public static int Next()
    {
        return Instance.Next();
    }

    /// <summary>
    /// Thread safe proxy for <see cref="Random.Next(int)"/>.
    /// </summary>
    /// <param name="maxValue">The exclusive upper bound of
    ///     the random number to be generated. <paramref name="maxValue"/>
    ///     must be greater than or equal to 0.</param>
    /// <returns>A 32-bit signed integer that is greater than
    ///     or equal to 0, and less than maxValue;
    ///     that is, the range of return values ordinarily includes 0
    ///     but not maxValue. However, if maxValue equals 0,
    ///     maxValue is returned.</returns>
    public static int Next(int maxValue)
    {
        return Instance.Next(maxValue);
    }

    /// <summary>
    /// Thread safe proxy for <see cref="Random.Next(int, int)"/>.
    /// </summary>
    /// <param name="minValue">The inclusive lower bound
    ///     of the random number returned.</param>
    /// <param name="maxValue">The exclusive upper bound
    ///     of the random number returned. <paramref name="maxValue"/>
    ///     must be greater than or equal to minValue.</param>
    /// <returns>A 32-bit signed integer greater than or equal
    ///     to minValue and less than maxValue; that is,
    ///     the range of return values includes minValue
    ///     but not <paramref name="maxValue"/>.
    ///     If <paramref name="minValue"/> equals <paramref name="maxValue"/>,
    ///     <paramref name="minValue"/> is returned.</returns>
    public static int Next(int minValue, int maxValue)
    {
        return Instance.Next(minValue, maxValue);
    }

    /// <summary>
    /// Thread safe proxy for <see cref="Random.NextBytes(byte[])"/>.
    /// </summary>
    /// <param name="buffer">An array of bytes to contain random numbers.</param>
    public static void NextBytes(byte[] buffer)
    {
        Instance.NextBytes(buffer);
    }

    /// <summary>
    /// Thread safe proxy for <see cref="Random.NextDouble"/>.
    /// </summary>
    /// <returns>A double-precision floating point number that
    ///     is greater than or equal to 0.0, and less than 1.0.</returns>
    public static double NextDouble()
    {
        return Instance.NextDouble();
    }

#pragma warning restore CA5394 // Random is an insecure random number generator
}
