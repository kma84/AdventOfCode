namespace AdventOfCode.Utils
{
    public static class MathUtils
    {
        /// <summary>
        /// Least common multiple
        /// </summary>
        public static long Lcm(long[] numbers) => numbers.Aggregate((a, b) => Math.Abs(a * b) / Gdc(a, b));
        
        private static long Gdc(long a, long b) => b == 0 ? a : Gdc(b, a % b);

        public static int Mod(int a, int b) => (a % b + b) % b;

        public static List<int> Factors(int n)
        {
            List<int> factors = [];
            for (int i = 1; i <= Math.Sqrt(n); i++)
            {
                if (n % i == 0)
                {
                    factors.Add(i);
                    if (i != n / i)
                    {
                        factors.Add(n / i);
                    }
                }
            }
            factors.Sort();

            return factors;
        }
    }
}
