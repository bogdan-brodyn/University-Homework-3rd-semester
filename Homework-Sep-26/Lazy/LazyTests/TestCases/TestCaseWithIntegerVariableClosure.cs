// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace LazyTests.TestCases;

public class TestCaseWithIntegerVariableClosure : ITestCase<int>
{
    private int variableToBeCaptured = 0;

    public int ExpectedGetResult => 0;

    public Func<int> Supplier => () => this.variableToBeCaptured++;
}
