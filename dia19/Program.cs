
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
        resultPoints.UnionWith(GetPointsRelativeToA(scanners[0], collision.TargetScanner, collision.SourcePoint, collision.TargetPoint, collision.Rotation));
    }

    Console.WriteLine("\nPart1: Number of beacons detected: " + resultPoints.Count);
}

void Part2()
{
}


List<(int x, int y, int z)> GetPointsRelativeToA(Scanner scannerA, Scanner scannerB, (int x, int y, int z) pointA, (int x, int y, int z) pointB, string rotation)
{
    (int x, int y, int z) rotatedBPoint = RotatePoint(pointB, rotation);
    (int x, int y, int z) scannerCoordsRelToA = (pointA.x - rotatedBPoint.x, pointA.y - rotatedBPoint.y, pointA.z - rotatedBPoint.z);

    HashSet<(int x, int y, int z)> rotatedPoints = new(RotatePoints(scannerB.Beacons, rotation));
    foreach (Collision collision in scannerB.Collisions.Where(c => !c.TargetScanner.Merged))
    {
        collision.TargetScanner.Merged = true;
        var newpoints = GetPointsRelativeToA(scannerB, collision.TargetScanner, collision.SourcePoint, collision.TargetPoint, collision.Rotation);
        rotatedPoints.UnionWith(RotatePoints(newpoints, rotation));
    }

    List<(int x, int y, int z)> lstRotatedPoints = rotatedPoints.ToList();

    for (int i = 0; i < rotatedPoints.Count; i++)
    {
        lstRotatedPoints[i] = (
            scannerCoordsRelToA.x + lstRotatedPoints[i].x,
            scannerCoordsRelToA.y + lstRotatedPoints[i].y,
            scannerCoordsRelToA.z + lstRotatedPoints[i].z
        );
    }

    var intersection = scannerA.Beacons.Intersect(lstRotatedPoints);
    if (intersection.Count() >= Config.MIN_OVERLAPPING_BEACONS - 1)
    {
        return lstRotatedPoints;
    }

    throw new Exception($"Collision not found. {scannerA}-{scannerB}, {rotation}");
}


bool Intersect(Scanner scannerA, Scanner scannerB)
{
    for (int i = 0; i < scannerA.InterDistances.Count; i++)
    {
        for (int j = 0; j < scannerB.InterDistances.Count; j++)
        {
            foreach (var rotation in Config.ROTATIONS)
            {
                var result = scannerA.InterDistances[i].Intersect(RotatePoints(scannerB.InterDistances[j], rotation));

                if (result.Count() >= Config.MIN_OVERLAPPING_BEACONS - 1)
                {
                    if (Config.debug)
                        Console.WriteLine($"\n({scannerA},{scannerB}) - Overlapping beacons:{result.Count()} Rotation:({rotation})");

                    scannerA.Collisions.Add(new Collision(scannerA, scannerA.Beacons[i], scannerB, scannerB.Beacons[j], rotation));

                    return true;
                }
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

    public static bool debug = true;
    public static string input = "input.txt"; // "input.txt" "inputTest.txt" "inputTest2.txt"
}


class Scanner
{
    public int Id { get; set; }

    public bool Merged { get; set; } = false;

    public List<(int x, int y, int z)> Beacons { get; set; } = new();

    public List<List<(int x, int y, int z)>> InterDistances { get; set; } = new();

    public List<Collision> Collisions { get; set; } = new();

    public Scanner(List<(int x, int y, int z)> beacons, int id)
    {
        Beacons = beacons;
        InterDistances = GetInterDistances(beacons);
        Id = id;
    }

    private List<List<(int x, int y, int z)>> GetInterDistances(List<(int x, int y, int z)> points)
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

    private (int x, int y, int z) GetPointsDiff((int x, int y, int z) pointA, (int x, int y, int z) pointB)
    {
        return (pointB.x - pointA.x, pointB.y - pointA.y, pointB.z - pointA.z);
    }

    public override string ToString() => $"Scanner{Id}";
}

class Collision
{
    public Scanner TargetScanner { get; set; }
    public Scanner SourceScanner { get; set; }
    public (int x, int y, int z) SourcePoint { get; set; }
    public (int x, int y, int z) TargetPoint { get; set; }
    public string Rotation { get; set; }

    public Collision(Scanner sourceScanner, (int x, int y, int z) sourcePoint, Scanner targetScanner, (int x, int y, int z) targetPoint, string rotation)
    {
        TargetScanner = targetScanner;
        SourceScanner = sourceScanner;
        SourcePoint = sourcePoint;
        TargetPoint = targetPoint;
        Rotation = rotation;
    }
}