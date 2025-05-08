using System.Drawing;
using System.Windows.Forms;

namespace Bilim_Drop
{
    public class MyDataGridView : DataGridView
    {
        public MyDataGridView()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            dataGridViewCellStyle1.SelectionBackColor = Color.FromArgb(215, 236, 255);
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.ControlText;
            dataGridViewCellStyle1.Font = new Font("Arial", 9F);
            DefaultCellStyle = dataGridViewCellStyle1;
            RowHeadersVisible = false;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            AllowUserToResizeRows = false;
            AllowUserToOrderColumns = true;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9F);
            BackgroundColor = SystemColors.ControlLight;
            BorderStyle = BorderStyle.None;
            ReadOnly = true;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            AutoGenerateColumns = true;
            Margin = new Padding(0, 0, 0, 0);
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }
    }
}
