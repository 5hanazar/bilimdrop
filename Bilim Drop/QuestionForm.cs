using Bilim_Drop.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Bilim_Drop
{
    public partial class QuestionForm : Form
    {
        private Repository repo;
        private Question arg;
        public Question argResult;

        public QuestionForm(Repository repo, Question arg)
        {
            InitializeComponent();
            this.repo = repo;
            this.arg = arg;
            myDataGridView1.ReadOnly = false;
            myDataGridView1.MultiSelect = false;
            myDataGridView1.AllowUserToAddRows = true;
            myDataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;

            textBox1.Text = arg.id.ToString();
            textBox2.Text = arg.title;
            for (int i = 0; i < arg.answers.Length; i++)
            {
                var index = myDataGridView1.Rows.Add();
                myDataGridView1.Rows[index].Cells["colTitle"].Value = arg.answers[i].title;
                myDataGridView1.Rows[index].Cells["colCorrect"].Value = arg.answers[i].isCorrect;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            myDataGridView1.EndEdit();
            var list = new List<Answer>();
            for (int i = 0; i < myDataGridView1.Rows.Count; i++)
            {
                var c1 = myDataGridView1.Rows[i].Cells["colTitle"];
                if (c1.Value == null || string.IsNullOrWhiteSpace(c1.Value.ToString())) continue;
                var c2 = myDataGridView1.Rows[i].Cells["colCorrect"];
                var item = new Answer(i + 1, c1.Value.ToString(), c2.Value == null ? false : bool.Parse(c2.Value.ToString()));
                list.Add(item);
            }
            argResult = new Question(int.Parse(textBox1.Text), 0, textBox2.Text, list.ToArray());
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
