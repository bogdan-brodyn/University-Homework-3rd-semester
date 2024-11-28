// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace ConsoleAppBenchmark;

using BenchmarkDotNet.Attributes;
using MatrixMultiplicationMethods;

/// <summary>
/// Implements matrix multiplication method benchmarks.
/// </summary>
public class Benchmark
{
    private static readonly TestCaseCreationTool TestCaseCreationTool = new ();

    /// <summary>
    /// Represents test case sequence.
    /// </summary>
    /// <returns>Test cases arguments.</returns>
    public static IEnumerable<object[]> TestCaseArguments()
    {
        for (var i = 1; i <= 7; ++i)
        {
            yield return new object[]
            {
                new MatrixWraper(
                    MatrixIO.ReadMatrixFromFile(
                        TestCaseCreationTool.GetLeftMatrixPathByTestCaseId(i))),
                new MatrixWraper(
                    MatrixIO.ReadMatrixFromFile(
                        TestCaseCreationTool.GetRightMatrixPathByTestCaseId(i))),
            };
        }
    }

    /// <summary>
    /// Represents parallel matrix multiplication method benchmark.
    /// </summary>
    /// <param name="leftMatrix">Matrix to be multiplied wrapper.</param>
    /// <param name="rightMatrix">wrapper of matrix to be multiplied by.</param>
    [Benchmark]
    [ArgumentsSource(nameof(TestCaseArguments))]
    public void MultiplyParallel(MatrixWraper leftMatrix, MatrixWraper rightMatrix) =>
        MatrixMultiplicationMethods.MultiplyParallel(leftMatrix.Matrix, rightMatrix.Matrix);

    /// <summary>
    /// Represents sequentiall matrix multiplication method benchmark.
    /// </summary>
    /// <param name="leftMatrix">Matrix to be multiplied wrapper.</param>
    /// <param name="rightMatrix">wrapper of matrix to be multiplied by.</param>
    [Benchmark]
    [ArgumentsSource(nameof(TestCaseArguments))]
    public void MultiplySequentially(MatrixWraper leftMatrix, MatrixWraper rightMatrix) =>
        MatrixMultiplicationMethods.MultiplySequentially(leftMatrix.Matrix, rightMatrix.Matrix);

    /// <summary>
    /// The NuGet package used for benchmarking can only write test case input with ToString method.
    /// It's impossible to override it for int[,]. So we need the wrapper.
    /// </summary>
    /// <param name="matrix">The matrix to be wrapped.</param>
    public class MatrixWraper(int[,] matrix)
    {
        /// <summary>
        /// Gets wrapped matrix.
        /// </summary>
        public int[,] Matrix { get; } = matrix;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Matrix.GetLength(0)}x{this.Matrix.GetLength(1)}";
        }
    }
}
