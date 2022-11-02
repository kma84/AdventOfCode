using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;
using System.Collections;
using System.Text;

namespace AdventOfCode.Year2021.Day23
{
    [Problem(Year = 2021, Day = 23, ProblemName = "Amphipod")]
    internal class Problem : IProblem
    {
        private const int HALLWAY_LENGTH = 11;
        private const char EMPTY_NODE_CHAR = '.';

        public bool Debug { get; set; } = false;


        public string Part1(string input)
        {
            Burrow burrow = new();
            int? bestResult = null;

            var initialState = GetInitialState(input);
            Dictionary<List<Amphimod?>, int> cache = new(new ListComparer()) { { initialState, 0 } };
            burrow.MoveAmphimods(initialState, ref bestResult, cache);

            if (Debug)
                Console.WriteLine(burrow);

			return bestResult?.ToString() ?? String.Empty;
        }

        public string Part2(string input)
        {
			return string.Empty;
        }


        private static List<Amphimod?> GetInitialState(string input)
        {
            string[] lines = input.GetLines();

            List<Amphimod?> initialState = Enumerable.Repeat(default(Amphimod?), HALLWAY_LENGTH).ToList();
            initialState.Add(GetAmphimodFromChar(lines[2][3]));
            initialState.Add(GetAmphimodFromChar(lines[3][3]));
            initialState.Add(GetAmphimodFromChar(lines[2][5]));
            initialState.Add(GetAmphimodFromChar(lines[3][5]));
            initialState.Add(GetAmphimodFromChar(lines[2][7]));
            initialState.Add(GetAmphimodFromChar(lines[3][7]));
            initialState.Add(GetAmphimodFromChar(lines[2][9]));
            initialState.Add(GetAmphimodFromChar(lines[3][9]));

            return initialState;
        }


        private static List<Amphimod?> GetStateFromString(string stateStr) => stateStr.Select(GetAmphimodFromChar).ToList();
        private static string GetStringFromState(List<Amphimod?> state) => string.Join(string.Empty, state.Select(a => a?.ToString()?[..1] ?? EMPTY_NODE_CHAR.ToString()));


        private static Amphimod? GetAmphimodFromChar(char amphimodChar) => amphimodChar switch
	    {
            'A' => Amphimod.Amber,
            'B' => Amphimod.Bronze,
            'C' => Amphimod.Copper,
            'D' => Amphimod.Desert,
            _ => null,
        };


        private class Burrow
        {
            public List<HallwayNode> Hallway { get; set; } = new();
            public List<RoomNode> Room1 { get; set; } = new();
            public List<RoomNode> Room2 { get; set; } = new();
            public List<RoomNode> Room3 { get; set; } = new();
            public List<RoomNode> Room4 { get; set; } = new();
            public List<RoomNode> Rooms { get; set; } = new();
            public List<Node> Nodes { get; set; } = new();

            public Burrow()
            {
                for (int i = 0; i < HALLWAY_LENGTH; i++)
                    Hallway.Add(new HallwayNode(i));

                Room1.Add(new RoomNode(1, 0, Amphimod.Amber));
                Room1.Add(new RoomNode(1, 1, Amphimod.Amber));
                Room2.Add(new RoomNode(2, 0, Amphimod.Bronze));
                Room2.Add(new RoomNode(2, 1, Amphimod.Bronze));
                Room3.Add(new RoomNode(3, 0, Amphimod.Copper));
                Room3.Add(new RoomNode(3, 1, Amphimod.Copper));
                Room4.Add(new RoomNode(4, 0, Amphimod.Desert));
                Room4.Add(new RoomNode(4, 1, Amphimod.Desert));

                Rooms.AddRange(Room1);
                Rooms.AddRange(Room2);
                Rooms.AddRange(Room3);
                Rooms.AddRange(Room4);

                Nodes.AddRange(Hallway);
                Nodes.AddRange(Rooms);

                // linking nodes
                for (int i = 0; i < HALLWAY_LENGTH; i++)
                {
                    Hallway[i].LeftNode = Hallway.ElementAtOrDefault(i - 1);
                    Hallway[i].RightNode = Hallway.ElementAtOrDefault(i + 1);
                }

                LinkRoomNodes(Room1, Hallway[2]);
                LinkRoomNodes(Room2, Hallway[4]);
                LinkRoomNodes(Room3, Hallway[6]);
                LinkRoomNodes(Room4, Hallway[8]);

                static void LinkRoomNodes(List<RoomNode> room, HallwayNode hallwayNodeInFrontOfRoom)
                {
                    hallwayNodeInFrontOfRoom.IsInFrontOfRoom = true;

                    hallwayNodeInFrontOfRoom.BottomNode = room[0];
                    room[0].TopNode = hallwayNodeInFrontOfRoom;
                    room[0].BottomNode = room[1];
                    room[1].TopNode = room[0];
                }
            }


