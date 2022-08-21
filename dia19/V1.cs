using AoCUtils;
using System.Text;

namespace dia19
{
    internal class V1
    {

        public static void Part1()
        {
            //To show all 24 different rotations
            //Rotation3D.Print90DegreesRotations(1, 2, 3);

            List<Scanner> scanners = GetInput(Config.input);

            if (Config.input2D)
            {
                for (int i = 0; i < scanners.Count; i++)
                    Print2D(scanners[i].Beacons, new List<(int x, int y, int z)> { (0, 0, 0) }, $"Scanner{i}");
            }

            Map map = new(scanners[0]);
            Queue<Scanner> scannersToCheck = new(scanners.Except(map.Scanners.Values));

            while (scannersToCheck.TryDequeue(out Scanner? scanner))
            {
                if (map.Intersect(scanner))
                    scannersToCheck = new(scanners.Except(map.Scanners.Values));
            }

            if (Config.input2D)
                Print2D(map.Points, map.Scanners.Keys.ToList(), "Intersect(0,1)");

            if (Config.debug)
            {
                Console.WriteLine("\nMap points:");

                foreach ((int x, int y, int z) in map.Points)
                    Console.WriteLine($"{x},{y},{z}");
            }

            Console.WriteLine("\nPart1: Number of beacons detected: " + map.Points.Count);
        }

        public static void Part2()
        {
        }


        private static void Print2D(List<(int x, int y, int z)> beaconPoints, List<(int x, int y, int z)> scannerPoints, string title)
        {
            int offsetX = -Math.Min(0, beaconPoints.Min(b => b.x));
            int offsetY = -Math.Min(0, beaconPoints.Min(b => b.y));

            int lengthY = Math.Max(0, beaconPoints.Union(scannerPoints).Max(b => b.y)) + offsetY + 1;
            int lengthX = Math.Max(0, beaconPoints.Union(scannerPoints).Max(b => b.x)) + offsetX + 1;
            char[,] map = new char[lengthY, lengthX];
            map.Fill('.');

            foreach (var (x, y, _) in scannerPoints)
            {
                map[y + offsetY, x + offsetX] = 'S';
            }

            foreach (var (x, y, _) in beaconPoints)
            {
                map[y + offsetY, x + offsetX] = 'B';
            }

            StringBuilder sb = new();

            for (int y = map.GetLength(0) - 1; y >= 0; y--)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    sb.Append(map[y, x]);
                }

                sb.AppendLine();
            }

