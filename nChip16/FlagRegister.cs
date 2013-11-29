using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nChip16
{
    //Bit[0] - Reserved
    //Bit[1] - c (Unsigned Carry and Unsigned Borrow flag)
    //Bit[2] - z (Zero flag)
    //Bit[3] - Reserved
    //Bit[4] - Reserved
    //Bit[5] - Reserved
    //Bit[6] - o (Signed Overflow Flag)
    //Bit[7] - n (Negative Flag; aka Sign Flag)
    public class FlagRegister
    {
        public static class BitFlags
        {
            public const ushort C = 2;
            public const ushort Z = 4;
            public const ushort O = 64;
            public const ushort N = 128;
            public const ushort ALL = C | Z | N | O;
        };
        public bool C { get; set; }
        public bool Z { get; set; }
        public bool O { get; set; }
        public bool N { get; set; }

        internal void Reset()
        {
            C = false;
            Z = false;
            O = false;
            N = false;
        }

        public ushort BitValue
        {
            get { return (ushort)( (C?2:0) + (Z?4:0) + (O?64:0) + (N?128:0)); } 

            set
            {
                C = (value & 2) == 2;
                Z = (value & 4) == 4;
                O = (value & 64) == 64;
                N = (value & 128) == 128;
            }
        }

        public void UpdateFlags(int value)
        {
            UpdateFlags(value, BitFlags.ALL);
        }

        public void UpdateFlags(int value, ushort setFlagsPattern)
        {
            if( (setFlagsPattern & BitFlags.Z) != 0)
                Z = ((ushort)value == 0);

            if( (setFlagsPattern & BitFlags.N) != 0)
                N = (value & 0x8000) != 0;

            if( (setFlagsPattern & BitFlags.O) != 0)
                O = (value > 0xFFFF);

            if( (setFlagsPattern & BitFlags.C) != 0)
                C = (value & 0x10000) != 0;
        }

        public void UpdateFlagsModRem(int value)
        {
            UpdateFlags(value, BitFlags.Z | BitFlags.N);
        }

        public void UpdateFlagsMul(int value)
        {
            UpdateFlags(value, BitFlags.Z | BitFlags.N | BitFlags.C);
        }

        public void UpdateFlagsLogic(int value)
        {
            //Z = ((ushort)value == 0);
            //N = (value & 0x8000) != 0;
            UpdateFlags(value, BitFlags.Z | BitFlags.N);
        }

        internal void UpdateFlagsSub(ushort x, ushort y, int value)
        {
            UpdateFlags(value);
            // O => for subtraction (x-y=z) it is set when z is positive and x is negative and y is positive,
            // or if z is negative and x is positive and y is negative; else it is cleared.
            //O = (((value & 0x8000) == 0) && ((x & 0x8000) != 0) && ((y & 0x8000) == 0)) || 
            //    (((value & 0x8000) != 0) && ((x & 0x8000) == 0) && ((y & 0x8000) != 0));
            O = (IsPositive(value) && IsNegative(x) && IsPositive(y) ||
                 IsNegative(value) && IsPositive(x) && IsNegative(y));
        }

        internal void UpdateFlagsAdd(ushort x, ushort y, int value)
        {
            UpdateFlags(value);
            // O => for addition it is set when the result is positive and both operands were negative,
            // or if the result is negative and both operands were positive; else it is cleared.
            O = (IsPositive(value) && IsNegative(x) && IsNegative(y) ||
                IsNegative(value) && IsPositive(x) && IsPositive(y));
        }

        internal bool IsPositive(int value)
        {
            return (value & 0x8000) == 0;
        }

        internal bool IsNegative(int value)
        {
            return !IsPositive(value);
        }

        internal void UpdateFlagsDiv(int temp, int rem)
        {
            UpdateFlagsMul(temp);
            // C=> - for div - is set when the remainder of the division is non-zero, else it is cleared.
            C = (rem != 0);
        }

        internal void UpdateFlagsSar(int temp, int beforeShift)
        {
            UpdateFlagsLogic(temp);
            // N=> - for sar - leading bit is copied to N.
            N = (beforeShift & 0x8000) != 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("C={0} N={1} O={2} Z={3}", 
                FlagFormat(C), FlagFormat(N), FlagFormat(O), FlagFormat(Z));

            return sb.ToString();
        }

        private string FlagFormat(bool flag)
        {
            return flag ? "1" : "0";
        }
    }
}
