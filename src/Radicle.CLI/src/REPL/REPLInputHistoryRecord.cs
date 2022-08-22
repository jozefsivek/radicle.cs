namespace Radicle.CLI.REPL;

using System;
using Radicle.Common.Check;

/// <summary>
/// REPL history record.
/// </summary>
public readonly struct REPLInputHistoryRecord : IEquatable<REPLInputHistoryRecord>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="REPLInputHistoryRecord"/> struct.
    /// </summary>
    /// <param name="value">Value of the record, can be empty.</param>
    /// <param name="isInitRecord">Flag determining
    ///     if the record is from the initial history init.</param>
    /// <param name="isModified">Flag determining if this record
    ///     was modified because of peek window value replacement.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public REPLInputHistoryRecord(
            string value,
            bool isInitRecord = false,
            bool isModified = false)
    {
        this.Value = Ensure.Param(value).Value;
        this.IsInitRecord = isInitRecord;
        this.IsModified = isModified;
    }

    /// <summary>
    /// Gets a value indicating whether
    /// the record is from the initial history init.
    /// </summary>
    public bool IsInitRecord { get; }

    /// <summary>
    /// Gets a value indicating whether this record
    /// was modified because of peek window value replacement.
    /// </summary>
    public bool IsModified { get; }

    /// <summary>
    /// Gets the string value of the record, can be empty.
    /// </summary>
    public string Value { get; } = string.Empty;

    /// <summary>
    /// Compare records.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns><see langword="true"/> if equal.</returns>
    public static bool operator ==(REPLInputHistoryRecord left, REPLInputHistoryRecord right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Compare records.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns><see langword="true"/> if NON equal.</returns>
    public static bool operator !=(REPLInputHistoryRecord left, REPLInputHistoryRecord right)
    {
        return !(left == right);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is REPLInputHistoryRecord rec && this.Equals(rec);
    }

    /// <inheritdoc/>
    public bool Equals(REPLInputHistoryRecord other)
    {
        return this.Value.Equals(other.Value, StringComparison.Ordinal)
                && this.IsInitRecord == other.IsInitRecord
                && this.IsModified == other.IsModified;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.Value, this.IsInitRecord, this.IsModified);
    }
}