            public void MoveAmphimods(List<Amphimod?> currentState, ref int? bestResult, Dictionary<List<Amphimod?>, int> cache) // TODO cache
            {
                ///////////
                //if (GetStringFromState(currentState) == ".........A..ABBCCDD")
                //{

                //}
                /////////

                RestoreState(currentState);

                if (IsFinalPosition())
                {
                    if (!bestResult.HasValue || cache[currentState] < bestResult)
                        bestResult = cache[currentState];

                    return;
                }

                int movementEnergy;

                // Movimiento de los nodos del Hallway
                foreach (HallwayNode hallwayNode in Hallway.Where(h => !h.IsEmpty()))
                {
                    var refRoom = GetReferenceRoom(hallwayNode.Amphimod ?? default);
                    movementEnergy = 0;

                    if (refRoom[1].IsEmpty() && AmphimodCanReachTarget(hallwayNode, refRoom[1]))
                    {
                        // Movemos el amphimod al final del room
                        movementEnergy = MoveAmphimod(hallwayNode, refRoom[1], (int)(hallwayNode.Amphimod ?? 0));
                    }
                    else if (refRoom[1].IsConsolidated() && refRoom[0].IsEmpty() && AmphimodCanReachTarget(hallwayNode, refRoom[0]))
                    {
                        // Movemos el amphimod al principio del room                    
                        movementEnergy = MoveAmphimod(hallwayNode, refRoom[0], (int)(hallwayNode.Amphimod ?? 0));
                    }
                    // No hay m�s casos, si ho ha entrado en algunos de los casos anteriores, el amphimod no se puede mover

                    if (movementEnergy > 0)
                    {
                        int totalEnergy = cache[currentState] + movementEnergy;
                        List<Amphimod?> newState = GetState();

                        if (cache.TryGetValue(newState, out int cachedEnergy) && cachedEnergy <= totalEnergy)
                        {
                            RestoreState(currentState);
                            continue;
                        }

                        cache[newState] = totalEnergy;
                        MoveAmphimods(newState, ref bestResult, cache);

                        RestoreState(currentState);
                    }
                }

                // Movimiento de los nodos de los rooms
                foreach (RoomNode roomNode in Rooms.Where(r => r.AmphimodCanMove()))
                {
                    // TODO para cada espacio libre del hallway, mover amphimod 

                    foreach (HallwayNode hallwayNode in Hallway.Where(h => h.IsEmpty() && !h.IsInFrontOfRoom && AmphimodCanReachTarget(h, roomNode)))
                    {
                        movementEnergy = MoveAmphimod(hallwayNode, roomNode, (int)(roomNode.Amphimod ?? 0));

                        int totalEnergy = cache[currentState] + movementEnergy;
                        List<Amphimod?> newState = GetState();

                        if (cache.TryGetValue(newState, out int cachedEnergy) && cachedEnergy <= totalEnergy)
                        {
                            RestoreState(currentState);
                            continue;
                        }

                        cache[newState] = totalEnergy;
                        MoveAmphimods(newState, ref bestResult, cache);

                        RestoreState(currentState);
                    }
                }
            }


