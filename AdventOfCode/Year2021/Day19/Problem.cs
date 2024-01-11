using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2021.Day19
{
    [Problem(Year = 2021, Day = 19, ProblemName = "Beacon Scanner")]
    internal class Problem : IProblem
    {
        public bool Debug { get; set; } = false;

        internal static readonly char[] SEPARATORS = ['(', ')', ','];

        public string Part1(string input)
        {
            List<Scanner> scanners = GetInput(input);

            for (int i = 0; i < scanners.Count; i++)
            {
                for (int j = 0; j < scanners.Count; j++)
                {
                    if (i == j)
                        continue;

                    Intersect(scanners[i], scanners[j]);
                }
            }

            HashSet<(int x, int y, int z)> resultPoints = new(scanners[0].Beacons);
            scanners[0].Merged = true;
            foreach (Collision collision in scanners[0].Collisions.Where(c => !c.TargetScanner.Merged))
            {
                collision.TargetScanner.Merged = true;
                resultPoints.UnionWith(GetPointsRelativeToA(scanners[0], collision.TargetScanner, collision.SourcePoint, collision.TargetPoint));
            }

            if (Debug)
                Console.WriteLine("Number of beacons detected: " + resultPoints.Count);

            return resultPoints.Count.ToString();
        }

        public string Part2(string input)
        {
            List<Scanner> scanners = GetInput(input);

            for (int i = 0; i < scanners.Count; i++)
            {
                for (int j = 0; j < scanners.Count; j++)
                {
                    if (i == j)
                        continue;

                    Intersect(scanners[i], scanners[j]);
                }
            }

            HashSet<(int x, int y, int z)> scannersPoints = [(0, 0, 0)];
            scanners[0].Merged = true;
            foreach (Collision collision in scanners[0].Collisions.Where(c => !c.TargetScanner.Merged))
            {
                collision.TargetScanner.Merged = true;
                scannersPoints.UnionWith(GetScannerPointsRelativeToA(scanners[0], collision.TargetScanner, collision.SourcePoint, collision.TargetPoint));
            }

            List<(int x, int y, int z)> lstScannersPoints = new(scannersPoints);
            int maxManhattanDistance = 0;

            int iOffset = 0;
            for (int i = 0; i < lstScannersPoints.Count; i++)
            {
                for (int j = iOffset; j < lstScannersPoints.Count; j++)
                {
                    maxManhattanDistance = Math.Max(maxManhattanDistance, GetManhattanDistance(lstScannersPoints[i], lstScannersPoints[j]));
                }

                iOffset++;
            }

            if (Debug)
                Console.WriteLine("Largest manhattan distance: " + maxManhattanDistance);

            return maxManhattanDistance.ToString();
        }


        private static int GetManhattanDistance((int x, int y, int z) pointA, (int x, int y, int z) pointB)
        {
            return Math.Abs(pointA.x - pointB.x) + Math.Abs(pointA.y - pointB.y) + Math.Abs(pointA.z - pointB.z);
        }


        private static List<(int x, int y, int z)> GetScannerPointsRelativeToA(Scanner scannerA, Scanner scannerB, (int x, int y, int z) pointA, (int x, int y, int z) pointB)
        {
            foreach (string rotation in Config.ROTATIONS)
            {
                (int rotatedBPointX, int rotatedBPointY, int rotatedBPointZ) = RotatePoint(pointB, rotation);
                (int x, int y, int z) scannerCoordsRelToA = (pointA.x - rotatedBPointX, pointA.y - rotatedBPointY, pointA.z - rotatedBPointZ);

                List<(int x, int y, int z)> lstRotatedPoints = new(RotatePoints(scannerB.Beacons, rotation));
                List<(int x, int y, int z)> lstRotatedPointsRelToA = SumScannerCoords(lstRotatedPoints, scannerCoordsRelToA);

                var intersection = scannerA.Beacons.Intersect(lstRotatedPointsRelToA);
                if (intersection.Count() >= Config.MIN_OVERLAPPING_BEACONS - 1)
                {
                    HashSet<(int x, int y, int z)> hSetScannerPoints = [scannerCoordsRelToA];

                    foreach (Collision collision in scannerB.Collisions.Where(c => !c.TargetScanner.Merged))
                    {
                        collision.TargetScanner.Merged = true;
                        var newpoints = GetScannerPointsRelativeToA(scannerB, collision.TargetScanner, collision.SourcePoint, collision.TargetPoint);
                        var newPointsRotated = RotatePoints(newpoints, rotation);
                        hSetScannerPoints.UnionWith(SumScannerCoords(newPointsRotated, scannerCoordsRelToA));
                    }

                    return [.. hSetScannerPoints];
                }
            }

            throw new Exception($"Collision not found. {scannerA}-{scannerB}");
        }


        private static List<(int x, int y, int z)> GetPointsRelativeToA(Scanner scannerA, Scanner scannerB, (int x, int y, int z) pointA, (int x, int y, int z) pointB)
        {
            foreach (string rotation in Config.ROTATIONS)
            {
                (int rotatedBPointX, int rotatedBPointY, int rotatedBPointZ) = RotatePoint(pointB, rotation);
                (int x, int y, int z) scannerCoordsRelToA = (pointA.x - rotatedBPointX, pointA.y - rotatedBPointY, pointA.z - rotatedBPointZ);

                List<(int x, int y, int z)> lstRotatedPoints = new(RotatePoints(scannerB.Beacons, rotation));
                List<(int x, int y, int z)> lstRotatedPointsRelToA = SumScannerCoords(lstRotatedPoints, scannerCoordsRelToA);

                var intersection = scannerA.Beacons.Intersect(lstRotatedPointsRelToA);
                if (intersection.Count() >= Config.MIN_OVERLAPPING_BEACONS - 1)
                {
                    HashSet<(int x, int y, int z)> hSetRotatedPoints = new(lstRotatedPoints);

                    foreach (Collision collision in scannerB.Collisions.Where(c => !c.TargetScanner.Merged))
                    {
                        collision.TargetScanner.Merged = true;
                        var newpoints = GetPointsRelativeToA(scannerB, collision.TargetScanner, collision.SourcePoint, collision.TargetPoint);
                        hSetRotatedPoints.UnionWith(RotatePoints(newpoints, rotation));
                    }

                    return SumScannerCoords(hSetRotatedPoints, scannerCoordsRelToA);
                }
            }

            throw new Exception($"Collision not found. {scannerA}-{scannerB}");
        }


        private static List<(int x, int y, int z)> SumScannerCoords(IEnumerable<(int x, int y, int z)> lstPoints, (int x, int y, int z) scannerCoords) => lstPoints.Select(p => (
                    scannerCoords.x + p.x,
                    scannerCoords.y + p.y,
                    scannerCoords.z + p.z
                )).ToList();


        private bool Intersect(Scanner scannerA, Scanner scannerB)
        {
            for (int i = 0; i < scannerA.Distances.Count; i++)
            {
                for (int j = 0; j < scannerB.Distances.Count; j++)
                {
                    var intersection = scannerA.Distances[i].Intersect(scannerB.Distances[j]);

                    if (intersection.Count() >= Config.MIN_OVERLAPPING_BEACONS - 1)
                    {
                        if (Debug)
                            Console.WriteLine($"\n({scannerA},{scannerB}) - Overlapping beacons:{intersection.Count()}");

                        scannerA.Collisions.Add(new Collision(scannerA, scannerA.Beacons[i], scannerB, scannerB.Beacons[j]));

                        return true;
                    }
                }
            }

            return false;
        }


        private static List<(int x, int y, int z)> RotatePoints(List<(int x, int y, int z)> points, string rotation)
        {
            List<(int x, int y, int z)> rotatedPoints = [];

            foreach (var point in points)
                rotatedPoints.Add(RotatePoint(point, rotation));

            return rotatedPoints;
        }


        private static (int x, int y, int z) RotatePoint((int x, int y, int z) point, string rotation)
        {
            string[] rotationParts = rotation.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries);

            return (GetCoord(point, rotationParts[0]), GetCoord(point, rotationParts[1]), GetCoord(point, rotationParts[2]));
        }

        private static int GetCoord((int x, int y, int z) point, string rotationCoord) => rotationCoord switch
        {
            "x" => point.x,
            "y" => point.y,
            "z" => point.z,
            "-x" => -point.x,
            "-y" => -point.y,
            "-z" => -point.z,
            _ => throw new ArgumentOutOfRangeException(nameof(rotationCoord), $"Not expected rotationCoord value: {rotationCoord}"),
        };

        private static List<Scanner> GetInput(string input)
        {
            string[] rows = input.GetLines();

            List<Scanner> scanners = [];
            List<(int x, int y, int z)> beacons = [];
            int scannerIndex = 0;

            foreach (string row in rows)
            {
                if (row.StartsWith("---"))
                {
                    beacons = [];
                }
                else
                {
                    string[] numbers = row.Split(',', StringSplitOptions.RemoveEmptyEntries);

                    if (numbers.Length != 0)
                    {
                        beacons.Add((int.Parse(numbers[0]), int.Parse(numbers[1]), int.Parse(numbers[2])));
                    }
                    else
                    {
                        scanners.Add(new Scanner(beacons, scannerIndex));
                        scannerIndex++;
                    }
                }
            }

            return scanners;
        }


        private static class Config
        {
            public const int MIN_OVERLAPPING_BEACONS = 12;

            public static readonly List<string> ROTATIONS = [
                "(x,y,z)",
                "(x,y,-z)",
                "(x,-y,z)",
                "(x,-y,-z)",
                "(x,z,y)",
                "(x,z,-y)",
                "(x,-z,y)",
                "(x,-z,-y)",
                "(-x,y,z)",
                "(-x,y,-z)",
                "(-x,-y,z)",
                "(-x,-y,-z)",
                "(-x,z,y)",
                "(-x,z,-y)",
                "(-x,-z,y)",
                "(-x,-z,-y)",
                "(y,x,z)",
                "(y,x,-z)",
                "(y,-x,z)",
                "(y,-x,-z)",
                "(y,z,x)",
                "(y,z,-x)",
                "(y,-z,x)",
                "(y,-z,-x)",
                "(-y,x,z)",
                "(-y,x,-z)",
                "(-y,-x,z)",
                "(-y,-x,-z)",
                "(-y,z,x)",
                "(-y,z,-x)",
                "(-y,-z,x)",
                "(-y,-z,-x)",
                "(z,x,y)",
                "(z,x,-y)",
                "(z,-x,y)",
                "(z,-x,-y)",
                "(z,y,x)",
                "(z,y,-x)",
                "(z,-y,x)",
                "(z,-y,-x)",
                "(-z,x,y)",
                "(-z,x,-y)",
                "(-z,-x,y)",
                "(-z,-x,-y)",
                "(-z,y,x)",
                "(-z,y,-x)",
                "(-z,-y,x)",
                "(-z,-y,-x)"
            ];
        }


        private class Scanner(List<(int x, int y, int z)> beacons, int id)
        {
            public int Id { get; set; } = id;

            public bool Merged { get; set; } = false;

            public List<(int x, int y, int z)> Beacons { get; set; } = beacons;

            public List<List<double>> Distances { get; set; } = GetDistances(beacons);

            public List<Collision> Collisions { get; set; } = [];

            private static List<List<double>> GetDistances(List<(int x, int y, int z)> points)
            {
                List<List<double>> distances = [];

                for (int i = 0; i < points.Count; i++)
                {
                    distances.Add([]);

                    for (int j = 0; j < points.Count; j++)
                    {
                        if (i == j)
                            continue;

                        distances[i].Add(GetDistance(points[j], points[i]));
                    }
                }

                return distances;
            }

            private static double GetDistance((int x, int y, int z) pointA, (int x, int y, int z) pointB)
            {
                // Distance between two points in a three dimensional space
                // d = ((x2 - x1)^2 + (y2 - y1)^2 + (z2 - z1)^2)^1/2
                return Math.Pow(Math.Pow(pointB.x - pointA.x, 2) + Math.Pow(pointB.y - pointA.y, 2) + Math.Pow(pointB.z - pointA.z, 2), 0.5);
            }


            public override string ToString() => $"Scanner{Id}";
        }

        private class Collision(Scanner sourceScanner, (int x, int y, int z) sourcePoint, Scanner targetScanner, (int x, int y, int z) targetPoint)
        {
            public Scanner TargetScanner { get; set; } = targetScanner;
            public Scanner SourceScanner { get; set; } = sourceScanner;
            public (int x, int y, int z) SourcePoint { get; set; } = sourcePoint;
            public (int x, int y, int z) TargetPoint { get; set; } = targetPoint;
        }
    }
}