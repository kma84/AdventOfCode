using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2021.Day03
{
    [Problem(Year = 2021, Day = 3, ProblemName = "Binary Diagnostic")]
    internal class Problem : IProblem
    {
        private static readonly int NUMERO_DIGITOS = 12;

        public bool Debug { get; set; } = false;

        public string Part1(string input)
        {
            List<string> diagnostico = input.GetLines().ToList();

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
            int producto = gammaRate * epsilonRate;

            if (Debug)
                Console.WriteLine($"Gamma rate: {gammaRate}, epsilon rate: {epsilonRate}, producto: {producto}");

            return producto.ToString();
        }

        public string Part2(string input)
        {
            List<string> diagnostico = input.GetLines().ToList();

            string oxygenGeneratorRatingStr = string.Empty;
            string co2ScrubberRatingStr = string.Empty;

            List<string> candidatos = diagnostico;

            for (int i = 0; i < NUMERO_DIGITOS; i++)
            {
                var digitos = candidatos.Select(num => num.ElementAt(i));

                char masComun = MasComun(digitos).OrderByDescending(c => c).First();

                candidatos = candidatos.Where(num => num.ElementAt(i) == masComun).ToList();

                if (candidatos.Count == 1)
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

                if (candidatos.Count == 1)
                {
                    co2ScrubberRatingStr = candidatos.First();
                }
            }

            int oxygenGeneratorRating = Convert.ToInt32(oxygenGeneratorRatingStr, 2);
            int co2ScrubberRating = Convert.ToInt32(co2ScrubberRatingStr, 2);
            int producto = oxygenGeneratorRating * co2ScrubberRating;

            if (Debug)
                Console.WriteLine($"Oxygen generator rating: {oxygenGeneratorRating}, CO2 scrubber rating: {co2ScrubberRating}, producto: {producto}");

            return producto.ToString();
        }


        private static IEnumerable<T> MasComun<T>(IEnumerable<T> input)
        {
            var dict = input.ToLookup(x => x);
            if (dict.Count == 0)
                return Enumerable.Empty<T>();
            var maxCount = dict.Max(x => x.Count());
            return dict.Where(x => x.Count() == maxCount).Select(x => x.Key);
        }


        private static IEnumerable<T> MenosComun<T>(IEnumerable<T> input)
        {
            var dict = input.ToLookup(x => x);
            if (dict.Count == 0)
                return Enumerable.Empty<T>();
            var minCount = dict.Min(x => x.Count());
            return dict.Where(x => x.Count() == minCount).Select(x => x.Key);
        }
    }
}