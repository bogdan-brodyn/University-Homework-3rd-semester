// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace LazyTests;

using Lazy;

public class MultipleThreadLazyTests
{
    [TestCaseSource(typeof(TestCases.TestCases), nameof(TestCases.TestCases.GetTestCases))]
    public void TestLazyComputationMethod_SingleThread_WithTestCaseSupplier_ResultMustBeExpected<T>(
        (T expectedResult, Func<T> supplier) testCase)
    {
        const int threadCount = 10;

        // Arrange
        var threads = new Thread[threadCount];
        var actualResults = new T[threadCount];
        var lazyComputation = new LazyParallelSafe<T>(testCase.supplier);

        // Act
        for (var i = 0; i < threadCount; ++i)
        {
            var tmpI = i; // To avoid closure
            threads[tmpI] = new Thread(
                () => actualResults[tmpI] = lazyComputation.Get());
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        // Assert
        foreach (var actualResult in actualResults)
        {
            Assert.That(actualResult, Is.EqualTo(testCase.expectedResult));
        }
    }
}
