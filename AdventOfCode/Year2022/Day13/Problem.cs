using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;

namespace AdventOfCode.Year2022.Day13
{
    [Problem(Year = 2022, Day = 13, ProblemName = "Distress Signal")]
    internal class Problem : IProblem
    {
        public bool Debug => false;

        public string Part1(string input)
        {
            List<Pair> pairs = GetPairs(input.GetLines());

            foreach (Pair pair in pairs)
            {
                pair.RightOrder = CompareNodes(pair.LeftNode, pair.RightNode);
            }

			return pairs.Where(p => p.RightOrder ?? false).Sum(p => p.Index).ToString();
        }

        public string Part2(string input)
        {
            List<Node> nodes = GetNodes(input.GetLines());
            List<Node> orderedNodes = new() { CreateNodeFromLine("[[2]]", true), CreateNodeFromLine("[[6]]", true) };

            foreach (Node nodeToOrder in nodes)
            {
                bool inserted = false;

                for (int i = 0; i < orderedNodes.Count; i++)
                {
                    bool? result = CompareNodes(nodeToOrder, orderedNodes[i]);
                    if (result ?? false)
                    {
                        orderedNodes.Insert(i, nodeToOrder);
                        inserted = true;
                        break;
                    }
                }

                if (!inserted)
                {
                    orderedNodes.Add(nodeToOrder);
                }
            }

            return orderedNodes.Where(n => ((ListNode)n).IsSeparatorNode).Select(n => orderedNodes.IndexOf(n) + 1).Aggregate((total, next) => total * next).ToString();
        }



        private bool? CompareNodes(Node leftNode, Node rightNode)
        {
            if (leftNode is NumericNode lnode && rightNode is NumericNode rnode)
            {
                if (lnode.Value == rnode.Value)
                {
                    return null;
                }
                else
                    return lnode.Value < rnode.Value;
            }
            else if (leftNode is NumericNode lnode2 && rightNode is ListNode)
            {
                return CompareNodes(new ListNode(lnode2.Value), rightNode);
            }
            else if (leftNode is ListNode && rightNode is NumericNode rnode2)
            {
                return CompareNodes(leftNode, new ListNode(rnode2.Value));
            }
            else if (leftNode is ListNode llistnode && rightNode is ListNode rlistnode)
            {
                int i = 0;
                Node? lsubnode;
                Node? rsubnode;

                do
                {
                    lsubnode = llistnode.SubNodes.ElementAtOrDefault(i);
                    rsubnode = rlistnode.SubNodes.ElementAtOrDefault(i);

                    if (lsubnode == null && rsubnode != null)
                    {
                        return true;
                    }
                    if (lsubnode != null && rsubnode == null)
                    {
                        return false;
                    }

                    bool? result = CompareNodes(lsubnode, rsubnode);

                    if (result != null)
                    {
                        return result;
                    }

                    i++;
                } while (lsubnode != null && rsubnode != null);
            }

            return null;
        }


        private static List<Pair> GetPairs(string[] lines)
        {
            List<Pair> pairs = new();
            int i = 0;

            while (i < lines.Length)
            {
                Node leftNode = CreateNodeFromLine(lines[i++]);
                Node rightNode = CreateNodeFromLine(lines[i++]);

                //Console.WriteLine(leftNode);
                //Console.WriteLine(rightNode);
                //Console.WriteLine();

                pairs.Add(new Pair(pairs.Count + 1, leftNode, rightNode));
                i++;
            }

            return pairs;
        }

        private static List<Node> GetNodes(string[] lines)
        {
            string[] linesFiltered = lines.Where(l => !string.IsNullOrEmpty(l)).ToArray();
            List<Node> nodes = new();

            foreach (string line in linesFiltered)
            {
                nodes.Add(CreateNodeFromLine(line));
            }

            return nodes;
        }

        private static Node CreateNodeFromLine(string line, bool isSeparatorNode = false) 
        {
            ListNode listNode = new (isSeparatorNode);
            Stack<char> stack = new ();

            for (int i = 1; i < line.Length; i++)
            {
                char c = line[i];
                if (char.IsNumber(c))
                {   
                    string numStr = c.ToString() + (char.IsNumber(line[i+1]) ? line[i + 1].ToString() : string.Empty);
                    listNode.SubNodes.Add(new NumericNode(int.Parse(numStr)));
                    if (char.IsNumber(line[i + 1]))
                    {
                        i++;
                    }
                }
                else if(c == '[')
                {
                    listNode.SubNodes.Add(CreateNodeFromLine(line[i..]));
                    stack.Push(c);

                    while (stack.Any())
                    {
                        i++;
                        char c2 = line[i];
                        if (c2 == '[')
                            stack.Push(c2);
                        else if(c2 == ']')
                            stack.Pop();
                    }
                }
                else if (c == ']')
                {
                    return listNode;
                }
            }

            return listNode;
        }

        private class Pair
        {
            public int Index { get; set; }
            public bool? RightOrder { get; set; }

            public Node LeftNode { get; set; }
            public Node RightNode { get; set; }

            public Pair(int index, Node leftNode, Node rightNode)
            {
                Index = index;
                LeftNode = leftNode;
                RightNode = rightNode;
            }

        }

        private class Node
        {
        }

        private class NumericNode : Node
        {
            public int Value { get; set; }

            public NumericNode(int value)
            {
                Value = value;
            }

            public override string ToString() => Value.ToString();
        }

        private class ListNode : Node
        {
            public List<Node> SubNodes { get; set; } = new ();
            public bool IsSeparatorNode { get; set; } = false;

            public ListNode(bool isSeparatorNode)
            {
                IsSeparatorNode= isSeparatorNode;
            }
            public ListNode(int value)
            {
                SubNodes.Add(new NumericNode(value));
            }

            public override string ToString()
            {
                string str = "[";

                foreach (Node node in SubNodes)
                {
                    str += node.ToString() + ',';
                }


                str = str.TrimEnd(',');

                return str + "]";
            }
        }
    }
}