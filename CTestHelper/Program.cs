using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CTestHelper
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Setting setting = new Setting();
            setting.ShowDialog();
            if(setting.DialogResult==DialogResult.OK)
            {
                setting.Dispose();
                Application.Run(new MainForm());

            }
            else
            {
                setting.Dispose();
                return;
            }
    
        }
    }
}
