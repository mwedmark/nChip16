using System;

namespace MW.Communication
{
    public interface IComm
    {
        void Init();
        void Exit();
        void Close();
        void Open();
        byte[] ReadBytes(ulong startAddress, ulong size);
        void WriteBytes(byte[] data, ulong size);
        bool IsConnected { get; }
    }
}
