// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace LazyTests.TestCases;

public class TestCaseWithIntegerArrayVariableClosure : ITestCase<int[]>
{
    private readonly int[] expectedGetResult;

    private int[] variableToBeCaptured;

    public TestCaseWithIntegerArrayVariableClosure()
    {
        this.expectedGetResult = new int[] { 1, 2 };
        this.variableToBeCaptured = this.expectedGetResult;
    }

    public int[] ExpectedGetResult => this.expectedGetResult;

    public Func<int[]> Supplier => () =>
    {
        var tmp = this.variableToBeCaptured;
        this.variableToBeCaptured = new int[] { 1, 2 };
        return tmp;
    };
}
