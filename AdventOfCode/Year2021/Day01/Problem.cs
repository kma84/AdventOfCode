
using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;

namespace AdventOfCode.Year2021.Day01
{
    [Problem(Year = 2021, Day = 1, ProblemName = "Sonar Sweep")]
    internal class Problem : IProblem
    {
        public string Part1(string input)
        {
            IEnumerable<int> profundidades = input.Split('\n').Where(str => int.TryParse(str, out _)).Select(str => int.Parse(str));
            int aumentosProfundidad = CalcularAumentosProfundidad(profundidades);

            return aumentosProfundidad.ToString();
        }

        public string Part2(string input)
        {
            IEnumerable<int> profundidades = input.Split('\n').Where(str => int.TryParse(str, out _)).Select(str => int.Parse(str));
            int aumentosProfundidadAgrupados = CalcularAumentosProfundidadAgrupados(profundidades);

            return aumentosProfundidadAgrupados.ToString();
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
            List<int> profundidadesAgrupadas = [];
            List<int> grupo1 = [];
            List<int>? grupo2 = null;
            List<int>? grupo3 = null;

            foreach (int profundidad in profundidades)
            {
                if (grupo1.Count == 1 && grupo2 == null)
                    grupo2 = [];

                if (grupo2?.Count == 1 && grupo3 == null)
                    grupo3 = [];

                grupo1.Add(profundidad);
                grupo2?.Add(profundidad);
                grupo3?.Add(profundidad);

                if (grupo1.Count % 3 == 0)
                {
                    profundidadesAgrupadas.Add(grupo1.Sum());
                    grupo1.Clear();
                }

                if (grupo2 != null && grupo2.Count % 3 == 0)
                {
                    profundidadesAgrupadas.Add(grupo2.Sum());
                    grupo2.Clear();
                }

                if (grupo3 != null && grupo3.Count % 3 == 0)
                {
                    profundidadesAgrupadas.Add(grupo3.Sum());
                    grupo3.Clear();
                }
            }

            return CalcularAumentosProfundidad(profundidadesAgrupadas);
        }
    }
}
