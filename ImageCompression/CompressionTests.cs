using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip16
{
    internal class CompressionTests
    {
        private string FilePath;
        private byte[] ImageData;

        public CompressionTests(string filePath)
        {
            FilePath = filePath;
        }

        private void TestByte()
        {

            Console.WriteLine("original size: {0} bytes", ImageData.Length);
            Console.WriteLine();

            var countChanges = 0;
            var valueList = new List<byte>();
            var lastByteValue = 0xff;
            var longestStreakOfBytes = 0;
            var currentStreakofBytes = 0;

            for (int i = 0; i < ImageData.Length;i++)
            {
                if (!valueList.Contains(ImageData[i]))
                {
                    valueList.Add(ImageData[i]);
                    if (currentStreakofBytes > longestStreakOfBytes)
                        longestStreakOfBytes = currentStreakofBytes;

                    currentStreakofBytes = 0;
                }
                else
                    currentStreakofBytes++;

                if (ImageData[i] != lastByteValue)
                {
                    countChanges++;
                    lastByteValue = ImageData[i];
                }
            }
            Console.WriteLine("{0} changes of byte values", countChanges);
            Console.WriteLine("It holds {0} different values", valueList.Count);
            Console.WriteLine("Longest streak {0} in a row", longestStreakOfBytes);
            Console.WriteLine();
            var dictionarySize = valueList.Count;
            var packedData = Convert.ToString(valueList.Count, 2);
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Dictionary Size:{0} bytes", dictionarySize);
            var dataSize = countChanges * packedData.Length / 8;
            Console.WriteLine("Data size:{0} bytes", dataSize);
            Console.WriteLine("Total size: {0} bytes", dictionarySize + dataSize);
            Console.WriteLine();   
        }

        private void TestWord()
        {
            int countChanges = 0;
            ushort lastWordValue = 0x0000;
            var ushortList = new List<ushort>();
            ushort wordValue = 0;
            for (int i = 0; i < ImageData.Length; i += 2)
            {
                wordValue = (ushort)(ImageData[i] + ImageData[i + 1]);
                if (!ushortList.Contains(wordValue))
                    ushortList.Add(wordValue);

                if (wordValue != lastWordValue)
                {
                    countChanges++;
                    lastWordValue = wordValue;
                }
            }
            Console.WriteLine("{0} changes of word values", countChanges);
            Console.WriteLine("{0} different values of 65536 possible", ushortList.Count);

            int dictionarySize = 2 * ushortList.Count;
            string packedData = Convert.ToString(ushortList.Count, 2);
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Dictionary Size:{0} bytes", dictionarySize);
            int dataSize = countChanges * packedData.Length / 8;
            Console.WriteLine("Data size:{0} bytes", dataSize);
            Console.WriteLine("Total size: {0} bytes", dictionarySize + dataSize);

        }

        private void TestNibble()
        {

            int countChanges = 0;
            int lastByteValue = 0;
            var nibbleList = new List<byte>();
            for (int i = 0; i < ImageData.Length; i++)
            {
                var data = ImageData[i];
                var firstNibble = (data & 0xF0) >> 4;

                if (!nibbleList.Contains((byte)firstNibble))
                    nibbleList.Add((byte)firstNibble);

                if (firstNibble != lastByteValue)
                {
                    countChanges++;
                    lastByteValue = firstNibble;
                }

                var secondNibble = data & 0x0F;
                if (!nibbleList.Contains((byte)secondNibble))
                    nibbleList.Add((byte)secondNibble);

                if (secondNibble != lastByteValue)
                {
                    countChanges++;
                    lastByteValue = secondNibble;
                }
            }
            Console.WriteLine("{0} changes of 4-bit nibbles", countChanges);
            Console.WriteLine("{0} different values of 16 possible", nibbleList.Count);

            int dictionarySize = nibbleList.Count / 2;
            string packedData = Convert.ToString(nibbleList.Count, 2);
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Dictionary Size:{0} bytes", dictionarySize);
            int dataSize = countChanges * packedData.Length / 8;
            Console.WriteLine("Data size:{0} bytes", dataSize);
            Console.WriteLine("Total size: {0} bytes", dictionarySize + dataSize);
            Console.WriteLine();
        }

        private void TestLineBasedNibble()
        {
            var nibbleList = new List<byte>();
            // line based nibble compression, build dictionaries for each line
            var totalSize = 0;
            int countChanges = 0;
            int lastByteValue = 0;

            nibbleList.Clear();
            for (int y = 0; y < 240; y++)
            {
                nibbleList.Clear();
                countChanges = 0;
                for (int x = 0; x < 160; x++)
                {
                    var data = ImageData[x + (y * 160)];

                    var firstNibble = (data & 0xF0) >> 4;
                    if (!nibbleList.Contains((byte)firstNibble))
                        nibbleList.Add((byte)firstNibble);

                    if (firstNibble != lastByteValue)
                    {
                        countChanges++;
                        lastByteValue = firstNibble;
                    }

                    var secondNibble = data & 0x0F;
                    if (!nibbleList.Contains((byte)secondNibble))
                        nibbleList.Add((byte)secondNibble);

                    if (secondNibble != lastByteValue)
                    {
                        countChanges++;
                        lastByteValue = secondNibble;
                    }
                }
                int dictionarySize = nibbleList.Count / 2;
                string packedData = Convert.ToString(nibbleList.Count, 2);
                int dataSize = (countChanges * ((packedData.Length / 8) + 1)) + dictionarySize;
                //dataSize = 320*packedData.Length/8;
                totalSize += dataSize;
            }
            Console.WriteLine();
            Console.WriteLine("Line-based compression");
            Console.WriteLine("Total size: {0} bytes", totalSize);
        }

        public void TestAll()
        {
            var pathToFile = FilePath;
            ImageData = File.ReadAllBytes(pathToFile);

            TestByte();
            TestWord();
            TestNibble();
            TestLineBasedNibble();
        }
    }
}
