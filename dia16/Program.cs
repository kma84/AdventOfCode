
const int SUM_OPERATOR_PACKET_ID = 0;
const int PRODUCT_OPERATOR_PACKET_ID = 1;
const int MIN_OPERATOR_PACKET_ID = 2;
const int MAX_OPERATOR_PACKET_ID = 3;
const int LITERAL_PACKET_ID = 4;
const int GREATER_THAN_OPERATOR_PACKET_ID = 5;
const int LESS_THAN_OPERATOR_PACKET_ID = 6;
const int EQUAL_OPERATOR_PACKET_ID = 7;

const int LENTGH_TYPE_TOTAL_LENGTH = 0;

bool debug = false;
string input = debug ? "inputTest7.txt" : "input.txt";

Part1();

Part2();


void Part1()
{
    string binaryStr = GetInput(input);

    if (debug)
        Console.WriteLine(binaryStr);

    (Packet packet, _) = GetPackets(binaryStr);

    Console.WriteLine("Part1: Sum of versions: " + packet.SumOfVersions());
}


void Part2()
{
    (Packet packet, _) = GetPackets(GetInput(input));

    Console.WriteLine("Part2: Result of the evaluation of the expression: " + packet.GetResult());
}



(Packet, string) GetPackets(string binaryStr)
{
    int version = Convert.ToInt32(binaryStr[..3], 2);
    int typeId = Convert.ToInt32(binaryStr[3..6], 2);

    (Packet packet, string rest) result;

    if (typeId == LITERAL_PACKET_ID)
    {
        result = GetLiteralPacket(binaryStr, version, typeId);
    }
    else 
    {
        // Operator packet
        int lengthTypeId = (int)char.GetNumericValue(binaryStr[6]);

        if (lengthTypeId == LENTGH_TYPE_TOTAL_LENGTH)
        {
            result = GetOperatorPacketByTotalLenth(binaryStr, version, typeId);
        }
        else
        {
            result = GetOperatorPacketByNumOfSubpackets(binaryStr, version, typeId);
        }
    }

    return result;
}


(Packet packet, string rest) GetOperatorPacketByNumOfSubpackets(string binaryStr, int version, int typeId)
{
    int numSubPackets = Convert.ToInt32(binaryStr[7..18], 2);
    string subPacketRest = binaryStr[18..];

    OperatorPacket operatorPacket = OperatorPacketFactory(version, typeId);

    for (int i = 0; i < numSubPackets; i++)
    {
        (Packet subPacket, subPacketRest) = GetPackets(subPacketRest);
        operatorPacket.SubPackets.Add(subPacket);
    }

    return (operatorPacket, subPacketRest);
}


(Packet packet, string rest) GetOperatorPacketByTotalLenth(string binaryStr, int version, int typeId)
{
    int subPacketsLenth = Convert.ToInt32(binaryStr[7..22], 2);
    string subPacketRest = binaryStr[22..];
    int remainingLenth = subPacketRest.Length;

    OperatorPacket operatorPacket = OperatorPacketFactory(version, typeId);

    do
    {
        (Packet subPacket, subPacketRest) = GetPackets(subPacketRest);
        operatorPacket.SubPackets.Add(subPacket);

        subPacketsLenth -= remainingLenth - subPacketRest.Length;
        remainingLenth = subPacketRest.Length;
    } while (subPacketsLenth > 0);

    return (operatorPacket, subPacketRest);
}


(Packet, string) GetLiteralPacket(string binaryStr, int version, int typeId)
{
    LiteralPacket packet = new(version, typeId, string.Empty);

    int i = 6;
    int groupPrefix;

    do
    {
        groupPrefix = (int)char.GetNumericValue(binaryStr[i]);

        packet.BinaryNumber += binaryStr.Substring(i + 1, 4);

        i += 5;
    } while (groupPrefix == 1);

    string rest = string.Empty;
    if (i < binaryStr.Length)
    {
        rest = binaryStr[i..];
    }

    return (packet, rest);
}


