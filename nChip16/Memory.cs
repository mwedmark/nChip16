using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nChip16
{
    public delegate void OnWriteByte(int address, byte value);
    public delegate void OnWriteWord(int address, ushort value);

    public class Memory
    {

        public event OnWriteByte OnWriteByte;
        public event OnWriteWord OnWriteWord;

        private readonly byte[] mem;
        public static int MemSize = 65536;
 
        public Memory()
        {
            mem = new byte[MemSize];
        }

        public byte[] GetInternalMemoryArray()
        {
            return mem;
        }

        public void WriteByte(int address, byte value)
        {
            mem[address] = value;

            if (OnWriteByte != null)
                OnWriteByte(address, value);
        }

        public byte ReadByte(int address)
        {
            return mem[address];
        }

        public void WriteWord(int address, ushort value)
        {
            mem[address] = (byte)(value & 0xFF);
            mem[address + 1] = (byte) (value >> 8);

            if (OnWriteWord != null)
                OnWriteWord(address, value);
        }

        public ushort ReadWord(int address)
        {
            return (ushort)(mem[address] + (mem[address+1]<<8)) ;
        }

        public void Reset()
        {
            for (int m = 0; m < MemSize; m++)
                mem[m] = 0;
        }
    }
}
