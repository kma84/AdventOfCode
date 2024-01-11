namespace AdventOfCode.Utils.Extensions
{
    public static class StringExtensions
    {
        private static readonly string[] SEPARATORS = ["\r\n", "\r", "\n"];

        public static string[] GetLines(this string str) => str.GetLines(StringSplitOptions.None);

        public static string[] GetLines(this string str, StringSplitOptions options) => str.Split(SEPARATORS, options);
    }
}
