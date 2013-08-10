using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nChip16
{
    //- 0x00: Magic number 'CH16'
    //- 0x04: Reserved
    //- 0x05: Spec version (high nibble=major, low nibble=minor, so 0.7 = 0x07 and 1.0 = 0x10)
    //- 0x06: Rom size (excl. header, in bytes)
    //- 0x0A: Start address (initial value of PC)
    //- 0x0C: CRC32 checksum (excl. header, polynom = 0x04c11db7)
    //- 0x10: Start of Chip16 raw rom, end of header
    public class FileStructure
    {
        public string Path;

        public byte [] MagicNumber = new byte[4];
        public byte Reserved;
        public byte SpecVersion;
        public uint RomSize { get; set; }
        public ushort StartAddress { get; set; }
        public uint Checksum { get; set; }
        public byte[] Romdata { get; set; }

        public string SpecVersionString
        {
            get
            {
                var major = (SpecVersion & 0xF0)>>4;
                var minor = SpecVersion & 0x0F;
                return string.Format("{0}.{1}", major,minor);
            }
        }
    }
}
