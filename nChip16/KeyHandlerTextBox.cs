using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nChip16
{
    public partial class KeyHandleTextBox : TextBox
    {
        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Right || keyData == Keys.Left ||
                keyData == Keys.Up || keyData == Keys.Down)
            {
                return true;
            }
            else
            {
                return base.IsInputKey(keyData);
            }
        }

        private bool _focused;

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            if (MouseButtons == MouseButtons.None)
            {
                SelectAll();
                _focused = true;
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            _focused = false;
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            if (!_focused)
            {
                if (SelectionLength == 0)
                    SelectAll();
                _focused = true;
            }
        }

        protected override void OnKeyDown(KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.KeyCode == Keys.Return)
            {
                ushort result = 0;
                if (Text.Length == 0)
                    Text = 0.ToString("X4");

                if (!ushort.TryParse(Text, NumberStyles.HexNumber, null, out result))
                {
                    MessageBox.Show("Invalid number!");
                    return;
                }
                Text = result.ToString("X4");
                
                Enabled = false;
                Enabled = true;
                keyEventArgs.Handled = true;

                base.OnKeyDown(keyEventArgs);
            }
        }
    }
}
