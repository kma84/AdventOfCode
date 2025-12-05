using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2025.Day03
{
    [Problem(Year = 2025, Day = 3, ProblemName = "Lobby")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input)
        {
            List<int> joltages = [];

            foreach (string bankStr in input.GetLines())
            {
                List<char> bank = [.. bankStr];

                char firsDigit = bank.SkipLast(1).Max();
                char lastDigit = bank[(bank.IndexOf(firsDigit) + 1) ..].Max();

                joltages.Add(int.Parse($"{firsDigit}{lastDigit}"));
            }

            return joltages.Sum().ToString();
        }

        public string Part2(string input)
        {
            List<long> joltages = [];
            
            foreach (string bankStr in input.GetLines())
            {
                List<char> bank = [.. bankStr];
                int discards = Debug ? DEBUG_DISCARDS : REAL_DISCARDS;
                string joltageStr = string.Empty;

                while (joltageStr.Length < JOLTAGE_PART2_LENGTH)
                {
                    char nextDigit = bank[..Math.Min(discards + 1, bank.Count)].Max();
                    int digitIndex = bank.IndexOf(nextDigit);
                    bank.RemoveRange(0, digitIndex + 1);

                    joltageStr += nextDigit;
                    discards -= digitIndex;
                }

                joltages.Add(long.Parse(joltageStr));
            }

            return joltages.Sum().ToString();
        }


        private static readonly int DEBUG_DISCARDS = 3;
        private static readonly int REAL_DISCARDS = 88;
        private static readonly int JOLTAGE_PART2_LENGTH = 12;
    }
}