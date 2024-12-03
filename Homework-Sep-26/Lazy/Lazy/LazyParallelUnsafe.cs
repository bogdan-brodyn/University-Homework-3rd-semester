// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Lazy;

/// <summary>
/// Implements parallel unsafe interface of lazy computation.
/// </summary>
/// <typeparam name="T">The computing value type.</typeparam>
public class LazyParallelUnsafe<T>(Func<T> supplier)
    : ILazy<T>
{
    private readonly Func<T> supplier = supplier;
    private bool isValueComputed = false;
    private T? computedValue = default;

    /// <inheritdoc/>
    public T Get()
    {
        if (this.isValueComputed is false)
        {
            this.computedValue = this.supplier.Invoke();
            this.isValueComputed = true;
        }

        return this.computedValue!;
    }
}
