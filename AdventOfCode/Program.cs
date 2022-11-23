
using AdventOfCode.Core;


var argsHelper = ArgsHelper.CreateFromArgs(args);

if (!argsHelper.Validated)
{
    // TODO Show usage
    Console.WriteLine("Show usage not implemented.");
    return;
}

Runner.RunProblems(argsHelper.Years, argsHelper.Days);


class ArgsHelper
{
    public List<int> Years { get; set; } = new();
    public List<int> Days { get; set; } = new();
    public bool Validated { get; set; }

    public static ArgsHelper CreateFromArgs(string[] args)
    {
        ArgsHelper result = new() { Validated = false };

        if (args.Length > 2)
            return result;

        string? arg0 = args.ElementAtOrDefault(0);
        string? arg1 = args.ElementAtOrDefault(1);

        List<int> years = GetYears(arg0);

        if (years.Any())
        {
            List<int> days = GetDays(arg1);

            if (days.Any())
            {
                result.Years = years;
                result.Days = days;
                result.Validated = true;

                return result;
            }
        }

        // Not validated result
        return result;
    }

    private static List<int> GetYears(string? arg)
    {
        if (arg != null)
            return int.TryParse(arg, out int argYear) ? new List<int> { argYear } : new List<int>();

        int firstAoCYear = 2015;

        return Enumerable.Range(firstAoCYear, DateTime.Now.Year - firstAoCYear + 1).ToList();
    }

    private static List<int> GetDays(string? arg)
    {
        if (arg != null)
            return int.TryParse(arg, out int argDay) ? new List<int> { argDay } : new List<int>();

        return Enumerable.Range(1, 25).ToList();
    }
}