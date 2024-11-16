// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Lazy;

/// <summary>
/// Implements parallel safe interface of lazy computation.
/// </summary>
/// <typeparam name="T">The computing value type.</typeparam>
public class LazyParallelSafe<T>(Func<T> supplier)
    : ILazy<T>
{
    private readonly Func<T> supplier = supplier;
    private readonly ManualResetEvent manualResetEvent = new (false);
    private int isFirstThread = 1;
    private T? computedValue = default;

    private Exception? exception = null;

    /// <inheritdoc/>
    public T Get()
    {
        var isFirstThread = Interlocked.Exchange(ref this.isFirstThread, 0);
        if (isFirstThread == 1)
        {
            try
            {
                this.computedValue = this.supplier.Invoke();
            }
            catch (Exception exception)
            {
                this.exception = exception;
                throw;
            }
            finally
            {
                this.manualResetEvent.Set();
            }
        }

        this.manualResetEvent.WaitOne();

        if (this.exception is not null)
        {
            throw this.exception;
        }

        return this.computedValue!;
    }
}
