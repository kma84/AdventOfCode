using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;

namespace AdventOfCode.Year2022.Day02
{
    [Problem(Year = 2022, Day = 2, ProblemName = "Rock Paper Scissors")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input) => GetPart1StrategyGuide(input).Select(GetRoundScore).Sum().ToString();

        public string Part2(string input) => GetPart2StrategyGuide(input).Select(GetRoundScore).Sum().ToString();


        private static IEnumerable<Round> GetPart1StrategyGuide(string input) => 
            input.GetLines(StringSplitOptions.RemoveEmptyEntries).Select(l => new Round(GetShapeFromChar(l[0]), GetShapeFromChar(l[2])) );

        private static IEnumerable<Round> GetPart2StrategyGuide(string input) => 
            input.GetLines(StringSplitOptions.RemoveEmptyEntries).Select(l => 
            {
                ShapeEnum shapePlayerA = GetShapeFromChar(l[0]);
                return new Round(shapePlayerA, ChoosePlayer2Shape(l[2], shapePlayerA));
            });

        private static ShapeEnum ChoosePlayer2Shape(char c, ShapeEnum shapePlayerA) => c switch
        {
            'X' => WinnerRounds.First(r => r.ShapePlayerB == shapePlayerA).ShapePlayerA,            
            'Y' => shapePlayerA,
            'Z' => WinnerRounds.First(r => r.ShapePlayerA == shapePlayerA).ShapePlayerB,
            _ => throw new ArgumentOutOfRangeException(nameof(c), $"Not expected char value: {c}")
        };

        private static ShapeEnum GetShapeFromChar(char c) => c switch
        {
            'A' or 'X' => ShapeEnum.ROCK,
            'B' or 'Y' => ShapeEnum.PAPER,
            'C' or 'Z' => ShapeEnum.SCISSORS,
            _ => throw new ArgumentOutOfRangeException(nameof(c), $"Not expected char value: {c}")
        };

        private static int GetRoundScore(Round round)
        {
            int shapeValue = (int)round.ShapePlayerB;

            if (round.ShapePlayerA == round.ShapePlayerB)
                return 3 + shapeValue;

            if (WinnerRounds.Contains(round))
                return 6 + shapeValue;

            return shapeValue;
        }

        private static List<Round> WinnerRounds => new() {
            new Round(ShapeEnum.ROCK, ShapeEnum.PAPER),
            new Round(ShapeEnum.PAPER, ShapeEnum.SCISSORS),
            new Round(ShapeEnum.SCISSORS, ShapeEnum.ROCK)
        };

        private record Round(ShapeEnum ShapePlayerA, ShapeEnum ShapePlayerB);

        private enum ShapeEnum
        {
            ROCK = 1,
            PAPER = 2,
            SCISSORS = 3
        }
	}
}