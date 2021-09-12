using System;
using System.IO;

namespace Chip16.Shared
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


        ///ROMs are stored either in a Chip16 ROM file (.c16), or a raw binary file (preferred .bin, also .c16).
        ///The ROM file stores a 16-byte header, and the binary data after that.
        ///Emulators should load ROM headers for internal use, and only use the binary data for emulation.
        ///Binary data is always loaded at address 0x0000.

        ///The header is as follows (0x00 - 0x0F) :
        ///- 0x00: Magic number 'CH16'
        ///- 0x04: Reserved
        ///- 0x05: Spec version (high nibble=major, low nibble=minor, so 0.7 = 0x07 and 1.0 = 0x10)
        ///- 0x06: Rom size (excl. header, in bytes)
        ///- 0x0A: Start address (initial value of PC)
        ///- 0x0C: CRC32 checksum (excl. header, polynom = 0x04c11db7)
        ///- 0x10: Start of Chip16 raw rom, end of header
        public static FileStructure InterpretFile(string filePath)
        {
            var fs = new FileStructure();
            //var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            fs.Path = filePath;

            try
            {
                using(var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                { 
                    stream.Read(fs.MagicNumber, 0, 4);
                    fs.Reserved = (byte)stream.ReadByte();
                    fs.SpecVersion = (byte)stream.ReadByte();
                    //if (!DoesEmulatorHandleSpecVersion(fs.SpecVersion))
                    //    throw new Exception(
                    //        string.Format("nChip16 handles {0} but ROM uses {1}.. Halting load!", SpecVersionAsString(SpecVersion), SpecVersionAsString(fs.SpecVersion)));

                    const int soRomSize = 4;
                    var romSize = new byte[soRomSize];
                    stream.Read(romSize, 0, soRomSize);
                    fs.RomSize = ConvertBytesToDWord(romSize);

                    const int soStartAddress = 2;
                    var startAddress = new byte[soStartAddress];
                    stream.Read(startAddress, 0, soStartAddress);
                    fs.StartAddress = ConvertBytesToWord(startAddress);

                    const int soChecksum = 4;
                    var checksum = new byte[soChecksum];
                    stream.Read(checksum, 0, soChecksum);
                    fs.Checksum = ConvertBytesToDWord(checksum);

                    fs.Romdata = new byte[fs.RomSize];
                    stream.Read(fs.Romdata, 0, (int)fs.RomSize);

                    var dummyRead = stream.Read(new byte[10], 0, 10);
                    if (dummyRead != 0)
                        throw new Exception("Error in fileformat!");
                }
            }
            finally
            {
            //    stream.Close();
            }

            return fs;
        }

        private string SpecVersionAsString(byte specByte)
        {
            return (((specByte & 0xF0) >> 4)).ToString() + "." + ((int)(specByte & 0x0F)).ToString();
        }

        //private bool DoesEmulatorHandleSpecVersion(byte specVersion)
        //{
        //    return (specVersion <= SpecVersion);
        //}

        private static uint ConvertBytesToDWord(byte[] checksum)
        {
            return (uint)(checksum[0] + (checksum[1] << 8) + (checksum[2] << 16) + (checksum[3] << 24));
        }

        private static ushort ConvertBytesToWord(byte[] romSize)
        {
            return (ushort)((ushort)(romSize[1] << 8) + romSize[0]);
        }

    }
}
