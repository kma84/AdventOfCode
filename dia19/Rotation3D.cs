using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dia19
{
    internal static class Rotation3D
    {

        /// <summary>
        /// https://www.cs.helsinki.fi/group/goa/mallinnus/3dtransf/3drot.html
        /// </summary>
        internal static void Print90DegreesRotations(int x, int y, int z)
        {
            int[] degrees = new int[] { 0, 90, 180, 270 };

            int newX, newY, newZ;
            double radians;

            double GetRadians(double degrees) => degrees * Math.PI / 180;
            HashSet<(int x, int y, int z)> points = new();

            for (int i = 0; i < degrees.Length; i++)
            {
                radians = GetRadians(degrees[i]);
                newY = (int)Math.Round(y * Math.Cos(radians) - z * Math.Sin(radians));
                newZ = (int)Math.Round(y * Math.Sin(radians) + z * Math.Cos(radians));
                z = newZ;
                y = newY;


                for (int j = 0; j < degrees.Length; j++)
                {
                    radians = GetRadians(degrees[j]);
                    newZ = (int)Math.Round(z * Math.Cos(radians) - x * Math.Sin(radians));
                    newX = (int)Math.Round(z * Math.Sin(radians) + x * Math.Cos(radians));
                    z = newZ;
                    x = newX;

                    for (int k = 0; k < degrees.Length; k++)
                    {
                        radians = GetRadians(degrees[k]);
                        newX = (int)Math.Round(x * Math.Cos(radians) - y * Math.Sin(radians));
                        newY = (int)Math.Round(x * Math.Sin(radians) + y * Math.Cos(radians));

                        if (points.Add((newX, newY, newZ)))
                        {
                            Console.WriteLine($"Rotation: {degrees[i]}º {degrees[j]}º {degrees[k]}º");
                            Console.WriteLine($"({newX}, {newY}, {newZ})");
                            Console.WriteLine();
                        }
                    }
                }
            }
        }

    }
}
