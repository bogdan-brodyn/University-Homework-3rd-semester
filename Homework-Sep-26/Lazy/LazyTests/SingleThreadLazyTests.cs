// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace LazyTests;

using Lazy;
using TestCases;

public class SingleThreadLazyTests
{
    [TestCaseSource(nameof(SingleThreadTestCases))]
    public void TestLazyComputationMethod_SingleThread_WithTestCaseSupplier_ResultMustBeExpected<T>(
        (T expectedResult, Func<T> supplier) testCase)
    {
        const int getMethodInvocationCount = 10;

        // Arrange
        var actualResults = new T[getMethodInvocationCount];
        var lazyComputation = new LazyParallelUnsafe<T>(testCase.supplier);

        // Act
        for (var i = 0; i < getMethodInvocationCount; ++i)
        {
            actualResults[i] = lazyComputation.Get();
        }

        // Assert
        foreach (var actualResult in actualResults)
        {
            Assert.That(actualResult, Is.EqualTo(testCase.expectedResult));
        }
    }

    private static (T expectedResult, Func<T> supplier) ConvertToTuple<T>(ITestCase<T> testCase)
    {
        return (testCase.ExpectedGetResult, testCase.Supplier);
    }

    private static IEnumerable<TestCaseData> SingleThreadTestCases()
    {
        yield return new TestCaseData(ConvertToTuple(new TestCaseNoSupplierClosure()));
        yield return new TestCaseData(ConvertToTuple(new TestCaseWithIntegerVariableClosure()));
        yield return new TestCaseData(ConvertToTuple(new TestCaseWithStringVariableClosure()));
        yield return new TestCaseData(ConvertToTuple(new TestCaseWithLongVariableClosureAndThreadSleep()));
        yield return new TestCaseData(ConvertToTuple(new TestCaseWithIntegerArrayVariableClosure()));
        yield return new TestCaseData(ConvertToTuple(new TestCaseWithNullIntegerArrayVariableClosure()));
    }
}
