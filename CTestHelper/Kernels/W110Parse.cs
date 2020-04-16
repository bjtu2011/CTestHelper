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

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

namespace CTestHelper.Kernels
{
    public class W110Decode : IDecode
    {
        public W110Decode()
        {
        }

        public event DecodeEndHandler OneDecodeEnd;

        public void DataDecode(long instrumentId, string dbTypeof, string dbServer, string dbUser, string dbPwd)
        {
            try
            {
                DBHelper dBHelper = new DBHelper();
                dBHelper.CreateConnection(dbTypeof, dbServer, dbUser, dbPwd);
                SampleModel sampleModel = parseDb(dBHelper,instrumentId.ToString());
                OneDecodeEnd(this, 0, null, "", sampleModel);

            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
        }

        private SampleModel parseDb(DBHelper db,String instrumentId)
        {
            SampleModel sampleModel;
            //一直要有编号
            string sampleNo = "";
            while (true)
            {
                if (Utils.isTestEnd)
                {
                    string sql = "SELECT * from bbsz";
                    DataTable dataTable = db.ExecuteDataTable(sql);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        sampleNo = row["报告编号"].ToString();
                    }

                    // while(sampleModel.samples)   //循环：查询出来负荷力、负荷、伸长率、弹性模量中的任何一个不为空。
                    List<string> itemList = getItemList();
                    string sql2 = "SELECT * from nr";
                    DataTable dataTable2 = db.ExecuteDataTable(sql2);
                    List<Dictionary<String, String>> listDict = new List<Dictionary<string, string>>();
                    foreach (DataRow row2 in dataTable2.Rows)
                    {
                        string value = row2["Fp"].ToString();
                        string value2 = row2["Fm"].ToString();
                        string value3 = row2["Agt"].ToString();
                        string value4 = row2["E"].ToString();
                        Dictionary<String, String> dict = new Dictionary<string, string>();
                        if (!"".Equals(value) && !"".Equals(value2) && !"".Equals(value3) && !"".Equals(value4))
                        {
                            dict.Add("qufuli", value);
                            dict.Add("fuhe", value2);
                            dict.Add("shenchanglv", value3);
                            dict.Add("tanxingmoliang", value4);
                            listDict.Add(dict);
                        }
                        
                    }
                    sampleModel = new SampleModel(instrumentId, "W110", sampleNo, listDict);
                    Utils.isTestEnd = false;
                    break;
                }

                Thread.Sleep(500);
            }

            db.close();
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