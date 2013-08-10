namespace nChip16
{
    partial class WatchForm
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
            this.lblName = new System.Windows.Forms.Label();
            this.lblAddress = new System.Windows.Forms.Label();
            this.lblType = new System.Windows.Forms.Label();
            this.tbWatchAddress = new System.Windows.Forms.TextBox();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbLockTo = new System.Windows.Forms.ComboBox();
            this.lblLockTo = new System.Windows.Forms.Label();
            this.cbName = new System.Windows.Forms.ComboBox();
            this.cbShowAs = new System.Windows.Forms.ComboBox();
            this.lblPresentation = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(22, 13);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name:";
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(12, 37);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(48, 13);
            this.lblAddress.TabIndex = 1;
            this.lblAddress.Text = "Address:";
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(26, 90);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(34, 13);
            this.lblType.TabIndex = 2;
            this.lblType.Text = "Type:";
            // 
            // tbWatchAddress
            // 
            this.tbWatchAddress.Location = new System.Drawing.Point(67, 34);
            this.tbWatchAddress.MaxLength = 4;
            this.tbWatchAddress.Name = "tbWatchAddress";
            this.tbWatchAddress.Size = new System.Drawing.Size(112, 20);
            this.tbWatchAddress.TabIndex = 1;
            // 
            // cbType
            // 
            this.cbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbType.FormattingEnabled = true;
            this.cbType.Location = new System.Drawing.Point(66, 87);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(113, 21);
            this.cbType.TabIndex = 3;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(17, 149);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(104, 149);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cbLockTo
            // 
            this.cbLockTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLockTo.FormattingEnabled = true;
            this.cbLockTo.Location = new System.Drawing.Point(66, 60);
            this.cbLockTo.Name = "cbLockTo";
            this.cbLockTo.Size = new System.Drawing.Size(113, 21);
            this.cbLockTo.TabIndex = 2;
            // 
            // lblLockTo
            // 
            this.lblLockTo.AutoSize = true;
            this.lblLockTo.Location = new System.Drawing.Point(14, 63);
            this.lblLockTo.Name = "lblLockTo";
            this.lblLockTo.Size = new System.Drawing.Size(46, 13);
            this.lblLockTo.TabIndex = 9;
            this.lblLockTo.Text = "Lock to:";
            // 
            // cbName
            // 
            this.cbName.DisplayMember = "Name";
            this.cbName.FormattingEnabled = true;
            this.cbName.Location = new System.Drawing.Point(66, 7);
            this.cbName.Name = "cbName";
            this.cbName.Size = new System.Drawing.Size(113, 21);
            this.cbName.TabIndex = 0;
            this.cbName.SelectedValueChanged += new System.EventHandler(this.cbName_SelectedValueChanged);
            // 
            // cbShowAs
            // 
            this.cbShowAs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbShowAs.FormattingEnabled = true;
            this.cbShowAs.Location = new System.Drawing.Point(66, 114);
            this.cbShowAs.Name = "cbShowAs";
            this.cbShowAs.Size = new System.Drawing.Size(113, 21);
            this.cbShowAs.TabIndex = 10;
            // 
            // lblPresentation
            // 
            this.lblPresentation.AutoSize = true;
            this.lblPresentation.Location = new System.Drawing.Point(9, 117);
            this.lblPresentation.Name = "lblPresentation";
            this.lblPresentation.Size = new System.Drawing.Size(51, 13);
            this.lblPresentation.TabIndex = 11;
            this.lblPresentation.Text = "Show as:";
            // 
            // WatchForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(202, 184);
            this.ControlBox = false;
            this.Controls.Add(this.lblPresentation);
            this.Controls.Add(this.cbShowAs);
            this.Controls.Add(this.cbName);
            this.Controls.Add(this.cbLockTo);
            this.Controls.Add(this.lblLockTo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cbType);
            this.Controls.Add(this.tbWatchAddress);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.lblAddress);
            this.Controls.Add(this.lblName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WatchForm";
            this.ShowInTaskbar = false;
            this.Text = "Create Watch";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WatchForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.TextBox tbWatchAddress;
        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cbLockTo;
        private System.Windows.Forms.Label lblLockTo;
        private System.Windows.Forms.ComboBox cbName;
        private System.Windows.Forms.ComboBox cbShowAs;
        private System.Windows.Forms.Label lblPresentation;
    }
}