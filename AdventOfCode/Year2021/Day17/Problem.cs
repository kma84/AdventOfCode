using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2021.Day17
{
    [Problem(Year = 2021, Day = 17, ProblemName = "Trick Shot")]
    internal class Problem : IProblem
    {
        public bool Debug { get; set; } = false;

        internal static readonly char[] PARTS_X_SEPARATORS = ['.', '=', ','];
        internal static readonly char[] PARTS_Y_SEPARATORS = ['.', '='];

        public string Part1(string input)
        {
            var targetCoords = GetInput(input);

            int maxY = 0;

            for (int y = 0; y < Math.Abs(targetCoords.y.Item1); y++)
            {
                for (int x = 0; x < targetCoords.x.Item2; x++)
                {
                    int? maxTrajectoryY = CalculateTrajectory(x, y, targetCoords);
                    maxY = Math.Max(maxY, maxTrajectoryY ?? 0);
                }
            }

            return maxY.ToString();
        }

        public string Part2(string input)
        {
            var targetCoords = GetInput(input);

            int i = 0;

            for (int y = targetCoords.y.Item1; y <= Math.Abs(targetCoords.y.Item1); y++)
            {
                for (int x = 0; x <= targetCoords.x.Item2; x++)
                {
                    if (CalculateTrajectory(x, y, targetCoords).HasValue)
                    {
                        i++;
                    }
                }
            }

            // Initial velocity values that reach the target:
            return i.ToString();
        }


        private int? CalculateTrajectory(int startVelX, int startVelY, ((int, int) x, (int, int) y) targetCoords)
        {
            List<(int x, int y)> points = [];
            int x = 0;
            int y = 0;
            int velX = startVelX;
            int velY = startVelY;
            int? maxY = null;

            bool targetMissed(int x, int y) => x > targetCoords.x.Item2 || y < targetCoords.y.Item1;

            bool targetReached(int x, int y)
            {
                bool targetReached = x >= targetCoords.x.Item1 && x <= targetCoords.x.Item2 && y >= targetCoords.y.Item1 && y <= targetCoords.y.Item2;

                if (targetReached)
                    maxY = points.Max(p => p.y);

                return targetReached;
            }

            while (!targetMissed(x, y) && !targetReached(x, y))
            {
                (x, y, velX, velY) = Step(x, y, velX, velY);
                points.Add((x, y));
            }

            if (Debug)
                PrintMap(points, targetCoords, $"({startVelX},{startVelY})");

            return maxY;
        }


        private static void PrintMap(List<(int x, int y)> points, ((int, int) x, (int, int) y) targetCoords, string title)
        {
            int lengthX = Math.Max(points.Max(p => p.x), targetCoords.x.Item2) + 1;
            int offsetY = Math.Max(points.Max(p => p.y), 0);
            int lengthY = Math.Abs(Math.Min(points.Min(p => p.y), targetCoords.y.Item1)) + offsetY + 1;

            char[,] map = new char[lengthY, lengthX];

            map.Fill('.');
            map[offsetY, 0] = 'S';

            for (int y = Math.Abs(targetCoords.y.Item2) + offsetY; y <= Math.Abs(targetCoords.y.Item1) + offsetY; y++)
            {
                for (int x = targetCoords.x.Item1; x <= targetCoords.x.Item2; x++)
                {
                    map[y, x] = 'T';
                }
            }

            foreach ((int x, int y) in points)
            {
                map[Math.Abs(y - offsetY), x] = '#';
            }

            map.Print(title);
        }


        private static (int x, int y, int velX, int velY) Step(int x, int y, int velX, int velY)
        {
            x += velX;
            y += velY;

            if (velX > 0)
                velX--;
            else if (velX < 0)
                velX++;

            velY--;

            return (x, y, velX, velY);
        }


        private static ((int, int) x, (int, int) y) GetInput(string input)
        {
            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string[] partsX = parts[2].Split(PARTS_X_SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
            string[] partsY = parts[3].Split(PARTS_Y_SEPARATORS, StringSplitOptions.RemoveEmptyEntries);

            return ((int.Parse(partsX[1]), int.Parse(partsX[2])), (int.Parse(partsY[1]), int.Parse(partsY[2])));
        }
    }
}