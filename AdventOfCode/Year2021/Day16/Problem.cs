using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils;

namespace AdventOfCode.Year2021.Day16
{
    [Problem(Year = 2021, Day = 16, ProblemName = "Packet Decoder")]
    internal class Problem : IProblem
    {
        private const int SUM_OPERATOR_PACKET_ID = 0;
        private const int PRODUCT_OPERATOR_PACKET_ID = 1;
        private const int MIN_OPERATOR_PACKET_ID = 2;
        private const int MAX_OPERATOR_PACKET_ID = 3;
        private const int LITERAL_PACKET_ID = 4;
        private const int GREATER_THAN_OPERATOR_PACKET_ID = 5;
        private const int LESS_THAN_OPERATOR_PACKET_ID = 6;
        private const int EQUAL_OPERATOR_PACKET_ID = 7;
        
        private const int LENTGH_TYPE_TOTAL_LENGTH = 0;

        public bool Debug { get; set; } = false;


        public string Part1(string input)
        {
            string binaryStr = GetInput(input);

            if (Debug)
                Console.WriteLine(binaryStr);

            (Packet packet, _) = GetPackets(binaryStr);

            return packet.SumOfVersions().ToString();
        }

        public string Part2(string input)
        {
            (Packet packet, _) = GetPackets(GetInput(input));

            return packet.GetResult().ToString();
        }



        private (Packet, string) GetPackets(string binaryStr)
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


        private (Packet packet, string rest) GetOperatorPacketByNumOfSubpackets(string binaryStr, int version, int typeId)
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


        private (Packet packet, string rest) GetOperatorPacketByTotalLenth(string binaryStr, int version, int typeId)
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

        private static (Packet, string) GetLiteralPacket(string binaryStr, int version, int typeId)
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

        private static string GetInput(string input)
        {
            string hexadecimalString = input.GetLines(StringSplitOptions.RemoveEmptyEntries)[0];

            string binaryString = string.Join(
                string.Empty,
                hexadecimalString.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0'))
            );

            return binaryString;
        }

        private static OperatorPacket OperatorPacketFactory(int version, int typeId) => typeId switch
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


        private abstract class Packet
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

        private class LiteralPacket : Packet
        {
            public string BinaryNumber { get; set; }

            public override long GetResult() => Convert.ToInt64(BinaryNumber, 2);

            public LiteralPacket(int version, int typeId, string binaryNumber) : base(version, typeId)
            {
                BinaryNumber = binaryNumber;
            }

            public override int SumOfVersions()
            {
                return Version;
            }
        }

        private abstract class OperatorPacket : Packet
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

        private class SumOperatorPacket : OperatorPacket
        {
            public SumOperatorPacket(int version, int typeId) : base(version, typeId)
            {
            }

            public override long GetResult() => SubPackets.Sum(sp => sp.GetResult());
        }

        private class ProductOperatorPacket : OperatorPacket
        {
            public ProductOperatorPacket(int version, int typeId) : base(version, typeId)
            {
            }

            public override long GetResult() => SubPackets.Aggregate(seed: 1L, func: (result, packet) => result * packet.GetResult());
        }

        private class MinOperatorPacket : OperatorPacket
        {
            public MinOperatorPacket(int version, int typeId) : base(version, typeId)
            {
            }

            public override long GetResult() => SubPackets.Min(sp => sp.GetResult());
        }

        private class MaxOperatorPacket : OperatorPacket
        {
            public MaxOperatorPacket(int version, int typeId) : base(version, typeId)
            {
            }

            public override long GetResult() => SubPackets.Max(sp => sp.GetResult());
        }

        private class GreaterThanOperatorPacket : OperatorPacket
        {
            public GreaterThanOperatorPacket(int version, int typeId) : base(version, typeId)
            {
            }

            public override long GetResult() => SubPackets[0].GetResult() > SubPackets[1].GetResult() ? 1 : 0;
        }

        private class LessThanOperatorPacket : OperatorPacket
        {
            public LessThanOperatorPacket(int version, int typeId) : base(version, typeId)
            {
            }

            public override long GetResult() => SubPackets[0].GetResult() < SubPackets[1].GetResult() ? 1 : 0;
        }

        private class EqualOperatorPacket : OperatorPacket
        {
            public EqualOperatorPacket(int version, int typeId) : base(version, typeId)
            {
            }

            public override long GetResult() => SubPackets[0].GetResult() == SubPackets[1].GetResult() ? 1 : 0;
        }
    }
}