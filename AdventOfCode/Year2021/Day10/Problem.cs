using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;

namespace AdventOfCode.Year2021.Day10
{
    [Problem(Year = 2021, Day = 10, ProblemName = "Syntax Scoring")]
    internal class Problem : IProblem
    {
        private static readonly List<char> CARACTERES_APERTURA = new() { '(', '[', '{', '<' };
        private static readonly List<char> CARACTERES_CIERRE = new() { ')', ']', '}', '>' };

        public string Part1(string input) => input.GetLines().Select(GetPuntuacionFila).Sum().ToString();

        public string Part2(string input)
        {
            List<string> lineasIncompletas = input.GetLines().Where(l => GetPuntuacionFila(l) == 0).ToList();
            List<long> puntuaciones = lineasIncompletas.Select(CompletarFila).OrderBy(n => n).ToList();
            
            return puntuaciones[puntuaciones.Count / 2].ToString();
        }


        private long CompletarFila(string fila)
        {
            Dictionary<char, int> puntuaciones = new() {
                { ')', 1 },
                { ']', 2 },
                { '}', 3 },
                { '>', 4 }
            };

            Stack<char> pila = new();
            string cierreFila = string.Empty;
            long result = 0;

            foreach (char c in fila)
            {
                if (CARACTERES_APERTURA.Contains(c))
                {
                    pila.Push(c);
                }
                else if (pila.Peek() == CARACTERES_APERTURA[CARACTERES_CIERRE.IndexOf(c)])
                {
                    pila.Pop();
                }
            }

            while (pila.TryPop(out char lastChar))
            {
                cierreFila += CARACTERES_CIERRE[CARACTERES_APERTURA.IndexOf(lastChar)];
            }

            foreach (char c in cierreFila)
            {
                result = result * 5 + puntuaciones[c];
            }

            return result;
        }

        private int GetPuntuacionFila(string fila)
        {
            Dictionary<char, int> puntuaciones = new() {
                { ')', 3 },
                { ']', 57 },
                { '}', 1197 },
                { '>', 25137 }
            };

            Stack<char> pila = new();

            foreach (char c in fila)
            {
                if (CARACTERES_APERTURA.Contains(c))
                {
                    pila.Push(c);
                }
                else
                {
                    // es un caracter de cierre
                    if (pila.TryPop(out char lastChar))
                    {
                        if (lastChar != CARACTERES_APERTURA[CARACTERES_CIERRE.IndexOf(c)])
                        {
                            return puntuaciones[c];
                        }
                    }
                    else
                    {
                        // caso donde el primer char ya es erróneo
                        return puntuaciones[c];
                    }
                }
            }

            return 0;
        }
    }
}