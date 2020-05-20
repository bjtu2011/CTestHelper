/************************************************************************
* Copyright (c) 2020 All Rights Reserved.
*命名空间：CTestHelper.Kernel
*文件名： W110Parse
*创建人： 王华斌
*创建时间：2020/4/10 19:55:31
*描述：W110机器
*=======================================================================
*修改标记
*修改时间：2020/4/10 19:55:31
*修改人：XXX
*描述：
************************************************************************/

using log4net;
using NDatabase.Odb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace CTestHelper.Kernels
{
    public class W110Decode : IDecode
    {
        public W110Decode()
        {
        }

        private ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void DataDecode(long instrumentId, string dbTypeof, string dbServer, string dbUser, string dbPwd)
        {
            try
            {
                SampleModel sampleModel = parseDb(dbTypeof, dbServer, dbUser, dbPwd, instrumentId.ToString());
                NSampleModel nSampleModel = new NSampleModel();
                if (sampleModel != null)
                {
                    nSampleModel.id = sampleModel.sampleNo;
                    nSampleModel.sampleModel = sampleModel;
                    nSampleModel.status = -1;
                    //判断是否发送到发送线程处理
                    //条件1：如果数据库里存在该样本数据，则进行条件2,否则发送到发送线程处理
                    //条件2：如果发送数据库里的数据与解析的数据相同，则不再发送到发送线程处理，否则发送到发送线程
                    //
                    using (var odb = OdbFactory.Open("samplemodel.dat"))
                    {
                        var query = odb.Query<NSampleModel>();
                        query.Descend("id").Constrain(nSampleModel.id).Equal();
                        var nSampleModel_query = query.Execute<NSampleModel>().GetFirst();
                        if (nSampleModel_query != null)
                        {
                            if (nSampleModel_query != nSampleModel)
                            {
                                nSampleModel_query.status = nSampleModel.status;
                                nSampleModel_query.sampleModel = nSampleModel.sampleModel;
                                odb.Store<NSampleModel>(nSampleModel_query);
                                Utils.kernel.Notify_send(nSampleModel);
                            }
                        }
                        else
                        {
                            odb.Store<NSampleModel>(nSampleModel);
                            Utils.kernel.Notify_send(nSampleModel);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
        }

        private SampleModel parseDb(string dbTypeof, string dbServer, string dbUser, string dbPwd, String instrumentId)
        {
            SampleModel sampleModel = null;
            try
            { 
            log.Debug("读取试验结果数据");
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
            sampleModel.sampleDataList = listDict;
            return sampleModel;
            }
            catch(Exception e)
            {
                return null;
            }
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