using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Utils;

namespace AdventOfCode.Year2022.Day08
{
    [Problem(Year = 2022, Day = 8, ProblemName = "Treetop Tree House")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input)
        {
            int[,] treeMap = InputUtils.ParseIntMatrix(input);

            int edgeTrees = treeMap.GetLength(0) * 2 + treeMap.GetLength(1) * 2 - 4;
            int visibleInteriorTrees = 0;

            for (int y = 1; y < treeMap.GetLength(0) - 1; y++)
            {
                for (int x = 1; x < treeMap.GetLength(1) - 1; x++)
                {
                    if (IsTreeVisible(treeMap, y, x))
                        visibleInteriorTrees++;
                }
            }

			return (edgeTrees + visibleInteriorTrees).ToString();
        }

        public string Part2(string input)
        {
            int[,] treeMap = InputUtils.ParseIntMatrix(input);
            int maxScenicScore = 0;

            for (int y = 0; y < treeMap.GetLength(0) - 1; y++)
            {
                for (int x = 0; x < treeMap.GetLength(1) - 1; x++)
                {
                    maxScenicScore = Math.Max(maxScenicScore, GetScenicScore(treeMap, y, x));
                }
            }

            return maxScenicScore.ToString();
        }


        private static int GetScenicScore(int[,] treeMap, int y, int x) => 
            GetScenicScoreFromDirection(treeMap, y, x, Direction.Top)
                * GetScenicScoreFromDirection(treeMap, y, x, Direction.Right)
                * GetScenicScoreFromDirection(treeMap, y, x, Direction.Bottom)
                * GetScenicScoreFromDirection(treeMap, y, x, Direction.Left);

        private static int GetScenicScoreFromDirection(int[,] treeMap, int y, int x, Direction direction)
        {
            int treeHeight = treeMap[y, x];
            int visibleTrees = 0;

            foreach (int currentHeight in GetElementsInDirection(treeMap, y, x, direction))
            {
                visibleTrees++;

                if (currentHeight >= treeHeight)
                    break;
            }

            return visibleTrees;
        }

        private static bool IsTreeVisible(int[,] treeMap, int y, int x) => 
            IsTreeVisibleFromDirection(treeMap, y, x, Direction.Top)
                || IsTreeVisibleFromDirection(treeMap, y, x, Direction.Right)
                || IsTreeVisibleFromDirection(treeMap, y, x, Direction.Bottom)
                || IsTreeVisibleFromDirection(treeMap, y, x, Direction.Left);

        private static bool IsTreeVisibleFromDirection(int[,] treeMap, int y, int x, Direction direction)
        {
            int treeHeight = treeMap[y, x];

            foreach (int currentHeight in GetElementsInDirection(treeMap, y, x, direction))
            {
                if (currentHeight >= treeHeight)
                {
                    return false;
                }
            }

            return true;
        }

        private static IEnumerable<int> GetElementsInDirection(int[,] treeMap, int y, int x, Direction direction)
        {
            (int yModifier, int xModifier) = direction switch 
            {
                Direction.Top       => (-1, 0),
                Direction.Right     => (0, 1),
                Direction.Bottom    => (1, 0),
                Direction.Left      => (0, -1),
                _                   => throw new InvalidOperationException()
            };

            int newY = y + yModifier;
            int newX = x + xModifier;

            while (newX >= 0 && newY >= 0 && newX < treeMap.GetLength(1) && newY < treeMap.GetLength(0))
            {
                yield return (treeMap[newY, newX]);

                newY += yModifier;
                newX += xModifier;
            }
        }


        private enum Direction
        {
            Top,
            Right,
            Bottom,
            Left,
        }
    }
}