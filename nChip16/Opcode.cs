using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nChip16
{
    public class Opcode
    {
        private string HexPrefix = "$";
        public Opcode()
        {
        }

        public byte[] RawOpcode = new byte[4];

        public byte HH { get; set; }
        public byte LL { get; set; }

        public ushort HHLL{ get { return (ushort) ((HH << 8) + LL); }}

        public byte N { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        public byte Z { get; set; }
        public BB BranchBits { get; set; }
        public string Label { get; set; }


        /// <summary>
        /// Non-unique opcode name for specified machine code instruction
        /// </summary>
        public string Name
        {
            get
            {
                var opcodeName = LookupOpcodeName(RawOpcode[0], RawOpcode[1]);
                if (opcodeName != "")
                    return opcodeName;

                // no opcode found, build complete hex number representation
                return string.Format("{0}{1} {2}{3}",
                    RawOpcode[1].ToString("X2"), RawOpcode[0].ToString("X2"), 
                    RawOpcode[3].ToString("X2"), RawOpcode[2].ToString("X2"));
            }
        }

        private enum GroupMembers { LLHH, YX, OX, YXZ, ON, BB}
        public enum BB {Z = 0,NZ,N,NN,P,O,NO,A,AE,B,BE,G,GE,L,LE,RES}
        
        ///Opcodes:
        ///---------
        /// HH - high byte.
        /// LL - low byte.
        /// N - nibble (4 bit value).
        /// X, Y, Z - 4 bit register identifier.
        /// Extract all above values with out executing 
        public void InterpretCode(byte[] machineCode)
        {
            // save original machine code values
            machineCode.CopyTo(RawOpcode,0);

            //if (Is_GroupMember(machineCode[0], GroupMembers.LLHH))
            {
                LL = machineCode[2];
                HH = machineCode[3];
            }

            //if (Is_GroupMember(machineCode[0], GroupMembers.YX))
            {
                //Y = (byte)(machineCode[1]>>4);
                //X = (byte)(machineCode[1] & 0x0F);
            }

            //if (Is_GroupMember(machineCode[0], GroupMembers.OX))
            {
                //X = (byte)(machineCode[1] & 0x0F);
            }

            //if (Is_GroupMember(machineCode[0], GroupMembers.YXZ))
            {
                Y = (byte)(machineCode[1] >> 4);
                X = (byte)(machineCode[1] & 0x0F);
                Z = (byte) (machineCode[2] & 0x0F);
            }

            //if (Is_GroupMember(machineCode[0], GroupMembers.BB))
            {
                BranchBits = (BB)(machineCode[1] & 0x0F);
            }

            //if (Is_GroupMember(machineCode[0], GroupMembers.ON))
            {
                N = (byte) (machineCode[2] & 0x0F);
            }
        }

        /// <summary>
        /// byte array holding the group membership of all opcodes
        /// The ones marked with 0xFF is NOT used, but only entered as filing
        /// 6 groups => 6 bits => use 6 LSB of byte
        /// </summary>
        #region OpcodeGroupMembership
        private byte[] OpcodeGroupMembership = new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            };
        #endregion

        // TODO: rework this section so that it uses bits for each group membership/machinecode => FASTER!
        private static readonly List<byte> LLHH_Group = new List<byte>
            {0x04,0x05,0x07,0x0A,0x0B,0x0C,0x0D,0x10,0x11,0x12,0x13,0x14,0x17,0x20,0x21,0x22,0x30,
            0x40,0x50,0x53,0x60,0x63,0x70,0x80,0x90,0xA0,0xD0,0xA3,0xA6,0xE0,0xE3 };
        private static readonly List<byte> YX_Group = new List<byte>
            {0x05,0x13,0x23,0x24,0x31,0x41,0x42,0x51,0x52,0x54,0x61,0x62,0x64,0x71,0x72,
            0x81,0x82,0x91,0x92,0xA1,0xA2,0xB3,0xB4,0xB5,0xA4,0xA7,0xE2,0xE5};
        private static readonly List<byte> OX_Group = new List<byte>
            {0x07,0x0D,0x16,0x18,0x20,0x22,0x30,0x40,0x50,0x53,0x60,0x63,0x70,0x80,0x90,0xA0,
            0xB0,0xB1,0xB2,0xC0,0xC1,0xE1,0xE4,0xA3,0xA6,0xE0,0xE3};
        private static readonly List<byte> YXZ_Group = new List<byte>
            {0x06,0x42,0x52,0x62,0x72,0x82,0x92,0xA2,0xA8,0xA5};
        private static readonly List<byte> ON_Group = new List<byte>
            {0x03,0xB0,0xB1,0xB2};
        private static readonly List<byte> BB_Group = new List<byte>
            {0x12, 0x17};

        private static bool Is_GroupMember(byte machineCode, GroupMembers group)
        {
            switch (group)
            {
                case GroupMembers.LLHH:
                    return LLHH_Group.Exists(mc => mc == machineCode);
                case  GroupMembers.YX:
                    return YX_Group.Exists(mc => mc == machineCode);
                case GroupMembers.OX:
                    return OX_Group.Exists(mc => mc == machineCode);
                case GroupMembers.YXZ:
                    return YXZ_Group.Exists(mc => mc == machineCode);
                case GroupMembers.ON:
                    return ON_Group.Exists(mc => mc == machineCode);
                case GroupMembers.BB:
                    return BB_Group.Exists(mc => mc == machineCode);
                default: 
                    throw new Exception("Invalid Group of opcode family");
            }
        }

        private static string LookupOpcodeName(byte machineCode, byte branchSelect)
        {
            if (!OpcodeNameTable.ContainsKey(machineCode))
                return "";

            var opcodeName = OpcodeNameTable[machineCode];
            string branchName;

            if (machineCode != 0x12 && machineCode != 0x17)
                return opcodeName;

            switch (branchSelect)
            {
                case 0x0: branchName = "Z"; break;
                case 0x1: branchName = "NZ"; break;
                case 0x2: branchName = "N"; break;
                case 0x3: branchName = "NN"; break;
                case 0x4: branchName = "P"; break;
                case 0x5: branchName = "O"; break;
                case 0x6: branchName = "NO"; break;
                case 0x7: branchName = "A"; break;
                case 0x8: branchName = "AE"; break;
                case 0x9: branchName = "B"; break;
                case 0xA: branchName = "BE"; break;
                case 0xB: branchName = "G"; break;
                case 0xC: branchName = "GE"; break;
                case 0xD: branchName = "L"; break;
                case 0xE: branchName = "LE"; break;
                //case 0xF: branchName = "RESERVED"; break;
                default: //throw new Exception(
                    //string.Format("LookupOpcodename failed, unknown machinecode: 0x{0}, 0x{1} ",
                    //machineCode.ToString("X2"), branchSelect.ToString("X2")));
                    return string.Format("{0}{1}", machineCode.ToString("X2"), branchSelect.ToString("X2"));
            }

            return opcodeName + branchName;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var opcodeName = Name;
            while (opcodeName.Length < 4)
                opcodeName += " ";

            //sb.AppendFormat("{0} X{1} Y{2} Z{3} N{4} ",
            //    opcodeName, X.ToString("X1"), Y.ToString("X1"), Z.ToString("X1"), N.ToString("X1"));
            //sb.AppendFormat("HHLL:{0} [{1}]", HHLL.ToString("X4"), ArrayToString(RawOpcode));

            string addressHHLL = HexPrefix + HHLL.ToString("X");
            if (Is_GroupMember(RawOpcode[0], GroupMembers.LLHH) && !string.IsNullOrEmpty(Label))
                addressHHLL = Label;

            sb.AppendFormat("{0} ", opcodeName);

            if (Is_GroupMember(RawOpcode[0], GroupMembers.YX) &&
                !Is_GroupMember(RawOpcode[0], GroupMembers.LLHH))
            {
                sb.AppendFormat("R{0}, R{1}", X.ToString("X"),Y.ToString("X") );
            }
            if (Is_GroupMember(RawOpcode[0], GroupMembers.YX) &&
               Is_GroupMember(RawOpcode[0], GroupMembers.LLHH))
            {
                sb.AppendFormat("R{0}, R{1}, {2}", X.ToString("X"), Y.ToString("X"), addressHHLL);
            }
            if (Is_GroupMember(RawOpcode[0], GroupMembers.LLHH) &&
                Is_GroupMember(RawOpcode[0], GroupMembers.OX))
            {
                sb.AppendFormat("R{0}, {1}", X.ToString("X"), addressHHLL);
            }
            if (Is_GroupMember(RawOpcode[0], GroupMembers.LLHH) &&
                !Is_GroupMember(RawOpcode[0], GroupMembers.OX) &&
                !Is_GroupMember(RawOpcode[0], GroupMembers.YX))
            {
                sb.AppendFormat("{0}", addressHHLL);
            }
            if (Is_GroupMember(RawOpcode[0], GroupMembers.YXZ))
            {
                sb.AppendFormat("R{0}, R{1}, R{2}", X.ToString("X"), Y.ToString("X"), Z.ToString("X"));
            }
            if (Is_GroupMember(RawOpcode[0], GroupMembers.ON))
            {
                if (Is_GroupMember(RawOpcode[0], GroupMembers.OX))
                    sb.AppendFormat("R{0}, {2}{1}", X.ToString("X"), N.ToString("X"), HexPrefix);
                else
                    sb.AppendFormat("{1}{0}", N.ToString("X"), HexPrefix);
            }
            else
            {
                if(Is_GroupMember(RawOpcode[0], GroupMembers.OX) &&
                    (RawOpcode[0] == 0xc0 || RawOpcode[0] == 0xc1) || RawOpcode[0] == 0xd1 ||
                    RawOpcode[0] == 0xe1 || RawOpcode[0] == 0xe4)
                    sb.AppendFormat("R{0}", X.ToString("X"));
            }
            return sb.ToString();
        }

        public string ToStringShort()
        {
            var sb = new StringBuilder();

            var opcodeName = Name;
            while (opcodeName.Length < 4)
                opcodeName += " "; 

            sb.AppendFormat("{0} X{1} Y{2} Z{3} N{4} ",
                opcodeName, X.ToString("X1"), Y.ToString("X1"), Z.ToString("X1"), N.ToString("X1"));
            sb.AppendFormat("HHLL:{0}", HHLL.ToString("X4"));

            return sb.ToString();
        }

        private static string ArrayToString(byte[] data)
        {
            var sb = new StringBuilder();

            foreach (var dataByte in data)
                sb.AppendFormat("{0},", dataByte.ToString("X2"));

            return sb.ToString();
        }

        public bool IsCallInstruction()
        {
            return (RawOpcode[0] == 0x14 || RawOpcode[0] == 0x17 || RawOpcode[0] == 0x18);
        }

        #region OpcodeTable
        private static readonly Dictionary<byte, string> OpcodeNameTable = new Dictionary<byte, string>
            {
            {0x00, "NOP"},{0x01, "CLS"},{0x02, "VBL"},{0x03, "BGC"},{0x04, "SPR"},{0x05, "DRW"},{0x06, "DRW"},{0x07, "RND"},

            {0x08, "FLIP"},

            {0x09, "SND0"},{0x0A, "SND1"},{0x0B, "SND2"},{0x0C, "SND3"},{0x0D, "SNP"},

            {0x0E, "SNG"},

            // JUMPS
            {0x10, "JMP"},{0x11, "JC" /*OBSOLETE*/},{0x12, "J"},{0x13, "JME"},{0x16, "JMP"},

            // CALLS
            {0x14, "CALL"},{0x15, "RET"},{0x17, "C"},{0x18, "CALL"},

            // LOADS
            {0x20, "LDI"},{0x21, "LDI"},{0x22, "LDM"},{0x23, "LDM"},{0x24, "MOV"},

            // STORES
            {0x30, "STM"},{0x31, "STM"},

            // ARITHMETIC
            {0x40, "ADDI"},{0x41, "ADD"},{0x42, "ADD"},
            {0x50, "SUBI"},{0x51, "SUB"},{0x52, "SUB"},
            {0x53, "CMPI"},{0x54, "CMP"},
            {0x60, "ANDI"},{0x61, "AND"},{0x62, "AND"},
            {0x63, "TSTI"},{0x64, "TST"},
            {0x70, "ORI"},{0x71, "OR"},{0x72, "OR"},
            {0x80, "XORI"},{0x81, "XOR"},{0x82, "XOR"},
            {0x90, "MULI"},{0x91, "MUL"},{0x92, "MUL"},
            {0xA0, "DIVI"},{0xA1, "DIV"},{0xA2, "DIV"},
            {0xA3, "MODI"},{0xA4, "MOD"},{0xA5, "MOD"},
            {0xA6, "REMI"},{0xA7, "REM"},{0xA8, "REM"},
            {0xB0, "SHL"},{0xB1, "SHR"},{0xB2, "SAR"},{0xB3, "SHL"},{0xB4, "SHR"},{0xB5, "SAR"},

            // PUSH/POP
            {0xC0, "PUSH"},{0xC1, "POP"},{0xC2, "PSHA"},{0xC3, "POPA"},{0xC4, "PSHF"},{0xC5, "POPF"},

            // Palette
            {0xD0, "PAL"},{0xD1, "PAL"},
            // NOT
            {0xE0, "NOTI"},{0xE1, "NOT"},{0xE2, "NOT"},
            // NEG
            {0xE3, "NEGI"},{0xE4, "NEG"},{0xE5, "NEG"}
        };
        #endregion

    }
}
