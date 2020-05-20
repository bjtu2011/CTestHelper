/************************************************************************
* Copyright (c) 2020 All Rights Reserved.
*命名空间：CTestHelper.Kernels
*文件名： W119Parse
*创建人： XXX
*创建时间：2020/4/22 19:03:35
*描述
*=======================================================================
*修改标记
*修改时间：2020/4/22 19:03:35
*修改人：XXX
*描述：
************************************************************************/
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CTestHelper.Kernels
{

    class W119Decode : IDecode
    {
        public W119Decode()
        {
        }

        private ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void DataDecode(long instrumentId, string dbTypeof, string dbServer, string dbUser, string dbPwd)
        {
            try
            {
                SampleModel sampleModel = parseDb(dbTypeof, dbServer, dbUser, dbPwd, instrumentId.ToString());
                NSampleModel nSampleModel = new NSampleModel();

                nSampleModel.id = Guid.NewGuid().ToString();
                nSampleModel.sampleModel = sampleModel;
                nSampleModel.status = -1;
                Utils.kernel.Notify_send(nSampleModel);
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
        }

        private SampleModel parseDb(string dbTypeof, string dbServer, string dbUser, string dbPwd, String instrumentId)
        {
            SampleModel sampleModel = null;

            log.Debug("开始监测试验是否结束");
            long times = 0L; //记录循环了多少次
            while (true)
            {
                //如果核心解析线程队列中等待数量大于0    或者  20分钟试验还没结束（新试验未开始）  或者  关机事件执行，则读取数据条数，如果数据条数为3,6,9,则发送
                if (Utils.kernel.Count_decode > 0 || times % 2400 == 0 || Utils.isShutDown)
                {
                    DBHelper dBHelper = new DBHelper();
                    dBHelper.CreateConnection(dbTypeof, dbServer, dbUser, dbPwd);
                    log.Debug("试验结束，开始连接数据库" + dbServer + "读取数据");
                    string sql = "SELECT * from bbsz";
                    DataTable dataTable = dBHelper.ExecuteDataTable(sql);
                    //一直要有编号
                    string sampleNo = "";
                    foreach (DataRow row in dataTable.Rows)
                    {
                        sampleNo = row["报告编号"].ToString();
                    }

                    // while(sampleModel.samples)   //循环：查询出来负荷力、负荷、伸长率、弹性模量中的任何一个不为空。
                    List<string> itemList = getItemList();
                    string sql2 = "SELECT * from nr";
                    DataTable dataTable2 = dBHelper.ExecuteDataTable(sql2);
                    dBHelper.close();
                    List<Dictionary<String, String>> listDict = new List<Dictionary<string, string>>();
                    foreach (DataRow row2 in dataTable2.Rows)
                    {
                        string value = row2["Fp"].ToString();
                        string value2 = row2["Fm"].ToString();
                        string value3 = row2["Agt"].ToString();
                        string value4 = row2["E"].ToString();
                        Dictionary<String, String> dict = new Dictionary<string, string>();
                        if (!"".Equals(value) || !"".Equals(value2) || !"".Equals(value3) || !"".Equals(value4))
                        {
                            dict.Add("qufuli", value);
                            dict.Add("fuhe", value2);
                            dict.Add("shenchanglv", value3);
                            dict.Add("tanxingmoliang", value4);
                            listDict.Add(dict);
                        }
                    }
                    sampleModel = new SampleModel();
                    sampleModel.instrumentId = instrumentId;
                    sampleModel.sampleNo = sampleNo;
                    sampleModel.instrumentName = "W110";
                    if (listDict.Count == 3 || listDict.Count == 6 || listDict.Count == 9)
                    {
                        sampleModel.sampleDataList = listDict;

                        log.Debug("读取完毕，准备发送");

                    }

                    if (Utils.kernel.Count_decode > 0 || Utils.isShutDown)//如果是试验结束或者关机,跳出循环
                    {
                        break;
                    }

                }

                Thread.Sleep(500);
                times++;

            }
            return sampleModel;
        }

        private List<string> getItemList()
        {
            List<string> list = new List<string>();
            list.Add("0.2%屈服力");
            list.Add("破断负荷");
            list.Add("最大力总伸长率");
            list.Add("弹性模量");
            return list;
        }
    }
}
