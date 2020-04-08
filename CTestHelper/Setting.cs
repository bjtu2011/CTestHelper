﻿using System;
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
    public partial class Setting : CCSkinMain
    {
       
        public Setting()
        {
            InitializeComponent();
        }

        private void skinPictureBox1_Click(object sender, EventArgs e)
        {
          
            String folderPath=Utils.OpenSelectFolderDialog(this);
            if(folderPath.Equals("0"))
            {
                MessageBox.Show("请选择数据存储文件夹");
            }
            else if(folderPath.Equals("-1"))
            {
                DataFolder.Text = "";
            }
            else
            {
                DataFolder.Text = folderPath;
            }
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            //判断仪器设备 和 数据文件夹是否为空
            if(DataFolder.Text.Equals("") || ChooseInstrument.Text.Equals(""))
            {
                MessageBox.Show("设置项目不能为空！");
                this.DialogResult = DialogResult.None;
            }
            else
            {
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
