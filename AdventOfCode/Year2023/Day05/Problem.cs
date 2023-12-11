using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;

namespace AdventOfCode.Year2023.Day05
{
    [Problem(Year = 2023, Day = 5, ProblemName = "If You Give A Seed A Fertilizer")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input)
        {
            string[] lines = input.GetLines();

            List<long> seeds = GetSeeds(lines[0]);
            List<List<MapRecord>> maps = GetMaps(lines.Skip(2).ToList());
            List<long> locations = new();

            foreach (long seed in seeds)
            {
                long location = seed;

                foreach (List<MapRecord> map in maps)
                {
                    foreach (MapRecord mapRecord in map)
                    {
                        if (location >= mapRecord.SourceRangeStart && location < mapRecord.SourceRangeStart + mapRecord.RangeLength)
                        {
                            location = mapRecord.DestinationRangeStart + location - mapRecord.SourceRangeStart;
                            break;
                        }
                    }
                }

                locations.Add(location);
            }

            return locations.Min().ToString();
        }

        public string Part2(string input)
        {
            string[] lines = input.GetLines();

            List<SeedRange> seeds = GetSeedRanges(lines[0]);
            List<List<MapRecord>> maps = GetMaps(lines.Skip(2).ToList());
            long location = 0;

            for (long i = 0; i < long.MaxValue; i++)
            {
                long minSeed = i;
                location = i;

                for (int mapIndex = maps.Count - 1; mapIndex >= 0; mapIndex--)
                {
                    foreach (MapRecord mapRecord in maps[mapIndex])
                    {
                        if (minSeed >= mapRecord.DestinationRangeStart && minSeed < mapRecord.DestinationRangeStart + mapRecord.RangeLength)
                        {
                            minSeed = mapRecord.SourceRangeStart + minSeed - mapRecord.DestinationRangeStart;
                            break;
                        }
                    }
                }

                if (IsValidSeed(minSeed, seeds))
                    break;
            }

            return location.ToString();
        }

        private static bool IsValidSeed(long minSeed, List<SeedRange> seeds) => seeds.Any(s => minSeed >= s.RangeStart && minSeed < s.RangeStart + s.RangeLength);

        private static List<SeedRange> GetSeedRanges(string line)
        {
            List<SeedRange> seeds = new();
            List<long> tokens = GetSeeds(line);

            for (int i = 0; i < tokens.Count; i += 2)
            {
                seeds.Add(new SeedRange(tokens[i], tokens[i + 1]));
            }

            return seeds;
        }

        private static List<long> GetSeeds(string line) => line[7..].Split().Select(long.Parse).ToList();

        private static List<List<MapRecord>> GetMaps(List<string> lines)
        {
            List<List<MapRecord>> maps = new();
            int mapIndex = -1;

            for (int i = 0; i < lines.Count; i++)
            {
                string[] tokens = lines[i].Split();

                if (long.TryParse(tokens[0], out long destinationRangeStart))
                {
                    maps[mapIndex].Add(new MapRecord(destinationRangeStart, long.Parse(tokens[1]), long.Parse(tokens[2])));
                }
                else if (tokens.Length > 1)
                {
                    maps.Add(new List<MapRecord>());
                    mapIndex++;
                }
            }

            return maps;
        }


        private record MapRecord(long DestinationRangeStart, long SourceRangeStart, long RangeLength);

        private record SeedRange(long RangeStart, long RangeLength);
	}
}