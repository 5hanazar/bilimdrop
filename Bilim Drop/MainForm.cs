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
            StartServer($"http://+:80");
            GetData();
        }

        private async void GetData()
        {
            var l = await repo.getQuizzes();
            myDataGridView1.DataSource = l;
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
    }
}
