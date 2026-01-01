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
            string username = "unknown";

            if (args.Length >= 1)
                mode = args[0];        // "create" hoặc "join"

            if (args.Length >= 2)
                username = args[1];
            Application.Run(new Form1(mode, username));

        }
    }
}