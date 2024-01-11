using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;

namespace AdventOfCode.Year2021.Day07
{
    [Problem(Year = 2021, Day = 7, ProblemName = "The Treachery of Whales")]
    internal class Problem : IProblem
    {
        public bool Debug { get; set; } = false;

        public string Part1(string input)
        {
            List<int> positions = input.Split(',').Select(n => int.Parse(n)).ToList();
            int maxDistancia = positions.Max();
            ConsumoFuel[] aConsumos = new ConsumoFuel[maxDistancia + 1];

            for (int d = 0; d <= maxDistancia; d++)
            {
                ConsumoFuel consumoDistanciaActual = new() { DistanciaDestino = d };

                foreach (int position in positions)
                {
                    consumoDistanciaActual.FuelTotal += Math.Abs(position - d);
                }

                aConsumos[d] = consumoDistanciaActual;
            }

            int fuelMinimo = aConsumos.Min(c => c.FuelTotal);

            if (Debug)
                Console.WriteLine($"Puzle 1: Fuel mínimo para alinear la posición: {fuelMinimo}");

            return fuelMinimo.ToString();
        }

        public string Part2(string input)
        {
            List<int> positions = input.Split(',').Select(n => int.Parse(n)).ToList();
            int maxDistancia = positions.Max();
            ConsumoFuel[] aConsumos = new ConsumoFuel[maxDistancia + 1];

            for (int d = 0; d <= maxDistancia; d++)
            {
                ConsumoFuel consumoDistanciaActual = new() { DistanciaDestino = d };

                foreach (int position in positions)
                {
                    int distanciaEntreInputYDistanciaActual = Math.Abs(position - d);
                    consumoDistanciaActual.FuelTotal += CalcularConsumoFuel(distanciaEntreInputYDistanciaActual);
                }

                aConsumos[d] = consumoDistanciaActual;
            }

            int fuelMinimo = aConsumos.Min(c => c.FuelTotal);

            if (Debug)
                Console.WriteLine($"Puzle 2: Fuel mínimo para alinear la posición: {fuelMinimo}");
            
            return fuelMinimo.ToString();
        }


        private static int CalcularConsumoFuel(int dist) => (int)(dist / 2F * (1 + dist));


        public class ConsumoFuel
        {
            public int DistanciaDestino { get; set; }
            public int FuelTotal { get; set; }
        }
    }
}