using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2023.Day06
{
    [Problem(Year = 2023, Day = 6, ProblemName = "Wait For It")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input)
        {
            string[] lines = input.GetLines();
            int[] times = GetIntArray(lines[0]);
            int[] distances = GetIntArray(lines[1]);

            int winsMult = 1;

            for (int i = 0; i < times.Length; i++)
            {
                int currentTime = times[i];
                int currentDistance = distances[i];

                int wins = Enumerable.Range(1, currentTime - 1).Where(holdTime => IsWinningTime(currentTime, currentDistance, holdTime)).Count();

                winsMult *= wins;
            }

            return winsMult.ToString();
        }

        public string Part2(string input)
        {
            string[] lines = input.GetLines();
            long time = GetLong(lines[0]);
            long distance = GetLong(lines[1]);

            long minHoldTime = 0;
            long maxHoldTime = time;

            while (!IsWinningTime(time, distance, ++minHoldTime)) {};
            while (!IsWinningTime(time, distance, --maxHoldTime)) {};

            return (maxHoldTime - minHoldTime + 1).ToString();
        }

        private static bool IsWinningTime(long time, long distance, long holdTime) => holdTime * (time - holdTime) > distance;

        private static int[] GetIntArray(string line) => line.Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

        private static long GetLong(string line) => long.Parse(line.Split(':')[1].Replace(" ", string.Empty));

    }
}