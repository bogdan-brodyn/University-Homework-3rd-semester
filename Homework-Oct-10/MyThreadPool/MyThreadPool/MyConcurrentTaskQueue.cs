// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyThreadPool;

/// <summary>
/// Implements the concurrent task queue containing instances implementing <see cref="IMyTask"/>.
/// </summary>
internal class MyConcurrentTaskQueue(int n)
{
    private const int InitialStatus = 0;
    private const int ShutdownStatus = 1;
    private static volatile int status = InitialStatus;

    // Synchronization
    private readonly Semaphore elementsCountSemaphore = new (initialCount: 0, maximumCount: int.MaxValue);
    private readonly ManualResetEvent manualResetEvent = new (false);
    private readonly int maximumBlockedThreadsCount = n;
    private volatile int blockedThreadsCount = 0;
    private volatile int elementsCount = 0;

    // Task queue content
    private QueueElement? head = null;
    private QueueElement? tail = null;

    /// <summary>
    /// Enqueues the task if possible, otherwise throws an exception.
    /// </summary>
    /// <param name="task">The task to be enqueued.</param>
    public void EnqueueTask(IMyTask task)
    {
        var tmp = new QueueElement(task);

        lock (this)
        {
            // Occures if user continues a task when shutdown process is working.
            if (status == ShutdownStatus)
            {
                throw new InvalidOperationException("The task can't be continued after the thread pool shutdown.");
            }

            this.manualResetEvent.Reset();

            if (this.tail is null)
            {
                this.head = this.tail = tmp;
            }
            else
            {
                this.tail = this.tail.NextElement = tmp;
            }
        }

        Interlocked.Increment(ref this.elementsCount);
        this.elementsCountSemaphore.Release();
        this.manualResetEvent.Reset();
    }

    /// <summary>
    /// Equeues the task queue.
    /// </summary>
    /// <param name="taskQueue">The task queue.</param>
    public void EnqueueTaskQueue(MyConcurrentTaskQueue taskQueue)
    {
        if (taskQueue.tail is null)
        {
            return;
        }

        lock (this)
        {
            if (this.tail is null)
            {
                this.head = taskQueue.head;
            }
            else
            {
                this.tail.NextElement = taskQueue.head;
            }

            this.tail = taskQueue.tail;
        }

        Interlocked.Add(ref this.elementsCount, taskQueue.elementsCount);
        this.elementsCountSemaphore.Release(taskQueue.elementsCount);
    }

    /// <summary>
    /// Dequeues the task and completes it if possible, otherwise blocks the thread, until it is possible.
    /// </summary>
    public void DequeueAndCompleteTaskWithCurrentThread()
    {
        // Checks whether the queue is empty and all threads are sleeping.
        var actualBlockedThreadsCount = Interlocked.Increment(ref this.blockedThreadsCount);
        if (actualBlockedThreadsCount == this.maximumBlockedThreadsCount
            && this.elementsCount == 0)
        {
            this.manualResetEvent.Set();
        }

        // Booking a task.
        this.elementsCountSemaphore.WaitOne();
        Interlocked.Decrement(ref this.elementsCount);

        // Check was passed.
        Interlocked.Decrement(ref this.blockedThreadsCount);

        IMyTask tmp;

        lock (this)
        {
            tmp = this.head?.Task ?? throw new AggregateException("Am I fired????");
            this.head = this.head.NextElement;
            this.tail = this.head is not null ? this.tail : null;
        }

        tmp.Complete();
    }

    /// <summary>
    /// Waits for all tasks taken to be completed.
    /// </summary>
    public void Shutdown()
    {
        lock (this)
        {
            Interlocked.Exchange(ref status, ShutdownStatus);
        }

        this.manualResetEvent.WaitOne();
    }

    private class QueueElement(IMyTask task)
    {
        public IMyTask Task { get; init; } = task;

        public QueueElement? NextElement { get; set; } = null;
    }
}
