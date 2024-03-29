using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2021.Day15
{
    [Problem(Year = 2021, Day = 15, ProblemName = "Chiton")]
    internal class Problem : IProblem
    {
        public bool Debug { get; set; } = false;


        public string Part1(string input)
        {
            Node[,] matrix = GetInput(input);

            // Seg�n el enunciado en el nodo inicial no se cuenta el riesgo
            matrix[0, 0].Risk = 0;

            List<(Node startNode, Node endNode, long cost)> edges = GetEdges(matrix);

            // Dijkstra
            //var path = GraphExtensions.DijkstraShortestPath(edges, matrix[0, 0], matrix[matrix.GetLength(0) - 1, matrix.GetLength(1) - 1]);

            // A Star
            long GetEstimatedCost(Node node) => matrix.GetLength(0) - node.MatrixCoordinateY + matrix.GetLength(1) - node.MatrixCoordinateX - 2;
            var path = GraphExtensions.AStarShortestPath(edges, matrix[0, 0], matrix[matrix.GetLength(0) - 1, matrix.GetLength(1) - 1], GetEstimatedCost);

            path.ForEach(n => n.Selected = true);

            if (Debug)
            {
                matrix.Print();
                Console.WriteLine("Part1: The total risk of this path is " + path.Sum(n => n.Risk));
            }            

            return path.Sum(n => n.Risk).ToString();
        }

        public string Part2(string input)
        {
            int GetNewRisk(int baseRisk, int tileX, int tileY) => ((baseRisk - 1 + tileY + tileX) % 9) + 1;

            const int numTiles = 5;
            Node[,] matrix = GetInput(input);

            Node[,] newMatrix = new Node[matrix.GetLength(0) * numTiles, matrix.GetLength(1) * numTiles];

            for (int tileY = 0; tileY < numTiles; tileY++)
            {
                for (int tileX = 0; tileX < numTiles; tileX++)
                {
                    for (int y = 0; y < matrix.GetLength(0); y++)
                    {
                        for (int x = 0; x < matrix.GetLength(1); x++)
                        {
                            int newY = y + tileY * matrix.GetLength(0);
                            int newX = x + tileX * matrix.GetLength(1);

                            newMatrix[newY, newX] = new Node(newX, newY, GetNewRisk(matrix[y, x].Risk, tileX, tileY));
                        }
                    }
                }
            }

            // Seg�n el enunciado en el nodo inicial no se cuenta el riesgo
            newMatrix[0, 0].Risk = 0;

            List<(Node startNode, Node endNode, long cost)> edges = GetEdges(newMatrix);

            // Dijkstra
            //var path = GraphExtensions.DijkstraShortestPath(edges, newMatrix[0, 0], newMatrix[newMatrix.GetLength(0) - 1, newMatrix.GetLength(1) - 1]);

            // A Star
            long GetEstimatedCost(Node node) => newMatrix.GetLength(0) - node.MatrixCoordinateY + newMatrix.GetLength(1) - node.MatrixCoordinateX - 2;
            Node target = newMatrix[newMatrix.GetLength(0) - 1, newMatrix.GetLength(1) - 1];
            var path = GraphExtensions.AStarShortestPath(edges, newMatrix[0, 0], target, GetEstimatedCost);

            path.ForEach(n => n.Selected = true);

            if (Debug)
            {
                newMatrix.Print();
                Console.WriteLine("Part2: The total risk of this path is " + path.Sum(n => n.Risk));
            }

            return path.Sum(n => n.Risk).ToString();
        }



        private static Node[,] GetInput(string input)
        {
            string[] rows = input.GetLines(StringSplitOptions.RemoveEmptyEntries);

            Node[,] matrix = new Node[rows.Length, rows.First().Length];

            for (int y = 0; y < matrix.GetLength(0); y++)
            {
                for (int x = 0; x < matrix.GetLength(1); x++)
                {
                    matrix[y, x] = new Node(x, y, (int)char.GetNumericValue(rows[y][x]));
                }
            }

            return matrix;
        }


        private static List<(Node startNode, Node endNode, long cost)> GetEdges(Node[,] matrix)
        {
            List<(Node startNode, Node endNode, long cost)> edges = [];

            for (int y = 0; y < matrix.GetLength(0); y++)
            {
                for (int x = 0; x < matrix.GetLength(1); x++)
                {
                    foreach (Node adjacent in matrix.GetCrossAdjacents(x, y).Select(a => a.value))
                    {
                        edges.Add((matrix[y, x], adjacent, matrix[y, x].Risk));
                    }
                }
            }

            return edges;
        }


        private class Node(int matrixCoordinateX, int matrixCoordinateY, int risk)
        {
            public int MatrixCoordinateX { get; set; } = matrixCoordinateX;
            public int MatrixCoordinateY { get; set; } = matrixCoordinateY;
            public int Risk { get; set; } = risk;
            public bool Selected { get; set; } = false;

            public override string ToString()
            {
                string baseStr = Selected ? "[{0}]" : " {0} ";

                return string.Format(baseStr, Risk);
            }
        }
    }
}