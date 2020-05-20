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
using NDatabase.Odb;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace CTestHelper.Kernels
{
    public class DecodeFactory
    {
        private ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IniFiles ini = new IniFiles(Application.StartupPath + @"\MyConfig.INI");

        public event TaskEndHanlder OnTaskEndEvent;

        public event sendProgressHanlder OnSendProgressEvent;//进度事件，当该事件发生，调用主线程的更新界面

        private static DecodeFactory defaultInstance = new DecodeFactory();

        public static DecodeFactory Instance => defaultInstance;

        public DecodeFactory()
        {
        }

        public void SendToServer(NSampleModel nsampleModel)
        {
            SampleModel sampleModel = nsampleModel.sampleModel;
            if (sampleModel != null)
            {
                String tmp = "0";
                //获取天气数据
                String weatherRes = Utils.HttpPost("https://free-api.heweather.net/s6/weather/now?location=auto_ip&key=12d41376e9e34bb18c54b95de9cfb85a", null, "");
                if (weatherRes != null && !weatherRes.Equals(""))
                {
                    JObject job = JObject.Parse(weatherRes);
                    JArray jArray = JArray.Parse(job["HeWeather6"].ToString());
                    job = (JObject)jArray[0];
                    job = JObject.Parse(job["now"].ToString());
                    tmp = job["tmp"].ToString();
                }
                //发送数据
                log.Debug("发送采集到的数据");
                Dictionary<String, Object> dict = new Dictionary<string, object>();
                dict.Add("jsonData", Utils.ObjToJson<SampleModel>(sampleModel));
                String response = Utils.HttpPostJson(ini.IniReadValue("配置", "ServerUrl") + "?tmp=" + tmp, dict, "");
                log.Debug("发送成功，准备更新界面");
                //通知界面更新,如果不在进度条显示
                if (!sampleModel.isProgressShow)
                {
                    OnTaskEndEvent(this, nsampleModel, response);
                }
                else
                {
                   
                    //向progressbar发送消息
                    OnSendProgressEvent(response,nsampleModel);
                  
                }
            }
            else
            {
                OnTaskEndEvent(this, nsampleModel, "null");
                log.Error("发送时sampleModel为null");
            }
        }

        public void Decode(Message4Kernel msg)
        {
            switch (msg.fileType)
            {
                case "*.csv":

                    break;

                case "*.mdb":
                    {
                        string dbServer = Path.Combine(msg.filePath, msg.fileName);
                        if (ini.IniReadValue("配置", "ChooseInstrument").Equals("W110"))
                        {
                            log.Debug(msg.filePath + "：按照W110设备设置进行解析");
                            msg.dbUser = "Admin";
                            msg.dbPwd = "";
                            W110Decode w110Decode = new W110Decode();
                            w110Decode.DataDecode(msg.id, msg.fileType, dbServer, msg.dbUser, msg.dbPwd);
                        }
                        if (ini.IniReadValue("配置", "ChooseInstrument").Equals("W119"))
                        {
                            msg.dbUser = "Admin";
                            msg.dbPwd = "88888888";
                            W119Decode w119Decode = new W119Decode();
                            w119Decode.DataDecode(msg.id, msg.fileType, dbServer, msg.dbUser, msg.dbPwd);
                        }
                        if (ini.IniReadValue("配置", "ChooseInstrument").Equals("W120"))
                        {
                            msg.dbUser = "Admin";
                            msg.dbPwd = "";
                            W120Decode w120Decode = new W120Decode();
                            w120Decode.DataDecode(msg.id, msg.fileType, dbServer, msg.dbUser, msg.dbPwd);
                        }
                        break;
                    }
            }
        }
    }
}