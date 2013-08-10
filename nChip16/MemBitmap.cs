using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace nChip16
{
    /// <summary>
    /// This class was derived from "Fast Pointerless Image Processing in .NET", J. Dunlap, Nov 2006, http://www.codeproject.com/KB/GDI-plus/pointerlessimageproc.aspx
    /// </summary>
    public class MemBitmap
    {
        //member variables

        private Bitmap bitmap;
        private byte[] bits;
        private GCHandle handle;
        private int stride;
        private int pixelFormatSize;
        private int height;
        private int width;

        //creation routine
        public MemBitmap( int AWidth, int AHeight )
        {
            Resize(AWidth, AHeight);
        }

        public void Resize(int AWidth, int AHeight)
        {
            height = AHeight;
            width = AWidth;
            var format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            pixelFormatSize = Image.GetPixelFormatSize(format) / 8;
            stride = width * pixelFormatSize;
            bits = new byte[stride * height];
            handle = GCHandle.Alloc(bits, GCHandleType.Pinned);
            IntPtr pointer = Marshal.UnsafeAddrOfPinnedArrayElement(bits, 0);
            bitmap = new Bitmap(width, height, stride, format, pointer);
        }

        public Bitmap Bitmap
        {
            get { return bitmap; }
        }

        public byte[] Bits
        {
            get { return bits; }
        }


        public int Height
        {
            get { return height; }
            /*set
            {
                Resize(this.Width, value);
            }*/
        }

        public int Width
        {
            get { return width; }
            /*set
            {
                Resize(value, this.Height);
            }*/
        }

        public void SetPixel(int x, int y, byte A, byte R, byte G, byte B)
        {
            int pos = (x + y * width) * pixelFormatSize;
            bits[pos] = B;
            bits[pos+1] = G;
            bits[pos+2] = R;
            bits[pos+3] = A;
        }

        public Color GetPixel(int x, int y)
        {
            int pos = (x + y * width) * pixelFormatSize;
            byte B = bits[pos];
            byte G = bits[pos+1];
            byte R = bits[pos+2];
            byte A = bits[pos+3];
            return Color.FromArgb(A, R, G, B);
        }

        /// <summary>
        /// Translates a single value into a rainbow of colors from red -> green -> blue
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="val"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        public void SetPixelTranslate(int x, int y, int val, int max, int min)
        {
            int irange = max - min;
            val = (val - min) % irange + min;
            double range = irange;
            double per50 = range * 0.5;
            double newVal = val - min;
            double temp = (newVal)/(range/ 2);
            byte R = (byte)(255.0 / (temp * temp + 1.0));
            temp = (newVal - per50) / (range / 3);
            byte G = (byte)(255.0 / (temp * temp + 1.0));
            temp = (newVal - range) / (range / 3);
            byte B = (byte)(255.0 / (temp * temp + 1.0));
            SetPixel(x, y, 255, R, G, B);
        }

    }
}
