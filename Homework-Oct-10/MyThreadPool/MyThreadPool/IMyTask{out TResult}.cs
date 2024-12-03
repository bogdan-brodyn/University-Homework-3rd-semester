// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyThreadPool;

/// <summary>
/// Interface of task to be provided to user to interact with <see cref="MyThreadPool"/>.
/// </summary>
/// <typeparam name="TResult">Task completion result type.</typeparam>
public interface IMyTask<out TResult>
{
    /// <summary>
    /// Gets a value indicating whether the task is completed.
    /// </summary>
    public bool IsCompleted { get; }

    /// <summary>
    /// Gets the task completion result if the task is completed successfully, otherwise it throws an <see cref="AggregateException"/>.
    /// </summary>
    public TResult? Result { get; }

    /// <summary>
    /// Applies the function received to the current task completion result and returns a new <see cref="IMyTask{TNewResult}"/> provided to <see cref="MyThreadPool"/>.
    /// </summary>
    /// <typeparam name="TNewResult">The completion result type of the task to be returned.</typeparam>
    /// <param name="continuationTaskDescribingFunc">The function to be applied.</param>
    /// <returns>The continuation task.</returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> continuationTaskDescribingFunc);
}
