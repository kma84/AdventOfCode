using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2023.Day03
{
    [Problem(Year = 2023, Day = 3, ProblemName = "Gear Ratios")]
    internal class Problem : IProblem
    {
        public bool Debug => false;


        public string Part1(string input) => GetNumbers(GetInput(input)).Sum(n => n.Number).ToString();

        public string Part2(string input) => GetNumbers(GetInput(input))
                .Where(n => n.Symbol.value == '*')
                .GroupBy(n => new { n.Symbol.x, n.Symbol.y })
                .Where(grp => grp.Count() == 2)
                .Aggregate(0, (total, group) => total + group.First().Number * group.Last().Number)
                .ToString();


        private static List<NumberRecord> GetNumbers(char[,] matrix)
        {
            List<NumberRecord> numbers = new ();

            for (int y = 0; y < matrix.GetLength(0); y++)
            {
                List<char> numberChars = new();
                (int x, int y, char value)? symbol = null;
                int number;

                for (int x = 0; x < matrix.GetLength(1); x++)
                {
                    if (char.IsNumber(matrix[y, x]))
                    {
                        numberChars.Add(matrix[y, x]);

                        var symbols = matrix.GetAdjacents(x, y).Where(adj => adj.value != '.' && !char.IsNumber(adj.value));

                        if (symbols.Any())
                            symbol = symbols.First();
                    }
                    else
                    {
                        if (symbol.HasValue)
                        {
                            number = int.Parse(new String(numberChars.ToArray()));
                            numbers.Add(new NumberRecord(number, symbol.Value));

                            symbol = null;
                        }

                        numberChars.Clear();
                    }
                }

                if (symbol.HasValue)
                {
                    number = int.Parse(new String(numberChars.ToArray()));
                    numbers.Add(new NumberRecord(number, symbol.Value));
                }
            }

            return numbers;
        }


        private static char[,] GetInput(string input)
        {
            string[] rows = input.GetLines();
            char[,] matrix = new char[rows.Length, rows.First().Length];

            for (int y = 0; y < matrix.GetLength(0); y++)
            {
                for (int x = 0; x < matrix.GetLength(1); x++)
                {
                    matrix[y, x] = rows[y][x];
                }
            }

            return matrix;
        }


        private record NumberRecord(int Number, (int x, int y, char value) Symbol);

    }
}