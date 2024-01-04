using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;
using AdventOfCode.Utils;
using System.Text.RegularExpressions;

namespace AdventOfCode.Year2023.Day08
{
    [Problem(Year = 2023, Day = 8, ProblemName = "Haunted Wasteland")]
    internal partial class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input)
        {
            string[] lines = input.GetLines();

            List<Instruction> instructions = GetInstructions(lines[0]);
            Dictionary<string, List<string>> elements = GetElements(lines.Skip(2));

            return GetSteps(START_ELEMENT, elements, instructions).ToString();
        }

        public string Part2(string input)
        {
            string[] lines = input.GetLines();

            List<Instruction> instructions = GetInstructions(lines[0]);
            Dictionary<string, List<string>> elements = GetElements(lines.Skip(2));

            var startElements = elements.Where(k => k.Key.Last() == START_CHAR).Select(k => k.Key);
            long[] elementsSteps = startElements.Select(e => GetSteps(e, elements, instructions)).ToArray();

            return MathUtils.Lcm(elementsSteps).ToString();
        }


        private static readonly char LEFT_CHAR = 'L';
        private static readonly char START_CHAR = 'A';
        private static readonly char END_CHAR = 'Z';
        private static readonly string START_ELEMENT = "AAA";

        private long GetSteps(string element, Dictionary<string, List<string>> elements, List<Instruction> instructions)
        {
            long steps = 0;
            string nextElement = element;

            do
            {
                int instIndex = (int)(steps % instructions.Count);
                nextElement = elements[nextElement][(int)instructions[instIndex]];
                steps++;
            }
            while (nextElement.Last() != END_CHAR);

            return steps;
        }

        private static List<Instruction> GetInstructions(string line) => line.Select(c => c == LEFT_CHAR ? Instruction.Left : Instruction.Right).ToList();

        private static Dictionary<string, List<string>> GetElements(IEnumerable<string> lines)
        {
            Dictionary<string, List<string>> elements = new();

            foreach (string line in lines)
            {
                Match match = InputRegex().Match(line);

                string element = match.Groups[1].Value;
                string left = match.Groups[2].Value;
                string right = match.Groups[3].Value;

                elements.Add(element, new List<string> { left, right });
            }

            return elements;
        }


        internal enum Instruction 
        {
            Left = 0,
            Right
        }

        [GeneratedRegex("(.*?) = \\((.*?), (.*?)\\)")]
        private static partial Regex InputRegex();
    }
}