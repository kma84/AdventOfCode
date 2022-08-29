
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

                UpdateCsproj(xdoc, Path.Combine("Year" + year, "Day" + day.ToString("D2"), "input.txt"));
                UpdateCsproj(xdoc, Path.Combine("Year" + year, "Day" + day.ToString("D2"), "debugInput.txt"));

                Console.WriteLine($"Files generated for year {year} day {day}");
            }
        }
    }

    xdoc.Save(GetCsprojPath());
}

// TODO refactor
void UpdateCsproj(XDocument xdoc, string input)
{
    var projectElement = xdoc.Elements("Project").FirstOrDefault();

    if (projectElement != null)
    {
        // ItemGroup None
        List<XElement> itemGropNone = projectElement.Elements("ItemGroup")?.Elements("None")?.Ancestors("ItemGroup")?.ToList() ?? new();

        if (!itemGropNone.Any())
        {
            XElement newItemGroup = new XElement("ItemGroup");
            projectElement.Add(newItemGroup);
            itemGropNone.Add(newItemGroup);
        }

        if (!itemGropNone.Elements("None").Attributes("Remove").Any(a => a.Value == input))
        {
            XElement newNoneElement = new XElement("None");
            newNoneElement.Add(new XAttribute("Remove", input));
            itemGropNone.First().Add(newNoneElement);
        }


        // ItemGroup Content
        List<XElement> itemGropContent = projectElement.Elements("ItemGroup")?.Elements("Content")?.Ancestors("ItemGroup")?.ToList() ?? new();

        if (!itemGropContent.Any())
        {
            XElement newItemGroup = new XElement("ItemGroup");
            projectElement.Add(newItemGroup);
            itemGropContent.Add(newItemGroup);
        }

        if (itemGropContent.Elements("Content").Attributes("Include").Any(a => a.Value == input))
        {
            XElement contentElement = itemGropContent.Elements("Content").Where(e => e.Attributes("Include").Any(a => a.Value == input)).First();
            XElement? copyElement = contentElement.Elements("CopyToOutputDirectory").FirstOrDefault();

            if (copyElement == null)
            {
                XElement newCopyElement = new XElement("CopyToOutputDirectory", "PreserveNewest");
                contentElement.Add(newCopyElement);
            }
            else if (copyElement.Value != "PreserveNewest")
            {
                copyElement.SetValue("PreserveNewest");
            }
        }
        else
        {
            XElement newCopyElement = new XElement("CopyToOutputDirectory", "PreserveNewest");

            XElement newContentElement = new XElement("Content");
            newContentElement.Add(newCopyElement);
            newContentElement.Add(new XAttribute("Include", input));
            newContentElement.Add();

            itemGropContent.First().Add(newContentElement);
        }

    }
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
