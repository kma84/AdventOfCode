using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2025.Day06
{
    [Problem(Year = 2025, Day = 6, ProblemName = "Trash Compactor")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input)
        {
            (List<List<long>> worksheet, List<char> operations) = GetWorksheet(input, Part.Part1);
            long total = CalculateWorksheet(worksheet, operations);

            return total.ToString();
        }

        public string Part2(string input)
        {
            (List<List<long>> worksheet, List<char> operations) = GetWorksheet(input, Part.Part2);
            long total = CalculateWorksheet(worksheet, operations);

            return total.ToString();
        }


        private static readonly char ADD = '+';
        private static readonly char MULT = '*';
        private static readonly char[] OPERATION_TYPES = [ADD, MULT];

        private enum Part
        {
            Part1,
            Part2
        }
        
        private static (List<List<long>> worksheet, List<char> operations) GetWorksheet(string input, Part part)
        {
            List<List<long>> worksheet = [];
            string[] lines = input.GetLines();
            string operationsLine = lines.Last();
            List<char> operations = [.. operationsLine.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(op => op[0])];

            for (int i = 0; i < operations.Count; i++)
                worksheet.Add([]);

            if (part == Part.Part1)
                GetWorksheetPart1(worksheet, lines);
            else if (part == Part.Part2)
                GetWorksheetPart2(worksheet, lines, operationsLine);

            return (worksheet, operations);
        }

        private static void GetWorksheetPart1(List<List<long>> worksheet, string[] lines)
        {
            foreach (string line in lines.SkipLast(1))
            {
                string[] numbers = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < numbers.Length; i++)
                {
                    worksheet[i].Add(long.Parse(numbers[i]));
                }
            }
        }

        private static void GetWorksheetPart2(List<List<long>> worksheet, string[] lines, string operationsLine)
        {
            int currentIndex = 0;
            int nextIndex = 0;
            int currentWorksheetIndex = 0;
            int colLength;

            while (nextIndex != -1)
            {
                nextIndex = operationsLine.IndexOfAny(OPERATION_TYPES, currentIndex + 1);
                colLength = (nextIndex == -1 ? operationsLine.Length : nextIndex - 1) - currentIndex;

                for (int i = 0; i < colLength; i++)
                    worksheet[currentWorksheetIndex].Add(GetNumber(lines, currentIndex + i));

                currentIndex = nextIndex;
                currentWorksheetIndex++;
            }
        }

        private static long GetNumber(string[] lines, int index)
        {
            string numberStr = string.Empty;

            foreach (string line in lines.SkipLast(1))
                numberStr += line[index];

            return long.Parse(numberStr);
        }

        private static long ApplyOperation(long number1, long number2, char operation) => operation switch
        {
            var op when op == ADD => number1 + number2,
            var op when op == MULT => number1 * number2,
            _ => throw new NotSupportedException($"Operation '{operation}' is not supported.")
        };

        private static long CalculateWorksheet(List<List<long>> worksheet, List<char> operations)
        {
            long total = 0;

            for (int i = 0; i < worksheet.Count; i++)
            {
                List<long> numbers = worksheet[i];
                long seed = operations[i] == ADD ? 0L : 1L;

                total += numbers.Aggregate(seed, (total, number) => ApplyOperation(total, number, operations[i]));
            }

            return total;
        }
    }
}