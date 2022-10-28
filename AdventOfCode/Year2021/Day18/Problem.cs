using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;

namespace AdventOfCode.Year2021.Day18
{
    [Problem(Year = 2021, Day = 18, ProblemName = "Snailfish")]
    internal class Problem : IProblem
    {
        public bool Debug { get; set; } = false;

        public string Part1(string input)
        {
            var numbers = GetInput(input);

            (SnailfishNumber result, _) = numbers.Aggregate((result, number) => Add(result, number));
            int magnitude = result.GetMagnitude();

            if (Debug)
            {
                Console.WriteLine("Sum:");
                Console.WriteLine(result);
                Console.WriteLine("Magnitude: " + result.GetMagnitude());
            }

            return magnitude.ToString();
        }

        public string Part2(string input)
        {
            List<string> numbersStr = GetInputAsStr(input);

            int maxMagnitude = 0;

            for (int i = 0; i < numbersStr.Count; i++)
            {
                for (int j = numbersStr.Count - 1; j >= 0; j--)
                {
                    if (i == j)
                        continue;

                    (SnailfishNumber result, _) = Add(ParseNumber(numbersStr[i]), ParseNumber(numbersStr[j]));
                    maxMagnitude = Math.Max(maxMagnitude, result.GetMagnitude());
                }
            }

            return maxMagnitude.ToString();
        }


        private static (SnailfishNumber, List<RegularNumber>) Add(
            (SnailfishNumber number, List<RegularNumber> regularNumbers) left,
            (SnailfishNumber number, List<RegularNumber> regularNumbers) right)
        {
            PairNumber result = new() { Left = left.number, Right = right.number };
            List<RegularNumber> newRegNumbers = new(left.regularNumbers);
            newRegNumbers.AddRange(right.regularNumbers);

            while (Explode(newRegNumbers) || Split(newRegNumbers)) { }

            return (result, newRegNumbers);
        }


        private static bool Explode(List<RegularNumber> regNumbers)
        {
            int pIndex = regNumbers.FindIndex(rn => rn.GetDepth() >= 5);

            if (pIndex != -1)
            {
                PairNumber? numberToExplode = (PairNumber?)regNumbers[pIndex].Parent;

                RegularNumber newNumber = new(0);
                regNumbers.RemoveAt(pIndex);
                regNumbers[pIndex] = newNumber;

                if (pIndex > 0)
                    regNumbers[pIndex - 1].Value += ((RegularNumber?)numberToExplode?.Left)?.Value ?? 0;

                if (pIndex < regNumbers.Count - 1)
                    regNumbers[pIndex + 1].Value += ((RegularNumber?)numberToExplode?.Right)?.Value ?? 0;

                if (numberToExplode?.Parent != null)
                {
                    if (numberToExplode.IsLeftNumber)
                        ((PairNumber)numberToExplode.Parent).Left = newNumber;
                    else
                        ((PairNumber)numberToExplode.Parent).Right = newNumber;
                }

                return true;
            }

            return false;
        }


        private static bool Split(List<RegularNumber> regNumbers)
        {
            int rnIndex = regNumbers.FindIndex(rn => rn.Value >= 10);

            if (rnIndex != -1)
            {
                RegularNumber numberToSplit = regNumbers[rnIndex];

                PairNumber newNumber = new()
                {
                    Left = new RegularNumber(numberToSplit.Value / 2),
                    Right = new RegularNumber(numberToSplit.Value / 2 + numberToSplit.Value % 2)
                };

                regNumbers.Insert(rnIndex, (RegularNumber)newNumber.Left);
                regNumbers[rnIndex + 1] = (RegularNumber)newNumber.Right;

                if (numberToSplit.Parent != null)
                {
                    if (numberToSplit.IsLeftNumber)
                        ((PairNumber)numberToSplit.Parent).Left = newNumber;
                    else
                        ((PairNumber)numberToSplit.Parent).Right = newNumber;
                }

                return true;
            }

            return false;
        }


        private static List<(SnailfishNumber, List<RegularNumber>)> GetInput(string input) => GetInputAsStr(input).Select(ParseNumber).ToList();

        private static List<string> GetInputAsStr(string input) => input.GetLines(StringSplitOptions.RemoveEmptyEntries).ToList();
        

        private static (SnailfishNumber, List<RegularNumber>) ParseNumber(string numberStr)
        {
            PairNumber? currentNumber = null;
            List<RegularNumber> regularNumbers = new();

            foreach (char c in numberStr)
            {
                if (c == '[')
                {
                    PairNumber newPairNumber = new();

                    if (currentNumber != null)
                    {
                        if (currentNumber.Left == null)
                            currentNumber.Left = newPairNumber;
                        else
                            currentNumber.Right = newPairNumber;
                    }

                    currentNumber = newPairNumber;
                }
                else if (char.IsDigit(c))
                {
                    RegularNumber newRegularNumber = new((int)char.GetNumericValue(c));
                    regularNumbers.Add(newRegularNumber);

                    if (currentNumber != null)
                    {
                        if (currentNumber.Left == null)
                            currentNumber.Left = newRegularNumber;
                        else
                            currentNumber.Right = newRegularNumber;
                    }
                }
                else if (c == ']' && currentNumber?.Parent != null)
                {
                    currentNumber = (PairNumber)currentNumber.Parent;
                }
            }

            while (currentNumber?.Parent != null)
                currentNumber = (PairNumber)currentNumber.Parent;

            return (currentNumber ?? (SnailfishNumber)new RegularNumber(-1), regularNumbers);
        }


        private abstract class SnailfishNumber
        {
            public SnailfishNumber? Parent { get; set; }

            public bool IsLeftNumber { get; set; } = false;
            public bool IsRightNumber { get; set; } = false;

            public int GetDepth()
            {
                if (Parent != null)
                    return Parent.GetDepth() + 1;

                return 0;
            }

            public abstract int GetMagnitude();
        }

        private class RegularNumber : SnailfishNumber
        {
            public int Value { get; set; }

            public RegularNumber(int value)
            {
                Value = value;
            }

            public override string ToString()
            {
                return Value.ToString();
            }

            public override int GetMagnitude()
            {
                return Value;
            }
        }

        private class PairNumber : SnailfishNumber
        {
            private SnailfishNumber? left;
            public SnailfishNumber? Left
            {
                get { return left; }
                set
                {
                    left = value;

                    if (left != null)
                    {
                        left.Parent = this;
                        left.IsLeftNumber = true;
                    }
                }
            }

            private SnailfishNumber? right;
            public SnailfishNumber? Right
            {
                get { return right; }
                set
                {
                    right = value;

                    if (right != null)
                    {
                        right.Parent = this;
                        right.IsRightNumber = true;
                    }
                }
            }

            public override string ToString()
            {
                return $"[{Left?.ToString()},{Right?.ToString()}]";
            }

            public override int GetMagnitude()
            {
                return 3 * (Left?.GetMagnitude() ?? 0) + 2 * (Right?.GetMagnitude() ?? 0);
            }
        }
    }
}