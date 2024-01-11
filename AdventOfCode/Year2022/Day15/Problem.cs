using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Classes;
using AdventOfCode.Utils.Extensions;
using AdventOfCode.Utils;
using System.Text.RegularExpressions;

namespace AdventOfCode.Year2022.Day15
{
    [Problem(Year = 2022, Day = 15, ProblemName = "Beacon Exclusion Zone")]
    internal partial class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input)
        {
            List<Sensor> sensors = GetSensorsInfo(input.GetLines());

            int minXT = sensors.Select(s => s.ExclusionZone.pointD.X).Min();
            int maxXT = sensors.Select(s => s.ExclusionZone.pointB.X).Max();
            int y = Debug ? 10 : 2000000;

            var intervals = GetIntervals(sensors, y);
            int baeconsInY = sensors.Select(s => s.ClosestBeacon).Distinct().Count(p => p.Y == y);

            return (CountPositions(intervals) - baeconsInY).ToString();
        }

        public string Part2(string input)
        {
            List<Sensor> sensors = GetSensorsInfo(input.GetLines());
            int distressSignalZone = Debug ? 20 : 4000000;
            Point? distressPoint = null;

            Parallel.For(0, distressSignalZone, (i, state) =>
            {
                int lineIndex = distressSignalZone - i - 1;
                var intervals = GetIntervals(sensors, lineIndex);

                if (intervals.Count > 2)
                {
                    distressPoint = new Point(intervals[1].x + 1, intervals[1].y);
                    state.Stop();
                }
            });

            long tuningFrequency = (distressPoint?.X ?? 0) * 4000000L + (distressPoint?.Y ?? 0);

            return tuningFrequency.ToString();
        }


        private static List<(int x, int y, bool startOrEnd)> GetIntervals(List<Sensor> sensors, int y)
        {
            // Indexes where the exclusion zones of each sensor begin and end (for line y). True for begin, false for end.
            List<(int x, int y, bool startOrEnd)> intervals = [];

            foreach (Sensor sensor in sensors.Where(s => s.ExclusionZone.pointA.Y <= y && s.ExclusionZone.pointC.Y >= y))
            {
                int minX = sensor.ExclusionZone.pointD.X;
                int maxX = sensor.ExclusionZone.pointB.X;
                int distanceToY = GeometryCalculations.GetManhattanDistance(sensor.ExclusionZone.pointD, new Point(minX, y));
                int intervalStart = minX + distanceToY;
                int intervalEnd = maxX - distanceToY;

                intervals.Add((intervalStart, y, true));
                intervals.Add((intervalEnd, y, false));
            }

            return SimplifyIntervals(intervals);
        }

        private static List<(int x, int y, bool startOrEnd)> SimplifyIntervals(List<(int x, int y, bool startOrEnd)> intervals)
        {
            var orderedIntervals = intervals.OrderBy(i => i.x).ThenByDescending(i => i.startOrEnd);

            List<(int x, int y, bool startOrEnd)> newIntervals = [];
            Stack<(int x, int y, bool startOrEnd)> stack = new();

            foreach (var currentPair in orderedIntervals)
            {
                if (currentPair.startOrEnd)
                {
                    stack.Push(currentPair);
                }
                else
                {
                    var intervalStart = stack.Pop();

                    if (stack.Count == 0)
                    {
                        newIntervals.Add(intervalStart);
                        newIntervals.Add(currentPair);
                    }
                }
            }

            return newIntervals;
        }

        private static int CountPositions(List<(int x, int y, bool startOrEnd)> intervals)
        {
            int count = 0;

            for (int i = 0; i < intervals.Count - 1; i++)
            {
                count += intervals[i + 1].x - intervals[i].x + 1;
            }

            return count;
        }

        private static List<Sensor> GetSensorsInfo(string[] lines)
        {
            List <Sensor> sensors = [];

            foreach (string line in lines)
            {
                Match match = InputRegex().Match(line);
                Point sensorPoint = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
                Point closestBeacon = new(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));

                sensors.Add(new Sensor(sensorPoint, closestBeacon));
            }

            return sensors;
        }


        private class Sensor(Point sensorPoint, Point closestBeacon)
        {
            public Point SensorPoint { get; set; } = sensorPoint;
            public Point ClosestBeacon { get; set; } = closestBeacon;
            public (Point pointA, Point pointB, Point pointC, Point pointD) ExclusionZone { get; set; } = GetExclusionZone(sensorPoint, closestBeacon);

            private static (Point pointA, Point pointB, Point pointC, Point pointD) GetExclusionZone(Point sensorPoint, Point closestBeacon)
            {
                int manhattanDistance = GeometryCalculations.GetManhattanDistance(sensorPoint, closestBeacon);

                return (
                    new Point(sensorPoint.X, sensorPoint.Y - manhattanDistance),
                    new Point(sensorPoint.X + manhattanDistance, sensorPoint.Y),
                    new Point(sensorPoint.X, sensorPoint.Y + manhattanDistance),
                    new Point(sensorPoint.X - manhattanDistance, sensorPoint.Y)
                );
            }
        }

        [GeneratedRegex("Sensor at x=(.*?), y=(.*?): closest beacon is at x=(.*?), y=(.*?)$")]
        private static partial Regex InputRegex();
    }
}