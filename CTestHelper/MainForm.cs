using CCWin;
using CTestHelper.Kernels;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using NDatabase.Odb;
using System.Linq;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;
//监控网络状态
using System.Net.NetworkInformation;
using CCWin.SkinControl;

namespace CTestHelper
{
    public partial class MainForm : CCSkinMain
    {
        private ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private FileSystemWatcher fsw = new FileSystemWatcher();
        private IniFiles ini = new IniFiles(Application.StartupPath + @"\MyConfig.INI");
        private int success = 0;
        private int failed = 0;
        private String title;


        //进度显示
        private int successSend = 0;
        private int failedSend = 0;


        private List<String> lvCheked = new List<string>();//存储选中行的GUID

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

            DecodeFactory.Instance.OnSendProgressEvent += updateProgress;
           
            this.title = this.Text;
            DecodeFactory.Instance.OnTaskEndEvent += OnTaskEnd;
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkAvailabilityChanged);
            
            InitializeComponent();
        }
        public void updateProgress(String response, NSampleModel nSampleModel)
        {
            if(response!=null)
            if (response.Equals("success"))
            {
                //更新数据库中的status
                using (var odb = OdbFactory.Open("samplemodel.dat"))
                {
                    var query = odb.Query<NSampleModel>();
                    query.Descend("id").Constrain(nSampleModel.id).Equal();
                    var nSm = query.Execute<NSampleModel>().GetFirst();
                    nSm.status = 0;
                    odb.Store<NSampleModel>(nSm);
                }
            }
            this.skinProgressBar1.BeginInvoke((Action)delegate
            {
                if (response != null)
                    if (response.Equals("success"))
                    {
                        successSend++;
                    }
                    else
                    {
                        failedSend++;
                    }
                else failedSend++;
                try
                {
                    this.skinProgressBar1.Value = this.skinProgressBar1.Value + 1;
                    if (this.skinProgressBar1.Value >= skinProgressBar1.Maximum)
                    {
                        MessageBox.Show("成功" + successSend + "个，失败" + failedSend + "个");
                        skinProgressBar1.Visible = false;
                        failedSend = 0;
                        
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.ToString());
                }

            });

        }
     
        void NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (e.IsAvailable)
            {
                this.Invoke((Action)delegate { RetrySend(); }) ;

            }
            else
            {
                MessageBox.Show( "网络已断开");
            }
      
        }
        private void OnTaskEnd(object sender, NSampleModel msg1, String response)
        {
            
                if (response == "success")
                {
                    msg1.status = 0;//解析成功，发送成功
                  
                }
                else if(response=="null")
                {
                    msg1.status = -2;//解析失败
                }
                else
                {
                    msg1.status = -1;//解析成功，发送失败
                }
                
                //判断samplemodel是否存在，如果存在则更新，如果不存在则创建

                using (var odb = OdbFactory.Open("samplemodel.dat"))
                {
                var query = odb.Query<NSampleModel>();
                query.Descend("id").Constrain(msg1.id).Equal();
                var nSampleModel = query.Execute<NSampleModel>().GetFirst();
                if (nSampleModel != null)
                {
                    nSampleModel.status = msg1.status;
                    odb.Store<NSampleModel>(nSampleModel);
                }
                else
                    odb.Store<NSampleModel>(msg1);
                }
                dataListView.BeginInvoke(new EventHandler(updateLogListBox), new object[] { msg1});
            
          
        }

        private void updateLogListBox(object sender, EventArgs e)
        {
            if(((NSampleModel)sender).status.Equals(0))
            {
                success++;
            }
            else
            {
                failed++;
            }
            this.Text = this.title + "--成功" + success + "个," + "失败" + failed + "个";
            //根据检测机器，填入数据
            if (((NSampleModel)sender).sampleModel.sampleDataList != null)
                insertData2LV(ini.IniReadValue("配置", "ChooseInstrument"), (NSampleModel)sender);
            else
                logRichTextBox.AppendText(((NSampleModel)sender).sampleModel.sampleNo+":样本数不正确\n");
        }



        private void insertData2LV(string v, NSampleModel nsampleModel)
        {
            SampleModel sampleModel = nsampleModel.sampleModel;
            String statusText = "";
            switch(nsampleModel.status)
            {
                case 0:
                    statusText = "解析成功，发送成功";
                    break;
                case -1:
                    statusText = "解析成功，发送失败";
                    break;
                case -2:
                    statusText = "解析失败，发送失败";
                    break;
                default:
                    statusText = "异常";
                    break;
            }
            if (v.Equals("W110"))
            {
                //插入报告编号
                ListViewGroup group1 = new ListViewGroup(sampleModel.sampleNo + "-" + sampleModel.instrumentName);
                dataListView.Groups.Add(group1);

                List<Dictionary<String, String>> ldict = sampleModel.sampleDataList;
                ListViewItem item = null;

                foreach (Dictionary<String, String> dict in ldict)
                {
                    item = new ListViewItem(new string[]{nsampleModel.id,sampleModel.sampleNo , dict["qufuli"], dict["fuhe"],dict["shenchanglv"],dict["tanxingmoliang"],statusText}, 0, group1);
                    item.Tag = group1;
                    listViewItems.Add(item);
                    dataListView.Items.Add(item);
                }
                dataListView.View = View.Details;
            }
            else if(v.Equals("W120"))
            {
                List<Dictionary<String, String>> ldict = sampleModel.sampleDataList;
                ListViewItem item = null;

                foreach (Dictionary<String, String> dict in ldict)
                {
                    ListViewGroup group1 = new ListViewGroup(sampleModel.sampleNo + "-" + sampleModel.instrumentName);
                    if(!dataListView.Groups.Contains(group1))
                    dataListView.Groups.Add(group1);
                    item = new ListViewItem(new string[]{ nsampleModel.id, sampleModel.sampleNo , dict["试验方法名称"], dict["第几根"],dict["最大负荷"],dict["破断负荷"],dict["抗拉强度"], dict["弹性模量"],dict["屈服强度"], statusText}, 0,group1);
                    item.Tag = group1;
                    listViewItems.Add(item);
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
            _beforeDialogSize = this.Size;
            //禁用关闭按钮
            banCloseButton();
            //根据检测机器，生成数据行列
            InitListView(ini.IniReadValue("配置", "ChooseInstrument"));
            setAutoSize(dataListView);
           
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.notifyIcon1.Visible = true;

            //如果为东北角机器，则直接向kernal发送消息
            if (ini.IniReadValue("配置", "ChooseInstrument").Equals(Utils.EASTERN_SOUTH))
            {
                Message4Kernel msg4kernel = new Message4Kernel
                {
                    id = Utils.EASTERN_SOUTH_ID,
                    name = Utils.EASTERN_SOUTH,
                    filePath = "",
                    fileName = ini.IniReadValue("配置", "DataFolder"),//此处为要监控的文件名
                    fileType = ini.IniReadValue("配置", "MonitorFileType")
                };
                //发送消息给kernel
                Utils.kernel.Notify_decode(msg4kernel);
            }
            else//否则监控文件夹新增*.mdb文件
            {
                //开启监控线程
                try
                {
                    String folderPath = ini.IniReadValue("配置", "DataFolder");
                    String MonitorFileType = ini.IniReadValue("配置", "MonitorFileType");
                    startFileWatcher(MonitorFileType, folderPath);
                    log.Debug("文件监控开启成功");
                    title = this.Text + "-文件监控开启成功";
                    
                }
                catch (Exception)
                {
                    log.Debug("文件监控开启失败");
                    title= this.Text + "-文件监控开启失败";
                   
                }
                this.Text = this.title;
            }

            
            
        }

        /**************************************************
        @brief   : 初始化listview，初始化列
        @author  : none
        @input   ：none
        @output  ：none
        @time    : none
        **************************************************/

        private void InitListView(string v)
        {
            if (v.Equals("W110"))
            {
                //添加头
                ColumnHeader h0 = new ColumnHeader();

                h0.Text = "GUID";

                h0.Tag = 0;
                ColumnHeader h1 = new ColumnHeader();

                h1.Text = "报告编号";

                h1.Tag = 20;

                ColumnHeader h2 = new ColumnHeader();

                h2.Text = "屈服力";
                h2.Tag = 10;
                ColumnHeader h3 = new ColumnHeader();

                h3.Text = "负荷";
                h3.Tag = 10;

                ColumnHeader h4 = new ColumnHeader();

                h4.Text = "伸长率";

                h4.Tag = 10;
                ColumnHeader h5 = new ColumnHeader();

                h5.Text = "弹性模量";

                h5.Tag = 10;
                ColumnHeader h6 = new ColumnHeader();
                h6.Text = "状态";
                h6.Tag = 40;

                dataListView.Columns.AddRange(new ColumnHeader[] {h0, h1, h2, h3, h4, h5, h6 });
            }
            else if(v.Equals("W120"))
            {
                //添加头
                ColumnHeader h0 = new ColumnHeader();

                h0.Text = "GUID";

                h0.Tag = 0;
                ColumnHeader h1 = new ColumnHeader();

                h1.Text = "报告编号";

                h1.Tag = 10;

                ColumnHeader h2 = new ColumnHeader();

                h2.Text = "试验方法名称";
                h2.Tag = 20;
                ColumnHeader h3 = new ColumnHeader();

                h3.Text = "第几根";
                h3.Tag = 10;

                ColumnHeader h4 = new ColumnHeader();

                h4.Text = "最大负荷";

                h4.Tag = 10;
                ColumnHeader h5 = new ColumnHeader();

                h5.Text = "破断负荷";

                h5.Tag = 10;
                ColumnHeader h6 = new ColumnHeader();

                h6.Text = "抗拉强度";

                h6.Tag = 10;
                ColumnHeader h7 = new ColumnHeader();

                h7.Text = "弹性模量";

                h7.Tag = 10;
                ColumnHeader h8 = new ColumnHeader();

                h8.Text = "屈服强度";

                h8.Tag = 10;

                ColumnHeader h9 = new ColumnHeader();
                h9.Text = "状态";
                h9.Tag = 10;

                dataListView.Columns.AddRange(new ColumnHeader[] {h0, h1, h2, h3, h4, h5, h6,h7,h8,h9 });
            }

            dataListView.View = View.Details;
          
        }

        /**
         * filename: 过滤文件类型
         * directoryName：目标文件夹
         * function：开启文件监控
         * 当文件发生变化时进行监控
         **/

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            logRichTextBox.Invoke((Action)delegate
            {
                logRichTextBox.AppendText("文件更改事件发生在" + e.FullPath + ";时间为" + DateTime.Now+"\n");
            });
            //定义消息
            Message4Kernel msg4kernel = new Message4Kernel
            {
                id = long.Parse("10717"),
                name = ini.IniReadValue("配置", "ChooseInstrument"),
                filePath = Path.GetDirectoryName(e.FullPath),

                fileName = e.Name,
                fileType = ini.IniReadValue("配置", "MonitorFileType")
            };
            //发送消息给kernel
            Utils.kernel.Notify_decode(msg4kernel);
        }
        /**
         * filename: 过滤文件类型
         * directoryName：目标文件夹
         * function：开启文件监控
         * 当文件创建时进行监控
         **/

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            logRichTextBox.Invoke((Action)delegate
            {
                logRichTextBox.AppendText("文件创建事件发生在" + e.FullPath + ";时间为" + DateTime.Now + "\n");
            });
            


           
       
        }

        private Boolean quit = false;

        private void QuitMenuItem_Click(object sender, EventArgs e)
        {
            if(Utils.isShutDown)
            {
                //关闭核心处理线程
                Utils.kernel.Close();
                quit = true;
                this.Close();
                Application.Exit();
            }
            else
            {
                MessageBox.Show("试验必须结束才能关闭，请先点击试验结束！", "提示");
            }
 

        }

        private void SettingMenuItem_Click(object sender, EventArgs e)
        {
            Setting setting = new Setting(this);
            setting.Show();
        }

        /**************************************************
        @brief   : 开启文件监控
        @author  : wanghuabin
        @input   ：none
        @output  ：void
        @time    : 2020/04/10
        **************************************************/

        public void startFileWatcher(String fileName, String directoryName)
        {
            fsw.IncludeSubdirectories = false;
            fsw.Filter = fileName;
            fsw.Path = directoryName;
            fsw.NotifyFilter = NotifyFilters.LastWrite;
            fsw.Created += new FileSystemEventHandler(this.OnFileCreated);
            fsw.Changed += new FileSystemEventHandler(this.OnFileChanged);
            fsw.EnableRaisingEvents = true;
            return;
        }
        public void stopFileWatcher()
        {
            fsw.EnableRaisingEvents = false;
            return;
        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!quit)
                e.Cancel = true;
            else
                e.Cancel = false;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
                notifyIcon1.Visible = true;
            }
            else
            {
                notifyIcon1.Visible = false;
                //控件随窗口大小改变
                Size endSize = this.Size;
                float percentWidth = (float)endSize.Width / _beforeDialogSize.Width;
                float percentHeight = (float)endSize.Height / _beforeDialogSize.Height;
                resize(this.Controls,percentWidth,percentHeight);
               
                _beforeDialogSize = endSize;

                setAutoSize(dataListView);
            }
        }
        private void resize(Control.ControlCollection controls,float percentWidth, float percentHeight)
        {
            foreach (Control control in controls)
            {
                if(control.HasChildren)
                {
                    resize(control.Controls, percentWidth, percentHeight);
                }
                //按比例改变控件大小
                control.Width = (int)(control.Width * percentWidth);
                control.Height = (int)(control.Height * percentHeight);

                //为了不使控件之间覆盖 位置也要按比例变化
                control.Left = (int)(control.Left * percentWidth);
                control.Top = (int)(control.Top * percentHeight);
            }
        }


        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            this.TopMost = true;
            this.WindowState = FormWindowState.Normal;
            this.TopMost = false;
        }

        private Size _beforeDialogSize = new Size();

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
        }

        private void setAutoSize(ListView listview)
        {
            foreach (ColumnHeader item in listview.Columns)
            {
                item.Width = (listview.Width / 100) * (int)item.Tag;
            }
        }

        private List<ListViewItem> listViewItems = new List<ListViewItem>();

        private void skinButton1_Click(object sender, EventArgs e)
        {
            //dataListView.Items.Clear();
            //List<ListViewItem> items = listViewItems.FindAll(delegate (ListViewItem s)
            //{
            //    return s.SubItems[0].Text.Contains(this.SearchText.Text);
            //});
        
            //foreach (ListViewItem item in items)
            //{
            //    item.Group = (ListViewGroup)item.Tag;
            //    if (!dataListView.Groups.Contains(item.Group))
            //        dataListView.Groups.Add(item.Group);
            //    dataListView.Items.Add(item);
            //}
            //dataListView.View = View.Details;
        }
        /**
         * 描述：重新发送数据库中失败的数据
         * */
        private void RetrySend()
        {

            //获取数据库中未发送成功的条数，提示是否将发送失败的数据进行发送
            List<NSampleModel> queslst = new List<NSampleModel>();
            using (var odb = OdbFactory.Open("samplemodel.dat"))
            {
                var query = odb.Query<NSampleModel>();
                query.Descend("status").Constrain(-1);
                queslst = query.Execute<NSampleModel>().ToList<NSampleModel>();
                if (queslst.Count > 0)
                {
                    if ((int)MessageBox.Show("网络已连接,数据库中存在" + queslst.Count + "条数据未发送成功，\n是否重新发送？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == 1)
                    {
                        skinProgressBar1.Maximum= queslst.Count;
                        skinProgressBar1.Value = 0;
                        skinProgressBar1.Visible = true;
                        for (int i = 0; i < queslst.Count; i++)
                        {
                            NSampleModel nsampleModel = queslst[i];
                            nsampleModel.sampleModel.isProgressShow = true;
                            Utils.kernel.Notify_send(nsampleModel);
                        }
                  

                       
                    }
                    
                }
                else
                {
                    MessageBox.Show("没有发送失败的数据！","提示");
                }
                
            }
            
        }

        private void dataListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.DrawBackground();
                bool value = false;
                try
                {
                    value = Convert.ToBoolean(e.Header.Tag);
                }
                catch (Exception)
                {
                }
                CheckBoxRenderer.DrawCheckBox(e.Graphics, new Point(e.Bounds.Left, e.Bounds.Top),
                    value ? System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal :
                    System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SystemEvents.SessionEnding -= new SessionEndingEventHandler(Utils.SystemEvents_SessionEnding);
        }

        private void 清空数据库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var odb = OdbFactory.Open("samplemodel.dat"))
            {
                if ((int)MessageBox.Show("是否确认清空数据库，清空后不可恢复！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == 1)
                {
                    foreach (var hero in odb.Query<NSampleModel>().Execute<NSampleModel>())
                    {
                        odb.Delete<NSampleModel>(hero);
                    }
                
                
                }
              
            }
        }
        private void 失败数据重新发送ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RetrySend();
            return;
        }
        private void 试验结束ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (Utils.sync)
            {
                Utils.isShutDown = true;
            }
        }

        private void loadDataButton_Click(object sender, EventArgs e)
        {
            //加载数据
            Dictionary<String, String> dict = new Dictionary<string, string>();
            dict.Add("sampleNo", sampleNoSearchText.Text);
            switch(statusCbx.Text)
            {
                case "":
                    dict.Add("status", "");
                    break;
                case "全部":
                    dict.Add("status", "");
                    break;
                case "解析成功，发送成功":
                    dict.Add("status", "0");
                    break;
                case "解析失败，发送成功":
                    dict.Add("status", "-1");
                    break;
                case "解析失败，发送失败":
                    dict.Add("status", "-2");
                    break;
            }
       
            loadDataFromDb(dict);
        }

        private void loadDataFromDb(Dictionary<String,String> dict)
        {
          
            using (var odb = OdbFactory.Open("samplemodel.dat"))
            {
                var queslst = from nSampleModel in odb.AsQueryable<NSampleModel>()
                              where nSampleModel.sampleModel.sampleNo.Contains(dict["sampleNo"]) && (dict["status"] == ""?true:nSampleModel.status.Equals(Int32.Parse(dict["status"])))
                              select nSampleModel;
              
              
                if (queslst.Count<NSampleModel>() > 0)
                {
                    
                    dataGridVIew.DataSource = queslst.ToList<NSampleModel>();
                }
            }

        }

        private void dataGridVIew_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {


            SkinDataGridView skinDataGridView = (SkinDataGridView)sender;
            String json = (String)skinDataGridView.CurrentCell.EditedFormattedValue;
            try
            {
                SampleModel sampleModel = Utils.JsonToObj<SampleModel>(json);

                using (var odb = OdbFactory.Open("samplemodel.dat"))
                {

                    List<NSampleModel> list = (List<NSampleModel>)skinDataGridView.DataSource;
                    var query = odb.Query<NSampleModel>();
                    query.Descend("id").Constrain(list[skinDataGridView.CurrentRow.Index].id).Equal();
                    var nSm = query.Execute<NSampleModel>().GetFirst();
                    nSm.sampleModel = sampleModel;
                    odb.Store<NSampleModel>(nSm);
                }
                skinDataGridView.CancelEdit();
                loadDataButton.PerformClick();
            }
            catch (Exception err)
            {
                MessageBox.Show("错误：修改格式错误。\n详情：" + err.ToString());
            }


        }

        private void dataGridVIew_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dataGridVIew_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void 发送ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<NSampleModel> list = (List<NSampleModel>)dataGridVIew.DataSource;
            NSampleModel nSampleModel = list[dataGridVIew.CurrentCell.RowIndex];
            nSampleModel.sampleModel.isProgressShow = true;
            skinProgressBar1.BringToFront();
            skinProgressBar1.Maximum = 1;
            skinProgressBar1.Value = 0;
            skinProgressBar1.Visible = true;
            Utils.kernel.Notify_send(nSampleModel);

        }
    }
}