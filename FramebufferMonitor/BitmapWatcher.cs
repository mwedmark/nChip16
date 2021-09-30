using nChip16;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace FramebufferMonitor
{
    public class BitmapWatcher
    {
        public string FilePath;
        private readonly IEmulateWindow _mainForm;

        public BitmapWatcher(IEmulateWindow mainForm)
        {
            _mainForm = mainForm;
        }

        public void SetupFileListener(string dirName, string filename)
        {
            var watcher = new FileSystemWatcher(dirName);
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = filename;
            watcher.Changed += Watcher_Changed;
            watcher.EnableRaisingEvents = true;

            FilePath = Path.Combine(dirName, filename);
        }

        private string ReadFileAsStringWithRetry()
        {
            while (true)
            {
                try
                {
                    return File.ReadAllText(FilePath);
                }
                catch (Exception)
                {
                    Thread.Sleep(100);
                }
            }
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            (sender as FileSystemWatcher).EnableRaisingEvents = false;
            var fileContentAsString = ReadFileAsStringWithRetry();
            var stringList = fileContentAsString.Split(new[] { " ", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //Console.WriteLine($"File changed! First byte is: {(char)fileContent.First()}");

            _mainForm.ClearBuffer();

            var intList = new List<int>();
            for (int i = 0; i < stringList.Count; i++)
            {
                var readChar = stringList[i];
                var readValue = 0;
                if (readChar != "x")
                    readValue = int.Parse(readChar.ToString(), NumberStyles.HexNumber);

                intList.Add(readValue);
            }

            _mainForm.SetBitmap(intList);

            (sender as FileSystemWatcher).EnableRaisingEvents = true;
        }
    }
}
