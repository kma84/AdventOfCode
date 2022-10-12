using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;

namespace AdventOfCode.Year2021.Day22
{
    [Problem(Year = 2021, Day = 22, ProblemName = "Reactor Reboot")]
    internal class Problem : IProblem
    {
        private const string TURN_ON_STR = "on";
        private const string TURN_OFF_STR = "off";
        private const int CUBES_LIMIT_PART1 = 50;
        private const int CORE_DIMENSIONS_PART1 = 101;

        public bool Debug => false;

        public string Part1(string input)
        {
            List<Step> steps = GetSteps(input).Where(s => s.Cuboid.All(c => c >= -CUBES_LIMIT_PART1 && c <= CUBES_LIMIT_PART1)).ToList();

            bool?[,,] core = new bool?[CORE_DIMENSIONS_PART1, CORE_DIMENSIONS_PART1, CORE_DIMENSIONS_PART1];

            foreach (Step step in steps)
            {
                for (int x = step.CuboidX1; x <= step.CuboidX2; x++)
                {
                    for (int y = step.CuboidY1; y <= step.CuboidY2; y++)
                    {
                        for (int z = step.CuboidZ1; z <= step.CuboidZ2; z++)
                        {
                            core[x + CUBES_LIMIT_PART1, y + CUBES_LIMIT_PART1, z + CUBES_LIMIT_PART1] = step.TurnOn;
                        }
                    }
                }
            }

            int cubesTurnedOn = 0;
            foreach (bool? cube in core)
            {
                if (cube ?? false)
                    cubesTurnedOn++;
            }
            
            return cubesTurnedOn.ToString();
        }

        public string Part2(string input)
        {
			return string.Empty;
        }


        private static List<Step> GetSteps(string input)
        {
            static bool GetTurnOnOff(string turnStr) => turnStr switch
            {
                TURN_ON_STR => true,
                TURN_OFF_STR => false,
                _ => throw new ArgumentException($"Not expected turn value: {turnStr}", nameof(turnStr)),
            };

            string[] lines = input.GetLines(StringSplitOptions.RemoveEmptyEntries);
            List <Step> steps = new();

            foreach (string line in lines)
            {
                string[] parts = line.Split(' ', '=', '.', ',');

                Step step = new()
                {
                    TurnOn = GetTurnOnOff(parts[0]),
                    Cuboid = new() {
                        int.Parse(parts[2]), int.Parse(parts[4]),
                        int.Parse(parts[6]), int.Parse(parts[8]),
                        int.Parse(parts[10]), int.Parse(parts[12])
                    }
                };

                steps.Add(step);
            }

            return steps;
        }


        record Step
        {
            public bool TurnOn { get; set; }

            public List<int> Cuboid { get; set; } = new List<int>();

            public int CuboidX1 => Cuboid[0];
            public int CuboidX2 => Cuboid[1];
            public int CuboidY1 => Cuboid[2];
            public int CuboidY2 => Cuboid[3];
            public int CuboidZ1 => Cuboid[4];
            public int CuboidZ2 => Cuboid[5];
        }
	}
}