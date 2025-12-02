using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2025.Day01
{
    [Problem(Year = 2025, Day = 1, ProblemName = "Secret Entrance")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input)
        {
            int currentPosition = DIAL_START;
            int password = 0;

            foreach (string line in input.GetLines())
            {
                int value = int.Parse(line[1..]);
                char direction = line[0];

                if (direction == LEFT_DIRECTION)
                {
                    currentPosition = MathUtils.Mod(currentPosition - value, DIAL_SIZE);
                }
                else
                {
                    currentPosition = MathUtils.Mod(currentPosition + value, DIAL_SIZE);
                }

                if (currentPosition == 0)
                    password++;
            }

            return password.ToString();
        }

        public string Part2(string input)
        {
            int currentPosition = DIAL_START;
            int password = 0;

            foreach (string line in input.GetLines())
            {
                int value = int.Parse(line[1..]);
                char direction = line[0];
                
                if (direction == LEFT_DIRECTION)
                {
                    password += (MathUtils.Mod(-currentPosition, DIAL_SIZE) + value) / DIAL_SIZE;
                    currentPosition = MathUtils.Mod(currentPosition - value, DIAL_SIZE);
                }
                else
                {
                    password += (currentPosition + value) / DIAL_SIZE;
                    currentPosition = MathUtils.Mod(currentPosition + value, DIAL_SIZE);
                }
            }

            return password.ToString();
        }

        private static readonly int DIAL_SIZE = 100;
        private static readonly int DIAL_START = 50;
        private static readonly char LEFT_DIRECTION = 'L';        
    }
}