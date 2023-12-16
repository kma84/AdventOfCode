using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2021.Day21
{
    [Problem(Year = 2021, Day = 21, ProblemName = "Dirac Dice")]
    internal class Problem : IProblem
    {
        const int BOARD_SPACES = 10;
        const int MAX_SCORE_PART1 = 1000;
        const int MAX_SCORE_PART2 = 21;

        private static readonly List<(int rollValue, int numUniverses)> NEW_ROLLS = new() {
            ( 3, 1 ),
            ( 4, 3 ),
            ( 5, 6 ),
            ( 6, 7 ),
            ( 7, 6 ),
            ( 8, 3 ),
            ( 9, 1 ),
        };

        public bool Debug { get; set; } = false;


        public string Part1(string input)
        {
            static void Roll(ref int playerPosition, ref int playerScore, ref int timesRolled, ref int deterministicDice)
            {
                int playerNewRoll = ++deterministicDice + ++deterministicDice + ++deterministicDice;
                timesRolled += 3;

                playerPosition = (playerPosition + playerNewRoll - 1) % BOARD_SPACES + 1;
                playerScore += playerPosition;
            }

            (int player1Position, int player2Position) = GetStartingPositions(input);
            int player1Score = 0;
            int player2Score = 0;
            int timesRolled = 0;
            int deterministicDice = 0;

            while (player1Score < MAX_SCORE_PART1 && player2Score < MAX_SCORE_PART1)
            {
                Roll(ref player1Position, ref player1Score, ref timesRolled, ref deterministicDice);

                if (player1Score < MAX_SCORE_PART1)
                {
                    Roll(ref player2Position, ref player2Score, ref timesRolled, ref deterministicDice);
                }
            }

            int product = Math.Min(player1Score, player2Score) * timesRolled;
            return product.ToString();
        }

        public string Part2(string input)
        {
            long winnerUniversesP1 = 0;
            long winnerUniversesP2 = 0;

            (int player1StartingPosition, int player2StartingPosition) = GetStartingPositions(input);
            CommonUniverses startingUniverse = new CommonUniverses(player1StartingPosition, player2StartingPosition);
            Dictionary<CommonUniverses, long> multiverse = new() { { startingUniverse, 1 } };

            while (multiverse.Any(d => d.Value > 0))
            {
                (Dictionary<CommonUniverses, long> newMultiverseAfterRoll1, long winnersP1) = Roll(multiverse, player: 1);
                winnerUniversesP1 += winnersP1;

                (Dictionary<CommonUniverses, long> newMultiverseAfterRoll2, long winnersP2) = Roll(newMultiverseAfterRoll1, player: 2);
                winnerUniversesP2 += winnersP2;

                multiverse = newMultiverseAfterRoll2;
            }

            return Math.Max(winnerUniversesP1, winnerUniversesP2).ToString();
        }

        private static (Dictionary<CommonUniverses, long> newMultiverse, long winnerUniverses) Roll(Dictionary<CommonUniverses, long> multiverse, int player)
        {
            long winnerUniverses = 0;
            Dictionary<CommonUniverses, long> newMultiverse = new();

            foreach (var roll in NEW_ROLLS)
            {
                foreach (var kvpUniverses in multiverse)
                {
                    long newUniverses = kvpUniverses.Value * roll.numUniverses;
                    int playerPosition = player == 1 ? kvpUniverses.Key.Player1Position : kvpUniverses.Key.Player2Position;
                    int newPosition = (playerPosition + roll.rollValue - 1) % BOARD_SPACES + 1;
                    int playerScore = player == 1 ? kvpUniverses.Key.Player1Score : kvpUniverses.Key.Player2Score;
                    int newScore = playerScore + newPosition;

                    CommonUniverses commonUniverses;
                    if (player == 1)
                        commonUniverses = new CommonUniverses(newPosition, kvpUniverses.Key.Player2Position, newScore, kvpUniverses.Key.Player2Score);
                    else
                        commonUniverses = new CommonUniverses(kvpUniverses.Key.Player1Position, newPosition, kvpUniverses.Key.Player1Score, newScore);

                    if (newScore >= MAX_SCORE_PART2)
                    {
                        winnerUniverses += newUniverses;
                    }
                    else
                    {
                        newMultiverse.TryAdd(commonUniverses, 0);
                        newMultiverse[commonUniverses] += newUniverses;
                    }
                }
            }

            return (newMultiverse, winnerUniverses);
        }

        private static (int player1StartingPosition, int player2StartingPosition) GetStartingPositions(string input)
        {
            int GetStartingPosition(string line) => int.Parse(line.Split(':')[1]);
            string[] lines = input.GetLines();

            return (GetStartingPosition(lines[0]), GetStartingPosition(lines[1]));
        }


        record CommonUniverses 
        {
            public CommonUniverses(int player1Position, int player2Position)
            {
                Player1Position = player1Position;
                Player2Position = player2Position;
            }

            public CommonUniverses(int player1Position, int player2Position, int player1Score, int player2Score)
            {
                Player1Position = player1Position;
                Player2Position = player2Position;
                Player1Score = player1Score;
                Player2Score = player2Score;
            }

            public int Player1Position { get; set; }
            public int Player2Position { get; set; }
            public int Player1Score { get; set; } = 0;
            public int Player2Score { get; set; } = 0;
        }               

	}
}