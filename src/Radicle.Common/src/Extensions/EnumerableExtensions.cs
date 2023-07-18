namespace Radicle.Common.Extensions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Radicle.Common.Check;

/// <summary>
/// Collection of enumeration extensions.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Like <see cref="List{T}.AddRange(IEnumerable{T})"/>,
    /// but for all collections.
    /// </summary>
    /// <typeparam name="T">The type of the item value.</typeparam>
    /// <param name="destination">The destination.</param>
    /// <param name="source">The source.</param>
    /// <returns>The destination collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required arguments are <see langword="null" />.</exception>
    public static ICollection<T> Extend<T>(
            this ICollection<T> destination,
            IEnumerable<T> source)
    {
        Ensure.Param(destination).Done();

        foreach (T item in Ensure.Param(source))
        {
            destination.Add(item);
        }

        return destination;
    }

    /// <summary>
    /// Like <see cref="List{T}.AddRange(IEnumerable{T})"/>,
    /// adds all the keys from the source collection into the destination
    /// but only and only if the <paramref name="source"/> is
    /// non <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of the item value.</typeparam>
    /// <param name="destination">The destination.</param>
    /// <param name="source">The source, may be <see langword="null"/>.</param>
    /// <returns>The destination collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if <paramref name="destination"/>
    ///     is <see langword="null"/>.</exception>
    public static ICollection<T> ExtendIfAny<T>(
            this ICollection<T> destination,
            IEnumerable<T>? source = null)
    {
        if (source is null)
        {
            return Ensure.Param(destination).Value;
        }

        return destination.Extend(source);
    }

    /// <summary>
    /// Batches the source collection into array batches.
    /// </summary>
    /// <typeparam name="T">Type of elements in
    ///     <paramref name="source"/> sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="batchSize">Size of the batches.</param>
    /// <returns>A sequence of array batches sized according to <paramref name="batchSize"/>
    ///     containing elements of the source collection. The returned batches
    ///     may be of different length than the provided <paramref name="batchSize"/>,
    ///     however never empty.</returns>
    /// <remarks>
    /// This operator uses deferred execution.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required arguments are <see langword="null" />.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if
    ///     <paramref name="batchSize"/> is negative or zero.</exception>
    public static IEnumerable<T[]> ArrayBatch<T>(
            this IEnumerable<T> source,
            int batchSize)
    {
        // see the implementation
        return Ensure.Param(source).Value.Batch(batchSize).Cast<T[]>();
    }

    /// <summary>
    /// Batches the source collection into batches.
    /// </summary>
    /// <typeparam name="T">Type of elements in
    ///     <paramref name="source"/> sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="batchSize">Size of the batches.</param>
    /// <returns>A sequence of batches sized according to <paramref name="batchSize"/>
    ///     containing elements of the source collection. The returned batches
    ///     may be of different length than the provided <paramref name="batchSize"/>,
    ///     however never empty.</returns>
    /// <remarks>
    /// This operator uses deferred execution.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required arguments are <see langword="null" />.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if
    ///     <paramref name="batchSize"/> is negative or zero.</exception>
    public static IEnumerable<IEnumerable<T>> Batch<T>(
            this IEnumerable<T> source,
            int batchSize)
    {
        Ensure.Param(batchSize).StrictlyPositive().Done();

        return source.Batch(() => batchSize);
    }

    /// <summary>
    /// Batches the source collection into batches.
    /// </summary>
    /// <typeparam name="T">Type of elements in
    ///     <paramref name="source"/> sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="batchSizeGetter">Caller mutable size of the batches,
    ///     the returned value is always clipped to 1 if it is lower than 1.
    ///     Mutability is handy for long running loops where there is a desire
    ///     to adjust the batch size during the enumeration. This getter
    ///     is called always ones before every batch.</param>
    /// <returns>A sequence of batches sized according to value of the
    ///     <paramref name="batchSizeGetter"/> at the beginning of the batch creation
    ///     containing elements of the source collection. The returned batches
    ///     may be of different length than provided <paramref name="batchSizeGetter"/>,
    ///     however never empty.</returns>
    /// <remarks>
    /// This operator uses deferred execution. If the value returned by
    /// <paramref name="batchSizeGetter"/> drops to zero or lower during
    /// enumeration execution, it will be clipped to value of 1.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required arguments are <see langword="null" />.</exception>
    public static IEnumerable<IEnumerable<T>> Batch<T>(
            this IEnumerable<T> source,
            Func<int> batchSizeGetter)
    {
        Ensure.Param(source).Done();
        Ensure.Param(batchSizeGetter).Done();

        T[]? currentBucket = null;
        int currentBatchSize = 0;
        int size = 1;
        bool getSizeFromGetter = true;

        // defer execution as much as possible, see MoreLinq library
        IEnumerable<IEnumerable<T>> Result()
        {
            foreach (T item in source)
            {
                // in this way we avoid any unnecessary
                // evaluation of batchSizeGetter
                if (getSizeFromGetter)
                {
                    size = Math.Max(batchSizeGetter(), 1);
                    getSizeFromGetter = false;
                }

                currentBucket ??= new T[size];
                currentBucket[currentBatchSize++] = item;

                if (currentBatchSize == size)
                {
                    yield return currentBucket;

                    currentBucket = null;
                    currentBatchSize = 0;
                    getSizeFromGetter = true;
                }
            }

            // Return the last bucket with all remaining elements
            if (currentBatchSize > 0)
            {
                Array.Resize(ref currentBucket, currentBatchSize);
                yield return currentBucket;
            }
        }

        // This will make sure the Ensure kicks
        // even if the enumeration is not used
        return Result();
    }

    /// <summary>
    /// Subtract <paramref name="collectionToSubstract"/>
    /// from the given collection using default equality operator
    /// and returns result as new enumeration.
    /// Inputs can contain repeating items and subtraction will work,
    /// e.g.: [ 1, 1, 2, 3 ] - [ 1, 3 ] = [ 1, 2 ], always first
    /// occurrence is removed. This is not set difference.
    /// </summary>
    /// <typeparam name="T">Type of elements in
    ///     <paramref name="source"/> sequence.</typeparam>
    /// <param name="source">Source collection.</param>
    /// <param name="collectionToSubstract">Collection to subtract.</param>
    /// <returns>Enumeration with subtracted values.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required arguments are <see langword="null" />.</exception>
    public static IEnumerable<T> Sub<T>(
            this IEnumerable<T> source,
            IEnumerable<T> collectionToSubstract)
    {
        Ensure.Param(source).Done();
        Ensure.Param(collectionToSubstract).Done();

        IEnumerable<T> Result()
        {
            List<T> substractList = collectionToSubstract.ToList();

            foreach (T item in source)
            {
                // if the item was not in sub return it
                if (!substractList.Remove(item))
                {
                    yield return item;
                }
            }
        }

        // This will make sure the Ensure kicks
        // even if the enumeration is not used
        return Result();
    }

    /// <summary>
    /// Prolongs (multiply) by repeating given collection <paramref name="source"/>.
    /// </summary>
    /// <typeparam name="T">Type of elements in
    ///     <paramref name="source"/> sequence.</typeparam>
    /// <param name="source">Source collection.</param>
    /// <param name="multiplier">Multiplier value.</param>
    /// <param name="delimiter">Optional delimiter.</param>
    /// <returns>Enumeration created by concatenation of <paramref name="multiplier"/>
    ///     amount of copies of the given <paramref name="source"/>
    ///     with optional <paramref name="delimiter"/>. Returns empty
    ///     <see cref="IEnumerable{T}"/> if the <paramref name="multiplier"/>
    ///     is zero or negative.</returns>
    /// <exception cref="ArgumentNullException">Thrown if required arguments are <see langword="null" />.</exception>
    public static IEnumerable<T> Mul<T>(
            this IEnumerable<T> source,
            int multiplier,
            IEnumerable<T>? delimiter = null)
    {
        Ensure.Param(source).Done();

        if (multiplier <= 0)
        {
            return Array.Empty<T>();
        }

        IEnumerable<T> Result()
        {
            if (multiplier > 0)
            {
                for (int i = 0; i < multiplier; i++)
                {
                    if (delimiter is not null && i > 0)
                    {
                        foreach (T delimeterItem in delimiter)
                        {
                            yield return delimeterItem;
                        }
                    }

                    foreach (T item in source)
                    {
                        yield return item;
                    }
                }
            }
        }

        // This will make sure the Ensure kicks
        // even if the enumeration is not used
        return Result();
    }

    /// <summary>
    /// Create collection of random items from
    /// the source by random pick.
    /// </summary>
    /// <typeparam name="T">Type of elements in
    ///     <paramref name="source"/> sequence.</typeparam>
    /// <param name="source">source array, note that any empty
    ///     source will lead to empty result.</param>
    /// <param name="count">length of new array.</param>
    /// <param name="fuzzy">Opt for fuzzy length of new
    ///     enumeration instead of fixed <paramref name="count"/>
    ///     (the returned enumeration length is in
    ///     inclusive range <paramref name="min"/>
    ///     to <paramref name="count"/>).</param>
    /// <param name="min">minimum length of new array
    ///     if <paramref name="fuzzy"/>
    ///     is set to <see langword="true"/>,
    ///     values larger than <paramref name="count"/>
    ///     will be clipped to value of <paramref name="count"/>.</param>
    /// <returns>Enumeration with random picks. If the <paramref name="source"/>
    ///     changed during the execution of this method
    ///     outdated items may be returned.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required arguments are <see langword="null" />.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="count"/> or <paramref name="min"/>
    ///     is negative.</exception>
    public static IEnumerable<T> RandomPick<T>(
            this IEnumerable<T> source,
            int count = 1,
            bool fuzzy = false,
            int min = 1)
    {
        Ensure.Param(source).Done();
        Ensure.Param(count).NonNegative().Done();
        Ensure.Param(min).NonNegative().Done();

        if (source is not (IList<T> or IReadOnlyList<T>))
        {
            source = source.ToArray();
        }

        ReadOnlyListWrap<T> list = new(source);

        if (list.Count != 0 && count > 0)
        {
            min = min > count ? count : min;
            int length = fuzzy
                    ? min + ThreadSafeRandom.Next(count + 1 - min)
                    : count;

            for (int i = 0; i < length; i++)
            {
                T next;

                while (true)
                {
                    try
                    {
                        next = list[ThreadSafeRandom.Next(list.Count)];
                        break;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        if (list.Count == 0)
                        {
                            yield break;
                        }
                    }
                }

                yield return next;
            }
        }
    }

    /// <summary>
    /// Shuffles list with Fisher–Yates shuffle
    /// algorithm. The execution time scales
    /// linearly with the length of <paramref name="source"/>
    /// as O(N).
    /// </summary>
    /// <typeparam name="TList">Collection type of <paramref name="source"/>.</typeparam>
    /// <param name="source">List to shuffle.</param>
    /// <returns>Instance which was shuffled.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static TList Shuffle<TList>(
            this TList source)
        where TList : IList
    {
        int n = Ensure.Param(source).Value.Count;

        if (n > 1)
        {
            while (n > 1)
            {
                n--;
                int randomIndex = ThreadSafeRandom.Next(n + 1);
                (source[n], source[randomIndex]) = (source[randomIndex], source[n]);
            }
        }

        return source;
    }

    /// <summary>
    /// Shuffles list with Fisher–Yates shuffle
    /// algorithm. The execution time scales
    /// linearly with the length of <paramref name="source"/>
    /// as O(N).
    /// </summary>
    /// <typeparam name="T">Item type of <paramref name="source"/>.</typeparam>
    /// <param name="source">List to shuffle.</param>
    /// <returns>Instance which was shuffled.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public static IList<T> Shuffle<T>(
            this IList<T> source)
    {
        int n = Ensure.Param(source).Value.Count;

        if (n > 1)
        {
            while (n > 1)
            {
                n--;
                int randomIndex = ThreadSafeRandom.Next(n + 1);
                (source[n], source[randomIndex]) = (source[randomIndex], source[n]);
            }
        }

        return source;
    }
}
