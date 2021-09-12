using Chip16.Shared;
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
            //var currentDir = Path.GetDirectoryName(fullPath);
            var currentDir = !string.IsNullOrWhiteSpace(Path.GetDirectoryName(fullPath))
                ? Path.GetDirectoryName(fullPath)
                : Directory.GetCurrentDirectory();
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullPath);
            // var binFile = File.ReadAllBytes(fullPath);

            var fileStructure = LoadProgram(fullPath);
            var coeFileContent = createValueForBinFile(fileStructure.Romdata);

            var coeFullPath = Path.Combine(currentDir, fileNameWithoutExtension + ".coe");
            File.WriteAllText(coeFullPath, coeFileContent.ToString());
        }

        private static string createValueForGreyfile(byte[] binFile)
        {
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
                if (currentIndex == valuesPerLine)
                {
                    currentIndex = 0;
                    coeFileContent.AppendLine();
                }
            }

            return coeFileContent.ToString();
        }

        private static string createValueForBinFile(byte[] binFile)
        {
            var coeFileContent = new StringBuilder();
            coeFileContent.AppendLine("memory_initialization_radix=16;");
            coeFileContent.AppendLine("memory_initialization_vector=");
            var valuesPerLine = 1;
            var currentIndex = 0;

            // start with strange instruction Big endian and Data small endian
            foreach (var FourValues in binFile.SplitIntoSets<byte>(4))
            {
                //var listWithValues = FourValues.Select(fv => fv>>4).ToList();
                //var fourBitGreyValues = (listWithValues[3] << 12) + (listWithValues[2] << 8) + (listWithValues[1] << 4) + listWithValues[0];

                var listWithValues = FourValues.ToList();
              
                if(listWithValues.Count > 0)
                { 
                    var coeLine1 = (listWithValues.Count > 1) ? ((listWithValues[0]<<8) + listWithValues[1]) : listWithValues[0];
                    var byteText1 = coeLine1.ToString("X4");
                    coeFileContent.Append($"{byteText1}, ");
                }
                if (listWithValues.Count > 2)
                {
                    var coeLine2 = (listWithValues.Count > 3) ? (listWithValues[2] + (listWithValues[3] << 8)) : listWithValues[2];
                    var byteText2 = coeLine2.ToString("X4");
                    coeFileContent.Append($"{byteText2},");
                }
                currentIndex++;
                if (currentIndex == valuesPerLine)
                {
                    currentIndex = 0;
                    coeFileContent.AppendLine();
                }
            }

            return coeFileContent.ToString();
        }

        internal static FileStructure LoadProgram(string programPath)
        {
            //var memory = new List<byte>();
            //memory
            // start by interpret the complete file
            var CurrentFileStructure = FileStructure.InterpretFile(programPath);

            if (Encoding.UTF8.GetString(CurrentFileStructure.MagicNumber) != "CH16")
                throw new Exception("Magic number is incorrect!");

            // load program into address 0 in memory
            //for (int index = 0; index < CurrentFileStructure.RomSize; index++)
            //{
            //    var dataByte = CurrentFileStructure.Romdata[index];
            //    memory.WriteByte(index, dataByte);
            //}

            return CurrentFileStructure;

            // CRC32 checksum of ROM (excluding header) (polynomial: 0x04C11DB7)
            //var crc32 = new CRC32(0x04C11DB7); // poly given via docs for Chip16
            //var calculatedChecksum = crc32.ComputeHash(currentFileStructure.Romdata);

            //if (calculatedChecksum != currentFileStructure.Checksum)
            {
                // Show Error message
            }

            // set starting PC correct
            //PC = CurrentFileStructure.StartAddress;

            // try and find the xxx.txt file in the same directory (where xxx is the program name) and read it
            //var mmapFullPath = Path.GetDirectoryName(programPath) + "\\" +
            //    Path.GetFileNameWithoutExtension(programPath) + ".txt";

            //if (File.Exists(mmapFullPath))
            //{
            //    Labels = MMapImport.ImportFile(mmapFullPath);
            //    UsingLineLabels = true;
            //}
        }

    }
}
