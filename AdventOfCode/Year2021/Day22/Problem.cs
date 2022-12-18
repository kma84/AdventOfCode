using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;
using System.Collections;

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
            List<Step> steps = GetSteps(input).Where(s => s.Cuboid.Coords.All(c => c >= -CUBES_LIMIT_PART1 && c <= CUBES_LIMIT_PART1)).ToList();

            bool?[,,] core = new bool?[CORE_DIMENSIONS_PART1, CORE_DIMENSIONS_PART1, CORE_DIMENSIONS_PART1];

            foreach (Step step in steps)
            {
                for (long x = step.Cuboid.X1; x <= step.Cuboid.X2; x++)
                {
                    for (long y = step.Cuboid.Y1; y <= step.Cuboid.Y2; y++)
                    {
                        for (long z = step.Cuboid.Z1; z <= step.Cuboid.Z2; z++)
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
            List<Step> steps = GetSteps(input);

            long turnedOnCubes = 0;
            long totalArea = 0;
            Dictionary<List<Cuboid>, long> cacheAreas = new(new ListComparer());

            for (int i = steps.Count - 1; i >= 0; i--)
            {
                long antArea = totalArea;

                totalArea = GetArea(steps.Skip(i).Select(s => s.Cuboid).ToList(), cacheAreas);

                if (steps[i].TurnOn)
                    turnedOnCubes += totalArea - antArea;
            }

            return turnedOnCubes.ToString();
        }


        private long GetArea(List<Cuboid> cuboids, Dictionary<List<Cuboid>, long> cache)
        {
            if (!cuboids.Any())
                return 0;

            if (cuboids.Count == 1)
                return cuboids[0].GetArea();

            if (cache.TryGetValue(cuboids, out long value))
                return value;

            List<Cuboid> intersections = new();
            for (int i = 1; i < cuboids.Count; i++)
            {
                Cuboid? intersectionCuboid = GetIntersectionCuboid(cuboids[0], cuboids[i]);

                if (intersectionCuboid != null)
                    intersections.Add(intersectionCuboid);
            }

            // Inclusion-exclusion principal. See https://stackoverflow.com/a/65114685
            long area = cuboids[0].GetArea() 
                        + GetArea(cuboids.Skip(1).ToList(), cache)
                        - GetArea(intersections, cache);

            cache[cuboids] = area;
            return area;
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

                Cuboid cuboid = new(
                    int.Parse(parts[2]), int.Parse(parts[4]),
                    int.Parse(parts[6]), int.Parse(parts[8]),
                    int.Parse(parts[10]), int.Parse(parts[12])
                );

                steps.Add(new Step(GetTurnOnOff(parts[0]), cuboid));
            }

            return steps;
        }


        private static Cuboid? GetIntersectionCuboid(Cuboid cuboidA, Cuboid cuboidB)
        {
            long xMin = Math.Max(cuboidA.X1, cuboidB.X1);
            long xMax = Math.Min(cuboidA.X2, cuboidB.X2);
            long yMin = Math.Max(cuboidA.Y1, cuboidB.Y1);
            long yMax = Math.Min(cuboidA.Y2, cuboidB.Y2);
            long zMin = Math.Max(cuboidA.Z1, cuboidB.Z1);
            long zMax = Math.Min(cuboidA.Z2, cuboidB.Z2);

            if (xMin > xMax || yMin > yMax || zMin > zMax)
                return null;

            return new Cuboid(xMin, xMax, yMin, yMax, zMin, zMax);
        }


        class Step
        {
            public Step(bool turnOn, Cuboid cuboid)
            {
                TurnOn = turnOn;
                Cuboid = cuboid;
            }

            public bool TurnOn { get; set; }

            public Cuboid Cuboid { get; set; }
        }


        record Cuboid
        {
            public Cuboid(long x1, long x2, long y1, long y2, long z1, long z2)
            {
                Coords = new() { x1, x2, y1, y2, z1, z2 };
            }

            public List<long> Coords { get; set; }

            public long X1 => Coords[0];
            public long X2 => Coords[1];
            public long Y1 => Coords[2];
            public long Y2 => Coords[3];
            public long Z1 => Coords[4];
            public long Z2 => Coords[5];

            public long GetArea() => Math.Abs(X2 - X1 + 1) * Math.Abs(Y2 - Y1 + 1) * Math.Abs(Z2 - Z1 + 1);
        }


        class ListComparer : EqualityComparer<List<Cuboid>>
        {
            public override bool Equals(List<Cuboid>? x, List<Cuboid>? y)
              => StructuralComparisons.StructuralEqualityComparer.Equals(x?.ToArray(), y?.ToArray());

            public override int GetHashCode(List<Cuboid> x)
              => StructuralComparisons.StructuralEqualityComparer.GetHashCode(x.ToArray());
        }
    }
}