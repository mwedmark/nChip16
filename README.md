nChip16
=======

Chip16 emulator written in C# and for Windows. Visual Studio 2012 project files included

Features:
- Drag'n'Drop loading of Chip16 ROM files
- Command-line loading by supplying path to Chip16 ROM as parameter
- Breakpoints
- Full real-time Watches support
- Full real-time register/flags viewing
- Advanced memory hex editor included
- Fullscreen mode!! Just click the Chip16 output screen!
- Full source listing including labels from assembler program (if mmap.txt found)
- STEP-OVER / STEP-INTO
- Performace good thanks to using GDI+ low-level handling of pixels from C#/.NET
- Most ROM's working good


Missing for now:
- No sound
- Bug with palette changes not updating already written pixels
- No joystick control
