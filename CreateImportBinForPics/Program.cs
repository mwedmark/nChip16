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
            if(args.Length != 2)
                throw new Exception("Call CreateImportBinForPics with 2 parameters as: CreateImportBinForPics %INPUT_DIRECTORY_PATH% %OUTPUT_ASM_FILEPATH%");
            var inputDirectory = args[0];

            if (string.IsNullOrEmpty(inputDirectory) || !Directory.Exists(inputDirectory))
                throw new Exception(string.Format("Dir:{0} does not exist", inputDirectory));

            // find all files
            var cciFiles = Directory.GetFiles(inputDirectory, "*.cci");
            // create both Pic-directive and Palette-directive seperately so they can
            // be joined later
            var numOfPicsDirective = new StringBuilder();
            var pictureDirectives = new List<string>();
            var paletteDirectives = new List<string>();

            numOfPicsDirective.AppendLine("NumberOfPics:");
            numOfPicsDirective.AppendFormat("db {0}", cciFiles.Length);
            numOfPicsDirective.AppendLine();
            numOfPicsDirective.AppendLine();

            //var picStringDirectives = new List<string>();

            bool isFirst = true;
            foreach (var cciFile in cciFiles)
            {
                pictureDirectives.Add(CreatePictureImportBin(RemoveEndingCci(cciFile)));
                paletteDirectives.Add(CreatePaletteImportBin(RemoveEndingCci(cciFile)));
                //picStringDirectives.Add(
                //    Path.GetFileNameWithoutExtension(
                //    Path.GetFileNameWithoutExtension(cciFile)).ToUpper().Replace("_"," "));
                
                isFirst = false;
            }

            //var longestString = picStringDirectives.Max(ps => ps.Length);
            //for (int i = 0; i < picStringDirectives.Count; i++)
            //    picStringDirectives[i] = string.Format("db \"{0}\"", NormalizeStringLength(picStringDirectives[i], longestString));

            var fullContent = new StringBuilder();

            fullContent.Append(numOfPicsDirective);

            foreach (var pictureDirective in pictureDirectives)
                fullContent.AppendLine(pictureDirective);

            fullContent.AppendLine();

            foreach (var paletteDirective in paletteDirectives)
                fullContent.AppendLine(paletteDirective);

            fullContent.AppendLine();
            fullContent.AppendLine("PictureStrings:");

            //foreach (var picStringDirective in picStringDirectives)
            //    fullContent.AppendLine(picStringDirective);

            fullContent.AppendFormat("; Sum of sizes for all files: {0} bytes\r\n", FullSize);
            fullContent.AppendFormat("; Max size for Chip16 target platform: 65536 bytes (64K)");

            var exportFile = args[1];
            var dirForExport = Path.GetDirectoryName(exportFile);

            if (!string.IsNullOrEmpty(dirForExport))
            {
                if (!Directory.Exists(dirForExport))
                    throw new Exception(string.Format("File:{0} does not exist", exportFile));
            }
            Console.WriteLine("Output written to: {0}", exportFile);
            File.WriteAllText(exportFile, fullContent.ToString());
        }

        private static string NormalizeStringLength(string picString, int longestString)
        {
            string fixedString = picString;
            while(true)
            {
                if(fixedString.Length < longestString)
                    fixedString = fixedString.Insert(0, " ");
                else
                    break;

                if (fixedString.Length < longestString)
                    fixedString = fixedString + " ";
                else
                    break;
            }

            return fixedString;
        }

        private static string CreatePicString(string filename)
        {
            return string.Format("db \"{0}\"", filename);
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
