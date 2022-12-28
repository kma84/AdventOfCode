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
        public bool Debug => true;

        public string Part1(string input)
        {
            List<Node> nodes = GetNodes(input.GetLines());
            Dictionary<(string source, string target), int> pathsLength = GetPathsLength(nodes);
            List<TreeNode> expandedTree = CreateExpandedTreePart1(nodes, pathsLength, minutes: 30);

            return GetLongestPathLength(expandedTree).ToString();
        }

        public string Part2(string input)
        {
            List<Node> nodes = GetNodes(input.GetLines());
            Dictionary<(string source, string target), int> pathsLength = GetPathsLength(nodes);
            List<TreeNode> expandedTree = CreateExpandedTreePart2(nodes, pathsLength, minutes: 26);

            return GetLongestPathLength(expandedTree).ToString();
            //return string.Empty;
        }


        private static int GetLongestPathLength(List<TreeNode> expandedTree)
        {
            Dictionary<TreeNode, int> accumulatedFlowRate = new() { { expandedTree.First(), 0 } };

            foreach (TreeNode source in expandedTree)
            {
                foreach (TreeNode target in source.AdjacentNodes.Cast<TreeNode>())
                {
                    if (accumulatedFlowRate.GetValueOrDefault(target) <= target.FlowRate)
                    {
                        accumulatedFlowRate[target] = target.FlowRate + accumulatedFlowRate.GetValueOrDefault(source);
                    }
                }
            }

            return accumulatedFlowRate.Max(kvp => kvp.Value);
        }

        private static List<TreeNode> CreateExpandedTreePart1(List<Node> nodes, Dictionary<(string source, string target), int> pathsLength, int minutes)
        {
            var nodesWithFlowRatePositive = nodes.Where(n => n.FlowRate > 0).ToImmutableList();
            Dictionary<string, int> cache = new ();

            TreeNode root = new(nodes[0].Name) { ClosedNodes = nodesWithFlowRatePositive };            
            root.AdjacentNodes.AddRange(GetDecendentsPart1(root, pathsLength, minutes, cache));

            return TopologicalSort(root);
        }

        private static List<Node> GetDecendentsPart1(TreeNode current, Dictionary<(string source, string target), int> pathsLength, int minutes, Dictionary<string, int> cache)
        {
            List<Node> branch = new();

            if (minutes > 2)
            {
                foreach (Node next in current.ClosedNodes)
                {
                    int remainingMinutes = minutes - pathsLength[(current.Name, next.Name)];

                    if (remainingMinutes > 0)
                    {
                        TreeNode newNode = new(next.Name)
                        {
                            FlowRate = remainingMinutes * next.FlowRate,
                            ClosedNodes = current.ClosedNodes.Remove(next)
                        };



                        if (newNode.ClosedNodes.Any())
                        {
                            string key = GetKeyNodeStr(newNode, remainingMinutes);

                            if (cache.ContainsKey(key))
                            {
                                newNode.FlowRate = cache[key];
                            }
                            else
                            {
                                newNode.AdjacentNodes.AddRange(GetDecendentsPart1(newNode, pathsLength, remainingMinutes, cache));

                                if (newNode.ClosedNodes.Any())
                                {
                                    cache.Add(key, newNode.FlowRate + GetLongestPathLength(TopologicalSort(newNode)));
                                }
                            }
                        }

                            

                        branch.Add(newNode);
                    }
                }
            }

            return branch;
        }

        private static string GetKeyNodeStr(TreeNode treeNode, int remainingMinutes) => 
            treeNode.Name + "_" + string.Join(string.Empty, treeNode.ClosedNodes.Select(cn => cn.Name).OrderBy(n => n)) + "_" + remainingMinutes;

        private static List<TreeNode> CreateExpandedTreePart2(List<Node> nodes, Dictionary<(string source, string target), int> pathsLength, int minutes)
        {
            var nodesWithFlowRatePositive = nodes.Where(n => n.FlowRate > 0).ToImmutableList();
            string nodeName = nodes[0].Name;

            TreeNode root = new(nodeName) 
            {
                CurrentNodeName = nodeName,
                ElephantCurrentNodeName = nodeName,
                ClosedNodes = nodesWithFlowRatePositive 
            };
            root.AdjacentNodes.AddRange(GetDecendentsPart2(root, pathsLength, minutes, minutes, true));

            return TopologicalSort(root);
        }

        private static List<Node> GetDecendentsPart2(TreeNode current, Dictionary<(string source, string target), int> pathsLength, int minutes, int elephantMinutes, bool firstIteration)
        {
            List<Node> branch = new();

            foreach ((Node? next, Node? elephantNext) in GetPermutations(current, firstIteration))
            {
                int remainingMinutes = next != null ? Math.Max(minutes - pathsLength[(current.CurrentNodeName, next.Name)], 0) : 0;
                int elephantRemainingMinutes = elephantNext != null ? Math.Max(elephantMinutes - pathsLength[(current.ElephantCurrentNodeName, elephantNext.Name)], 0) : 0;

                if (remainingMinutes > 0 || elephantRemainingMinutes > 0)
                {
                    ImmutableList<Node> closedNodes = current.ClosedNodes;

                    if (next != null)
                        closedNodes = closedNodes.Remove(next);
                    if (elephantNext != null)
                        closedNodes = closedNodes.Remove(elephantNext);

                    TreeNode newNode = new($"{next?.Name ?? string.Empty}-{elephantNext?.Name ?? string.Empty}")
                    {
                        FlowRate = remainingMinutes * (next?.FlowRate ?? 0) + elephantRemainingMinutes * (elephantNext?.FlowRate ?? 0),
                        ClosedNodes = closedNodes,
                        CurrentNodeName = next?.Name ?? current.CurrentNodeName,
                        ElephantCurrentNodeName = elephantNext?.Name ?? current.ElephantCurrentNodeName
                    };

                    newNode.AdjacentNodes.AddRange(GetDecendentsPart2(newNode, pathsLength, remainingMinutes, elephantRemainingMinutes, false));
                    branch.Add(newNode);
                }
                else
                    break;
            }

            return branch;
        }

        private static List<(Node? next, Node? elephantNext)> GetPermutations(TreeNode current, bool firstIteration)
        {
            List<(Node? next, Node? elephantNext)> permutations = new();

            List<Node?> closedNodes = current.ClosedNodes.Cast<Node?>().ToList();
            if (closedNodes.Count == 1)
                closedNodes.Add(null);

            for (int i = 0; i < closedNodes.Count; i++)
            {
                for (int j = firstIteration ? i + 1 : 0; j < closedNodes.Count; j++)
                {
                    if (i == j)
                        continue;

                    permutations.Add((closedNodes[i], closedNodes[j]));
                }
            }

            return permutations;
        }

        private static List<TreeNode> TopologicalSort(TreeNode startNode)
        {
            List<TreeNode> sortedNodes = new();
            Queue<TreeNode> nodesWithNoIncomingEdges = new();
            nodesWithNoIncomingEdges.Enqueue(startNode);

            while (nodesWithNoIncomingEdges.TryDequeue(out TreeNode? current) && current != null)
            {
                sortedNodes.Add(current);

                foreach (TreeNode next in current.AdjacentNodes.Cast<TreeNode>())
                {
                    nodesWithNoIncomingEdges.Enqueue(next);
                }
            }

            return sortedNodes;
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

        private class Node
        {
            public List<Node> AdjacentNodes { get; set; } = new();
            public int FlowRate { get; set; }
            public string Name { get; set; }

            public Node(string name)
            {
                Name = name;
            }
        }

        private class TreeNode : Node
        {
            // Closed valves in the universe of this node (for the expanded tree)
            public ImmutableList<Node> ClosedNodes { get; set; } = ImmutableList<Node>.Empty;
            //public int AccumulatedFlowRate { get; set; }

            // Exclusive properties for part two
            //public int ElephantFlowRate { get; set; }
            public string CurrentNodeName { get; set; } = string.Empty;
            public string ElephantCurrentNodeName { get; set; } = string.Empty;

            public TreeNode(string name) : base(name) {}
        }

        [GeneratedRegex("Valve ([A-Z]{2}) has flow rate=(\\d+); tunnels? leads? to valves? (.*?)$")]
        private static partial Regex InputRegex();
    }
}