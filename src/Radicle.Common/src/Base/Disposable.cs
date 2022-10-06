namespace Radicle.Common.Base;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Radicle.Common.Check;

/// <summary>
/// <see cref="Disposable"/> can be used to aggregate
/// several disposable objects into one as base class or collection.
/// </summary>
/// <remarks>
/// <para>
/// Disposing is NOT thread safe.
/// Read https://alistairevans.co.uk/2019/10/24/net-asynchronous-disposal-tips-for-implementing-iasyncdisposable-on-your-own-types/ ,
/// https://docs.microsoft.com/en-us/dotnet/api/system.object.finalize?view=net-5.0
/// and
/// https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/unmanaged .
/// </para>
/// <para>
/// If you need to react to disposing follow: https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose#implement-the-dispose-pattern-for-a-derived-class ,
/// define your own dispose flag to detect repeated calls
/// and call base <see cref="Dispose(bool)"/> after yours override.
/// </para>
/// </remarks>
public class Disposable : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Disposable"/> class.
    /// </summary>
    /// <param name="disposables">Collection of disposables.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public Disposable(
            params IDisposable[] disposables)
    {
        foreach (IDisposable d in Ensure.Param(disposables))
        {
            this.Add(d);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Disposable"/> class.
    /// </summary>
    /// <param name="disposables">Optional collection of disposables.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public Disposable(
            IEnumerable<IDisposable>? disposables = null)
    {
        foreach (IDisposable d in Ensure.Optional(disposables))
        {
            this.Add(d);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether
    /// this instance is disposed.
    /// </summary>
    internal bool Disposed { get; set; }

    /// <summary>
    /// Gets collection of stored disposables
    /// which will be disposed with this instance.
    /// </summary>
    /// <remarks>There is no concurrent set.</remarks>
    internal ConcurrentDictionary<IDisposable, bool> Disposables { get; } = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        // dispose unmanaged resources
        this.Dispose(true);

        // suppress finalization so that no double work is done.
        // note however here we avoid having any finalization code
        // as unmanaged resources should be handled with safe handles
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Actual dispose methods, as in
    /// https://docs.microsoft.com/en-us/visualstudio/code-quality/ca1063-implement-idisposable-correctly?view=vs-2019 .
    /// </summary>
    /// <param name="disposing">Flag determining if this method is
    ///     deterministically called, i.e. from Dispose. If called
    ///     from <see cref="Dispose()"/> (<see langword="true"/>)
    ///     or from finalizer (<see langword="false"/>).</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this.Disposed)
        {
            return;
        }

        // release managed resources only if disposing
        // e.g. called from Dispose()
        if (disposing)
        {
            foreach (IDisposable d in this.Disposables.Keys)
            {
                d.Dispose();
            }
        }

        /*
        no unmanaged resources to dispose
        https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
        */

        // set large fields to null to speed up GC
        this.Disposables.Clear();

        this.Disposed = true;
    }

    /// <summary>
    /// Add <paramref name="disposable"/> to disposables owned
    /// by this instance which will be disposed with this instance.
    /// </summary>
    /// <param name="disposable">Disposable object to add.</param>
    /// <exception cref="ArgumentNullException">Thrown if
    ///     required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this instance
    ///     is already disposed.</exception>
    protected void Add(IDisposable disposable)
    {
        this.EnsureNotDisposed();

        _ = this.Disposables.TryAdd(Ensure.Param(disposable).Value, true);
    }

    /// <summary>
    /// Remove contained <paramref name="disposable"/> and dispose it.
    /// </summary>
    /// <param name="disposable">Disposable object to remove
    ///     and dispose.</param>
    /// <exception cref="ArgumentNullException">Thrown if
    ///     the required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="disposable"/>
    ///     was not owned by this instance.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this instance
    ///     is already disposed.</exception>
    protected void RemoveAndDispose(IDisposable disposable)
    {
        Ensure.Param(disposable).Done();

        this.EnsureNotDisposed();

        if (this.Disposables.TryRemove(disposable, out _))
        {
            disposable.Dispose();
        }
        else
        {
            throw new ArgumentException(
                    "Disposable was not found",
                    nameof(disposable));
        }
    }

    /// <summary>
    /// Ensure this instance is not disposed.
    /// </summary>
    /// <param name="message">Custom optional message to use
    ///     in place of the default generic message.</param>
    /// <exception cref="ObjectDisposedException">Thrown
    ///     if this instance is disposed.</exception>
    protected void EnsureNotDisposed(string? message = null)
    {
        if (this.Disposed)
        {
            message ??= $"{this.GetType().Name} is already disposed";

            throw new ObjectDisposedException(null, message);
        }
    }
}
