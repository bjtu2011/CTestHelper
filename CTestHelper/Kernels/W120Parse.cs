/************************************************************************
* Copyright (c) 2020 All Rights Reserved.
*命名空间：CTestHelper.Kernels
*文件名： W120Parse
*创建人： XXX
*创建时间：2020/4/24 16:12:20
*描述
*=======================================================================
*修改标记
*修改时间：2020/4/24 16:12:20
*修改人：XXX
*描述：
************************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CTestHelper.Kernels
{
    class W120Decode : IDecode
    {
        public event DecodeEndHandler OneDecodeEnd;
        public void DataDecode(long instrumentId, string dbTypeof, string dbServer, string dbUser, string dbPwd)
        {
            try
            {
                DBHelper dBHelper = new DBHelper();
                dBHelper.CreateConnection(dbTypeof, dbServer, dbUser, dbPwd);
                parseDb(dBHelper, instrumentId.ToString());
                

            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
        }

        private void parseDb(DBHelper db, String instrumentId)
        {
            SampleModel sampleModel;
            //一直要有编号
            int temp = 2147483647;
            while (true)
            {
                if (!Utils.isShutDown)
                {
                    //获取条数
                    string sql = "SELECT count(*)  from GBT228LA2010";
                    DataTable dataTable = db.ExecuteDataTable(sql);
                    //如果本次条数大于上次条数
                    int current = (int)dataTable.Rows[0][0];
                    
                    if (current > temp)
                    //获取最新插入的数据
                    {
                        sql = "select Top "+(current-temp)+" * from GBT228LA2010 order by 试验数据ID号 desc";
                        dataTable = db.ExecuteDataTable(sql);
                        sampleModel = new SampleModel();
                        sampleModel.instrumentId = instrumentId;
                        sampleModel.instrumentName = "W120";
                        sampleModel.sampleNo = dataTable.Rows[0]["试验数据名称"].ToString();
                        sampleModel.sampleDataList = Utils.ToJson(dataTable);
                        OneDecodeEnd(this, 0, null, "", sampleModel);
                        temp = current;
                    }
                    else
                    {
                        temp = current;
                    }
                    
                }
                else
                {
                    break;
                }

                Thread.Sleep(500);
            }

            db.close();
          
        }

    }
}
