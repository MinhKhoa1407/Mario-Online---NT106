using System;
using System.Windows.Forms;
namespace RoomLauncher
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            string mode = "default";
            if (args.Length > 0)
            {
                mode = args[0]; // Lấy lệnh "create" hoặc "join"
            }
            Application.Run(new Form1(mode));

        }
    }
}