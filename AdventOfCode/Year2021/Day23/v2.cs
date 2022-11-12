using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Year2021.Day23
{
    internal class V2
    {
        private const int HALLWAY_LENGTH = 11;
        private const char EMPTY_NODE_CHAR = '.';


        public string Part1(string input)
        {
            Burrow burrow = new(roomCapacity: 2);
            string initialState = GetInitialState(input);

            int bestResult = burrow.GetBestResult(initialState);

            return bestResult.ToString();
        }

        public string Part2(string input)
        {
            Burrow burrow = new(roomCapacity: 4);
            string initialState = GetInitialState(input, part2Extension: true);

            int bestResult = burrow.GetBestResult(initialState);

            return bestResult.ToString();
        }


        private static string GetInitialState(string input, bool part2Extension = false)
        {
            string[] lines = input.GetLines();

            List<char> initialState = Enumerable.Repeat(EMPTY_NODE_CHAR, HALLWAY_LENGTH).ToList();

            List<char> room1 = new() { lines[2][3] };
            List<char> room2 = new() { lines[2][5] };
            List<char> room3 = new() { lines[2][7] };
            List<char> room4 = new() { lines[2][9] };

            if (part2Extension)
            {
                room1.Add(Amphimod.Desert.ToString()[0]);
                room1.Add(Amphimod.Desert.ToString()[0]);

                room2.Add(Amphimod.Copper.ToString()[0]);
                room2.Add(Amphimod.Bronze.ToString()[0]);

                room3.Add(Amphimod.Bronze.ToString()[0]);
                room3.Add(Amphimod.Amber.ToString()[0]);

                room4.Add(Amphimod.Amber.ToString()[0]);
                room4.Add(Amphimod.Copper.ToString()[0]);
            }

            room1.Add(lines[3][3]);
            room2.Add(lines[3][5]);
            room3.Add(lines[3][7]);
            room4.Add(lines[3][9]);

            initialState.AddRange(room1);
            initialState.AddRange(room2);
            initialState.AddRange(room3);
            initialState.AddRange(room4);

            return new string(initialState.ToArray());
        }


        private class Burrow
        {
            public List<HallwayNode> Hallway { get; set; } = new();
            public List<RoomNode> Room1 { get; set; } = new();
            public List<RoomNode> Room2 { get; set; } = new();
            public List<RoomNode> Room3 { get; set; } = new();
            public List<RoomNode> Room4 { get; set; } = new();
            public List<RoomNode> Rooms { get; set; } = new();
            public List<Node> Nodes { get; set; } = new();

            public Burrow(int roomCapacity)
            {
                for (int i = 0; i < HALLWAY_LENGTH; i++)
                    Hallway.Add(new HallwayNode(i));

                CreateRoomNodes(Room1, 1, roomCapacity, Amphimod.Amber);
                CreateRoomNodes(Room2, 2, roomCapacity, Amphimod.Bronze);
                CreateRoomNodes(Room3, 3, roomCapacity, Amphimod.Copper);
                CreateRoomNodes(Room4, 4, roomCapacity, Amphimod.Desert);

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

            private void CreateRoomNodes(List<RoomNode> room, int roomNumber, int roomCapacity, Amphimod referenceAmphimod)
            {
                for (int i = 0; i < roomCapacity; i++)
                    room.Add(new RoomNode(roomNumber, i, referenceAmphimod));
            }


            public int GetBestResult(string initialState)
            {
                Queue<string> newStates = new();
                newStates.Enqueue(initialState);

                int? bestResult = null;
                Dictionary<string, int> cache = new() { { initialState, 0 } };

                while (newStates.TryDequeue(out string? currentState))
                {
                    RestoreState(currentState);

                    int movementEnergy;
                    bool pathFinished = false;

                    // Movimiento de los nodos del Hallway
                    foreach (HallwayNode hallwayNode in Hallway.Where(h => !h.IsEmpty()))
                    {
                        var refRoom = GetReferenceRoom(hallwayNode.Amphimod ?? default);
                        movementEnergy = 0;

                        for (int i = refRoom.Count - 1; i >= 0; i--)
                        {
                            if ((refRoom.ElementAtOrDefault(i + 1)?.IsConsolidated() ?? true) && refRoom[i].IsEmpty() && AmphimodCanReachTarget(hallwayNode, refRoom[i]))
                            {
                                movementEnergy = MoveAmphimod(hallwayNode, refRoom[i], (int)(hallwayNode.Amphimod ?? 0));
                                break;
                            }
                        }

                        // No hay más casos, si ho ha entrado en algunos de los casos anteriores, el amphimod no se puede mover

                        if (movementEnergy > 0)
                        {
                            int totalEnergy = cache[currentState] + movementEnergy;
                            string newState = GetState();

                            if (PathEnergyGtePreviousResults(totalEnergy, bestResult, newState, cache))
                            {
                                pathFinished = true;
                                break;
                            }

                            cache[newState] = totalEnergy;
                            currentState = newState;

                            if (IsFinalPosition())
                            {
                                if (!bestResult.HasValue || totalEnergy < bestResult)
                                    bestResult = totalEnergy;

                                pathFinished = true;
                                break;
                            }

                        }
                    }

                    if (!pathFinished)
                    {
                        // Movimiento de los nodos de los rooms
                        foreach (RoomNode roomNode in Rooms.Where(r => r.AmphimodCanMove()))
                        {
                            foreach (HallwayNode hallwayNode in Hallway.Where(h => h.IsEmpty() && !h.IsInFrontOfRoom && AmphimodCanReachTarget(h, roomNode)))
                            {
                                movementEnergy = MoveAmphimod(hallwayNode, roomNode, (int)(roomNode.Amphimod ?? 0));

                                int totalEnergy = cache[currentState] + movementEnergy;
                                string newState = GetState();

                                if (PathEnergyLtPreviousResults(totalEnergy, bestResult, newState, cache))
                                {
                                    newStates.Enqueue(newState);
                                    cache[newState] = totalEnergy;
                                }

                                RestoreState(currentState);
                            }
                        }
                    }
                    
                }

                return bestResult ?? 0;
            }


            private static List<Amphimod?> GetLstAmphimodsFromStateString(string stateStr) => stateStr.Select(GetAmphimodFromChar).ToList();

            private static string GetStateStringFromLstAmphimods(List<Amphimod?> state)
                => string.Join(string.Empty, state.Select(a => a?.ToString()?[..1] ?? EMPTY_NODE_CHAR.ToString()));

            private static Amphimod? GetAmphimodFromChar(char amphimodChar) => amphimodChar switch
            {
                'A' => Amphimod.Amber,
                'B' => Amphimod.Bronze,
                'C' => Amphimod.Copper,
                'D' => Amphimod.Desert,
                _ => null,
            };

            private static bool PathEnergyGtePreviousResults(int totalEnergy, int? bestResult, string newState, Dictionary<string, int> cache)
            {
                return (bestResult != null && totalEnergy >= bestResult.Value)
                    || cache.TryGetValue(newState, out int cachedEnergy) && cachedEnergy <= totalEnergy;
            }

            private static bool PathEnergyLtPreviousResults(int totalEnergy, int? bestResult, string newState, Dictionary<string, int> cache)
            {
                return (!cache.ContainsKey(newState) || totalEnergy < cache[newState]) && (!bestResult.HasValue || totalEnergy < bestResult);
            }

            private bool AmphimodCanReachTarget(HallwayNode hallwayNode, RoomNode roomNode)
            {
                if (roomNode.TopNode?.Amphimod != null)
                    return false;

                HallwayNode? hallwayNodeInFrontOfRoom = Hallway[roomNode.Room * 2];

                int currentNumber = hallwayNodeInFrontOfRoom?.Number ?? 0;

                return Hallway.Where(h => h.Number > Math.Min(currentNumber, hallwayNode.Number) && h.Number < Math.Max(currentNumber, hallwayNode.Number))
                            .All(n => n.IsEmpty());
            }

            private bool IsFinalPosition() => Rooms.All(n => n.IsConsolidated());

            private List<RoomNode> GetReferenceRoom(Amphimod amphimod) => amphimod switch
            {
                Amphimod.Amber => Room1,
                Amphimod.Bronze => Room2,
                Amphimod.Copper => Room3,
                Amphimod.Desert => Room4,
                _ => throw new ArgumentOutOfRangeException(nameof(amphimod), $"Not expected Amphimod value: {amphimod}")
            };

            private void RestoreState(string state)
            {
                List<Amphimod?> amphimods = GetLstAmphimodsFromStateString(state);

                for (int i = 0; i < Nodes.Count; i++)
                    Nodes[i].Amphimod = amphimods[i];
            }

            private string GetState() => GetStateStringFromLstAmphimods(Nodes.Select(n => n.Amphimod).ToList());

            private int GetDistance(HallwayNode hallwayNode, RoomNode roomNode)
            {
                HallwayNode? hallwayNodeInFrontOfRoom = Hallway[roomNode.Room * 2];

                return roomNode.Number + Math.Abs(hallwayNode.Number - hallwayNodeInFrontOfRoom?.Number ?? 0) + 1;
            }

            private int GetConsumedEnergy(HallwayNode hallwayNode, RoomNode roomNode, int stepEnergy) => GetDistance(hallwayNode, roomNode) * stepEnergy;

            private int MoveAmphimod(HallwayNode hallwayNode, RoomNode roomNode, int stepEnergy)
            {
                (hallwayNode.Amphimod, roomNode.Amphimod) = (roomNode.Amphimod, hallwayNode.Amphimod);

                return GetConsumedEnergy(hallwayNode, roomNode, stepEnergy);
            }

            public override string ToString()
            {
                StringBuilder sb = new();

                sb.AppendLine($"#############");
                sb.AppendLine($"#{string.Join(string.Empty, Hallway)}#");
                sb.AppendLine($"###{Room1[0]}#{Room2[0]}#{Room3[0]}#{Room4[0]}###");

                for (int i = 1; i < Room1.Count; i++)
                    sb.AppendLine($"  #{Room1[i]}#{Room2[i]}#{Room3[i]}#{Room4[i]}#  ");

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

        private enum Amphimod
        {
            Amber = 1,
            Bronze = 10,
            Copper = 100,
            Desert = 1000
        }
    }
}
