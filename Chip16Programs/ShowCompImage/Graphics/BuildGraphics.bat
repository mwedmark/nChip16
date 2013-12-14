rem Make a safe-copy of the original ShowCompImage.asm file
copy /Y ..\ShowCompImage.asm ..\ShowCompImage.asm.bak
Bmp16ToBin.exe /d C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures
ImageCompression.exe /d C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures
CreateImportBinForPics.exe C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures ..\Graphics.asm
copy /Y ..\ShowCompImageBefore.asm+..\Graphics.asm ..\ShowCompImage.asm