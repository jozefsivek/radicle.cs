namespace Radicle.Alike.Redis.Models;

using System;
using Radicle.Common.Check;

/// <summary>
/// Model representing cached in-memory value meta-data wrapping actual value.
/// </summary>
/// <typeparam name="TValue">Type of the stored value.</typeparam>
internal sealed class InMemoryValueWrapper<TValue>
    where TValue : notnull
{
    /// <summary>
    /// Expiration time span relative to <see cref="CreationTime"/>.
    /// </summary>
    private readonly TimeSpan? expiration;

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryValueWrapper{TValue}"/> class.
    /// </summary>
    /// <param name="value">Value to store.</param>
    /// <param name="expiration">Expiration of the value relative to creation time.
    ///     Zero or negative expiration will result in eviction.
    ///     Defaults to persistent value with no expiration.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public InMemoryValueWrapper(
            TValue value,
            TimeSpan? expiration = null)
    {
        this.Value = Ensure.Param(value).Value;
        this.CreationTime = DateTimeOffset.UtcNow;
        this.expiration = expiration;
    }

    /// <summary>
    /// Gets the value stored in this instance.
    /// </summary>
    public TValue Value { get; }

    /// <summary>
    /// Gets the creation time of this instance.
    /// </summary>
    public DateTimeOffset CreationTime { get; }

    /// <summary>
    /// Gets a value indicating whether this value has no expiration and is thus persistent.
    /// </summary>
    public bool IsPersistent => !this.expiration.HasValue;

    /// <summary>
    /// Gets the time this instance is alive from the time of creation.
    /// </summary>
    public TimeSpan Alive => DateTimeOffset.UtcNow - this.CreationTime;

    /// <summary>
    /// Gets the remaining time this value should be still alive.
    /// The value can be zeor or negative, signalizing immidiate eviction.
    /// If <see langword="null"/>, this value is persistent.
    /// </summary>
    public TimeSpan? TimeToLive => this.expiration.HasValue
            ? this.expiration - this.Alive
            : null;

    /// <summary>
    /// Gets a value indicating whether this value should be evicted.
    /// </summary>
    public bool Evict => !this.IsPersistent && this.TimeToLive <= TimeSpan.Zero;
}
