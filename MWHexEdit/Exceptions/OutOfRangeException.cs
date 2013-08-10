using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MW.HexEdit
{
    public class OutOfRangeException : Exception
    {
        public OutOfRangeException(string s)
            : base(s)
        {
        }
    }
}
