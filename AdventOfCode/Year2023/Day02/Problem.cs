using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2023.Day02
{
    [Problem(Year = 2023, Day = 2, ProblemName = "Cube Conundrum")]
    internal class Problem : IProblem
    {
        private const string RED = "red";
        private const string GREEN = "green";
        private const string BLUE = "blue";

        private const int RED_CUBES = 12;
        private const int GREEN_CUBES = 13;
        private const int BLUE_CUBES = 14;

        public bool Debug => false;

        public string Part1(string input) => 
            GetGames(input.GetLines())
                .Where(g => g.MaxRed <= RED_CUBES && g.MaxGreen <= GREEN_CUBES && g.MaxBlue <= BLUE_CUBES)
                .Sum(g => g.Id)
                .ToString();

        public string Part2(string input) =>
            GetGames(input.GetLines())
                .Sum(g => g.MaxRed * g.MaxGreen * g.MaxBlue)
                .ToString();


        private static List<Game> GetGames(string[] lines)
        {
            List<Game> games = new();

            foreach (string line in lines)
            {
                string[] tokens = line.Split(':');
                string[] subsetArray = tokens[1].Split(";");

                int id = int.Parse(tokens[0].Split(' ')[1]);
                int maxRed = 0;
                int maxGreen = 0;
                int maxBlue = 0;

                foreach (var subset in subsetArray)
                {
                    string[] colors = subset.Split(",");

                    foreach (string color in colors)
                    {
                        int cubes = int.Parse(color.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]);

                        if (color.Contains(RED) && cubes > maxRed)
                            maxRed = cubes;

                        if (color.Contains(GREEN) && cubes > maxGreen)
                            maxGreen = cubes;

                        if (color.Contains(BLUE) && cubes > maxBlue)
                            maxBlue = cubes;
                    }
                }

                games.Add(new Game(id, maxRed, maxGreen, maxBlue));
            }

            return games;
        }


        private record Game(int Id, int MaxRed, int MaxGreen, int MaxBlue);

	}
}