using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace dia9
{
    class Program
    {
        private readonly static int MAX_HEIGHT = 9;


        static void Main(string[] args)
        {
            int[,] heightmap = GetMapa();
            //int[,] heightmap = GetTestMapa();

            List<(int x, int y)> puntosBajos = GetPuntosBajos(heightmap);

            Puzle1(heightmap, puntosBajos);
            Puzle2(heightmap, puntosBajos);
        }


        private static int[,] GetTestMapa()
        {
            return new int[,] { 
                { 2,1,9,9,9,4,3,2,1,0 }, 
                { 3,9,8,7,8,9,4,9,2,1 }, 
                { 9,8,5,6,7,8,9,8,9,2 }, 
                { 8,7,6,7,8,9,6,7,8,9 },
                { 9,8,9,9,9,6,5,6,7,8 }
            };
        }


        private static List<(int x, int y)> GetPuntosBajos(int[,] mapa)
        {
            int maxY = mapa.GetLength(0);
            int maxX = mapa.GetLength(1);

            List<(int x, int y)> puntosBajos = new();

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


        private static void Puzle1(int[,] mapa, List<(int, int)> puntosBajos)
        {
            int totalRiesgos = 0;

            foreach ((int y, int x) in puntosBajos)
            {
                totalRiesgos += mapa[y, x] + 1;
            }            

            Console.WriteLine($"Puzle1. Suma de los niveles de riesgo: {totalRiesgos}");
        }


        private static void Puzle2(int[,] mapa, List<(int, int)> puntosBajos)
        {
            List<(int, int)> puntosComprobados = new();
            List<int> basinSizes = new();

            foreach ((int, int) punto in puntosBajos)
            {
                basinSizes.Add(ComprobarPuntosAdyacentes(mapa, punto, puntosComprobados));
            }

            int producto = basinSizes.OrderByDescending(b => b).Take(3).Aggregate(0, (total, next) => total == 0 ? next : total * next);

            Console.WriteLine($"Puzle2. producto de las 3 vaguadas más grandes: {producto}");
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


        private static int[,] GetMapa()
        {
            string input =
                File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "inputs" + Path.DirectorySeparatorChar + "input.txt");
            
            string[] filas = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            int maxX = filas.First().Length;
            int maxY = filas.Length;
            int[,] mapa = new int[maxY, maxX];

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    mapa[y, x] = (int)char.GetNumericValue(filas[y][x]);
                }
            }

            return mapa;
        }
    }
}
