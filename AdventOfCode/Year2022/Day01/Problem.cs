using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2022.Day01
{
    [Problem(Year = 2022, Day = 1, ProblemName = "Calorie Counting")]
    internal class Problem : IProblem
    {
        public string Part1(string input) => GetCaloriesByElf(input).Max().ToString();

        public string Part2(string input) => GetCaloriesByElf(input).OrderByDescending(c => c).Take(3).Sum().ToString();

        private static IEnumerable<int> GetCaloriesByElf(string input) =>
            input.Split(new string[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.None)
                 .Select(caloriesStr => caloriesStr.GetLines(StringSplitOptions.RemoveEmptyEntries).Select(c => int.Parse(c)).Sum());
    }
}