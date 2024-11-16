// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Lazy;

/// <summary>
/// Represents interface of lazy computation.
/// </summary>
/// <typeparam name="T">The computing value type.</typeparam>
public interface ILazy<T>
{
    /// <summary>
    /// Returns the lazy computed value.
    /// </summary>
    /// <returns>The lazy computed value.</returns>
    public T Get();
}
