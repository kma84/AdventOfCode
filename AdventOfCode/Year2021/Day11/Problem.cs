using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;
using System.Text;

namespace AdventOfCode.Year2021.Day11
{
    [Problem(Year = 2021, Day = 11, ProblemName = "Dumbo Octopus")]
    internal class Problem : IProblem
    {
        const int GRID_SIZE = 10;
        const int OCTOPUS_MAX_ENERGY = 9;
        
        public static bool LoggerDetailed => false;

        public string Part1(string input)
        {
            Octopus[,] grid = GetGrid(input);

            if (LoggerDetailed)
                MostrarGrid(grid, "step0");

            int numFlashes = 0;
            int numPasos = 100;
            for (int i = 1; i <= numPasos; i++)
            {
                numFlashes += AvanzarGrid(grid);
                if (LoggerDetailed)
                    MostrarGrid(grid, "step" + i);
            }

            return numFlashes.ToString();
        }

        public string Part2(string input)
        {
            Octopus[,] grid = GetGrid(input);
            int numPasos = 0;

            while (!FlashSimultaneo(grid))
            {
                AvanzarGrid(grid);
                numPasos++;
            }

            return numPasos.ToString();
        }


        private static bool FlashSimultaneo(Octopus[,] grid)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                for (int x = 0; x < GRID_SIZE; x++)
                {
                    if (grid[y, x].Energy > 0)
                        return false;
                }
            }

            return true;
        }


        private static int AvanzarGrid(Octopus[,] grid)
        {
            int numFlashes = 0;

            for (int y = 0; y < GRID_SIZE; y++)
            {
                for (int x = 0; x < GRID_SIZE; x++)
                {
                    grid[y, x].Energy++;
                }
            }

            while (PulposPorFlashear(grid))
            {
                for (int y = 0; y < GRID_SIZE; y++)
                {
                    for (int x = 0; x < GRID_SIZE; x++)
                    {
                        if (!grid[y, x].FlashEnEsteTurno && grid[y, x].Energy > OCTOPUS_MAX_ENERGY)
                        {
                            numFlashes++;
                            grid[y, x].Energy = 0;
                            grid[y, x].FlashEnEsteTurno = true;

                            // Aumentamos la energía de los pulpos adyacentes
                            if (y - 1 >= 0 && x - 1 >= 0 && !grid[y - 1, x - 1].FlashEnEsteTurno)
                                grid[y - 1, x - 1].Energy++;

                            if (y - 1 >= 0 && !grid[y - 1, x].FlashEnEsteTurno)
                                grid[y - 1, x].Energy++;

                            if (y - 1 >= 0 && x + 1 < GRID_SIZE && !grid[y - 1, x + 1].FlashEnEsteTurno)
                                grid[y - 1, x + 1].Energy++;

                            if (x - 1 >= 0 && !grid[y, x - 1].FlashEnEsteTurno)
                                grid[y, x - 1].Energy++;

                            if (x + 1 < GRID_SIZE && !grid[y, x + 1].FlashEnEsteTurno)
                                grid[y, x + 1].Energy++;

                            if (y + 1 < GRID_SIZE && x - 1 >= 0 && !grid[y + 1, x - 1].FlashEnEsteTurno)
                                grid[y + 1, x - 1].Energy++;

                            if (y + 1 < GRID_SIZE && !grid[y + 1, x].FlashEnEsteTurno)
                                grid[y + 1, x].Energy++;

                            if (y + 1 < GRID_SIZE && x + 1 < GRID_SIZE && !grid[y + 1, x + 1].FlashEnEsteTurno)
                                grid[y + 1, x + 1].Energy++;
                        }
                    }
                }
            }

            LimpiarFlashes(grid);

            return numFlashes;
        }


        private static bool PulposPorFlashear(Octopus[,] grid)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                for (int x = 0; x < GRID_SIZE; x++)
                {
                    if (grid[y, x].Energy > OCTOPUS_MAX_ENERGY)
                        return true;
                }
            }

            return false;
        }


        private static void LimpiarFlashes(Octopus[,] grid)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                for (int x = 0; x < GRID_SIZE; x++)
                {
                    grid[y, x].FlashEnEsteTurno = false;
                }
            }
        }


        private static Octopus[,] GetGrid(string input)
        {
            string[] filas = input.GetLines();

            Octopus[,] grid = new Octopus[GRID_SIZE, GRID_SIZE];

            for (int y = 0; y < GRID_SIZE; y++)
            {
                for (int x = 0; x < GRID_SIZE; x++)
                {
                    grid[y, x] = new Octopus { Energy = (int)char.GetNumericValue(filas[y][x]) };
                }
            }

            return grid;
        }


        private static void MostrarGrid(Octopus[,] grid, string titulo)
        {
            StringBuilder sb = new();

            for (int y = 0; y < GRID_SIZE; y++)
            {
                for (int x = 0; x < GRID_SIZE; x++)
                {
                    sb.Append(grid[y, x].Energy);
                }

                sb.AppendLine();
            }

            Console.WriteLine(titulo);
            Console.WriteLine(sb.ToString());
        }


        private class Octopus
        {
            public int Energy { get; set; }

            public bool FlashEnEsteTurno { get; set; } = false;
        }
    }
}