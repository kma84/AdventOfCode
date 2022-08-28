
using AdventOfCode.Generator.Properties;


ArgsHelper argsHelper = ArgsHelper.CreateFromArgs(args);

if (!argsHelper.Validated)
{
    // TODO Show usage
    Console.WriteLine("Show usage not implemented.");
    return;
}

GenerateCode(argsHelper.Years, argsHelper.Days);


void GenerateCode(List<int> years, List<int> days)
{
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

                // TODO Modificar .csproj para incluir input.txt y debugInput.txt en el resultado de la compilación

                Console.WriteLine($"Files generated for year {year} day {day}");
            }
        }
    }
}

string GetDayPath(int year, int day)
{
    char dirSeparator = Path.DirectorySeparatorChar;
    string appDir = AppDomain.CurrentDomain.BaseDirectory;

    return $"{appDir}Year{year}{dirSeparator}Day{day:D2}{dirSeparator}";
}


class ArgsHelper
{
    public List<int> Years { get; set; } = new();
    public List<int> Days { get; set; } = new();
    public bool Validated { get; set; }

    public static ArgsHelper CreateFromArgs(string[] args)
    {
        static List<int> GetDays(string? arg)
        {
            if (arg != null)
                return int.TryParse(arg, out int argDay) ? new List<int> { argDay } : new List<int>();

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
