using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day4
{
    class Program
    {
        private static readonly int TAMAÑO_FILA_COLUMNA = 5;


        public class Casilla
        {
            public int Numero { get; set; }
            public bool Marcado { get; set; }
        }


        public class Board
        {
            public Casilla[,] Casillas { get; set; }
            public bool Marcado { get; set; }
        }


        static void Main(string[] args)
        {
            (List<int> numeros, List<Casilla[,]> boards) = GetInputs();

            Puzle1(numeros, boards);
            Puzle2(numeros, boards);
        }


        private static void Puzle1(List<int> numeros, List<Casilla[,]> boards)
        {
            foreach (int numero in numeros)
            {
                foreach (Casilla[,] board in boards)
                {
                    for (int i = 0; i < TAMAÑO_FILA_COLUMNA; i++)
                    {
                        for (int j = 0; j < TAMAÑO_FILA_COLUMNA; j++)
                        {
                            if (board[i, j].Numero == numero)
                            {
                                board[i, j].Marcado = true;

                                if (ComprobarFila(board, i) || ComprobarColumna(board, j))
                                {
                                    Console.WriteLine("Puzle 1:");
                                    CalcularPuntuacion(board, numero);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }


        private static void Puzle2(List<int> numeros, List<Casilla[,]> casillas)
        {
            List<Board> boards = casillas.Select(c => new Board { Casillas = c, Marcado = false }).ToList();

            foreach (int numero in numeros)
            {
                foreach (Board board in boards)
                {
                    for (int i = 0; i < TAMAÑO_FILA_COLUMNA; i++)
                    {
                        for (int j = 0; j < TAMAÑO_FILA_COLUMNA; j++)
                        {
                            if (board.Casillas[i, j].Numero == numero)
                            {
                                board.Casillas[i, j].Marcado = true;

                                if (ComprobarFila(board.Casillas, i) || ComprobarColumna(board.Casillas, j))
                                {
                                    board.Marcado = true;

                                    if (boards.All(b => b.Marcado))
                                    {
                                        Console.WriteLine("Puzle 2:");
                                        CalcularPuntuacion(board.Casillas, numero);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        private static void CalcularPuntuacion(Casilla[,] board, int numero)
        {
            int sumaNumsNoMarcados = 0;

            for (int i = 0; i < TAMAÑO_FILA_COLUMNA; i++)
            {
                for (int j = 0; j < TAMAÑO_FILA_COLUMNA; j++)
                {
                    if (!board[i, j].Marcado)
                    {
                        sumaNumsNoMarcados += board[i, j].Numero;
                    }
                }
            }

            Console.WriteLine($"Suma números no marcados: {sumaNumsNoMarcados}, último número: {numero}, producto: {sumaNumsNoMarcados * numero}");
        }


        private static (List<int> numeros, List<Casilla[,]> boards) GetInputs()
        {
            string input =
                File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "inputs" + Path.DirectorySeparatorChar + "input.txt");

            string[] filas = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            List<int> numeros = filas.First().Split(',').Select(n => int.Parse(n)).ToList();

            Casilla[,] board = new Casilla[TAMAÑO_FILA_COLUMNA, TAMAÑO_FILA_COLUMNA];
            int filaBoard = 0;
            List<Casilla[,]> boards = new();

            foreach (string fila in filas.Skip(1))
            {
                int[] numsFila = fila.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(n => int.Parse(n)).ToArray();

                board[filaBoard, 0] = new Casilla { Numero = numsFila[0], Marcado = false };
                board[filaBoard, 1] = new Casilla { Numero = numsFila[1], Marcado = false };
                board[filaBoard, 2] = new Casilla { Numero = numsFila[2], Marcado = false };
                board[filaBoard, 3] = new Casilla { Numero = numsFila[3], Marcado = false };
                board[filaBoard, 4] = new Casilla { Numero = numsFila[4], Marcado = false };

                if (++filaBoard == TAMAÑO_FILA_COLUMNA)
                {
                    boards.Add(board);
                    board = new Casilla[TAMAÑO_FILA_COLUMNA, TAMAÑO_FILA_COLUMNA];
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

    }
}