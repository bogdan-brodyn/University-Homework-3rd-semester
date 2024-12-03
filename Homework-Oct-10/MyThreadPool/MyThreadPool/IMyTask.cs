// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyThreadPool;

/// <summary>
/// Interface of task to be provided to <see cref="MyConcurrentTaskQueue"/> to manage the task represented.
/// </summary>
internal interface IMyTask
{
    /// <summary>
    /// Completes the task.
    /// Any task must be completed no more than once, otherwise the method throws an <see cref="InvalidOperationException"/>.
    /// </summary>
    public void Complete();
}
