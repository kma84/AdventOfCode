using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;

namespace AdventOfCode.Year2025.Day02
{
    [Problem(Year = 2025, Day = 2, ProblemName = "Gift Shop")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input)
        {
            var idRanges = GetIdRanges(input);
            int patternRepetitions = 2;
            List<long> ids = [];

            foreach (var (firstIdStr, lastIdStr) in idRanges)
            {
                long firstId = long.Parse(firstIdStr);
                long lastId = long.Parse(lastIdStr);

                string currentIdStr = firstIdStr;
                if (firstIdStr.Length % 2 != 0)
                    currentIdStr = GetExpandedIdString(firstIdStr);

                long currentIdPattern = GetCurrentIdPattern(currentIdStr, patternRepetitions);
                ids.AddRange(GetPatternIds(firstId, lastId, currentIdPattern, patternRepetitions));
            }

            return ids.Sum().ToString();
        }

        public string Part2(string input)
        {
            var idRanges = GetIdRanges(input);
            List<long> ids = [];

            foreach (var (firstIdStr, lastIdStr) in idRanges)
            {
                ids.AddRange(GetPatternIdsByFactors(firstIdStr, lastIdStr));

                if (firstIdStr.Length < lastIdStr.Length)
                {
                    string expandedIdStr = GetExpandedIdString(firstIdStr);
                    ids.AddRange(GetPatternIdsByFactors(expandedIdStr, lastIdStr));
                }
            }

            return ids.Distinct().Sum().ToString();
        }


        private static List<(string firstIdStr, string lastIdStr)> GetIdRanges(string input)
        {
            List<(string, string)> ranges = [];

            foreach (string range in input.Split(','))
            {
                string[] parts = range.Split('-');
                ranges.Add((parts[0], parts[1]));
            }

            return ranges;
        }

        private static List<long> GetPatternIdsByFactors(string idStr, string lastIdStr)
        {
            List<long> ids = [];
            long firstId = long.Parse(idStr);
            long lastId = long.Parse(lastIdStr);
            IEnumerable<int> idFactors = MathUtils.Factors(idStr.Length).Skip(1);

            foreach (int factor in idFactors)
            {
                long currentIdPattern = GetCurrentIdPattern(idStr, factor);
                ids.AddRange(GetPatternIds(firstId, lastId, currentIdPattern, factor));
            }

            return ids;
        }

        private static List<long> GetPatternIds(long firstId, long lastId, long idPattern, int repetitions)
        {
            List<long> ids = [];
            long currentId;

            do
            {
                currentId = GetCurrentId(idPattern, repetitions);

                if (currentId >= firstId && currentId <= lastId)
                {
                    ids.Add(currentId);
                }

                idPattern++;

            } while (currentId <= lastId);

            return ids;
        }

        private static long GetCurrentId(long template, int repetitions)
        {
            return long.Parse(string.Concat(Enumerable.Repeat(template, repetitions)));
        }

        private static long GetCurrentIdPattern(string currentIdStr, int divisor)
        {
            return long.Parse(currentIdStr[..(currentIdStr.Length / divisor)]);
        }

        private static string GetExpandedIdString(string firstIdStr)
        {
            return "1" + string.Concat(Enumerable.Repeat('0', firstIdStr.Length));
        }
    }
}