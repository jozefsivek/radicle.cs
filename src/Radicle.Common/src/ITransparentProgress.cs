namespace Radicle.Common;

using System;

/// <summary>
/// Interface of thread save <see cref="TransparentProgress{T}"/>.
/// </summary>
/// <typeparam name="T">Type of the counter.</typeparam>
public interface ITransparentProgress<T> : IProgressContributor<T>, IProgress<T>
    where T : struct
{
    /// <summary>
    /// Raised for each reported progress value,
    /// and or change <see cref="Total"/> value.
    /// </summary>
    event EventHandler<ProgressEventArgs<T>>? ProgressChanged;

    /// <summary>
    /// Gets UTC based start date of this progress
    /// this is by default set to creation time of this object.
    /// </summary>
    DateTime StartDate { get; }

    /// <summary>
    /// Gets last report of this progress
    /// this is by default set to <see cref="StartDate"/>
    /// and initial value of <see cref="Count"/>.
    /// </summary>
    ProgressReport<T> LastReport { get; }

    /// <summary>
    /// Gets current count.
    /// See <see cref="SetCount(T)"/>.
    /// </summary>
    T Count { get; }

    /// <summary>
    /// Gets total count if available.
    /// See <see cref="SetTotal(T?)"/>.
    /// </summary>
    T? Total { get; }

    /// <summary>
    /// Gets previous stage to this one if any.
    /// This value is set when parent creates consequtive child stage.
    /// </summary>
    TransparentProgress<T>? PreviousStage { get; }

    /// <summary>
    /// Gets current child stage is any.
    /// See <see cref="CreateChildStage(string)"/>.
    /// </summary>
    TransparentProgress<T>? CurrentChildStage { get; }

    /// <summary>
    /// Gets small descriptive human readable
    /// one-line stage name of no more than 64 characters.
    /// Can be empty.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if set value contains new lines.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if set value is longer that 64 characters.</exception>
    string StageName { get; }

    /// <summary>
    /// Gets or sets small descriptive human readable
    /// one-line status text of no more than 1024 characters.
    /// Can be empty.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if set value contains new lines.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if set value is longer that 1024 characters.</exception>
    string Status { get; set; }

    /// <summary>
    /// Set total value or clear one if <see langword="null"/>.
    /// See also <see cref="IProgressContributor{T}.IncrementTotal(T)"/>.
    /// </summary>
    /// <param name="total">New total value
    ///     overriding any previous one.</param>
    void SetTotal(T? total);

    /// <summary>
    /// Set total count, overriding any previous
    /// value, this is proxy to <see cref="IProgress{T}.Report(T)"/>. See also
    /// <see cref="IProgressContributor{T}.IncrementCount(T)"/>.
    /// </summary>
    /// <param name="count">New count value
    ///     overriding any previous value.</param>
    public void SetCount(T count);

    /// <summary>
    /// Gets object which can be used to contribute to this progress
    /// from multiple threads.
    /// </summary>
    /// <returns>Instance of <see cref="IProgressContributor{T}"/>.</returns>
    IProgressContributor<T> GetContributor();

    /// <summary>
    /// Create new child stage replacing any
    /// other existing child stage.
    /// </summary>
    /// <param name="name">Name of the new child stage no more than 64 characters.</param>
    /// <returns>New child instance of <see cref="TransparentProgress{T}"/>
    ///     which will reflect its progress into this instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if set value contains new lines.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if set value is longer that 64 characters.</exception>
    TransparentProgress<T> CreateChildStage(string name);

    /// <summary>
    /// Construct one line description from <see cref="StageName"/>,
    /// <see cref="CurrentChildStage"/> and <see cref="Status"/>.
    /// </summary>
    /// <returns>Human readable description.</returns>
    string GetDescription();
}
