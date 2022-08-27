
namespace AdventOfCode.Core.Interfaces
{
    internal interface IProblem
    {
        string Part1(string input);
        string Part2(string input);

        bool Debug { get => false; set { } }
        string DebugInputFileName { get => string.Empty; set { } }
    }
}
