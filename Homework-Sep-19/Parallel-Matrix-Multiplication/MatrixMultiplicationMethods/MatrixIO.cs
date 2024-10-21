// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MatrixMultiplicationMethods;

using System.Text;

/// <summary>
/// Matrix input/output methods.
/// </summary>
public static class MatrixIO
{
    /// <summary>
    /// Writes matrix to file.
    /// </summary>
    /// <param name="matrix">Writing source.</param>
    /// <param name="filePath">Writing destination file path.</param>
    /// <param name="fileMode">Specifies how the operating system should open a file. As default it is FileMode.CreateNew.</param>
    public static void WriteMatrixToFile(this int[,] matrix, string filePath, FileMode fileMode = FileMode.CreateNew)
    {
        using var streamWriter = new StreamWriter(new FileStream(filePath, fileMode));
        matrix.WriteMatrixToStream(streamWriter);
    }

    /// <summary>
    /// Writes matrix to stream.
    /// </summary>
    /// <param name="matrix">Writing source.</param>
    /// <param name="streamWriter">Writing destination.</param>
    public static void WriteMatrixToStream(this int[,] matrix, StreamWriter streamWriter)
    {
        streamWriter.WriteLine($"{matrix.GetLength(0)} {matrix.GetLength(1)}");
        var lineBufer = new StringBuilder();
        for (var line = 0; line < matrix.GetLength(0); ++line)
        {
            lineBufer.Clear();
            for (var column = 0; column < matrix.GetLength(1); ++column)
            {
                lineBufer.Append($"{matrix[line, column],10}");
            }

            streamWriter.WriteLine(lineBufer);
        }
    }

    /// /// <summary>
    /// Reads matrix written in WriteMatrix method notation.
    /// </summary>
    /// <param name="filePath">Source file path.</param>
    /// <param name="fileMode">Specifies how the operating system should open a file. As default it is FileMode.Open.</param>
    /// <returns>Matrix readed.</returns>
    /// <exception cref="ArgumentException">Notation not followed.</exception>
    public static int[,] ReadMatrixFromFile(string filePath, FileMode fileMode = FileMode.Open)
    {
        using var streamReader = new StreamReader(new FileStream(filePath, fileMode));
        return ReadMatrixFromStream(streamReader);
    }

    /// <summary>
    /// Reads matrix written in WriteMatrix method notation.
    /// </summary>
    /// <param name="streamReader">Reading source. Must follow WriteMatrix method notation.</param>
    /// <returns>Matrix readed.</returns>
    /// <exception cref="ArgumentException">Notation not followed.</exception>
    public static int[,] ReadMatrixFromStream(StreamReader streamReader)
    {
        string argumentExceptionText = $"{nameof(streamReader)} doesn't follow WriteMatrix method notation.";

        var firstLine = streamReader.ReadLine()?.Split().Select(int.Parse).ToArray();

        if (firstLine is not[int n, int m])
        {
            throw new ArgumentException(argumentExceptionText);
        }

        var matrix = new int[n, m];

        for (var row = 0; row < n; ++row)
        {
            var newLine = streamReader.ReadLine()?.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)
                ?? throw new ArgumentException(argumentExceptionText);

            var column = 0;
            foreach (var num in newLine)
            {
                matrix[row, column] = num;
                ++column;
            }

            if (column != m)
            {
                throw new ArgumentException(argumentExceptionText);
            }
        }

        return matrix;
    }
}
