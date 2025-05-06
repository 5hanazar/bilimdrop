using Microsoft.Owin.Hosting;
using System;
using System.Windows.Forms;

namespace Bilim_Drop
{
    public partial class MainForm : Form
    {
        IDisposable server = null;
        public MainForm()
        {
            InitializeComponent();
            StartServer($"http://+:80");
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
