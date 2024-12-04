using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2024.Day01
{
    [Problem(Year = 2024, Day = 1, ProblemName = "Historian Hysteria")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input)
        {
            (List<int> list1, List<int> list2) = GetLists(input);

            list1.Sort();
            list2.Sort();

            int result = 0;
            for (int i = 0; i < list1.Count; i++)
            {
                result += Math.Abs(list1[i] - list2[i]);
            }

			return result.ToString();
        }

        public string Part2(string input)
        {
            (List<int> list1, List<int> list2) = GetLists(input);

            int result = 0;
            for (int i = 0; i < list1.Count; i++)
            {
                result += list1[i] * list2.Count(n => n == list1[i]);
            }

            return result.ToString();
        }

        private static (List<int>, List<int>) GetLists(string input)
        {
            List<int> list1 = [];
            List<int> list2 = [];

            foreach (string line in input.GetLines())
            {
                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                list1.Add(int.Parse(parts[0]));
                list2.Add(int.Parse(parts[1]));
            }

            return (list1, list2);
        }
	}
}