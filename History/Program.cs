using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace History
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string uid = "";

            if (args.Length > 0)
                uid = args[0];      // Lấy UID từ C++ truyền sang
            else
                MessageBox.Show("Không nhận được UID!");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new HistoryForm(uid));
        }
    }
}
