
namespace AdventOfCode.Utils
{
    public static class StringExtensions
    {

        public static string[] GetLines(this string str) => str.GetLines(StringSplitOptions.None);

        public static string[] GetLines(this string str, StringSplitOptions options) => str.Split(new string[] { "\r\n", "\r", "\n" }, options);

    }
}
