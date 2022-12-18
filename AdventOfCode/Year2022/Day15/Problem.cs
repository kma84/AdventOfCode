using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Geometry;
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

            // Indexes where the exclusion zones of each sensor begin and end (for line y). True for begin, false for end.
            List<(int index, bool startOrEnd)> intervals = new();

            foreach (Sensor sensor in sensors.Where(s => s.ExclusionZone.pointA.Y <= y && s.ExclusionZone.pointC.Y >= y))
            {
                int minX = sensor.ExclusionZone.pointD.X;
                int maxX = sensor.ExclusionZone.pointB.X;
                int distanceToY = Calculations.GetManhattanDistance(sensor.ExclusionZone.pointD, new Point(minX, y));
                int intervalStart = minX + distanceToY;
                int intervalEnd = maxX - distanceToY;

                intervals.Add((intervalStart, true));
                intervals.Add((intervalEnd, false));
            }

            intervals = SimplifyIntervals(intervals);            
            int baeconsInY = sensors.Select(s => s.ClosestBeacon).Distinct().Count(p => p.Y == y);

            return (CountPositions(intervals) - baeconsInY).ToString();
        }


        public string Part2(string input)
        {
			return string.Empty;
        }


        private static List<(int index, bool startOrEnd)> SimplifyIntervals(List<(int index, bool startOrEnd)> intervals)
        {
            var orderedIntervals = intervals.OrderBy(i => i.index).ThenByDescending(i => i.startOrEnd);

            List<(int index, bool startOrEnd)> newIntervals = new();
            Stack<(int index, bool startOrEnd)> stack = new();

            foreach (var currentPair in orderedIntervals)
            {
                if (currentPair.startOrEnd)
                {
                    stack.Push(currentPair);
                }
                else
                {
                    var intervalStart = stack.Pop();

                    if (!stack.Any())
                    {
                        newIntervals.Add(intervalStart);
                        newIntervals.Add(currentPair);
                    }
                }
            }

            return newIntervals;
        }


        private static int CountPositions(List<(int index, bool startOrEnd)> intervals)
        {
            int count = 0;

            for (int i = 0; i < intervals.Count - 1; i++)
            {
                count += intervals[i + 1].index - intervals[i].index + 1;
            }

            return count;
        }


        private static List<Sensor> GetSensorsInfo(string[] lines)
        {
            List <Sensor> sensors = new ();

            foreach (string line in lines)
            {
                Match match = InputRegex().Match(line);
                Point sensorPoint = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
                Point closestBeacon = new(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));

                sensors.Add(new Sensor(sensorPoint, closestBeacon));
            }

            return sensors;
        }


        private class Sensor
        {
            public Point SensorPoint { get; set; }
            public Point ClosestBeacon { get; set; }
            public (Point pointA, Point pointB, Point pointC, Point pointD) ExclusionZone { get; set; }

            public Sensor(Point sensorPoint, Point closestBeacon)
            {
                SensorPoint = sensorPoint;
                ClosestBeacon = closestBeacon;
                ExclusionZone = GetExclusionZone(sensorPoint, closestBeacon);
            }

            private static (Point pointA, Point pointB, Point pointC, Point pointD) GetExclusionZone(Point sensorPoint, Point closestBeacon)
            {
                int manhattanDistance = Calculations.GetManhattanDistance(sensorPoint, closestBeacon);

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