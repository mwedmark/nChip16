using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MW.Communication
{
    public class NotConnectedException : Exception
    {
        public NotConnectedException(string message) : base(message) { }
    }
}
