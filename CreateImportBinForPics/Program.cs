using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateImportBinForPics
{
    
    class Program
    {
        private static long FullSize = 0;
        /// <summary>
        /// All cci files located in the current directory will be added
        /// as importbin directive both for cci-file and PAL-file.
        /// If a parameter is given, this shall be the fullPath to the
        /// target file which will be overwritten.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // find all files
            var cciFiles = Directory.GetFiles(Directory.GetCurrentDirectory(),"*.cci") ;
            // create both Pic-directive and Palette-directive seperately so they can
            // be joined later
            var pictureDirectives = new List<string>();
            var paletteDirectives = new List<string>();

            bool isFirst = true;
            foreach (var cciFile in cciFiles)
            {
                pictureDirectives.Add(CreatePictureImportBin(RemoveEndingCci(cciFile)));
                paletteDirectives.Add(CreatePaletteImportBin(RemoveEndingCci(cciFile)));
                isFirst = false;
            }

            var fullContent = new StringBuilder();
            foreach (var pictureDirective in pictureDirectives)
                fullContent.AppendLine(pictureDirective);

            fullContent.AppendLine();

            foreach (var paletteDirective in paletteDirectives)
                fullContent.AppendLine(paletteDirective);

            fullContent.AppendFormat("; Sum of sizes for all files: {0} bytes\r\n", FullSize);
            fullContent.AppendFormat("; Max size for Chip16 target platform: 65536 bytes (64K)");

            var exportFile = "";
            if(args.Length == 0)
                Console.WriteLine(fullContent.ToString());
            else
            {
                Console.WriteLine("Output written to: {0}", args[0]);
                File.WriteAllText(args[0],fullContent.ToString());
            }
        }

        private static string RemoveEndingCci(string fullPath)
        {
            return fullPath.Substring(0, fullPath.Length - 3);
        }

        // CreatePictureImportBin
        // importbin MarioWorld.bmp.cci 0 6277 Picture
        private static string CreatePictureImportBin(string initialPath)
        {
            var fullLine = new StringBuilder();
            fullLine.Append(BuildImportBinStructure(initialPath + "cci", CreatePictureLabelName()));
            return fullLine.ToString();
        }

        private static int PicCount = -1;
        private static string CreatePictureLabelName()
        {
            PicCount++;
            if (PicCount == 0) return "Picture";

            return "PIC" + PicCount.ToString();
        }

        private static int PalCount = -1;
        private static string CreatePaletteLabelName()
        {
            PalCount++;
            if (PalCount == 0) return "Palette";

            return "PAL" + PalCount.ToString();
        }
        
        // CreatePaletteImportBin
        // importbin MarioWorld.bmp.PAL 0 48 Palette
        private static string CreatePaletteImportBin(string initialPath)
        {
            var fullLine = new StringBuilder();
            fullLine.Append(BuildImportBinStructure(initialPath + "PAL", CreatePaletteLabelName()));
            return fullLine.ToString();
        }

        // importbin MarioWorld.bmp.'extension' 0 48 'labelname'
        private static string BuildImportBinStructure(string filePath, string labelName)
        {
            var size = GetFileSize(filePath);
            FullSize += size;
            return string.Format("importbin {0} 0 {1} {2}", filePath,size , labelName);
        }

        private static long GetFileSize(string fullPath)
        {
            return new FileInfo(fullPath).Length;
        }
    }
}
