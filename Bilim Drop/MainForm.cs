using Bilim_Drop.Models;
using Microsoft.Owin.Hosting;
using System;
using System.Windows.Forms;

namespace Bilim_Drop
{
    public partial class MainForm : Form
    {
        IDisposable server = null;
        Repository repo = new RepositoryImpl();
        public MainForm()
        {
            InitializeComponent();
            StartServer($"https://+:450");
            GetData();
        }

        private void StartServer(string url)
        {
            try
            {
                server = WebApp.Start<Startup>(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private async void GetData()
        {
            var b1 = 0; var b2 = 0;
            if (myDataGridView1.RowCount > 0)
            {
                b1 = myDataGridView1.SelectedRows[0].Index;
                b2 = myDataGridView1.FirstDisplayedScrollingRowIndex;
            }

            var l = await repo.getQuizzes();
            myDataGridView1.DataSource = l;

            if (myDataGridView1.RowCount > 0)
            {
                if (b1 >= myDataGridView1.RowCount) b1 = myDataGridView1.RowCount - 1;
                myDataGridView1.ClearSelection();
                myDataGridView1.Rows[b1].Selected = true;
                myDataGridView1.FirstDisplayedScrollingRowIndex = b2;
            }
        }

        private void F_FormClosed(object sender, FormClosedEventArgs e)
        {
            var s = sender as Form;
            if (s.DialogResult == DialogResult.OK) GetData();
        }

        private void myDataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            myDataGridView1.AutoResizeColumns();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addToolStripButton_Click(sender, e);
        }
        private void addToolStripButton_Click(object sender, EventArgs e)
        {
            var f = new QuizForm(repo, 0);
            f.Text = $"Täze test";
            f.StartPosition = FormStartPosition.CenterScreen;
            f.FormClosed += F_FormClosed;
            f.ShowDialog();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editToolStripButton_Click(sender, e);
        }
        private void editToolStripButton_Click(object sender, EventArgs e)
        {
            if (myDataGridView1.SelectedRows.Count == 0) return;
            var arg = (Quiz)myDataGridView1.SelectedRows[0].DataBoundItem;
            var f = new QuizForm(repo, arg.id);
            f.Text = $"Testi üýtget";
            f.StartPosition = FormStartPosition.CenterScreen;
            f.FormClosed += F_FormClosed;
            f.ShowDialog();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteToolStripButton_Click(sender, e);
        }
        private void deleteToolStripButton_Click(object sender, EventArgs e)
        {
            delete();
        }
        private async void delete()
        {
            if (myDataGridView1.SelectedRows.Count == 0) return;
            if (MessageBox.Show("Ýok etmek isleýärsiňizmi?", "Ýok et", MessageBoxButtons.YesNo) == DialogResult.No) return;
            var arg = (Quiz)myDataGridView1.SelectedRows[0].DataBoundItem;
            try
            {
                await repo.deleteQuiz(arg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            GetData();
        }
    }
}
