namespace Radicle.Common.Profiling;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Radicle.Common.Extensions;

/// <summary>
/// Concurrent counter utility for counting
/// amounts or time with given labels. This class is thread safe.
/// </summary>
public sealed class CountersDictionary : IReadOnlyDictionary<string, long>
{
    /// <summary>
    /// Default instance of the counter.
    /// </summary>
    public static readonly CountersDictionary Default = new();

    private static readonly TimeSpan TimeThresholdForSmallValues = TimeSpan.FromMilliseconds(100);

    /*
    this is easier than
    https://stackoverflow.com/questions/29108060/c-sharp-pass-element-of-value-type-array-by-reference
    and we can do it because of
    https://medium.com/gft-engineering/correctly-using-concurrentdictionarys-addorupdate-method-94b7b41719d6
    the values here are value types and the concurency model
    of the ConcurrentDictionary will work for values
    */
    private readonly ConcurrentDictionary<string, long> counters = new();

    private CounterStyle preferredCounterStyle = CounterStyle.Numeric;

    /// <summary>
    /// Gets a value indicating whether this instance of counters is empty.
    /// </summary>
    public bool IsEmpty => this.counters.IsEmpty;

    /// <inheritdoc/>
    public IEnumerable<string> Keys => this.counters.Keys;

    /// <inheritdoc/>
    public IEnumerable<long> Values => this.counters.Values;

    /// <inheritdoc/>
    public int Count => this.counters.Count;

    /// <inheritdoc/>
    public long this[string key] => this.counters[key];

    /// <summary>
    /// Start counting thread to measure time intervals
    /// in style of <see cref="Stopwatch"/>. Do not use returned
    /// <see cref="TimeThread"/> concurrently.
    /// </summary>
    /// <returns>Stopwatch.</returns>
    public TimeThread StartThread()
    {
        return new TimeThread(this);
    }

    /// <summary>
    /// Decrement counter of the given <paramref name="key"/>
    /// by the given <paramref name="amount"/>. Non existing
    /// counter is initialized with value 0.
    /// </summary>
    /// <param name="key">Counter key.</param>
    /// <param name="amount">Amount to decrement with.</param>
    /// <returns>Value of the counter after decrement.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public long Decr(string key, long amount = 1L)
    {
        // we will pass null check to save as much time as possible
        return this.counters.AddOrUpdate(
                key,
                _ => -amount,
                (_, v) => v - amount);
    }

    /// <summary>
    /// Increment counter of the given <paramref name="key"/>
    /// by the given <paramref name="amount"/>. Non existing
    /// counter is initialized with value 0.
    /// </summary>
    /// <param name="key">Counter key.</param>
    /// <param name="amount">Amount to increment with.</param>
    /// <returns>Value of the counter after increment.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public long Incr(string key, long amount = 1L)
    {
        // we will pass null check to save as much time as possible
        return this.counters.AddOrUpdate(
                key,
                _ => amount,
                (_, v) => v + amount);
    }

    /// <summary>
    /// Set counter of the given <paramref name="key"/>
    /// to specific <paramref name="value"/>, resetting
    /// any previously held one.
    /// </summary>
    /// <remarks>Use of this method will make this instance prefer
    /// time serialization with <see cref="ToString()"/>.</remarks>
    /// <param name="key">Counter key.</param>
    /// <param name="value">Value to set.</param>
    /// <returns>Value of the counter after the set.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public long Set(string key, long value)
    {
        // we will pass null check to save as much time as possible
        return this.counters.AddOrUpdate(
                key,
                _ => value,
                (_, _) => value);
    }

    /// <summary>
    /// Decrement counter of the given <paramref name="key"/>
    /// by the given <paramref name="elapsed"/> time. Non existing
    /// counter is initialized with zero value.
    /// </summary>
    /// <remarks>Use of this method will make this instance prefer
    /// time serialization with <see cref="ToString()"/>.</remarks>
    /// <param name="key">Counter key.</param>
    /// <param name="elapsed">Amount of time span to decrement with.</param>
    /// <returns>Value of the counter after decrement,
    ///     the time span is represented internally by ticks.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public TimeSpan Decr(string key, TimeSpan elapsed)
    {
        this.preferredCounterStyle = CounterStyle.TimeSpan;

        return TimeSpan.FromTicks(this.Decr(key, elapsed.Ticks));
    }

    /// <summary>
    /// Increment counter of the given <paramref name="key"/>
    /// by the given <paramref name="elapsed"/> time. Non existing
    /// counter is initialized with zero value.
    /// </summary>
    /// <remarks>Use of this method will make this instance prefer
    /// time serialization with <see cref="ToString()"/>.</remarks>
    /// <param name="key">Counter key.</param>
    /// <param name="elapsed">Amount of time span to increment with.</param>
    /// <returns>Value of the counter after increment,
    ///     the time span is represented internally by ticks.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public TimeSpan Incr(string key, TimeSpan elapsed)
    {
        this.preferredCounterStyle = CounterStyle.TimeSpan;

        return TimeSpan.FromTicks(this.Incr(key, elapsed.Ticks));
    }

