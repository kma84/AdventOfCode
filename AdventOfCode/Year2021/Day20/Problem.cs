using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AoCUtils;

namespace AdventOfCode.Year2021.Day20
{
    [Problem(Year = 2021, Day = 20, ProblemName = "Trench Map")]
    internal class Problem : IProblem
    {
        public bool Debug => true;

        public string Part1(string input)
        {
            void PrintDebug(char[,] image)
            {
                //if (Debug)
                    image.Print();
            }

            (string enhancementAlgorithm, char[,] image) = GetDataFromInput(input);
            PrintDebug(image);
            
            image = ResizeMatrix(image, 2);
            PrintDebug(image);

            for (int i = 0; i < 2; i++)
            {
                image = ProcessImage(image, enhancementAlgorithm);
                image = ResizeMatrix(image, 1);

                PrintDebug(image);
            }
            
            return image.Count(c => c == '#').ToString();
        }

        public string Part2(string input)
        {
			return "Part2";
        }


        private static char[,] ProcessImage(char[,] image, string enhancementAlgorithm)
        {
            char[,] newImage = new char[image.GetLength(0), image.GetLength(1)];
            newImage.Fill('.');

            for (int y = 1; y < image.GetLength(0) - 1; y++)
            {
                for (int x = 1; x < image.GetLength(1) - 1; x++)
                {
                    string pixels = GetPixels(image, x, y);

                    char newPixel = enhancementAlgorithm[GetIndex(pixels)];
                    newImage[y, x] = newPixel;
                }
            }

            return newImage;
        }

        private static string GetPixels(char[,] image, int x, int y)
        {
            List<char> pixels = new()
            {
                image[y - 1, x - 1],
                image[y - 1, x],
                image[y - 1, x + 1],
                image[y, x - 1],
                image[y, x],
                image[y, x + 1],
                image[y + 1, x - 1],
                image[y + 1, x],
                image[y + 1, x + 1]
            };

            return new string(pixels.ToArray());
        }

        private static (string enhancementAlgorithmString, char[,] image) GetDataFromInput(string input)
        {
            string[] lines = input.GetLines();

            string enhancementAlgorithmString = lines.First();
            List<string> imageLines = lines.Skip(2).ToList();

            char[,] image = new char[imageLines.Count, imageLines[0].Length];

            for (int y = 0; y < image.GetLength(0); y++)
                for (int x = 0; x < image.GetLength(1); x++)
                    image[y, x] = imageLines[y][x];

            return (enhancementAlgorithmString, image);
        }

        private static char[,] ResizeMatrix(char[,] original, int offset)
        {
            int newYLength = original.GetLength(0) + offset * 2;
            int newXLength = original.GetLength(1) + offset * 2;
            char[,] newArray = new char[newYLength, newXLength];

            for (int y = 0; y < newYLength; y++)
            {
                for (int x = 0; x < newXLength; x++)
                {
                    int adjustedY = y - offset;
                    int adjustedX = x - offset;

                    if (adjustedY >= 0 && adjustedY < original.GetLength(0) && adjustedX >= 0 && adjustedX < original.GetLength(1))
                        newArray[y, x] = original[adjustedY, adjustedX];
                    else
                        newArray[y, x] = '.';
                }
            }

            return newArray;
        }

        private static int GetIndex(string pixels)
        {
            string pixelsBin = pixels.Replace('.', '0').Replace('#', '1');
            return Convert.ToInt32(pixelsBin, 2);
        }

    }
}