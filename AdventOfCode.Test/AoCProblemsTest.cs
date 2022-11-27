using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;

namespace AdventOfCode.Test
{
    [TestClass]
    public class AoCProblemsTest
    {
        [TestMethod]
        public void ProblemsTest()
        {
            var iProblemType = typeof(IProblem);
            var problemTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => iProblemType.IsAssignableFrom(t) && t.IsClass);

            Dictionary<ProblemAttribute, Type> dictProblems = problemTypes.ToDictionary(t => (ProblemAttribute)t.GetCustomAttributes(typeof(ProblemAttribute), true).First());

            foreach (ProblemAttribute problemMeta in dictProblems.Keys.OrderBy(k => k.Year).ThenBy(k => k.Day))
            {
                IProblem? problem = (IProblem?)Activator.CreateInstance(dictProblems[problemMeta]);

                string dayPath = GetDayPath(problemMeta.Year, problemMeta.Day);
                string solutionsPath = dayPath + Constants.SOLUTIONS_FILENAME;

                if (File.Exists(solutionsPath))
                {
                    Console.WriteLine($"Testing year {problemMeta.Year}, day {problemMeta.Day}, problem \"{problemMeta.ProblemName}\"");

                    string input = File.ReadAllText(dayPath + Constants.INPUT_FILENAME);
                    string[] solutions = File.ReadAllLines(solutionsPath);
                                        
                    string testFailedMessage = $"Unexpected value in year {problemMeta.Year}, day {problemMeta.Day}, problem \"{problemMeta.ProblemName}\", part {{0}}.";
                    string solutionPart1 = solutions.ElementAtOrDefault(0) ?? string.Empty;
                    string solutionPart2 = solutions.ElementAtOrDefault(1) ?? string.Empty;

                    if (!string.IsNullOrEmpty(solutionPart1))
                        Assert.AreEqual(solutionPart1, problem?.Part1(input), string.Format(testFailedMessage, 1));

                    if (!string.IsNullOrEmpty(solutionPart2))
                        Assert.AreEqual(solutionPart2, problem?.Part2(input), string.Format(testFailedMessage, 2));
                }
            }

            Assert.IsNotNull(AppDomain.CurrentDomain.BaseDirectory);
        }

        static string GetDayPath(int year, int day)
        {
            char dirSeparator = Path.DirectorySeparatorChar;
            string appDir = AppDomain.CurrentDomain.BaseDirectory;

            return $"{appDir}Year{year}{dirSeparator}Day{day:D2}{dirSeparator}";
        }
    }
}