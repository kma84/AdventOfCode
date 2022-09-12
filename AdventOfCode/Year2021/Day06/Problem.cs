using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;

namespace AdventOfCode.Year2021.Day06
{
    [Problem(Year = 2021, Day = 6, ProblemName = "Lanternfish")]
    internal class Problem : IProblem
    {
        private static readonly int MAX_DIAS_CREACION_PEZ = 9;
        private static readonly int DIAS_CREACION_PEZ = 7;


        public string Part1(string input)
        {
			return CalcularNumLanternfishArrayDias(input, dias: 80).ToString();
        }

        public string Part2(string input)
        {
			return CalcularNumLanternfishArrayDias(input, dias: 256).ToString();
        }


        private static long CalcularNumLanternfishArrayDias(string input, int dias)
        {
            List<int> pecesIniciales = input.Split(',').Select(n => int.Parse(n)).ToList();

            List<long[]> listDias = InicializarArrayDias(pecesIniciales, dias);

            for (int i = 1; i <= dias; i++)
            {
                int diaAnterior = i - 1;

                for (int numDiasRestantesDiaAnterior = 0; numDiasRestantesDiaAnterior < MAX_DIAS_CREACION_PEZ; numDiasRestantesDiaAnterior++)
                {
                    int numDiasRestantesHoy = numDiasRestantesDiaAnterior == 0 ? DIAS_CREACION_PEZ - 1 : numDiasRestantesDiaAnterior - 1;

                    listDias[i][numDiasRestantesHoy] += listDias[diaAnterior][numDiasRestantesDiaAnterior];

                    if (numDiasRestantesDiaAnterior == 0)
                    {
                        listDias[i][MAX_DIAS_CREACION_PEZ - 1] = listDias[diaAnterior][numDiasRestantesDiaAnterior];
                    }
                }
            }

            return listDias.Last().Sum();
        }


        private static List<long[]> InicializarArrayDias(List<int> pecesIniciales, int dias)
        {
            List<long[]> listDias = new List<long[]>();

            for (int i = 0; i <= dias; i++)
            {
                listDias.Add(new long[MAX_DIAS_CREACION_PEZ]);
            }

            foreach (int pezInicial in pecesIniciales)
            {
                listDias[0][pezInicial]++;
            }

            return listDias;
        }

    }
}