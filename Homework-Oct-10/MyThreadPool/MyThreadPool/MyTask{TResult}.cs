// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyThreadPool;

/// <summary>
/// Implements task to be provided to <see cref="MyConcurrentTaskQueue"/> and user by <see cref="MyThreadPool"/>.
/// </summary>
/// <typeparam name="TResult">Task completion result type.</typeparam>
internal class MyTask<TResult>(Func<TResult> taskDescribingFunc, MyConcurrentTaskQueue generalTaskQueue)
    : IMyTask<TResult>, IMyTask
{
    private const int InitialStatus = 0;
    private const int IsCompletingStatus = 1;
    private const int IsCompletedStatus = 2;

    // Task description
    private readonly Func<TResult> taskDescribingFunc = taskDescribingFunc;
    private readonly MyConcurrentTaskQueue generalTaskQueue = generalTaskQueue;
    private readonly MyConcurrentTaskQueue continuationTaskQueue = new (0);

    // Synchronization: blocks threads waiting for the task result
    private readonly ManualResetEvent manualResetEvent = new (false);

    // Task result
    private volatile int status = InitialStatus;
    private AggregateException? aggregateException = null;
    private TResult? result = default;

    /// <inheritdoc/>
    public bool IsCompleted { get => this.status == IsCompletedStatus; }

    /// <inheritdoc/>
    public TResult? Result
    {
        get
        {
            this.manualResetEvent.WaitOne();
            if (this.aggregateException is not null)
            {
                throw this.aggregateException;
            }

            return this.result;
        }
    }

    /// <inheritdoc/>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> continuationTaskDescribingFunc)
    {
        var continuationTask = new MyTask<TNewResult>(
            () => continuationTaskDescribingFunc(this.Result),
            this.generalTaskQueue);

        // This lock prevents race related to the continuation task queue.
        lock (this)
        {
            if (this.status == IsCompletedStatus)
            {
                this.generalTaskQueue.EnqueueTask(continuationTask);
            }
            else
            {
                this.continuationTaskQueue.EnqueueTask(continuationTask);
            }
        }

        return continuationTask;
    }

    /// <inheritdoc/>
    public void Complete()
    {
        // This code will be executed no more than once for any task.
        // There can only be threads from the thread pool here.
        var actualStatus = Interlocked.CompareExchange(ref this.status, IsCompletingStatus, InitialStatus);

        // Doesn't happen if the program is correct
        if (actualStatus != InitialStatus)
        {
            throw new InvalidOperationException("Any task must be completed no more than once.");
        }

        try
        {
            this.result = this.taskDescribingFunc();
        }
        catch (Exception exception)
        {
            this.aggregateException = new AggregateException(exception);
        }

        this.status = IsCompletedStatus;
        this.manualResetEvent.Set();

        // This lock prevents race related to the continuation task queue.
        lock (this)
        {
            this.generalTaskQueue.EnqueueTaskQueue(this.continuationTaskQueue);
        }
    }
}
