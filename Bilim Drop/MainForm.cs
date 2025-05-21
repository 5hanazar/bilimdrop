using Bilim_Drop.Models;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
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
            Icon = Icon.ExtractAssociatedIcon("icon.ico");
            myDataGridView1.MultiSelect = false;
            StartServer($"http://+:450");
            GetData();
            GetFilesAsync();
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
            f.Icon = Icon;
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
            f.Icon = Icon;
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

        private async void GetFilesAsync()
        {
            var list = await _getFiles();
            var imgs = new ImageList();
            imgs.ColorDepth = ColorDepth.Depth16Bit;
            imgs.ImageSize = new Size(32, 32);
            listView1.Items.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                var item = new ListViewItem(list[i].name);
                item.ImageIndex = i;
                listView1.Items.Add(item);
                imgs.Images.Add(list[i].image);
            }
            listView1.LargeImageList = imgs;
        }

        private Task<List<FileDto>> _getFiles() => Task.Run(() => {
            var list = new List<FileDto>();
            try
            {
                var files = Directory.GetFiles($"files");
                foreach (var f in files)
                {
                    list.Add(new FileDto(Icon.ExtractAssociatedIcon(f).ToBitmap(), Path.GetFileName(f), ""));
                }
            } catch (Exception e) {}
            return Task.FromResult(list);
        });

        private void buttonFirewall_Click(object sender, EventArgs e)
        {
            if (FirewallRuleExists("Allow Bilim Drop"))
            {
                MessageBox.Show("Firewall rule already exists.", "Firewall");
            } else
            {
                //executeCmd($"netsh advfirewall firewall add rule name=\"Allow Bilim Drop\" dir=in action=allow program=\"{Application.ExecutablePath}\" protocol=TCP localport=450");
                executeCmd($"netsh advfirewall firewall add rule name=\"Allow Bilim Drop\" dir=in action=allow protocol=TCP localport=450");
                MessageBox.Show("New firewall rule was added.", "Firewall");
            }
        }

        private bool FirewallRuleExists(string ruleName)
        {
            return !string.IsNullOrWhiteSpace(executeCmd($"netsh advfirewall firewall show rule name=all | find \"Rule Name:\" | find \"{ruleName}\""));
        }
        private string executeCmd(string cmd)
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {cmd}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }
    }
}
