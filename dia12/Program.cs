
Cave startCave = GetInput("input.txt");

int numOfVisitsFirstSmallCave = 1;
List<string> paths = new();

GetPaths(startCave, new List<Cave>(), paths);

Console.WriteLine(string.Join('\n', paths));
Console.WriteLine($"Puzzle1: Hay un total de {paths.Count} caminos únicos.");


paths = new();
numOfVisitsFirstSmallCave++;

GetPaths(startCave, new List<Cave>(), paths);

Console.WriteLine();
Console.WriteLine($"Puzzle2: Hay un total de {paths.Count} caminos únicos.");



void GetPaths(Cave cave, List<Cave> currentPath, List<string> paths)
{
    if (cave.IsEndCave())
    {
        currentPath.Add(cave);
        paths.Add(string.Join(',', currentPath.Select(c => c.Name)));
        return;
    }

    if (cave.IsSmallCave() && !CanVisitSmallCave(cave, currentPath))
    {
        return;
    }

    currentPath.Add(cave);

    foreach (Cave adjacentCave in cave.Connections)
    {
        GetPaths(adjacentCave, new List<Cave>(currentPath), paths);
    }

    return;
}


bool CanVisitSmallCave(Cave smallCave, List<Cave> currentPath)
{
    if (!currentPath.Contains(smallCave))
        return true;

    if (smallCave.IsStartCave())
        return false;

    return !currentPath.Where(c => c.IsSmallCave())
                       .GroupBy(c => c)
                       .Any(g => g.Count() >= numOfVisitsFirstSmallCave);        
}


Cave GetInput(string filename)
{
    string input =
        File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "inputs" + Path.DirectorySeparatorChar + filename);

    Dictionary<string, Cave> caves = new ();

    foreach (string line in input.Split('\n', StringSplitOptions.RemoveEmptyEntries))
    {
        string[] cavesStr = line.Split('-', StringSplitOptions.RemoveEmptyEntries);
        string caveAStr = cavesStr[0];
        string caveBStr = cavesStr[1];

        if (!caves.ContainsKey(caveAStr))
            caves.Add(caveAStr, new Cave { Name = caveAStr });

        if (!caves.ContainsKey(caveBStr))
            caves.Add(caveBStr, new Cave { Name = caveBStr });

        caves[caveAStr].Connections.Add(caves[caveBStr]);
        caves[caveBStr].Connections.Add(caves[caveAStr]);
    }

    return caves.Values.Single(c => c.IsStartCave());
}


class Cave
{
    public string Name { get; set; } = string.Empty;

    public List<Cave> Connections { get; set; } = new ();

    public bool IsSmallCave()
    {
        return Name.All(c => char.IsLower(c));
    }

    public bool IsStartCave()
    {
        return Name == "start";
    }

    public bool IsEndCave()
    {
        return Name == "end";
    }
}