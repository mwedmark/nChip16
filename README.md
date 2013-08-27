nChip16
=======

Chip16 emulator written in C# and for Windows. Visual Studio 2012 project files included. 

NOTE: You need to install .NET Framework 4.5 before running nChip16.

Bmp16ToBin
==========

Takes a 24-bit uncompresssed BMP-picture with only 16 different colors and makes a Chip16 Compatible picture .bin file with indexed colors.

NOTE: You need to install .NET Framework 4.5 before running Bmp16ToBin.

ImageCompression
================

Different classes/methods used to compress .BIN-file created with the above Bmp16ToBin executable.

NOTE: You need to install .NET Framework 4.5 before running ImageCompression.

The complete tool-chain for getting nice pictures on the Chip16 platform is:

- Paint.NET
- PSFilterPdn for using Photoshop filters (http://www.namesuppressed.com/support/plugins-paint-net.shtml)
- XiQuantizer - 30 day test version (http://www.ximagic.com/q_download.html)
- Bmp16ToBin
- ImageCompression

Description of how to use Bmp16ToBon and ImageCompression
=========================================================

- Find some good pictures and load them into Paint.NET. 
- Scale the picture to 320*240 for a full-screen Chip16 picture.
- Do filtering such as Auto-level or maybe Black-and-White by using Adjustments in Paint.NET. 
- Open up XiQuantizer which after a button-press shows the picture inside its own GUI.
- Choose 16 colors to match the Chip16 color palette.
- Try out different algorithms both to choose palette AND to choose dither type and setting.
- Save picture to an uncompressed 24-bit BMP file which then only uses 16 different colors chosen from 24-bit color space
- Send picture as a parameter into Bmp16ToBin which will convert it into a 16 color indexed/palettetized image and save it as a .BIN data file.
- Send .BIN data file as a parameter into ImageCompression which will create a .CCI file that is an exact representation (non-destroyed version) of the .BIN file, but in a RLE compressed format. It will also create a .PAL palette file to be used together with the picture.
- Use the reference Chip16 assembler code to both import and extract the CCI-file+PAL-file directly into the framebuffer of Chip16.
