
using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;


var argsHelper = ArgsHelper.CreateFromArgs(args);

if (!argsHelper.Validated)
{
    // TODO Show usage
    Console.WriteLine("Show usage not implemented.");
    return;
}

if (argsHelper.IsGenerationCodeRequest)
{
    // TODO Generate code
    Console.WriteLine("Generation of code not implemented.");
    return;
}

RunProblems(argsHelper.Years, argsHelper.Days);


void RunProblems(List<int> years, List<int> days)
{
    var iProblemType = typeof(IProblem);
    var problemTypes = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(a => a.GetTypes())
        .Where(t => iProblemType.IsAssignableFrom(t) && t.IsClass);

    Dictionary<ProblemAttribute, Type> dictProblems = problemTypes.ToDictionary(t => (ProblemAttribute)t.GetCustomAttributes(typeof(ProblemAttribute), true).First());

    foreach (int year in years)
    {
        foreach (int day in days)
        {
            var problemMeta = dictProblems.Keys.FirstOrDefault(pa => pa.Year == year && pa.Day == day);

            if (problemMeta != null)
            {
                IProblem? problem = (IProblem?)Activator.CreateInstance(dictProblems[problemMeta]);
                string input = File.ReadAllText(GetInputPath(year, day));

                Console.WriteLine($"Year {year}, Day {day}, Problem: {problemMeta.ProblemName}");
                Console.WriteLine($"Part1: {problem?.Part1(input)}");
                Console.WriteLine($"Part1: {problem?.Part2(input)}");
            }

        }
    }
}

string GetInputPath(int year, int day)
{
    char dirSeparator = Path.DirectorySeparatorChar;
    string appDir = AppDomain.CurrentDomain.BaseDirectory;

    return $"{appDir}{dirSeparator}Year{year}{dirSeparator}Day{day:D2}{dirSeparator}input.txt";
}


class ArgsHelper
{
    private const string GENERATE_ARG = "generate";

    public List<int> Years { get; set; } = new();
    public List<int> Days { get; set; } = new();
    public bool IsGenerationCodeRequest { get; set; } = false;
    public bool Validated { get; set; }


    // TODO Use regex to match accepted combinations of args
    public static ArgsHelper CreateFromArgs(string[] args)
    {
        static List<int> GetDays(string? arg)
        {
            if (arg != null)
                return int.TryParse(arg, out int argDay) ? new List<int> { argDay } : new List<int>();

            return Enumerable.Range(1, 25).ToList();
        }

        ArgsHelper result = new() { Validated = false };

        if (args.Length < 1 || args.Length > 3)
            return result;

        string arg0 = args[0];
        string? arg1 = args.ElementAtOrDefault(1);
        string? arg2 = args.ElementAtOrDefault(2);

        // Generate code request
        if (arg0 == GENERATE_ARG && int.TryParse(arg1, out int year))
        {
            var days = GetDays(arg2);

            if (days.Any())
            {
                result.Years = new() { year };
                result.Days = days;
                result.IsGenerationCodeRequest = true;
                result.Validated = true;
                return result;
            }
        }
        // Problems execution request
        else if (int.TryParse(arg0, out year) && args.Length < 3)
        {
            var days = GetDays(arg1);

            if (days.Any())
            {
                result.Years = new() { year };
                result.Days = days;
                result.Validated = true;
                return result;
            }
        }

        // Not validated result
        return result;
    }
}