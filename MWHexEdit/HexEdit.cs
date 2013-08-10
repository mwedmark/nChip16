using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using MWHexEdit;
using System.Reflection;

namespace MW.HexEdit
{
    public delegate void dOnDataChanged(object sender, DataChangedEventArgs argsDataChanged);
    public delegate void dScroll(object sender, ScrollEventArgs e);

    public partial class HexEdit : UserControl
    {
        private bool isSelecting = false;
        //private bool isRebuildingSelection = false;
        private bool isRedrawing = false;

        private byte[] data;
        public void SetData(byte[] newData, int startAddress)
        {
            this.data = newData;
            RedrawGrid();
        }

        public HexEdit()
        {
            ColumnsPerRow = 8;
            BitCount = eBitCount.eight;
            Radix = eRadix.Hexadecimal;
            InitializeComponent();

            // init with dummy data
            data = new byte[256];
            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)i;

            StartAddress = 0;
            EndAddress = 0;

            Type type = dataGridView1.GetType();
            PropertyInfo f1 = type.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            f1.SetValue(dataGridView1, true, null);

            //remove selection
            dataGridView1.ClearSelection();
        }

        Point startSelectionPoint = new Point();
        Point endSelectionPoint = new Point();

        #region PublicEvents

        /// <summary>
        /// Event that signals external classes when data has been changed from within the control
        /// </summary>
        public event dOnDataChanged OnDataChanged;

        public void DataChangedExternally(object sender, DataChangedEventArgs argsDataChanged)
        {
        }

        public event dScroll OnScrolling; 
        
        #endregion