            Console.WriteLine(title);
            Console.WriteLine(sb.ToString());
        }


        private static List<Scanner> GetInput(string filename)
        {
            string path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "inputs" + Path.DirectorySeparatorChar + filename;
            string input = File.ReadAllText(path);

            string[] rows = input.Split('\n');

            List<Scanner> scanners = new();
            List<(int x, int y, int z)> beacons = new();
            int scannerIndex = 0;

            foreach (string row in rows)
            {
                if (row.StartsWith("---"))
                {
                    beacons = new();
                }
                else
                {
                    string[] numbers = row.Split(',', StringSplitOptions.RemoveEmptyEntries);

                    if (numbers.Any())
                    {
                        beacons.Add((int.Parse(numbers[0]), int.Parse(numbers[1]), int.Parse(numbers[2])));
                    }
                    else
                    {
                        scanners.Add(new Scanner(beacons, $"Scanner{scannerIndex}"));
                        scannerIndex++;
                    }
                }
            }

            return scanners;
        }


        static class Config
        {
            public const int MIN_OVERLAPPING_BEACONS = 12; // 3 12

            public static bool debug = true;
            public static bool input2D = false;
            public static string input = "inputTest2.txt"; // "input.txt" "inputTest.txt" "inputTest2.txt"
        }


        private class Map
        {
            static readonly List<(int xAxis, int yAxis, int zAxis)> rotations = new() {
                (0, 0, 0),
                (0, 0, 90),
                (0, 0, 180),
                (0, 0, 270),
                (0, 90, 0),
                (0, 90, 90),
                (0, 90, 180),
                (0, 90, 270),
                (0, 180, 0),
                (0, 180, 90),
                (0, 180, 180),
                (0, 180, 270),
                (0, 270, 0),
                (0, 270, 90),
                (0, 270, 180),
                (0, 270, 270),
                (90, 0, 0),
                (90, 0, 90),
                (90, 0, 180),
                (90, 0, 270),
                (90, 270, 0),
                (90, 270, 90),
                (90, 270, 180),
                (90, 270, 270)
            };

            public SortedList<(int x, int y, int z), Scanner> Scanners { get; set; } = new();

            /// <summary>
            /// Points relative to scanner 0
            /// </summary>
            public List<(int x, int y, int z)> Points { get; set; } = new();

            public List<List<(int x, int y, int z)>> InterDistances { get; set; } = new();


            public Map(Scanner scanner0)
            {
                Scanners.Add((0, 0, 0), scanner0);
                Points.AddRange(scanner0.Beacons);
                InterDistances.AddRange(scanner0.InterDistances);
            }


            public bool Intersect(Scanner scanner)
            {
                for (int i = 0; i < this.InterDistances.Count; i++)
                {
                    for (int j = 0; j < scanner.InterDistances.Count; j++)
                    {
                        foreach (var rotation in rotations)
                        {
                            var result = this.InterDistances[i].Intersect(RotatePoints(scanner.InterDistances[j], rotation));

                            if (result.Count() >= Config.MIN_OVERLAPPING_BEACONS - 1)
                            {
                                if (Config.debug)
                                    Console.WriteLine($"\n{scanner} - Overlapping beacons:{result.Count()} Rotation:({rotation.xAxis},{rotation.yAxis},{rotation.zAxis})");

                                (int x, int y, int z) rotatedPoint = RotatePoint(scanner.Beacons[j], rotation);
                                (int x, int y, int z) scannerCoordsRelTo0 = (Points[i].x - rotatedPoint.x, Points[i].y - rotatedPoint.y, Points[i].z - rotatedPoint.z);
                                this.Scanners.Add(scannerCoordsRelTo0, scanner);

                                for (int bIndex = 0; bIndex < scanner.Beacons.Count; bIndex++)
                                {
                                    (int x, int y, int z) = RotatePoint(scanner.Beacons[bIndex], rotation);
                                    var newPoint = (scannerCoordsRelTo0.x + x, scannerCoordsRelTo0.y + y, scannerCoordsRelTo0.z + z);

                                    if (!Points.Contains(newPoint))
                                    {
                                        Points.Add(newPoint);
                                        InterDistances.Add(RotatePoints(scanner.InterDistances[bIndex], rotation));
                                    }
                                    else if (Config.debug)
                                    {
                                        Console.WriteLine($"({newPoint.Item1},{newPoint.Item2},{newPoint.Item3})");
                                    }
                                }

                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            static List<(int x, int y, int z)> RotatePoints(List<(int x, int y, int z)> points, (int xAxis, int yAxis, int zAxis) rotation)
            {
                List<(int x, int y, int z)> rotatedPoints = new();

                foreach (var point in points)
                    rotatedPoints.Add(RotatePoint(point, rotation));

                return rotatedPoints;
            }

            static (int x, int y, int z) RotatePoint((int x, int y, int z) point, (int xAxis, int yAxis, int zAxis) rotation)
            {
                static double GetRadians(double degrees) => degrees * Math.PI / 180;

                int newX, newY, newZ;
                double radians;

                radians = GetRadians(rotation.xAxis);
                newX = (int)Math.Round(point.x * Math.Cos(radians) - point.y * Math.Sin(radians));
                newY = (int)Math.Round(point.x * Math.Sin(radians) + point.y * Math.Cos(radians));
                point.x = newX;
                point.y = newY;

                radians = GetRadians(rotation.yAxis);
                newY = (int)Math.Round(point.y * Math.Cos(radians) - point.z * Math.Sin(radians));
                newZ = (int)Math.Round(point.y * Math.Sin(radians) + point.z * Math.Cos(radians));
                point.z = newZ;
                point.y = newY;

                radians = GetRadians(rotation.zAxis);
                newZ = (int)Math.Round(point.z * Math.Cos(radians) - point.x * Math.Sin(radians));
                newX = (int)Math.Round(point.z * Math.Sin(radians) + point.x * Math.Cos(radians));

                return (newX, newY, newZ);
            }
        }


        private class Scanner
        {
            public string Name { get; set; }
            public List<(int x, int y, int z)> Beacons { get; set; } = new();

            public List<List<(int x, int y, int z)>> InterDistances { get; set; } = new();

            public Scanner(List<(int x, int y, int z)> beacons, string name)
            {
                Beacons = beacons;
                InterDistances = GetInterDistances(beacons);
                Name = name;
            }

            static List<List<(int x, int y, int z)>> GetInterDistances(List<(int x, int y, int z)> beacons)
            {
                List<List<(int x, int y, int z)>> interDistances = new();

                for (int i = 0; i < beacons.Count; i++)
                {
                    interDistances.Add(new List<(int x, int y, int z)>());

                    for (int j = 0; j < beacons.Count; j++)
                    {
                        if (i == j)
                            continue;

                        interDistances[i].Add(GetPointsDiff(beacons[j], beacons[i]));
                    }
                }

                return interDistances;
            }

            static (int x, int y, int z) GetPointsDiff((int x, int y, int z) pointA, (int x, int y, int z) pointB)
            {
                return (pointB.x - pointA.x, pointB.y - pointA.y, pointB.z - pointA.z);
            }

            public override string ToString() => Name;
        }

    }
}
