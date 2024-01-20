using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2022.Day13
{
    [Problem(Year = 2022, Day = 13, ProblemName = "Distress Signal")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input) => GetPairs(input.GetLines())
            .Where(p => IsInRightOrder(p.LeftNode, p.RightNode) ?? false)
            .Sum(p => p.Index)
            .ToString();

        public string Part2(string input)
        {
            string separatorNode1 = "[[2]]";
            string separatorNode2 = "[[6]]";

            List<Node> nodes = GetNodes(input.GetLines());
            List<Node> orderedNodes =
            [
                CreateNodeFromLine(separatorNode1, isSeparatorNode: true),
                CreateNodeFromLine(separatorNode2, isSeparatorNode: true)
            ];

            foreach (Node nodeToOrder in nodes)
            {
                bool inserted = false;

                for (int i = 0; i < orderedNodes.Count; i++)
                {
                    bool? result = IsInRightOrder(nodeToOrder, orderedNodes[i]);
                    if (result ?? false)
                    {
                        orderedNodes.Insert(i, nodeToOrder);
                        inserted = true;
                        break;
                    }
                }

                if (!inserted)
                    orderedNodes.Add(nodeToOrder);
            }

            return orderedNodes.Where(n => n.IsSeparatorNode)
                               .Select(n => orderedNodes.IndexOf(n) + 1)
                               .Aggregate((total, next) => total * next)
                               .ToString();
        }


        private static List<Node> GetNodes(string[] lines)
        {
            string[] linesFiltered = lines.Where(l => !string.IsNullOrEmpty(l)).ToArray();
            List<Node> nodes = [];

            foreach (string line in linesFiltered)
                nodes.Add(CreateNodeFromLine(line));

            return nodes;
        }

        private List<Pair> GetPairs(string[] lines)
        {
            List<Pair> pairs = [];
            int i = 0;

            while (i < lines.Length)
            {
                Node leftNode = CreateNodeFromLine(lines[i++]);
                Node rightNode = CreateNodeFromLine(lines[i++]);

                if (Debug)
                {
                    Console.WriteLine(leftNode);
                    Console.WriteLine(rightNode);
                    Console.WriteLine();
                }

                pairs.Add(new Pair(pairs.Count + 1, leftNode, rightNode));
                i++;
            }

            return pairs;
        }

        private static Node CreateNodeFromLine(string line, bool isSeparatorNode = false)
        {
            int index = 1;
            return CreateNodeFromLine(line, ref index, isSeparatorNode);
        }

        private static Node CreateNodeFromLine(string line, ref int index, bool isSeparatorNode = false)
        {
            Node node = new(isSeparatorNode);

            while (index < line.Length)
            {
                char c = line[index];

                switch (c)
                {
                    case char numberChar when char.IsNumber(numberChar):
                        node.SubNodes.Add(new() { Value = GetNodeValue(c, line[index + 1], ref index) });
                        break;
                    case START_NODE_CHAR:
                        index++;
                        node.SubNodes.Add(CreateNodeFromLine(line, ref index));
                        break;
                    case END_NODE_CHAR:
                        return node;
                }

                index++;
            }

            return node;
        }

        private static int GetNodeValue(char c1, char c2, ref int index)
        {
            string numStr = c1.ToString();

            if (char.IsNumber(c2))
            {
                numStr += c2.ToString();
                index++;
            }

            return int.Parse(numStr);
        }

        private static bool? IsInRightOrder(Node? leftNode, Node? rightNode) => (leftNode, rightNode) switch
        {
            (null, null) => null,
            (Node l, Node r) when l.Value.HasValue && r.Value.HasValue && l.Value == r.Value => null,
            (Node l, Node r) when l.Value.HasValue && r.Value.HasValue => l.Value < r.Value,
            (Node l, Node r) when l.Value.HasValue && !r.Value.HasValue => IsInRightOrder(new Node(l.Value.Value), r),
            (Node l, Node r) when !l.Value.HasValue && r.Value.HasValue => IsInRightOrder(l, new Node(r.Value.Value)),
            _ => CheckSubnodesOrder(leftNode, rightNode),
        };

        private static bool? CheckSubnodesOrder(Node? leftNode, Node? rightNode)
        {
            int i = 0;
            Node? lsubnode;
            Node? rsubnode;

            do
            {
                lsubnode = leftNode?.SubNodes.ElementAtOrDefault(i);
                rsubnode = rightNode?.SubNodes.ElementAtOrDefault(i);

                if (lsubnode == null && rsubnode != null)
                    return true;

                if (lsubnode != null && rsubnode == null)
                    return false;

                bool? result = IsInRightOrder(lsubnode, rsubnode);

                if (result != null)
                    return result;

                i++;
            } while (lsubnode != null && rsubnode != null);

            return null;
        }


        private const char START_NODE_CHAR = '[';
        private const char END_NODE_CHAR = ']';

        private record Pair(int Index, Node LeftNode, Node RightNode);

        private class Node
        {
            public int? Value { get; set; }
            public bool IsSeparatorNode { get; set; }
            public List<Node> SubNodes { get; set; } = [];

            public Node(bool isSeparatorNode = false)
            {
                IsSeparatorNode = isSeparatorNode;
            }

            public Node(int subnodeValue)
            {
                SubNodes.Add(new Node() { Value = subnodeValue });
            }

            public override string ToString() => Value.HasValue ? Value.Value.ToString() : START_NODE_CHAR + string.Join(",", SubNodes) + END_NODE_CHAR;
        }
    }
}