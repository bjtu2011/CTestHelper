using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CCWin;
using CCWin.SkinControl;
namespace CTestHelper
{
    public partial class MainForm : CCSkinMain
    {
        public MainForm()
        {
            InitializeComponent();
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
          
            
        }

        private void SettingMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void QuitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            new About().ShowDialog();
        }

        private void ContactAuthorMenuItem_Click(object sender, EventArgs e)
        {
            new ContactAuthor().ShowDialog();
        }
    }
}
