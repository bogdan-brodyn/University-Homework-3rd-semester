// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MatrixMultiplicationMethodsTests;

using MatrixMultiplicationMethods;

public class MatrixExtraMethodsTests
{
    [TestCase(1, 1)]
    [TestCase(10, 10)]
    [TestCase(100, 200)]
    public void TestWriteReadMethods_OnCreatedMatrixInput_MatrixMustBeSame(int n, int m)
    {
        // Arrange
        var matrix = TestCaseCreationTool.CreateRandomValuedMatrix(n, m);

        // Act
        const string tempFilePath = "Temp.txt";
        matrix.WriteMatrixToFile(tempFilePath);
        int[,] resultMatrix = MatrixIO.ReadMatrixFromFile(tempFilePath);
        File.Delete(tempFilePath);

        // Assert
        CollectionAssert.AreEqual(expected: matrix, actual: resultMatrix);
    }
}
