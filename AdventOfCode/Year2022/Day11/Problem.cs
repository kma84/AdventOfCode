using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;

namespace AdventOfCode.Year2022.Day11
{
    [Problem(Year = 2022, Day = 11, ProblemName = "Monkey in the Middle")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input) => GetMonkeyBusinessLvl(GetMonkeys(input.GetLines()), rounds: 20).ToString();

        public string Part2(string input) => GetMonkeyBusinessLvl(GetMonkeys(input.GetLines()), rounds: 10000, simplifyWorryLvl: true).ToString();


        private static long GetMonkeyBusinessLvl(List<Monkey> monkeys, int rounds, bool simplifyWorryLvl = false)
        {
            List<int> monkeysDivisors = monkeys.Select(m => m.TestDivisibleBy).ToList();
            long divisorsCommonMultiple = monkeysDivisors.Aggregate((total, current) => total * current);

            for (int i = 0; i < rounds; i++)
            {
                foreach (Monkey monkey in monkeys)
                {
                    while (monkey.Items.TryDequeue(out long item))
                    {
                        long worryLvl = monkey.Operation(item);
                        worryLvl = simplifyWorryLvl ? worryLvl % divisorsCommonMultiple : worryLvl / 3;

                        int throwTo = worryLvl % monkey.TestDivisibleBy == 0 ? monkey.MonkeyToThrowIfTrue : monkey.MonkeyToThrowIfFalse;

                        monkeys[throwTo].Items.Enqueue(worryLvl);
                        monkey.InspectedItems++;
                    }
                }
            }

            // Multiplication of the two most active monkeys
            return monkeys.Select(m => (long)m.InspectedItems).OrderDescending().Take(2).Aggregate((total, next) => total * next);
        }

        private static List<Monkey> GetMonkeys(string[] lines)
        {
            List < Monkey > monkeys = new ();
            int i = 0;

            while (i < lines.Length)
            {
                int monkeyNumber = int.Parse(lines[i++].Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries)[1]);
                Queue<long> items = new(lines[i++].Split(new char[] { ' ', ':', ',' }, StringSplitOptions.RemoveEmptyEntries).Skip(2).Select(s => long.Parse(s)));
                Func<long, long> operation = GetOperationFromInput(lines[i++]);
                int divisibleBy = int.Parse(lines[i++].Split().Last());
                int monkeyToThrowIfTrue = int.Parse(lines[i++].Split().Last());
                int monkeyToThrowIfFalse = int.Parse(lines[i++].Split().Last());

                monkeys.Add(new Monkey { 
                    Number = monkeyNumber,
                    Items= items,
                    Operation = operation,
                    TestDivisibleBy = divisibleBy,
                    MonkeyToThrowIfTrue = monkeyToThrowIfTrue,
                    MonkeyToThrowIfFalse = monkeyToThrowIfFalse
                });

                i++;
            }

            return monkeys;
        }

        private static Func<long, long> GetOperationFromInput(string line) => line.Split() switch
        {
            [.. _, "*", "old"]              => (long old) => old * old,
            [.. _, "*", string numberStr]   => (long old) => old * long.Parse(numberStr),
            [.. _, "+", string numberStr]   => (long old) => old + long.Parse(numberStr),
            _                               => throw new NotImplementedException()
        };

        private class Monkey
        {
            public int Number { get; set; }
            public Queue<long> Items { get; set; } = new ();
            public required Func<long, long> Operation { get; set; }
            public int TestDivisibleBy { get; set; }
            public int MonkeyToThrowIfTrue { get; set; }
            public int MonkeyToThrowIfFalse { get; set; }
            public int InspectedItems { get; set; } = 0;
        }
	}
}