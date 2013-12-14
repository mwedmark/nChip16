using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chip16;

namespace Chip16
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileList = ExtractFiles(args);

            foreach (var file in fileList)
            {
                var compress = new ImageCompression();
                compress.PerformNibbleCompression(file);
            }
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

                var pictureFiles = Directory.GetFiles(dirPath, "*.bin");
                fileList.AddRange(pictureFiles);
            }
            else
            {
                fileList.AddRange((pars));
            }

            return fileList;
        }
    }
}
