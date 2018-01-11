using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nChip16
{
    public enum JoystickMapping { Default, Chip8 };

    public partial class MainForm : Form
    {        
        public JoystickMapping JoystickMapping;
 
        private Form BackupForm = new Form();
        private bool WasPanel2Collapsed = false;

        private MemBitmap memBitmap;
        private Chip16Vm vm = new Chip16Vm();
        private bool Running = false;
        private ushort KeyboardState = 0;
        private string FileDropped;
        //private bool AutoStart = false;
        private bool FullScreen = false;

        public static readonly Color BreakpointColor = Color.Red;
        public static readonly Color ProgramCounterColor = Color.Yellow;
        public static readonly Color BreakpointOnPcColor = Color.Orange;

        private const int PcTag = 16;
        private const int SpTag = 17;
        private const int CTag = 18;
        private const int ZTag = 19;
        private const int NTag = 20;
        private const int OTag = 21;

        private readonly List<RegisterTextBox> Registers = new List<RegisterTextBox>();
        private readonly List<Watch> Watches = new List<Watch>();
        private int endOfProgram = 0;

        public MainForm(string[] args)
        {
            InitializeComponent();
            KeyPreview = true;
            vm.OnPCChange += vm_OnPCChange;

            // show version in window title
            var assemblyInfoReader = new AssemblyInfoHelper(GetType());

            Text = assemblyInfoReader.Title + " v" + assemblyInfoReader.AssemblyVersion;

            // dynamically create all register TextBox's and add them to both Form and Registers array
            for (int i = 0; i < 16;i++ )
                Registers.Add(CreateRegisterTextBox("R" + i.ToString("X1"), new Point(25 + (i/8*70), 70+((i%8)*25)), i));

            // setup tags for SP and PC and for Flag textboxes
            tbPC.Tag = PcTag;
            tbSP.Tag = SpTag;
            tbFlagC.Tag = CTag;
            tbFlagN.Tag = NTag;
            tbFlagO.Tag = OTag;
            tbFlagZ.Tag = ZTag;

            // is a single parameter used? Then this is a Chip16 file to load at startup
            if (args.Length == 2)
                FileDropped = args[1];

            vm.Memory.OnWriteByte += Memory_OnWriteByte;
            vm.Memory.OnWriteWord += Memory_OnWriteWord;
            LoadAndRestart();

            var filterDropDown = PopulateInterpolationDropDown();
            fullScreenFilterToolStripMenuItem.DropDown = filterDropDown;

            defaultMappingToolStripMenuItem.Checked = true;
            JoystickMapping = JoystickMapping.Default;
        }

        private ToolStripDropDown PopulateInterpolationDropDown()
        {
            var dropdown = new ToolStripDropDown();

            dropdown.Items.Add(BuildMenuItem(InterpolationMode.Bicubic/*, Keys.F6*/));
            dropdown.Items.Add(BuildMenuItem(InterpolationMode.Bilinear/*, Keys.F7*/));
            dropdown.Items.Add(BuildMenuItem(InterpolationMode.High/*, Keys.F8*/));
            dropdown.Items.Add(BuildMenuItem(InterpolationMode.HighQualityBicubic/*, Keys.F9*/));
            dropdown.Items.Add(BuildMenuItem(InterpolationMode.HighQualityBilinear/*, Keys.F10*/));
            dropdown.Items.Add(BuildMenuItem(InterpolationMode.Low/*, Keys.F11*/));
            dropdown.Items.Add(BuildMenuItem(InterpolationMode.NearestNeighbor/*,Keys.F12*/));
            

            pbEmuScreen.InterpolationMode = InterpolationMode.Default;
            var nnItem = BuildMenuItem(InterpolationMode.Default /*, Keys.F11*/);
            nnItem.Font = new Font(nnItem.Font,FontStyle.Bold);
            dropdown.Items.Add(nnItem);
            dropdown.ItemClicked += dropdown_ItemClicked;
            return dropdown;
        }

        private static ToolStripMenuItem BuildMenuItem(InterpolationMode filter/*, Keys shortCutKey*/)
        {
            var item = new ToolStripMenuItem(filter.ToString());
            item.Tag = filter;
            //item.Checked = true;
            //item.CheckState = CheckState.Checked;
            //item.ShortcutKeys = shortCutKey;
            //item.ShortcutKeyDisplayString = "bla bla";
            item.ShowShortcutKeys = true;
            return item;

        }
        
        /// <summary>
        /// Changed selected filtering algorithm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dropdown_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var clickedTsmi = (ToolStripMenuItem) e.ClickedItem;
            // remove all checked
            foreach (ToolStripMenuItem item in fullScreenFilterToolStripMenuItem.DropDown.Items)
                item.Font = new Font(item.Font, FontStyle.Regular);

            //clickedTsmi.Checked = true;
            var selectedFiltering = (InterpolationMode)clickedTsmi.Tag;
            clickedTsmi.Font = new Font(clickedTsmi.Font, FontStyle.Bold);
            pbEmuScreen.InterpolationMode = selectedFiltering;
        }

        //maybe we dont need this, but instead can reread all watches 
        // at stop, mem change, Step and these.
        void Memory_OnWriteWord(int address, ushort value)
        {
            /*
            var updatedWatches = Watches.FindAll(w => w.Address == address);

            
            foreach (var updatedWatch in updatedWatches)
            {
                updatedWatch.
            }*/
        }

        void Memory_OnWriteByte(int address, byte value)
        {
            //throw new NotImplementedException();
        }

        private RegisterTextBox CreateRegisterTextBox(string registerName, Point location, int vmRegisterIndex)
        {
            var textBox = new RegisterTextBox();
            var label = new Label();

            textBox.Name = "tb" + registerName;
            textBox.Tag = vmRegisterIndex;
            textBox.Font = new Font("Lucida Console", 8.3F, FontStyle.Regular, GraphicsUnit.Point, (0));
            textBox.Location = location;
            textBox.Size = new Size(40, 19);
            textBox.TabIndex = 41;
            textBox.TabStop = false;
            textBox.Text = "0000";
            textBox.MaxLength = 4;
            textBox.TextAlign = HorizontalAlignment.Center;
            textBox.KeyDown += textBox_KeyDown;  
            label.Size = new Size(20, 19);
            label.Name = "lbl" + registerName;
            label.Text = registerName;
            label.Location = new Point(location.X-20,location.Y+4);

            gbRegisters.Controls.Add(textBox);
            gbRegisters.Controls.Add(label);

            return textBox;
        }

        void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = (KeyHandleTextBox)sender;
            var vmRegisterIndex = (int)textBox.Tag;

            ushort value = ushort.Parse(textBox.Text, NumberStyles.HexNumber);
            
            if(vmRegisterIndex < 16)
                vm.Regs[vmRegisterIndex] = value;
            else
            {
                switch (vmRegisterIndex)
                {
                    case PcTag:
                        vm.PC = value;
                        break;
                    case SpTag:
                        vm.SP = value;
                        break;
                    //case ZTag:
                    //    vm.Flags.Z = ConvertUshortToBool(value);
                    //    break;
                    //case NTag:
                    //    vm.Flags.N = ConvertUshortToBool(value);
                    //    break;
                    //case OTag:
                    //    vm.Flags.O = ConvertUshortToBool(value);
                    //    break;
                    //case CTag:
                    //    vm.Flags.C = ConvertUshortToBool(value);
                    //    break;
                    default:
                        throw new Exception("Invalid tag given in textBox_KeyDown");

                }   
            }
        }

        private bool ConvertUshortToBool(ushort value)
        {
            return value != 0;
        }

        private void vm_OnPCChange(ushort oldValue, ushort newValue)
        {
            if (Running || string.IsNullOrEmpty(tbSource.Text))
                return;

            RemovePcHighlight();
            MarkWithHighlight(tbSource, newValue);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateRegisterFields();
            var cmd = Environment.GetCommandLineArgs();
            var pathInCommandLine = (cmd.Count() == 2);
            try
            {
                memBitmap = new MemBitmap(pbEmuScreen.Width, pbEmuScreen.Height);
                pbEmuScreen.Image = memBitmap.Bitmap;
                pbEmuScreen.DrawToBitmap(memBitmap.Bitmap, new Rectangle(0, 0, 320, 240));
                vm.SetScreen(memBitmap);
                vm.GetKeyboardState += vm_GetKeyboardState;

                if (pathInCommandLine)
                {
                    FileDropped = Environment.GetCommandLineArgs()[1];
                    LoadAndRestart();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateSourceTextBox(uint lastSourceLine)
        {
            if (vm != null && vm.CurrentFileStructure != null)
            {
                var endOfSource = (lastSourceLine+1)*Chip16Vm.InstructionSize;
                vm.RenderOpcodes(tbSource, endOfSource);

                if(vm.CurrentState != RunningState.Running)
                    MarkWithHighlight(tbSource, vm.PC);

                //TODO: make sure the PC in seen on-screen
            }
        }

        public uint vm_GetKeyboardState()
        {
            return KeyboardState;
        }

        private void mainForm_KeyDown(object sender, KeyEventArgs e)
        {
            // is C# application key?
            switch (e.KeyCode)
            {
                case Keys.F1:
                    InvokeOnClick(btnReset, new EventArgs());
                    e.Handled = true;
                    break;
                case Keys.F5:
                    InvokeOnClick(btnRun, new EventArgs());
                    e.Handled = true;
                    break;
                case Keys.F10:
                    InvokeOnClick(btnStep, new EventArgs());
                    e.Handled = true;
                    break;
                case Keys.F11:
                    InvokeOnClick(btnStepInto, new EventArgs());
                    e.Handled = true;
                    break;
            }

            // is Chip16 emulator key?
            var bitValueAddress = GetKeyboardBitValue(e.KeyCode);

            if (bitValueAddress.Item2 != 0) Debug.WriteLine(" pressed.");

            // apply changes to current keyboard state
            KeyboardState = (ushort)(KeyboardState | bitValueAddress.Item2);
        }

        private void mainForm_KeyUp(object sender, KeyEventArgs e)
        {
            var bitValueAddress = GetKeyboardBitValue(e.KeyCode);
            var negatedBitValue = (ushort)(0xFFFF ^ bitValueAddress.Item2);

            if (bitValueAddress.Item2 != 0) Debug.WriteLine(" released.");
            // apply changes to current keyboard state
            KeyboardState = (ushort)(KeyboardState & negatedBitValue);
        }

        ///Controllers:
        ///-------------
        ///Controllers are accessed through memory mapped I/O ports.
        ///Controller 1: FFF0.
        ///Controller 2: FFF2.

        ///Bit[0] - Up
        ///Bit[1] - Down
        ///Bit[2] - Left
        ///Bit[3] - Right
        ///Bit[4] - Select
        ///Bit[5] - Start
        ///Bit[6] - A
        ///Bit[7] - B
        ///Bit[8 - 15] - Unused (Always zero).
        private Tuple<int,ushort> GetKeyboardBitValue(Keys keys)
        {
            int address = 0;
            ushort bitValue = 1;
            int shiftSteps = 0;

            var joyMove = JoystickMapping==JoystickMapping.Default?
                TranslateKeyToChip16JoysticksDefault(keys):TranslateKeyToChip16JoysticksChip8(keys);

            switch (joyMove.JoystickNumber)
            {
                case (0): address = 0xFFF0;
                    break;
                case (1): address = 0xFFF2;
                    break;
                default:
                    throw new Exception("Invalid joystick number");
            }

            switch (joyMove.Change)
            {
                case JoystickMoves.Up:
                    shiftSteps = 0;
                    break;
                case JoystickMoves.Down:
                    shiftSteps = 1;
                    break;
                case JoystickMoves.Left:
                    shiftSteps = 2;
                    break;    
                case JoystickMoves.Right:
                    shiftSteps = 3;
                    break;
                case JoystickMoves.Select:
                    shiftSteps = 4;
                    break;
                case JoystickMoves.Start:
                    shiftSteps = 5;
                    break;
                case JoystickMoves.A:
                    shiftSteps = 6;
                    break;
                case JoystickMoves.B:
                    shiftSteps = 7;
                    break;
                case JoystickMoves.NotMapped:
                    shiftSteps = 0;
                    bitValue = 0;
                    break;
            }

            if (bitValue != 0)
                Debug.Write(string.Format("Key: '{0}' ", joyMove.ToString()));

            bitValue <<= shiftSteps;
            return new Tuple<int, ushort>(address, bitValue);
        }

        private static Joystick TranslateKeyToChip16JoysticksDefault(Keys keys)
        {
             var currentJoy = new Joystick(0);
           
            switch (keys)
            {
                // JOY 0
                case Keys.Up:   //UP
                    currentJoy.Change = JoystickMoves.Up;
                    break;
                case Keys.Down: //DOWN
                    currentJoy.Change = JoystickMoves.Down;
                    break;
                case Keys.Left: //LEFT
                    currentJoy.Change = JoystickMoves.Left;
                    break;
                case Keys.Right: //RIGHT
                    currentJoy.Change = JoystickMoves.Right;
                    break;
                case Keys.A:    //SELECT
                    currentJoy.Change = JoystickMoves.Select;
                    break;
                case Keys.S:    //START
                    currentJoy.Change = JoystickMoves.Start;
                    break;
                case Keys.Z:   //A
                    currentJoy.Change = JoystickMoves.A;
                    break;
                case Keys.X:   //B
                    currentJoy.Change = JoystickMoves.B;
                    break;
                // JOY 1
                case Keys.Y: //UP
                    currentJoy.JoystickNumber = 1;
                    currentJoy.Change = JoystickMoves.Up;
                    break;
                case Keys.H: //DOWN
                    currentJoy.JoystickNumber = 1;
                    currentJoy.Change = JoystickMoves.Down;
                    break;
                case Keys.J: //LEFT
                    currentJoy.JoystickNumber = 1;
                    currentJoy.Change = JoystickMoves.Left;
                    break;
                case Keys.K: //RIGHT
                    currentJoy.JoystickNumber = 1;
                    currentJoy.Change = JoystickMoves.Right;
                    break;
                case Keys.N: //SELECT
                    currentJoy.JoystickNumber = 1;
                    currentJoy.Change = JoystickMoves.Select;
                    break;
                case Keys.M: //START
                    currentJoy.JoystickNumber = 1;
                    currentJoy.Change = JoystickMoves.Start;
                    break;
                case Keys.Oemcomma: //A
                    currentJoy.JoystickNumber = 1;
                    currentJoy.Change = JoystickMoves.A;
                    break;
                case Keys.OemPeriod: //B
                    currentJoy.JoystickNumber = 1;
                    currentJoy.Change = JoystickMoves.B;
                    break;
                default:
                    currentJoy.Change = JoystickMoves.NotMapped;
                    break;
            }

            return currentJoy;
        }

        private static Joystick TranslateKeyToChip16JoysticksChip8(Keys keys)
        {
            var currentJoy = new Joystick(0);
           
            switch (keys)
            {
                // JOY 0
                case Keys.D1:   //UP
                    currentJoy.Change = JoystickMoves.Up;
                    break;
                case Keys.D2: //DOWN
                    currentJoy.Change = JoystickMoves.Down;
                    break;
                case Keys.D3: //LEFT
                    currentJoy.Change = JoystickMoves.Left;
                    break;
                case Keys.D4: //RIGHT
                    currentJoy.Change = JoystickMoves.Right;
                    break;
                case Keys.Q:    //SELECT
                    currentJoy.Change = JoystickMoves.Select;
                    break;
                case Keys.W:    //START
                    currentJoy.Change = JoystickMoves.Start;
                    break;
                case Keys.E:   //A
                    currentJoy.Change = JoystickMoves.A;
                    break;
                case Keys.R:   //B
                    currentJoy.Change = JoystickMoves.B;
                    break;
                // JOY 1
                case Keys.A: //UP
                    currentJoy.JoystickNumber = 1;
                    currentJoy.Change = JoystickMoves.Up;
                    break;
                case Keys.S: //DOWN
                    currentJoy.JoystickNumber = 1;
                    currentJoy.Change = JoystickMoves.Down;
                    break;
                case Keys.D: //LEFT
                    currentJoy.JoystickNumber = 1;
                    currentJoy.Change = JoystickMoves.Left;
                    break;
                case Keys.F: //RIGHT
                    currentJoy.JoystickNumber = 1;
                    currentJoy.Change = JoystickMoves.Right;
                    break;
                case Keys.Z: //SELECT
                    currentJoy.JoystickNumber = 1;
                    currentJoy.Change = JoystickMoves.Select;
                    break;
                case Keys.X: //START
                    currentJoy.JoystickNumber = 1;
                    currentJoy.Change = JoystickMoves.Start;
                    break;
                case Keys.C: //A
                    currentJoy.JoystickNumber = 1;
                    currentJoy.Change = JoystickMoves.A;
                    break;
                case Keys.V: //B
                    currentJoy.JoystickNumber = 1;
                    currentJoy.Change = JoystickMoves.B;
                    break;
                default:
                    currentJoy.Change = JoystickMoves.NotMapped;
                    break;
            }

            return currentJoy;
        }

        private void emuTimer_Tick(object sender, EventArgs e)
        {
            //vm.TimerElapsed = true;

            emuTimer.Enabled = true;
            ExecuteFrame();
           
            if (cbRealtimeRegisterUpdate.Checked)
                UpdateRegisterFields();

            if (cbRealtimeWatches.Checked)
                UpdateWatchValues();
        }

        private void UpdateRegisterFields()
        {
            tbPC.Text = Utils.UShortToHex16BitFormat(vm.PC);
            tbSP.Text = Utils.UShortToHex16BitFormat(vm.SP);
            
            for (int i = 0; i < 16; i++)
            {
                var reg = Registers[i];
                var readValue = vm.Regs[i];
                
                // TODO: Buggy at start-up, inital values aren't updated, so we skip this for now
                // only update register field if value actually changes.
                // Optimization done to be able to change rarely changing registers
                // while updating them in real-time
                //if (reg.LastValue == readValue)
                //    return;

                reg.LastValue = readValue;
                reg.Text = Utils.UShortToHex16BitFormat(readValue);
            }

            tbFlagC.Text = Utils.ConvertBoolToNumber(vm.Flags.C);
            tbFlagN.Text = Utils.ConvertBoolToNumber(vm.Flags.N);
            tbFlagO.Text = Utils.ConvertBoolToNumber(vm.Flags.O);
            tbFlagZ.Text = Utils.ConvertBoolToNumber(vm.Flags.Z);
        }

        private int Chip16Usage = 0;

        private void ExecuteFrame()
        {
            Chip16Usage = vm.ExecuteFrame();

            if (Chip16Usage == -1)
                return;

            // update graphics and sound via buffer pointers
            if(vm.GraphicsEnabled)
                UpdateScreen();

            // check running flag for breakpoint
            if (vm.CurrentState == RunningState.Paused)
            {
                // we've hit a breakpoint, pause, then stop timer and set mode to Paused
                ToggleRunState();
                if (!string.IsNullOrEmpty(vm.ErrorMessage))
                {
                    MessageBox.Show(vm.ErrorMessage, "Chip16 Internal VM error!");
                    vm.ErrorMessage = "";
                }
            }
        }

        //private void ExecuteTimerRound()
        //{
        //    while(true)
        //    { 
        //        Chip16Usage = vm.ExecuteTimerRound();

        //        if (Chip16Usage == -1)
        //            return;

        //        // update graphics and sound via buffer pointers
        //        //if (false)
        //        UpdateScreen();

        //        emuTimer.Enabled = true;
        //    }
        //}

        private void UpdateSlowGraphics()
        {
            tsslChip16Usage.Text =
                string.Format("[{0}/{1}] {2}%", Chip16Usage, Chip16Vm.InstructionsPerFrame,
                              (int)(Chip16Usage * 100.0 / Chip16Vm.InstructionsPerFrame));
            tssDrawFrameTimer.Text = vm.LastFrameTime + " ms";
            if (vm.InstructionsLastFrame != 0)
                tssFps.Text = (60 * Chip16Vm.InstructionsPerFrame / vm.InstructionsLastFrame).ToString();
            else
                tssFps.Text = "---";

            UpdateInstructionCount();
        }

        private void UpdateScreen()
        {
            // first give command to vm to redraw from internal Chip16 frambuffer
            vm.UpdateScreenFromFramebuffer();
            pbEmuScreen.Invalidate();
            pbEmuScreen.Refresh();
        }

        private void UpdateAllControls()
        {
            UpdateScreen();
            UpdateRegisterFields();

            if(tsmiShowSourceListing.Checked)
                UpdateSourceTextBox((uint)endOfProgram); // use romsize as limit for source rendering if 0
            
            hexEdit1.RenewVisibleValues();
            UpdateWatchValues();
            UpdateInstructionCount();
        }

        private void UpdateInstructionCount()
        {
            tsslInstructionCount.Text = vm.InstructionCount.ToString();
        }

        private void UpdateWatchValues()
        {
            foreach (ListViewItem lvWatch in lvWatches.Items)
                UpdateWatchValue(lvWatch);
        }

        /// <summary>
        /// Support for loading and starting a Chip16 binary file when dropped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // only handle a single file drop
            if (files.Count() != 1)
                return;

            FileDropped = files[0];
            LoadAndRestart();
        }

        private void LoadAndRestart()
        {
            var waitForm = new WaitForm();

            try
            {
                //waitForm.ShowDialog(this);
                Cursor = Cursors.WaitCursor;
                // first make full reset of vm
                vm.Reset();
                
                // then load and start program
                if (!string.IsNullOrEmpty(FileDropped))
                    vm.LoadProgram(FileDropped);

                hexEdit1.SetData(vm.Memory.GetInternalMemoryArray(), 0);
                UpdateAllControls();

                // after program has loaded, maybe labels have been updated. 
                // Update watches that use LockTo: Label
                if (vm.UsingLineLabels && (Watches.Count > 0))
                {
                    var lockToLabelWatches = Watches.Where(w => w.LockTo == LockTo.Label);

                    foreach (var watch in lockToLabelWatches)
                    {
                        var watchName = watch.Name;
                        var matchedLabels = vm.Labels.FindAll(l => l.Name == watchName);
                        if (matchedLabels.Count != 1)
                            continue;

                        watch.Address = matchedLabels[0].Address;
                    }

                    // redraw lvWatches
                    RedrawWatches();
                }

                // update rom info
                if (vm.CurrentFileStructure != null)
                {
                    tsslRomName.Text = Path.GetFileName(vm.CurrentFileStructure.Path);
                    tsslInitialPC.Text = "0x" + Utils.UShortToHex16BitFormat(vm.CurrentFileStructure.StartAddress);
                    tsslRomSize.Text = (vm.CurrentFileStructure.RomSize + 16).ToString() + " bytes";
                        // 16 bytes header size
                    tsslSpecVersion.Text = vm.CurrentFileStructure.SpecVersionString;
                }

                if (vm.CurrentFileStructure != null)
                    endOfProgram = (int) (vm.CurrentFileStructure.RomSize/Chip16Vm.InstructionSize);

                if (vm.UsingLineLabels)
                {
                    // if label EndOfProgram is found, use this to further minimize source listing
                    var endOfProgramLabel = vm.Labels.FindAll(l => l.Name == "EndOfProgram");

                    if (endOfProgramLabel.Count == 1)
                        endOfProgram = endOfProgramLabel[0].Address/Chip16Vm.InstructionSize;
                }

                // update starting source code
                if (tsmiShowSourceListing.Checked) //skip all
                    UpdateAllControls();

                if (autostartToolStripMenuItem.Checked)
                {
                    emuTimer.Enabled = true;
                    Running = true;
                    vm.CurrentState = RunningState.Running;
                }

               //TODO: Just added for troubleshooting COAC
                if (vm.Labels == null || vm.Labels.Count == 0 || 
                    !vm.Labels.Exists(l => l.Name == "VREGS"))
                    return;

                var baseRegAddr = vm.Labels.Single(l => l.Name == "VREGS").Address;
                for (int i = 0; i < 16; i++)
                {
                    var currentWatchName = "V" + i.ToString("X1");
                    if (!Watches.Exists(w => w.Name == currentWatchName))
                    {
                        var regWatch = new Watch()
                        {
                            Address = (ushort)(baseRegAddr + (i * 2)),
                            LockTo = LockTo.Address,
                            Name = currentWatchName,
                            ShowAs = ShowAs.Hexadecimal,
                            Type = WatchType.Byte
                        };
                        Watches.Add(regWatch);
                    }
                    RedrawWatches();
                }


            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //waitForm.Close();
                Cursor = Cursors.Default;
                
            }
        }

        private void RedrawWatches()
        {
            // first remove all watches from lvWatches
            lvWatches.Items.Clear();
            // recreate all watches using Watches list
            foreach (var watch in Watches)
                AddWatchToListView(watch);
        }

        private void AddWatchToListView(Watch watch)
        {
            var lvItem = new ListViewItem { Tag = watch };
            lvItem.SubItems.Add("Address");
            lvItem.SubItems.Add("Value");
            UpdateListViewItemWithWatch(lvItem);
            lvWatches.Items.Add(lvItem);
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Link;
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            ToggleRunState();
        }

        private Task ExecutingTask;

        private void ToggleRunState()
        {
            if (!Running)
            {
                slowTimer.Enabled = true;
                RemovePcHighlight();
                btnRun.Text = "Stop (F5)";
                Running = true;
                emuTimer.Enabled = true;

                if (vm.WaitForVBlank)
                    vm.CurrentState = RunningState.Started;

                //if (ExecutingTask == null)
                //{
                //    ExecutingTask = Task.Run(() => Chip16Vm.ExecuteThread(vm));
                //}
            }
            else
            {
                slowTimer.Enabled = false;
                tsslChip16Usage.Text = "-%";
                btnRun.Text = "Run  (F5)";
                Running = false;
                emuTimer.Enabled = false;
                vm.CurrentState = RunningState.Paused;
                UpdateAllControls();
            }
        }

        private void btnStepInto_Click(object sender, EventArgs e)
        {
            emuTimer.Enabled = false;
            Running = true;
            ToggleRunState();

            vm.ExecuteInstruction();
            UpdateAllControls();
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            emuTimer.Enabled = false;
            Running = true;
            ToggleRunState();

            Running = false; // to disable graphics update while performing StepOver
            vm.ExecuteStepOver();
            UpdateAllControls();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            //emuTimer.Enabled = false;
            //Running = false;
            Running = true;
            ToggleRunState();

            //if(!string.IsNullOrEmpty(FileDropped))
            LoadAndRestart();
        }

        private void tbSP_Enter(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///  Toggle breakpoint on current line
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSource_DoubleClick(object sender, EventArgs e)
        {
            // first calculate which line is active
            var selectedLine = tbSource.GetLineFromCharIndex(tbSource.SelectionStart);
            var address = (ushort)(selectedLine*Chip16Vm.InstructionSize);
            // Then toggle breakpoint
            var newState = vm.ToggleBreakpoint(address);
            var backColor = tbSource.BackColor;

            // Update graphics for specific line
            if (newState)
                backColor = BreakpointColor;

            if (address == vm.PC)
                backColor = newState?BreakpointOnPcColor:ProgramCounterColor;

            SetBackgroundColorOfLine(selectedLine, backColor);
            tbSource.Select(tbSource.GetFirstCharIndexFromLine(selectedLine),0);
        }

        public void MarkWithHighlight(RichTextBox rtbSource, ushort address)
        {
            var currentLine = address / 4;

            if (!rtbSource.Lines.Any() || (currentLine > rtbSource.Lines.Count()))
                return;

            var startIndex = rtbSource.GetFirstCharIndexFromLine(currentLine);
            var length = rtbSource.Lines[currentLine].Length;

            rtbSource.Focus();

            // Mark current PC
            rtbSource.Select(startIndex, length);

            var backColor = ProgramCounterColor;

            if (vm.BreakpointAtCurrentPc())
                backColor = BreakpointOnPcColor;

            rtbSource.SelectionBackColor = backColor;
            //rtbSource.ScrollToCaret();  
            //TODO: Fix so that this only scrolls if target is off-screen. 
            ScrollToMakeLineVisible(rtbSource, currentLine);
            rtbSource.Select(startIndex, 0); 
        }

        /// <summary>
        /// Found at http://www.codeproject.com/Articles/12152/Numbering-lines-of-RichTextBox-in-NET-2-0
        /// </summary>
        /// <param name="textBox">The textbox to scroll</param>
        /// <param name="line">The line to make sure it is visible</param>
        private void ScrollToMakeLineVisible(RichTextBox textBox, int scrollToLine)
        {
            //we get index of first visible char and 
            //number of first visible line
            var pos = new Point(0, 0);
            int firstIndex = textBox.GetCharIndexFromPosition(pos);
            int firstLine = textBox.GetLineFromCharIndex(firstIndex);

            //now we get index of last visible char 
            //and number of last visible line
            pos.X = ClientRectangle.Width;
            pos.Y = ClientRectangle.Height;
            int lastIndex = textBox.GetCharIndexFromPosition(pos);
            int lastLine = textBox.GetLineFromCharIndex(lastIndex);

            if ((scrollToLine >= firstLine) && (scrollToLine <= lastLine))
                return; // line already visible

            // try to set scrollToLine in the middle of the screen
            var visibleLineSpan = lastLine - firstLine;
            
            //MessageBox.Show("Line not visible");
        }

        private void RemovePcHighlight()
        {
            var currentLine = vm.PC / Chip16Vm.InstructionSize;

            // do not remove high-light from lines missing
            if (!tbSource.Lines.Any() || (currentLine > tbSource.Lines.Count()))
                return;

            var startIndex = tbSource.GetFirstCharIndexFromLine(currentLine);
            var length = tbSource.Lines[currentLine].Length;

            // unmark current PC
            tbSource.Select(startIndex, length);
            var backColor = tbSource.BackColor;

            if (vm.BreakpointAtCurrentPc())
                backColor = BreakpointColor;

            tbSource.SelectionBackColor = backColor;
            tbSource.Select(0,0);
        }

        private void SetBackgroundColorOfLine(int lineIndex, Color backgroundColor)
        {
            if (lineIndex > tbSource.Lines.Count())
                return;

            var startIndex = tbSource.GetFirstCharIndexFromLine(lineIndex);
            var length = tbSource.Lines[lineIndex].Length;

            // Mark current line with given color
            tbSource.Select(startIndex, length);
            tbSource.SelectionBackColor = backgroundColor;
        }

        void cmsiSetLastSource_Click(object sender, System.EventArgs e)
        {
            var currentLine = tbSource.GetLineFromCharIndex(tbSource.SelectionStart);
            endOfProgram = currentLine;
            UpdateSourceTextBox((uint)endOfProgram);

            //select the last line
            tbSource.Select(tbSource.GetFirstCharIndexFromLine(currentLine),0);
            // and move there because thats were we where before..
            tbSource.ScrollToCaret();
        }

        private void tbSource_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // move selection to current mouse positon
                var mousePostionInSource = tbSource.PointToClient(Cursor.Position);
                var lineUnderMouseCursor = tbSource.GetLineFromCharIndex(tbSource.GetCharIndexFromPosition(mousePostionInSource));
                var firstChar = tbSource.GetFirstCharIndexFromLine(lineUnderMouseCursor);
                tbSource.Select(firstChar,0);
                cmsSetLastSource.Show(tbSource, mousePostionInSource);
            }
        }

        private void tbSource_MouseMove(object sender, MouseEventArgs e)
        {
            //var mousePostionInSource = tbSource.PointToClient(Cursor.Position);
            //var lineUnderMouseCursor = tbSource.GetLineFromCharIndex(tbSource.GetCharIndexFromPosition(mousePostionInSource));
            //tsslCurrentLine.Text = lineUnderMouseCursor.ToString();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void pbEmuScreen_Click(object sender, EventArgs e)
        {
            if (!FullScreen)
            {
                // back-up current state
                BackupForm.FormBorderStyle = FormBorderStyle;
                BackupForm.WindowState = WindowState;
                BackupForm.BackColor = BackColor;
                BackupForm.Location = pbEmuScreen.Location;
                BackupForm.Size = pbEmuScreen.Size;

                // set new state
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
                
                gbRegisters.Visible = false;
                gbControls.Visible = false;
                gbWatches.Visible = false;
                gbFlags.Visible = false;
                gbMemory.Visible = false;
                gbSource.Visible = false;
                WasPanel2Collapsed = splitContainer1.Panel2Collapsed;
                splitContainer1.Panel2Collapsed = true;

                statusStrip1.Visible = false;
                mainMenu.Visible = false;

                BackColor = Color.Black;

                // size bitmap to keep same ratio but full-screen
                var screenSize = Screen.PrimaryScreen.Bounds;

                var ratio = screenSize.Height/240.0;
                var newX = 320.0 * ratio;
                var newY = 240.0 * ratio;
                pbEmuScreen.Location = new Point((int)(screenSize.Width-newX)/2, 0);
                pbEmuScreen.Size = new Size((int)newX, (int)newY);

                FullScreen = true;
            }
            else
            {
                // recover back-up state
                FormBorderStyle = BackupForm.FormBorderStyle;
                WindowState = BackupForm.WindowState;
                BackColor = BackupForm.BackColor;
                pbEmuScreen.Location = BackupForm.Location;
                pbEmuScreen.Size =  BackupForm.Size;
                BackColor = BackupForm.BackColor;

                gbRegisters.Visible = true;
                gbControls.Visible = true;
                gbWatches.Visible = true;
                statusStrip1.Visible = true;
                gbFlags.Visible = true;
                gbMemory.Visible = true;
                gbSource.Visible = true;
                mainMenu.Visible = true;
                splitContainer1.Panel2Collapsed = WasPanel2Collapsed;

                FullScreen = false;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Chip16 executable (*.C16)|*.C16";
            var result = fileDialog.ShowDialog(this);

            if (result != DialogResult.OK)
                return;

            // File has been selected, load it!
            FileDropped = fileDialog.FileName;
            LoadAndRestart();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        
        private void lvWatches_MouseDown(object sender, MouseEventArgs e)
        {
            
            //lvWatches.SelectedIndices.Add(lvWatches.Inde(e.X, e.Y));
            Point mousePosition = lvWatches.PointToClient(Control.MousePosition);
            ListViewHitTestInfo hit = lvWatches.HitTest(mousePosition);
            if(hit.Item != null)
            {
                var indexUnderCursor = hit.Item.Index;
                lvWatches.SelectedIndices.Add(indexUnderCursor);
            }

            bool hasSelected = lvWatches.SelectedItems.Count != 0;

            cmsWatchesMenu.Items[1].Enabled = hasSelected;
            cmsWatchesMenu.Items[2].Enabled = hasSelected;
        }
        

        private void addWatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var watchForm = new WatchForm();

            if(vm.Labels.Count != 0)
                watchForm.InitializeLabels(vm.Labels);

            watchForm.StartPosition = FormStartPosition.CenterParent;
            watchForm.Text = "Create Watch";
            var result =  watchForm.ShowDialog(lvWatches);

            if (result != DialogResult.OK)
                return;

            // add a new watch
            Watches.Add(watchForm.Watch);   
            AddWatchToListView(watchForm.Watch);

            UpdateWatchValues();
        }

        private void UpdateListViewItemWithWatch(ListViewItem lvi)
        {
            var watch = (Watch)lvi.Tag;
            lvi.SubItems[0].Text = watch.Name;
            lvi.SubItems[1].Text = Utils.UShortToHex16BitFormat(watch.Address);
             
            UpdateWatchValue(lvi);
        }

        private string FormatWatchValue(ushort value, ShowAs showAs)
        {
            switch (showAs)
            {
                    case ShowAs.Hexadecimal:
                    return Utils.UShortToHex16BitFormat(value);
                    case ShowAs.Decimal:
                    return value.ToString();
                    case ShowAs.Binary:
                    return Utils.FormatBinary(value);
                    break;
                default:
                    throw new Exception("Unknown format specified for watch");
            }
        }

        private string FormatWatchValue(byte value, ShowAs showAs)
        {
            switch (showAs)
            {
                case ShowAs.Hexadecimal:
                    return value.ToString("X2");
                case ShowAs.Decimal:
                    return value.ToString();
                case ShowAs.Binary:
                    return Utils.FormatBinary(value);
                default:
                    throw new Exception("Unknown format specified for watch");
            }
        }

        private void UpdateWatchValue(ListViewItem listItem)
        {
            var watch = (Watch) listItem.Tag;
            if (watch.Type == WatchType.Byte)
                listItem.SubItems[2].Text = FormatWatchValue(vm.Memory.ReadByte(watch.Address),watch.ShowAs);
            else
                listItem.SubItems[2].Text = FormatWatchValue(vm.Memory.ReadWord(watch.Address), watch.ShowAs);
        }

        private void editWatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get the selected Watch
            var lvi = lvWatches.SelectedItems[0];
            var selectedWatch = (Watch)lvi.Tag;
            var watchForm = new WatchForm(selectedWatch);
            if (vm.Labels.Count != 0)
                watchForm.InitializeLabels(vm.Labels);

            watchForm.StartPosition = FormStartPosition.CenterParent;
            watchForm.Text = "Edit Watch";
            var result = watchForm.ShowDialog(lvWatches);

            if (result != DialogResult.OK)
                return;

            UpdateListViewItemWithWatch(lvi);
        }

        private void deleteWatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedWatch();
        }

        private void DeleteSelectedWatch()
        {
            if (lvWatches.SelectedItems.Count == 0)
                return;

            // get the selected Watch
            var lvi = lvWatches.SelectedItems[0];
            var selectedWatch = (Watch)lvi.Tag;

            // delete both lvi and watch
            Watches.Remove(selectedWatch);
            lvWatches.SelectedItems[0].Remove();
        }

        private void lvWatches_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                DeleteSelectedWatch();
        }

        private void hexEdit1_OnDataChanged(object sender, MW.HexEdit.DataChangedEventArgs argsDataChanged)
        {
            UpdateWatchValues();
        }

        private void tmsiWaitForVBLANK_Click(object sender, EventArgs e)
        {
            vm.WaitForVBlank = !vm.WaitForVBlank;
        }

        private void slowTimer_Tick(object sender, EventArgs e)
        {
            UpdateSlowGraphics();
        }

        /// <summary>
        /// When Checkbox is enabled, the source listing is shown and used when debugging.
        /// Is shows breakpoints and full assembler listing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showSourceListingToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !tsmiShowSourceListing.Checked;
        }

        private void resetInstructionCounterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            vm.ResetInstructionCount();
            UpdateInstructionCount();
        }

        private void tbFlagC_DoubleClick(object sender, EventArgs e)
        {
            var currentTag = (int)((TextBox) sender).Tag;

            switch (currentTag)
            {
                case CTag:
                    vm.Flags.C = !vm.Flags.C;
                    break;
                case NTag:
                    vm.Flags.N = !vm.Flags.N;
                    break;
                case OTag:
                    vm.Flags.O = !vm.Flags.O;
                    break;
                case ZTag:
                    vm.Flags.Z = !vm.Flags.Z;
                    break;
                default:
                    throw new Exception("Unknown tbFlag Control connected to Double-click");
            }
            UpdateRegisterFields();
            ActiveControl = null;
        }

        private void defaultMappingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JoystickMapping = JoystickMapping.Default;
            chip8MappingToolStripMenuItem.Checked = false;
            defaultMappingToolStripMenuItem.Checked = true;
        }

        private void chip8MappingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JoystickMapping = JoystickMapping.Chip8;
            chip8MappingToolStripMenuItem.Checked = true;
            defaultMappingToolStripMenuItem.Checked = false;
        }

        private void disableGraphicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            vm.GraphicsEnabled = !vm.GraphicsEnabled;
        }
    }
}
