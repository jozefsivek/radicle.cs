namespace Radicle.Common;

using System;
using System.Text;
using Radicle.Common.Check;

/* TODO: use Generic math ( https://learn.microsoft.com/en-us/dotnet/standard/generics/math ) ones in .net 7/8 */

/// <summary>
/// Mutable implementation of <see cref="IProgress{T}"/>
/// which allows external inspection so it is not closed
/// and can be used by other code and not only original owner.
/// </summary>
/// <typeparam name="T">Type of the counter.</typeparam>
public sealed class TransparentProgress<T> : IProgress<T>, ITransparentProgress<T>
    where T : struct
{
    private readonly object concurrentLock = new();

    private readonly Func<object, object, object>? sum;

    private readonly Func<object, object, object>? diff;

    private readonly string stageNameValue = string.Empty;

    private string statusValue = string.Empty;

    private T countValue;

    private T? totalValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransparentProgress{T}"/> class.
    /// </summary>
    public TransparentProgress()
    {
        if (this.Count is int)
        {
            this.sum = static (object a, object b) => (int)a + (int)b;
            this.diff = static (object a, object b) => (int)a - (int)b;
        }
        else if (this.Count is long)
        {
            this.sum = static (object a, object b) => (long)a + (long)b;
            this.diff = static (object a, object b) => (long)a - (long)b;
        }
        else if (this.Count is double)
        {
            this.sum = static (object a, object b) => (double)a + (double)b;
            this.diff = static (object a, object b) => (double)a - (double)b;
        }
    }

    /// <inheritdoc/>
    public event EventHandler<ProgressEventArgs<T>>? ProgressChanged;

    /// <inheritdoc/>
    public DateTime StartDate { get; } = DateTime.UtcNow;

    /// <inheritdoc/>
    public ProgressReport<T> LastReport { get; private set; } = new ProgressReport<T>(default);

    /// <inheritdoc/>
    public T Count
    {
        get => this.countValue;
        set => this.SetCount(value);
    }

    /// <inheritdoc/>
    public T? Total
    {
        get => this.totalValue;
        set => this.SetTotal(value);
    }

    /// <inheritdoc/>
    public TransparentProgress<T>? PreviousStage { get; private init; }

    /// <inheritdoc/>
    public TransparentProgress<T>? CurrentChildStage { get; private set; }

    /// <inheritdoc/>
    public string StageName
    {
        get => this.stageNameValue;
        init => this.stageNameValue = Ensure.Param(value)
                .NoNewLines()
                .InRange(0, 64)
                .Value;
    }

    /// <inheritdoc/>
    public string Status
    {
        get => this.statusValue;
        set => this.statusValue = Ensure.Param(value)
                .NoNewLines()
                .InRange(0, 1024)
                .Value;
    }

    /// <summary>
    /// Gets connected parent stage if any.
    /// </summary>
    private TransparentProgress<T>? ParentStage { get; init; }

#pragma warning disable CA1000 // Do not declare static members on generic types
    /// <summary>
    /// Construct usable wrapper for given <paramref name="progress"/>
    /// or return instance if it is <see cref="TransparentProgress{T}"/>.
    /// </summary>
    /// <remarks>Note the state of the <paramref name="progress"/>
    /// may not be preserved, e.g. <see cref="Progress{T}"/>
    /// does not allow reading the crurrent count value,
    /// so wrap the <paramref name="progress"/> before reporting count.</remarks>
    /// <param name="progress">Progress instance to wrap.</param>
    /// <returns>Instance of <see cref="TransparentProgress{T}"/>
    ///     which can be used to controll wrapped <paramref name="progress"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if required
    ///     parameter is <see langword="null"/>.</exception>
    public static ITransparentProgress<T> From(
            IProgress<T> progress)
    {
        Ensure.Param(progress).Done();

        // do not wrap itself, it will be waste
        if (progress is ITransparentProgress<T> tp)
        {
            return tp;
        }

        TransparentProgress<T> result = new();

        result.ProgressChanged += (object _, ProgressEventArgs<T> e) => progress.Report(e.Count);

        return result;
    }

    /// <summary>
    /// Construct usable wrapper for given <paramref name="progress"/>
    /// or return instance if it is <see cref="TransparentProgress{T}"/>.
    /// </summary>
    /// <remarks>Note the state of the <paramref name="progress"/>
    /// may not be preserved, e.g. <see cref="Progress{T}"/>
    /// does not allow reading the crurrent count value,
    /// so wrap the <paramref name="progress"/> before reporting count.</remarks>
    /// <param name="progress">Progress instance to wrap.</param>
    /// <returns>Instance of <see cref="TransparentProgress{T}"/>
    ///     which can be used to controll wrapped <paramref name="progress"/>
    ///     or <see langword="null"/> if <paramref name="progress"/> was.</returns>
    public static ITransparentProgress<T>? FromOrDefault(
            IProgress<T>? progress)
    {
        if (progress is null)
        {
            return null;
        }

        return From(progress);
    }
#pragma warning restore CA1000 // Do not declare static members on generic types

    /// <inheritdoc/>
    public void Report(T value)
    {
        this.SetCount(value);
    }

    /// <inheritdoc/>
    public T IncrementCount(T increment)
    {
        T reportCount;
        T? reportTotal;
        DateTime reportDate;

        if (this.sum is not null)
        {
            lock (this.concurrentLock)
            {
                this.countValue = (T)this.sum(this.countValue, increment);
                this.LastReport = this.countValue;
                reportCount = this.countValue;
                reportTotal = this.totalValue;
                reportDate = this.LastReport.Date;
            }

            _ = this.ParentStage?.IncrementCount(increment);

            this.ProgressChanged?.Invoke(
                    this,
                    new ProgressEventArgs<T>(reportDate, reportCount, reportTotal));

            return reportCount;
        }

        throw new NotSupportedException(
                $"Type {typeof(T)} is not supported for increments, choose int. long or double.");
    }

    /// <inheritdoc/>
    public T IncrementTotal(T increment)
    {
        T reportTotal;

        if (this.sum is not null)
        {
            lock (this.concurrentLock)
            {
                if (this.totalValue.HasValue)
                {
                    this.totalValue = (T)this.sum(this.totalValue.Value, increment);
                }
                else
                {
                    this.totalValue = increment;
                }

                reportTotal = this.totalValue.Value;
            }

            _ = this.ParentStage?.IncrementTotal(increment);

            return reportTotal;
        }

        throw new NotSupportedException(
                $"Type {typeof(T)} is not supported for increments, choose int. long or double.");
    }

    /// <inheritdoc/>
    public void SetCount(T count)
    {
        T prev;
        T reportCount = count;
        T? reportTotal;
        DateTime reportDate;

        lock (this.concurrentLock)
        {
            /* Even if count does not change the date will be updated,
             * so we set values anyway */

            prev = this.countValue;
            this.countValue = count;
            this.LastReport = this.countValue;
            reportTotal = this.Total;
            reportDate = this.LastReport.Date;
        }

        this.ProgressChanged?.Invoke(
                this,
                new ProgressEventArgs<T>(reportDate, reportCount, reportTotal));

        if (this.ParentStage is not null)
        {
            if (this.diff is not null)
            {
                _ = this.ParentStage.IncrementCount((T)this.diff(count, prev));
            }
            else
            {
                throw new NotSupportedException(
                        $"Type {typeof(T)} is not supported for count set in child stage, "
                        + "choose int. long or double.");
            }
        }
    }

    /// <inheritdoc/>
    public void SetTotal(T? total)
    {
        T? prev;
        T reportCount;
        T? reportTotal;
        DateTime reportDate;

        lock (this.concurrentLock)
        {
            // TODO: return early if total did not changed
            prev = this.totalValue;
            reportCount = this.LastReport.Count;
            reportDate = this.LastReport.Date;
            reportTotal = total;
            this.totalValue = total;
        }

        this.ProgressChanged?.Invoke(
                this,
                new ProgressEventArgs<T>(reportDate, reportCount, reportTotal));

        if (this.ParentStage is not null)
        {
            if (this.diff is not null)
            {
                if (prev.HasValue && total.HasValue)
                {
                    T amount = (T)this.diff(total, prev);
                    _ = this.ParentStage.IncrementTotal(amount);
                }
                else if (!prev.HasValue && total.HasValue)
                {
                    _ = this.ParentStage.IncrementTotal(total.Value);
                }
                else if (prev.HasValue && !total.HasValue)
                {
                    T amount = (T)this.diff(default(T), prev.Value);
                    _ = this.ParentStage.IncrementTotal(amount);
                }
            }
            else
            {
                throw new NotSupportedException(
                        $"Type {typeof(T)} is not supported for count set in child stage, "
                        + "choose int. long or double.");
            }
        }
    }

    /// <inheritdoc/>
    public IProgressContributor<T> GetContributor()
    {
        return this;
    }

    /// <inheritdoc/>
    public TransparentProgress<T> CreateChildStage(string name)
    {
        lock (this.concurrentLock)
        {
            this.CurrentChildStage = new()
            {
                StageName = name,
                PreviousStage = this.CurrentChildStage,
                ParentStage = this,
            };

            return this.CurrentChildStage;
        }
    }

    /// <inheritdoc/>
    public string GetDescription()
    {
        StringBuilder result = new();
        bool first = true;
        TransparentProgress<T> last = this;
        TransparentProgress<T>? current = this;

        while (current is not null)
        {
            if (!string.IsNullOrWhiteSpace(current.StageName))
            {
                if (!first)
                {
                    result = result.Append(" > ");
                }

                first = false;
                result = result.Append(current.StageName);
            }

            current = current.CurrentChildStage;
            last = current ?? last;
        }

        if (!string.IsNullOrWhiteSpace(last.Status))
        {
            if (!first)
            {
                result = result.Append(": ");
            }

            result = result.Append(last.Status);
        }

        return result.ToString();
    }
}
