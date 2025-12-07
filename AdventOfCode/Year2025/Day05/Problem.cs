using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2025.Day05
{
    [Problem(Year = 2025, Day = 5, ProblemName = "Cafeteria")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input)
        {
            int freshIngredients = 0;
            var (ranges, availableIngredients) = GetData(input);

            foreach (long ingredient in availableIngredients)
            {
                if (ranges.Any(r => IsIngredientInRange(r, ingredient)))
                    freshIngredients++;
            }

            return freshIngredients.ToString();
        }
                
        public string Part2(string input)
        {
            List<(long firstIngredient, long lastIngredient)> newRanges = [];
            var (ranges, _) = GetData(input);

            ranges = [.. ranges.OrderBy(r => r.firstIngredient).ThenBy(r => r.lastIngredient)];

            (long firstIngredient, long lastIngredient)? newRange = null;
            for (int i = 0; i < ranges.Count; i++) 
            {
                newRange ??= ranges[i];
                int nextIndex = i + 1;

                if (nextIndex < ranges.Count && newRange.Value.lastIngredient >= ranges[nextIndex].firstIngredient)
                {
                    newRange = 
                    (
                        newRange.Value.firstIngredient,
                        Math.Max(newRange.Value.lastIngredient, ranges[nextIndex].lastIngredient)
                    );
                }
                else
                {
                    newRanges.Add(newRange.Value);
                    newRange = null;
                }
            }

            return newRanges.Aggregate(0L, (result, range) => result + range.lastIngredient - range.firstIngredient + 1).ToString();
        }


        private static (List<(long firstIngredient, long lastIngredient)> ranges, List<long> availableIngredients) GetData(string input)
        {
            (List<(long firstIngredient, long lastIngredient)> ranges, List<long> availableIngredients) result = ([], []);

            List<string> lines = [.. input.GetLines()];

            int i = 0;
            string line = lines[i];

            while (!string.IsNullOrEmpty(line))
            {
                string[] parts = line.Split('-');
                result.ranges.Add((long.Parse(parts[0]), long.Parse(parts[1])));
                line = lines[++i];
            } 

            result.availableIngredients = lines[++i ..].ConvertAll(ingredientStr => long.Parse(ingredientStr));

            return result;
        }

        private static bool IsIngredientInRange((long firstIngredient, long lastIngredient) r, long ingredient)
        {
            return r.firstIngredient <= ingredient && r.lastIngredient >= ingredient;
        }
    }
}