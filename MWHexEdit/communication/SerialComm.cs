using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace MW.Communication
{
    public class SerialComm : IComm
    {
        private SerialPort serial;

        public SerialComm()
        {
            // create object
            serial = new SerialPort();
        }

        public static string[] GetIdentities()
        {
            return SerialPort.GetPortNames();
        }

        public void SetIdentity(string name)
        {
            serial.PortName = name;
        }

        #region IComm Members

        public void Init()
        {
            // setup parameters to match Plasma's serial port settings
            serial.BaudRate = 57600;
            serial.DataBits = 8;
            serial.StopBits = StopBits.None;
            serial.Parity = Parity.None;
            serial.DiscardNull = false;
            serial.DtrEnable = false;
            serial.Handshake = Handshake.None;
            serial.ReceivedBytesThreshold = 1;
            serial.RtsEnable = false;
        }

        public void Exit()
        {

        }

        public void Open()
        {
            serial.Open();
        }

        public void Close()
        {
            if(serial.IsOpen)
                serial.Close();
        }

        public byte[] ReadBytes(ulong startAddress, ulong size)
        {
            return new byte[size];
        }

        public void WriteBytes(byte[] data, ulong size)
        {

        }

        public bool IsConnected
        {
            get { return serial.IsOpen; }
        }

        #endregion
    }
}
