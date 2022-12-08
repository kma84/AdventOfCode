using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Utils
{
    public static class InputUtils
    {
        public static int[,] ParseIntMatrix(string matrixStr) => CreateMatrixFromString<int>(matrixStr, (char c) => (int)char.GetNumericValue(c));

        public static char[,] ParseMatrix(string matrixStr) => CreateMatrixFromString<char>(matrixStr, (char c) => c);

        private static T[,] CreateMatrixFromString<T>(string matrixStr, Func<char, T> transformFunc)
        {
            string[] lines = matrixStr.GetLines(StringSplitOptions.RemoveEmptyEntries);

            int maxX = lines.First().Length;
            int maxY = lines.Length;
            T[,] matrix = new T[maxY, maxX];

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    matrix[y, x] = transformFunc(lines[y][x]);
                }
            }

            return matrix;
        }

    }
}
