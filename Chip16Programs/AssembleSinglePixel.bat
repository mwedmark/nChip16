cd SinglePixel
..\tchip16.exe SinglePixel.asm -o SinglePixel.c16 -m
copy mmap.txt SinglePixel.txt
..\..\Bin2Coe\bin\Release\Bin2Coe.exe SinglePixel.c16
cd ..