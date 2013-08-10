using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nChip16
{
    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color textColor, Color backgroundColor)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = textColor;
            box.SelectionBackColor = backgroundColor;

            box.AppendText(text);
            //box.SelectionColor = box.ForeColor;
        }
    }
}
