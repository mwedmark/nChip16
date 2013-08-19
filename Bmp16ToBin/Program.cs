using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bmp16ToBin
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var filePath in args)
                MakeBin(filePath);
        }

        private static void MakeBin(string filePath)
        {
            var bmpPath = filePath;

            var bmpImage = new Bitmap(bmpPath);
            var binData = new List<byte>();
            var palette = new List<Color>();
            //if(bmpImage.Width )
            //    throw new Exception("BMp16ToBin can only handle images as 320*240");

            //binData.Add((byte)(bmpImage.Width >> 1));
            //binData.Add((byte)(bmpImage.Height));

            // first build palette
            for (int y = 0; y < bmpImage.Height; y++)
            {
                for (int x = 0; x < bmpImage.Width; x++)
                {
                    var currentPixel = bmpImage.GetPixel(x, y);
                    if (!palette.Contains(currentPixel))
                        palette.Add(currentPixel);
                }
            }

            // try and match closest color to black and move it to index 0
            palette.Sort((color, color1) => (color.R + color.G + color.B) - (color1.R + color1.G + color1.B));
            File.WriteAllBytes(bmpPath + ".PAL", ToByteArray(palette));

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
