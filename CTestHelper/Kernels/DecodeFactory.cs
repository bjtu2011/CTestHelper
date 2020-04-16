/************************************************************************
* Copyright (c) 2020 All Rights Reserved.
*命名空间：CTestHelper.Kernel
*文件名： DecodeFactory
*创建人： 王华斌
*创建时间：2020/4/10 19:47:01
*描述：解析文件，并上传数据库
*=======================================================================
*修改标记
*修改时间：2020/4/10 19:47:01
*修改人：XXX
*描述：
************************************************************************/

using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace CTestHelper.Kernels
{
    public class DecodeFactory
    {
        private ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public event TaskEndHanlder OnTaskEndEvent;
        W110Decode w110Decode = new W110Decode();
        private static DecodeFactory defaultInstance = new DecodeFactory();


        public static DecodeFactory Instance => defaultInstance;


        public DecodeFactory()
        {
            init();
        }
        public void init()
        {
           
            w110Decode.OneDecodeEnd += oneDecodeEnd;
        }


        private void oneDecodeEnd(object sender, int status, Exception ex, string strSampleData, SampleModel sampleModel)
        {

            //获取天气数据
            String weatherRes = Utils.HttpPost("https://free-api.heweather.net/s6/weather/now?location=auto_ip&key=12d41376e9e34bb18c54b95de9cfb85a", null, "");
            JObject job = JObject.Parse(weatherRes) ;
            JArray jArray = JArray.Parse(job["HeWeather6"].ToString());
            job = (JObject)jArray[0];
            job = JObject.Parse(job["now"].ToString());
            String tmp = job["tmp"].ToString();
            //发送数据
            Dictionary<String, Object> dict = new Dictionary<string, object>();
            dict.Add("jsonData", Utils.ObjToJson<SampleModel>(sampleModel));
            String response = Utils.HttpPostJson("http://127.0.0.1:8080/zgjc/insertData.php?&tmp="+tmp, dict, "");

            //通知界面更新
            
            OnTaskEndEvent(this, sampleModel,response);
            
        }

        public void Decode( Message4Kernel msg)
        {
            log.Debug(Thread.CurrentThread.ManagedThreadId.ToString()+" 开始解析 " + msg.id.ToString());
            switch (msg.fileType)
            {
                case "*.csv":

                    break;

                case "*.mdb":
                    {
                        string dbServer = Path.Combine(msg.filePath, msg.fileName);
                        msg.dbUser = "Admin";
                        msg.dbPwd = "";
                        w110Decode.DataDecode(msg.id, msg.fileType, dbServer, msg.dbUser, msg.dbPwd);
                        break;
                    }
            }
        }

   
    }
}