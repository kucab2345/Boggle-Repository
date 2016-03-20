using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoggleClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Application.Run(new Form1());
            //ApplicationContext version of launcher which allows for multiwindow setup
            var context = GameApplicationContext.GetContext();
            GameApplicationContext.GetContext().RunNew();
            Application.Run(context);
        }
    }
}
