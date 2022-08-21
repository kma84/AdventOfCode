using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dia19
{
    internal class V3
    {
        private const int MIN_OVERLAPPING_BEACONS = 12; // 3 12

        private static readonly List<(int xAxis, int yAxis, int zAxis)> ROTATIONS = new() {
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

        private static bool debug = true;
        private static string input = "input.txt"; // "input.txt" "inputTest.txt" "inputTest2.txt"



        public static void Part1()
        {
            List<Scanner> scanners = GetInput(input);

            int iOffset = 1;
            for (int i = 0; i < scanners.Count; i++)
            {
                for (int j = iOffset; j < scanners.Count; j++)
                {
                    if (Intersect(scanners[i], scanners[j]))
                    {
                        //scanners[i].Collisions.Add(scanners[j].Id);
                        //scanners[j].Collisions.Add(scanners[i].Id);
                    }
                }

                iOffset++;
            }

            GetPointsRelativeToA(scanners[0], scanners[scanners[0].Collisions[0].scannerId]);
            GetPointsRelativeToA(scanners[0], scanners[scanners[0].Collisions[2].scannerId]);
            //GetPointsRelativeToA(scanners[0], scanners[2]);

            //for (int i = 0; i < scanners.Count; i++)
            //{
            //    for (int j = 0; j < scanners.Count; j++)
            //    {
            //        if (i == j)
            //            continue;

            //        if (Intersect(scanners[i], scanners[j]))
            //        {
            //            scanners[i].Collisions.Add(scanners[j].Id);
            //            //scanners[j].Collisions.Add(scanners[i].Id);
            //        }
            //    }
            //}
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


        private static bool Intersect(Scanner scannerA, Scanner scannerB)
        {
            for (int i = 0; i < scannerA.Distances.Count; i++)
            {
                for (int j = 0; j < scannerB.Distances.Count; j++)
                {
                    var intersection = scannerA.Distances[i].Intersect(scannerB.Distances[j]);

                    if (intersection.Count() >= MIN_OVERLAPPING_BEACONS - 1)
                    {
                        if (debug)
                            Console.WriteLine($"\n({scannerA},{scannerB}) - Overlapping beacons:{intersection.Count()}");

                        scannerA.Collisions.Add((scannerB.Id, i, j));
                        scannerB.Collisions.Add((scannerA.Id, j, i));

                        return true;
                    }
                }
            }

            return false;
        }


        private static List<(int x, int y, int z)> GetPointsRelativeToA(Scanner scannerA, Scanner scannerB)
        {
            var collision = scannerA.Collisions.First(c => c.scannerId == scannerB.Id);

            //(int x, int y, int z) scannerCoordsRelTo0 = (Points[i].x - rotatedPoint.x, Points[i].y - rotatedPoint.y, Points[i].z - rotatedPoint.z);

            foreach (var rotation in ROTATIONS)
            {
                //(int x, int y, int z) sourcePoint = scannerA.Beacons[collision.iSourcePoint];
                //(int x, int y, int z) rotatedPoint = RotatePoint(scannerB.Beacons[collision.iTargetPoint], rotation);
                //(int x, int y, int z) scannerCoordsRelTo0 = (sourcePoint.x - rotatedPoint.x, sourcePoint.y - rotatedPoint.y, sourcePoint.z - rotatedPoint.z);

                //var newPoint = (scannerCoordsRelTo0.x + rotatedPoint.x, scannerCoordsRelTo0.y + rotatedPoint.y, scannerCoordsRelTo0.z + rotatedPoint.z);

                //if (newPoint == sourcePoint)
                //{

                //}

                (int x, int y, int z) sourcePoint = scannerA.Beacons[collision.iSourcePoint];
                List<(int x, int y, int z)> rotatedPoints = RotatePoints(scannerB.Beacons, rotation);
                (int x, int y, int z) scannerCoordsRelTo0 = (sourcePoint.x - rotatedPoints[0].x, sourcePoint.y - rotatedPoints[0].y, sourcePoint.z - rotatedPoints[0].z);

                for (int i = 0; i < rotatedPoints.Count; i++)
                {
                    rotatedPoints[i] = (scannerCoordsRelTo0.x + rotatedPoints[i].x, scannerCoordsRelTo0.y + rotatedPoints[i].y, scannerCoordsRelTo0.z + rotatedPoints[i].z);
                }

                var intersection = scannerA.Beacons.Intersect(rotatedPoints);
                if (intersection.Count() >= MIN_OVERLAPPING_BEACONS - 1)
                {
                }

                //var scannerBRotatedPoints = RotatePoints(scannerB.Beacons, rotation);
                //var intersection = scannerA.Beacons.Intersect(scannerBRotatedPoints);

                //if (intersection.Count() >= MIN_OVERLAPPING_BEACONS - 1)
                //{
                //    if (debug)
                //        Console.WriteLine($"\n({scannerA},{scannerB}) - Overlapping:{intersection.Count()} Rotation:({rotation.xAxis},{rotation.yAxis},{rotation.zAxis})");

                //    List<(int x, int y, int z)> result = new(scannerA.Beacons);
                //    result.AddRange(scannerBRotatedPoints.Except(intersection));

                //    return result;
                //}
            }

            return new List<(int x, int y, int z)>();
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


        private static List<List<double>> GetDistances(List<(int x, int y, int z)> points)
        {
            List<List<double>> distances = new();

            for (int i = 0; i < points.Count; i++)
            {
                distances.Add(new List<double>());

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


        private class Scanner
        {
            public int Id { get; set; }

            public List<(int x, int y, int z)> Beacons { get; set; } = new();

            public List<List<double>> Distances { get; set; } = new();

            public List<(int scannerId, int iSourcePoint, int iTargetPoint)> Collisions { get; set; } = new();

            public Scanner(List<(int x, int y, int z)> beacons, int id)
            {
                Beacons = beacons;
                Distances = GetDistances(beacons);
                Id = id;
            }

            public override string ToString() => $"Scanner{Id}";
        }
    }
}
