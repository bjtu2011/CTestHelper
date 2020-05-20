using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CCWin;
using CCWin.SkinControl;
using CTestHelper.AllUtils;

namespace CTestHelper
{
    public partial class Setting : CCSkinMain
    {
        IniFiles ini = new IniFiles(Application.StartupPath + @"\MyConfig.INI");
        MainForm main;
       
        public Setting(MainForm mainForm)
        {
            main = mainForm;
            InitializeComponent();
        }

        private void skinPictureBox1_Click(object sender, EventArgs e)
        {
          
            if(ChooseInstrument.Text.Equals(Utils.EASTERN_SOUTH))
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();//打开文件对话框              
                if (InitialDialog(openFileDialog, "Open"))
                {
                    DataFolder.Text = openFileDialog.FileName;
                }
            }
            else
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
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            //判断仪器设备 和 数据文件夹是否为空
            if(DataFolder.Text.Equals("") || ChooseInstrument.Text.Equals("") || MonitorFileType.Text.Equals("") || ServerUrl.Text.Equals(""))
            {
                MessageBox.Show("设置中的项目不能为空！");
                this.DialogResult = DialogResult.None;
            }
            else
            {
                //存储设置信息到INI
                ini.IniWriteValue("配置", "ChooseInstrument", ChooseInstrument.Text);
                ini.IniWriteValue("配置", "DataFolder", DataFolder.Text);
                ini.IniWriteValue("配置", "MonitorFileType", MonitorFileType.Text);
                ini.IniWriteValue("配置", "ServerUrl", ServerUrl.Text);

                main.stopFileWatcher();
                main.startFileWatcher(ini.IniReadValue("配置", "MonitorFileType"), ini.IniReadValue("配置", "DataFolder"));
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void Setting_Load(object sender, EventArgs e)
        {
           
            //设置开机自启动
            new AutoStartUtil().SetMeAutoStart(true);
            ChooseInstrument.Text=ini.IniReadValue("配置", "ChooseInstrument");
            DataFolder.Text = ini.IniReadValue("配置", "DataFolder");
            MonitorFileType.Text=ini.IniReadValue("配置", "MonitorFileType");
            ServerUrl.Text = ini.IniReadValue("配置", "ServerUrl");
        }

        //默认打开路径
        private string InitialDirectory = "D:\\";
        //统一对话框
        private bool InitialDialog(FileDialog fileDialog, string title)
        {
            fileDialog.InitialDirectory = InitialDirectory;//初始化路径
            fileDialog.Filter = "txt files (*.txt,*.*)|*.txt;*.*";//过滤选项设置，文本文件，所有文件。
            fileDialog.FilterIndex = 1;//当前使用第二个过滤字符串
            fileDialog.RestoreDirectory = true;//对话框关闭时恢复原目录
            fileDialog.Title = title;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                for (int i = 1; i <= fileDialog.FileName.Length; i++)
                {
                    if (fileDialog.FileName.Substring(fileDialog.FileName.Length - i, 1).Equals(@"\"))
                    {
                        //更改默认路径为最近打开路径
                        InitialDirectory = fileDialog.FileName.Substring(0, fileDialog.FileName.Length - i + 1);
                        return true;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private void connectTestButton_Click(object sender, EventArgs e)
        {
            String response = Utils.HttpGet(ServerUrl.Text + "?a=connectTest");
            MessageBox.Show(response);
        }
    }
}
