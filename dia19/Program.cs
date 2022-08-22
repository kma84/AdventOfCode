
Part1();

Part2();


void Part1()
{
    List<Scanner> scanners = GetInput(Config.input);

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

    Console.WriteLine("\nPart1: Number of beacons detected: " + resultPoints.Count);
}

void Part2()
{
}


List<(int x, int y, int z)> GetPointsRelativeToA(Scanner scannerA, Scanner scannerB, (int x, int y, int z) pointA, (int x, int y, int z) pointB)
{

    foreach (string rotation in Config.ROTATIONS)
    {
        (int x, int y, int z) rotatedBPoint = RotatePoint(pointB, rotation);
        (int x, int y, int z) scannerCoordsRelToA = (pointA.x - rotatedBPoint.x, pointA.y - rotatedBPoint.y, pointA.z - rotatedBPoint.z);

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


List<(int x, int y, int z)> SumScannerCoords(IEnumerable<(int x, int y, int z)> lstPoints, (int x, int y, int z) scannerCoords) => lstPoints.Select(p => (
            scannerCoords.x + p.x,
            scannerCoords.y + p.y,
            scannerCoords.z + p.z
        )).ToList();


bool Intersect(Scanner scannerA, Scanner scannerB)
{
    for (int i = 0; i < scannerA.Distances.Count; i++)
    {
        for (int j = 0; j < scannerB.Distances.Count; j++)
        {
            var intersection = scannerA.Distances[i].Intersect(scannerB.Distances[j]);

            if (intersection.Count() >= Config.MIN_OVERLAPPING_BEACONS - 1)
            {
                if (Config.debug)
                    Console.WriteLine($"\n({scannerA},{scannerB}) - Overlapping beacons:{intersection.Count()}");

                scannerA.Collisions.Add(new Collision(scannerA, scannerA.Beacons[i], scannerB, scannerB.Beacons[j]));

                return true;
            }
        }
    }

    return false;
}


List<(int x, int y, int z)> RotatePoints(List<(int x, int y, int z)> points, string rotation)
{
    List<(int x, int y, int z)> rotatedPoints = new();

    foreach (var point in points)
        rotatedPoints.Add(RotatePoint(point, rotation));

    return rotatedPoints;
}


(int x, int y, int z) RotatePoint((int x, int y, int z) point, string rotation)
{
    string[] rotationParts = rotation.Split(new char[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries);

    return (GetCoord(point, rotationParts[0]), GetCoord(point, rotationParts[1]), GetCoord(point, rotationParts[2]));
}

int GetCoord((int x, int y, int z) point, string rotationCoord) => rotationCoord switch
{
    "x" => point.x,
    "y" => point.y,
    "z" => point.z,
    "-x" => -point.x,
    "-y" => -point.y,
    "-z" => -point.z,
    _ => throw new ArgumentOutOfRangeException(nameof(rotationCoord), $"Not expected rotationCoord value: {rotationCoord}"),
};

List<Scanner> GetInput(string filename)
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


static class Config
{
    public const int MIN_OVERLAPPING_BEACONS = 12; // 3 12

    public static readonly List<string> ROTATIONS = new() {
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
    };

    public static bool debug = false;
    public static string input = "input.txt"; // "input.txt" "inputTest.txt" "inputTest2.txt"
}


class Scanner
{
    public int Id { get; set; }

    public bool Merged { get; set; } = false;

    public List<(int x, int y, int z)> Beacons { get; set; } = new();

    public List<List<double>> Distances { get; set; } = new();

    public List<Collision> Collisions { get; set; } = new();

    public Scanner(List<(int x, int y, int z)> beacons, int id)
    {
        Beacons = beacons;
        Distances = GetDistances(beacons);
        Id = id;
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


    public override string ToString() => $"Scanner{Id}";
}

class Collision
{
    public Scanner TargetScanner { get; set; }
    public Scanner SourceScanner { get; set; }
    public (int x, int y, int z) SourcePoint { get; set; }
    public (int x, int y, int z) TargetPoint { get; set; }

    public Collision(Scanner sourceScanner, (int x, int y, int z) sourcePoint, Scanner targetScanner, (int x, int y, int z) targetPoint)
    {
        TargetScanner = targetScanner;
        SourceScanner = sourceScanner;
        SourcePoint = sourcePoint;
        TargetPoint = targetPoint;
    }
}