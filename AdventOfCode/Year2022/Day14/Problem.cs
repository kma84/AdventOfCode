using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2022.Day14
{
    [Problem(Year = 2022, Day = 14, ProblemName = "Regolith Reservoir")]
    internal class Problem : IProblem
    {
        private const char SAND_UNIT = 'o';
        private const char SAND_SOURCE = '+';
        private const char AIR = '.';
        private const char ROCK = '#';

        public bool Debug => false;
        public static bool Visualize => false;


        public string Part1(string input)
        {
            (char[,] map, Point source) = GetMap(input.GetLines());

            SimulateFallingSand(map, source);

            if (Visualize)
                map.Print();

            return map.Count(c => c == SAND_UNIT).ToString();
        }

        public string Part2(string input)
        {
            (char[,] map, Point source) = GetMap(input.GetLines(), addFloor: true);

            SimulateFallingSand(map, source);

            if (Visualize)
                map.Print();

            return map.Count(c => c == SAND_UNIT).ToString();
        }


        private static void SimulateFallingSand(char[,] map, Point source)
        {
            Point? currentPoint = source;
            Point? nextPoint;

            do
            {
                nextPoint = GetSandNextPosition(map, currentPoint);
                if (nextPoint == currentPoint)
                {
                    map[currentPoint.Y, currentPoint.X] = SAND_UNIT;
                    currentPoint = source;
                }
                else
                    currentPoint = nextPoint;
            }
            while (currentPoint != null && nextPoint != source);
        }

        private static Point? GetSandNextPosition(char[,] map, Point currentPoint)
        {
            if (currentPoint.Y == map.GetLength(0) - 1)
                return null;

            if (map[currentPoint.Y + 1, currentPoint.X] == AIR)
                return new Point(currentPoint.X, currentPoint.Y + 1);

            if (currentPoint.X == 0)
                return null;

            if (map[currentPoint.Y + 1, currentPoint.X - 1] == AIR)
                return new Point(currentPoint.X - 1, currentPoint.Y + 1);

            if (currentPoint.X == map.GetLength(1) - 1)
                return null;

            if (map[currentPoint.Y + 1, currentPoint.X + 1] == AIR)
                return new Point(currentPoint.X + 1, currentPoint.Y + 1);

            return currentPoint;
        }

        private static (char[,] map, Point source) GetMap(string[] lines, bool addFloor = false)
        {
            List<List<Point>> coordinates = GetCoordinates(lines);
            List<Point> points = coordinates.SelectMany(l => l).ToList();

            int maxY = points.Max(p => p.Y) + 1 + (addFloor ? 2 : 0);
            int minX = addFloor ? 500 - maxY : points.Min(p => p.X);

            points.ForEach(p => { p.X -= minX; });

            int maxX = addFloor ? maxY * 2 : points.Max(p => p.X) + 1;

            char[,] map = new char[maxY, maxX];
            map.Fill(AIR);

            foreach (List <Point> coordinateList in coordinates)
            {
                for (int i = 1; i < coordinateList.Count; i++)
                {
                    CreateRocks(map, coordinateList[i - 1], coordinateList[i]);
                }
            }

            if (addFloor)
                CreateRocks(map, new Point(0, maxY - 1), new Point(maxX - 1, maxY - 1));

            // Source
            Point sourcePoint = new(500 - minX, 0);
            map[sourcePoint.Y, sourcePoint.X] = SAND_SOURCE;

            return (map, sourcePoint);
        }

        private static void CreateRocks(char[,] map, Point point1, Point point2)
        {
            int minX = Math.Min(point1.X, point2.X);
            int maxX = Math.Max(point1.X, point2.X);
            int minY = Math.Min(point1.Y, point2.Y);
            int maxY = Math.Max(point1.Y, point2.Y);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    map[y, x] = ROCK;
                }
            }
        }

        private static List<List<Point>> GetCoordinates(string[] lines)
        {
            List<List<Point>> coordinates = [];

            foreach (string line in lines)
            {
                List<Point> points = [];

                foreach (string pair in line.Split(" -> "))
                {
                    string[] coors = pair.Split(',');
                    points.Add(new Point ( int.Parse(coors[0]), int.Parse(coors[1]) ));
                }  

                coordinates.Add(points);
            }

            return coordinates;
        }

        private class Point(int x, int y)
        {
            public int X { get; set; } = x;
            public int Y { get; set; } = y;
        };
	}
}