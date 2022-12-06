using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;

namespace AdventOfCode.Year2022.Day06
{
    [Problem(Year = 2022, Day = 6, ProblemName = "Tuning Trouble")]
    internal class Problem : IProblem
    {
        public string Part1(string input) => GetCharactersProcessed(input, markerLength: 4).ToString();

        public string Part2(string input) => GetCharactersProcessed(input, markerLength: 14).ToString();


        public static int GetCharactersProcessed(string input, int markerLength)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input.Skip(i).Take(markerLength).Distinct().Count() == markerLength)
                {
                    return i + markerLength;
                }
            }

            return 0;
        }
    }
}