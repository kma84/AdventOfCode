using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2021.Day14
{
    [Problem(Year = 2021, Day = 14, ProblemName = "Extended Polymerization")]
    internal class Problem : IProblem
    {
        public bool Debug { get; set; } = false;

        public string Part1(string input)
        {
            (var template, var rules) = GetInput(input);

            string polymer10Steps = RunStepsBruteForce(template, rules, 10);

            IEnumerable<int> counts = polymer10Steps.GroupBy(c => c).Select(g => g.Count());
            int quantityMostCommon = counts.OrderByDescending(c => c).First();
            int quantityLeastCommon = counts.OrderBy(c => c).First();
            int substract = quantityMostCommon - quantityLeastCommon;

            if (Debug)
            {
                Console.WriteLine("Part1");
                Console.WriteLine("Polymer length: " + polymer10Steps.Length);
                Console.WriteLine("Quantity of the most common element: " + quantityMostCommon);
                Console.WriteLine("Quantity of the least common element: " + quantityLeastCommon);
                Console.WriteLine("Substract: " + substract);
            }

            return substract.ToString();
        }

        public string Part2(string input)
        {
            (var template, var rules) = GetInput(input);

            List<Node> nodes = GetNodes(template, rules);
            Dictionary<char, long> letters = GetLetters(template, rules);

            RunSteps(nodes, letters, 40);

            long substract = letters.Values.Max() - letters.Values.Min();

            if (Debug)
            {
                Console.WriteLine("Part2");
                Console.WriteLine("Quantity of the most common element: " + letters.Values.Max());
                Console.WriteLine("Quantity of the least common element: " + letters.Values.Min());
                Console.WriteLine("Substract: " + substract);
            }

            return substract.ToString();
        }


        private static void RunSteps(List<Node> nodes, Dictionary<char, long> letters, int numSteps)
        {
            for (int i = 1; i <= numSteps; i++)
            {
                foreach (Node node in nodes)
                    node.Step(letters);

                foreach (Node node in nodes)
                    node.ApplyIncrement();
            }
        }


        private static Dictionary<char, long> GetLetters(string template, Dictionary<string, char> rules)
        {
            Dictionary<char, long> letters = new();

            foreach (char letter in rules.Values.Distinct())
            {
                letters.Add(letter, 0);
            }

            foreach (char letter in template)
            {
                letters[letter]++;
            }

            return letters;
        }


        private static List<Node> GetNodes(string template, Dictionary<string, char> rules)
        {
            Dictionary<string, Node> nodes = new();

            foreach (var ruleKvp in rules)
            {
                nodes.Add(ruleKvp.Key, new Node(ruleKvp.Key, ruleKvp.Value));
            }

            foreach (Node node in nodes.Values)
            {
                node.Node1 = nodes[new string(new char[] { node.Value[0], node.NewElement })];
                node.Node2 = nodes[new string(new char[] { node.NewElement, node.Value[1] })];
            }

            int i = 0;
            while (i < template.Length - 1)
            {
                string pair = template.Substring(i, 2);
                nodes[pair].Ocurrences++;

                i++;
            }

            return nodes.Values.ToList();
        }


        private static string RunStepsBruteForce(string initialPolymer, Dictionary<string, char> rules, int numSteps)
        {
            string polymer = initialPolymer;

            for (int i = 1; i <= numSteps; i++)
            {
                polymer = StepBruteForce(polymer, rules);
            }

            return polymer;
        }


        private static string StepBruteForce(string polymer, Dictionary<string, char> rules)
        {
            int i = 0;

            while (i < polymer.Length - 1)
            {
                string pair = polymer.Substring(i, 2);

                if (rules.ContainsKey(pair))
                {
                    polymer = polymer.Insert(i + 1, rules[pair].ToString());
                    i++;
                }

                i++;
            }

            return polymer;
        }


        private static (string template, Dictionary<string, char> rules) GetInput(string input)
        {
            string[] lines = input.GetLines(StringSplitOptions.RemoveEmptyEntries);

            string template = lines.First();
            Dictionary<string, char> rules = new();

            foreach (string line in lines.Skip(1))
            {
                string[] parts = line.Split(" -> ");
                rules.Add(parts[0], parts[1][0]);
            }

            return (template, rules);
        }


        private class Node
        {
            public string Value { get; set; }
            public char NewElement { get; set; }
            public long Ocurrences { get; set; } = 0;
            public long Increment { get; set; } = 0;
            public Node? Node1 { get; set; }
            public Node? Node2 { get; set; }

            public Node(string value, char newElement)
            {
                Value = value;
                NewElement = newElement;
            }

            public void ApplyIncrement()
            {
                Ocurrences = Increment;
                Increment = 0;
            }

            public void Step(Dictionary<char, long> letters)
            {
                letters[NewElement] += Ocurrences;

                if (Node1 != null && Node2 != null)
                {
                    Node1.Increment += Ocurrences;
                    Node2.Increment += Ocurrences;
                }

                Ocurrences = 0;
            }
        }
    }
}