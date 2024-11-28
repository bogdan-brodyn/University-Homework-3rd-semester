// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MatrixMultiplicationMethodsTests;

using MatrixMultiplicationMethods;

public class MatrixMultiplicationMethodsTests
{
    private static readonly TestCaseCreationTool TestCaseCreationTool = new ();

    private static readonly Func<int[,], int[,], int[,]>[] Evaluation =
    [
        MatrixMultiplicationMethods.MultiplySequentially,
        MatrixMultiplicationMethods.MultiplyParallel,
    ];

    [Test]
    public void TestMultiplicationMethods_WithWrittenTestCases_ResultMustBeExpected(
        [ValueSource(nameof(Evaluation))] Func<int[,], int[,], int[,]> multiplicationFunction,
        [Range(1, 7)] int testCaseId)
    {
        // Arrange
        int[,] leftMatrix = MatrixIO.ReadMatrixFromFile(
            TestCaseCreationTool.GetLeftMatrixPathByTestCaseId(testCaseId));
        int[,] rightMatrix = MatrixIO.ReadMatrixFromFile(
            TestCaseCreationTool.GetRightMatrixPathByTestCaseId(testCaseId));
        int[,] multiplicationResultMatrix = MatrixIO.ReadMatrixFromFile(
            TestCaseCreationTool.GetMultiplicationResultMatrixPathByTestCaseId(testCaseId));

        // Act
        var actualResult = multiplicationFunction(leftMatrix, rightMatrix);

        // Assert
        CollectionAssert.AreEqual(expected: multiplicationResultMatrix, actual: actualResult);
    }
}
