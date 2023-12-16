using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2022.Day03
{
    [Problem(Year = 2022, Day = 3, ProblemName = "Rucksack Reorganization")]
    internal class Problem : IProblem
    {
        private static List<char> GetItemTypes() => Enumerable.Range('a', 26).Select(c => (char)c).Union(
                       Enumerable.Range('A', 26).Select(c => (char)c)
                   ).ToList();

        public string Part1(string input)
        {
            int sumOfPriorities = 0;
            List<char> itemTypes = GetItemTypes();

            foreach (string line in input.GetLines(StringSplitOptions.RemoveEmptyEntries))
            {
                char repeatedItem = line[..(line.Length / 2)].Intersect(line[(line.Length / 2)..]).First();
                sumOfPriorities += itemTypes.IndexOf(repeatedItem) + 1;
            }

            return sumOfPriorities.ToString();
        }

        public string Part2(string input)
        {
            int sumOfPriorities = 0;
            List<char> itemTypes = GetItemTypes();

            foreach (var group in input.GetLines(StringSplitOptions.RemoveEmptyEntries).Chunk(3))
            {
                char repeatedItem = group[0].Intersect(group[1]).Intersect(group[2]).First();
                sumOfPriorities += itemTypes.IndexOf(repeatedItem) + 1;
            }

            return sumOfPriorities.ToString();
        }
	}
}