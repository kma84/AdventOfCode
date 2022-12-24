using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace AdventOfCode.Year2022.Day16
{
    [Problem(Year = 2022, Day = 16, ProblemName = "Proboscidea Volcanium")]
    internal partial class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input)
        {
            List<Node> nodes = GetNodes(input.GetLines());
            Dictionary<(string source, string target), int> pathsLength = GetPathsLength(nodes);
            List<Node> expandedTree = CreateExpandedTree(nodes, pathsLength);

            return GetLongestPathLength(expandedTree).ToString();
        }

        public string Part2(string input)
        {
			return string.Empty;
        }


        private static long GetLongestPathLength(List<Node> expandedTree)
        {
            foreach (Node source in expandedTree)
            {
                foreach (Node target in source.AdjacentNodes)
                {
                    if (target.AccumulatedFlowRate <= target.FlowRate)
                    {
                        target.AccumulatedFlowRate = target.FlowRate + source.AccumulatedFlowRate;
                    }
                }
            }

            return expandedTree.Max(n => n.AccumulatedFlowRate);
        }

        private static List<Node> CreateExpandedTree(List<Node> nodes, Dictionary<(string source, string target), int> pathsLength)
        {
            var nodesWithFlowRatePositive = nodes.Where(n => n.FlowRate > 0).ToImmutableList();

            Node root = new(nodes[0].Name) { ClosedNodes = nodesWithFlowRatePositive };            
            root.AdjacentNodes.AddRange(GetDecendents(root, pathsLength, 30));

            return TopologicalSort(new() { root });
        }

        private static List<Node> GetDecendents(Node current, Dictionary<(string source, string target), int> pathsLength, int minutes)
        {
            List<Node> branch = new();

            if (minutes > 2)
            {
                foreach (Node next in current.ClosedNodes)
                {
                    int remainingMinutes = minutes - pathsLength[(current.Name, next.Name)];

                    if (remainingMinutes > 0)
                    {
                        Node newNode = new(next.Name)
                        {
                            FlowRate = remainingMinutes * next.FlowRate,
                            ClosedNodes = current.ClosedNodes.Remove(next)
                        };

                        newNode.AdjacentNodes.AddRange(GetDecendents(newNode, pathsLength, remainingMinutes));
                        branch.Add(newNode);
                    }
                }
            }

            return branch;
        }

        private static List<Node> GetNodes(string[] lines)
        {
            SortedList<string, Node> nodes = new();

            foreach (string line in lines) 
            {
                Match match = InputRegex().Match(line);
                string nodeName = match.Groups[1].Value;

                Node currentNode = GetOrCreateNode(nodes, nodeName);
                currentNode.FlowRate = int.Parse(match.Groups[2].Value);

                foreach (var nodeStr in match.Groups[3].Value.Split(',', StringSplitOptions.TrimEntries))
                {
                    currentNode.AdjacentNodes.Add(GetOrCreateNode(nodes, nodeStr));
                }
            }

            return nodes.Values.ToList();

            static Node GetOrCreateNode(SortedList<string, Node> nodes, string nodeName)
            {
                if (!nodes.TryGetValue(nodeName, out Node? node))
                {
                    node = new Node(nodeName);
                    nodes.Add(nodeName, node);
                }

                return node;
            }
        }

        private static Dictionary<(string source, string target), int> GetPathsLength(List<Node> nodes)
        {
            Dictionary<(string source, string target), int> pathsLength = new();

            foreach (Node node in nodes)
            {
                Dictionary<Node, List<Node>> paths = BFSShortestPaths(node);

                foreach (var kvPath in paths)
                {
                    pathsLength.Add((node.Name, kvPath.Key.Name), kvPath.Value.Count);
                }
            }

            return pathsLength;
        }

        private static Dictionary<Node, List<Node>> BFSShortestPaths(Node source)
        {
            Queue<Node> frontier = new();
            frontier.Enqueue(source);

            Dictionary<Node, Node?> cameFrom = new() { { source, default } };

            while (frontier.TryDequeue(out Node? current))
            {
                foreach (Node next in current.AdjacentNodes)
                {
                    if (!cameFrom.ContainsKey(next))
                    {
                        frontier.Enqueue(next);
                        cameFrom[next] = current;
                    }
                }
            }

            return cameFrom.ToDictionary(kvp => kvp.Key, kvp => GetPath(cameFrom, kvp.Key));
        }

        private static List<Node> GetPath(Dictionary<Node, Node?> prev, Node target)
        {
            Stack<Node> path = new();
            Node? currentPathNode = target;

            while (currentPathNode != default)
            {
                path.Push(currentPathNode);
                currentPathNode = prev[currentPathNode];
            }

            return path.ToList();
        }

        private static List<Node> TopologicalSort(List<Node> startNodes)
        {
            List<Node> sortedNodes = new();
            Queue<Node> nodesWithNoIncomingEdges = new(startNodes);

            while (nodesWithNoIncomingEdges.TryDequeue(out Node? current) && current != null)
            {
                sortedNodes.Add(current);

                foreach (Node next in current.AdjacentNodes)
                {
                    nodesWithNoIncomingEdges.Enqueue(next);
                }
            }

            return sortedNodes;
        }


        private class Node
        {
            public string Name { get; set; }
            public int FlowRate { get; set; }
            public List<Node> AdjacentNodes { get; set; } = new();

            // Closed valves in the universe of this node (for the expanded tree)
            public ImmutableList<Node> ClosedNodes { get; set; } = ImmutableList<Node>.Empty;
            public long AccumulatedFlowRate { get; set; }

            public Node(string name)
            {
                Name = name;
            }
        }

        [GeneratedRegex("Valve ([A-Z]{2}) has flow rate=(\\d+); tunnels? leads? to valves? (.*?)$")]
        private static partial Regex InputRegex();
    }
}