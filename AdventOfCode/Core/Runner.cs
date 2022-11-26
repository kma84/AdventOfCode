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

                    WriteTableRow(problemData.GetDay(), problemData.GetName(), solutionPart1, elapsedTimePart1, solutionPart2, elapsedTimePart2);
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
                .Where(t => iProblemType.IsAssignableFrom(t) && t.IsClass);

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
                            yearProblems.Add(new ProblemData {
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

        private static string GetInputPath(int year, int day, bool debug) => GetDayPath(year, day) + (debug ? Constants.DEBUG_INPUT_DEFAULT_FILENAME : Constants.INPUT_FILENAME);

        private static string GetDayPath(int year, int day)
        {
            char dirSeparator = Path.DirectorySeparatorChar;
            string appDir = AppDomain.CurrentDomain.BaseDirectory;

            return $"{appDir}Year{year}{dirSeparator}Day{day:D2}{dirSeparator}";
        }

        private static void WriteTableTitle(int year) => Console.WriteLine($"    {year}");

        private static void WriteTableRow(int day, string problemName, string solutionPart1, TimeSpan tsPart1, string solutionPart2, TimeSpan tsPart2)
        {
            string timeSpanFormat = "s\\.ffffff";
            string row = string.Format(
                "    ║ {0, 3} ║ {1, -26} ║ {2, 16} ║ {3, 15} ║ {4, 16} ║ {5, 15} ║",
                day,
                problemName,
                solutionPart1,
                tsPart1.ToString(timeSpanFormat),
                solutionPart2,
                tsPart2.ToString(timeSpanFormat)
            );

            Console.WriteLine(row);
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
