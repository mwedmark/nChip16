using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MWHexEdit
{
    public class MemoryRange
    {
        public int StartAddress { get; set; }
        public int Length { get; set; }
        public int EndAddress { get { return StartAddress + Length-1; } }
    }
}
