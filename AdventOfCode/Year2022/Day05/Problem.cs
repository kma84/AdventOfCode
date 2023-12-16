using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2022.Day05
{
    [Problem(Year = 2022, Day = 5, ProblemName = "Supply Stacks")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input)
        {
            (List<List<char>> cratesLists, List<Movement> movements) = ParseInput(input);
            List<Stack<char>> cratesStacks = cratesLists.Select(cl => new Stack<char>(cl.Reverse<char>())).ToList();

            foreach (var (num, source, target) in movements)
            {
                for (int i = 0; i < num; i++)
                    cratesStacks[target].Push(cratesStacks[source].Pop());
            }

			return new string(cratesStacks.Select(s => s.Peek()).ToArray());
        }

        public string Part2(string input)
        {
            (List<List<char>> cratesLists, List<Movement> movements) = ParseInput(input);

            foreach (var (num, source, target) in movements)
            {
                cratesLists[target].InsertRange(0, cratesLists[source].GetRange(0, num));
                cratesLists[source].RemoveRange(0, num);
            }

            return new string(cratesLists.Select(l => l[0]).ToArray());
        }


        private static (List<List<char>> cratesLists, List<Movement> movements) ParseInput(string input)
        {
            List<string> lines = input.GetLines().ToList();
            IEnumerable<string> stackLines = lines.Take(lines.IndexOf(string.Empty) - 1);
            IEnumerable<string> movsLines = lines.Skip(stackLines.Count() + 2);

            return (ParseCratesStacks(stackLines), ParseMovements(movsLines).ToList());
        }

        private static List<List<char>> ParseCratesStacks(IEnumerable<string> stackLines)
        {
            List<List<char>> crates = Enumerable.Range(0, (stackLines.First().Length + 1) / 4).Select(_ => new List<char>()).ToList();

            foreach (string cratesLine in stackLines)
            {
                for (int i = 0; i < crates.Count; i++)
                {
                    int lineIndex = ((i + 1) * 4) - 3;

                    if (char.IsUpper(cratesLine, lineIndex))
                        crates[i].Add(cratesLine[lineIndex]);
                }
            }

            return crates;
        }

        private static IEnumerable<Movement> ParseMovements(IEnumerable<string> movsLines)
        {
            foreach (string movLine in movsLines)
            {
                string[] parts = movLine.Split();

                yield return new Movement(int.Parse(parts[1]), int.Parse(parts[3]) - 1, int.Parse(parts[5]) - 1);
            }
        }

        private record Movement(int CratesNumber, int SourceStack, int TargetStack);
	}
}