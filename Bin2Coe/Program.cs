using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Bin2Coe.ListExtensions;

namespace Bin2Coe
{
    class Program
    {
        static void Main(string[] args)
        {
            var fullPath = Path.GetFullPath(args[0]);
            var currentDir = Path.GetDirectoryName(fullPath);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullPath);
            var binFile = File.ReadAllBytes(fullPath);

            var coeFileContent = new StringBuilder();
            coeFileContent.AppendLine("memory_initialization_radix=16;");
            coeFileContent.AppendLine("memory_initialization_vector=");
            var valuesPerLine = 16;
            var currentIndex = 0;

            foreach (var FourValues in binFile.SplitIntoSets<byte>(4))
            {
                //var listWithValues = FourValues.Select(fv => fv>>4).ToList();
                //var fourBitGreyValues = (listWithValues[3] << 12) + (listWithValues[2] << 8) + (listWithValues[1] << 4) + listWithValues[0];

                var listWithValues = FourValues.ToList();
                var fourBitGreyValues = (listWithValues[3] << 12) + (listWithValues[2] << 8) + (listWithValues[1] << 4) + listWithValues[0];
                var byteText = fourBitGreyValues.ToString("X4");
                coeFileContent.Append($"{byteText},");
                currentIndex++;
                if(currentIndex == valuesPerLine)
                {
                    currentIndex = 0;
                    coeFileContent.AppendLine();
                }
            }

            var coeFullPath = Path.Combine(currentDir, fileNameWithoutExtension + ".coe");
            File.WriteAllText(coeFullPath, coeFileContent.ToString());
        }
    }
}
