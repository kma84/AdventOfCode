using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2021.Day12
{
    [Problem(Year = 2021, Day = 12, ProblemName = "Passage Pathing")]
    internal class Problem : IProblem
    {
        public int NumOfVisitsFirstSmallCave { get; set; }

        public bool Debug { get; set; } = false;


        public string Part1(string input)
        {
            NumOfVisitsFirstSmallCave = 1;
            List<string> paths = [];

            Cave startCave = GetInput(input);
            GetPaths(startCave, [], paths);

            if (Debug)
                Console.WriteLine(string.Join('\n', paths));

            return paths.Count.ToString();
        }

        public string Part2(string input)
        {
            NumOfVisitsFirstSmallCave = 2;
            List<string> paths = [];

            Cave startCave = GetInput(input);
            GetPaths(startCave, [], paths);

            return paths.Count.ToString();
        }


        private void GetPaths(Cave cave, List<Cave> currentPath, List<string> paths)
        {
            if (cave.IsEndCave())
            {
                currentPath.Add(cave);
                paths.Add(string.Join(',', currentPath.Select(c => c.Name)));
                return;
            }

            if (cave.IsSmallCave() && !CanVisitSmallCave(cave, currentPath))
            {
                return;
            }

            currentPath.Add(cave);

            foreach (Cave adjacentCave in cave.Connections)
            {
                GetPaths(adjacentCave, new List<Cave>(currentPath), paths);
            }

            return;
        }


        private bool CanVisitSmallCave(Cave smallCave, List<Cave> currentPath)
        {
            if (!currentPath.Contains(smallCave))
                return true;

            if (smallCave.IsStartCave())
                return false;

            return !currentPath.Where(c => c.IsSmallCave())
                               .GroupBy(c => c)
                               .Any(g => g.Count() >= NumOfVisitsFirstSmallCave);
        }

        static Cave GetInput(string input)
        {
            Dictionary<string, Cave> caves = [];

            foreach (string line in input.GetLines(StringSplitOptions.RemoveEmptyEntries))
            {
                string[] cavesStr = line.Split('-', StringSplitOptions.RemoveEmptyEntries);
                string caveAStr = cavesStr[0];
                string caveBStr = cavesStr[1];

                if (!caves.ContainsKey(caveAStr))
                    caves.Add(caveAStr, new Cave { Name = caveAStr });

                if (!caves.ContainsKey(caveBStr))
                    caves.Add(caveBStr, new Cave { Name = caveBStr });

                caves[caveAStr].Connections.Add(caves[caveBStr]);
                caves[caveBStr].Connections.Add(caves[caveAStr]);
            }

            return caves.Values.Single(c => c.IsStartCave());
        }


        class Cave
        {
            public string Name { get; set; } = string.Empty;

            public List<Cave> Connections { get; set; } = [];

            public bool IsSmallCave()
            {
                return Name.All(c => char.IsLower(c));
            }

            public bool IsStartCave()
            {
                return Name == "start";
            }

            public bool IsEndCave()
            {
                return Name == "end";
            }
        }
    }
}