using CCWin;
using CCWin.SkinControl;
using CTestHelper.Kernels;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace CTestHelper
{
    public partial class MainForm : CCSkinMain
    {
        private ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private FileSystemWatcher fsw;
        private static SampleModel msg;
        private IniFiles ini = new IniFiles(Application.StartupPath + @"\MyConfig.INI");
        Kernel kernel = new Kernel("");
        private Boolean isFirst = true;
        private long curTime = 0L;
        //禁用关闭按钮
        private void banCloseButton()
        {
            Bitmap b1 = new Bitmap(32, 18); //新建位图b1
            Graphics g1 = Graphics.FromImage(b1); //创建b1的Graphics
            g1.FillRectangle(Brushes.Gray, new Rectangle(0, 0, 32, 18)); //把b1涂成灰色
            CloseMouseBack = b1;
            CloseDownBack = b1;
            CloseNormlBack = b1;
        }
      

        public MainForm()
        {
            DecodeFactory.Instance.OnTaskEndEvent += OnTaskEnd;
            InitializeComponent();
        }

        private void OnTaskEnd(object sender, SampleModel msg1)
        {
          msg = msg1;
          logListBox.BeginInvoke(new EventHandler(updateLogListBox), null);
          
        }

        private void updateLogListBox(object sender, EventArgs e)
        {
            //根据检测机器，填入数据
            insertData2LV(ini.IniReadValue("配置", "ChooseInstrument"),msg);


            logListBox.Items.Add(new SkinListBoxItem("处理结束"));
          
        }

        private void insertData2LV(string v,SampleModel sampleModel)
        {
            if(v.Equals("W110"))
            {

                //插入报告编号
                ListViewGroup group1 = new ListViewGroup(sampleModel.sampleNo+"-"+sampleModel.instrumentName);
                dataListView.Groups.Add(group1);
                List<Dictionary<String, String>> ldict = sampleModel.sampleDataList;
                foreach(Dictionary<String,String> dict in ldict)
                {
                    ListViewItem item = new ListViewItem(new string[]
                                                 {sampleModel.sampleNo , dict["屈服力"], dict["负荷"],dict["伸长率"],dict["弹性模量"] }, 0, group1);
                    dataListView.Items.Add(item);
                }
                dataListView.View = View.Details;
               



            }
            return;
        }

        private delegate void renamedDelegate(RenamedEventArgs e);

        private delegate void setLogTextDelegate(FileSystemEventArgs e);
        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            new About().ShowDialog();
        }

        private void ContactAuthorMenuItem_Click(object sender, EventArgs e)
        {
            new ContactAuthor().ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //禁用关闭按钮
            banCloseButton();
            //根据检测机器，生成数据行列
            InitListView(ini.IniReadValue("配置", "ChooseInstrument"));

            //开启监控线程
            try
            {
                String folderPath = ini.IniReadValue("配置", "DataFolder");
                String MonitorFileType = ini.IniReadValue("配置", "MonitorFileType");
                startFileWatcher(MonitorFileType, folderPath);
                logListBox.Items.Add(new SkinListBoxItem("文件监控开启成功"));
            }
            catch (Exception)
            {
                logListBox.Items.Add(new SkinListBoxItem("文件监控开启失败"));
            }
        }

        private void InitListView(string v)
        {
           if(v.Equals("W110"))
            {
                //添加头
                ColumnHeader h1 = new ColumnHeader();

                h1.Text = "报告编号";

                h1.Width = 150;

                ColumnHeader h2 = new ColumnHeader();

                h2.Text = "屈服力";

                ColumnHeader h3 = new ColumnHeader();

                h3.Text = "负荷";

                ColumnHeader h4 = new ColumnHeader();

                h4.Text = "伸长率";

                h4.Width = 150;
                ColumnHeader h5 = new ColumnHeader();

                h5.Text = "弹性模量";

                h5.Width = 150;
                dataListView.Columns.AddRange(new ColumnHeader[] { h1, h2, h3, h4,h5 });
                dataListView.View = View.Details;
            }
        }




        /**
         * filename: 过滤文件类型
         * directoryName：目标文件夹
         * function：开启文件监控
         **/

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            //防止多个文件同时拖入
            if(curTime==0L)
            {
                curTime= Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
               
            }
            else
            {
                long interval = Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds) - curTime;
                if(interval<1000)
                {
                    Thread.Sleep(1000);
                }
            }
            //判断是否第一次
            if(isFirst)
            {
                isFirst = false;
            }
            else
            {
              Utils.isTestEnd = true;
            }
            //定义消息
            Message4Kernel msg4kernel = new Message4Kernel
            {
                id = long.Parse("10717"),
                name = ini.IniReadValue("配置", "ChooseInstrument"),
                filePath = Path.GetDirectoryName(e.FullPath),
                
                fileName = e.Name,
                fileType= ini.IniReadValue("配置", "MonitorFileType")

            };
            //发送消息给kernel
         
            kernel.Notify(msg4kernel);
        }

        private Boolean quit = false;
        private void QuitMenuItem_Click(object sender, EventArgs e)
        {
            Utils.isTestEnd = true;
            quit = true;
            this.Close();
            Application.Exit();
        }

        private void SettingMenuItem_Click(object sender, EventArgs e)
        {
            Setting setting = new Setting();
            setting.Show();
        }


        /**************************************************
        @brief   : 开启文件监控
        @author  : wanghuabin
        @input   ：none
        @output  ：void
        @time    : 2020/04/10
        **************************************************/


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

      

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!quit)
                e.Cancel = true;
            else
                e.Cancel = false;
        }
    }
}