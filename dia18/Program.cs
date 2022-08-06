
bool debug = false;
string input = debug ? "inputTest2.txt" : "input.txt";


Part1();

Part2();


void Part1()
{
    List<SnailfishNumber> numbers = GetInput(input);

    if (debug)
    {
        foreach (SnailfishNumber number in numbers)
            Console.WriteLine(number);

        Console.WriteLine();
    }

    SnailfishNumber result = numbers.Aggregate((result, number) => Add(result, number));

    Console.WriteLine("Sum:");
    Console.WriteLine(result);
    Console.WriteLine("Part1. Magnitude: " + result.GetMagnitude());
}

void Part2()
{
    List<string> numbersStr = GetInputAsStr(input);

    int maxMagnitude = 0;

    for (int i = 0; i < numbersStr.Count; i++)
    {
        for (int j = numbersStr.Count - 1; j >= 0; j--)
        {
            if (i == j)
                continue;

            SnailfishNumber result = Add(ParseNumber(numbersStr[i]), ParseNumber(numbersStr[j]));
            maxMagnitude = Math.Max(maxMagnitude, result.GetMagnitude());
        }
    }

    Console.WriteLine("Part2. Max. magnitude: " + maxMagnitude);
}


PairNumber Add(SnailfishNumber left, SnailfishNumber right)
{
    PairNumber result = new () { Left = left, Right = right };

    while (Explode(result) || Split(result)){}

    return result;
}


bool Split(PairNumber number)
{
    RegularNumber? numberToSplit = (RegularNumber?)FindNumberToSplit(number);

    if (numberToSplit != null && numberToSplit.Parent != null)
    {
        PairNumber newNumber = new()
        {
            Left = new RegularNumber(numberToSplit.Value / 2),
            Right = new RegularNumber(numberToSplit.Value / 2 + numberToSplit.Value % 2)
        };

        if (numberToSplit.IsLeftNumber)
            ((PairNumber)numberToSplit.Parent).Left = newNumber;
        else
            ((PairNumber)numberToSplit.Parent).Right = newNumber;

        return true;
    }

    return false;
}


bool Explode(PairNumber number)
{
    PairNumber? numberToExplode = (PairNumber?)FindNumberToExplode(number);

    if (numberToExplode != null && numberToExplode.Parent != null)
    {
        // Left
        SnailfishNumber? currentNumber = numberToExplode;

        while (currentNumber != null)
        {
            if (currentNumber.IsLeftNumber)
            {
                currentNumber = currentNumber.Parent;
            }
            else
            {
                currentNumber = ((PairNumber?)currentNumber.Parent)?.Left;

                while (currentNumber is PairNumber currentPairNumber)
                {
                    currentNumber = currentPairNumber.Right;
                }

                if (currentNumber is RegularNumber)
                {
                    ((RegularNumber)currentNumber).Value += ((RegularNumber?)numberToExplode.Left)?.Value ?? 0;
                }

                currentNumber = null;
            }
        }

        // Right
        currentNumber = numberToExplode;

        while (currentNumber != null)
        {
            if (currentNumber.IsRightNumber)
            {
                currentNumber = currentNumber.Parent;
            }
            else
            {
                currentNumber = ((PairNumber?)currentNumber.Parent)?.Right;

                while (currentNumber is PairNumber currentPairNumber)
                {
                    currentNumber = currentPairNumber.Left;
                }

                if (currentNumber is RegularNumber)
                {
                    ((RegularNumber)currentNumber).Value += ((RegularNumber?)numberToExplode.Right)?.Value ?? 0;
                }

                currentNumber = null;
            }
        }


        RegularNumber newNumber = new(0);

        if (numberToExplode.IsLeftNumber)
            ((PairNumber)numberToExplode.Parent).Left = newNumber;
        else
            ((PairNumber)numberToExplode.Parent).Right = newNumber;

        return true;
    }

    return false;
}


SnailfishNumber? FindNumberToSplit(SnailfishNumber? number)
{
    if (number == null)
        return null;

    if (number is RegularNumber regularNumber)
        return regularNumber.Value >= 10 ? regularNumber : null;
        
    return FindNumberToSplit(((PairNumber)number).Left) ?? FindNumberToSplit(((PairNumber)number).Right);
}


SnailfishNumber? FindNumberToExplode(SnailfishNumber? number)
{
    if (number == null || number is RegularNumber)
        return null;

    if (number.GetDepth() >= 4)
        return number;
    
    return FindNumberToExplode(((PairNumber)number).Left) ?? FindNumberToExplode(((PairNumber)number).Right);
}


List<SnailfishNumber> GetInput(string filename)
{
    return GetInputAsStr(filename).Select(r => ParseNumber(r)).ToList();
}


List<string> GetInputAsStr(string filename)
{
    string path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "inputs" + Path.DirectorySeparatorChar + filename;
    string input = File.ReadAllText(path);

    string[] rows = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

    return rows.ToList();
}


SnailfishNumber ParseNumber(string numberStr)
{
    PairNumber? currentNumber = null;

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

    return currentNumber ?? (SnailfishNumber)new RegularNumber(-1);
}


abstract class SnailfishNumber
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

class RegularNumber : SnailfishNumber
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

class PairNumber : SnailfishNumber
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