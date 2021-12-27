﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace dia8
{
    class Program
    {
        public class Input
        {
            public List<SecuenciaDigito> Patrones { get; set; }
            public List<SecuenciaDigito> Digitos { get; set; }
            public int Valor { get; set; }
        }


        public class SecuenciaDigito
        {
            public List<char> Secuencia { get; set; }
            public string DigitoCorrespondiente { get; set; }
        }


        static void Main(string[] args)
        {
            List<Input> inputs = GetInputs();

            Puzle1(inputs);
            Puzle2(inputs);
        }


        private static void Puzle1(List<Input> inputs)
        {
            int totalNumsSegmentoUnico = 0;

            List<SecuenciaDigito> todosLosDigitos = inputs.SelectMany(i => i.Digitos).ToList();

            // Dígito 1
            totalNumsSegmentoUnico += todosLosDigitos.Count(d => d.Secuencia.Count == 2);

            // Dígito 7
            totalNumsSegmentoUnico += todosLosDigitos.Count(d => d.Secuencia.Count == 3);

            // Dígito 4
            totalNumsSegmentoUnico += todosLosDigitos.Count(d => d.Secuencia.Count == 4);

            // Dígito 8
            totalNumsSegmentoUnico += todosLosDigitos.Count(d => d.Secuencia.Count == 7);

            Console.WriteLine($"Puzle1. Total de dígitos con número de segmentos único: {totalNumsSegmentoUnico}");
        }


        private static void Puzle2(List<Input> inputs)
        {
            foreach (Input input in inputs)
            {
                // Dígito 1
                input.Patrones.First(p => p.Secuencia.Count == 2).DigitoCorrespondiente = "1";

                // Dígito 7
                SecuenciaDigito siete = input.Patrones.First(p => p.Secuencia.Count == 3);
                siete.DigitoCorrespondiente = "7";

                // Dígito 4
                input.Patrones.First(p => p.Secuencia.Count == 4).DigitoCorrespondiente = "4";

                // Dígito 8
                input.Patrones.First(p => p.Secuencia.Count == 7).DigitoCorrespondiente = "8";

                // Dígito 3
                SecuenciaDigito tres = input.Patrones.First(p => p.Secuencia.Count == 5 && p.Secuencia.Except(siete.Secuencia).Count() == 2);
                tres.DigitoCorrespondiente = "3";

                // Dígito 9
                input.Patrones.First(p => p.Secuencia.Count == 6 && p.Secuencia.Except(tres.Secuencia).Count() == 1).DigitoCorrespondiente = "9";

                // Dígito 0
                input.Patrones.First(p => p.Secuencia.Count == 6 && p.DigitoCorrespondiente == null && p.Secuencia.Except(siete.Secuencia).Count() == 3)
                    .DigitoCorrespondiente = "0";

                // Dígito 6
                SecuenciaDigito seis = input.Patrones.First(p => p.Secuencia.Count == 6 && p.DigitoCorrespondiente == null);
                seis.DigitoCorrespondiente = "6";

                // Dígito 5
                input.Patrones.First(p => p.Secuencia.Count == 5 && p.DigitoCorrespondiente == null && seis.Secuencia.Except(p.Secuencia).Count() == 1)
                    .DigitoCorrespondiente = "5";

                // Dígito 2
                input.Patrones.First(p => p.DigitoCorrespondiente == null).DigitoCorrespondiente = "2";


                foreach (SecuenciaDigito digito in input.Digitos)
                {
                    digito.DigitoCorrespondiente =
                        input.Patrones.First(p => p.Secuencia.Count == digito.Secuencia.Count && !digito.Secuencia.Except(p.Secuencia).Any())
                                      .DigitoCorrespondiente;
                }

                input.Valor = int.Parse(string.Join(string.Empty, input.Digitos.Select(d => d.DigitoCorrespondiente)));
            }

            int valorTotal = inputs.Sum(i => i.Valor);
            Console.WriteLine($"Puzle2. Suma de todos los valores: {valorTotal}");
        }


        private static List<Input> GetInputs()
        {
            string input =
                File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "inputs" + Path.DirectorySeparatorChar + "input.txt");
            //string input = "acedgfb cdfbe gcdfa fbcad dab cefabd cdfgeb eafb cagedb ab | cdfeb fcadb cdfeb cdbaf";

            string[] filas = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            List<Input> inputs = new();

            foreach (string fila in filas)
            {
                string[] patronesYDigitos = fila.Split('|');

                List<SecuenciaDigito> patrones = patronesYDigitos[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(s => new SecuenciaDigito { Secuencia = s.ToCharArray().ToList() })
                                   .ToList();

                List<SecuenciaDigito> digitos = patronesYDigitos[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(s => new SecuenciaDigito { Secuencia = s.ToCharArray().ToList() })
                                   .ToList();

                inputs.Add(new Input {
                    Patrones = patrones,
                    Digitos = digitos
                });
            }

            return inputs;
        }

    }
}
