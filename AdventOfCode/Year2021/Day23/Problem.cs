using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;
using System.Text;

namespace AdventOfCode.Year2021.Day23
{
    [Problem(Year = 2021, Day = 23, ProblemName = "Amphipod")]
    internal class Problem : IProblem
    {
        private const int HALLWAY_LENGTH = 11;
        private const char EMPTY_NODE_CHAR = '.';
        //private const char WALL_CHAR = '#';

        public bool Debug { get; set; } = true;


        public string Part1(string input)
        {
            Burrow burrow = GetInitialState(input);

            if (Debug)
                Console.WriteLine(burrow);

			return string.Empty;
        }

        public string Part2(string input)
        {
			return string.Empty;
        }



        private static Burrow GetInitialState(string input)
        {
            string[] lines = input.GetLines();

            Amphimod amphimodRoom1_1 = GetAmphimodFromChar(lines[2][3]);
            Amphimod amphimodRoom1_2 = GetAmphimodFromChar(lines[3][3]);
            Amphimod amphimodRoom2_1 = GetAmphimodFromChar(lines[2][5]);
            Amphimod amphimodRoom2_2 = GetAmphimodFromChar(lines[3][5]);
            Amphimod amphimodRoom3_1 = GetAmphimodFromChar(lines[2][7]);
            Amphimod amphimodRoom3_2 = GetAmphimodFromChar(lines[3][7]);
            Amphimod amphimodRoom4_1 = GetAmphimodFromChar(lines[2][9]);
            Amphimod amphimodRoom4_2 = GetAmphimodFromChar(lines[3][9]);

            return new Burrow(amphimodRoom1_1, amphimodRoom1_2, amphimodRoom2_1, amphimodRoom2_2, amphimodRoom3_1, amphimodRoom3_2, amphimodRoom4_1, amphimodRoom4_2);
        }


        private static Amphimod GetAmphimodFromChar(char amphimodChar) => amphimodChar switch
	    {
            'A' => Amphimod.Amber,
            'B' => Amphimod.Bronze,
            'C' => Amphimod.Copper,
            'D' => Amphimod.Desert,
            _ => throw new ArgumentOutOfRangeException(nameof(amphimodChar), $"Not expected Amphimod char value: {amphimodChar}"),
        };


    private class Burrow
        {
            public List<HallwayNode> Hallway { get; set; }
            public List<RoomNode> Room1 { get; set; } = new();
            public List<RoomNode> Room2 { get; set; } = new();
            public List<RoomNode> Room3 { get; set; } = new();
            public List<RoomNode> Room4 { get; set; } = new();
            public List<RoomNode> Rooms { get; set; } = new();
            public List<Node> Nodes { get; set; } = new();

            public Burrow(
                Amphimod amphimodRoom1_1, Amphimod amphimodRoom1_2, 
                Amphimod amphimodRoom2_1, Amphimod amphimodRoom2_2, 
                Amphimod amphimodRoom3_1, Amphimod amphimodRoom3_2, 
                Amphimod amphimodRoom4_1, Amphimod amphimodRoom4_2)
            {
                int i = 1;
                Hallway = Enumerable.Repeat(new HallwayNode(i++), HALLWAY_LENGTH).ToList();

                Room1.Add(new RoomNode(1, 1, amphimodRoom1_1));
                Room1.Add(new RoomNode(1, 2, amphimodRoom1_2));
                Room2.Add(new RoomNode(2, 1, amphimodRoom2_1));
                Room2.Add(new RoomNode(2, 2, amphimodRoom2_2));
                Room3.Add(new RoomNode(3, 1, amphimodRoom3_1));
                Room3.Add(new RoomNode(3, 2, amphimodRoom3_2));
                Room4.Add(new RoomNode(4, 1, amphimodRoom4_1));
                Room4.Add(new RoomNode(4, 2, amphimodRoom4_2));

                Rooms.AddRange(Room1);
                Rooms.AddRange(Room2);
                Rooms.AddRange(Room3);
                Rooms.AddRange(Room4);

                Nodes.AddRange(Hallway);
                Nodes.AddRange(Rooms);

                // TODO enlazar nodos y setear IsInFrontOfRoom
            }

            public override string ToString()
            {
                StringBuilder sb = new ();

                sb.AppendLine($"#############");
                sb.AppendLine($"#{string.Join(string.Empty, Hallway)}#");
                sb.AppendLine($"###{Room1[0]}#{Room2[0]}#{Room3[0]}#{Room4[0]}###");
                sb.AppendLine($"  #{Room1[1]}#{Room2[1]}#{Room3[1]}#{Room4[1]}#  ");
                sb.AppendLine($"  #########  ");

                return sb.ToString();
            }
        }

        private abstract class Node
        {
            public int Number { get; set; }
            public Amphimod? Amphimod { get; set; }

            public Node? TopNode { get; set; }
            public Node? RightNode { get; set; }
            public Node? BottomNode { get; set; }
            public Node? LeftNode { get; set; }

            public Node(int number, Amphimod? amphimod = null)
            {
                Number = number;
                Amphimod = amphimod;
            }

            public bool IsEmpty() => Amphimod == null;

            public override string ToString()
            {
                return IsEmpty() ? EMPTY_NODE_CHAR.ToString() : Amphimod?.ToString()?[..1] ?? String.Empty;
            }
        }

        private class RoomNode : Node
        {
            public int Room { get; set; }

            public RoomNode(int room, int number, Amphimod? amphimod = null) : base(number, amphimod)
            {
                Room = room;
            }
        }

        private class HallwayNode : Node
        {
            public bool IsInFrontOfRoom { get; set; } = false;

            public HallwayNode(int number) : base(number)
            {
            }
        }

        private enum Amphimod
        {
            Amber = 1,
            Bronze = 10,
            Copper = 100,
            Desert = 1000
        }
	}
}