
namespace AdventOfCode.Utils
{
    public static class StringExtensions
    {

        public static string[] GetLines(this string str) => str.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

    }
}
