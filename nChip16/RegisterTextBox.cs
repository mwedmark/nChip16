using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nChip16
{
    class RegisterTextBox : KeyHandleTextBox
    {
        public ushort LastValue { get; set; }
    }
}
