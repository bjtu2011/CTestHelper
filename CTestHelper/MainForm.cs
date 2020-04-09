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
using System.IO;
namespace CTestHelper
{
    public partial class MainForm : CCSkinMain
    {
        IniFiles ini = new IniFiles(Application.StartupPath + @"\MyConfig.INI");
         Queue<String> fileList = new Queue<String>();;
        private delegate void setLogTextDelegate(FileSystemEventArgs e);

        private delegate void renamedDelegate(RenamedEventArgs e);

        FileSystemWatcher fsw;
        public MainForm()
        {
            InitializeComponent();
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            //开启文件处理线程：包括文件解析，数据发送



            //开启监控线程
            String folderPath = ini.IniReadValue("配置", "DataFolder");
            String MonitorFileType= ini.IniReadValue("配置", "MonitorFileType");
            startFileWatcher(MonitorFileType, folderPath);
            
            logListBox.Items.Add(new SkinListBoxItem("测试"));
            
        }
        /**
         * filename: 过滤文件类型
         * directoryName：目标文件夹
         * function：开启文件监控
         **/
        private void startFileWatcher(String fileName, String directoryName)
        {
            fsw = new FileSystemWatcher();
            fsw.IncludeSubdirectories = false;
            fsw.Filter = fileName;
            fsw.Path = directoryName;
            fsw.NotifyFilter = (NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.CreationTime);
            fsw.Created += new FileSystemEventHandler(this.OnFileCreated);
            fsw.EnableRaisingEvents = true;
            return;
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            //添加到文件队列中
            fileList.Enqueue(e.FullPath);
           
        }

        private void SettingMenuItem_Click(object sender, EventArgs e)
        {
            Setting setting = new Setting();
            setting.Show();
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
