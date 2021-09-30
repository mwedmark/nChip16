using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FramebufferMonitor
{
    class Program
    {
        const string filename = "testFrameBuffer.dump";
        static MainForm mainForm;

        static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.WriteLine("Program only works with a single supplied parameter which is a filepath");
                return;
            }

            if(!Directory.Exists(args[0]))
            {
                Console.WriteLine($"Not a valid file path: {args[0]}");
                return;
            }

            var filePath = Path.Combine(args[0], filename);
            mainForm = new MainForm();
            var bitmapWatcher = new BitmapWatcher(mainForm);
            bitmapWatcher.SetupFileListener(args[0], filename);

            mainForm.ShowDialog();
        }
    }
}
