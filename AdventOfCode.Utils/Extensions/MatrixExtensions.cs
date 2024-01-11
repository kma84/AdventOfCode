using System.Text;

namespace AdventOfCode.Utils.Extensions
{
    public static class MatrixExtensions
    {

        public static void Fill<T>(this T[,] matrix, T value)
        {
            for (int y = 0; y < matrix.GetLength(0); y++)
            {
                for (int x = 0; x < matrix.GetLength(1); x++)
                {
                    matrix[y, x] = value;
                }
            }
        }


        public static void Print<T>(this T[,] matrix, string? title = null)
        {
            StringBuilder sb = new();

            for (int y = 0; y < matrix.GetLength(0); y++)
            {
                for (int x = 0; x < matrix.GetLength(1); x++)
                {
                    sb.Append(matrix[y, x]?.ToString() ?? string.Empty);
                }

                sb.AppendLine();
            }

            if (title != null)
                Console.WriteLine(title);

            Console.WriteLine(sb.ToString());
        }


        public static int Count<T>(this T[,] matrix, Func<T, bool> predicate)
        {
            int count = 0;

            foreach (T item in matrix)
            {
                if (predicate(item))
                    count++;
            }

            return count;
        }


        public static IEnumerable<(int x, int y, T value)> Where<T>(this T[,] matrix, Func<T, bool> predicate)
        {
            for (int y = 0; y < matrix.GetLength(0); y++)
            {
                for (int x = 0; x < matrix.GetLength(1); x++)
                {
                    if (predicate(matrix[y, x]))
                        yield return (x, y, matrix[y, x]);
                }
            }

            yield break;
        }


        public static List<(int x, int y, T value)> GetCrossAdjacents<T>(this T[,] matrix, int x, int y)
        {
            CheckBounds(matrix, x, y);

            List<(int x, int y, T value)> crossAdjacents =
            [
                .. y - 1 >= 0 ? [(x, y - 1, matrix[y - 1, x])] : (ReadOnlySpan<(int, int, T)>)[],
                .. x + 1 < matrix.GetLength(1) ? [(x + 1, y, matrix[y, x + 1])] : (ReadOnlySpan<(int, int, T)>)[],
                .. y + 1 < matrix.GetLength(0) ? [(x, y + 1, matrix[y + 1, x])] : (ReadOnlySpan<(int, int, T)>)[],
                .. x - 1 >= 0 ? [(x - 1, y, matrix[y, x - 1])] : (ReadOnlySpan<(int, int, T)>)[],
            ];

            return crossAdjacents;
        }


        public static List<(int x, int y, T value)> GetAdjacents<T>(this T[,] matrix, int x, int y)
        {
            List <(int x, int y, T value)> adjacents =
            [
                .. matrix.GetCrossAdjacents(x, y).ToArray(),
                .. y - 1 >= 0 && x - 1 >= 0 ? [(x - 1, y - 1, matrix[y - 1, x - 1])] : (ReadOnlySpan<(int, int, T)>)[],
                .. y - 1 >= 0 && x + 1 < matrix.GetLength(1) ? [(x + 1, y - 1, matrix[y - 1, x + 1])] : (ReadOnlySpan<(int, int, T)>)[],
                .. y + 1 < matrix.GetLength(0) && x - 1 >= 0 ? [(x - 1, y + 1, matrix[y + 1, x - 1])] : (ReadOnlySpan<(int, int, T)>)[],
                .. y + 1 < matrix.GetLength(0) && x + 1 < matrix.GetLength(1) ? [(x + 1, y + 1, matrix[y + 1, x + 1])] : (ReadOnlySpan<(int, int, T)>)[],
            ];

            return adjacents;
        }


        private static void CheckBounds<T>(T[,] matrix, int x, int y)
        {
            if (y < 0 || y >= matrix.GetLength(0))
            {
                throw new IndexOutOfRangeException($"Parameter {nameof(y)} with value {y} is out of array bounds.");
            }

            if (x < 0 || x >= matrix.GetLength(1))
            {
                throw new IndexOutOfRangeException($"Parameter {nameof(x)} with value {x} is out of array bounds.");
            }
        }
    }
}