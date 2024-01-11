using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;

namespace AdventOfCode.Year2021.Day09
{
    [Problem(Year = 2021, Day = 9, ProblemName = "Smoke Basin")]
    internal class Problem : IProblem
    {
        private readonly static int MAX_HEIGHT = 9;
        public bool Debug { get; set; } = false;

        public string Part1(string input)
        {
            int totalRiesgos = 0;
            int[,] heightmap = InputUtils.ParseIntMatrix(input);

            List<(int x, int y)> puntosBajos = GetPuntosBajos(heightmap);

            foreach ((int y, int x) in puntosBajos)
            {
                totalRiesgos += heightmap[y, x] + 1;
            }

            if (Debug)
                Console.WriteLine($"Puzle1. Suma de los niveles de riesgo: {totalRiesgos}");

            return totalRiesgos.ToString();
        }

        public string Part2(string input)
        {
            List<(int, int)> puntosComprobados = [];
            List<int> basinSizes = []; 
            
            int[,] heightmap = InputUtils.ParseIntMatrix(input);
            List<(int x, int y)> puntosBajos = GetPuntosBajos(heightmap);

            foreach ((int, int) punto in puntosBajos)
            {
                basinSizes.Add(ComprobarPuntosAdyacentes(heightmap, punto, puntosComprobados));
            }

            int producto = basinSizes.OrderByDescending(b => b).Take(3).Aggregate(0, (total, next) => total == 0 ? next : total * next);

            if (Debug)
                Console.WriteLine($"Puzle2. producto de las 3 vaguadas más grandes: {producto}");

            return producto.ToString();
        }


        private static List<(int x, int y)> GetPuntosBajos(int[,] mapa)
        {
            int maxY = mapa.GetLength(0);
            int maxX = mapa.GetLength(1);

            List<(int x, int y)> puntosBajos = [];

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    // Arriba
                    if (y - 1 >= 0 && mapa[y - 1, x] <= mapa[y, x])
                        continue;

                    // Abajo
                    if (y + 1 < maxY && mapa[y + 1, x] <= mapa[y, x])
                        continue;

                    // Derecha
                    if (x - 1 >= 0 && mapa[y, x - 1] <= mapa[y, x])
                        continue;

                    // Izquierda
                    if (x + 1 < maxX && mapa[y, x + 1] <= mapa[y, x])
                        continue;

                    puntosBajos.Add((y, x));
                }
            }

            return puntosBajos;
        }


        private static int ComprobarPuntosAdyacentes(int[,] mapa, (int, int) punto, List<(int, int)> puntosComprobados)
        {
            int maxY = mapa.GetLength(0);
            int maxX = mapa.GetLength(1);

            int basinSize = 1;
            puntosComprobados.Add(punto);
            (int y, int x) = punto;

            // Arriba
            if (y - 1 >= 0 && mapa[y - 1, x] < MAX_HEIGHT && !puntosComprobados.Contains((y - 1, x)))
            {
                basinSize += ComprobarPuntosAdyacentes(mapa, (y - 1, x), puntosComprobados);
            }

            // Abajo
            if (y + 1 < maxY && mapa[y + 1, x] < MAX_HEIGHT && !puntosComprobados.Contains((y + 1, x)))
            {
                basinSize += ComprobarPuntosAdyacentes(mapa, (y + 1, x), puntosComprobados);
            }

            // Derecha
            if (x - 1 >= 0 && mapa[y, x - 1] < MAX_HEIGHT && !puntosComprobados.Contains((y, x - 1)))
            {
                basinSize += ComprobarPuntosAdyacentes(mapa, (y, x - 1), puntosComprobados);
            }

            // Izquierda
            if (x + 1 < maxX && mapa[y, x + 1] < MAX_HEIGHT && !puntosComprobados.Contains((y, x + 1)))
            {
                basinSize += ComprobarPuntosAdyacentes(mapa, (y, x + 1), puntosComprobados);
            }

            return basinSize;
        }

    }
}