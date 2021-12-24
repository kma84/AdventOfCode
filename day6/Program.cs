using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day6
{
    class Program
    {
        private static readonly int DIAS = 256; // 18 80 256;
        private static readonly int MAX_DIAS_CREACION_PEZ = 9;
        private static readonly int DIAS_CREACION_PEZ = 7;


        static void Main(string[] args)
        {
            //long numeroLanternfish = CalcularNumLanternfishFuerzaBruta();            
            long numeroLanternfish = CalcularNumLanternfishArrayDias();

            Console.WriteLine($"Número de lanternfish tras {DIAS} días: {numeroLanternfish}");
        }


        private static long CalcularNumLanternfishFuerzaBruta()
        {
            //List<int> lanternfish = GetInitialLanternfish();
            List<int> lanternfish = new() { 3, 4, 3, 1, 2 };

            int dia = 0;

            while (dia < DIAS)
            {
                for (int i = lanternfish.Count - 1; i >= 0; i--)
                {
                    if (lanternfish[i] == 0)
                    {
                        lanternfish[i] = 6;
                        lanternfish.Add(8);
                    }
                    else
                    {
                        lanternfish[i]--;
                    }
                }

                dia++;
            }

            return lanternfish.Count;
        }


        private static long CalcularNumLanternfishArrayDias()
        {
            List<int> pecesIniciales = GetInitialLanternfish();
            //List<int> pecesIniciales = new() { 3, 4, 3, 1, 2 };

            List<long[]> listDias = InicializarArrayDias(pecesIniciales);

            for (int i = 1; i <= DIAS; i++)
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


        private static List<long[]> InicializarArrayDias(List<int> pecesIniciales)
        {
            List<long[]> listDias = new List<long[]>();

            for (int i = 0; i <= DIAS; i++)
            {
                listDias.Add(new long[MAX_DIAS_CREACION_PEZ]);
            }

            foreach (int pezInicial in pecesIniciales)
            {
                listDias[0][pezInicial]++;
            }

            return listDias;
        }


        private static List<int> GetInitialLanternfish()
        {
            string input =
                File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "inputs" + Path.DirectorySeparatorChar + "input.txt");

            return input.Split(',').Select(n => int.Parse(n)).ToList();
        }
    }
}
