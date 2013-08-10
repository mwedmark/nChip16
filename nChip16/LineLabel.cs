using System;
using System.Globalization;

namespace nChip16
{
    public class LineLabel
    {
        public ushort Address { get; set; }
        public string Name { get; set; }

        //0x0000 : main
        public void InterpretLabel(string text)
        {
            var addressAndName = text.Split(':');   
            if(addressAndName.Length != 2)
                throw new Exception("Bad format on label lines!");

            // trim leading and trailing whitespaces.
            addressAndName[0] = addressAndName[0].Trim();
            addressAndName[1] = addressAndName[1].Trim();

            Address = ushort.Parse(addressAndName[0].Remove(0,2), NumberStyles.HexNumber);
            Name = addressAndName[1];
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Address.ToString("X4"), Name);
        }
    }
}