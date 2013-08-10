using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nChip16
{
    public partial class WatchForm : Form
    {
        public Watch Watch = new Watch();

        public WatchForm()
        {
            InitializeComponent();

            PopulateAllFields();
        }

        private void PopulateAllFields()
        {
            PopulateLockToItems(cbLockTo);
            PopulateTypeItems(cbType);
            PopulateShowAs(cbShowAs);
        }

        private static void PopulateShowAs(ComboBox comboBox)
        {
            comboBox.Items.Add(ShowAs.Hexadecimal);
            comboBox.Items.Add(ShowAs.Decimal);
            comboBox.Items.Add(ShowAs.Binary);

            comboBox.SelectedIndex = 0;
        }

        private static void PopulateLabels(ComboBox comboBox, List<LineLabel> labels)
        {
            foreach (var label in labels)
                comboBox.Items.Add(label);
        }
        
        private static void PopulateTypeItems(ComboBox comboBox)
        {
            comboBox.Items.Add(WatchType.Byte);
            comboBox.Items.Add(WatchType.Word);

            comboBox.SelectedIndex = 0;
        }

        private static void PopulateLockToItems(ComboBox comboxBox)
        {
            comboxBox.Items.Add(LockTo.Address);
            comboxBox.Items.Add(LockTo.Label);

            comboxBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Constructor used when initializing WatchForm. Used for Edit and Drag'n'Drop
        /// </summary>
        /// <param name="initWatch"></param>
        public WatchForm(Watch initWatch)
        {
            InitializeComponent();

            Watch = initWatch;

            PopulateAllFields();

            tbWatchAddress.Text = Watch.Address.ToString("X4");
            cbName.Text = Watch.Name;
            cbType.SelectedItem = Watch.Type;
            cbLockTo.SelectedItem = Watch.LockTo;
            cbShowAs.SelectedItem = Watch.ShowAs;
        }

        private void WatchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                return;

            // check all values so they are valid
            if (string.IsNullOrEmpty(tbWatchAddress.Text))
            {
                MessageBox.Show("Address cannot be empty");
                e.Cancel = true;
                return;
            }

            ushort address = 0;
            if (!ushort.TryParse(tbWatchAddress.Text, NumberStyles.HexNumber, null, out address))
            {
                MessageBox.Show("Address should be a 16-bit hexadecimal number. Valid values are: 0000-FFFF");
                e.Cancel = true;
                return;
            }
            Watch.Address = address;
            Watch.Name = cbName.Text;
            Watch.LockTo = (LockTo) cbLockTo.SelectedItem;
            Watch.Type = (WatchType) cbType.SelectedItem;
            Watch.ShowAs = (ShowAs) cbShowAs.SelectedItem;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        internal void InitializeLabels(List<LineLabel> labels)
        {
            PopulateLabels(cbName, labels);
        }

        private void cbName_SelectedValueChanged(object sender, EventArgs e)
        {
            // a LineLabel was chosen, fill textboxes with info
            var selectedItem = (LineLabel)cbName.SelectedItem;

            tbWatchAddress.Text = selectedItem.Address.ToString("X4");
            cbLockTo.SelectedItem = LockTo.Label; // is added twice
        }
    }
}
