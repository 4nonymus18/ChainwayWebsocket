using WinFormsApp1.Modul.WebSocket; 
using WinFormsApp1.Helper;
using System.Net.WebSockets;

namespace WinFormsApp1
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.


            //new WebSocketServer(new string[] { "http://localhost:5000/" });

            Console.WriteLine("Welcome");

            UR4API.Initialize();
            UR4API.Connect();

            Task.Run(async () =>
            {
                await SocketServer.Main();
            });

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}