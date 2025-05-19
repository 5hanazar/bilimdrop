using Bilim_Drop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Bilim_Drop
{
    public partial class QuizForm : Form
    {
        private Repository repo;
        private Quiz arg;
        private BindingList<Question> bindList;
        public QuizForm(Repository repo, int id)
        {
            InitializeComponent();
            this.repo = repo;
            myDataGridView1.MultiSelect = false;
            GetData(id);
        }
        private async void GetData(int id)
        {
            if (id == 0) arg = new Quiz(0, true, "", "", "", new Question[] { });
            else arg = await repo.getQuiz(id);
            textBox1.Text = arg.title;
            textBox2.Text = arg.description;
            checkBox1.Checked = arg.active;

            bindList = new BindingList<Question>(new List<Question>(arg.questions));
            myDataGridView1.DataSource = bindList;
        }
        private async void insertOrUpdate()
        {
            try
            {
                var ql = new List<PostQuestion>();
                for(int i = 0; i < bindList.Count; i++)
                {
                    var q = bindList[i];
                    var al = new List<PostAnswer>();
                    for (int j = 0; j < q.answers.Length; j++)
                    {
                        var a = q.answers[j];
                        al.Add(new PostAnswer(j + 1, a.title, a.isCorrect));
                    }
                    ql.Add(new PostQuestion(q.id, i + 1, q.questionType, q.title, al.ToArray()));
                }
                await repo.insertOrUpdateQuiz(new PostQuiz(arg.id, checkBox1.Checked, textBox1.Text, textBox2.Text, ql.ToArray()));
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
        private void New_QuestionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            var s = sender as QuestionForm;
            if (s.DialogResult == DialogResult.OK) {
                bindList.Add(s.argResult);
            };
        }
        private void Edit_QuestionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            var s = sender as QuestionForm;
            if (s.DialogResult == DialogResult.OK)
            {
                bindList[myDataGridView1.SelectedRows[0].Index] = s.argResult;
            };
        }
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addRowButton_Click(sender, e);
        }
        private void addRowButton_Click(object sender, EventArgs e)
        {
            var f = new QuestionForm(repo, new Question((new Random()).Next(100000, 1000000), 0, "", new Answer[] { }));
            f.Text = $"Täze sorag";
            f.StartPosition = FormStartPosition.CenterScreen;
            f.FormClosed += New_QuestionForm_FormClosed;
            f.ShowDialog();
        }
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editRowButton_Click(sender, e);
        }
        private void editRowButton_Click(object sender, EventArgs e)
        {
            if (myDataGridView1.SelectedRows.Count == 0) return;
            var arg = (Question)myDataGridView1.SelectedRows[0].DataBoundItem;
            var f = new QuestionForm(repo, arg);
            f.Text = $"Soragy üýtget";
            f.StartPosition = FormStartPosition.CenterScreen;
            f.FormClosed += Edit_QuestionForm_FormClosed;
            f.ShowDialog();
        }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteRowButton_Click(sender, e);
        }
        private void deleteRowButton_Click(object sender, EventArgs e)
        {
            if (myDataGridView1.SelectedRows.Count == 0) return;
            if (MessageBox.Show("Ýok etmek isleýärsiňizmi?", "Ýok et", MessageBoxButtons.YesNo) == DialogResult.No) return;
            bindList.RemoveAt(myDataGridView1.SelectedRows[0].Index);
        }
    }
}
