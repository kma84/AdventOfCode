using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;
using Range = AdventOfCode.Utils.Classes.Range;

namespace AdventOfCode.Year2023.Day05
{
    [Problem(Year = 2023, Day = 5, ProblemName = "If You Give A Seed A Fertilizer")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input)
        {
            string[] lines = input.GetLines();

            List<Range> seeds = GetSeeds(lines[0]);
            List<List<MapRecord>> maps = GetMaps(lines.Skip(2).ToList());
            List<Range> resultSeeds = GetResults(seeds, maps);

            return resultSeeds.Min(r => r.Start).ToString();
        }

        public string Part2(string input)
        {
            string[] lines = input.GetLines();

            List<Range> seeds = GetSeedRanges(lines[0]);
            List<List<MapRecord>> maps = GetMaps(lines.Skip(2).ToList());
            List<Range> resultSeeds = GetResults(seeds, maps);

            return resultSeeds.Min(r => r.Start).ToString();
        }


        private static List<Range> GetResults(List<Range> seeds, List<List<MapRecord>> maps)
        {
            List<Range> resultSeeds = seeds;

            foreach (var map in maps)
            {
                List<Range> newResultRanges = [];

                foreach (var srcSeed in resultSeeds)
                {
                    List<Range> intDest = [];
                    List<Range> currentRanges = [srcSeed];
                    List<Range> newRanges = [];

                    foreach (var mapRecord in map)
                    {
                        foreach (var currentRange in currentRanges)
                        {
                            Range? intSrc = srcSeed.Intersect(mapRecord.SourceRecord);

                            if (intSrc != null)
                            {
                                intDest.Add(mapRecord.GetDestFromSrcRange(intSrc));

                                newRanges.Clear();
                                newRanges.AddRange(currentRange.Except(mapRecord.SourceRecord));
                            }
                            else if (!newRanges.Contains(currentRange))
                            {
                                newRanges.Add(currentRange);
                            }
                        }

                        currentRanges = new(newRanges);
                    }

                    if (intDest.Count == 0)
                        newResultRanges.Add(srcSeed);

                    newResultRanges.AddRange(newRanges);
                    newResultRanges.AddRange(intDest);
                }

                resultSeeds = newResultRanges;
            }

            return resultSeeds;
        }

        private static List<Range> GetSeeds(string line) => line[7..].Split().Select(s => new Range(long.Parse(s))).ToList();

        private static List<Range> GetSeedRanges(string line)
        {
            List<Range> seeds = [];
            List<long> tokens = line[7..].Split().Select(long.Parse).ToList(); ;

            for (int i = 0; i < tokens.Count; i += 2)
            {
                seeds.Add(new Range(tokens[i], tokens[i] + tokens[i + 1] - 1));
            }

            return seeds;
        }

        private static List<List<MapRecord>> GetMaps(List<string> lines)
        {
            List<List<MapRecord>> maps = [];
            int mapIndex = -1;

            for (int i = 0; i < lines.Count; i++)
            {
                string[] tokens = lines[i].Split();

                if (long.TryParse(tokens[0], out long dest))
                {
                    long src = long.Parse(tokens[1]);
                    long length = long.Parse(tokens[2]);

                    MapRecord mapRecord = new(
                        new Range(dest, dest + length - 1),
                        new Range(src, src + length - 1)
                    );

                    maps[mapIndex].Add(mapRecord);
                }
                else if (tokens.Length > 1)
                {
                    maps.Add([]);
                    mapIndex++;
                }
            }

            return maps;
        }
                

        private record MapRecord(Range DestinationRecord, Range SourceRecord)
        {
            public Range GetDestFromSrcRange(Range src)
            {
                long startOffset = src.Start - SourceRecord.Start;
                long endOffset = SourceRecord.End - src.End;

                return new Range(DestinationRecord.Start + startOffset, DestinationRecord.End - endOffset);
            }
        }

    }
}