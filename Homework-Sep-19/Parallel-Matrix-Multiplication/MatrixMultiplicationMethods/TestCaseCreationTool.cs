// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MatrixMultiplicationMethods;

/// <summary>
/// Implements methods to generate matrix multiplication methods test cases.
/// </summary>
public class TestCaseCreationTool
{
    private readonly string testCasesFolderPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestCaseCreationTool"/> class.
    /// </summary>
    public TestCaseCreationTool()
    {
        var directory = new DirectoryInfo(Environment.CurrentDirectory);
        while (directory.Name != "Parallel-Matrix-Multiplication")
        {
            directory = directory.Parent ?? throw new Exception("Working directory must contain solution diretory.");
        }

        this.testCasesFolderPath = Path.Combine(directory.FullName, "TestCases");
    }

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
    /// Gets current test case folder path by its id.
    /// </summary>
    /// <param name="testCaseId">Currnet test case id.</param>
    /// <returns>Test case folder path.</returns>
    public string GetTestCasePathById(int testCaseId) =>
        Path.Combine(this.testCasesFolderPath, $"TestCase{testCaseId}");

    /// <summary>
    /// Gets current test case left matrix file path by its id.
    /// </summary>
    /// <param name="testCaseId">Currnet test case id.</param>
    /// <returns>Left matrix file path.</returns>
    public string GetLeftMatrixPathByTestCaseId(int testCaseId) =>
        Path.Combine(this.testCasesFolderPath, $"TestCase{testCaseId}", "leftMatrix.txt");

    /// <summary>
    /// Gets current test case right matrix file path by its id.
    /// </summary>
    /// <param name="testCaseId">Currnet test case id.</param>
    /// <returns>Right matrix file path.</returns>
    public string GetRightMatrixPathByTestCaseId(int testCaseId) =>
        Path.Combine(this.testCasesFolderPath, $"TestCase{testCaseId}", "rightMatrix.txt");

    /// <summary>
    /// Gets current test case multiplication result matrix file path by its id.
    /// </summary>
    /// <param name="testCaseId">Currnet test case id.</param>
    /// <returns>Multiplication result matrix file path.</returns>
    public string GetMultiplicationResultMatrixPathByTestCaseId(int testCaseId) =>
        Path.Combine(this.testCasesFolderPath, $"TestCase{testCaseId}", "multiplicationResultMatrix.txt");

    /// <summary>
    /// Creates test case with random valued matricies of size nxm and mxk multiplication test.
    /// </summary>
    /// <param name="n">First matrix of size nxm.</param>
    /// <param name="m">First matrix of size nxm. Second matrix of size mxk.</param>
    /// <param name="k">Second matrix of size mxk.</param>
    /// <param name="testCaseId">Test case id.</param>
    public void CreateRandomValuedMatrixMultiplicationTestCase(int n, int m, int k, int testCaseId)
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

        Directory.CreateDirectory(this.GetTestCasePathById(testCaseId));
        leftMatrix.WriteMatrixToFile(this.GetLeftMatrixPathByTestCaseId(testCaseId));
        rightMatrix.WriteMatrixToFile(this.GetRightMatrixPathByTestCaseId(testCaseId));
        multiplicationResultMatrix.WriteMatrixToFile(this.GetMultiplicationResultMatrixPathByTestCaseId(testCaseId));
    }
}
