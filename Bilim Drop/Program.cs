using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Bilim_Drop
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [STAThread]
        static void Main()
        {
            bool createdNew = true;
            using (System.Threading.Mutex mutex = new System.Threading.Mutex(true, Application.ProductName, out createdNew))
            {
                if (createdNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainForm());
                }
                else
                {
                    Process current = Process.GetCurrentProcess();
                    foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                    {
                        if (process.Id != current.Id) { SetForegroundWindow(process.MainWindowHandle); break; }
                    }
                }
            }
        }
    }
}
