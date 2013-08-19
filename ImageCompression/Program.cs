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
            // var compressionTests = new CompressionTests(args[0]);
            // compressionTests.TestAll();
            foreach (var arg in args)
            {
                var compress = new ImageCompression();
                compress.PerformNibbleCompression(arg);
            }
        }
    }
}
