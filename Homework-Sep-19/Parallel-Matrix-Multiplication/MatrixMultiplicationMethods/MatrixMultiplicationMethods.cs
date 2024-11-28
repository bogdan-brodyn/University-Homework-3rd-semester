// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MatrixMultiplicationMethods;

using System.Threading;

/// <summary>
/// Represents matrix multiplication methods.
/// </summary>
public static class MatrixMultiplicationMethods
{
    /// <summary>
    /// Represents sequentiall matrix multiplication method.
    /// </summary>
    /// <param name="leftMatrix">Matrix to be multiplied.</param>
    /// <param name="rightMatrix">Matrix to be multiplied by.</param>
    /// <returns>Multiplication result matrix.</returns>
    /// <exception cref="ArgumentException">Throws if left matrix column count not equal to right matrix row count.</exception>
    public static int[,] MultiplySequentially(int[,] leftMatrix, int[,] rightMatrix)
    {
        ThrowIfMultiplicationUndefined(leftMatrix, rightMatrix);

        var multiplicationResultMatrix = new int[leftMatrix.GetLength(0), rightMatrix.GetLength(1)];

        for (var row = 0; row < multiplicationResultMatrix.GetLength(0); ++row)
        {
            for (var column = 0; column < multiplicationResultMatrix.GetLength(1); ++column)
            {
                multiplicationResultMatrix[row, column] = Enumerable.Range(0, leftMatrix.GetLength(1))
                    .Sum(i => leftMatrix[row, i] * rightMatrix[i, column]);
            }
        }

        return multiplicationResultMatrix;
    }

    /// <summary>
    /// Represents parallel matrix multiplication method.
    /// </summary>
    /// <param name="leftMatrix">Matrix to be multiplied.</param>
    /// <param name="rightMatrix">Matrix to be multiplied by.</param>
    /// <returns>Multiplication result matrix.</returns>
    /// <exception cref="ArgumentException">Throws if left matrix column count not equal to right matrix row count.</exception>
    public static int[,] MultiplyParallel(int[,] leftMatrix, int[,] rightMatrix)
    {
        ThrowIfMultiplicationUndefined(leftMatrix, rightMatrix);

        var multiplicationResultMatrix = new int[leftMatrix.GetLength(0), rightMatrix.GetLength(1)];

        var threadsCount = Math.Min(Environment.ProcessorCount, multiplicationResultMatrix.GetLength(0));
        var threads = new Thread[threadsCount];
        for (var threadId = 0; threadId < threadsCount; ++threadId)
        {
            var tempThreadId = threadId; // against closures
            threads[threadId] = new Thread(() =>
            {
                for (var row = tempThreadId; row < multiplicationResultMatrix.GetLength(0); row += threadsCount)
                {
                    for (var column = 0; column < multiplicationResultMatrix.GetLength(1); ++column)
                    {
                        multiplicationResultMatrix[row, column] = Enumerable.Range(0, leftMatrix.GetLength(1))
                            .Sum(i => leftMatrix[row, i] * rightMatrix[i, column]);
                    }
                }
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return multiplicationResultMatrix;
    }

    private static void ThrowIfMultiplicationUndefined(int[,] leftMatrix, int[,] rightMatrix)
    {
        if (leftMatrix.GetLength(1) != rightMatrix.GetLength(0))
        {
            throw new ArgumentException(
                $"{nameof(leftMatrix)} column count must be equal to {nameof(rightMatrix)} row count");
        }
    }
}
