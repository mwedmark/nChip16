using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nChip16
{
    public class MMapImport
    {
        public static List<string> RawLabels = new List<string>();
        public static List<LineLabel> Labels = new List<LineLabel>();
        public MMapImport(){}

        public static List<LineLabel> ImportFile(string path)
        {
            var rows = File.ReadAllLines(path);
            rows = rows.Where(row => !string.IsNullOrEmpty(row)).ToArray();

            if(rows.First() != "Label memory mapping:")
                throw new Exception( "The file does not seem to follow the mmap.txt format created "
                    + "by the TChip16_1.4.5 assembler. Please recompile with this version and use the -m switch");

            RawLabels = rows.Skip(2).ToList();
            RawLabels.RemoveAt(RawLabels.Count - 1);

            foreach (var rawLabel in RawLabels)
            {
                var label = new LineLabel();
                label.InterpretLabel(rawLabel);
                Labels.Add(label);
            }
            return Labels;
        }
        
    }
}
