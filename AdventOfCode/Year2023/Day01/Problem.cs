using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2023.Day01
{
    [Problem(Year = 2023, Day = 1, ProblemName = "Trebuchet?!")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input) => input.GetLines().Aggregate(0, (total, currentLine) => total + GetCalibration(currentLine)).ToString();

        public string Part2(string input) => input.GetLines().Aggregate(0, (total, currentLine) => total + GetCalibrationNumbersOrLetters(currentLine)).ToString();


        private static readonly Dictionary<string, char> _numbersStrCharMap = new()
        {
            { "1", '1' },
            { "2", '2' },
            { "3", '3' },
            { "4", '4' },
            { "5", '5' },
            { "6", '6' },
            { "7", '7' },
            { "8", '8' },
            { "9", '9' },
            { "one", '1' },
            { "two", '2' },
            { "three", '3' },
            { "four", '4' },
            { "five", '5' },
            { "six", '6' },
            { "seven", '7' },
            { "eight", '8' },
            { "nine", '9' }
        };

        private static int GetCalibration(string line) => GetNumberFromChars(line.First(char.IsNumber), line.Last(char.IsNumber));

        private static int GetCalibrationNumbersOrLetters(string line)
        {
            int minIndex = line.Length - 1;
            int maxIndex = 0;
            char firstNumber = '0';
            char secondNumber = '0';

            foreach (string key in _numbersStrCharMap.Keys)
            {
                int index = line.IndexOf(key);

                if (index == -1)
                    continue;

                if (index <= minIndex)
                {
                    minIndex = index;
                    firstNumber = _numbersStrCharMap[key];
                }
            }

            foreach (string key in _numbersStrCharMap.Keys)
            {
                int index = line.LastIndexOf(key);

                if (index == -1)
                    continue;

                if (index >= maxIndex)
                {
                    maxIndex = index;
                    secondNumber = _numbersStrCharMap[key];
                }
            }

            return GetNumberFromChars(firstNumber, secondNumber);
        }

        private static int GetNumberFromChars(char firstNumber, char secondNumber)
        {
            string numberStr = new(new char[] { firstNumber, secondNumber });

            return int.Parse(numberStr);
        }
    }
}