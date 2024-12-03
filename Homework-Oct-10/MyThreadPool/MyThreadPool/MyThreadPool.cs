// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyThreadPool;

/// <summary>
/// Implements my version of thread pool.
/// </summary>
public class MyThreadPool
{
    private readonly Thread[] threads;
    private readonly MyConcurrentTaskQueue taskQueue;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class.
    /// </summary>
    /// <param name="n">The number of threads to be run with the pool.</param>
    public MyThreadPool(int n)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(n, "The number of threads to be run with the pool must be positive.");

        this.taskQueue = new (n);
        this.threads = new Thread[n];
        for (int i = 0; i < n; ++i)
        {
            this.threads[i] = new Thread(() =>
                {
                    while (true)
                    {
                        this.taskQueue.DequeueAndCompleteTaskWithCurrentThread();
                    }
                });
            this.threads[i].IsBackground = true;
            this.threads[i].Start();
        }
    }

    /// <summary>
    /// Submits a task to the thread pool if the last is working, otherwise throws exception.
    /// </summary>
    /// <typeparam name="TResult">Task completion result type.</typeparam>
    /// <param name="taskDescribingFunc">The task describing function.</param>
    /// <returns>The function invocation result.</returns>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> taskDescribingFunc)
    {
        var task = new MyTask<TResult>(taskDescribingFunc, this.taskQueue);
        this.taskQueue.EnqueueTask(task);
        return task;
    }

    /// <summary>
    /// Shutdowns thread pool.
    /// Returns control only after shutdown.
    /// Throws an exception for all threads trying to add a new task.
    /// </summary>
    public void Shutdown() => this.taskQueue.Shutdown();
}
