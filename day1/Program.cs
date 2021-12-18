using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day1_puzzle1
{
    class Program
    {
        static void Main(string[] args)
        {
            string input =
                File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "inputs" + Path.DirectorySeparatorChar + "input.txt");

            IEnumerable<int> profundidades = input.Split('\n').Where(str => int.TryParse(str, out _)).Select(str => int.Parse(str));
            int aumentosProfundidad = CalcularAumentosProfundidad(profundidades);
            int aumentosProfundidadAgrupados = CalcularAumentosProfundidadAgrupados(profundidades);

            Console.WriteLine($"Aumentos de profundidad: {aumentosProfundidad}");
            Console.WriteLine($"Aumentos de profundidad agrupados: {aumentosProfundidadAgrupados}");
        }


        private static int CalcularAumentosProfundidad(IEnumerable<int> profundidades)
        {
            int? profundidadAnterior = null;
            int aumentosProfundidad = 0;
            foreach (int profundidad in profundidades)
            {
                if (profundidadAnterior.HasValue && profundidad > profundidadAnterior)
                {
                    aumentosProfundidad++;
                }

                profundidadAnterior = profundidad;
            }

            return aumentosProfundidad;
        }


        private static int CalcularAumentosProfundidadAgrupados(IEnumerable<int> profundidades)
        {
            List<int> profundidadesAgrupadas = new();
            List<int> grupo1 = new();
            List<int> grupo2 = null;
            List<int> grupo3 = null;

            foreach (int profundidad in profundidades)
            {
                if (grupo1.Count == 1 && grupo2 == null)
                    grupo2 = new();

                if (grupo2?.Count == 1 && grupo3 == null)
                    grupo3 = new();

                grupo1.Add(profundidad);
                if (grupo2 != null)
                    grupo2.Add(profundidad);
                if (grupo3 != null)
                    grupo3.Add(profundidad);

                if (grupo1.Count % 3 == 0)
                {
                    profundidadesAgrupadas.Add(grupo1.Sum());
                    grupo1.Clear();
                }

                if (grupo2?.Count % 3 == 0)
                {
                    profundidadesAgrupadas.Add(grupo2.Sum());
                    grupo2.Clear();
                }

                if (grupo3?.Count % 3 == 0)
                {
                    profundidadesAgrupadas.Add(grupo3.Sum());
                    grupo3.Clear();
                }
            }

            return CalcularAumentosProfundidad(profundidadesAgrupadas);
        }

    }
}
