
namespace AdventOfCode.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class ProblemAttribute : Attribute
    {
        public int Year { get; set; }
        public int Day { get; set; }
        public string ProblemName { get; set; } = string.Empty;
    }
}
