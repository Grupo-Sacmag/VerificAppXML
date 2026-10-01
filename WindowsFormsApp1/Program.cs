using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var cultureMx = new System.Globalization.CultureInfo("es-MX");
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureMx;
            System.Threading.Thread.CurrentThread.CurrentUICulture = cultureMx;
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureMx;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureMx;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