        #region PrivateEvents

        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            // call the public event
            if(OnScrolling != null)
                OnScrolling(sender, e);
        }
        
        #endregion

        public bool ShowCharacterColumn { get; set; }

        public int CharIndex { get; set; }

        public eBitCount BitCount { get; set; }

        public eRadix Radix { get; set; }

        public eEndianess Endianess { get; set; }

        public int ColumnsPerRow { get; set; }

        public uint StartAddress { get; set; }

        public uint EndAddress { get; set; }

        public delegate void RenewVisibleValuesDelegate();

        public void RenewVisibleValues()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new RenewVisibleValuesDelegate(RenewVisibleValues));
                return;
            }

            //var range = GetCurrentVisibleRange();

            // update all graphical values inside visible range
            var firstIndex = GetFirstRowIndexShown();
            var lastIndex = GetLastRowIndexShown();

            dataGridView1.SuspendLayout();
            for (var rowIndex = firstIndex; rowIndex<=lastIndex;rowIndex++)
            {
                var currentRow = dataGridView1.Rows.SharedRow(rowIndex); // test with shared row instead of direct index
                RenderDataGridRow(currentRow, (uint)(StartAddress+(rowIndex * ColumnsPerRow)),false);
            }
            dataGridView1.ResumeLayout();
        }

        private int GetFirstRowIndexShown()
        {
            return dataGridView1.FirstDisplayedScrollingRowIndex;
        }

        private int GetLastRowIndexShown()
        {
            return GetFirstRowIndexShown() + dataGridView1.DisplayedRowCount(true) -1;
        }

        private int GetAddressForRow(int rowIndex)
        {
            return 0; // fix this
        }

        // public for debugging purposes, later used only internally by RenewVisibleValues
        public MemoryRange GetCurrentVisibleRange()
        {
            var range = new MemoryRange();
            
            var rowsShown = dataGridView1.DisplayedRowCount(true);
            var firstVisibleRow = GetFirstRowIndexShown();

            // calculate addresses using the columnsPerRow
            range.StartAddress = (int)((firstVisibleRow * ColumnsPerRow)+StartAddress);
            range.Length = rowsShown*ColumnsPerRow;

            return range;
        }

        public void RedrawGrid()
        {
            isRedrawing = true;

            try
            {
                dataGridView1.ColumnCount = 2;
                dataGridView1.RowCount = 0;
                FillControl();
            }
            finally
            {
                isRedrawing = false;
            }
        }

        public void FillControl()
        {
            if (data == null || data.Length == 0)
                return;

            DataGridViewColumn defaultCol = dataGridView1.Columns[1];

            // make sure to scale the defaultCol correctly depending on the chosen Radix
            int newDataWidth = 0;
            
            switch(Radix)
            {
                case eRadix.Binary:
                    newDataWidth = 80;
                    break;
                case eRadix.Decimal:
                    newDataWidth = 35;
                    break;
                case eRadix.Hexadecimal:
                    newDataWidth = 25;
                    break;

            }
            defaultCol.Width = newDataWidth;

            // setup Memory view window by creating right number of cells with the same properties as the one initially there
            for (int i = 1; i < ColumnsPerRow; i++)
            {
                DataGridViewColumn dgColumn = new DataGridViewColumn(defaultCol.CellTemplate);
                dgColumn.Width = defaultCol.Width;
                dgColumn.Name = (i).ToString("X2");
                
                dataGridView1.Columns.Add(dgColumn);
            }

            // create the last column as a character display
            DataGridViewColumn dgCharColumn = new DataGridViewColumn(defaultCol.CellTemplate);
            dgCharColumn.Width = 240;
            //dgCharColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; This gave the BAD performance for DataGridView
            dgCharColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns.Add(dgCharColumn);
            
            // update current index of where to find the character column
            CharIndex = dataGridView1.ColumnCount - 1;

            int dataIndex = 0;
            var createdRows = new List<DataGridViewRow>();

            dataGridView1.SuspendLayout();

            // if no endAddress has been set, make sure to sync graphics and data end.
            uint lastAddress = EndAddress == 0 ? (uint)data.Length : EndAddress;

            while (true)
            {
                uint currentRowMemStart = (uint)(StartAddress+dataIndex);

                if (currentRowMemStart >= lastAddress)
                    break;

                var dataGridRow = new DataGridViewRow();
                for (int columnCount = 0; columnCount < ColumnsPerRow; columnCount++)
                    dataGridRow.CreateCells(dataGridView1);

                // add address value to address column
                RenderDataGridRow(dataGridRow, currentRowMemStart, false);

                createdRows.Add(dataGridRow);
                dataIndex += ColumnsPerRow;
            }
            
            dataGridView1.Rows.AddRange(createdRows.ToArray());
            dataGridView1.ResumeLayout();

            // does not work to remove inital errornous selection of address column!!!
            dataGridView1.ClearSelection();
            dataGridView1[0, 0].Selected = false;
            dataGridView1[1, 0].Selected = true;
            
        }

        private void RenderDataGridRow(DataGridViewRow row, uint startAddressForRow, bool autoLayoutControl)
        {
            // data row
            byte[] memRowData = new byte[ColumnsPerRow];

            // format row 
            string[] memRowDataFormatted = new string[ColumnsPerRow + 2];

            // add address value to address column
            memRowDataFormatted[0] = "0x" + (startAddressForRow).ToString("X4");
            
            // format address column
            for (int i = 0;i < ColumnsPerRow;i++)
            {
                long dataIndex = i + startAddressForRow;

                // format values regarding to chosen Radix
                memRowDataFormatted[i + 1] = FormatSingleDataValue(data[dataIndex], Radix);
                memRowData[i] = data[dataIndex];
            }

            // build character string and the end of each row
            if(ShowCharacterColumn)
                memRowDataFormatted[memRowDataFormatted.Length - 1] = buildCharacterString(memRowData);

            if(autoLayoutControl)
                dataGridView1.SuspendLayout();

            row.SetValues(memRowDataFormatted);
            
            if(autoLayoutControl)
                dataGridView1.ResumeLayout();
        }

        private string FormatSingleDataValue(byte data, eRadix radix)
        {
            string result;

            switch (Radix)
            {
                case eRadix.Binary:
                    result = Convert.ToString(data, 2).PadLeft(8, '0');
                    break;
                case eRadix.Decimal:
                    result = Convert.ToString(data, 10).PadLeft(3, '0');
                    break;
                case eRadix.Hexadecimal:
                    result = data.ToString("X2");
                    break;
                default:
                    throw new Exception("Format unknown!");
            }

            return result;
        }

        private void SetSingleValue(DataGridCell cell, byte value)
        {

        }

        private static string buildCharacterString(byte[] memRowData)
        {
            var sb = new StringBuilder();

            foreach (byte b in memRowData)
            {
                if ((b == 0) || (b == 0xBF) ||
                    (b >= 0x7F && b <= 0xA0) ||
                    (b >= 0x00 && b <= 0x1F)
                    )
                {
                    sb.Append(".");
                }
                else
                {
                    sb.Append(Convert.ToChar((byte)b));
                }
            }
            return sb.ToString();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (isRedrawing || e.RowIndex < 0 || e.ColumnIndex < 0 || e.ColumnIndex == CharIndex)
                    return;

                DataGridView gridView = (DataGridView)sender;
                //gridView[e.ColumnIndex, e.RowIndex].Value;
                DataGridViewCell cell = gridView[e.ColumnIndex, e.RowIndex];

                int basen = ConvertRadixToBase(Radix);
                var dataValue = Convert.ToByte((string)cell.Value, basen);
                
                //calculate where in memory this cell points 
                long address = ((e.RowIndex * this.ColumnsPerRow) + e.ColumnIndex - 1) + StartAddress;

                // make actual change in memory
                data[address] = dataValue;

                if (OnDataChanged != null)
                    OnDataChanged(sender, new DataChangedEventArgs((int)address, dataValue));

                int rowIndex = cell.RowIndex;

                if (ShowCharacterColumn)
                {
                    // update the corresponding character string
                    byte[] byteData = GetData(dataGridView1.Rows[rowIndex]);
                    string newCharData = buildCharacterString(byteData);
                    dataGridView1[CharIndex, rowIndex].Value = newCharData;
                }
                else
                    dataGridView1[CharIndex, rowIndex].Value = ""; // clear row if ShowCharacterColumn=false
                
            }
            catch (ConvertException cex)
            {
                MessageBox.Show(string.Format("There was a problem with converting the new value given, aborting! \r\nProblem: {0}",cex.Message));
            }
        }

        private byte[] GetData(DataGridViewRow dataGridViewRow)
        {
            byte[] newRowData = new byte[ColumnsPerRow];
            for(int i=1;i<=ColumnsPerRow;i++)
            {
                newRowData[i-1] = Convert.ToByte(dataGridViewRow.Cells[i].Value.ToString(), ConvertRadixToBase(Radix));
            }
            return newRowData;
        }

        private static int ConvertRadixToBase(eRadix radix)
        {
            int basen = 0;
            switch (radix)
            {
                case eRadix.Binary:
                    basen = 2;
                    break;
                case eRadix.Decimal:
                    basen = 10;
                    break;
                case eRadix.Hexadecimal:
                    basen = 16;
                    break;
            }
            return basen;
        }

        private int GetRowByCell(DataGridViewCell cell)
        {
            return 1;
        }

        private void ConvertToSeqentialSelection(DataGridViewSelectedCellCollection dataGridViewSelectedCellCollection)
        {
            try
            {
                if (dataGridViewSelectedCellCollection.Count == 0)
                    return;

                DataGridViewCell first = GetTopLeftSelectedCell(dataGridViewSelectedCellCollection);
                DataGridViewCell last = GetBottomRightSelectedCell(dataGridViewSelectedCellCollection);
                dataGridView1.ClearSelection();

                if (first != null && last != null && !NotDataCell(first) && !NotDataCell(last))
                {
                    //build new selection
                    CreateSequentialSelection(first, last);
                }
            }
            finally
            {
                isSelecting = false;
            }
        }

        private void CreateSequentialSelection(DataGridViewCell first, DataGridViewCell last)
        {
            first.Selected = true;
            last.Selected = true;

            int currentColIndex = first.ColumnIndex;
            int currentRowIndex = first.RowIndex;

            while (true)
            {
                dataGridView1[currentColIndex, currentRowIndex].Selected = true;
                currentColIndex++;
                if (currentColIndex > (dataGridView1.ColumnCount - 2))
                {
                    currentColIndex = 1;
                    currentRowIndex++;
                }
                if (currentColIndex >= last.ColumnIndex && currentRowIndex >= last.RowIndex)
                    break;
            }
        }

        private static DataGridViewCell GetBottomRightSelectedCell(DataGridViewSelectedCellCollection dataGridViewSelectedCellCollection)
        {
            int rightBottomIndex = 0;
            int rightMostIndex = 100;
            int bottomMostIndex = 100;
            for (int i = 0; i < dataGridViewSelectedCellCollection.Count; i++)
            {
                int currentCol = dataGridViewSelectedCellCollection[i].ColumnIndex;
                int currentRow = dataGridViewSelectedCellCollection[i].RowIndex;

                if (currentCol == rightMostIndex)
                {
                    if (currentRow > bottomMostIndex)
                    {
                        bottomMostIndex = currentRow;
                        rightBottomIndex = i;
                    }
                }

                if (currentCol > rightMostIndex)
                {
                    rightBottomIndex = currentCol;
                    bottomMostIndex = currentRow;
                    rightBottomIndex = i;
                }
            }

            return dataGridViewSelectedCellCollection[rightBottomIndex];
        }

        private static DataGridViewCell GetTopLeftSelectedCell(DataGridViewSelectedCellCollection dataGridViewSelectedCellCollection)
        {
            int leftTopIndex = 0;
            int leftMostIndex = 100;
            int topMostIndex = 100;
            for (int i = 0; i < dataGridViewSelectedCellCollection.Count; i++)
            {
                int currentCol = dataGridViewSelectedCellCollection[i].ColumnIndex;
                int currentRow = dataGridViewSelectedCellCollection[i].RowIndex;

                if (currentCol == leftMostIndex)
                {
                    if (currentRow < topMostIndex)
                    {
                        topMostIndex = currentRow;
                        leftTopIndex = i;
                    }
                }

                if (currentCol < leftMostIndex)
                {
                    leftTopIndex = currentCol;
                    topMostIndex = currentRow;
                    leftTopIndex = i;
                }
            }

            return dataGridViewSelectedCellCollection[leftTopIndex];
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // make sure to try and format the given value in regard to the currectly selected Radix.
            try
            {
                if (NotDataCell(dataGridView1[e.ColumnIndex,e.RowIndex]))
                    return;

                object o = e.FormattedValue;
                string st = (string)o;
                ulong newValue = ConvertValueOnRadix(st);
                ulong maxValue = 0;

                switch (BitCount)
                {
                    case eBitCount.eight:
                        maxValue = byte.MaxValue;
                        break;
                    case eBitCount.sixteen:
                        maxValue = UInt16.MaxValue;
                        break;
                    case eBitCount.thirtytwo:
                        maxValue = UInt32.MaxValue;
                        break;
                }
                
                // make sure that the new value fits into the cell it has been entered into
                if (maxValue < (ulong)newValue)
                    throw new OutOfRangeException("The value is outside the limits of which this cell can handle with the currently selected Radix.");
            }
            catch (ConvertException cex)
            {
                MessageBox.Show(string.Format("There was a problem with converting the new value given, aborting! \r\nProblem: {0}", cex.Message));
                e.Cancel = true;
            }
        }

        private ulong ConvertValueOnRadix(string st)
        {
            int currentRadix = ConvertRadixToBase(Radix);
            ulong newValue = Convert.ToUInt64(st, currentRadix);
            return newValue;
        }

        private bool NotDataCell(DataGridViewCell cell)
        {
            return isRedrawing || cell.ColumnIndex < 1 || cell.ColumnIndex > ColumnsPerRow;
        }

        private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            // possible to change next active cell
            //dataGridView1.CurrentCell = dataGridView1[e.ColumnIndex, e.RowIndex];
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.CellStyle.BackColor = Color.FromArgb(255,240,240);
            }

            if(NotDataCell(dataGridView1[e.ColumnIndex,e.RowIndex]))
                return;

            ulong value = ConvertValueOnRadix((string)e.Value);
            switch(Radix)
            {
                case eRadix.Binary:
                    e.Value = Convert.ToString((long)value,2).PadLeft(8,'0');
                break;
                case eRadix.Decimal:
                    e.Value = value.ToString("000");
                break;
                case eRadix.Hexadecimal:
                    e.Value = value.ToString("X2");
                break;
            }
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            //if (e.ColumnIndex < 1)
            //    return;

            Debug.WriteLine(string.Format(" MouseDown [{0},{1}]",e.ColumnIndex,e.RowIndex));
            startSelectionPoint.X = e.ColumnIndex;
            startSelectionPoint.Y = e.RowIndex;
            endSelectionPoint = startSelectionPoint;
            isSelecting = true; // start selection
        }

        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            Debug.WriteLine(string.Format(" MouseUp [{0},{1}]", e.ColumnIndex, e.RowIndex));
            endSelectionPoint.X = e.ColumnIndex;
            endSelectionPoint.Y = e.RowIndex;

            //RebuildSelection();
            isSelecting = false; // end selection
        }

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (isSelecting)
            {
                endSelectionPoint = new Point(e.ColumnIndex, e.RowIndex);
                RebuildSelection();
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (isSelecting)
            {
                dataGridView1.ClearSelection();
                SelectionChangedMode(false);
                RebuildSelection();
                SelectionChangedMode(true);
            }
        }

        private void SelectionChangedMode(bool mode)
        {
            if (mode)
                dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            else
                dataGridView1.SelectionChanged -= dataGridView1_SelectionChanged;   
        }

        private void RebuildMinimalSelection()
        {
            if (!IsBothCoordsWithinGrid())
                return;

            dataGridView1[startSelectionPoint.X, startSelectionPoint.Y].Selected = true;
            dataGridView1[endSelectionPoint.X, endSelectionPoint.Y].Selected = true;
        }

        private void RebuildSelection()
        {
            //RebuildMinimalSelection();
            //return;

            if (!IsBothCoordsWithinGrid())
                return;

            Point leftUpper = LeftUpper();
            Point rightLower = RightLower();

            Point currentPoint = leftUpper;
            while (true)
            {
                dataGridView1[currentPoint.X, currentPoint.Y].Selected = true;
                if (currentPoint == rightLower)
                    break;

                currentPoint.X++;
                if (currentPoint.X > (dataGridView1.ColumnCount - 2))
                {
                    currentPoint.X = 1;
                    currentPoint.Y++;
                }
            }
        }

        private bool IsBothCoordsWithinGrid()
        {
            return
                WithinRange(startSelectionPoint.X,1,dataGridView1.ColumnCount - 2) &&
                WithinRange(startSelectionPoint.Y,0,dataGridView1.RowCount-1)  &&
                WithinRange(endSelectionPoint.X,1,dataGridView1.ColumnCount - 2) &&
                WithinRange(endSelectionPoint.Y, 0, dataGridView1.RowCount - 1);
        }

        private static bool WithinRange(int val, int lowBoundary, int highBoundary)
        {
            return (val >= lowBoundary && val <= highBoundary);
        }

        private Point RightLower()
        {
            if (LeftUpper() == startSelectionPoint)
                return endSelectionPoint;
            else
                return startSelectionPoint;
        }

        private Point LeftUpper()
        {
            if (startSelectionPoint.Y < endSelectionPoint.Y ||
                (startSelectionPoint.Y == endSelectionPoint.Y && 
                startSelectionPoint.X <= endSelectionPoint.X))
                return startSelectionPoint;
            else
                return endSelectionPoint;
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new KeyEventHandler(dataGridView1_KeyDown), new object[]{sender,e});
                return;
            }

            int newColumnIndex = dataGridView1.CurrentCell.ColumnIndex;
            int newRowIndex = dataGridView1.CurrentCell.RowIndex;

            switch (e.KeyValue)
            {
                case 0x25: // arrow left
                    Debug.WriteLine("Pressed Arrow left");
                    newColumnIndex--;
                    break;
                case 0x26: // arrow up
                    Debug.WriteLine("Pressed Arrow up");
                    newRowIndex--;
                    break;
                case 0x27: // arrow right
                    Debug.WriteLine("Pressed Arrow right");
                    newColumnIndex++;
                    break;
                case 0x28: // arrow down
                    Debug.WriteLine("Pressed Arrow down");
                    newRowIndex++;
                    break;
                    // use current cell as selection
                default:
                    Debug.WriteLine(string.Format("Keycode '{0}' not used.",e.KeyValue.ToString("X2") ));
                    e.Handled = true;
                    return;
            }

            
            if (newColumnIndex < 1 || newRowIndex < 0 || 
                newColumnIndex > ColumnsPerRow || newRowIndex > (dataGridView1.RowCount-1))
            {
                Debug.WriteLine("Position NOT changed!");
                e.Handled = true;
                return;
            }
            

            //SelectionChangedMode(false);
            dataGridView1.ClearSelection();
            dataGridView1[newColumnIndex, newRowIndex].Selected = true;
            dataGridView1.CurrentCell = dataGridView1.SelectedCells[0];
            //SelectionChangedMode(true);
            e.Handled = true;
        }

        private void dataGridView1_RowUnshared(object sender, DataGridViewRowEventArgs e)
        {
            MessageBox.Show("Unshared");
        }
    }
}
