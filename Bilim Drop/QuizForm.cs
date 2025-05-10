using Bilim_Drop.Models;
using System;
using System.Windows.Forms;

namespace Bilim_Drop
{
    public partial class QuizForm : Form
    {
        private Repository repo;
        private Quiz arg;
        public QuizForm(Repository repo, int id)
        {
            InitializeComponent();
            this.repo = repo;
            GetData(id);
        }
        private async void GetData(int id)
        {
            if (id == 0) arg = new Quiz(0, true, "", "", "");
            else arg = await repo.getQuiz(id);
            textBox1.Text = arg.title;
            textBox2.Text = arg.description;
            checkBox1.Checked = arg.active;
        }
        private async void insertOrUpdate()
        {
            try
            {
                await repo.insertOrUpdateQuiz(new PostQuiz(arg.id, checkBox1.Checked, textBox1.Text, textBox2.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }
        private void okButton_Click(object sender, EventArgs e)
        {
            insertOrUpdate();
        }

        private void QuizForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (arg.active != checkBox1.Checked
                || arg.title != textBox1.Text
                || arg.description != textBox2.Text)
            {
                if (DialogResult != DialogResult.OK && MessageBox.Show("Üýtgemeler ýiter?", "Ýap", MessageBoxButtons.YesNo) == DialogResult.No) e.Cancel = true;
            }
        }
    }
}
