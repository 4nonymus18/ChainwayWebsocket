using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Windows.Forms;
using WinFormsApp1.Interfaces;

namespace WinFormsApp1.Helper
{
    internal class UR4API
    {
        // Declare _uhf as a static variable
        public static UHFAPI _UR4 = null;

        // Static method to get the API version

        public static void Initialize()
        {
            _UR4 = UHFAPI.getInstance();
        }

        public static string GetAPIVersion()
        {

            return _UR4.GetAPIVersion();
        }

        public static void Connect()
        {
            bool err = false;
            Console.WriteLine("Connecting to server");

            while (!err)
            {
                err = _UR4.TcpConnect("192.168.86.99", (uint)8888);
                Console.WriteLine("[Connection] Result: " + err);
                Thread.Sleep(2000);
            }

            string status = _UR4.GetTemperature();
            Console.WriteLine("Temperatur: " + status);
            Console.WriteLine("Device ID : " + _UR4.GetUHFGetDeviceID());
            Console.WriteLine("Connected Successfuly!");
        }

        public static void Disconnect(RichTextBox richTextBox)
        {
            Console.WriteLine("Disconnecting from server");
            _UR4.TcpDisconnect();
            richTextBox.AppendText("Disconnected from server");
        }

        public static string[] ScanRFID()
        {
            
            int err = -1;
            int count = 0;
            List<string> IdTagInfo = new List<string>();

            if (!_UR4.GetTemperature().Equals(""))
            {
                //Debug.WriteLine("STOP Scan");
                _UR4.StopGet();
                Task.Delay(5);

                //Debug.WriteLine(err);
                err = Convert.ToInt32(_UR4.Inventory());
                if (err == 1)
                {
                    while (count <= 30)
                    {
                        UHFTAGInfo info = _UR4.uhfGetReceived();

                        if (info != null)
                        {
                            Console.WriteLine(info.Epc.ToString());
                            IdTagInfo.Add(info.Epc.ToString());
                        }

                        Task.Delay(1).Wait();
                        count++;
                    }
                    count = 0;
                }
                _UR4.StopGet();
                err = 0;
            }

            string[] ArrResult = IdTagInfo.ToArray();

            return ArrResult;
        }
    }
}