using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyBanHangWatsons
{
    internal static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [STAThread]
        static void Main()
        {
            
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (Environment.OSVersion.Version.Major >= 6)
            {
                try
                {
                    SetProcessDPIAware();
                }
                catch { }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmDangNhap());
        }
    }
}
