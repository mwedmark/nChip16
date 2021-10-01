using nChip16;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FramebufferMonitor
{
    public partial class MainForm : Form, IEmulateWindow
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private readonly List<int> values = new List<int>();
        public void SetBitmap(List<int> newValues)
        {
            values.Clear();
            newValues.ForEach(n => values.Add(n));
            pictureBox1.Invalidate();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        //private SolidBrush solidBrush = new SolidBrush(color);

        //public void SetPixel(int x, int y, Color color)
        //{
        //    //pictureBox1.CreateGraphics().FillRectangle(solidBrush, x, y, 1, 1);
        //    var graphics = pictureBox1.CreateGraphics();
            
        //    using (var pixel = new Bitmap(1, 1, graphics))
        //    {
        //        pixel.SetPixel(0, 0, color);
        //        graphics.DrawImage(pixel, x, y);
        //    }
           
        //}

        public void ClearBuffer()
        {
            var g = pictureBox1.CreateGraphics();
            g.Clear(Color.Black);
            g.Dispose();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //var graphicsSize = e.Graphics.ClipBounds;
            //e.Graphics.DrawImage()
            int renderWidth = pictureBox1.Width;
            int renderHeight = pictureBox1.Height;
            //int renderWidth = (int)graphicsSize.Width;
            //int renderHeight = (int)graphicsSize.Height;
            //e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            
            IntPtr hdc = e.Graphics.GetHdc();
           
            for (int y = 0; y < renderHeight; y++)
            {
                for (int x = 0; x < renderWidth; x++)
                {
                    //Color pixelColor = GetPixelColor(x, y);

                    //// NOTE: GDI colors are BGR, not ARGB.
                    //uint colorRef = (uint)((pixelColor.B << 16) | (pixelColor.G << 8) | (pixelColor.R));
                    //GDI.SetPixel(hdc, x, y, colorRef);
                    uint colorRef = GetFakeColor(x, y);
                    GDI.SetPixel(hdc, x, y, colorRef);
                }
            }

            e.Graphics.ReleaseHdc(hdc);
        }

        private uint GetFakeColor(int x, int y)
        {
            var offset = 10;
            if ( (x == offset && y > offset && y < 240-offset) ||
                 (x == 320-offset && y > offset && y < 240-offset) ||
                 (y == offset && x > offset && x < 320-offset) ||
                 (y == 240-offset && x > offset && x < 320-offset))
                return (uint)0;
            else
                return (uint)0xffffff;
        }

        //private void pictureBox1_Paint(object sender, PaintEventArgs e)
        //{
        //    Graphics g = e.Graphics;

        //    // Maximize performance
        //    g.CompositingMode = CompositingMode.SourceOver;
        //    g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
        //    g.CompositingQuality = CompositingQuality.HighSpeed;
        //    g.InterpolationMode = InterpolationMode.NearestNeighbor;
        //    g.SmoothingMode = SmoothingMode.None;

        //    var bitmap = new Bitmap(320,240);
        //    var intPtr = bitmap.GetHbitmap();
        //    GDI.SetPixel(intPtr, 100, 100, 0x000000);
        //    g.DrawImage(Image.FromHbitmap(intPtr), Rectangle.FromLTRB(0,0,319,239));
        //}

        private Color GetPixelColor(int x, int y)
        {
            if (values == null || values.Count == 0) return Color.Violet;

            var currentIndex = x + y * 240;

            if (currentIndex >= values.Count)
                currentIndex = values.Count - 1;

            var actualValue = values[currentIndex];

            if (actualValue <= 0xF)
                return InitPalette[actualValue];
            else
                return Color.Black;
        }

        private readonly List<Color> InitPalette = new List<Color>
        {
            Color.FromArgb(0x00, 0x00, 0x00, 0x00),
            Color.FromArgb(0x00, 0x00, 0x00),
            Color.FromArgb(0x88, 0x88, 0x88),
            Color.FromArgb(0xBF, 0x39, 0x32),
            Color.FromArgb(0xDE, 0x7A, 0xAE),
            Color.FromArgb(0x4C, 0x3D, 0x21),
            Color.FromArgb(0x90, 0x5F, 0x25),
            Color.FromArgb(0xE4, 0x94, 0x52),
            Color.FromArgb(0xEA, 0xD9, 0x79),
            Color.FromArgb(0x53, 0x7A, 0x3B),
            Color.FromArgb(0xAB, 0xD5, 0x4A),
            Color.FromArgb(0x25, 0x2E, 0x38),
            Color.FromArgb(0x00, 0x46, 0x7F),
            Color.FromArgb(0x68, 0xAB, 0xCC),
            Color.FromArgb(0xBC, 0xDE, 0xE4),
            Color.FromArgb(0xFF, 0xFF, 0xFF),
        };
    }
}
