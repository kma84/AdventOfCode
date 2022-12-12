using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;

namespace AdventOfCode.Year2022.Day12
{
    [Problem(Year = 2022, Day = 12, ProblemName = "Hill Climbing Algorithm")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input)
        {
            Point startPoint = new(0, 0);
            Point endPoint = new(0, 0);
            char[,] map = InputUtils.ParseMatrix(input);

            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    if (map[y, x] == 'S')
                    {
                        map[y, x] = 'a';
                        startPoint = new(x, y);
                    }

                    if (map[y, x] == 'E')
                    {
                        map[y, x] = 'z';
                        endPoint = new(x, y);
                    }
                }
            }

            return GetNumSteps(startPoint, endPoint, map).ToString();
        }

        public string Part2(string input)
        {
            char[,] map = InputUtils.ParseMatrix(input);
            Point startPoint = new(0, 0);
            Point endPoint = new(0, 0);

            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    if (map[y, x] == 'S')
                    {
                        map[y, x] = 'a';
                    }

                    if (map[y, x] == 'E')
                    {
                        map[y, x] = 'z';
                        startPoint = new(x, y);
                    }
                }
            }

            return GetNumStepsPart2(startPoint, map).ToString();
        }


        private static List<char> GetCharValue() => Enumerable.Range('a', 26).Select(c => (char)c).Union(
               Enumerable.Range('A', 26).Select(c => (char)c)
           ).ToList();


        private static int GetNumSteps(Point startPoint, Point endPoint, char[,] map)
        {
            Dictionary<Point, int> dist = new() { { startPoint, 0 } };
            Dictionary<Point, Point?> prev = new() { { startPoint, default } };
            PriorityQueue<Point, int> priorityQueue = new();

            priorityQueue.Enqueue(startPoint, dist[startPoint]);

            while (priorityQueue.Count > 0)
            {
                Point currentState = priorityQueue.Dequeue();

                if (currentState == endPoint)
                {
                    return dist[currentState];
                }

                foreach ((Point nextPoint, int nextPointDist) in GetNextPoints(currentState, map))
                {
                    int tryDist = dist[currentState] + nextPointDist;

                    if (!dist.ContainsKey(nextPoint) || tryDist < dist[nextPoint])
                    {
                        dist[nextPoint] = tryDist;
                        prev[nextPoint] = currentState;
                        priorityQueue.Enqueue(nextPoint, dist[nextPoint]);
                    }
                }
            }

            return 0;
        }

        private static int GetNumStepsPart2(Point startPoint, char[,] map)
        {
            Dictionary<Point, int> dist = new() { { startPoint, 0 } };
            Dictionary<Point, Point?> prev = new() { { startPoint, default } };
            PriorityQueue<Point, int> priorityQueue = new();

            priorityQueue.Enqueue(startPoint, dist[startPoint]);

            while (priorityQueue.Count > 0)
            {
                Point currentState = priorityQueue.Dequeue();

                if (map[currentState.Y, currentState.X] == 'a')
                {
                    return dist[currentState];
                }

                foreach ((Point nextPoint, int nextPointDist) in GetNextPointsPart2(currentState, map))
                {
                    int tryDist = dist[currentState] + nextPointDist;

                    if (!dist.ContainsKey(nextPoint) || tryDist < dist[nextPoint])
                    {
                        dist[nextPoint] = tryDist;
                        prev[nextPoint] = currentState;
                        priorityQueue.Enqueue(nextPoint, dist[nextPoint]);
                    }
                }
            }

            return 0;
        }

        private static IEnumerable<(Point nextPoint, int dist)> GetNextPointsPart2(Point currentState, char[,] map)
        {
            foreach (var (x, y, dest) in map.GetCrossAdjacents(currentState.X, currentState.Y))
            {
                int dist = GetCharValue().IndexOf(map[currentState.Y, currentState.X]) - GetCharValue().IndexOf(dest);

                if (dist <= 1)
                {
                    yield return new(new(x, y), 1);
                }
            }

            yield break;
        }

        private static IEnumerable<(Point nextPoint, int dist)> GetNextPoints(Point currentState, char[,] map)
        {
            foreach (var (x, y, dest) in map.GetCrossAdjacents(currentState.X, currentState.Y))
            {
                int dist = GetCharValue().IndexOf(dest) - GetCharValue().IndexOf(map[currentState.Y, currentState.X]);

                if (dist <= 1)
                {
                    yield return new(new (x, y), 1);
                }
            }

            yield break;
        }

        private record Point(int X, int Y);
    }
}