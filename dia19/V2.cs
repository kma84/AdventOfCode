using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dia19
{
    internal class V2
    {
        private const int MIN_OVERLAPPING_BEACONS = 12; // 3 12

        private static bool debug = true;
        private static string input = "input.txt"; // "input.txt" "inputTest.txt" "inputTest2.txt"

        private static readonly List<(int xAxis, int yAxis, int zAxis)> rotations = new() {
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


        public static void Part1()
        {
            List<Scanner> scanners = GetInput(input);

            //int iOffset = 1;
            //for (int i = 0; i < scanners.Count; i++)
            //{
            //    for (int j = iOffset; j < scanners.Count; j++)
            //    {
            //        if (Intersect(scanners[i], scanners[j]))
            //            scanners[i].Collisions.Add(scanners[j].Id);
            //    }

            //    iOffset++;
            //}

            for (int i = 0; i < scanners.Count; i++)
            {
                for (int j = 0; j < scanners.Count; j++)
                {
                    if (i == j)
                        continue;

                    if (Intersect(scanners[i], scanners[j]))
                    {
                        scanners[i].Collisions.Add(scanners[j].Id);
                        //scanners[j].Collisions.Add(scanners[i].Id);
                    }
                }
            }
        }

        public static void Part2()
        {
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
                        scanners.Add(new Scanner(beacons, scannerIndex));
                        scannerIndex++;
                    }
                }
            }

            return scanners;
        }


        private static List<List<(int x, int y, int z)>> GetInterDistances(List<(int x, int y, int z)> points)
        {
            List<List<(int x, int y, int z)>> interDistances = new();

            for (int i = 0; i < points.Count; i++)
            {
                interDistances.Add(new List<(int x, int y, int z)>());

                for (int j = 0; j < points.Count; j++)
                {
                    if (i == j)
                        continue;

                    interDistances[i].Add(GetPointsDiff(points[j], points[i]));
                }
            }

            return interDistances;
        }

        private static (int x, int y, int z) GetPointsDiff((int x, int y, int z) pointA, (int x, int y, int z) pointB)
        {
            return (pointB.x - pointA.x, pointB.y - pointA.y, pointB.z - pointA.z);
        }

        private static bool Intersect(Scanner scannerA, Scanner scannerB)
        {
            for (int i = 0; i < scannerA.InterDistances.Count; i++)
            {
                for (int j = 0; j < scannerB.InterDistances.Count; j++)
                {
                    foreach (var rotation in rotations)
                    {
                        var result = scannerA.InterDistances[i].Intersect(RotatePoints(scannerB.InterDistances[j], rotation));

                        if (result.Count() >= MIN_OVERLAPPING_BEACONS - 1)
                        {
                            if (debug)
                                Console.WriteLine($"\n({scannerA},{scannerB}) - Overlapping beacons:{result.Count()} Rotation:({rotation.xAxis},{rotation.yAxis},{rotation.zAxis})");

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static List<(int x, int y, int z)> RotatePoints(List<(int x, int y, int z)> points, (int xAxis, int yAxis, int zAxis) rotation)
        {
            List<(int x, int y, int z)> rotatedPoints = new();

            foreach (var point in points)
                rotatedPoints.Add(RotatePoint(point, rotation));

            return rotatedPoints;
        }

        private static (int x, int y, int z) RotatePoint((int x, int y, int z) point, (int xAxis, int yAxis, int zAxis) rotation)
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


        private class Scanner
        {
            public int Id { get; set; }

            public List<(int x, int y, int z)> Beacons { get; set; } = new();

            public List<List<(int x, int y, int z)>> InterDistances { get; set; } = new();

            public List<int> Collisions { get; set; } = new();

            public Scanner(List<(int x, int y, int z)> beacons, int id)
            {
                Beacons = beacons;
                InterDistances = GetInterDistances(beacons);
                Id = id;
            }

            public override string ToString() => $"Scanner{Id}";
        }

    }
}