    /// <summary>
    /// Set counter of the given <paramref name="key"/>
    /// to specific <paramref name="elapsed"/> time, resetting
    /// any previously held one.
    /// </summary>
    /// <param name="key">Counter key.</param>
    /// <param name="elapsed">Amount of time span to set.</param>
    /// <returns>Value of the counter after set.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public TimeSpan Set(string key, TimeSpan elapsed)
    {
        this.preferredCounterStyle = CounterStyle.TimeSpan;

        _ = this.Set(key, elapsed.Ticks);

        return elapsed;
    }

    /// <summary>
    /// Clear all counters and reset any preference in counter style.
    /// </summary>
    public void Clear()
    {
        this.counters.Clear();
        this.preferredCounterStyle = CounterStyle.Numeric;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return this.ToString(this.preferredCounterStyle);
    }

    /// <summary>
    /// Return string representation of this instance
    /// in the given <paramref name="style"/>. Note that if
    /// you use this instance exclusivelly for counters or time spans,
    /// you can just call <see cref="ToString()"/> and get expected result.
    /// </summary>
    /// <remarks><see cref="TimeSpan"/> values are represented in the format:
    /// hh:mm:ss.sss if large enough, or as milliseconds with fraction
    /// for very small values (e.g. 9.003145ms).
    /// </remarks>
    /// <param name="style">Enforse specific style.</param>
    /// <returns>String representation of this instance.</returns>
    public string ToString(CounterStyle style)
    {
        if (this.IsEmpty)
        {
            return "(empty)";
        }

        StringBuilder result = new();
        bool first = true;

        foreach (string k in this.Keys.OrderBy(k => k))
        {
            if (first)
            {
                first = false;
            }
            else
            {
                result = result.Append(Environment.NewLine);
            }

            string counter = "n/a";

            if (style == CounterStyle.TimeSpan && this.TryGetElapsed(k, out TimeSpan ts))
            {
                counter = Math.Abs(ts.TotalMilliseconds) < TimeThresholdForSmallValues.TotalMilliseconds
                        ? ts.TotalMilliseconds.ToString("F6", CultureInfo.InvariantCulture) + "ms"
                        : ts.ToCounter(secondsFloatingPrecission: 3);
            }
            else if (this.TryGetValue(k, out long numCounter))
            {
                counter = numCounter.ToString(CultureInfo.InvariantCulture);
            }

            result = result
                    .Append("- ")
                    .Append(k)
                    .Append(": ")
                    .Append(counter);
        }

        return result.ToString();
    }

    /// <inheritdoc/>
    public bool ContainsKey(string key)
    {
        return this.counters.ContainsKey(key);
    }

    /// <inheritdoc/>
    public bool TryGetValue(string key, out long value)
    {
        return this.counters.TryGetValue(key, out value);
    }

    /// <summary>
    /// Try to retrieve counter of given <paramref name="key"/>
    /// as time span.
    /// </summary>
    /// <param name="key">Key of the counter.</param>
    /// <param name="value">Value if any.</param>
    /// <returns><see langword="true"/>
    ///     if counter exist;
    ///     <see langword="false"/> otherwise.</returns>
    public bool TryGetElapsed(string key, out TimeSpan value)
    {
        value = default;

        if (this.counters.TryGetValue(key, out long numValue))
        {
            value = TimeSpan.FromTicks(numValue);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets <see cref="TimeSpan"/> counter under given <paramref name="key"/>.
    /// </summary>
    /// <param name="key">Counter key.</param>
    /// <returns>Instance of <see cref="TimeSpan"/>.</returns>
    /// <exception cref="KeyNotFoundException">Thrown
    ///     if <paramref name="key"/> is not part of collection.</exception>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public TimeSpan GetElapsed(string key)
    {
        long numValue = this[key];

        return TimeSpan.FromTicks(numValue);
    }

    /// <summary>
    /// Enumerate counters as <see cref="TimeSpan"/> values.
    /// Non time span values will be skipped
    /// (see <see cref="TryGetElapsed(string, out TimeSpan)"/>).
    /// </summary>
    /// <returns>Enumeration of counters as <see cref="TimeSpan"/> instances.</returns>
    public IEnumerable<KeyValuePair<string, TimeSpan>> EnumerateElapsed()
    {
        foreach (string k in this.Keys)
        {
            if (this.TryGetElapsed(k, out TimeSpan ts))
            {
                yield return new KeyValuePair<string, TimeSpan>(k, ts);
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<string, long>> GetEnumerator()
    {
        return this.counters.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
