using AdventOfCode.Core.Interfaces;

namespace AdventOfCode.Core
{
    internal class Runner
    {

        internal static void RunProblems(List<int> years, List<int> days)
        {
            var problemsByYear = GetProblemsByYear(years, days);

            foreach ((int year, List<ProblemData> yearProblems) in problemsByYear)
            {
                Console.WriteLine($"    {year}");

                Console.WriteLine(
                    """
                        ╔═════╦════════════════════════════╦══════════════════╦═══════════════╦══════════════════╦═══════════════╗
                        ║ Day ║ Problem                    ║ Part 1 Solution  ║ Part 1 Time   ║ Part 2 Solution  ║ Part 2 Time   ║
                        ╠═════╬════════════════════════════╬══════════════════╬═══════════════╬══════════════════╬═══════════════╣
                    """
                );

                foreach (ProblemData problemData in yearProblems)
                {
                    string input = File.ReadAllText(problemData.InputPath);

                    Console.WriteLine($"    ║{problemData.GetDay(), 4} ║ {problemData.GetName(), -27}║ {problemData.Problem.Part1(input), 16} ║               ║ {problemData.Problem.Part2(input),16} ║               ║");
                }

                Console.WriteLine(
                    """
                        ╚═════╩════════════════════════════╩══════════════════╩═══════════════╩══════════════════╩═══════════════╝
                    """
                );
            }
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

                        //string input = File.ReadAllText(GetInputPath(year, day, problem?.Debug ?? false));

                        //Console.WriteLine($"Year {year}, Day {day}, Problem: {problemMeta.ProblemName}");
                        //Console.WriteLine($"Part1: {problem?.Part1(input)}");
                        //Console.WriteLine($"Part2: {problem?.Part2(input)}");

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


        private class ProblemData
        {
            public required IProblem Problem { get; set; }
            public required ProblemAttribute ProblemInfo { get; set; }
            public required string InputPath { get; set; }
            //public string Part1Solution { get; set; }
            //public string Part2Solution { get; set; }

            public int GetDay() => ProblemInfo.Day;
            public string GetName() => ProblemInfo.ProblemName;
        }

    }
}
