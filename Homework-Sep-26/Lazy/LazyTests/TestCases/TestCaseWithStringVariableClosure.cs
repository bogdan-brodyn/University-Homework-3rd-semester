// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace LazyTests.TestCases;

public class TestCaseWithStringVariableClosure : ITestCase<string>
{
    private string variableToBeCaptured = "sourceString";

    public string ExpectedGetResult => "sourceString";

    public Func<string> Supplier => () =>
    {
        var tmp = this.variableToBeCaptured;
        this.variableToBeCaptured = "AnotherString";
        return tmp;
    };
}
