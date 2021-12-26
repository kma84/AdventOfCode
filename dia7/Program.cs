using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace dia7
{
    class Program
    {
        public class ConsumoFuel 
        {
            public int DistanciaDestino { get; set; }
            public int FuelTotal { get; set; }
        }


        static void Main(string[] args)
        {
            List<int> inputs = GetInputs();
            //List<int> inputs = new List<int> { 16, 1, 2, 0, 4, 2, 7, 1, 2, 14 };
            int maxDistancia = inputs.Max();
            
            Puzle1(inputs, maxDistancia);
            Puzle2(inputs, maxDistancia);
        }


        private static void Puzle1(List<int> inputs, int maxDistancia)
        {
            ConsumoFuel[] aConsumos = new ConsumoFuel[maxDistancia + 1];

            for (int d = 0; d <= maxDistancia; d++)
            {
                ConsumoFuel consumoDistanciaActual = new ConsumoFuel { DistanciaDestino = d };

                foreach (int input in inputs)
                {
                    consumoDistanciaActual.FuelTotal += Math.Abs(input - d);
                }

                aConsumos[d] = consumoDistanciaActual;
            }

            int fuelMinimo = aConsumos.Min(c => c.FuelTotal);

            Console.WriteLine($"Puzle 1: Fuel mínimo para alinear la posición: {fuelMinimo}");
        }


        private static void Puzle2(List<int> inputs, int maxDistancia)
        {
            ConsumoFuel[] aConsumos = new ConsumoFuel[maxDistancia + 1];

            for (int d = 0; d <= maxDistancia; d++)
            {
                ConsumoFuel consumoDistanciaActual = new ConsumoFuel { DistanciaDestino = d };

                foreach (int input in inputs)
                {
                    int distanciaEntreInputYDistanciaActual = Math.Abs(input - d);                    
                    consumoDistanciaActual.FuelTotal += CalcularConsumoFuel(distanciaEntreInputYDistanciaActual);
                }

                aConsumos[d] = consumoDistanciaActual;
            }

            int fuelMinimo = aConsumos.Min(c => c.FuelTotal);

            Console.WriteLine($"Puzle 2: Fuel mínimo para alinear la posición: {fuelMinimo}");
        }


        private static int CalcularConsumoFuel(int distancia)
        {
            int fuel = 0;

            for (int d = 1; d <= distancia; d++)
            {
                fuel += d;
            }

            return fuel;
        }


        private static List<int> GetInputs()
        {
            string input =
                File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "inputs" + Path.DirectorySeparatorChar + "input.txt");

            return input.Split(',').Select(n => int.Parse(n)).ToList();
        }
    }
}
