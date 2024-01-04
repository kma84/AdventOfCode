namespace AdventOfCode.Utils
{
    public static class MathUtils
    {
        /// <summary>
        /// Least common multiple
        /// </summary>
        public static long Lcm(long[] numbers) => numbers.Aggregate((a, b) => Math.Abs(a * b) / Gdc(a, b));
        
        private static long Gdc(long a, long b) => b == 0 ? a : Gdc(b, a % b);
    }
}
