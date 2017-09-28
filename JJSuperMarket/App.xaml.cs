using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace JJSuperMarket
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static System.IO.StreamWriter file = new System.IO.StreamWriter(System.Windows.Forms.Application.StartupPath + "\\test.txt", true);
        public static frmHome frmHome;
        public static frmUserHome frmUserHome;


        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            App.LogWriter("App Started");
            Window frm = new frmLogin(); 
            frm.Show();
        }

        public static void LogWriter(string str)
        {
            file.WriteLine(String.Format("{0:dd/MM/yyyy hh:mm:ss} => {1}\n", DateTime.Now, str));
        }
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            App.LogWriter("App Exit");
            file.Close();
        }
    }
}
