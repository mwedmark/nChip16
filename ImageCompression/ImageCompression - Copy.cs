using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip16
{
    public class ImageCompression
    {
        private byte[] Data;
        private List<int> CompressData = new List<int>();
        private List<ushort> Dictionary = new List<ushort>();

        private List<byte> ByteDictionary = new List<byte>(); 
        private string outputFileName;

        public ImageCompression(){}

        public void PerformNibbleCompression(string filePath)
        {
            Data = File.ReadAllBytes(filePath);

            CreateCompressedNibbleData();

            var compressedDataBytes = CompressData.Count;
            Console.WriteLine("Compressed Data: {0} bytes", compressedDataBytes);
            const int paletteSize = 3 * 16;
            Console.WriteLine("Palette Data: {0} bytes", paletteSize);
            Console.WriteLine("All Data: {0} bytes", compressedDataBytes + paletteSize);

            // write all data to output file
            outputFileName = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath) + ".cci";
            var outputFileData = new List<byte>();

            // Write X and Y sizes to be able to write imgae correct on target
            outputFileData.AddRange(ConvertCompressNibbleDataToBytes());

            File.WriteAllBytes(outputFileName, outputFileData.ToArray());

            // make reference decompresssion and compare result

            var refimage = ReferenceNibbleDecompressImage();
            if (refimage.Count != Data.Length)
                Console.WriteLine("incorrect size of reference decompression!");

            for (int i = 0; i < refimage.Count; i++)
            {
                if (refimage[i] != Data[i])
                {
                    Console.WriteLine("Data error! Not same data in reference decompression at {0}!", i);
                    return;
                }
            }
            Console.WriteLine("Exact match of reference data!");
             
        }

        private void CreateCompressedNibbleData()
        {
            var totalCounter = 0;
            var nibbleData = new List<byte>();

            // create nibble data for simpler compression alg.
            for (int i = 0; i < Data.Length; i++)
            {
                nibbleData.Add((byte)((Data[i]&0xF0)>>4));
                nibbleData.Add((byte)(Data[i] & 0xF));    
            }

            byte lastNibble = nibbleData[0];
            byte counter = 0xff;
            for (int i = 0; i < nibbleData.Count; i++)
            {
                counter++;
                var currentNibble = nibbleData[i];

                if (((i+1) == nibbleData.Count))
                {
                    totalCounter += counter;
                    counter++;
                    if (counter <= 15)
                    {
                        // 4-bit data and 4-bit counter
                        CompressData.Add(lastNibble + ((counter) << 4));
                    }
                    
                    else
                    {
                        CompressData.Add(lastNibble);
                        CompressData.Add((counter & 0xFF));
                    }
                    continue;
                }

                if ((currentNibble != lastNibble) || counter == 255)
                {
                    totalCounter += counter;
                    if(counter <= 15)
                    {
                        // 4-bit data and 4-bit counter
                        CompressData.Add(lastNibble + ((counter) << 4));
                    }
                    
                    else
                    {
                        CompressData.Add(lastNibble);
                        CompressData.Add((counter & 0xFF));
                    }
                    lastNibble = currentNibble;
                    counter = 0;
                }
            }
        }

        public void PerformByteCompression(string filePath)
        {
            Data = File.ReadAllBytes(filePath);

            CreateCompressedByteData();

            var compressedDataBytes = CompressData.Count*2;
            Console.WriteLine("Compressed Data: {0} bytes", compressedDataBytes);
            const int paletteSize = 3 * 16;
            Console.WriteLine("Palette Data: {0} bytes", paletteSize);
            Console.WriteLine("All Data: {0} bytes",  compressedDataBytes + paletteSize);

            // write all data to output file
            outputFileName = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath) + ".cci";
            var outputFileData = new List<byte>();

            // Write X and Y sizes to be able to write imgae correct on target
            outputFileData.AddRange(ConvertCompressDataToBytes());

            File.WriteAllBytes(outputFileName, outputFileData.ToArray());

            // make reference decompresssion and compare result
            var refimage = ReferenceByteDecompressImage();
            if (refimage.Count != Data.Length)
                Console.WriteLine("incorrect size of reference decompression!");

            for (int i = 0; i < refimage.Count; i++)
            {
                if (refimage[i] != Data[i])
                {
                    Console.WriteLine("Data error! Not same data in reference decompression at {0}!", i);
                    return;
                }
            }
            Console.WriteLine("Exact match of reference data!");
        }

        private List<byte> ReferenceByteDecompressImage()
        {
            var decompressedImage = new List<byte>();

            var compressedImage = File.ReadAllBytes(outputFileName);

            var compPointer = 0;
            // recreate image data
            while (compPointer < compressedImage.Length)
            {
                var data = compressedImage[compPointer];
                var count = compressedImage[compPointer + 1];

                for (int i = 0; i < count; i++)
                    decompressedImage.Add(data);

                compPointer += 2;
            }

            return decompressedImage;
        }

        private List<byte> ReferenceNibbleDecompressImage()
        {
            var decompressedImageNibbles = new List<byte>();
            var decompressedImageBytes = new List<byte>();
            var compressedImage = File.ReadAllBytes(outputFileName);

            var compPointer = 0;
            var fullCounter = 0;
            // recreate image data
            for (int i = 0; i < compressedImage.Length;i++ )
            {
                var compData = compressedImage[i];

                var data = (byte)(compData & 0xF);
                var count = (byte)((compData & 0xF0) >> 4);
                fullCounter += count;

                if (count == 0) // activated LotsOfData-mode
                {
                    i++;
                    count = compressedImage[i];
                }

                for (int x = 0; x < count; x++)
                    decompressedImageNibbles.Add(data);
            }

            for(int i=0;i<(decompressedImageNibbles.Count-1);i+=2)
            {
                var nibble1 = decompressedImageNibbles[i];
                var nibble2 = decompressedImageNibbles[i+1];
                decompressedImageBytes.Add( (byte)((nibble1<<4) + nibble2));
            }

            return decompressedImageBytes;
        }

        private void CreateCompressedByteData()
        {
            byte lastByte = 0;
            byte counter = 0xff;
            for (int i = 0; i < Data.Length; i++)
            {
                counter++;
                var currentByte = Data[i];
                if (i == 0)
                    lastByte = currentByte;

                if ((currentByte != lastByte) || (counter == 255))
                {
                    // 8-bit index and 8-bit counter
                    CompressData.Add(lastByte + ((counter) << 8));
                    counter = 0;
                    lastByte = currentByte;
                }

                if ((i + 1) == Data.Length)
                {
                    CompressData.Add(lastByte + ((counter+1) << 8));
                }
            }
        }

        public void PerformWordCompression(string filePath)
        {
            Data = File.ReadAllBytes(filePath);

            // find all dictionary entries (pairs of consecutive bytes)
            for (int i = 0; i < Data.Length; i += 2)
            {
                var Byte1 = Data[i];
                var Byte2 = Data[i+1];
                var dicKey = (ushort)((Byte1 << 8) + Byte2);
                if(!Dictionary.Contains(dicKey))
                    Dictionary.Add(dicKey);
            }
            var dictionarySizeBytes = Dictionary.Count*2;
            Console.WriteLine("Found {0} keys! Dictionary size: {1} bytes", Dictionary.Count, Dictionary.Count*2);
            
            CreateCompressedWordData();
            
            var dicSizeBytes = Convert.ToString(Dictionary.Count, 2);
            var dicEntrySize = 16;// dicSizeBytes.Length;
            Console.WriteLine("Number of bits to save each key: {0} bits => xxx bytes", dicEntrySize);
            var compressedDataBytes = CompressData.Count*2;
            Console.WriteLine("Compressed Data: {0} bytes", compressedDataBytes);
            const int paletteSize = 3*16;
            Console.WriteLine("Palette Data: {0} bytes", paletteSize);
            Console.WriteLine("All Data: {0} bytes", dictionarySizeBytes+compressedDataBytes+paletteSize);

            // write all data to output file
            outputFileName = Path.GetDirectoryName(filePath) +"\\"+ Path.GetFileNameWithoutExtension(filePath) + ".cci";
            var outputFileData = new List<byte>();

            outputFileData.AddRange(WriteDictionarySize());
            outputFileData.AddRange(ConvertDictionaryToBytes());
            // Write X and Y sizes to be able to write imgae correct on target
            outputFileData.AddRange(ConvertCompressDataToBytes());

            File.WriteAllBytes(outputFileName, outputFileData.ToArray());

            // make reference decompresssion and compare result
            var refimage = ReferenceWordDecompressImage();
            if(refimage.Count != Data.Length)
                Console.WriteLine("incorrect size of reference decompression!");

            for (int i = 0; i < refimage.Count; i++)
            {
                if (refimage[i] != Data[i])
                {
                    Console.WriteLine("Data error! Not same data in reference decompression at {0}!", i);
                    return;
                }
            }
            Console.WriteLine("Exact match of reference data!");
        }

        private IEnumerable<byte> WriteDictionarySize()
        {
            var size = new List<byte>();

            var dictionarySize = Dictionary.Count;
            size.Add((byte)(dictionarySize & 0xFF));
            size.Add((byte)((dictionarySize>>8) & 0xFF));

            return size;
        }

        private List<byte> ReferenceWordDecompressImage()
        {
            var decompressedImage = new List<byte>();

            var compressedImage = File.ReadAllBytes(outputFileName);

            // recreate dictionary
            var dictionary = new List<ushort>();
            var dictionarySize = compressedImage[0] + (compressedImage[1] << 8);
            var currentKey = 1;
            var compPointer = 2;
            while (currentKey <= dictionarySize)
            {
                dictionary.Add((ushort)(compressedImage[compPointer] + (compressedImage[compPointer + 1] << 8)));
                compPointer += 2;
                currentKey++;
            }
            // recreate image data
            while (compPointer != compressedImage.Length)
            {
                var byte1 = compressedImage[compPointer];
                var byte2 = compressedImage[compPointer+1];

                // ERROR READING THIS
                var word = (ushort)(byte1 + (byte2 << 8));
                var index = word & 0x7FF;
                var count = (word - index) >> 11;

                var dicKey = Dictionary[index];
                for (int i = 0; i < count;i++ )
                {
                    var lowByte = (byte)(dicKey & 0xFF);
                    var highByte = (byte) ((dicKey & 0xFF00) >> 8);
                    decompressedImage.Add(highByte);
                    decompressedImage.Add(lowByte);
                }
                compPointer += 2;
            }

            return decompressedImage;
        }

        private IEnumerable<byte> ConvertCompressDataToBytes()
        {
            var compressData = new List<byte>();

            for (int i = 0; i < CompressData.Count;i++)
            {
                var data1 = CompressData[i];
                var byte1 = (byte)(data1 & 0xFF);
                var byte2 = (byte) (((data1 & 0xFF00) >> 8));
                compressData.Add(byte1);
                compressData.Add(byte2);
            }

            return compressData;
        }

        private IEnumerable<byte> ConvertCompressNibbleDataToBytes()
        {
            var compressData = new List<byte>();

            for (int i = 0; i < CompressData.Count; i++)
            {
                var data1 = CompressData[i];
                compressData.Add((byte)data1);
            }

            return compressData;
        }

        private IEnumerable<byte> ConvertDictionaryToBytes()
        {
            var dictionaryBytes = new List<byte>();

            for (int i = 0; i < Dictionary.Count;i++)
            {
                var dicKey = Dictionary[i];
                dictionaryBytes.Add((byte)(dicKey & 0xFF));
                dictionaryBytes.Add((byte)((dicKey & 0xFF00)>>8));
            }

            return dictionaryBytes;
        }

        private void CreateCompressedWordData()
        {
            ushort lastIndex = 0;
            byte counter = 0xff;
            for (int i = 0; i < Data.Length; i +=2)
            {
                counter++;
                var Byte1 = Data[i];
                var Byte2 = Data[i + 1];
                var dicKey = (ushort)((Byte1 << 8) + Byte2);
                var index = Dictionary.FindIndex(0, k => k == dicKey);

                if (i == 0)
                    lastIndex = (ushort)index;

                if ((index != lastIndex) || (counter == 31) || ((i+2) == Data.Length))
                {
                    // 11-bit index and 5-bit counter
                    CompressData.Add(lastIndex + ((counter) << 11));
                    counter = 0;
                    lastIndex = (ushort)index;
                }
                
            }
        }
    }
}
