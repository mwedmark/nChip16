using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MW.Communication
{
    public class EthernetComm : IComm  
    {
        #region IComm Members

        public void Init()
        {
            throw new NotImplementedException();
        }

        public void Exit()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public byte[] ReadBytes(ulong startAddress, ulong size)
        {
            throw new NotImplementedException();
        }

        public void WriteBytes(byte[] data, ulong size)
        {
            throw new NotImplementedException();
        }

        public bool IsConnected
        {
            get { return false; }
        }

        #endregion
    }
}
