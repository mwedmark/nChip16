using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MW.HexEdit
{
    public class DataChangedEventArgs : EventArgs
    {
        private int index;
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        private uint value;
        public uint Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public DataChangedEventArgs(int index, uint value)
        {
            this.index = index;
            this.value = value;
        }
    }
}
