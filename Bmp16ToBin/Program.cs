using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Bmp16ToBin
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileList = ExtractFiles(args);

            foreach (var filePath in fileList)
                MakeBin(filePath);
        }

        private static List<string> ExtractFiles(string[] pars)
        {
            var fileList = new List<string>();

            if ((pars[0] == "/d") && (pars.Length == 2))
            {
                var dirPath = pars[1];
                // extract all files in the given directory
                if (!Directory.Exists(dirPath))
                    throw new Exception("The given second parameter is not a directory");

                var pictureFiles = Directory.GetFiles(dirPath, "*.bmp");
                fileList.AddRange(pictureFiles);
            }
            else
            {
                fileList.AddRange((pars));
            }

            return fileList;
        }

        private static void MakeBin(string filePath)
        {
            var bmpPath = filePath;

            var bmpImage = new Bitmap(bmpPath);
            var binData = new List<byte>();
            var palette = new List<Color>();

            // first build palette
            // color of pixel (0,0) will set background color
            for (int y = 0; y < bmpImage.Height; y++)
            {
                for (int x = 0; x < bmpImage.Width; x++)
                {
                    var currentPixel = bmpImage.GetPixel(x, y);
                    if (!palette.Contains(currentPixel))
                        palette.Add(currentPixel);
                }
            }

            if (palette.Count > 16)
            {
                throw new Exception("A maximum of 16 colors in each BMP-file can be handled.");
            }
            // Special version for Flags-demo, uses 14 colors per flags and hardcodes
            // 0=Black and 1=White. These are then used for background and text.
            //if(palette.Count > 13)
            //    throw new Exception("Special version, only 13 colors allowed per image");

            //palette.Insert(0, Color.Black); // transparent
            //palette.Insert(1, Color.White);
            //palette.Insert(2, Color.Black); // non-transparent
            
            // make sure all palettes have 16 colors / 48 bytes in size
            while (palette.Count != 16)
                palette.Add(Color.Black);

            // try and match closest color to black and move it to index 0
            //palette.Sort((color, color1) => (color.R + color.G + color.B) - (color1.R + color1.G + color1.B));
            File.WriteAllBytes(bmpPath + ".PAL", ToByteArray(palette));

            // will handle images up to 511x255
            var xHighByte = (byte)((bmpImage.Width&0x100)>>8); // only a single bit used
            var xLowByte = (byte)(bmpImage.Width&0xFF);
            var yByte = (byte)(bmpImage.Height&0xFF);
            binData.Add(xHighByte);
            binData.Add(xLowByte);
            binData.Add(yByte);

            for (int y = 0; y < bmpImage.Height; y++)
            {
                for (int x = 0; x < bmpImage.Width; x += 2)
                {
                    var currentPixelLeft = bmpImage.GetPixel(x, y);
                    var currentPixelRight = bmpImage.GetPixel(x + 1, y);

                    var leftIndex = palette.FindIndex(c => c == currentPixelLeft);
                    var rightIndex = palette.FindIndex(c => c == currentPixelRight);

                    var dataByte = (byte) ((leftIndex << 4) + rightIndex);
                    binData.Add(dataByte);
                }
            }

            File.WriteAllBytes(bmpPath + ".bin", binData.ToArray());
        }

        private static byte[] ToByteArray(List<Color> colorList)
        {
            var byteList = new List<byte>();

            foreach (var color in colorList)
            {
                byteList.Add(color.R);
                byteList.Add(color.G);
                byteList.Add(color.B);
            }

            return byteList.ToArray();
        }
    }
}
