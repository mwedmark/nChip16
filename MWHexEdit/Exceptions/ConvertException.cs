using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MWHexEdit
{
    class ConvertException : Exception
    {
        public ConvertException(string message) : base(message) { }
    }
}
