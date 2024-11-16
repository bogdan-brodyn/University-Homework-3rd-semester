// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace LazyTests.TestCases;

#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
public class TestCaseWithNullIntegerArrayVariableClosure : ITestCase<int[]?>
{
    private int[]? variableToBeCaptured = null;

    public int[]? ExpectedGetResult => null;

    public Func<int[]?> Supplier => () =>
    {
        var tmp = this.variableToBeCaptured;
        this.variableToBeCaptured = new int[] { 1, 2 };
        return tmp;
    };
}
#pragma warning restore SA1011 // Closing square brackets should be spaced correctly
