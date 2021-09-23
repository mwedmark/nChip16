using System.Windows.Forms;

namespace nChip16
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.emuTimer = new System.Windows.Forms.Timer(this.components);
            this.cmsSetLastSource = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmsiSetLastSource = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslRomName = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssLabelRomSize = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslRomSize = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel7 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslInitialPC = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslSpecVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssLabelChip16Usage = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslChip16Usage = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssLabelInstructionCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslInstructionCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssLabelDrawFrameTimer = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssDrawFrameTimer = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssLabelFps = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssFps = new System.Windows.Forms.ToolStripStatusLabel();
            this.cmsWatchesMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addWatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editWatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteWatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dumpMemoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tmsiWaitForVBLANK = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowSourceListing = new System.Windows.Forms.ToolStripMenuItem();
            this.autostartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullScreenFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetInstructionCounterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.joystickMappingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultMappingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chip8MappingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableGraphicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slowTimer = new System.Windows.Forms.Timer(this.components);
            this.gbControls = new System.Windows.Forms.GroupBox();
            this.btnStep = new nChip16.KeyHandleButton();
            this.btnStepInto = new nChip16.KeyHandleButton();
            this.btnRun = new nChip16.KeyHandleButton();
            this.btnReset = new nChip16.KeyHandleButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gbMemory = new System.Windows.Forms.GroupBox();
            this.hexEdit1 = new MW.HexEdit.HexEdit();
            this.pbEmuScreen = new nChip16.PictureBoxExtended();
            this.gbRegisters = new System.Windows.Forms.GroupBox();
            this.cbRealtimeRegisterUpdate = new System.Windows.Forms.CheckBox();
            this.tbSP = new nChip16.KeyHandleTextBox();
            this.lblI = new System.Windows.Forms.Label();
            this.tbPC = new nChip16.KeyHandleTextBox();
            this.lblPC = new System.Windows.Forms.Label();
            this.gbFlags = new System.Windows.Forms.GroupBox();
            this.tbFlagN = new nChip16.KeyHandleTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbFlagO = new nChip16.KeyHandleTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbFlagZ = new nChip16.KeyHandleTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbFlagC = new nChip16.KeyHandleTextBox();
            this.gbWatches = new System.Windows.Forms.GroupBox();
            this.cbRealtimeWatches = new System.Windows.Forms.CheckBox();
            this.lvWatches = new System.Windows.Forms.ListView();
            this.chName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chAddress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbSource = new System.Windows.Forms.GroupBox();
            this.tbSource = new System.Windows.Forms.RichTextBox();
            this.cmsSetLastSource.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.cmsWatchesMenu.SuspendLayout();
            this.mainMenu.SuspendLayout();
            this.gbControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gbMemory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbEmuScreen)).BeginInit();
            this.gbRegisters.SuspendLayout();
            this.gbFlags.SuspendLayout();
            this.gbWatches.SuspendLayout();
            this.gbSource.SuspendLayout();
            this.SuspendLayout();
            // 
            // emuTimer
            // 
            this.emuTimer.Interval = 15;
            this.emuTimer.Tick += new System.EventHandler(this.emuTimer_Tick);
            // 
            // cmsSetLastSource
            // 
            this.cmsSetLastSource.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.cmsSetLastSource.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmsiSetLastSource});
            this.cmsSetLastSource.Name = "contextMenuStrip1";
            this.cmsSetLastSource.Size = new System.Drawing.Size(164, 26);
            // 
            // cmsiSetLastSource
            // 
            this.cmsiSetLastSource.Name = "cmsiSetLastSource";
            this.cmsiSetLastSource.Size = new System.Drawing.Size(163, 22);
            this.cmsiSetLastSource.Text = "Set as last source";
            this.cmsiSetLastSource.Click += new System.EventHandler(this.cmsiSetLastSource_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.tsslRomName,
            this.tssLabelRomSize,
            this.tsslRomSize,
            this.toolStripStatusLabel7,
            this.tsslInitialPC,
            this.toolStripStatusLabel5,
            this.tsslSpecVersion,
            this.tssLabelChip16Usage,
            this.tsslChip16Usage,
            this.tssLabelInstructionCount,
            this.tsslInstructionCount,
            this.tssLabelDrawFrameTimer,
            this.tssDrawFrameTimer,
            this.tssLabelFps,
            this.tssFps});
            this.statusStrip1.Location = new System.Drawing.Point(0, 612);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1188, 22);
            this.statusStrip1.TabIndex = 20;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(43, 17);
            this.toolStripStatusLabel1.Text = "Name:";
            this.toolStripStatusLabel1.Click += new System.EventHandler(this.toolStripStatusLabel1_Click);
            // 
            // tsslRomName
            // 
            this.tsslRomName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tsslRomName.Name = "tsslRomName";
            this.tsslRomName.Size = new System.Drawing.Size(32, 17);
            this.tsslRomName.Text = "-----";
            // 
            // tssLabelRomSize
            // 
            this.tssLabelRomSize.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tssLabelRomSize.Name = "tssLabelRomSize";
            this.tssLabelRomSize.Size = new System.Drawing.Size(33, 17);
            this.tssLabelRomSize.Text = "Size:";
            // 
            // tsslRomSize
            // 
            this.tsslRomSize.Name = "tsslRomSize";
            this.tsslRomSize.Size = new System.Drawing.Size(22, 17);
            this.tsslRomSize.Text = "---";
            // 
            // toolStripStatusLabel7
            // 
            this.toolStripStatusLabel7.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripStatusLabel7.Name = "toolStripStatusLabel7";
            this.toolStripStatusLabel7.Size = new System.Drawing.Size(58, 17);
            this.toolStripStatusLabel7.Text = "Initial PC:";
            // 
            // tsslInitialPC
            // 
            this.tsslInitialPC.Name = "tsslInitialPC";
            this.tsslInitialPC.Size = new System.Drawing.Size(43, 17);
            this.tsslInitialPC.Text = "0x0000";
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(81, 17);
            this.toolStripStatusLabel5.Text = "Spec Version:";
            // 
            // tsslSpecVersion
            // 
            this.tsslSpecVersion.Name = "tsslSpecVersion";
            this.tsslSpecVersion.Size = new System.Drawing.Size(20, 17);
            this.tsslSpecVersion.Text = "-.-";
            // 
            // tssLabelChip16Usage
            // 
            this.tssLabelChip16Usage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tssLabelChip16Usage.Name = "tssLabelChip16Usage";
            this.tssLabelChip16Usage.Size = new System.Drawing.Size(83, 17);
            this.tssLabelChip16Usage.Text = "Chip16 usage:";
            // 
            // tsslChip16Usage
            // 
            this.tsslChip16Usage.Name = "tsslChip16Usage";
            this.tsslChip16Usage.Size = new System.Drawing.Size(22, 17);
            this.tsslChip16Usage.Text = "-%";
            // 
            // tssLabelInstructionCount
            // 
            this.tssLabelInstructionCount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tssLabelInstructionCount.Name = "tssLabelInstructionCount";
            this.tssLabelInstructionCount.Size = new System.Drawing.Size(106, 17);
            this.tssLabelInstructionCount.Text = "Instruction count:";
            // 
            // tsslInstructionCount
            // 
            this.tsslInstructionCount.Name = "tsslInstructionCount";
            this.tsslInstructionCount.Size = new System.Drawing.Size(22, 17);
            this.tsslInstructionCount.Text = "---";
            // 
            // tssLabelDrawFrameTimer
            // 
            this.tssLabelDrawFrameTimer.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tssLabelDrawFrameTimer.Name = "tssLabelDrawFrameTimer";
            this.tssLabelDrawFrameTimer.Size = new System.Drawing.Size(108, 17);
            this.tssLabelDrawFrameTimer.Text = "DrawFrameTimer:";
            // 
            // tssDrawFrameTimer
            // 
            this.tssDrawFrameTimer.Name = "tssDrawFrameTimer";
            this.tssDrawFrameTimer.Size = new System.Drawing.Size(22, 17);
            this.tssDrawFrameTimer.Text = "---";
            // 
            // tssLabelFps
            // 
            this.tssLabelFps.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tssLabelFps.Name = "tssLabelFps";
            this.tssLabelFps.Size = new System.Drawing.Size(30, 17);
            this.tssLabelFps.Text = "FPS:";
            // 
            // tssFps
            // 
            this.tssFps.Name = "tssFps";
            this.tssFps.Size = new System.Drawing.Size(22, 17);
            this.tssFps.Text = "---";
            // 
            // cmsWatchesMenu
            // 
            this.cmsWatchesMenu.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.cmsWatchesMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addWatchToolStripMenuItem,
            this.editWatchToolStripMenuItem,
            this.deleteWatchToolStripMenuItem});
            this.cmsWatchesMenu.Name = "cmsWatchesMenu";
            this.cmsWatchesMenu.Size = new System.Drawing.Size(145, 70);
            // 
            // addWatchToolStripMenuItem
            // 
            this.addWatchToolStripMenuItem.Name = "addWatchToolStripMenuItem";
            this.addWatchToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.addWatchToolStripMenuItem.Text = "Add Watch";
            this.addWatchToolStripMenuItem.Click += new System.EventHandler(this.addWatchToolStripMenuItem_Click);
            // 
            // editWatchToolStripMenuItem
            // 
            this.editWatchToolStripMenuItem.Name = "editWatchToolStripMenuItem";
            this.editWatchToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.editWatchToolStripMenuItem.Text = "Edit Watch";
            this.editWatchToolStripMenuItem.Click += new System.EventHandler(this.editWatchToolStripMenuItem_Click);
            // 
            // deleteWatchToolStripMenuItem
            // 
            this.deleteWatchToolStripMenuItem.Name = "deleteWatchToolStripMenuItem";
            this.deleteWatchToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.deleteWatchToolStripMenuItem.Text = "Delete Watch";
            this.deleteWatchToolStripMenuItem.Click += new System.EventHandler(this.deleteWatchToolStripMenuItem_Click);
            // 
            // mainMenu
            // 
            this.mainMenu.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(1188, 24);
            this.mainMenu.TabIndex = 22;
            this.mainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.dumpMemoryToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Font = new System.Drawing.Font("Lucida Console", 8.3F);
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // dumpMemoryToolStripMenuItem
            // 
            this.dumpMemoryToolStripMenuItem.Enabled = false;
            this.dumpMemoryToolStripMenuItem.Name = "dumpMemoryToolStripMenuItem";
            this.dumpMemoryToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.dumpMemoryToolStripMenuItem.Text = "Dump memory";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmsiWaitForVBLANK,
            this.tsmiShowSourceListing,
            this.autostartToolStripMenuItem,
            this.fullScreenFilterToolStripMenuItem,
            this.resetInstructionCounterToolStripMenuItem,
            this.joystickMappingToolStripMenuItem,
            this.disableGraphicsToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // tmsiWaitForVBLANK
            // 
            this.tmsiWaitForVBLANK.Checked = true;
            this.tmsiWaitForVBLANK.CheckOnClick = true;
            this.tmsiWaitForVBLANK.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tmsiWaitForVBLANK.Name = "tmsiWaitForVBLANK";
            this.tmsiWaitForVBLANK.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.tmsiWaitForVBLANK.Size = new System.Drawing.Size(266, 22);
            this.tmsiWaitForVBLANK.Text = "Wait for VBLANK";
            this.tmsiWaitForVBLANK.Click += new System.EventHandler(this.tmsiWaitForVBLANK_Click);
            // 
            // tsmiShowSourceListing
            // 
            this.tsmiShowSourceListing.Checked = true;
            this.tsmiShowSourceListing.CheckOnClick = true;
            this.tsmiShowSourceListing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiShowSourceListing.Name = "tsmiShowSourceListing";
            this.tsmiShowSourceListing.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.tsmiShowSourceListing.Size = new System.Drawing.Size(266, 22);
            this.tsmiShowSourceListing.Text = "Show source listing";
            this.tsmiShowSourceListing.CheckStateChanged += new System.EventHandler(this.showSourceListingToolStripMenuItem_CheckStateChanged);
            // 
            // autostartToolStripMenuItem
            // 
            this.autostartToolStripMenuItem.CheckOnClick = true;
            this.autostartToolStripMenuItem.Name = "autostartToolStripMenuItem";
            this.autostartToolStripMenuItem.Size = new System.Drawing.Size(266, 22);
            this.autostartToolStripMenuItem.Text = "Auto start";
            // 
            // fullScreenFilterToolStripMenuItem
            // 
            this.fullScreenFilterToolStripMenuItem.Name = "fullScreenFilterToolStripMenuItem";
            this.fullScreenFilterToolStripMenuItem.Size = new System.Drawing.Size(266, 22);
            this.fullScreenFilterToolStripMenuItem.Text = "Full-Screen Filter";
            // 
            // resetInstructionCounterToolStripMenuItem
            // 
            this.resetInstructionCounterToolStripMenuItem.Name = "resetInstructionCounterToolStripMenuItem";
            this.resetInstructionCounterToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.I)));
            this.resetInstructionCounterToolStripMenuItem.Size = new System.Drawing.Size(266, 22);
            this.resetInstructionCounterToolStripMenuItem.Text = "Reset Instruction counter";
            this.resetInstructionCounterToolStripMenuItem.Click += new System.EventHandler(this.resetInstructionCounterToolStripMenuItem_Click);
            // 
            // joystickMappingToolStripMenuItem
            // 
            this.joystickMappingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.defaultMappingToolStripMenuItem,
            this.chip8MappingToolStripMenuItem});
            this.joystickMappingToolStripMenuItem.Name = "joystickMappingToolStripMenuItem";
            this.joystickMappingToolStripMenuItem.Size = new System.Drawing.Size(266, 22);
            this.joystickMappingToolStripMenuItem.Text = "Joystick mapping";
            // 
            // defaultMappingToolStripMenuItem
            // 
            this.defaultMappingToolStripMenuItem.Name = "defaultMappingToolStripMenuItem";
            this.defaultMappingToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.defaultMappingToolStripMenuItem.Text = "Default mapping";
            this.defaultMappingToolStripMenuItem.Click += new System.EventHandler(this.defaultMappingToolStripMenuItem_Click);
            // 
            // chip8MappingToolStripMenuItem
            // 
            this.chip8MappingToolStripMenuItem.Name = "chip8MappingToolStripMenuItem";
            this.chip8MappingToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.chip8MappingToolStripMenuItem.Text = "Chip8 mapping";
            this.chip8MappingToolStripMenuItem.Click += new System.EventHandler(this.chip8MappingToolStripMenuItem_Click);
            // 
            // disableGraphicsToolStripMenuItem
            // 
            this.disableGraphicsToolStripMenuItem.Checked = true;
            this.disableGraphicsToolStripMenuItem.CheckOnClick = true;
            this.disableGraphicsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.disableGraphicsToolStripMenuItem.Name = "disableGraphicsToolStripMenuItem";
            this.disableGraphicsToolStripMenuItem.Size = new System.Drawing.Size(266, 22);
            this.disableGraphicsToolStripMenuItem.Text = "Disable Graphics";
            this.disableGraphicsToolStripMenuItem.Click += new System.EventHandler(this.disableGraphicsToolStripMenuItem_Click);
            // 
            // slowTimer
            // 
            this.slowTimer.Enabled = true;
            this.slowTimer.Interval = 1000;
            this.slowTimer.Tick += new System.EventHandler(this.slowTimer_Tick);
            // 
            // gbControls
            // 
            this.gbControls.Controls.Add(this.btnStep);
            this.gbControls.Controls.Add(this.btnStepInto);
            this.gbControls.Controls.Add(this.btnRun);
            this.gbControls.Controls.Add(this.btnReset);
            this.gbControls.Font = new System.Drawing.Font("Lucida Console", 8.3F);
            this.gbControls.Location = new System.Drawing.Point(157, 265);
            this.gbControls.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbControls.Name = "gbControls";
            this.gbControls.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbControls.Size = new System.Drawing.Size(320, 71);
            this.gbControls.TabIndex = 20;
            this.gbControls.TabStop = false;
            this.gbControls.Text = "Controls";
            // 
            // btnStep
            // 
            this.btnStep.Location = new System.Drawing.Point(162, 17);
            this.btnStep.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(75, 34);
            this.btnStep.TabIndex = 18;
            this.btnStep.TabStop = false;
            this.btnStep.Text = "Step (F10)";
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            // 
            // btnStepInto
            // 
            this.btnStepInto.Location = new System.Drawing.Point(243, 17);
            this.btnStepInto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnStepInto.Name = "btnStepInto";
            this.btnStepInto.Size = new System.Drawing.Size(75, 34);
            this.btnStepInto.TabIndex = 17;
            this.btnStepInto.TabStop = false;
            this.btnStepInto.Text = "StepInto (F11)";
            this.btnStepInto.UseVisualStyleBackColor = true;
            this.btnStepInto.Click += new System.EventHandler(this.btnStepInto_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(81, 17);
            this.btnRun.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 34);
            this.btnRun.TabIndex = 16;
            this.btnRun.TabStop = false;
            this.btnRun.Text = "Run  (F5)";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnReset
            // 
            this.btnReset.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.btnReset.Location = new System.Drawing.Point(0, 17);
            this.btnReset.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 34);
            this.btnReset.TabIndex = 15;
            this.btnReset.TabStop = false;
            this.btnReset.Text = "Reset (F1)";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gbMemory);
            this.splitContainer1.Panel1.Controls.Add(this.pbEmuScreen);
            this.splitContainer1.Panel1.Controls.Add(this.gbRegisters);
            this.splitContainer1.Panel1.Controls.Add(this.gbWatches);
            this.splitContainer1.Panel1.Controls.Add(this.gbControls);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.gbSource);
            this.splitContainer1.Size = new System.Drawing.Size(1188, 588);
            this.splitContainer1.SplitterDistance = 875;
            this.splitContainer1.TabIndex = 28;
            // 
            // gbMemory
            // 
            this.gbMemory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbMemory.Controls.Add(this.hexEdit1);
            this.gbMemory.Font = new System.Drawing.Font("Lucida Console", 8.3F);
            this.gbMemory.Location = new System.Drawing.Point(3, 341);
            this.gbMemory.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbMemory.Name = "gbMemory";
            this.gbMemory.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbMemory.Size = new System.Drawing.Size(869, 243);
            this.gbMemory.TabIndex = 28;
            this.gbMemory.TabStop = false;
            this.gbMemory.Text = "Memory";
            // 
            // hexEdit1
            // 
            this.hexEdit1.BitCount = MW.HexEdit.eBitCount.eight;
            this.hexEdit1.CharIndex = 0;
            this.hexEdit1.ColumnsPerRow = 16;
            this.hexEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexEdit1.EndAddress = ((uint)(0u));
            this.hexEdit1.Endianess = MW.HexEdit.eEndianess.Little;
            this.hexEdit1.Location = new System.Drawing.Point(3, 16);
            this.hexEdit1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.hexEdit1.Name = "hexEdit1";
            this.hexEdit1.Radix = MW.HexEdit.eRadix.Hexadecimal;
            this.hexEdit1.ShowCharacterColumn = true;
            this.hexEdit1.Size = new System.Drawing.Size(863, 223);
            this.hexEdit1.StartAddress = ((uint)(0u));
            this.hexEdit1.TabIndex = 17;
            // 
            // pbEmuScreen
            // 
            this.pbEmuScreen.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pbEmuScreen.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.pbEmuScreen.Location = new System.Drawing.Point(157, 21);
            this.pbEmuScreen.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pbEmuScreen.Name = "pbEmuScreen";
            this.pbEmuScreen.Size = new System.Drawing.Size(320, 240);
            this.pbEmuScreen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbEmuScreen.TabIndex = 27;
            this.pbEmuScreen.TabStop = false;
            this.pbEmuScreen.Click += new System.EventHandler(this.pbEmuScreen_Click);
            // 
            // gbRegisters
            // 
            this.gbRegisters.Controls.Add(this.cbRealtimeRegisterUpdate);
            this.gbRegisters.Controls.Add(this.tbSP);
            this.gbRegisters.Controls.Add(this.lblI);
            this.gbRegisters.Controls.Add(this.tbPC);
            this.gbRegisters.Controls.Add(this.lblPC);
            this.gbRegisters.Controls.Add(this.gbFlags);
            this.gbRegisters.Font = new System.Drawing.Font("Lucida Console", 8.3F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbRegisters.Location = new System.Drawing.Point(3, 4);
            this.gbRegisters.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbRegisters.Name = "gbRegisters";
            this.gbRegisters.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbRegisters.Size = new System.Drawing.Size(148, 332);
            this.gbRegisters.TabIndex = 12;
            this.gbRegisters.TabStop = false;
            this.gbRegisters.Text = "Registers";
            // 
            // cbRealtimeRegisterUpdate
            // 
            this.cbRealtimeRegisterUpdate.AutoSize = true;
            this.cbRealtimeRegisterUpdate.Location = new System.Drawing.Point(12, 17);
            this.cbRealtimeRegisterUpdate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbRealtimeRegisterUpdate.Name = "cbRealtimeRegisterUpdate";
            this.cbRealtimeRegisterUpdate.Size = new System.Drawing.Size(129, 16);
            this.cbRealtimeRegisterUpdate.TabIndex = 20;
            this.cbRealtimeRegisterUpdate.Text = "Realtime Update";
            this.cbRealtimeRegisterUpdate.UseVisualStyleBackColor = true;
            // 
            // tbSP
            // 
            this.tbSP.Font = new System.Drawing.Font("Lucida Console", 8.3F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSP.Location = new System.Drawing.Point(98, 38);
            this.tbSP.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbSP.MaxLength = 4;
            this.tbSP.Name = "tbSP";
            this.tbSP.Size = new System.Drawing.Size(43, 19);
            this.tbSP.TabIndex = 14;
            this.tbSP.TabStop = false;
            this.tbSP.Text = "FFFF";
            this.tbSP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblI
            // 
            this.lblI.AutoSize = true;
            this.lblI.Font = new System.Drawing.Font("Lucida Console", 8.3F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblI.Location = new System.Drawing.Point(74, 41);
            this.lblI.Name = "lblI";
            this.lblI.Size = new System.Drawing.Size(19, 12);
            this.lblI.TabIndex = 13;
            this.lblI.Text = "SP";
            this.lblI.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbPC
            // 
            this.tbPC.Font = new System.Drawing.Font("Lucida Console", 8.3F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPC.Location = new System.Drawing.Point(26, 38);
            this.tbPC.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbPC.MaxLength = 4;
            this.tbPC.Name = "tbPC";
            this.tbPC.Size = new System.Drawing.Size(43, 19);
            this.tbPC.TabIndex = 12;
            this.tbPC.TabStop = false;
            this.tbPC.Text = "FFFF";
            this.tbPC.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbPC.TextChanged += new System.EventHandler(this.tbPC_TextChanged);
            // 
            // lblPC
            // 
            this.lblPC.AutoSize = true;
            this.lblPC.Font = new System.Drawing.Font("Lucida Console", 8.3F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPC.Location = new System.Drawing.Point(4, 41);
            this.lblPC.Name = "lblPC";
            this.lblPC.Size = new System.Drawing.Size(19, 12);
            this.lblPC.TabIndex = 11;
            this.lblPC.Text = "PC";
            // 
            // gbFlags
            // 
            this.gbFlags.Controls.Add(this.tbFlagN);
            this.gbFlags.Controls.Add(this.label4);
            this.gbFlags.Controls.Add(this.tbFlagO);
            this.gbFlags.Controls.Add(this.label3);
            this.gbFlags.Controls.Add(this.tbFlagZ);
            this.gbFlags.Controls.Add(this.label1);
            this.gbFlags.Controls.Add(this.label2);
            this.gbFlags.Controls.Add(this.tbFlagC);
            this.gbFlags.Font = new System.Drawing.Font("Lucida Console", 8.3F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbFlags.Location = new System.Drawing.Point(7, 269);
            this.gbFlags.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbFlags.Name = "gbFlags";
            this.gbFlags.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbFlags.Size = new System.Drawing.Size(134, 52);
            this.gbFlags.TabIndex = 10;
            this.gbFlags.TabStop = false;
            this.gbFlags.Text = "Flags";
            // 
            // tbFlagN
            // 
            this.tbFlagN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFlagN.Location = new System.Drawing.Point(113, 26);
            this.tbFlagN.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbFlagN.MaxLength = 1;
            this.tbFlagN.Name = "tbFlagN";
            this.tbFlagN.ReadOnly = true;
            this.tbFlagN.Size = new System.Drawing.Size(16, 19);
            this.tbFlagN.TabIndex = 14;
            this.tbFlagN.TabStop = false;
            this.tbFlagN.Text = "0";
            this.tbFlagN.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbFlagN.DoubleClick += new System.EventHandler(this.tbFlagC_DoubleClick);
            this.tbFlagN.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(115, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(12, 12);
            this.label4.TabIndex = 13;
            this.label4.Text = "N";
            // 
            // tbFlagO
            // 
            this.tbFlagO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFlagO.Location = new System.Drawing.Point(93, 26);
            this.tbFlagO.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbFlagO.MaxLength = 1;
            this.tbFlagO.Name = "tbFlagO";
            this.tbFlagO.ReadOnly = true;
            this.tbFlagO.Size = new System.Drawing.Size(16, 19);
            this.tbFlagO.TabIndex = 12;
            this.tbFlagO.TabStop = false;
            this.tbFlagO.Text = "0";
            this.tbFlagO.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbFlagO.DoubleClick += new System.EventHandler(this.tbFlagC_DoubleClick);
            this.tbFlagO.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(95, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "O";
            // 
            // tbFlagZ
            // 
            this.tbFlagZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFlagZ.Location = new System.Drawing.Point(73, 26);
            this.tbFlagZ.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbFlagZ.MaxLength = 1;
            this.tbFlagZ.Name = "tbFlagZ";
            this.tbFlagZ.ReadOnly = true;
            this.tbFlagZ.Size = new System.Drawing.Size(16, 19);
            this.tbFlagZ.TabIndex = 10;
            this.tbFlagZ.TabStop = false;
            this.tbFlagZ.Text = "0";
            this.tbFlagZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbFlagZ.DoubleClick += new System.EventHandler(this.tbFlagC_DoubleClick);
            this.tbFlagZ.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(75, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(12, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "Z";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(55, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "C";
            // 
            // tbFlagC
            // 
            this.tbFlagC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFlagC.Location = new System.Drawing.Point(53, 26);
            this.tbFlagC.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbFlagC.MaxLength = 1;
            this.tbFlagC.Name = "tbFlagC";
            this.tbFlagC.ReadOnly = true;
            this.tbFlagC.Size = new System.Drawing.Size(16, 19);
            this.tbFlagC.TabIndex = 7;
            this.tbFlagC.TabStop = false;
            this.tbFlagC.Text = "0";
            this.tbFlagC.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbFlagC.DoubleClick += new System.EventHandler(this.tbFlagC_DoubleClick);
            this.tbFlagC.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            // 
            // gbWatches
            // 
            this.gbWatches.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbWatches.Controls.Add(this.cbRealtimeWatches);
            this.gbWatches.Controls.Add(this.lvWatches);
            this.gbWatches.Font = new System.Drawing.Font("Lucida Console", 8.3F);
            this.gbWatches.Location = new System.Drawing.Point(483, 5);
            this.gbWatches.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbWatches.Name = "gbWatches";
            this.gbWatches.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbWatches.Size = new System.Drawing.Size(389, 331);
            this.gbWatches.TabIndex = 26;
            this.gbWatches.TabStop = false;
            this.gbWatches.Text = "Watches";
            // 
            // cbRealtimeWatches
            // 
            this.cbRealtimeWatches.Location = new System.Drawing.Point(25, 16);
            this.cbRealtimeWatches.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbRealtimeWatches.Name = "cbRealtimeWatches";
            this.cbRealtimeWatches.Size = new System.Drawing.Size(160, 14);
            this.cbRealtimeWatches.TabIndex = 21;
            this.cbRealtimeWatches.Text = "Realtime Update";
            this.cbRealtimeWatches.UseVisualStyleBackColor = true;
            // 
            // lvWatches
            // 
            this.lvWatches.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvWatches.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chName,
            this.chAddress,
            this.chValue});
            this.lvWatches.ContextMenuStrip = this.cmsWatchesMenu;
            this.lvWatches.FullRowSelect = true;
            this.lvWatches.GridLines = true;
            this.lvWatches.HideSelection = false;
            this.lvWatches.Location = new System.Drawing.Point(3, 33);
            this.lvWatches.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lvWatches.Name = "lvWatches";
            this.lvWatches.Size = new System.Drawing.Size(386, 295);
            this.lvWatches.TabIndex = 0;
            this.lvWatches.UseCompatibleStateImageBehavior = false;
            this.lvWatches.View = System.Windows.Forms.View.Details;
            // 
            // chName
            // 
            this.chName.Text = "Name";
            this.chName.Width = 113;
            // 
            // chAddress
            // 
            this.chAddress.Text = "Address";
            this.chAddress.Width = 72;
            // 
            // chValue
            // 
            this.chValue.Text = "Value";
            // 
            // gbSource
            // 
            this.gbSource.Controls.Add(this.tbSource);
            this.gbSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbSource.Font = new System.Drawing.Font("Lucida Console", 8.3F);
            this.gbSource.Location = new System.Drawing.Point(0, 0);
            this.gbSource.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbSource.Name = "gbSource";
            this.gbSource.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbSource.Size = new System.Drawing.Size(309, 588);
            this.gbSource.TabIndex = 28;
            this.gbSource.TabStop = false;
            this.gbSource.Text = "Source";
            // 
            // tbSource
            // 
            this.tbSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbSource.Font = new System.Drawing.Font("Lucida Console", 9.3F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSource.Location = new System.Drawing.Point(3, 16);
            this.tbSource.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbSource.Name = "tbSource";
            this.tbSource.ReadOnly = true;
            this.tbSource.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.tbSource.Size = new System.Drawing.Size(303, 568);
            this.tbSource.TabIndex = 1;
            this.tbSource.TabStop = false;
            this.tbSource.Text = "";
            this.tbSource.WordWrap = false;
            this.tbSource.DoubleClick += new System.EventHandler(this.tbSource_DoubleClick);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1188, 634);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.mainMenu);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MainMenuStrip = this.mainMenu;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.Text = "nChip16";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mainForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mainForm_KeyUp);
            this.cmsSetLastSource.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.cmsWatchesMenu.ResumeLayout(false);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.gbControls.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.gbMemory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbEmuScreen)).EndInit();
            this.gbRegisters.ResumeLayout(false);
            this.gbRegisters.PerformLayout();
            this.gbFlags.ResumeLayout(false);
            this.gbFlags.PerformLayout();
            this.gbWatches.ResumeLayout(false);
            this.gbSource.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer emuTimer;
        private ContextMenuStrip cmsSetLastSource;
        private ToolStripMenuItem cmsiSetLastSource;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel tsslInitialPC;
        private ToolStripStatusLabel toolStripStatusLabel5;
        private ToolStripStatusLabel tsslSpecVersion;
        private ToolStripStatusLabel toolStripStatusLabel7;
        private ToolStripStatusLabel tssLabelRomSize;
        private ToolStripStatusLabel tsslRomSize;
        private ToolStripStatusLabel tsslRomName;
        private MenuStrip mainMenu;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem dumpMemoryToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ContextMenuStrip cmsWatchesMenu;
        private ToolStripMenuItem addWatchToolStripMenuItem;
        private ToolStripMenuItem editWatchToolStripMenuItem;
        private ToolStripMenuItem deleteWatchToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem tmsiWaitForVBLANK;
        private ToolStripStatusLabel tssLabelChip16Usage;
        private ToolStripStatusLabel tsslChip16Usage;
        private Timer slowTimer;
        private ToolStripMenuItem tsmiShowSourceListing;
        private GroupBox gbControls;
        private KeyHandleButton btnStep;
        private KeyHandleButton btnStepInto;
        private KeyHandleButton btnRun;
        private KeyHandleButton btnReset;
        private SplitContainer splitContainer1;
        private GroupBox gbMemory;
        private MW.HexEdit.HexEdit hexEdit1;
        private PictureBoxExtended pbEmuScreen;
        private GroupBox gbRegisters;
        private CheckBox cbRealtimeRegisterUpdate;
        private KeyHandleTextBox tbSP;
        private Label lblI;
        private KeyHandleTextBox tbPC;
        private Label lblPC;
        private GroupBox gbFlags;
        private KeyHandleTextBox tbFlagN;
        private Label label4;
        private KeyHandleTextBox tbFlagO;
        private Label label3;
        private KeyHandleTextBox tbFlagZ;
        private Label label1;
        private Label label2;
        private KeyHandleTextBox tbFlagC;
        private GroupBox gbWatches;
        private CheckBox cbRealtimeWatches;
        private ListView lvWatches;
        private ColumnHeader chName;
        private ColumnHeader chAddress;
        private ColumnHeader chValue;
        private GroupBox gbSource;
        private RichTextBox tbSource;
        private ToolStripMenuItem fullScreenFilterToolStripMenuItem;
        private ToolStripMenuItem autostartToolStripMenuItem;
        private ToolStripStatusLabel tssLabelInstructionCount;
        private ToolStripStatusLabel tsslInstructionCount;
        private ToolStripMenuItem resetInstructionCounterToolStripMenuItem;
        private ToolStripMenuItem joystickMappingToolStripMenuItem;
        private ToolStripMenuItem defaultMappingToolStripMenuItem;
        private ToolStripMenuItem chip8MappingToolStripMenuItem;
        private ToolStripStatusLabel tssLabelDrawFrameTimer;
        private ToolStripStatusLabel tssDrawFrameTimer;
        private ToolStripStatusLabel tssLabelFps;
        private ToolStripStatusLabel tssFps;
        private ToolStripMenuItem disableGraphicsToolStripMenuItem;
    }
}

