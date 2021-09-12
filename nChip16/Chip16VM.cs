using Chip16.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace nChip16
{
    public delegate void ChangedValueHandler(ushort oldValue, ushort newValue);

    public enum RunningState { Started, Running, Paused };

    public class Chip16Vm
    {
        public const byte SpecVersion = ((1 << 4) + 3); //1.3
        public const int InstructionSize = 4;
        public const int InstructionsPerFrame = 1000000/60;
        public bool WaitForVBlank = true;
        private const int InitStackAddress = 0xFDF0; // Start of stack (512 bytes total size, 128 addresses depth).
        private readonly Random rnd = new Random((int)DateTime.Now.Ticks);

        private List<Color> CurrentPalette = new List<Color>();
        private ushort? lastPaletteAddress = null;

        private ushort bgc = 0;
        private FlipMode FlipMode = FlipMode.None;

        public int InstructionCount = 0;
        //public int InstructionsLastFrame = 0;

        public FileStructure CurrentFileStructure;
        public List<LineLabel> Labels = new List<LineLabel>();
        private readonly List<ushort> Breakpoints = new List<ushort>();
 
        public bool UsingLineLabels { get; set; }

        public string ErrorMessage;

        // delegates and events
        public delegate uint dGetKeyboardState();
        public event dGetKeyboardState GetKeyboardState;

        public RunningState CurrentState = RunningState.Paused;

        public Chip16Framebuffer FrameBuffer = new Chip16Framebuffer();
        //1x 16 bit program counter (PC)
        private ushort pc;
        public ushort PC 
        { 
            get { return pc; } 
            set
            {
                if(OnPCChange != null)
                    OnPCChange(pc, value);

                pc = value;
            } 
        }
        public event ChangedValueHandler OnPCChange;
        //1x 16 bit stack pointer (SP)
        public ushort SP { get; set; }
        public bool TimerElapsed { get; internal set; }
        public bool GraphicsEnabled { get; internal set; } = true;

        //16x 16 bit general purpose registers (R0 - RF)
        public ushort [] Regs = new ushort[16];
        
        ///1x 8 bit flag register
        ///Flag register:
        ///---------------
        ///Bit[0] - Reserved
        ///Bit[1] - c (Unsigned Carry and Unsigned Borrow flag)
        ///Bit[2] - z (Zero flag)
        ///Bit[3] - Reserved
        ///Bit[4] - Reserved
        ///Bit[5] - Reserved
        ///Bit[6] - o (Signed Overflow Flag)
        ///Bit[7] - n (Negative Flag; aka Sign Flag)
        ///private byte flags { get; set; }
        public FlagRegister Flags = new FlagRegister();
        public Memory Memory = new Memory();
        public Size SpriteSize = new Size();
        private MemBitmap Screen;

        public long LastFrameTime;
        public long ExecutedFrameTime;
        //Machine uses little-endian byte ordering.
        //All opcodes take exactly 1 cycle to execute.
        //CPU speed is 1 Mhz.
        public Chip16Vm()
        {
            Reset();
        }

        public void SetScreen(MemBitmap memBitmap)
        {
            Screen = memBitmap;
        }

        public bool ToggleBreakpoint(ushort address)
        {
            var foundBreakpoint = Breakpoints.Exists(b => b == address);
            if (foundBreakpoint)
                Breakpoints.Remove(address);
            else
                Breakpoints.Add(address);

            return !foundBreakpoint; // return the new state, false=>Breakpoint removed
        }

        public void ResetInstructionCount()
        {
            InstructionCount = 0;
        }

        public bool BreakpointAtCurrentPc()
        {
            return Breakpoints.Contains(PC);
        }

        public void Reset()
        {
            UsingLineLabels = false;
            Labels.Clear();

            PC = 0;
            SP = InitStackAddress;

            // set all registers to 0
            for(int i=0;i<Regs.Length;i++)
                Regs[i] = 0;

            // set all memory to 0
            Memory.Reset();
            // set all Flags to 0
            Flags.Reset();

            ResetInstructionCount();

            // Graphics Reset
            // ------------------
            // Reset Palette
            CurrentPalette = new List<Color>(InitPalette);
            FlipMode = FlipMode.None;
            bgc = 0;

            ClearScreen();
        }

        internal void LoadProgram(string programPath)
        {
            // start by interpret the complete file
            CurrentFileStructure = InterpretFile(programPath);

            if (Encoding.UTF8.GetString(CurrentFileStructure.MagicNumber) != "CH16")
                throw new Exception("Magic number is incorrect!");

            // load program into address 0 in memory
            for (int index = 0; index < CurrentFileStructure.RomSize; index++)
            {
                var dataByte = CurrentFileStructure.Romdata[index];
                Memory.WriteByte(index, dataByte);
            }

            // CRC32 checksum of ROM (excluding header) (polynomial: 0x04C11DB7)
            //var crc32 = new CRC32(0x04C11DB7); // poly given via docs for Chip16
            //var calculatedChecksum = crc32.ComputeHash(currentFileStructure.Romdata);

            //if (calculatedChecksum != currentFileStructure.Checksum)
            {
            // Show Error message
            }

            // set starting PC correct
            PC = CurrentFileStructure.StartAddress;

            // try and find the xxx.txt file in the same directory (where xxx is the program name) and read it
            var mmapFullPath = Path.GetDirectoryName(programPath) + "\\" +
                Path.GetFileNameWithoutExtension(programPath) + ".txt";

            if (File.Exists(mmapFullPath))
            {
                Labels = MMapImport.ImportFile(mmapFullPath);
                UsingLineLabels = true;
            }
        }

        public int InstructionsLastFrame = 0;

        /// <summary>
        /// Skriv specifikt program som fyller skärmen och klocka detta.
        /// Antalet inst kommer att vara samma, men förhoppningsvis kan vi optimera
        /// själva exekveringen.
        /// </summary>
        public void ExecuteFillScreen()
        {
            var sw = new Stopwatch();
            sw.Start();

            ExecutedFrameTime = sw.ElapsedMilliseconds;
        }

        public static void ExecuteThread(Chip16Vm vm)
        {
            while (true) // should never end
            {
                if (vm.WaitForVBlank && (vm.InstructionsLastFrame > InstructionsPerFrame))
                {
                    //break;
                    vm.CurrentState = RunningState.Paused; // current frame elapsed
                }

                if (!vm.WaitForVBlank && vm.TimerElapsed)
                {
                    vm.TimerElapsed = false;
                    vm.CurrentState = RunningState.Paused; // current frame elapsed
                    //break;
                }

                vm.InstructionsLastFrame++;
                //TODO: DO other check to be able to RUN rounds between a single brekpoint
                if ((vm.Breakpoints.Contains(vm.PC) && vm.CurrentState != RunningState.Started))
                {
                    vm.CurrentState = RunningState.Paused;
                    //return 0;
                }
                if (vm.CurrentState == RunningState.Paused)
                {
                    Thread.Sleep(100);
                    // check error code for crash
                    if (!string.IsNullOrEmpty(vm.ErrorMessage))
                    {
                        //return 0;
                    }
                }
                try
                {
                    if (vm.CurrentState == RunningState.Started) // just started. State used to skip initial breakpoint
                        vm.CurrentState = RunningState.Running;

                    if (vm.ExecuteInstruction())
                    {
                        var temp = vm.InstructionsLastFrame;
                        //InstructionsPerVBlank = 0;
                        // Found VBLNK, present num of Chip16 instructions as usage
                        //return temp; // found infinitive loop, break current frame and wait for next frame
                    }
                    vm.InstructionCount++; // only count instructions that isn't an infintive loop
                }
                catch (Exception e)
                {
                    //MessageBox.Show(e.Message);
                    //return -1;
                    throw;
                    //return 0;
                }
            }
        } 

        /// <summary>
        /// Execute 1'000'000/60 instructions to get correct timing of Chip16 machine
        /// </summary>
        public int ExecuteFrame()
        {
            InstructionsLastFrame = 0;
            var sw = new Stopwatch();
            sw.Start();

            UpdateKeyboardState();

            // update palette from memory once for each frame
            if(lastPaletteAddress != null)
                SetPalette(lastPaletteAddress.GetValueOrDefault());

            while(true)
            {
                if (WaitForVBlank && (InstructionsLastFrame > InstructionsPerFrame))
                    break;

                if (!WaitForVBlank && TimerElapsed)
                {
                    TimerElapsed = false;
                    break;
                }

                InstructionsLastFrame++;
                //TODO: DO other check to be able to RUN rounds between a single brekpoint
                if ((Breakpoints.Contains(PC) && CurrentState != RunningState.Started))
                {
                    CurrentState = RunningState.Paused;
                    return 0;
                }
                if (CurrentState == RunningState.Paused)
                {
                    // check error code for crash
                    if (!string.IsNullOrEmpty(ErrorMessage))
                    {
                        return 0;
                    }
                }
                try
                {
                if(CurrentState == RunningState.Started) // just started. State used to skip initial breakpoint
                    CurrentState = RunningState.Running;
                
                if (ExecuteInstruction())
                {
                    var temp = InstructionsLastFrame;
                    //InstructionsPerVBlank = 0;
                    // Found VBLNK, present num of Chip16 instructions as usage
                    return temp; // found infinitive loop, break current frame and wait for next frame
                }
                InstructionCount++; // only count instructions that isn't an infintive loop
                }
                catch (Exception e)
                {
                    //MessageBox.Show(e.Message);
                    //return -1;
                    //throw;
                    return 0;
                }
            }

            ExecutedFrameTime = sw.ElapsedMilliseconds;
            return InstructionsLastFrame;
        }

        /// <summary>
        /// Simple implementation with no breakpoints or other features
        /// </summary>
        //public int ExecuteTimerRound()
        //{
        //    InstructionsLastFrame = 0;
        //    while(!TimerElapsed)
        //    {
        //        InstructionsLastFrame++;
        //        ExecuteInstruction();
        //    }

        //    TimerElapsed = false;

        //    // timer has elapsed
        //    return InstructionsLastFrame;
        //}

        private void UpdateKeyboardState()
        {
            // map keyboard to Chip16 Joysticks, use the current mapping: default or Chip8
            var keyboardState = GetKeyboardState();
            Memory.WriteByte(0xFFF0, (byte)(keyboardState&0xFF));

            //TODO: Debug to always press A
            //Memory.WriteByte(0xFFF0, (byte)(0x40)); // press A
        }

        public bool ExecuteInstruction()
        {
            try
            {
                // start by creating the opcode from machineCode
                var opcode = new Opcode();
                var machineCode = FetchCurrentMachineCode();
                opcode.InterpretCode(machineCode);
                return ExecuteCode(opcode);
            }
            catch (Exception e)
            {
                /*
                MessageBox.Show(
                    string.Format("Error in instruction: xxxx - {0}", e.Message), "CHIP16 ERROR", 
                    MessageBoxButtons.OK,MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);*/
                // here we would like to set the machine in Pause and stop on trouble instruction..
                ErrorMessage = e.Message;
                CurrentState = RunningState.Paused;
                //throw new Exception(
                //    string.Format("Error in instruction: xxxx - {0}", e.Message));
                //MessageBox.Show("Error in instruction: xxxx - {0}", e.Message);
                return false;
            }
        }

        public void ExecuteStepOver()
        {
            var nextPc = PC + 4;

            var firstOpcode = new Opcode();
            var firstMachineCode = FetchCurrentMachineCode();
            firstOpcode.InterpretCode(firstMachineCode);
            ExecuteCode(firstOpcode);

            // if not a CALL instruction, stop after executing this 
            if (!firstOpcode.IsCallInstruction())
                return;

            //if current instruction is a CALL, execute until returning from call
            while(true)
            {
                var opcode = new Opcode();
                var machineCode = FetchCurrentMachineCode();
                opcode.InterpretCode(machineCode);
                ExecuteCode(opcode);

                if (PC == nextPc)
                    break;
            }
        }

        private void ClearScreen()
        {
            if (Screen == null || Screen.Bitmap == null)
                return;
            
            FrameBuffer.ClearBuffer(bgc);
        }

        private bool ExecuteCode(Opcode opcode)
        {
            int temp;
            int rem;
            bool executedJumpInstruction = false;
            switch (opcode.RawOpcode[0])
            {
                case 0x00: // NOP
                    break;
                case 0x01: // CLS
                    bgc = 0;
                    ClearScreen();
                    //bgc = 0;
                    break;
                case 0x02: // VBLNK
                    PC += 4;
                    return true;
                case 0x03: // BGC N
                    bgc = opcode.N;
                    break;
                case 0x04: // SPR HHLL
                    SpriteSize.Width = (ushort)(opcode.LL<<1);
                    SpriteSize.Height = opcode.HH;
                    break;
                case 0x05: // DRW RX, RY, HHLL
                    var spriteAddressImm = opcode.HHLL;
                    Flags.C = false;
                    for (int y = 0; y < SpriteSize.Height; y++) // 2 pixels/byte
                    {
                        for (int x = 0; x < SpriteSize.Width; x += 2) // 2 pixels/byte
                        {
                            byte colorData = Memory.ReadByte(spriteAddressImm++);
                            var colorIndex1 = (byte)((colorData & 0xF0) >> 4);
                            var colorIndex2 = (byte)(colorData & 0x0F);

                            var yCoord = 0;
                            if (FlipMode == FlipMode.None)
                                yCoord = (short)Regs[opcode.Y] + y;
                            else if (FlipMode == FlipMode.VFlip || FlipMode == FlipMode.VFlipHFlip)
                                yCoord = (short)Regs[opcode.Y] + SpriteSize.Height - y;

                            if (yCoord < 0 || yCoord > 239)
                                continue;

                            if (colorIndex1 != 0)
                            {
                                var color1 = ColorLookup(colorIndex1);
                                //var xCoord = Regs[opcode.X] + x;
                                var xCoord = 0;
                                if (FlipMode == FlipMode.None)
                                    xCoord = (short)Regs[opcode.X] + x;
                                else if (FlipMode == FlipMode.HFlip || FlipMode == FlipMode.VFlipHFlip)
                                    xCoord = (short)Regs[opcode.X] + SpriteSize.Width - x;


                                if (xCoord < 0 || xCoord > 319)
                                    continue;

                                //if (!IsBackgroundColor(Screen.GetPixel(xCoord, yCoord)))
                                if (!IsBackgroundIndex(FrameBuffer.GetPixel(xCoord, yCoord)))
                                    Flags.C = true;

                                //Screen.SetPixel(xCoord, yCoord, color1.A, color1.R, color1.G, color1.B);
                                FrameBuffer.SetPixel(xCoord,yCoord,colorIndex1);
                            }
                            else // writing index0, which never covers others pixels, for now do nothing
                            {
                                
                            }

                            if (colorIndex2 != 0)
                            {
                                var color2 = ColorLookup(colorIndex2);
                                
                                //var xCoord = Regs[opcode.X] + x + 1;
                                var xCoord = 0;
                                if (FlipMode == FlipMode.None)
                                    xCoord = (short)Regs[opcode.X] + x + 1;
                                else if (FlipMode == FlipMode.HFlip || FlipMode == FlipMode.VFlipHFlip)
                                    xCoord = (short)Regs[opcode.X] + SpriteSize.Width - (x + 1);

                                if (xCoord < 0 || xCoord > 319)
                                    continue;

                                //if (!IsBackgroundColor(Screen.GetPixel(xCoord, yCoord)))
                                if (!IsBackgroundIndex(FrameBuffer.GetPixel(xCoord, yCoord)))
                                    Flags.C = true;

                                //Screen.SetPixel(xCoord, yCoord, color2.A, color2.R, color2.G, color2.B);
                                FrameBuffer.SetPixel(xCoord, yCoord, colorIndex2);
                            }
                            else // writing index0, which never covers others pixels, for now do nothing
                            {

                            }
                        }
                    }
                    break;
                case 0x06: // DRW Rx,Ry,Rz
                    var spriteAddressZ = Regs[opcode.Z];
                    for (int y = 0; y != SpriteSize.Height; y++)
                    {
                        for (int x = 0; x != SpriteSize.Width; x += 2) // 2 pixels/byte
                        {
                            byte colorData = Memory.ReadByte(spriteAddressZ++);

                            var leftRightPixel = new byte[2];
                            var leftPixel = (byte) ((colorData & 0xF0) >> 4);
                            var rightPixel = (byte) (colorData & 0x0F);
                            leftRightPixel[0] = leftPixel;
                            leftRightPixel[1] = rightPixel;

                            var yCoord = 0;
                            if (FlipMode == FlipMode.None || FlipMode == FlipMode.HFlip)
                                yCoord = (short)(Regs[opcode.Y]) + y;
                            else if (FlipMode == FlipMode.VFlip || FlipMode == FlipMode.VFlipHFlip)
                                yCoord = (short)(Regs[opcode.Y]) - y + SpriteSize.Height;
                            
                            if (yCoord < 0 || yCoord > 239)
                                continue;

                            for (var i = 0; i < 2;i++)
                            {
                                var pixel = leftRightPixel[i];

                                if (pixel != 0)
                                {
                                    var xCoord = 0;
                                    if (FlipMode == FlipMode.None || FlipMode == FlipMode.VFlip)
                                        xCoord = (short)(Regs[opcode.X]) + (x + i);
                                    else if (FlipMode == FlipMode.HFlip || FlipMode == FlipMode.VFlipHFlip)
                                        xCoord = (short)(Regs[opcode.X]) - (x + i) + SpriteSize.Width;

                                    if (xCoord < 0 || xCoord > 319)
                                        continue;
                                    var color1 = ColorLookup(pixel);
                                    //if (!IsBackgroundColor(Screen.GetPixel(xCoord, yCoord)))
                                    if (!IsBackgroundIndex(FrameBuffer.GetPixel(xCoord, yCoord))) 
                                            Flags.C = true;

                                    //Screen.SetPixel(xCoord, yCoord, color1.A, color1.R, color1.G, color1.B);
                                    FrameBuffer.SetPixel(xCoord, yCoord, pixel);
                                }
                            }
                        }
                    }
                    break;
                case 0x07: // RND HHLL
                    Regs[opcode.X] = (ushort)(rnd.Next(opcode.HHLL+1));
                    break;
                case 0x08: // FLIP
                    switch (opcode.RawOpcode[3])
                    {
                        case 0x0: FlipMode = FlipMode.None; break;
                        case 0x1: FlipMode = FlipMode.VFlip; break;
                        case 0x2: FlipMode = FlipMode.HFlip; break;
                        case 0x3: FlipMode = FlipMode.VFlipHFlip; break;
                        default: throw new Exception("Unknown FlipMode");
                    }
                    break;
                case 0x0a: // SND1 HHLL (500hz)
                    break;
                case 0x0b: // SND2 HHLL (1000hz)
                    break;
                case 0x0c: // SND3 HHLL (1500hz)
                    break;
                case 0x0d: // SNP RX, HHLL
                    break;
                case 0x0E: // SNG AD, VTSR
                    // no implementation
                    break;
                case 0x10:  // JUMP HHLL
                    if (PC == opcode.HHLL)
                        return true;

                    PC = opcode.HHLL;
                    executedJumpInstruction = true;
                    break;
                case 0x11: // obsolete opcode JC HHLL (f.e. CollisionTest)
                    if (Flags.C)
                    {
                        PC = opcode.HHLL;
                        executedJumpInstruction = true;
                    }
                    break;
                case 0x12: //Jx HHLL
                    if (CheckBranchConditions(opcode.BranchBits))
                    {
                        PC = opcode.HHLL;
                        executedJumpInstruction = true;
                    }
                    break;
                case 0x13: // JME RX, RY, HHLL
                    if (Regs[opcode.X] == Regs[opcode.Y])
                    {
                        PC = opcode.HHLL;
                        executedJumpInstruction = true;
                    }
                    break;
                case 0x14: // CALL HHLL
                    Memory.WriteWord(SP, PC);
                    SP += 2;
                    PC = opcode.HHLL;
                    executedJumpInstruction = true;
                    break;
                case 0x15: // RET
                    SP -= 2;
                    PC = Memory.ReadWord(SP);
                    break;
                case 0x17: // Cx HHLL
                    if (CheckBranchConditions(opcode.BranchBits))
                    {
                        Memory.WriteWord(SP, PC);
                        SP += 2;
                        PC = opcode.HHLL;
                        executedJumpInstruction = true;
                    }
                    break;
                case 0x18: // CALL Rx
                    Memory.WriteWord(SP, PC);
                    SP += 2;
                    PC = Regs[opcode.X];
                    executedJumpInstruction = true;
                    break;
                case 0x20:  // Rx = HHLL
                    Regs[opcode.X] = opcode.HHLL; 
                    break;
                case 0x21:  // SP = HHLL
                    SP = opcode.HHLL;
                    break;
                case 0x22: // Rx = mem[HHLL]
                    Regs[opcode.X] = Memory.ReadWord(opcode.HHLL);
                    break;
                case 0x23: // Rx = mem[Ry]
                    Regs[opcode.X] = Memory.ReadWord(Regs[opcode.Y]);
                    break;
                case 0x24: // Rx = Ry
                    Regs[opcode.X] = Regs[opcode.Y];
                    break;
                case 0x30: // mem[HHLL] = Rx 
                    Memory.WriteWord(opcode.HHLL, Regs[opcode.X]);
                    break;
                case 0x31: // mem[Ry] = Rx
                    Memory.WriteWord( Regs[opcode.Y], Regs[opcode.X]);
                    break;
                case 0x40: // Rx = Rx + HHLL 
                    temp = Regs[opcode.X] + opcode.HHLL;
                    Flags.UpdateFlagsAdd(Regs[opcode.X], opcode.HHLL, temp);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0x41: // Rx = Rx + Ry
                    temp = Regs[opcode.X] + Regs[opcode.Y];
                    Flags.UpdateFlagsAdd(Regs[opcode.X], Regs[opcode.Y], temp);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0x42: // Rz = Rx + Ry
                    temp = Regs[opcode.X] + Regs[opcode.Y];
                    Flags.UpdateFlagsAdd(Regs[opcode.X], Regs[opcode.Y], temp);
                    Regs[opcode.Z] = (ushort)temp;
                    break;
                case 0x50: // SUBI Rx,HHLL => Rx = Rx - HHLL
                    temp = Regs[opcode.X] - opcode.HHLL;
                    Flags.UpdateFlagsSub(Regs[opcode.X], opcode.HHLL, temp);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0x51: // Rx = Rx - Ry
                    temp = Regs[opcode.X] - Regs[opcode.Y];
                    Flags.UpdateFlagsSub(Regs[opcode.X], Regs[opcode.Y], temp);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0x52: // Rz = Rx - Ry
                    temp = Regs[opcode.X] - Regs[opcode.Y];
                    Flags.UpdateFlagsSub(Regs[opcode.X], Regs[opcode.Y], temp);
                    Regs[opcode.Z] = (ushort)temp;
                    break;
                case 0x53: // CMPI Rx, HHLL 
                    temp = Regs[opcode.X] - opcode.HHLL;
                    Flags.UpdateFlagsSub(Regs[opcode.X], opcode.HHLL, temp);
                    break;
                case 0x54: // CMPI Rx, Ry // DEBUGGED ABOVE, CONTINUE
                    temp = Regs[opcode.X] - Regs[opcode.Y];
                    Flags.UpdateFlagsSub(Regs[opcode.X], Regs[opcode.Y], temp);
                    break;
                case 0x60: // Rx = Rx & HHLL
                    temp = Regs[opcode.X] & opcode.HHLL;
                    Flags.UpdateFlagsLogic(temp);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0x61: // Rx = Rx & Ry
                    Regs[opcode.X] = (ushort)(Regs[opcode.X] & Regs[opcode.Y]);
                    Flags.UpdateFlagsLogic(Regs[opcode.X]);
                    break;
                case 0x62: // Rz = Rx & Ry
                    temp = Regs[opcode.X] & Regs[opcode.Y];
                    Flags.UpdateFlagsLogic(temp);
                    Regs[opcode.Z] = (ushort)temp;
                    break;
                case 0x63: // Rx & HHLL
                    temp = Regs[opcode.X] & opcode.HHLL;
                    Flags.UpdateFlagsLogic(temp);
                    break;
                case 0x64: // Rx & Ry
                    temp = Regs[opcode.X] & Regs[opcode.Y];
                    Flags.UpdateFlagsLogic(temp);
                    break;
                case 0x70: // Rx = Rx | HHLL
                    temp = Regs[opcode.X] | opcode.HHLL;
                    Flags.UpdateFlagsLogic(temp);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0x71:  // Rx = Rx | Ry
                    temp = (ushort)(Regs[opcode.X] | Regs[opcode.Y]);
                    Flags.UpdateFlags(temp);
                    Regs[opcode.X] = (ushort) temp;
                    break;
                case 0x72:  // Rz = Rx | Ry
                    temp = (ushort)(Regs[opcode.X] | Regs[opcode.Y]);
                    Flags.UpdateFlagsLogic(temp);
                    Regs[opcode.Z] = (ushort) temp;
                    break;
                case 0x80: // Rx = Rx ^ HHLL
                    temp = Regs[opcode.X] ^ opcode.HHLL;
                    Flags.UpdateFlagsLogic(temp);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0x81:  // Rx = Rx ^ Ry
                    Regs[opcode.X] = (ushort)(Regs[opcode.X] ^ Regs[opcode.Y]);
                    Flags.UpdateFlagsLogic(Regs[opcode.X]);
                    break;
                case 0x82:  // Rz = Rx ^ Ry
                    temp = (ushort)(Regs[opcode.X] ^ Regs[opcode.Y]);
                    Flags.UpdateFlagsLogic(temp);
                    Regs[opcode.Z] = (ushort) temp;
                    break;
                case 0x90: // Rx = Rx * HHLL
                    temp = Regs[opcode.X] * opcode.HHLL;
                    Flags.UpdateFlagsMul(temp);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0x91: // Rx = Rx * Ry
                    temp = Regs[opcode.X] * Regs[opcode.Y];
                    Flags.UpdateFlagsMul(temp);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0x92: // Rz = Rx * Ry
                    temp = Regs[opcode.X] * Regs[opcode.Y];
                    Flags.UpdateFlagsMul(temp);
                    Regs[opcode.Z] = (ushort)temp;
                    break;
                case 0xA0: // Rx = Rx / HHLL
                    //temp = Regs[opcode.X] / opcode.HHLL;
                    Math.DivRem(Regs[opcode.X], opcode.HHLL, out rem);
                    temp = (short) Regs[opcode.X]/opcode.HHLL;
                    Flags.UpdateFlagsDiv(temp, rem);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0xA1: // Rx = Rx / Ry
                    //temp = Regs[opcode.X]/Regs[opcode.Y];
                    Math.DivRem(Regs[opcode.X], Regs[opcode.Y], out rem);
                    temp = (short)Regs[opcode.X] / Regs[opcode.Y];
                    Flags.UpdateFlagsDiv(temp, rem);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0xA2: // Rz = Rx / Ry
                    //temp = Regs[opcode.X] / Regs[opcode.Y];
                    Math.DivRem(Regs[opcode.X], Regs[opcode.Y], out rem);
                    temp = (short)Regs[opcode.X] / Regs[opcode.Y];
                    Flags.UpdateFlagsDiv(temp, rem);
                    Regs[opcode.Z] = (ushort)temp;
                    break;
                case 0xA3: // Rx = Rx mod HHLL
                    temp = PerformShortMod((short)Regs[opcode.X], (short)opcode.HHLL);
                    Flags.UpdateFlagsModRem(temp);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0xA4: // Rx = Rx mod Ry
                    temp = PerformShortMod((short)Regs[opcode.X], (short)Regs[opcode.Y]);
                    Flags.UpdateFlagsModRem(temp);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0xA5: // Rz = Rx mod Ry
                    temp = PerformShortMod((short)Regs[opcode.X], (short)Regs[opcode.Y]);
                    Flags.UpdateFlagsModRem(temp);
                    Regs[opcode.Z] = (ushort)temp;
                    break;
                case 0xA6: // Rx = Rx rem HHLL
                    temp = Math.DivRem((short)Regs[opcode.X], (short)opcode.HHLL, out rem);
                    Flags.UpdateFlagsModRem(rem);
                    Regs[opcode.X] = (ushort)rem;
                    break;
                case 0xA7: // Rx = Rx rem Ry
                    temp = Math.DivRem((short)Regs[opcode.X], (short)Regs[opcode.Y], out rem);
                    Flags.UpdateFlagsModRem(rem);
                    Regs[opcode.X] = (ushort)rem;
                    break;
                case 0xA8: // Rz = Rx rem Ry
                    temp = Math.DivRem((short)Regs[opcode.X], (short)Regs[opcode.Y], out rem);
                    Flags.UpdateFlagsModRem(rem);
                    Regs[opcode.Z] = (ushort)rem;
                    break;
                case 0xB0: // Rx = SHL N
                    temp = Regs[opcode.X] << opcode.N;
                    Regs[opcode.X] = (ushort)temp;
                    Flags.UpdateFlags(temp);
                    break;
                case 0xB1: // Rx = SHR N
                    temp = Regs[opcode.X] >> opcode.N;
                    Regs[opcode.X] = (ushort)temp;
                    Flags.UpdateFlagsLogic(temp);
                    break;
                case 0xB2: // Rx = Rx >> N, copying leading bit
                    temp = (short)Regs[opcode.X] >> opcode.N;
                    //Flags.UpdateFlagsSar(temp, Regs[opcode.X]);
                    Flags.UpdateFlagsLogic(temp);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0xb3: // Rx = Rx << Ry
                    temp = Regs[opcode.X] << Regs[opcode.Y];
                    Flags.UpdateFlagsLogic(temp);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0xb4: // Rx = Rx >> Ry
                    temp = Regs[opcode.X] >> Regs[opcode.Y];
                    Flags.UpdateFlagsLogic(temp);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0xb5: // Rx = Rx >> Ry, copying leading bit
                    temp = (short)Regs[opcode.X] >> Regs[opcode.Y];
                    //Flags.UpdateFlagsSar(temp, Regs[opcode.X]);
                    Flags.UpdateFlagsLogic(temp);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0xc0: // PUSH Rx 
                    Memory.WriteWord(SP, Regs[opcode.X]);
                    SP += 2; // stack grows forward
                    break;
                case 0xC1: // POP Rx
                    SP -= 2;
                    Regs[opcode.X] = Memory.ReadWord(SP);
                    break;
                case 0xC2: // PUSH ALL
                    for (int r = 0; r < 16; r++)
                    {
                        Memory.WriteWord(SP, Regs[r]);
                        SP += 2;
                    }
                    break;
                case 0xc3:
                    for (int r = 15; r >= 0; r--)
                    {
                        SP -= 2;
                        Regs[r] = Memory.ReadWord(SP);
                    }
                    break;
                case 0xc4: // PUSHF
                    Memory.WriteWord(SP, Flags.BitValue);
                    SP += 2; // stack grows forward
                    break;
                case 0xc5: // POPF
                    SP -= 2;
                    Flags.BitValue = Memory.ReadWord(SP);
                    break;
                case 0xd0: // PAL HHLL
                    SetPalette(opcode.HHLL);
                    break;
                case 0xd1: // PAL Rx
                    SetPalette(Regs[opcode.X]);
                    break;
                case 0xe0: // RX = NOT HHLL
                    temp = ~opcode.HHLL;
                    Flags.UpdateFlagsLogic(temp);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0xe1: // RX = NOT RX
                    temp = ~Regs[opcode.X];
                    Flags.UpdateFlagsLogic(temp);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0xe2: // RX = NOT RY
                    temp = ~Regs[opcode.Y];
                    Flags.UpdateFlagsLogic(temp);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0xe3: // RX = NEG HHLL
                    temp = 0 - opcode.HHLL;
                    Flags.UpdateFlagsLogic(temp);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0xe4: // RX = NEG RX
                    temp = 0 - Regs[opcode.X];
                    Flags.UpdateFlagsLogic(temp);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                case 0xe5: // RX = NEG RY
                    temp = 0 - Regs[opcode.Y];
                    Flags.UpdateFlagsLogic(temp);
                    Regs[opcode.X] = (ushort)temp;
                    break;
                default:
                    throw new Exception(
                        string.Format("invalid opcode: {0}\r\nat address: {1}",
                        RenderOpcode(PC), RenderCurrentAddressInfo(PC)));
            }

            if (!executedJumpInstruction)
                PC += 4;

            return false;
        }

        private int PerformShortMod(short par1, short par2)
        {
            //return Math.Abs((par1%par2 + par1)%par1);
            //return Math.Abs(par1 % par2); //Mod should ALWAYS return positive
            return par1 - (int)Math.Floor((double)par1 / par2) * par2;
        }

        public void UpdateScreenFromFramebuffer()
        {
            var sw = new Stopwatch();
            sw.Start();
            if(lastPaletteAddress != null)
                SetPalette(lastPaletteAddress.GetValueOrDefault());
           
            if (Screen == null)
                return;

            for (int y = 0; y < Chip16Framebuffer.Ymax; y++)
            {
                for (int x = 0; x < Chip16Framebuffer.Xmax; x++)
                {
                    var colorIndex = FrameBuffer.GetPixel(x, y);
                    var color = ColorLookup(colorIndex);

                    Screen.SetPixel(x,y,color.A,color.R,color.G,color.B);
                }
            }
            LastFrameTime = sw.ElapsedMilliseconds;
            //Debug.WriteLine(msLastFrame); 
        }

        private void SetPalette(ushort HHLL)
        {
            lastPaletteAddress = HHLL;
            for (int ci = 0; ci < 16; ci++)
            {
                int R = Memory.ReadByte(lastPaletteAddress.GetValueOrDefault() + (3 * ci));
                int G = Memory.ReadByte(lastPaletteAddress.GetValueOrDefault() + 1 + (3 * ci));
                int B = Memory.ReadByte(lastPaletteAddress.GetValueOrDefault() + 2 + (3 * ci));
                CurrentPalette[ci] = Color.FromArgb(R, G, B);
            }
            
            //ClearScreen(); // TODO: temporary fix to be able to use index0 in images correctly
        }

        private bool IsBackgroundIndex(ushort index)
        {
            return bgc == index;
        }

        private bool IsBackgroundColor(Color color)
        {
            var backgroundColor = ColorLookup((byte)bgc);
            //return color == backgroundColor;
            if (Math.Abs(color.R - backgroundColor.R) > 2)
                return false;

            if (Math.Abs(color.G - backgroundColor.G) > 2)
                return false;

            if (Math.Abs(color.B - backgroundColor.B) > 2)
                return false;

            return true;
        }

        //Color indexes (0xRRGGBB):
        //------------------------
        //0x0 - 0x000000 (Black, Transparent in foreground layer)
        //0x1 - 0x000000 (Black)
        //0x2 - 0x888888 (Gray)
        //0x3 - 0xBF3932 (Red)
        //0x4 - 0xDE7AAE (Pink)
        //0x5 - 0x4C3D21 (Dark brown)
        //0x6 - 0x905F25 (Brown)
        //0x7 - 0xE49452 (Orange)
        //0x8 - 0xEAD979 (Yellow)
        //0x9 - 0x537A3B (Green)
        //0xA - 0xABD54A (Light green)
        //0xB - 0x252E38 (Dark blue)
        //0xC - 0x00467F (Blue)
        //0xD - 0x68ABCC (Light blue)
        //0xE - 0xBCDEE4 (Sky blue)
        //0xF - 0xFFFFFF (White)
        private Color ColorLookup(byte colorIndex)
        {
            return CurrentPalette[colorIndex];
        }

        private readonly List<Color> InitPalette = new List<Color> 
        {
            Color.FromArgb(0x00, 0x00, 0x00, 0x00),
            Color.FromArgb(0x00, 0x00, 0x00),
            Color.FromArgb(0x88, 0x88, 0x88),
            Color.FromArgb(0xBF, 0x39, 0x32),
            Color.FromArgb(0xDE, 0x7A, 0xAE),
            Color.FromArgb(0x4C, 0x3D, 0x21),
            Color.FromArgb(0x90, 0x5F, 0x25),
            Color.FromArgb(0xE4, 0x94, 0x52),
            Color.FromArgb(0xEA, 0xD9, 0x79),
            Color.FromArgb(0x53, 0x7A, 0x3B),
            Color.FromArgb(0xAB, 0xD5, 0x4A),
            Color.FromArgb(0x25, 0x2E, 0x38),
            Color.FromArgb(0x00, 0x46, 0x7F),
            Color.FromArgb(0x68, 0xAB, 0xCC),
            Color.FromArgb(0xBC, 0xDE, 0xE4),
            Color.FromArgb(0xFF, 0xFF, 0xFF),
        };

        private Color GetBackgroundColor()
        {
            return Color.FromArgb(0x00000000);   
        }

        private bool CheckBranchConditions(Opcode.BB branchBits)
        {
            switch (branchBits)
            {
                case Opcode.BB.Z:  return Flags.Z;
                case Opcode.BB.NZ: return !Flags.Z;
                case Opcode.BB.N: return Flags.N;
                case Opcode.BB.NN: return !Flags.N;
                case Opcode.BB.P: return (!Flags.N && !Flags.Z);
                case Opcode.BB.O: return Flags.O;
                case Opcode.BB.NO: return !Flags.O;
                case Opcode.BB.A: return (!Flags.C && !Flags.Z);
                case Opcode.BB.AE: return !Flags.C;
                case Opcode.BB.B: return Flags.C;
                case Opcode.BB.BE: return (Flags.C || Flags.Z);
                case Opcode.BB.G: return ((Flags.O == Flags.N) || !Flags.Z);
                case Opcode.BB.GE: return (Flags.O == Flags.N);
                case Opcode.BB.L: return (Flags.O != Flags.N);
                case Opcode.BB.LE: return ((Flags.O != Flags.N) || Flags.Z);
                case Opcode.BB.RES: throw new Exception("reserved branch condition");
                default: throw new Exception("unknown branch combination");
            }
        }

        private byte[] FetchCurrentMachineCode()
        {
            var machineCode = new byte[4];

            machineCode[0] = Memory.ReadByte(PC);
            machineCode[1] = Memory.ReadByte(PC+1);
            machineCode[2] = Memory.ReadByte(PC+2);
            machineCode[3] = Memory.ReadByte(PC+3);

            return machineCode;
        }

        private byte[] FetchMachineCode(int address)
        {
            var machineCode = new byte[4];

            machineCode[0] = Memory.ReadByte(address);
            machineCode[1] = Memory.ReadByte(address + 1);
            machineCode[2] = Memory.ReadByte(address + 2);
            machineCode[3] = Memory.ReadByte(address + 3);

            return machineCode;
        }

        ///ROMs are stored either in a Chip16 ROM file (.c16), or a raw binary file (preferred .bin, also .c16).
        ///The ROM file stores a 16-byte header, and the binary data after that.
        ///Emulators should load ROM headers for internal use, and only use the binary data for emulation.
        ///Binary data is always loaded at address 0x0000.

        ///The header is as follows (0x00 - 0x0F) :
        ///- 0x00: Magic number 'CH16'
        ///- 0x04: Reserved
        ///- 0x05: Spec version (high nibble=major, low nibble=minor, so 0.7 = 0x07 and 1.0 = 0x10)
        ///- 0x06: Rom size (excl. header, in bytes)
        ///- 0x0A: Start address (initial value of PC)
        ///- 0x0C: CRC32 checksum (excl. header, polynom = 0x04c11db7)
        ///- 0x10: Start of Chip16 raw rom, end of header
        private FileStructure InterpretFile(string filePath)
        {
            var fs = new FileStructure();
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            fs.Path = filePath;

            try
            {
                stream.Read(fs.MagicNumber, 0, 4);
                fs.Reserved = (byte)stream.ReadByte();
                fs.SpecVersion = (byte)stream.ReadByte();
                if(!DoesEmulatorHandleSpecVersion(fs.SpecVersion))
                    throw new Exception(
                        string.Format("nChip16 handles {0} but ROM uses {1}.. Halting load!",SpecVersionAsString(SpecVersion),SpecVersionAsString(fs.SpecVersion)));

                const int soRomSize = 4;
                var romSize = new byte[soRomSize];
                stream.Read(romSize, 0, soRomSize);
                fs.RomSize = ConvertBytesToDWord(romSize);

                const int soStartAddress = 2;
                var startAddress = new byte[soStartAddress];
                stream.Read(startAddress, 0, soStartAddress);
                fs.StartAddress = ConvertBytesToWord(startAddress);

                const int soChecksum = 4;
                var checksum = new byte[soChecksum];
                stream.Read(checksum, 0, soChecksum);
                fs.Checksum = ConvertBytesToDWord(checksum);

                fs.Romdata = new byte[fs.RomSize];
                stream.Read(fs.Romdata, 0, (int)fs.RomSize);

                var dummyRead = stream.Read(new byte[10], 0,10);
                if(dummyRead != 0)
                    throw new Exception("Error in fileformat!");
            }
            finally
            {
                stream.Close();
            }

            return fs;
        }

        private string SpecVersionAsString(byte specByte)
        {
            return (((specByte & 0xF0) >> 4)).ToString() + "." + ((int) (specByte & 0x0F)).ToString();
        }

        private bool DoesEmulatorHandleSpecVersion(byte specVersion)
        {
            return (specVersion <= SpecVersion);
        }

        private uint ConvertBytesToDWord(byte[] checksum)
        {
            return (uint)(checksum[0] + (checksum[1] << 8) + (checksum[2] << 16) + (checksum[3] << 24));
        }

        private ushort ConvertBytesToWord(byte[] romSize)
        {
            return (ushort)((ushort)(romSize[1] << 8) + romSize[0]);
        }

        public Opcode RenderOpcode(int address)
        {
            var machineCode = FetchMachineCode(address);
            var opcode = new Opcode();
            opcode.InterpretCode(machineCode);

            return opcode;
        }

        public void RenderOpcodes(RichTextBox tbSource, uint romSize)
        {
            tbSource.Hide();
            tbSource.Clear();
            var startAddress = 0;
            
            if(CurrentFileStructure != null)
                startAddress = CurrentFileStructure.StartAddress;

            for (int i = 0; i < romSize; i += InstructionSize)
            {
                var currentAddress = startAddress + i;
                var currentLine = i/InstructionSize;

                var opcode = new Opcode();
                var machineCode = FetchMachineCode(currentAddress);

                opcode.InterpretCode(machineCode);

                if (Labels.Exists(l => l.Address == opcode.HHLL))
                    opcode.Label = Labels.Single(l => l.Address == opcode.HHLL).Name;

                var currentAddressText = RenderCurrentAddressInfo(currentAddress);
                tbSource.AppendText(currentAddressText);

                // only use LightGrey for addresses when using Labels from tchip assembler mmap.txt file
                if(UsingLineLabels && char.IsDigit(currentAddressText[0]))
                {    
                    var startIndexOfAddressText = tbSource.GetFirstCharIndexOfCurrentLine();
                    tbSource.Select(startIndexOfAddressText, currentAddressText.Length);
                    tbSource.SelectionColor = Color.LightGray;
                    tbSource.DeselectAll();
                }

                // select only opcode
                var opcodeText = opcode.ToString();
                tbSource.AppendText(opcodeText);
                var startIndexOfOpcode = tbSource.GetFirstCharIndexOfCurrentLine() + currentAddressText.Length;
                tbSource.Select(startIndexOfOpcode, opcodeText.Length);

                tbSource.SelectionColor = char.IsDigit(opcodeText[0]) ? Color.LightGray : tbSource.ForeColor;

                // make full line select
                var startIndexOfLine = tbSource.GetFirstCharIndexOfCurrentLine();
                tbSource.Select(startIndexOfLine, tbSource.Lines[currentLine].Length);
                // check for breakpoint, draw if found
                if (Breakpoints.Contains((ushort)i))
                    tbSource.SelectionBackColor = MainForm.BreakpointColor;

                tbSource.AppendText("\r\n");
            }
            tbSource.Show();
        }

        private string RenderCurrentAddressInfo(int currentAddress)
        {
            string addressInfo;

            var maxLength = 5;

            if (UsingLineLabels && Labels.Exists(l => l.Address == currentAddress))
                addressInfo = Labels.Single(l => l.Address == currentAddress).Name + ":";
            else
                addressInfo = Utils.UShortToHex16BitFormat((ushort)currentAddress);

            if(UsingLineLabels)
                maxLength = Labels.Max(l => l.Name.Length) + 1; // add 1 for :

            while (addressInfo.Length != maxLength)
                addressInfo += " ";

            return addressInfo;
        }

        internal bool BreakpointAt(ushort address)
        {
            return Breakpoints.Contains(address);
        }
    }
}
