using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2021.Day05
{
    [Problem(Year = 2021, Day = 5, ProblemName = "Hydrothermal Venture")]
    internal class Problem : IProblem
    {
        public bool Debug { get; set; } = false;

        public string Part1(string input)
        {
            return CalcularPuntosCoincidentes(GetLineas(input), filtrarLineasVerticalesYHorizontales: true).ToString();
        }

        public string Part2(string input)
        {
            return CalcularPuntosCoincidentes(GetLineas(input), filtrarLineasVerticalesYHorizontales: false).ToString();
        }


        private static List<Linea> GetLineas(string input)
        {
            string[] filas = input.GetLines(StringSplitOptions.RemoveEmptyEntries);

            List<Linea> lineas = new();

            foreach (string fila in filas)
            {
                int[] coordenadas = fila.Split(new[] { ",", " -> " }, StringSplitOptions.RemoveEmptyEntries).Select(c => int.Parse(c)).ToArray();

                lineas.Add(new Linea
                (
                    puntoInicial: new Punto { X = coordenadas[0], Y = coordenadas[1] },
                    puntoFinal: new Punto { X = coordenadas[2], Y = coordenadas[3] },
                    puntos: new List<Punto>()
                ));
            }

            return lineas;
        }


        private int CalcularPuntosCoincidentes(List<Linea> lineas, bool filtrarLineasVerticalesYHorizontales)
        {
            List<Linea> lineasCalculo = new();

            lineas.ForEach((l) =>
            {
                lineasCalculo.Add(new Linea
                (
                    puntoInicial: new Punto { X = l.PuntoInicial.X, Y = l.PuntoInicial.Y },
                    puntoFinal: new Punto { X = l.PuntoFinal.X, Y = l.PuntoFinal.Y },
                    puntos: new()
                ));
            });

            int maxX = Math.Max(lineasCalculo.Max(l => l.PuntoInicial.X), lineasCalculo.Max(l => l.PuntoFinal.X));
            int maxY = Math.Max(lineasCalculo.Max(l => l.PuntoInicial.Y), lineasCalculo.Max(l => l.PuntoFinal.Y));

            if (filtrarLineasVerticalesYHorizontales)
            {
                lineasCalculo = lineasCalculo.Where(l => EsLineaVertical(l) || EsLineaHorizontal(l)).ToList();
            }

            CalcularPuntosIntermedios(lineasCalculo);

            int[,] mapa = new int[maxY + 1, maxX + 1];
            List<Punto> coordenadas = lineasCalculo.SelectMany(l => l.Puntos).ToList();
            int numPuntosCoincidentes = 0;

            foreach (Punto coordenada in coordenadas)
            {
                mapa[coordenada.Y, coordenada.X]++;
            }

            for (int x = 0; x <= maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (mapa[y, x] > 1)
                    {
                        numPuntosCoincidentes++;
                    }
                }
            }

            if (Debug)
                Console.WriteLine($"Puntos coincidentes: {numPuntosCoincidentes}. Líneas verticales y horizontales filtradas: {filtrarLineasVerticalesYHorizontales}");

            return numPuntosCoincidentes;
        }


        private static bool EsLineaVertical(Linea linea)
        {
            return linea.PuntoInicial.X == linea.PuntoFinal.X;
        }


        private static bool EsLineaHorizontal(Linea linea)
        {
            return linea.PuntoInicial.Y == linea.PuntoFinal.Y;
        }


        private static void CalcularPuntosIntermedios(List<Linea> lineas)
        {
            foreach (Linea linea in lineas)
            {
                int incrementoX = GetIncrementoX(linea);
                int incrementoY = GetIncrementoY(linea);
                int x = linea.PuntoInicial.X;
                int y = linea.PuntoInicial.Y;

                linea.Puntos.Add(linea.PuntoInicial);

                do
                {
                    x += incrementoX;
                    y += incrementoY;

                    linea.Puntos.Add(new Punto { X = x, Y = y });

                } while (x != linea.PuntoFinal.X || y != linea.PuntoFinal.Y);
            }
        }


        private static int GetIncrementoX(Linea linea) => linea switch
        {
            { PuntoInicial: var pi, PuntoFinal: var pf } when pi.X < pf.X => 1,
            { PuntoInicial: var pi, PuntoFinal: var pf } when pi.X > pf.X => -1,
            _ => 0,
        };


        private static int GetIncrementoY(Linea linea) => linea switch
        {
            { PuntoInicial: var pi, PuntoFinal: var pf } when pi.Y < pf.Y => 1,
            { PuntoInicial: var pi, PuntoFinal: var pf } when pi.Y > pf.Y => -1,
            _ => 0,
        };


        public class Punto
        {
            public int X { get; set; }
            public int Y { get; set; }
        }


        public class Linea
        {
            public Punto PuntoInicial { get; set; }
            public Punto PuntoFinal { get; set; }
            public List<Punto> Puntos { get; set; }

            public Linea(Punto puntoInicial, Punto puntoFinal, List<Punto> puntos)
            {
                PuntoInicial = puntoInicial;
                PuntoFinal = puntoFinal;
                Puntos = puntos;
            }
        }
    }
}