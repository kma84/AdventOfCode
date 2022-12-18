
namespace AdventOfCode.Utils.Geometry
{
    public static class Calculations
    {

        /// <summary>
        /// <see cref="https://en.wikipedia.org/wiki/Barycentric_coordinate_system"/>
        /// </summary>
        public static bool PointBelongsToTriangle(Point point, Point pointA, Point pointB, Point pointC)
        {
            double denominador = ((pointB.Y - pointC.Y) * (pointA.X - pointC.X) + (pointC.X - pointB.X) * (pointA.Y - pointC.Y));
            double a = ((pointB.Y - pointC.Y) * (point.X - pointC.X) + (pointC.X - pointB.X) * (point.Y - pointC.Y)) / denominador;
            double b = ((pointC.Y - pointA.Y) * (point.X - pointC.X) + (pointA.X - pointC.X) * (point.Y - pointC.Y)) / denominador;
            double c = 1 - a - b;

            return 0 <= a && a <= 1 && 0 <= b && b <= 1 && 0 <= c && c <= 1;
        }

        /// <summary>
        /// <see cref="https://math.stackexchange.com/a/190373"/>
        /// </summary>
        public static bool PointBelongsToRectangle(Point pointM, Point pointA, Point pointB, Point pointD)
        {
            static int dot((int x, int y) u, (int x, int y) v) => u.x * v.x + u.y * v.y;

            var AB = ( x: pointB.X - pointA.X, y: pointB.Y - pointA.Y );
            var AD = ( x: pointD.X - pointA.X, y: pointD.Y - pointA.Y );
            var AM = ( x: pointM.X - pointA.X, y: pointM.Y - pointA.Y );

            int dotAMAB = dot(AM, AB);
            int dotABAB = dot(AB, AB);
            int dotAMAD = dot(AM, AD);
            int dotADAD = dot(AD, AD);

            return 0 <= dotAMAB && dotAMAB <= dotABAB && 0 <= dotAMAD && dotAMAD <= dotADAD;
        }

        public static double GetDistanceBetweenPoints(Point pointA, Point pointB)
        {
            return Math.Sqrt(Math.Pow(pointB.X - pointA.X, 2) + Math.Pow(pointB.Y - pointA.Y, 2));
        }

        public static int GetManhattanDistance(Point pointA, Point pointB)
        {
            return Math.Abs(pointA.X - pointB.X) + Math.Abs(pointA.Y - pointB.Y);
        }

        public static int GetManhattanDistance(Point3D pointA, Point3D pointB)
        {
            return Math.Abs(pointA.X - pointB.X) + Math.Abs(pointA.Y - pointB.Y) + Math.Abs(pointA.Z - pointB.Z);
        }

    }
}
