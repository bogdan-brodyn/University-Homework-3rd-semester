// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MatrixMultiplicationMethods;

/// <summary>
/// Implements methods to generate matrix multiplication methods test cases.
/// </summary>
public static class TestCaseCreationTool
{
    /// <summary>
    /// Represents test cases folder path. Folder contains all the tests.
    /// </summary>
    public const string TestCasesFolderPath = @$"../../../../TestCases";

    /// <summary>
    /// Gets current test case folder path by its id.
    /// </summary>
    /// <param name="testCaseId">Currnet test case id.</param>
    /// <returns>Test case folder path.</returns>
    public static string GetTestCasePathById(int testCaseId) =>
        Path.Combine(TestCasesFolderPath, $"TestCase{testCaseId}");

    /// <summary>
    /// Gets current test case left matrix file path by its id.
    /// </summary>
    /// <param name="testCaseId">Currnet test case id.</param>
    /// <returns>Left matrix file path.</returns>
    public static string GetLeftMatrixPathByTestCaseId(int testCaseId) =>
        Path.Combine(TestCasesFolderPath, $"TestCase{testCaseId}", "leftMatrix.txt");

    /// <summary>
    /// Gets current test case right matrix file path by its id.
    /// </summary>
    /// <param name="testCaseId">Currnet test case id.</param>
    /// <returns>Right matrix file path.</returns>
    public static string GetRightMatrixPathByTestCaseId(int testCaseId) =>
        Path.Combine(TestCasesFolderPath, $"TestCase{testCaseId}", "rightMatrix.txt");

    /// <summary>
    /// Gets current test case multiplication result matrix file path by its id.
    /// </summary>
    /// <param name="testCaseId">Currnet test case id.</param>
    /// <returns>Multiplication result matrix file path.</returns>
    public static string GetMultiplicationResultMatrixPathByTestCaseId(int testCaseId) =>
        Path.Combine(TestCasesFolderPath, $"TestCase{testCaseId}", "multiplicationResultMatrix.txt");

    /// <summary>
    /// Creates matrix of size rowCountxcolumnCount with random values from (-100, 100).
    /// </summary>
    /// <param name="rowCount">Result matrix row count.</param>
    /// <param name="columnCount">Result matrix column count.</param>
    /// <returns>Result matrix.</returns>
    public static int[,] CreateRandomValuedMatrix(int rowCount, int columnCount)
    {
        var resultMatrix = new int[rowCount, columnCount];

        var random = new Random();
        for (int row = 0; row < rowCount; ++row)
        {
            for (int column = 0; column < columnCount; ++column)
            {
                resultMatrix[row, column] = random.Next(-99, 100);
            }
        }

        return resultMatrix;
    }

    /// <summary>
    /// Creates test case with random valued matricies of size nxm and mxk multiplication test.
    /// </summary>
    /// <param name="n">First matrix of size nxm.</param>
    /// <param name="m">First matrix of size nxm. Second matrix of size mxk.</param>
    /// <param name="k">Second matrix of size mxk.</param>
    /// <param name="testCaseId">Test case id.</param>
    public static void CreateRandomValuedMatrixMultiplicationTestCase(int n, int m, int k, int testCaseId)
    {
        var leftMatrix = CreateRandomValuedMatrix(n, m);
        var rightMatrix = CreateRandomValuedMatrix(m, k);
        var multiplicationResultMatrix = new int[n, k];
        for (int row = 0; row < n; ++row)
        {
            for (int column = 0; column < k; ++column)
            {
                for (int i = 0; i < m; ++i)
                {
                    multiplicationResultMatrix[row, column] += leftMatrix[row, i] * rightMatrix[i, column];
                }
            }
        }

        Directory.CreateDirectory(GetTestCasePathById(testCaseId));
        leftMatrix.WriteMatrixToFile(GetLeftMatrixPathByTestCaseId(testCaseId));
        rightMatrix.WriteMatrixToFile(GetRightMatrixPathByTestCaseId(testCaseId));
        multiplicationResultMatrix.WriteMatrixToFile(GetMultiplicationResultMatrixPathByTestCaseId(testCaseId));
    }
}
