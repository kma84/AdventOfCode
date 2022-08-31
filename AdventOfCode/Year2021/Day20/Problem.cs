using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AoCUtils;

namespace AdventOfCode.Year2021.Day20
{
    [Problem(Year = 2021, Day = 20, ProblemName = "Trench Map")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        private const char DARK_PIXEL = '.';
        private const char LIGHT_PIXEL = '#';

        public string Part1(string input)
        {
            (string enhancementAlgorithm, char[,] image) = GetDataFromInput(input);
            PrintDebug(image);
            
            return GetLitPixels(image, enhancementAlgorithm, iterations: 2).ToString();
        }

        public string Part2(string input)
        {
            (string enhancementAlgorithm, char[,] image) = GetDataFromInput(input);

            return GetLitPixels(image, enhancementAlgorithm, iterations: 50).ToString();
        }


        private int GetLitPixels(char[,] image, string enhancementAlgorithm, int iterations)
        {
            var infinitePixels = GetInfinitePixels(enhancementAlgorithm);

            for (int i = 0; i < iterations; i++)
            {
                image = ProcessImage(image, enhancementAlgorithm, infinitePixels[i % infinitePixels.Count]);
            }

            return image.Count(c => c == LIGHT_PIXEL);
        }


        private char[,] ProcessImage(char[,] image, string enhancementAlgorithm, char infinitePixel)
        {
            char[,] newImage = new char[image.GetLength(0), image.GetLength(1)];
            newImage.Fill(DARK_PIXEL);

            for (int y = 0; y < image.GetLength(0); y++)
            {
                for (int x = 0; x < image.GetLength(1); x++)
                {                    
                    newImage[y, x] = ProcessPixel(image, x, y, enhancementAlgorithm, infinitePixel);
                }
            }

            PrintDebug(newImage);

            while (!FrameFormed(newImage))
            {
                image = ResizeMatrix(image, 1, infinitePixel);
                newImage = ResizeMatrix(newImage, 1, infinitePixel);
                char newPixel;

                for (int y = 0; y < image.GetLength(0); y++)
                {
                    if (y == 0 || y == image.GetLength(0) - 1)
                    {
                        for (int x = 0; x < image.GetLength(1); x++)
                        {
                            newPixel = ProcessPixel(image, x, y, enhancementAlgorithm, infinitePixel);
                            newImage[y, x] = newPixel;
                        }
                    }
                    else
                    {
                        newPixel = ProcessPixel(image, 0, y, enhancementAlgorithm, infinitePixel);
                        newImage[y, 0] = newPixel;

                        newPixel = ProcessPixel(image, image.GetLength(1) - 1, y, enhancementAlgorithm, infinitePixel);
                        newImage[y, image.GetLength(1) - 1] = newPixel;
                    }
                }

                PrintDebug(newImage);
            }

            return newImage;
        }

        private static bool FrameFormed(char[,] image)
        {
            char control = image[0, 0];

            for (int y = 0; y < image.GetLength(0); y++)
            {
                if (y == 0 || y > image.GetLength(0) - 1)
                {
                    for (int x = 0; x < image.GetLength(1); x++)
                    {
                        if (image[y, x] != control)
                            return false;
                    }
                }
                else
                {
                    if (image[y, 0] != control)
                        return false;

                    if (image[y, image.GetLength(1) - 1] != control)
                        return false;
                }
            }

            return true;
        }

        private static char ProcessPixel(char[,] image, int x, int y, string enhancementAlgorithm, char infinitePixel)
        {
            string pixels = GetPixels(image, x, y, infinitePixel);

            return enhancementAlgorithm[GetIndex(pixels)];
        }

        private static string GetPixels(char[,] image, int x, int y, char infinitePixel)
        {
            List<char> pixels = new()
            {
                y - 1 >= 0 && x - 1 >= 0 ? image[y - 1, x - 1] : infinitePixel,
                y - 1 >= 0 ? image[y - 1, x] : infinitePixel,
                y - 1 >= 0 && x + 1 < image.GetLength(1) ? image[y - 1, x + 1] : infinitePixel,
                x - 1 >= 0 ? image[y, x - 1] : infinitePixel,
                image[y, x],
                x + 1 < image.GetLength(1) ? image[y, x + 1] : infinitePixel,
                y + 1 < image.GetLength(0) && x - 1 >= 0 ? image[y + 1, x - 1] : infinitePixel,
                y + 1 < image.GetLength(0) ? image[y + 1, x] : infinitePixel,
                y + 1 < image.GetLength(0) && x + 1 < image.GetLength(1) ? image[y + 1, x + 1] : infinitePixel
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

        private static char[,] ResizeMatrix(char[,] original, int offset, char infinitePixel)
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
                        newArray[y, x] = infinitePixel;
                }
            }

            return newArray;
        }

        private static int GetIndex(string pixels)
        {
            string pixelsBin = pixels.Replace(DARK_PIXEL, '0').Replace(LIGHT_PIXEL, '1');
            return Convert.ToInt32(pixelsBin, 2);
        }

        private static List<char> GetInfinitePixels(string enhancementAlgorithm)
        {
            char firstPixel = enhancementAlgorithm[0];
            List<char> infinitePixels = new List<char> { firstPixel };

            if (firstPixel == LIGHT_PIXEL)
                infinitePixels.Insert(0, enhancementAlgorithm.Last());

            return infinitePixels;
        }

        private void PrintDebug(char[,] image)
        {
            if (Debug)
                image.Print();
        }

    }
}