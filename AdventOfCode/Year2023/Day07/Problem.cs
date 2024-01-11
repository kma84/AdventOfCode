using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;
using System.Text.RegularExpressions;

namespace AdventOfCode.Year2023.Day07
{
    [Problem(Year = 2023, Day = 7, ProblemName = "Camel Cards")]
    internal partial class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input)
        {
            List<Hand> hands = GetHands(input.GetLines());

            hands.Sort();

			return GetTotalWinnings(hands).ToString();
        }

        public string Part2(string input)
        {
            CARDS.Remove(JOKER);
            CARDS.Insert(0, JOKER);

            List<Hand> hands = GetHands(input.GetLines(), withJokers: true);

            hands.Sort();

            return GetTotalWinnings(hands).ToString();
        }


        private static readonly char JOKER = 'J';

        // Cards ordered by strength ascending
        private static readonly List<char> CARDS =
        [
            '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A'
        ];

        private static int GetTotalWinnings(List<Hand> hands) => hands.Select((h, i) => h.Bid * (i + 1)).Sum();

        private List<Hand> GetHands(string[] lines, bool withJokers = false)
        {
            List<Hand> hands = [];

            foreach (string line in lines) 
            {
                string[] tokens = line.Split();

                hands.Add(new Hand(tokens[0], int.Parse(tokens[1]), withJokers));
            }

            return hands;
        }

        private class Hand : IComparable<Hand> 
        {
            public string Cards { get; set; }
            public int Bid { get; set; }
            public int Value { get; set; }
            public HandType Type { get; set; }

            public Hand(string cards, int bid, bool withJokers = false)
            {
                Cards = cards;
                Bid = bid;

                var cardsStrength = cards.Select(c => CARDS.IndexOf(c).ToString("D2"));
                Value = int.Parse(string.Concat(cardsStrength));
                                
                Type = withJokers ? GetHandTypeWithJokers(cards) : GetHandTypeWithoutJokers(cards);
            }

            private static HandType GetHandTypeWithJokers(string cards)
            {
                if (!cards.Contains(JOKER))
                    return GetHandTypeWithoutJokers(cards);

                string cardsWithoutJokers = cards.Replace(JOKER.ToString(), string.Empty);

                if (string.IsNullOrEmpty(cardsWithoutJokers))
                    return HandType.FiveOfAKind;

                char mostRepeatedCard = cardsWithoutJokers.GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;
                string newCards = new(cards.Replace(JOKER, mostRepeatedCard).OrderBy(x => x).ToArray());

                return GetHandType(newCards);
            }

            private static HandType GetHandTypeWithoutJokers(string cards)
            {
                string orderedCards = new(cards.OrderBy(x => x).ToArray());
                return GetHandType(orderedCards);
            }

            private static HandType GetHandType(string cards) => cards switch
            {
                string card when FiveOfAKindRegex().IsMatch(card)   => HandType.FiveOfAKind,
                string card when FourOfAKindRegex().IsMatch(card)   => HandType.FourOfAKind,
                string card when FullHouseRegex().IsMatch(card)     => HandType.FullHouse,
                string card when ThreeOfAKindRegex().IsMatch(card)  => HandType.ThreeOfAKind,
                string card when TwoPairRegex().IsMatch(card)       => HandType.TwoPair,
                string card when OnePairRegex().IsMatch(card)       => HandType.OnePair,
                _                                                   => HandType.HighCard,
            };

            public int CompareTo(Hand? other)
            {
                int typeOrder = (int)(other?.Type ?? 0) - (int)this.Type;

                if (typeOrder != 0)
                    return typeOrder;

                return this.Value - (other?.Value ?? 0);
            }
        }

        private enum HandType 
        {
            FiveOfAKind = 0,
            FourOfAKind,
            FullHouse,
            ThreeOfAKind,
            TwoPair,
            OnePair,
            HighCard
        }
                
        [GeneratedRegex("([AKQJT]|[2-9])\\1{4}")]
        private static partial Regex FiveOfAKindRegex();

        [GeneratedRegex("([AKQJT]|[2-9])\\1{3}")]
        private static partial Regex FourOfAKindRegex();

        [GeneratedRegex("([AKQJT]|[2-9])\\1{2}([AKQJT]|[2-9])\\2{1}|([AKQJT]|[2-9])\\3{1}([AKQJT]|[2-9])\\4{2}")]
        private static partial Regex FullHouseRegex();

        [GeneratedRegex("([AKQJT]|[2-9])\\1{2}")]
        private static partial Regex ThreeOfAKindRegex();

        [GeneratedRegex("([AKQJT]|[2-9])\\1{1}.?([AKQJT]|[2-9])\\2{1}")]
        private static partial Regex TwoPairRegex();

        [GeneratedRegex("([AKQJT]|[2-9])\\1{1}")]
        private static partial Regex OnePairRegex();
    }
}