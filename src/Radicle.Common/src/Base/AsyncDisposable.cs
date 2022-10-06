namespace Radicle.Common.Base;

using System;
using System.Threading.Tasks;

/// <summary>
/// <see cref="AsyncDisposable"/> can be used to aggregate
/// several (async)disposable objects into one as a base class
/// or collection.
/// </summary>
/// <remarks>
/// <para>
/// Disposing is NOT thread safe.
/// Read https://alistairevans.co.uk/2019/10/24/net-asynchronous-disposal-tips-for-implementing-iasyncdisposable-on-your-own-types/ ,
/// https://docs.microsoft.com/en-us/dotnet/api/system.object.finalize?view=net-5.0 ,
/// https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#disposeasync-and-disposeasynccore
/// and
/// https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/unmanaged .
/// Important, do not stack async usings:
/// https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#unacceptable-pattern .
/// </para>
/// <para>
/// If you need to react to disposing follow: https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose#implement-the-dispose-pattern-for-a-derived-class ,
/// define your own dispose flag to detect repeated calls
/// and call base <see cref="DisposeAsyncCore"/> after yours override.
/// </para>
/// </remarks>
public class AsyncDisposable : Disposable, IAsyncDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncDisposable"/> class.
    /// </summary>
    /// <param name="disposables">Collection of
    ///     (async)disposables.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public AsyncDisposable(params IDisposable[] disposables)
        : base(disposables)
    {
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        // Perform async cleanup.
        await this.DisposeAsyncCore().ConfigureAwait(false);

        // Dispose of unmanaged resources.
        this.Dispose(false);

        // Suppress finalization.
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Actual dispose methods, as in
    /// https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync .
    /// </summary>
    /// <returns>Awaitable <see cref="ValueTask"/>.</returns>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (this.Disposed)
        {
            return;
        }

        // release managed resources
        foreach (IDisposable d in this.Disposables.Keys)
        {
            if (d is IAsyncDisposable a)
            {
                await a.DisposeAsync().ConfigureAwait(false);
            }
            else
            {
                d.Dispose();
            }
        }

        /* unmanaged resources see call to Dispose(false) */

        this.Disposed = true;
    }
}
