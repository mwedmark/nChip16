using System;

namespace nChip16
{
    public class Chip16Framebuffer
    {
        public const int Xmax = 320;
        public const int Ymax = 240;
        public const int ColorIndexCount = 16;
        private readonly byte[] Framebuffer = new byte[Xmax*Ymax];
        public int FullSize { get { return Xmax*Ymax; } }
        public void SetPixel(int x, int y, byte colorIndex)
        {
            if (colorIndex == 0)
                return;

            if (x >= Xmax)
                throw new Exception("too high X coord in SetPixel");

            if (y >= Ymax)
                throw new Exception("too high Y coord in SetPixel");

            if (colorIndex >= ColorIndexCount)
                throw new Exception("colorIndex > 15 in SetPixel");

            Framebuffer[y*Xmax + x] = colorIndex;
        }

        public byte GetPixel(int x, int y)
        {
            return Framebuffer[y*Xmax + x];
        }

        public void ClearBuffer(ushort bgc)
        {
            for (int i = 0; i < FullSize;i++ )
                Framebuffer[i] = (byte)bgc;
        }
    }
}
