using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2022.Day09
{
    [Problem(Year = 2022, Day = 9, ProblemName = "Rope Bridge")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input)
        {
            Point head = new(0, 0);
            Point tail = new(0, 0);
            HashSet<Point> visitedPoints = [tail];

            foreach (Movement movement in GetMovements(input.GetLines()))
            {
                (int xModifier, int yModifier) = GetCoordsModifiers(movement);

                for (int i = 1; i <= movement.NumSteps; i++)
                {
                    // head movement
                    head = new Point(head.X + xModifier, head.Y + yModifier);

                    // tail movement
                    if (GetMaxDistance(head, tail) > 1)
                    {
                        tail = GetNewTailPoint(head, tail);
                        visitedPoints.Add(tail);
                    }
                }
            }

            return visitedPoints.Count.ToString();
        }

        public string Part2(string input)
        {
            List<Point> points = Enumerable.Repeat(new Point(0, 0), 10).ToList();
            HashSet<Point> visitedPoints = [points.Last()];

            foreach (Movement movement in GetMovements(input.GetLines()))
            {
                (int xModifier, int yModifier) = GetCoordsModifiers(movement);

                for (int n = 1; n <= movement.NumSteps; n++)
                {
                    // head movement
                    points[0] = new Point(points[0].X + xModifier, points[0].Y + yModifier);
                    
                    // tails movements
                    for (int i = 1; i < points.Count; i++)                        
                        points[i] = GetNewTailPoint(points[i - 1], points[i]);

                    visitedPoints.Add(points.Last());
                }
            }

            return visitedPoints.Count.ToString();
        }


        private static (int xModifier, int yModifier) GetCoordsModifiers(Movement movement) => movement.Direction switch
        {
            DirectionEnum.UP    => (0, 1),
            DirectionEnum.DOWN  => (0, -1),
            DirectionEnum.RIGHT => (1, 0),
            DirectionEnum.LEFT  => (-1, 0),
            _ => throw new NotImplementedException()
        };

        private static Point GetNewTailPoint(Point head, Point tail)
        {
            int newX = tail.X;
            int newY = tail.Y;

            if (Math.Abs(head.X - tail.X) > 1)
            {
                (newX, newY) = GetNewCoords(head.X, head.Y, tail.X, tail.Y);
            }
            else if (Math.Abs(head.Y - tail.Y) > 1)
            {
                (newY, newX) = GetNewCoords(head.Y, head.X, tail.Y, tail.X);
            }

            return new Point(newX, newY);

            static (int newCoord1, int newCoord2) GetNewCoords(int hCoord1, int hCoord2, int tCoord1, int tCoord2)
            {
                int newCoord1 = hCoord1 > tCoord1 ? hCoord1 - 1 : hCoord1 + 1;
                int newCoord2 = tCoord2;

                int dif = Math.Abs(hCoord2 - tCoord2);

                if (dif == 1)
                    newCoord2 = hCoord2;
                else if (dif > 1)
                    newCoord2 = hCoord2 > tCoord2 ? hCoord2 - 1 : hCoord2 + 1;

                return (newCoord1, newCoord2);
            }
        }

        private static int GetMaxDistance(Point head, Point tail) => Math.Max(Math.Abs(head.X - tail.X), Math.Abs(head.Y - tail.Y));

        private static IEnumerable<Movement> GetMovements(string[] lines)
        {
            foreach (string line in lines)
            {
                string[] parts = line.Split();

                yield return new Movement((DirectionEnum)parts[0][0], int.Parse(parts[1]));
            }
        }


        private record Point(int X, int Y);

        private record Movement(DirectionEnum Direction, int NumSteps);

        // The values of the items are the ASCII codes of the initials ('U', 'D', 'L', 'R')
        private enum DirectionEnum
        {
            UP = 85,    
            DOWN = 68,  
            LEFT = 76,
            RIGHT = 82
        }
    }
}