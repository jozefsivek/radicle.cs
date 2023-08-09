namespace Radicle.Common;

using System;

/// <summary>
/// Source of time periods.
/// </summary>
public interface IPeriodSource
{
    /// <summary>
    /// Gets current value of strictly positive period.
    /// Call <see cref="MoveNext"/> before retrieving
    /// this value first time.
    /// </summary>
    TimeSpan Current { get; }

    /// <summary>
    /// Advance counter and return <see cref="Current"/>.
    /// </summary>
    /// <returns>Value of <see cref="Current"/> after advancing the poisition.</returns>
    TimeSpan MoveNext();

    /// <summary>
    /// Reset state of this source
    /// as if newly created.
    /// </summary>
    void Reset();
}
