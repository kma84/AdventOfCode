using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;
using System.Text;

namespace AdventOfCode.Year2021.Day13
{
    [Problem(Year = 2021, Day = 13, ProblemName = "Transparent Origami")]
    internal class Problem : IProblem
    {
        private const char POINT_CHAR = '#';
        private const char VOID_CHAR = '.';

        public bool Debug { get; set; } = false;


        public string Part1(string input)
        {
            (char[,] paper, var folds) = GetInput(input);

            if (Debug)
                MostrarPaper(paper, "paper inicial");

            return GetNumOfDots(GetPaper(paper, folds[0], 1)).ToString();
        }

        public string Part2(string input)
        {
            (char[,] paper, var folds) = GetInput(input);

            int i = 0;
            foreach (var fold in folds)
            {
                paper = GetPaper(paper, fold, i);
                i++;
            }

            return GetNumOfDots(paper).ToString();
        }


        private char[,] GetPaper(char[,] paper, (FoldDirection direction, int value) fold, int i)
        {
            int maxY = fold.direction == FoldDirection.HORIZONTAL ? fold.value : paper.GetLength(0);
            int maxX = fold.direction == FoldDirection.VERTICAL ? fold.value : paper.GetLength(1);

            char[,] newPaper = new char[maxY, maxX];

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    int foldY = fold.direction == FoldDirection.HORIZONTAL ? paper.GetLength(0) - 1 - y : y;
                    int foldX = fold.direction == FoldDirection.VERTICAL ? paper.GetLength(1) - 1 - x : x;

                    newPaper[y, x] = GetCharPoint(paper[y, x], paper[foldY, foldX]);
                }
            }

            if (Debug)
            {
                MostrarPaper(newPaper, $"Fold{i}.");
                Console.WriteLine($"Fold{i}. Num of dots {GetNumOfDots(newPaper)}");
            }

            return newPaper;
        }

        private static (char[,] paper, List<(FoldDirection direction, int value)> folds) GetInput(string input)
        {
            List<string> lines = input.GetLines().ToList();

            // Coordinates
            var coordinates = new List<(int x, int y)>();
            coordinates = lines.Take(lines.IndexOf(string.Empty)).Select(l => {
                string[] coordStr = l.Split(',');
                return (int.Parse(coordStr[0]), int.Parse(coordStr[1]));
            }).ToList();

            char[,] paper = new char[coordinates.Max(c => c.y) + 1, coordinates.Max(c => c.x) + 1];

            for (int y = 0; y < paper.GetLength(0); y++)
            {
                for (int x = 0; x < paper.GetLength(1); x++)
                {
                    paper[y, x] = VOID_CHAR;
                }
            }

            foreach ((int x, int y) in coordinates)
                paper[y, x] = POINT_CHAR;

            // Folds
            IEnumerable<string> foldsStr = lines.Skip(coordinates.Count + 1).TakeWhile(l => l != string.Empty);

            var folds = new List<(FoldDirection direction, int value)>();

            foreach (string foldStr in foldsStr)
            {
                string[] foldSplit = foldStr.Split(' ', '=');
                FoldDirection direction = foldSplit[2][0] == 'x' ? FoldDirection.VERTICAL : FoldDirection.HORIZONTAL;

                folds.Add((direction, int.Parse(foldSplit[3])));
            }

            return (paper, folds);
        }

        private static int GetNumOfDots(char[,] paper)
        {
            int numDots = 0;

            foreach (char c in paper)
            {
                if (c == POINT_CHAR)
                {
                    numDots++;
                }
            }

            return numDots;
        }


        private static char GetCharPoint(char c1, char c2)
        {
            return new[] { c1, c2 }.Contains(POINT_CHAR) ? POINT_CHAR : VOID_CHAR;
        }


        private static void MostrarPaper(char[,] paper, string titulo)
        {
            StringBuilder sb = new();

            for (int y = 0; y < paper.GetLength(0); y++)
            {
                for (int x = 0; x < paper.GetLength(1); x++)
                {
                    sb.Append(paper[y, x]);
                }

                sb.AppendLine();
            }

            Console.WriteLine(titulo);
            Console.WriteLine(sb.ToString());
        }


        private enum FoldDirection
        {
            HORIZONTAL,
            VERTICAL
        }
    }
}