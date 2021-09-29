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

            filePath = Path.Combine(args[0], filename);

            var watcher = new FileSystemWatcher(args[0]);
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = filename;
            watcher.Changed += Watcher_Changed;
            watcher.EnableRaisingEvents = true;

            mainForm = new MainForm();
            mainForm.ShowDialog();
        }

        static string filePath;

        private static string ReadFileAsString()
        {
            while (true)
            {
                try
                {
                    return File.ReadAllText(filePath);
                }
                catch (Exception)
                {
                    Thread.Sleep(100);
                }
            }
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            (sender as FileSystemWatcher).EnableRaisingEvents = false;
            var fileContentAsString = ReadFileAsString();
            var stringList = fileContentAsString.Split(new[] { " ", "\r\n" },StringSplitOptions.RemoveEmptyEntries).ToList();
            //Console.WriteLine($"File changed! First byte is: {(char)fileContent.First()}");

            mainForm.ClearBuffer();

            var intList = new List<int>();
            //var bitmap = new Bitmap(320, 240);
            for (int i=0;i< stringList.Count;i++)
            {
                var readChar = stringList[i];
                var readValue = 0;
                if(readChar != "x")
                    readValue = int.Parse(readChar.ToString(), NumberStyles.HexNumber);

                intList.Add(readValue);
                //bitmap.SetPixel(i % 320, i / 320, Color.FromArgb(readValue * 16, readValue * 16, readValue * 16));
                //mainForm.SetPixel(i%320, i/320, Color.FromArgb(readValue*16, readValue*16, readValue*16));
            }

            mainForm.SetBitmap(intList);

            (sender as FileSystemWatcher).EnableRaisingEvents = true;
        }
    }
}