            private bool AmphimodCanReachTarget(HallwayNode hallwayNode, RoomNode roomNode)
            {
                if (roomNode.TopNode?.Amphimod != null)
                    return false;

                HallwayNode? hallwayNodeInFrontOfRoom = (HallwayNode?)(roomNode.Number == 0 ? roomNode.TopNode : roomNode.TopNode?.TopNode);

                int currentNumber = hallwayNodeInFrontOfRoom?.Number ?? 0;
                //while (currentNumber != hallwayNode.Number)
                //{
                //    if (true)
                //    {

                //    }
                //}

                return Hallway.Where(h => h.Number > Math.Min(currentNumber, hallwayNode.Number) && h.Number < Math.Max(currentNumber, hallwayNode.Number))
                            .All(n => n.IsEmpty());
                //return Hallway.Skip(Math.Min(hallwayNode.Number, hallwayNodeInFrontOfRoom?.Number ?? 0))
                //        .Take(Math.Abs(hallwayNode.Number - hallwayNodeInFrontOfRoom?.Number ?? 0))
                //        .All(n => n.IsEmpty());
            }


            public bool IsFinalPosition() => Rooms.All(n => n.IsConsolidated());

            public List<RoomNode> GetReferenceRoom(Amphimod amphimod) => amphimod switch
	        {
		        Amphimod.Amber  => Room1,
                Amphimod.Bronze => Room2,
                Amphimod.Copper => Room3,
                Amphimod.Desert => Room4,
                _ => throw new ArgumentOutOfRangeException(nameof(amphimod), $"Not expected Amphimod value: {amphimod}")
            };

            public void RestoreState(List<Amphimod?> state)
            {
                for (int i = 0; i < Nodes.Count; i++)
                    Nodes[i].Amphimod = state[i];
            }

            public List<Amphimod?> GetState() => Nodes.Select(n => n.Amphimod).ToList();

            public static int GetDistance(HallwayNode hallwayNode, RoomNode roomNode)
            {
                HallwayNode? hallwayNodeInFrontOfRoom = (HallwayNode?)(roomNode.Number == 0 ? roomNode.TopNode : roomNode.TopNode?.TopNode);

                return roomNode.Number + Math.Abs(hallwayNode.Number - hallwayNodeInFrontOfRoom?.Number ?? 0) + 1;
            }

            public static int GetConsumedEnergy(HallwayNode hallwayNode, RoomNode roomNode, int stepEnergy) => GetDistance(hallwayNode, roomNode) * stepEnergy;

            public static int MoveAmphimod(HallwayNode hallwayNode, RoomNode roomNode, int stepEnergy)
            {               
                (hallwayNode.Amphimod, roomNode.Amphimod) = (roomNode.Amphimod, hallwayNode.Amphimod);

                return GetConsumedEnergy(hallwayNode, roomNode, stepEnergy);
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

            public Node(int number)
            {
                Number = number;
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
            public Amphimod ReferenceAmphimod { get; set; }

            public RoomNode(int room, int number, Amphimod referenceAmphimod) : base(number)
            {
                Room = room;
                ReferenceAmphimod = referenceAmphimod;
            }

            private bool IsBlocked() => TopNode?.Amphimod != null;
            public bool IsConsolidated() => ReferenceAmphimod == Amphimod && (BottomNode == null || ((RoomNode)BottomNode).IsConsolidated());
            public bool AmphimodCanMove() => !IsEmpty() && !IsConsolidated() && !IsBlocked();
        }

        private class HallwayNode : Node
        {
            public bool IsInFrontOfRoom { get; set; } = false;

            public HallwayNode(int number) : base(number)
            {
            }
        }

        class ListComparer : EqualityComparer<List<Amphimod?>>
        {
            public override bool Equals(List<Amphimod?>? x, List<Amphimod?>? y)
              => StructuralComparisons.StructuralEqualityComparer.Equals(x?.ToArray(), y?.ToArray());

            public override int GetHashCode(List<Amphimod?> x)
              => StructuralComparisons.StructuralEqualityComparer.GetHashCode(x.ToArray());
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