using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nChip16
{
    public static class Utils
    {
        public static string UShortToHex16BitFormat(ushort address)
        {
            return address.ToString("X4");
        }

        public static string ConvertBoolToNumber(bool value)
        {
            return value ? "1" : "0";
        }

        public static string FormatBinary(ushort value)
        {
            var binaryString = Convert.ToString(value, 2);
            binaryString = binaryString.PadLeft(16, '0');
            return binaryString;
        }

        public static string FormatBinary(byte value)
        {
            var binaryString = Convert.ToString(value, 2);
            binaryString = binaryString.PadLeft(8, '0');
            return binaryString;
        }
    }
}
