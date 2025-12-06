using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2025.Day04
{
    [Problem(Year = 2025, Day = 4, ProblemName = "Printing Department")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input) => GetAccessibleRolls(GetDiagram(input)).ToString();

        public string Part2(string input)
        {
            char[,] diagram = GetDiagram(input);

            int totalAccessibleRolls = 0;
            int accessibleRolls;

            do
            {
                accessibleRolls = GetAccessibleRolls(diagram);
                totalAccessibleRolls += accessibleRolls;
            } while (accessibleRolls > 0);

            return totalAccessibleRolls.ToString();
        }


        private static readonly char ROLL_CHAR = '@';
        private static readonly char ROLL_DELETED_CHAR = 'x';

        private static int GetAccessibleRolls(char[,] diagram)
        {
            int accessibleRolls = 0;

            for (int y = 0; y < diagram.GetLength(0); y++)
            {
                for (int x = 0; x < diagram.GetLength(1); x++)
                {
                    if (diagram[y, x] == ROLL_CHAR && GetAdjacentRollsCount(diagram, y, x) < 4)
                    {
                        accessibleRolls++;
                        diagram[y, x] = ROLL_DELETED_CHAR;
                    }
                }
            }

            return accessibleRolls;
        }

        private static char[,] GetDiagram(string input)
        {
            string[] lines = input.GetLines();
            char[,] diagram = new char[lines.Length, lines[0].Length];

            for (int y = 0; y < diagram.GetLength(0); y++)
                for (int x = 0; x < diagram.GetLength(1); x++)
                    diagram[y, x] = lines[y][x];
            
            return diagram;
        }

        private static int GetAdjacentRollsCount(char[,] diagram, int y, int x)
        {
            return diagram.GetAdjacents(x, y).Count(adj => adj.value == ROLL_CHAR);
        }
    }
}