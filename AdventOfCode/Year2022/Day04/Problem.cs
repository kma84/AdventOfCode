using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;

namespace AdventOfCode.Year2022.Day04
{
    [Problem(Year = 2022, Day = 4, ProblemName = "Camp Cleanup")]
    internal class Problem : IProblem
    {
        public string Part1(string input) => GetAssignmentList(input).Count(t => AssignmentsFullyOverlap(t.assignment1, t.assignment2)).ToString();

        public string Part2(string input) => GetAssignmentList(input).Count(t => AssignmentsPartiallyOverlap(t.assignment1, t.assignment2)).ToString();


        private static List<(Assignment assignment1, Assignment assignment2)> GetAssignmentList(string input) => input.GetLines().Select(GetAssignments).ToList();

        private static (Assignment assignment1, Assignment assignment2) GetAssignments(string line)
        {
            string[] parts = line.Split(',');
            List<int> idsAssign1 = GetAssignmentIds(parts[0]);
            List<int> idsAssign2 = GetAssignmentIds(parts[1]);

            return (new(idsAssign1[0], idsAssign1[1]), new(idsAssign2[0], idsAssign2[1]));
        }

        private static List<int> GetAssignmentIds(string assignmentStr) => assignmentStr.Split('-').Select(id => int.Parse(id)).ToList();

        private static bool AssignmentsFullyOverlap(Assignment a1, Assignment a2) => (a1.Min <= a2.Min && a1.Max >= a2.Max) || (a2.Min <= a1.Min && a2.Max >= a1.Max);

        private bool AssignmentsPartiallyOverlap(Assignment a1, Assignment a2) => (a1.Min <= a2.Max && a1.Max >= a2.Min) || (a1.Min <= a2.Max && a1.Max >= a2.Min);


        private record Assignment(int Min, int Max);
    }
}