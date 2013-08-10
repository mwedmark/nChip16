using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MW.Communication
{
    public class AlreadyConnectedException : Exception
    {
        public AlreadyConnectedException(string message) : base(message) { }
    }
}
