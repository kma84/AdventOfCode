
using AdventOfCode.Generator.Properties;
using System.Xml.Linq;

ArgsHelper argsHelper = ArgsHelper.CreateFromArgs(args);

if (!argsHelper.Validated)
{
    // TODO Show usage
    Console.WriteLine("Show usage not implemented.");
    return;
}

if (!File.Exists(GetAppDir() + "AdventOfCode.csproj"))
{
    Console.WriteLine("File AdventOfCode.csproj not found.");
    return;
}

GenerateFiles(argsHelper.Years, argsHelper.Days);


void GenerateFiles(List<int> years, List<int> days)
{
    XDocument xdoc = XDocument.Load(GetCsprojPath());

    foreach (int year in years)
    {
        foreach (int day in days)
        {
            string dayPath = GetDayPath(year, day);

            if (!Directory.Exists(dayPath))
            {
                Directory.CreateDirectory(dayPath);

                File.WriteAllText(dayPath + "Problem.cs", string.Format(Resources.ProblemTemplate, year, day));
                File.Create(dayPath + "input.txt");
                File.Create(dayPath + "debugInput.txt");
                File.Create(dayPath + "solutions.txt");

                Console.WriteLine($"Files generated for year {year} day {day}");
            }
        }
    }

    xdoc.Save(GetCsprojPath());
}

string GetAppDir() => AppDomain.CurrentDomain.BaseDirectory;

string GetCsprojPath() => GetAppDir() + "AdventOfCode.csproj";

string GetDayPath(int year, int day)
{
    char dirSeparator = Path.DirectorySeparatorChar;

    return $"{GetAppDir()}Year{year}{dirSeparator}Day{day:D2}{dirSeparator}";
}


class ArgsHelper
{
    public List<int> Years { get; set; } = [];
    public List<int> Days { get; set; } = [];
    public bool Validated { get; set; }

    public static ArgsHelper CreateFromArgs(string[] args)
    {
        static List<int> GetDays(string? arg)
        {
            if (arg != null)
                return int.TryParse(arg, out int argDay) ? [argDay] : [];

            return Enumerable.Range(1, 25).ToList();
        }

        ArgsHelper result = new() { Validated = false };

        if (args.Length < 1 || args.Length > 2)
            return result;

        string arg0 = args[0];
        string? arg1 = args.ElementAtOrDefault(1);

        if (int.TryParse(arg0, out int year))
        {
            var days = GetDays(arg1);

            if (days.Count != 0)
            {
                result.Years = [year];
                result.Days = days;
                result.Validated = true;
                return result;
            }
        }
        
        // Not validated result
        return result;
    }
}
