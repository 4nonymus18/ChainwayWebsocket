using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace UHFAPP
{
    static class Program
    {
        static void Main()
        {

            // Wait for user input before closing the console window
            Test();
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        // some function to call class UHFAPI
        public static void Test()
        {
            UHFAPI uhf = new UHFAPI();
            var deviceInfoList = uhf.LinkGetDeviceInfo();

            foreach (var item in deviceInfoList)
            {
                Console.WriteLine(item);
            }
        }
    }
}
