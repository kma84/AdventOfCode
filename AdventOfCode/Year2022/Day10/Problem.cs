using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2022.Day10
{
    [Problem(Year = 2022, Day = 10, ProblemName = "Cathode-Ray Tube")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input)
        {
            List<int> interestingSignalIndexes = [20, 60, 100, 140, 180, 220];
            List<Cycle> cycles = GetCycles(input.GetLines());

            return interestingSignalIndexes.Select(i => cycles[i - 1].Start * i).Sum().ToString();
        }

        public string Part2(string input)
        {
            List<Cycle> cycles = GetCycles(input.GetLines());
            List<string> output = Enumerable.Repeat(string.Empty, 6).ToList();

            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 40; x++)
                {
                    int regValue = cycles[y * 40 + x].Start;
                    bool litPixel = x >= regValue - 1 && x <= regValue + 1;

                    output[y] += litPixel ? '#' : '.';
                }
            }

            if (Debug)
                output.ForEach(l => Console.WriteLine(l));

            // The returned value is the numeric representation of the CRT, result of transform the lines to binary and sum them
            return GetCRTNumericCode(output).ToString();
        }


        private static List<Cycle> GetCycles(string[] lines)
        {
            List<Cycle> cycles = [];

            foreach (string line in lines)
                cycles.AddRange(ProcessLine(line, cycles.LastOrDefault()?.End ?? 1));

            return cycles;
        }

        private static IEnumerable<Cycle> ProcessLine(string line, int currentRegValue) => line.Split() switch
        {
            ["noop"] => new[] { new Cycle(currentRegValue, currentRegValue) },
            ["addx", string addxValueStr] => new[] { new Cycle(currentRegValue, currentRegValue), new Cycle(currentRegValue, currentRegValue + int.Parse(addxValueStr)) },
            _ => Enumerable.Empty<Cycle>(),
        };

        private static long GetCRTNumericCode(List<string> output) => output.Select(l => Convert.ToInt64(l.Replace('#', '1').Replace('.', '0'), 2)).Sum();


        private record Cycle(int Start, int End);
    }
}