string GetInput(string filename)
{
    string input =
        File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "inputs" + Path.DirectorySeparatorChar + filename);

    string hexadecimalString = input.Split('\n', StringSplitOptions.RemoveEmptyEntries)[0];

    string binaryString = string.Join(
        string.Empty,
        hexadecimalString.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0'))
    );

    return binaryString;
}


OperatorPacket OperatorPacketFactory(int version, int typeId) => typeId switch 
{
    SUM_OPERATOR_PACKET_ID => new SumOperatorPacket(version, typeId),
    PRODUCT_OPERATOR_PACKET_ID => new ProductOperatorPacket(version, typeId),
    MIN_OPERATOR_PACKET_ID => new MinOperatorPacket(version, typeId),
    MAX_OPERATOR_PACKET_ID => new MaxOperatorPacket(version, typeId),
    GREATER_THAN_OPERATOR_PACKET_ID => new GreaterThanOperatorPacket(version, typeId),
    LESS_THAN_OPERATOR_PACKET_ID => new LessThanOperatorPacket(version, typeId),
    EQUAL_OPERATOR_PACKET_ID => new EqualOperatorPacket(version, typeId),
    _ => throw new ArgumentOutOfRangeException(nameof(typeId), $"Not expected typeId value: {typeId}")
};


abstract class Packet
{
    public int Version { get; set; }
    public int TypeId { get; set; }

    public Packet(int version, int typeId)
    {
        Version = version;
        TypeId = typeId;
    }

    public abstract int SumOfVersions();
    public abstract long GetResult();
}

class LiteralPacket : Packet
{
    public string BinaryNumber { get; set; }

    public override long GetResult() => Convert.ToInt64(BinaryNumber, 2);
    
    public LiteralPacket(int version, int typeId, string binaryNumber): base(version, typeId)
    {
        BinaryNumber = binaryNumber;
    }

    public override int SumOfVersions()
    {
        return Version;
    }
}

abstract class OperatorPacket : Packet
{
    public List<Packet> SubPackets { get; set; } = new List<Packet>();

    protected OperatorPacket(int version, int typeId) : base(version, typeId)
    {
    }

    public override int SumOfVersions()
    {
        return Version + SubPackets.Sum(sp => sp.SumOfVersions());
    }
}

class SumOperatorPacket : OperatorPacket
{
    public SumOperatorPacket(int version, int typeId) : base(version, typeId)
    {
    }

    public override long GetResult() => SubPackets.Sum(sp => sp.GetResult());    
}

class ProductOperatorPacket : OperatorPacket
{
    public ProductOperatorPacket(int version, int typeId) : base(version, typeId)
    {
    }

    public override long GetResult() => SubPackets.Aggregate(seed: 1L, func: (result, packet) => result * packet.GetResult());
}

class MinOperatorPacket : OperatorPacket
{
    public MinOperatorPacket(int version, int typeId) : base(version, typeId)
    {
    }

    public override long GetResult() => SubPackets.Min(sp => sp.GetResult());
}

class MaxOperatorPacket : OperatorPacket
{
    public MaxOperatorPacket(int version, int typeId) : base(version, typeId)
    {
    }

    public override long GetResult() => SubPackets.Max(sp => sp.GetResult());
}

class GreaterThanOperatorPacket : OperatorPacket
{
    public GreaterThanOperatorPacket(int version, int typeId) : base(version, typeId)
    {
    }

    public override long GetResult() => SubPackets[0].GetResult() > SubPackets[1].GetResult() ? 1 : 0;
}

class LessThanOperatorPacket : OperatorPacket
{
    public LessThanOperatorPacket(int version, int typeId) : base(version, typeId)
    {
    }

    public override long GetResult() => SubPackets[0].GetResult() < SubPackets[1].GetResult() ? 1 : 0;
}

class EqualOperatorPacket : OperatorPacket
{
    public EqualOperatorPacket(int version, int typeId) : base(version, typeId)
    {
    }

    public override long GetResult() => SubPackets[0].GetResult() == SubPackets[1].GetResult() ? 1 : 0;
}