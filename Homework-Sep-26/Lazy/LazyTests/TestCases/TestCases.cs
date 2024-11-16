// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace LazyTests.TestCases;

public static class TestCases
{
    public static IEnumerable<object> GetTestCases()
    {
        yield return ConvertToTuple(new TestCaseNoSupplierClosure());
        yield return ConvertToTuple(new TestCaseWithIntegerVariableClosure());
        yield return ConvertToTuple(new TestCaseWithStringVariableClosure());
        yield return ConvertToTuple(new TestCaseWithLongVariableClosureAndThreadSleep());
        yield return ConvertToTuple(new TestCaseWithIntegerArrayVariableClosure());
        yield return ConvertToTuple(new TestCaseWithNullIntegerArrayVariableClosure());
    }

    private static (T expectedResult, Func<T> supplier) ConvertToTuple<T>(ITestCase<T> testCase)
    {
        return (testCase.ExpectedGetResult, testCase.Supplier);
    }
}
