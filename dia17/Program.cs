
using AoCUtils;

bool debug = false;
string input = debug ? "inputTest.txt" : "input.txt";

Part1();

Part2();


void Part1()
{
    var targetCoords = GetInput(input);

    //CalculateTrajectory(7, 2, targetCoords);
    //CalculateTrajectory(6, 3, targetCoords);
    //CalculateTrajectory(9, 0, targetCoords);
    //CalculateTrajectory(17, -4, targetCoords);
    //CalculateTrajectory(6, 9, targetCoords);
    //CalculateTrajectory(6, 10, targetCoords);
    //CalculateTrajectory(7, 9, targetCoords);

    int maxY = 0;

    for (int y = 0; y < Math.Abs(targetCoords.y.Item1); y++)
    {
        for (int x = 0; x < targetCoords.x.Item2; x++)
        {
            int? maxTrajectoryY = CalculateTrajectory(x, y, targetCoords);
            maxY = Math.Max(maxY, maxTrajectoryY ?? 0);
        }
    }

    Console.WriteLine("Part 1: Highest y position: " + maxY);
}


void Part2()
{
    var targetCoords = GetInput(input);

    int i = 0;

    for (int y = targetCoords.y.Item1; y <= Math.Abs(targetCoords.y.Item1); y++)
    {
        for (int x = 0; x <= targetCoords.x.Item2; x++)
        {
            if (CalculateTrajectory(x, y, targetCoords).HasValue)
            {
                i++;
            }
        }
    }

    Console.WriteLine("Part 2: Initial velocity values that reach the target: " + i);
}



int? CalculateTrajectory(int startVelX, int startVelY, ((int, int) x, (int, int) y) targetCoords)
{
    List<(int x, int y)> points = new();
    int x = 0;
    int y = 0;
    int velX = startVelX;
    int velY = startVelY;
    int? maxY = null;

    bool targetMissed(int x, int y) => x > targetCoords.x.Item2 || y < targetCoords.y.Item1;
    
    bool targetReached(int x, int y)
    {
        bool targetReached = x >= targetCoords.x.Item1 && x <= targetCoords.x.Item2 && y >= targetCoords.y.Item1 && y <= targetCoords.y.Item2;

        if (targetReached)
            maxY = points.Max(p => p.y);

        return targetReached;
    }

    while (!targetMissed(x, y) && !targetReached(x, y))
    {
        (x, y, velX, velY) = Step(x, y, velX, velY);
        points.Add((x, y));
    }

    if (debug)
        PrintMap(points, targetCoords, $"({startVelX},{startVelY})");

    return maxY;
}


void PrintMap(List<(int x, int y)> points, ((int, int) x, (int, int) y) targetCoords, string title)
{
    int lengthX = Math.Max(points.Max(p => p.x), targetCoords.x.Item2) + 1;
    int offsetY = Math.Max(points.Max(p => p.y), 0);
    int lengthY = Math.Abs(Math.Min(points.Min(p => p.y), targetCoords.y.Item1)) + offsetY + 1;

    char[,] map = new char[lengthY, lengthX];
    
    map.Fill('.');
    map[offsetY, 0] = 'S';

    for (int y = Math.Abs(targetCoords.y.Item2) + offsetY; y <= Math.Abs(targetCoords.y.Item1) + offsetY; y++)
    {
        for (int x = targetCoords.x.Item1; x <= targetCoords.x.Item2; x++)
        {
            map[y, x] = 'T';
        }
    }

    foreach ((int x, int y) in points)
    {
        map[Math.Abs(y - offsetY), x] = '#';
    }

    map.Print(title);
}


(int x, int y, int velX, int velY) Step(int x, int y, int velX, int velY)
{
    x += velX;
    y += velY;

    if (velX > 0)
        velX--;
    else if (velX < 0)
        velX++;

    velY--;

    return (x, y, velX, velY);
}


((int, int) x, (int, int) y) GetInput(string filename)
{
    string input =
    File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "inputs" + Path.DirectorySeparatorChar + filename);

    string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    string[] partsX = parts[2].Split(new char[] { '.', '=', ',' }, StringSplitOptions.RemoveEmptyEntries);
    string[] partsY = parts[3].Split(new char[] { '.', '=' }, StringSplitOptions.RemoveEmptyEntries);

    return ((int.Parse(partsX[1]), int.Parse(partsX[2])), (int.Parse(partsY[1]), int.Parse(partsY[2])));
}
