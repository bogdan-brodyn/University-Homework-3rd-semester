// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace LazyTests.TestCases;

public interface ITestCase<T>
{
    public T ExpectedGetResult { get; }

    public Func<T> Supplier { get; }
}
