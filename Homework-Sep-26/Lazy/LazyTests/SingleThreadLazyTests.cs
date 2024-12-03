// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace LazyTests;

using Lazy;

public class SingleThreadLazyTests
{
    private const int UseLazyParallelUnsafe = 0;
    private const int UseLazyParallelSafe = 1;

    [TestCaseSource(nameof(SingleThreadTestCases))]
    public void TestLazyComputationMethod_SingleThread_WithTestCaseSupplier_ResultMustBeExpected<T>(
        (T expectedResult, Func<T> supplier) testCase,
        int iLazyImplimentationIdentifier)
    {
        const int getMethodInvocationCount = 10;

        // Arrange
        var actualResults = new T[getMethodInvocationCount];
        ILazy<T> lazyComputation = iLazyImplimentationIdentifier switch
        {
            UseLazyParallelUnsafe => new LazyParallelUnsafe<T>(testCase.supplier),
            UseLazyParallelSafe => new LazyParallelSafe<T>(testCase.supplier),
            _ => throw new Exception(),
        };

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

    private static IEnumerable<TestCaseData> SingleThreadTestCases()
    {
        foreach (var testCase in TestCases.TestCases.GetTestCases())
        {
            yield return new TestCaseData(testCase, UseLazyParallelUnsafe);
        }

        foreach (var testCase in TestCases.TestCases.GetTestCases())
        {
            yield return new TestCaseData(testCase, UseLazyParallelSafe);
        }
    }
}
