using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2021.Day04
{
    [Problem(Year = 2021, Day = 4, ProblemName = "Giant Squid")]
    internal class Problem : IProblem
    {
        private static readonly int TAMANYO_FILA_COLUMNA = 5;

        public bool Debug { get; set; } = false;


        public string Part1(string input)
        {
            (List<int> numeros, List<Casilla[,]> boards) = GetInputs(input);

            return GetSolutionPart1(numeros, boards).ToString();
        }

        public string Part2(string input)
        {
            (List<int> numeros, List<Casilla[,]> boards) = GetInputs(input);

            return GetSolutionPart2(numeros, boards).ToString();
        }


        private int GetSolutionPart1(List<int> numeros, List<Casilla[,]> boards)
        {
            foreach (int numero in numeros)
            {
                foreach (Casilla[,] board in boards)
                {
                    for (int i = 0; i < TAMANYO_FILA_COLUMNA; i++)
                    {
                        for (int j = 0; j < TAMANYO_FILA_COLUMNA; j++)
                        {
                            if (board[i, j].Numero == numero)
                            {
                                board[i, j].Marcado = true;

                                if (ComprobarFila(board, i) || ComprobarColumna(board, j))
                                {
                                    return CalcularPuntuacion(board, numero);
                                }
                            }
                        }
                    }
                }
            }

            return -1;
        }


        private int GetSolutionPart2(List<int> numeros, List<Casilla[,]> casillas)
        {
            List<Board> boards = casillas.Select(c => new Board { Casillas = c, Marcado = false }).ToList();

            foreach (int numero in numeros)
            {
                foreach (Board board in boards)
                {
                    for (int i = 0; i < TAMANYO_FILA_COLUMNA; i++)
                    {
                        for (int j = 0; j < TAMANYO_FILA_COLUMNA; j++)
                        {
                            if (board.Casillas != null && board.Casillas[i, j].Numero == numero)
                            {
                                board.Casillas[i, j].Marcado = true;

                                if (ComprobarFila(board.Casillas, i) || ComprobarColumna(board.Casillas, j))
                                {
                                    board.Marcado = true;

                                    if (boards.All(b => b.Marcado))
                                    {
                                        return CalcularPuntuacion(board.Casillas, numero);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return -1;
        }


        private int CalcularPuntuacion(Casilla[,] board, int numero)
        {
            int sumaNumsNoMarcados = 0;

            for (int i = 0; i < TAMANYO_FILA_COLUMNA; i++)
            {
                for (int j = 0; j < TAMANYO_FILA_COLUMNA; j++)
                {
                    if (!board[i, j].Marcado)
                    {
                        sumaNumsNoMarcados += board[i, j].Numero;
                    }
                }
            }

            int producto = sumaNumsNoMarcados * numero;

            if (Debug)
                Console.WriteLine($"Suma números no marcados: {sumaNumsNoMarcados}, último número: {numero}, producto: {producto}");

            return producto;
        }


        private static (List<int> numeros, List<Casilla[,]> boards) GetInputs(string input)
        {
            string[] filas = input.GetLines(StringSplitOptions.RemoveEmptyEntries);

            List<int> numeros = filas.First().Split(',').Select(n => int.Parse(n)).ToList();

            Casilla[,] board = new Casilla[TAMANYO_FILA_COLUMNA, TAMANYO_FILA_COLUMNA];
            int filaBoard = 0;
            List<Casilla[,]> boards = [];

            foreach (string fila in filas.Skip(1))
            {
                int[] numsFila = fila.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(n => int.Parse(n)).ToArray();

                board[filaBoard, 0] = new Casilla { Numero = numsFila[0], Marcado = false };
                board[filaBoard, 1] = new Casilla { Numero = numsFila[1], Marcado = false };
                board[filaBoard, 2] = new Casilla { Numero = numsFila[2], Marcado = false };
                board[filaBoard, 3] = new Casilla { Numero = numsFila[3], Marcado = false };
                board[filaBoard, 4] = new Casilla { Numero = numsFila[4], Marcado = false };

                if (++filaBoard == TAMANYO_FILA_COLUMNA)
                {
                    boards.Add(board);
                    board = new Casilla[TAMANYO_FILA_COLUMNA, TAMANYO_FILA_COLUMNA];
                    filaBoard = 0;
                }
            }

            return (numeros, boards);
        }


        public static Casilla[] GetColumn(Casilla[,] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(0))
                    .Select(x => matrix[x, columnNumber])
                    .ToArray();
        }


        public static Casilla[] GetRow(Casilla[,] matrix, int rowNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                    .Select(x => matrix[rowNumber, x])
                    .ToArray();
        }


        private static bool ComprobarFila(Casilla[,] matrix, int numFila)
        {
            Casilla[] fila = GetRow(matrix, numFila);

            return fila.All(c => c.Marcado);
        }


        private static bool ComprobarColumna(Casilla[,] matrix, int numColumna)
        {
            Casilla[] columna = GetColumn(matrix, numColumna);

            return columna.All(c => c.Marcado);
        }


        public class Casilla
        {
            public int Numero { get; set; }
            public bool Marcado { get; set; }
        }


        public class Board
        {
            public Casilla[,]? Casillas { get; set; }
            public bool Marcado { get; set; }
        }
    }
}