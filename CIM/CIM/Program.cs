using System;
using System.Threading;
using System.Windows.Forms;

namespace CIM
{
    internal static class Program
    {
        static readonly Mutex mutex = new Mutex(true, "{b74f4e7a-7326-4b84-8d33-2f2d56bdf1f3cimpc}");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("The program is running!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
