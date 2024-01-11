using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2021.Day08
{
    [Problem(Year = 2021, Day = 8, ProblemName = "Seven Segment Search")]
    internal class Problem : IProblem
    {
        public bool Debug { get; set; } = false;

        public string Part1(string input)
        {
            int totalNumsSegmentoUnico = 0;

            List<SecuenciaDigito> todosLosDigitos = GetInputs(input).SelectMany(i => i.Digitos).ToList();

            // Dígito 1
            totalNumsSegmentoUnico += todosLosDigitos.Count(d => d.Secuencia.Count == 2);

            // Dígito 7
            totalNumsSegmentoUnico += todosLosDigitos.Count(d => d.Secuencia.Count == 3);

            // Dígito 4
            totalNumsSegmentoUnico += todosLosDigitos.Count(d => d.Secuencia.Count == 4);

            // Dígito 8
            totalNumsSegmentoUnico += todosLosDigitos.Count(d => d.Secuencia.Count == 7);

            if (Debug)
                Console.WriteLine($"Puzle1. Total de dígitos con número de segmentos único: {totalNumsSegmentoUnico}");

            return totalNumsSegmentoUnico.ToString();
        }

        public string Part2(string input)
        {
            var inputs = GetInputs(input);

            foreach (Input dataInput in inputs)
            {
                // Dígito 1
                dataInput.Patrones.First(p => p.Secuencia.Count == 2).DigitoCorrespondiente = "1";

                // Dígito 7
                SecuenciaDigito siete = dataInput.Patrones.First(p => p.Secuencia.Count == 3);
                siete.DigitoCorrespondiente = "7";

                // Dígito 4
                dataInput.Patrones.First(p => p.Secuencia.Count == 4).DigitoCorrespondiente = "4";

                // Dígito 8
                dataInput.Patrones.First(p => p.Secuencia.Count == 7).DigitoCorrespondiente = "8";

                // Dígito 3
                SecuenciaDigito tres = dataInput.Patrones.First(p => p.Secuencia.Count == 5 && p.Secuencia.Except(siete.Secuencia).Count() == 2);
                tres.DigitoCorrespondiente = "3";

                // Dígito 9
                dataInput.Patrones.First(p => p.Secuencia.Count == 6 && p.Secuencia.Except(tres.Secuencia).Count() == 1).DigitoCorrespondiente = "9";

                // Dígito 0
                dataInput.Patrones.First(p => p.Secuencia.Count == 6 && p.DigitoCorrespondiente == null && p.Secuencia.Except(siete.Secuencia).Count() == 3)
                    .DigitoCorrespondiente = "0";

                // Dígito 6
                SecuenciaDigito seis = dataInput.Patrones.First(p => p.Secuencia.Count == 6 && p.DigitoCorrespondiente == null);
                seis.DigitoCorrespondiente = "6";

                // Dígito 5
                dataInput.Patrones.First(p => p.Secuencia.Count == 5 && p.DigitoCorrespondiente == null && seis.Secuencia.Except(p.Secuencia).Count() == 1)
                    .DigitoCorrespondiente = "5";

                // Dígito 2
                dataInput.Patrones.First(p => p.DigitoCorrespondiente == null).DigitoCorrespondiente = "2";


                foreach (SecuenciaDigito digito in dataInput.Digitos)
                {
                    digito.DigitoCorrespondiente =
                        dataInput.Patrones.First(p => p.Secuencia.Count == digito.Secuencia.Count && !digito.Secuencia.Except(p.Secuencia).Any())
                                      .DigitoCorrespondiente;
                }

                dataInput.Valor = int.Parse(string.Join(string.Empty, dataInput.Digitos.Select(d => d.DigitoCorrespondiente)));
            }

            int valorTotal = inputs.Sum(i => i.Valor);

            if(Debug)
                Console.WriteLine($"Puzle2. Suma de todos los valores: {valorTotal}");

            return valorTotal.ToString();
        }


        private static List<Input> GetInputs(string input)
        {
            string[] filas = input.GetLines(StringSplitOptions.RemoveEmptyEntries);

            List<Input> inputs = [];

            foreach (string fila in filas)
            {
                string[] patronesYDigitos = fila.Split('|');

                List<SecuenciaDigito> patrones = patronesYDigitos[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(s => new SecuenciaDigito { Secuencia = [.. s.ToCharArray()] })
                                   .ToList();

                List<SecuenciaDigito> digitos = patronesYDigitos[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(s => new SecuenciaDigito { Secuencia = [.. s.ToCharArray()] })
                                   .ToList();

                inputs.Add(new Input
                {
                    Patrones = patrones,
                    Digitos = digitos
                });
            }

            return inputs;
        }


        public class Input
        {
            public List<SecuenciaDigito> Patrones { get; set; } = [];
            public List<SecuenciaDigito> Digitos { get; set; } = [];
            public int Valor { get; set; }
        }


        public class SecuenciaDigito
        {
            public List<char> Secuencia { get; set; } = [];
            public string? DigitoCorrespondiente { get; set; }
        }
    }
}