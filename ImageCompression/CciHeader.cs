using System.Collections.Generic;

namespace ImageCompression
{
    public class CciHeader
    {
        public static int HeaderSize = sizeof (ushort)*2 + sizeof (byte);
        public ushort X { get; set; }
        public byte Y { get; set; }
        public ushort FileSize { get; set; }

        public List<byte> GetRawRepresentation()
        {
            var rawHeader = new List<byte>();

            rawHeader.Add((byte)(FileSize & 0xFF));
            rawHeader.Add((byte)((FileSize & 0xFF00) >> 8));
            rawHeader.Add((byte)(X & 0xFF));
            rawHeader.Add((byte)((X & 0x100) >> 8));
            rawHeader.Add((byte)(Y & 0xFF));
            return rawHeader;
        }
    }
}
