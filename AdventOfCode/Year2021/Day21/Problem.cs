using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;

namespace AdventOfCode.Year2021.Day21
{
    [Problem(Year = 2021, Day = 21, ProblemName = "Dirac Dice")]
    internal class Problem : IProblem
    {
        const int BOARD_SPACES = 10;

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

            const int maxScore = 1000;

            (int player1Position, int player2Position) = GetStartingPositions(input);
            int player1Score = 0;
            int player2Score = 0;
            int timesRolled = 0;
            int deterministicDice = 0;

            while (player1Score < maxScore && player2Score < maxScore)
            {
                Roll(ref player1Position, ref player1Score, ref timesRolled, ref deterministicDice);

                if (player1Score < maxScore)
                {
                    Roll(ref player2Position, ref player2Score, ref timesRolled, ref deterministicDice);
                }
            }

            int product = Math.Min(player1Score, player2Score) * timesRolled;
            return product.ToString();
        }

        public string Part2(string input)
        {
            const int maxScore = 21;
            long winnerUniversesP1 = 0;
            long winnerUniversesP2 = 0;

            List<(int rollValue, int numUniverses)> newRolls = new() {
                    ( 3, 1 ),
                    ( 4, 3 ),
                    ( 5, 6 ),
                    ( 6, 7 ),
                    ( 7, 6 ),
                    ( 8, 3 ),
                    ( 9, 1 ),
                };


            (int player1StartingPosition, int player2StartingPosition) = GetStartingPositions(input);
            CommonUniverses startingUniverse = new CommonUniverses(player1StartingPosition, player2StartingPosition);
            Dictionary<CommonUniverses, long> multiverse = new() { { startingUniverse, 1 } };

            while (multiverse.Any(d => d.Value > 0))
            {
                Dictionary<CommonUniverses, long> newMultiverseAfterRoll1 = new();
                Dictionary<CommonUniverses, long> newMultiverseAfterRoll2 = new();

                foreach (var rollPlayer1 in newRolls)
                {
                    foreach (var kvpUniverses in multiverse)
                    {
                        long newUniverses = kvpUniverses.Value * rollPlayer1.numUniverses;
                        int newPositionP1 = (kvpUniverses.Key.Player1Position + rollPlayer1.rollValue - 1) % BOARD_SPACES + 1;
                        int newScoreP1 = kvpUniverses.Key.Player1Score + newPositionP1;
                        CommonUniverses commonUniverses = new CommonUniverses(newPositionP1, kvpUniverses.Key.Player2Position, newScoreP1, kvpUniverses.Key.Player2Score);

                        if (newScoreP1 >= maxScore)
                        {
                            winnerUniversesP1 += newUniverses;
                        }
                        else
                        {
                            newMultiverseAfterRoll1.TryAdd(commonUniverses, 0);
                            newMultiverseAfterRoll1[commonUniverses] += newUniverses;
                        }
                    }
                }

                foreach (var rollPlayer2 in newRolls)
                {
                    foreach (var kvpUniverses in newMultiverseAfterRoll1)
                    {
                        long newUniverses = kvpUniverses.Value * rollPlayer2.numUniverses;
                        int newPositionP2 = (kvpUniverses.Key.Player2Position + rollPlayer2.rollValue - 1) % BOARD_SPACES + 1;
                        int newScoreP2 = kvpUniverses.Key.Player2Score + newPositionP2;
                        CommonUniverses commonUniverses = new CommonUniverses(kvpUniverses.Key.Player1Position, newPositionP2, kvpUniverses.Key.Player1Score, newScoreP2);

                        if (newScoreP2 >= maxScore)
                        {
                            winnerUniversesP2 += newUniverses;
                        }
                        else
                        {
                            newMultiverseAfterRoll2.TryAdd(commonUniverses, 0);
                            newMultiverseAfterRoll2[commonUniverses] += newUniverses;
                        }
                    }
                }

                multiverse = newMultiverseAfterRoll2;
            }

            return Math.Max(winnerUniversesP1, winnerUniversesP2).ToString();
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