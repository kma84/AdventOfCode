using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day3
{
    class Program
    {
        private static readonly int NUMERO_DIGITOS = 12;

        static void Main(string[] args)
        {
            List<string> diagnostico = GetDiagnostico();

            puzle1(diagnostico);
            puzle2(diagnostico);
        }


        private static void puzle1(List<string> diagnostico)
        {
            string gammaRateStr = string.Empty;
            string epsilonRateStr = string.Empty;

            for (int i = 0; i < NUMERO_DIGITOS; i++)
            {
                var digitos = diagnostico.Select(num => num.ElementAt(i));
                char moda = digitos.GroupBy(i => i)
                              .OrderByDescending(grp => grp.Count())
                              .Select(grp => grp.Key)
                              .First();

                gammaRateStr += moda;
                epsilonRateStr += moda == '0' ? '1' : '0';
            }

            int gammaRate = Convert.ToInt32(gammaRateStr, 2);
            int epsilonRate = Convert.ToInt32(epsilonRateStr, 2);

            Console.WriteLine($"Gamma rate: {gammaRate}, epsilon rate: {epsilonRate}, producto: {gammaRate * epsilonRate}");
        }


        private static void puzle2(List<string> diagnostico)
        {
            string oxygenGeneratorRatingStr = string.Empty;
            string co2ScrubberRatingStr = string.Empty;

            List<string> candidatos = diagnostico;

            for (int i = 0; i < NUMERO_DIGITOS; i++)
            {
                var digitos = candidatos.Select(num => num.ElementAt(i));

                char masComun = MasComun(digitos).OrderByDescending(c => c).First();

                candidatos = candidatos.Where(num => num.ElementAt(i) == masComun).ToList();

                if (candidatos.Count() == 1)
                {
                    oxygenGeneratorRatingStr = candidatos.First();
                }
            }

            candidatos = diagnostico;

            for (int i = 0; i < NUMERO_DIGITOS; i++)
            {
                var digitos = candidatos.Select(num => num.ElementAt(i));

                char menosComun = MenosComun(digitos).OrderBy(c => c).First();

                candidatos = candidatos.Where(num => num.ElementAt(i) == menosComun).ToList();

                if (candidatos.Count() == 1)
                {
                    co2ScrubberRatingStr = candidatos.First();
                }
            }

            int oxygenGeneratorRating = Convert.ToInt32(oxygenGeneratorRatingStr, 2);
            int co2ScrubberRating = Convert.ToInt32(co2ScrubberRatingStr, 2);

            Console.WriteLine($"Oxygen generator rating: {oxygenGeneratorRating}, CO2 scrubber rating: {co2ScrubberRating}, producto: {oxygenGeneratorRating * co2ScrubberRating}");
        }


        private static List<string> GetDiagnostico()
        {
            string input =
                File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "inputs" + Path.DirectorySeparatorChar + "input.txt");

            return input.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
        }


        public static IEnumerable<T> MasComun<T>(IEnumerable<T> input)
        {
            var dict = input.ToLookup(x => x);
            if (dict.Count == 0)
                return Enumerable.Empty<T>();
            var maxCount = dict.Max(x => x.Count());
            return dict.Where(x => x.Count() == maxCount).Select(x => x.Key);
        }


        public static IEnumerable<T> MenosComun<T>(IEnumerable<T> input)
        {
            var dict = input.ToLookup(x => x);
            if (dict.Count == 0)
                return Enumerable.Empty<T>();
            var minCount = dict.Min(x => x.Count());
            return dict.Where(x => x.Count() == minCount).Select(x => x.Key);
        }

    }
}
