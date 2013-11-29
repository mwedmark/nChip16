using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageCompression;

namespace Chip16
{
    public class ImageCompression
    {
        private byte[] Data;
        private List<int> CompressData = new List<int>();
        private List<ushort> Dictionary = new List<ushort>();
        private List<int> NonPacketData =  new List<int>();
        private const int MaxCountPerNibble = 2047;
        private CciHeader CompressedFileHeader = new CciHeader();
        private string outputFileName;

        public ImageCompression(){}

        public void PerformNibbleCompression(string filePath)
        {
            Data = File.ReadAllBytes(filePath);

            //extract the first 3 bytes as X,Y size information. This is needed for the destination compressed format
            var binFileHeaderSize = 3;
            var headerData = Data.Take(binFileHeaderSize).ToArray();
            CompressedFileHeader = IntepretBinHeader(headerData);
             Data = Data.Skip(binFileHeaderSize).ToArray();

            CreateCompressedNibbleData();

            var compressedDataBytes = CompressData.Count;
            Console.WriteLine("Compressed Data: {0} bytes", compressedDataBytes);
            const int paletteSize = 3 * 16;
            Console.WriteLine("Palette Data: {0} bytes", paletteSize);
            Console.WriteLine("All Data: {0} bytes", compressedDataBytes + paletteSize);

            // write all data to output file
            var fullPath = Path.GetFullPath(filePath);
            outputFileName = Path.GetDirectoryName(fullPath) + "\\" + Path.GetFileNameWithoutExtension(fullPath) + ".cci";
            var outputFileData = new List<byte>();

            var outputBytes = ConvertCompressNibbleDataToBytes().ToList();
            CompressedFileHeader.FileSize = (ushort)(outputBytes.Count);

            outputFileData.AddRange(CompressedFileHeader.GetRawRepresentation());
            outputFileData.AddRange(outputBytes);

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

        private static CciHeader IntepretBinHeader(byte[] headerData)
        {
            var cciHeader = new CciHeader();

            cciHeader.X = (ushort) ((headerData[0] << 8) + headerData[1]);
            cciHeader.Y = headerData[2];

            return cciHeader;
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

            byte lastNibble1 = nibbleData[0];
            int counter1 = -1;

            // pass 1 - Create Tuples with nibble data and count for each nibble data change
            var nibbleAndCount = new List<Tuple<byte, int>>();
            byte currentNibble1 = 0;
            for (int index = 0; index < nibbleData.Count; index++)
            {
                var currentNibble = nibbleData[index];
                currentNibble1 = currentNibble;
                counter1++;
                if (currentNibble != lastNibble1)
                {
                    nibbleAndCount.Add(new Tuple<byte, int>(lastNibble1, counter1));
                    lastNibble1 = currentNibble;
                    counter1 = 0;
                    if((index == nibbleData.Count-1)) // to get the last nibble IF it has a unique color
                        nibbleAndCount.Add(new Tuple<byte, int>(currentNibble, 1));
                }
            }
            if (counter1 != 0)
                nibbleAndCount.Add(new Tuple<byte, int>(currentNibble1, counter1+1));

            var totalCounter1 = nibbleAndCount.Sum(nc => nc.Item2);
            Console.WriteLine("Total number of nibbles:{0}", totalCounter1);
            Console.WriteLine("Number of nibble changes: {0}", nibbleAndCount.Count);

            // pass 2 - Split and create new smaller Tuples out of each big one
            var newList = new List<Tuple<byte, int>>();
            foreach (var currentNc in nibbleAndCount)
            {
                if (currentNc.Item2 > MaxCountPerNibble)
                {
                    var smallerItems = SplitIntoSmaller(currentNc);
                    //removeList.Add(currentNc);
                    newList.AddRange(smallerItems);
                }
                else
                    newList.Add(currentNc);
            }

            // pass 3 - Regroup nibble as new type of Tuple that can hold List<byte>
            var nibbleListAndCount = new List<Tuple< List<byte>, int>>();

            var smallGroupNibble = new List<byte>();
            for (int i = 0; i < newList.Count; i++)
            {
                var currentNibble = newList[i];

                if (currentNibble.Item2 < 5)
                {
                    for (int x = 0; x < currentNibble.Item2;x++)
                        smallGroupNibble.Add(currentNibble.Item1);
                }
                else
                {
                    if (smallGroupNibble.Count != 0)
                    {
                        var newTuple = new Tuple<List<byte>, int>(new List<byte>(smallGroupNibble),1);
                        nibbleListAndCount.Add(newTuple);
                        smallGroupNibble.Clear();
                    }
                    var miniList = new Tuple<List<byte>, int>(new List<byte>{currentNibble.Item1},currentNibble.Item2);
                    nibbleListAndCount.Add(miniList);
                }
            }
            // add last nibble group if there is something left
            if (smallGroupNibble.Count != 0)
            {
                var newTuple = new Tuple<List<byte>, int>(new List<byte>(smallGroupNibble), 1);
                nibbleListAndCount.Add(newTuple);

                smallGroupNibble.Clear();
            }

            // pass 4 - split big data collection into smaller 255 byte max groups
            var nibbleListAndCountSplit = new List<Tuple<List<byte>, int>>();

            foreach (var tuple in nibbleListAndCount)
            {
                if (tuple.Item1.Count < 255)
                {
                    nibbleListAndCountSplit.Add(tuple);
                    continue;
                }

                // this tuple is >255 chars long and will probably be a DataCollection.
                // Split into 255 char parts
                var splittedTuple = SplitIntoSmaller(tuple);
                nibbleListAndCountSplit.AddRange(splittedTuple);
            }

            var singleByteCount = 0;
            var bigDataCount = 0;
            var dataCollCount = 0;
            // pass 5 - roll-out into compressed data format
            for (int i = 0; i < nibbleListAndCountSplit.Count; i++)
            {
                var nibble = nibbleListAndCountSplit[i];
                // decide which of 3 different compress-types that should be used for this nibble
                if ((nibble.Item2 > 1) && (nibble.Item2 < 8) || (nibble.Item2) == 1 && (nibble.Item1.Count == 1))
                {
                    singleByteCount++;
                    CompressData.AddRange(CreateSingleByteNibble(nibble));
                }
                else if (nibble.Item2 >= 8)
                {
                    bigDataCount++;
                    CompressData.AddRange(CreateBigDataNibble(nibble));
                }
                else if ((nibble.Item1.Count > 1) && (nibble.Item2 == 1))
                {
                    if(nibble.Item1.Count > 254)
                        throw new Exception("Error in DataColl. size, >254");

                    dataCollCount++;
                    CompressData.AddRange(CreateDataCollection(nibble));
                }
                else
                    throw new Exception("Error in preparation data for compression");
            }
            Console.WriteLine("SingleByteNibbles:{0}", singleByteCount);
            Console.WriteLine("BigData:{0}", bigDataCount);
            Console.WriteLine("DataCollections:{0}", dataCollCount);

            #region OldSolution
            /*
            byte lastNibble = nibbleData[0];
            byte counter = 0xff;

                for (int i = 0; i < nibbleData.Count; i++)
                {
                    counter++;
                    var currentNibble = nibbleData[i];

                    if (((i + 1) == nibbleData.Count))
                    {
                        //WriteNonPacketData();

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
                        if (counter <= 15) // change to 7 to use MSB of counter bits for below DitheredDataMode
                        {
                            //if(counter > 3) // more than 3 nibbles in a row wtih the same value
                            {
                                //WriteNonPacketData();
                                // 4-bit data and 4-bit counter
                                CompressData.Add(lastNibble + ((counter) << 4));
                                lastNibble = currentNibble;
                                counter = 0;
                            }

                          
                        }

                        else
                        {
                            //WriteNonPacketData();
                            CompressData.Add(lastNibble);
                            CompressData.Add((counter & 0xFF));
                            lastNibble = currentNibble;
                            counter = 0;
                        }

                    }
                }

            */
            #endregion
        }

        private List<Tuple<List<byte>, int>> SplitIntoSmaller(Tuple<List<byte>, int> tuple)
        {
            const int nSize = 254;
            var splittedParts = new List<Tuple<List<byte>, int>>();

            var list = new List<byte>(tuple.Item1);

            while (list.Any())
            {
                splittedParts.Add(new Tuple<List<byte>, int>(list.Take(nSize).ToList(),1));
                list = list.Skip(nSize).ToList();
            }

            return splittedParts;
        }

        /// <summary>
        /// This is used for nibble counts between 1-7, found by looking a first byte's MSB=0 => 0XXX'XXXX 
        /// </summary>
        /// <returns></returns>
        private static List<int> CreateSingleByteNibble(Tuple< List<byte>, int> nibble)
        {
            var data = new List<int>();
            data.Add(nibble.Item1[0] + (nibble.Item2<<4));
            return data;
        }

        /// <summary>
        /// Used for counts over 7, found by looking att first byte's MSB=1 => 1XXX'XXXX 
        /// </summary>
        /// <param name="nibble"></param>
        /// <returns></returns>
        private static List<int> CreateBigDataNibble(Tuple<List<byte>, int> nibble)
        {
            var data = new List<int>();

            var countover255 = (nibble.Item2 & 0x700)>>4;
            data.Add(nibble.Item1[0] | 0x80 | countover255); // 0x80 to show BigData
            data.Add(nibble.Item2 & 0xFF);
            return data;
        }

        /// <summary>
        /// Used for collections of data that has lots of different nibbles randomly used.
        /// Found by looking at first byte = 0x00
        /// second byte: count of random nibbles following
        /// following bytes holds packed nibbles of data, that is a copy from the original data
        /// </summary>
        /// <param name="nibble"></param>
        /// <returns></returns>
        private static List<int> CreateDataCollection(Tuple<List<byte>, int> nibble)
        {
            var data = new List<int>();
            data.Add(0); // shows DataCollection
            data.Add(nibble.Item1.Count);
            data.AddRange(CreateNibbleData(nibble.Item1));

            return data;
        }

        private static IEnumerable<int> CreateNibbleData(List<byte> list)
        {
            var count = list.Count;
            var nibbleData = new List<int>();

            var extraNibbleAtEnd = (count & 0x1) == 1;

            if (extraNibbleAtEnd)
                count--;

            for (int i = 0; i < count; i += 2)
                nibbleData.Add(list[i]<<4 | list[i+1]);

            if (extraNibbleAtEnd)
                nibbleData.Add(list.Last() << 4);

            return nibbleData;
        }

        private List< Tuple<byte,int> > SplitIntoSmaller(Tuple<byte, int> currentNc)
        {
            var endResult = new List<Tuple<byte, int>>();
            int result = 0;
            var rem = Math.DivRem(currentNc.Item2, MaxCountPerNibble, out result);
            for (int x = 0; x < rem;x++ )
                endResult.Add(new Tuple<byte, int>(currentNc.Item1, MaxCountPerNibble));

            if (result != 0)
                endResult.Add(new Tuple<byte, int>(currentNc.Item1, result));

            return endResult;
        }

        public void WriteNonPacketData()
        {
            if (NonPacketData.Count == 0)
                return;

            if(NonPacketData.Count > 127)
                throw new Exception("Too large block of NonPacketData!!");
            // the NonPacketData holds data not saved, save it now!
            CompressData.Add((byte)((NonPacketData.Count&0x7F)|0x80));
            CompressData.AddRange(NonPacketData);
            NonPacketData.Clear();
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

            compressedImage = compressedImage.Skip(CciHeader.HeaderSize).ToArray();

            var compPointer = 0;
            var fullCounter = 0;
            // recreate image data
            for (int i = 0; i < compressedImage.Length;i++ )
            {
                var compData = compressedImage[i];

                if (((compData & 0x80) == 0) && (compData != 0)) // single byte nibble data
                    decompressedImageNibbles.AddRange(DecompressSingleByte(compData));
                else if ((compData & 0x80) == 0x80) // Big data
                {
                    i++;
                    int count = compressedImage[i];
                    decompressedImageNibbles.AddRange(DecompressBigData(compData, count));
                }
                else if (compData == 0x00) // Data collection data
                {
                    i++;
                    var count = compressedImage[i];
                    i++;
                    var data = new List<byte>();
                    var isOddNibbles = (count & 1) != 0;
                    for (int x = 0; x < (count/2); x++)
                    {
                        var doubleDataNibble = compressedImage[i++];
                        var firstNibble =  (doubleDataNibble & 0xF0) >> 4;
                        var secondNibble = (doubleDataNibble & 0x0F);
                        data.Add((byte)firstNibble);
                        data.Add((byte)secondNibble);
                    }

                    if (isOddNibbles)
                    {
                        var oddNibble = (byte)((compressedImage[i] & 0xF0) >> 4);
                        data.Add(oddNibble);
                    }
                    else
                        i--;

                    decompressedImageNibbles.AddRange(DecompressDataCollection(data));
                }
                else
                    throw new Exception("unknown type of data ");

            }

            for(int i=0;i<(decompressedImageNibbles.Count-1);i+=2)
            {
                var nibble1 = decompressedImageNibbles[i];
                var nibble2 = decompressedImageNibbles[i+1];
                decompressedImageBytes.Add( (byte)((nibble1<<4) + nibble2));
            }

            return decompressedImageBytes;
        }

        private List<byte> DecompressDataCollection(List<byte> dataCollection)
        {
            var data = new List<byte>(dataCollection);

            return data;
        }

        private List<byte> DecompressSingleByte(byte singleByteData)
        {
            var data = new List<byte>();

            var count = (singleByteData & 0x70) >> 4;
            var dataNibble = singleByteData & 0x0F;

            for (int i = 0; i < count;i++)
                data.Add((byte)dataNibble);

            return data;
        }

        private List<byte> DecompressBigData(byte byteData, int count)
        {
            var data = new List<byte>();

            var nibbleData = (byte)(byteData & 0x0F);
            var fullCount = ( (byteData & 0x70) << 4) + count;
            for (int i = 0; i < fullCount; i++)
                data.Add(nibbleData);

            return data;
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
