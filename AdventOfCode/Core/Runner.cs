using AdventOfCode.Core.Interfaces;
using System.Diagnostics;

namespace AdventOfCode.Core
{
    internal class Runner
    {

        internal static void RunProblems(List<int> years, List<int> days)
        {
            var problemsByYear = GetProblemsByYear(years, days);

            foreach ((int year, List<ProblemData> yearProblems) in problemsByYear)
            {
                WriteTableTitle(year);
                WriteTableHeader();

                foreach (ProblemData problemData in yearProblems)
                {
                    string input = File.ReadAllText(problemData.InputPath);

                    (string solutionPart1, TimeSpan elapsedTimePart1) = RunProblem(problemData.Problem.Part1, input);
                    (string solutionPart2, TimeSpan elapsedTimePart2) = RunProblem(problemData.Problem.Part2, input);

                    WriteTableRow(FormatRow(problemData, solutionPart1, elapsedTimePart1, solutionPart2, elapsedTimePart2));
                }

                CloseTable();
            }
        }

        private static (string solution, TimeSpan elapsedTime) RunProblem(Func<string, string> partFunction, string input)
        {
            Stopwatch watch = Stopwatch.StartNew();
            string solution = partFunction(input);
            watch.Stop();

            return (solution, watch.Elapsed);
        }

        private static List<(int year, List<ProblemData> problemsData)> GetProblemsByYear(List<int> years, List<int> days)
        {
            List<(int year, List<ProblemData> problemsData)> problems = new();

            var iProblemType = typeof(IProblem);
            var problemTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => iProblemType.IsAssignableFrom(t) && t.IsClass)
                .Where(t => t.IsDefined(typeof(ProblemAttribute), false));

            Dictionary<ProblemAttribute, Type> problemsTypes = problemTypes.ToDictionary(t => (ProblemAttribute)t.GetCustomAttributes(typeof(ProblemAttribute), true).First());

            foreach (int year in years)
            {
                List<ProblemData> yearProblems = new();

                foreach (int day in days)
                {
                    var problemMeta = problemsTypes.Keys.FirstOrDefault(pa => pa.Year == year && pa.Day == day);

                    if (problemMeta != null)
                    {
                        IProblem? problem = (IProblem?)Activator.CreateInstance(problemsTypes[problemMeta]);

                        if (problem != null)
                        {
                            yearProblems.Add(new ProblemData
                            {
                                Problem = problem,
                                ProblemInfo = problemMeta,
                                InputPath = GetInputPath(year, day, problem.Debug)
                            });
                        }
                    }
                }

                if (yearProblems.Any())
                    problems.Add((year, yearProblems));
            }

            return problems;
        }

        private static ConsoleColor GetPerformanceColor(TimeSpan elapsedTimePart) => elapsedTimePart.TotalMilliseconds switch
        {
            <= Constants.PERFORMANCE_MS_GREEN_THRESHOLD => ConsoleColor.Green,
            > Constants.PERFORMANCE_MS_GREEN_THRESHOLD and <= Constants.PERFORMANCE_MS_YELLOW_THRESHOLD => ConsoleColor.Yellow,
            _ => ConsoleColor.Red,
        };

        private static string GetInputPath(int year, int day, bool debug) => GetDayPath(year, day) + (debug ? Constants.DEBUG_INPUT_DEFAULT_FILENAME : Constants.INPUT_FILENAME);

        private static string GetDayPath(int year, int day)
        {
            char dirSeparator = Path.DirectorySeparatorChar;
            string appDir = AppDomain.CurrentDomain.BaseDirectory;

            return $"{appDir}Year{year}{dirSeparator}Day{day:D2}{dirSeparator}";
        }

        private static void WriteTableTitle(int year) => Console.WriteLine($"\n    {year}");

        private static void WriteTableRow(TableRow row)
        {
            Console.Write($"    ║ {row.Day,3} ║ {row.ProblemName,-26} ║ {row.SolutionPart1,16} ║ ");

            Console.ForegroundColor = row.PerformanceColorPart1;
            Console.Write(row.ElapsedTimePart1Str.PadLeft(15));
            Console.ResetColor();

            Console.Write($" ║ {row.SolutionPart2,16} ║ ");

            Console.ForegroundColor = row.PerformanceColorPart2;
            Console.Write(row.ElapsedTimePart2Str.PadLeft(15));
            Console.ResetColor();

            Console.Write($" ║");

            Console.WriteLine();
        }


        private static void WriteTableHeader() => Console.WriteLine(
            """
                ╔═════╦════════════════════════════╦══════════════════╦═════════════════╦══════════════════╦═════════════════╗
                ║ Day ║ Problem                    ║ Part 1 Solution  ║ Part 1 Time (s) ║ Part 2 Solution  ║ Part 2 Time (s) ║
                ╠═════╬════════════════════════════╬══════════════════╬═════════════════╬══════════════════╬═════════════════╣
            """
        );

        private static void CloseTable() => Console.WriteLine(
            """
                ╚═════╩════════════════════════════╩══════════════════╩═════════════════╩══════════════════╩═════════════════╝
            """
        );

        private static TableRow FormatRow(ProblemData problemData, string solutionPart1, TimeSpan elapsedTimePart1, string solutionPart2, TimeSpan elapsedTimePart2)
        {
            string timeSpanFormat = "s\\.ffffff";

            ConsoleColor performanceColorPart1 = GetPerformanceColor(elapsedTimePart1);
            ConsoleColor performanceColorPart2 = GetPerformanceColor(elapsedTimePart2);
            string elapsedTimePart1Str = elapsedTimePart1.ToString(timeSpanFormat);
            string elapsedTimePart2Str = elapsedTimePart2.ToString(timeSpanFormat);

            return new(problemData.GetDay().ToString(), problemData.GetName(), solutionPart1, elapsedTimePart1Str, performanceColorPart1, solutionPart2, elapsedTimePart2Str, performanceColorPart2);
        }


        private record TableRow(string Day, string ProblemName, string SolutionPart1, string ElapsedTimePart1Str, ConsoleColor PerformanceColorPart1, string SolutionPart2, string ElapsedTimePart2Str, ConsoleColor PerformanceColorPart2);


        private class ProblemData
        {
            public required IProblem Problem { get; set; }
            public required ProblemAttribute ProblemInfo { get; set; }
            public required string InputPath { get; set; }

            public int GetDay() => ProblemInfo.Day;
            public string GetName() => ProblemInfo.ProblemName;
        }

    }
}
