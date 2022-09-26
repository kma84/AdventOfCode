using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;

namespace AdventOfCode.Year2021.Day21
{
    [Problem(Year = 2021, Day = 21, ProblemName = "Dirac Dice")]
    internal class Problem : IProblem
    {
        public bool Debug { get; set; } = false;

        public string Part1(string input)
        {
            (int player1Position, int player2Position) = GetStartingPositions(input);
            int player1Score = 0;
            int player2Score = 0;
            int timesRolled = 0;
            int deterministicDice = 0;

            while (player1Score < 1000 && player2Score < 1000)
            {
                int player1NewRoll = GetNextDiceValue(ref deterministicDice) + GetNextDiceValue(ref deterministicDice) + GetNextDiceValue(ref deterministicDice);
                timesRolled += 3;

                player1Position = (player1Position + player1NewRoll - 1) % 10 + 1;
                player1Score += player1Position;

                if (player1Score < 1000)
                {
                    int player2NewRoll = GetNextDiceValue(ref deterministicDice) + GetNextDiceValue(ref deterministicDice) + GetNextDiceValue(ref deterministicDice);
                    timesRolled += 3;

                    player2Position = (player2Position + player2NewRoll - 1) % 10 + 1;
                    player2Score += player2Position;
                }
            }

            int product = Math.Min(player1Score, player2Score) * timesRolled;
            return product.ToString();
        }

        public string Part2(string input)
        {
			return string.Empty;
        }


        private static int GetNextDiceValue(ref int diceValue) => ++diceValue % 100;

        private static (int player1StartingPosition, int player2StartingPosition) GetStartingPositions(string input)
        {
            int GetStartingPosition(string line) => int.Parse(line.Split(':')[1]);
            string[] lines = input.GetLines();

            return (GetStartingPosition(lines[0]), GetStartingPosition(lines[1]));
        }
	}
}