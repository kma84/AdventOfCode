using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2023.Day04
{
    [Problem(Year = 2023, Day = 4, ProblemName = "Scratchcards")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input) => GetCards(input)
                    .Aggregate(0, (total, card) => total + (int)Math.Pow(2, card.Intersect.Length - 1))
                    .ToString();

        public string Part2(string input)
        {
            List<Card> cards = GetCards(input);

            for (int i = 0; i < cards.Count; i++)
                for (int j = i + 1; j <= i + cards[i].Intersect.Length; j++)
                    cards[j].Copies += cards[i].Copies;

            return cards.Sum(c => c.Copies).ToString();
        }


        private static List<Card> GetCards(string input)
        {
            List<Card> cards = [];

            foreach (string line in input.GetLines())
            {
                string[] tokens = line.Split(':', '|');
                int id = int.Parse(tokens[0].Remove(0, 5));
                string[] winners = tokens[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                string[] numbers = tokens[2].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                string[] intersect = winners.Intersect(numbers).ToArray();

                cards.Add(new Card(id, winners, numbers, intersect));
            }

            return cards;
        }


        private record Card(int Id, string[] Winners, string[] Numbers, string[] Intersect)
        {
            public int Copies { get; set; } = 1;
        }
    }
}