using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2022.Day12
{
    [Problem(Year = 2022, Day = 12, ProblemName = "Hill Climbing Algorithm")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input)
        {
            char[,] map = InputUtils.ParseMatrix(input);

            Point startPoint = FindAndReplacePoint(CURRENT_POSITION, LOWEST_ELEVATION, map);
            Point endPoint = FindAndReplacePoint(BEST_SIGNAL, HIGHEST_ELEVATION, map);
            bool targetReachedFunc(Point p, char[,] _) => p == endPoint;

            return GetNumSteps(startPoint, map, Direction.ASCENDING, targetReachedFunc).ToString();
        }

        public string Part2(string input)
        {
            char[,] map = InputUtils.ParseMatrix(input);
            
            FindAndReplacePoint(CURRENT_POSITION, LOWEST_ELEVATION, map);
            Point startPoint = FindAndReplacePoint(BEST_SIGNAL, HIGHEST_ELEVATION, map);
            static bool targetReachedFunc(Point p, char[,] m) => m[p.Y, p.X] == LOWEST_ELEVATION;

            return GetNumSteps(startPoint, map, Direction.DESCENDING, targetReachedFunc).ToString();
        }


        private static readonly char CURRENT_POSITION = 'S';
        private static readonly char BEST_SIGNAL = 'E';
        private static readonly char LOWEST_ELEVATION = 'a';
        private static readonly char HIGHEST_ELEVATION = 'z';

        private static List<char> CHAR_VALUES => Enumerable.Range('a', 26).Select(c => (char)c).Union(
               Enumerable.Range('A', 26).Select(c => (char)c)
           ).ToList();


        private static Point FindAndReplacePoint(char targetPoint, char newPoint, char[,] map)
        {
            (int x, int y, _) = map.Where(c => c == targetPoint).Single();
            map[y, x] = newPoint;

            return new(x, y);
        }

        private static int GetNumSteps(Point startPoint, char[,] map, Direction direction, Func<Point, char[,], bool> TargetReachedFunc)
        {
            Dictionary<Point, int> dist = new() { { startPoint, 0 } };
            Dictionary<Point, Point?> prev = new() { { startPoint, default } };
            PriorityQueue<Point, int> priorityQueue = new();

            priorityQueue.Enqueue(startPoint, dist[startPoint]);

            while (priorityQueue.Count > 0)
            {
                Point currentState = priorityQueue.Dequeue();

                if (TargetReachedFunc(currentState, map))
                {
                    return dist[currentState];
                }

                foreach ((Point nextPoint, int nextPointDist) in GetNextPoints(currentState, map, direction))
                {
                    int tryDist = dist[currentState] + nextPointDist;

                    if (!dist.TryGetValue(nextPoint, out int distNextPoint) || tryDist < distNextPoint)
                    {
                        dist[nextPoint] = tryDist;
                        prev[nextPoint] = currentState;
                        priorityQueue.Enqueue(nextPoint, dist[nextPoint]);
                    }
                }
            }

            return 0;
        }

        private static IEnumerable<(Point nextPoint, int dist)> GetNextPoints(Point currentState, char[,] map, Direction direction)
        {
            foreach (var (x, y, dest) in map.GetCrossAdjacents(currentState.X, currentState.Y))
            {
                int dist = (CHAR_VALUES.IndexOf(dest) - CHAR_VALUES.IndexOf(map[currentState.Y, currentState.X])) * (int)direction;

                if (dist <= 1)
                {
                    yield return new(new(x, y), 1);
                }
            }

            yield break;
        }


        private record Point(int X, int Y);

        private enum Direction 
        {
            DESCENDING = -1,
            ASCENDING = 1,
        }
    }
}