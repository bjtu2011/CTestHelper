using CTestHelper.Kernels;
using log4net;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
namespace CTestHelper
{
    class Utils
    {
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        

        public static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            lock(sync)
            { 
            isShutDown=true;
            }
        }

        public static Boolean isTestEnd = true;//判断实验是否做完
        public static Boolean isShutDown = false;//判断是否关机
        public static object sync=new object ();//锁定isShutDown
        public static String EASTERN_SOUTH = "W120";
        public static long EASTERN_SOUTH_ID = 88888L;

        //初始化处理核心
        public static Kernel kernel = new Kernel("");
        public static string Md5(string src)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(src);
            MD5 mD = new MD5CryptoServiceProvider();
            byte[] value = mD.ComputeHash(bytes);
            return BitConverter.ToString(value).Replace("-", "");
        }

        public static string GetMacByNetworkInterface()
        {
            string result = "";
            NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            NetworkInterface[] array = allNetworkInterfaces;
            foreach (NetworkInterface networkInterface in array)
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    result = networkInterface.GetPhysicalAddress().ToString();
                    break;
                }
            }
            return result;
        }

        public static Dictionary<string, string> GetIpconfig()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("machineName", Environment.MachineName);
            NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            NetworkInterface[] array = allNetworkInterfaces;
            foreach (NetworkInterface networkInterface in array)
            {
                if (networkInterface.OperationalStatus != OperationalStatus.Up || networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback || dictionary.ContainsKey("mac"))
                {
                    continue;
                }
                dictionary.Add("mac", networkInterface.GetPhysicalAddress().ToString());
                IPInterfaceProperties iPProperties = networkInterface.GetIPProperties();
                for (int j = 0; j < iPProperties.UnicastAddresses.Count; j++)
                {
                    if (iPProperties.UnicastAddresses[j].Address.IsIPv6LinkLocal)
                    {
                        if (!dictionary.ContainsKey("ipv6"))
                        {
                            dictionary.Add("ipv6", iPProperties.UnicastAddresses[j].Address.ToString());
                        }
                    }
                    else if (!dictionary.ContainsKey("ipv4"))
                    {
                        dictionary.Add("ipv4", iPProperties.UnicastAddresses[j].Address.ToString());
                    }
                }
            }
            return dictionary;
        }

        public static bool PortInUse(int port)
        {
            bool result = false;
            IPGlobalProperties iPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] activeTcpListeners = iPGlobalProperties.GetActiveTcpListeners();
            IPEndPoint[] array = activeTcpListeners;
            foreach (IPEndPoint iPEndPoint in array)
            {
                if (iPEndPoint.Port == port)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public static List<string> GetPrints()
        {
            PrintDocument printDocument = new PrintDocument();
            string printerName = printDocument.PrinterSettings.PrinterName;
            List<string> list = new List<string>();
            foreach (string installedPrinter in PrinterSettings.InstalledPrinters)
            {
                list.Add(installedPrinter);
            }
            return list;
        }

        public static string GetDefaultPrint()
        {
            PrintDocument printDocument = new PrintDocument();
            return printDocument.PrinterSettings.PrinterName;
        }

        public static string HttpDownload(string url, string destFile, string method)
        {
            try
            {
                using (FileStream fileStream = new FileStream(destFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
                    httpWebRequest.Method = method;
                    WebResponse response = httpWebRequest.GetResponse();
                    using (Stream stream = response.GetResponseStream())
                    {
                        byte[] array = new byte[1024];
                        for (int num = stream.Read(array, 0, array.Length); num > 0; num = stream.Read(array, 0, array.Length))
                        {
                            fileStream.Write(array, 0, num);
                        }
                        stream.Close();
                    }
                    fileStream.Close();
                }
                return destFile;
            }
            catch (Exception exception)
            {
                log.Error("HttpDownload ERROR", exception);
                return "";
            }
        }

        public static string HttpDownloadToTemp(string url, string fileExt)
        {
            return HttpDownloadToTemp(url, fileExt, "GET");
        }

        public static string HttpDownloadToTemp(string url, string fileExt, string method)
        {
            string tempFileName = Path.GetTempFileName();
            string text = Path.ChangeExtension(tempFileName, fileExt);
            File.Move(tempFileName, text);
            return HttpDownload(url, text, method);
        }

        public static string HttpDownloadString(string url)
        {
            try
            {
                string result = "";
                HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
                WebResponse response = httpWebRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader streamReader = new StreamReader(stream);
                    result = streamReader.ReadToEnd();
                    stream.Close();
                }
                return result;
            }
            catch (Exception exception)
            {
                log.Error("HttpDownloadString ERROR", exception);
                return null;
            }
        }

        public static Stream HttpDownloadStream(string url)
        {
            try
            {
                Stream stream = new MemoryStream();
                HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
                WebResponse response = httpWebRequest.GetResponse();
                using (Stream stream2 = response.GetResponseStream())
                {
                    byte[] array = new byte[1024];
                    for (int num = stream2.Read(array, 0, array.Length); num > 0; num = stream2.Read(array, 0, array.Length))
                    {
                        stream.Write(array, 0, num);
                    }
                    stream2.Close();
                }
                stream.Close();
                return stream;
            }
            catch (Exception exception)
            {
                log.Error("HttpDownloadStream ERROR", exception);
                return null;
            }
        }

        public static string UrlDecode(string s)
        {
            string[] array = new string[8]
            {
                "%",
                "+",
                " ",
                "/",
                "?",
                "#",
                "&",
                "="
            };
            string[] array2 = new string[8]
            {
                "%25",
                "%2B",
                "%20",
                "%2F",
                "%3F",
                "%23",
                "%26",
                "%3D"
            };
            string text = s;
            for (int i = 0; i < array.Length; i++)
            {
                text = text.Replace(array[i], array2[i]);
            }
            return text;
        }

        public static string HttpPostJson(string url, Dictionary<string, object> parameters, string authorization)
        {
            try
            {
                string text = "";
                HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
                httpWebRequest.Method = "POST";
                if (parameters != null && parameters.Count != 0)
                {
                    string text2 = JsonConvert.SerializeObject(parameters);
                    log.Info("http post request url:" + url);
                    log.Info("http post request ContentLength:" + text2.Length.ToString());
                    log.Debug("http post request data:" + text2);
                    httpWebRequest.ContentType = "application/json;charset=utf-8;";
 //                   httpWebRequest.ContentLength = text2.Length;
                    httpWebRequest.Headers.Add("Authorization", authorization);
                    using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(text2);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }
                WebResponse response = httpWebRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader streamReader = new StreamReader(stream);
                    text = streamReader.ReadToEnd();
                    stream.Close();
                }
                log.Info("http post response:" + text);
                return text;
            }
            catch (Exception exception)
            {
                log.Error("http post ERROR", exception);
                return null;
            }
        }
        /// <summary>
        /// HTTP GET方式请求数据.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 5000;
            request.AllowAutoRedirect = false;

            WebResponse response = null;
            string responseStr = null;

            try
            {
                response = request.GetResponse();

                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                responseStr="连接超时，请检查网络连接或者服务器地址是否正确！";
            }
            finally
            {
                request = null;
                response = null;
            }

            return responseStr;
        }
        public static string HttpPost(string url, Dictionary<string, object> parameters)
        {
            return HttpPost(url, parameters, "application/x-www-form-urlencoded;charset=utf-8;");
        }

        public static string HttpPost(string url, Dictionary<string, object> parameters, string contentType)
        {
            try
            {
                string text = "";
                HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
                httpWebRequest.Method = "POST";
                if (parameters != null && parameters.Count != 0)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    int num = 0;
                    foreach (string key in parameters.Keys)
                    {
                        if (parameters.ContainsKey(key) && parameters[key] != null)
                        {
                            string s = parameters[key].ToString();
                            s = UrlDecode(s);
                            if (num > 0)
                            {
                                stringBuilder.AppendFormat("&{0}={1}", key, s);
                            }
                            else
                            {
                                stringBuilder.AppendFormat("{0}={1}", key, s);
                            }
                            num++;
                        }
                    }
                    byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    log.Info("http post request url:" + url);
                    log.Info("http post request ContentLength:" + bytes.Length.ToString());
                    log.Debug("http post request data:" + stringBuilder.ToString());
                    if (string.IsNullOrEmpty(contentType))
                    {
                        httpWebRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8;";
                    }
                    else
                    {
                        httpWebRequest.ContentType = contentType;
                    }
                    httpWebRequest.ContentLength = bytes.Length;
                    using (Stream stream = httpWebRequest.GetRequestStream())
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                        stream.Close();
                    }
                }
                WebResponse response = httpWebRequest.GetResponse();
                using (Stream stream2 = response.GetResponseStream())
                {
                    StreamReader streamReader = new StreamReader(stream2);
                    text = streamReader.ReadToEnd();
                    stream2.Close();
                }
                log.Info("http post response:" + text);
                return text;
            }
            catch (Exception exception)
            {
                log.Error(url + " http post ERROR", exception);
                return null;
            }
        }

        public static string HttpPostByWebClient(string url, Dictionary<string, object> parameters)
        {
            try
            {
                string text = "";
                if (parameters != null && parameters.Count != 0)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    int num = 0;
                    foreach (string key in parameters.Keys)
                    {
                        if (num > 0)
                        {
                            stringBuilder.AppendFormat("&{0}={1}", key, parameters[key]);
                        }
                        else
                        {
                            stringBuilder.AppendFormat("{0}={1}", key, parameters[key]);
                        }
                        num++;
                    }
                    byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    log.Info("http post request url:" + url);
                    log.Info("http post request ContentLength:" + bytes.Length.ToString());
                    log.Debug("http post request data:" + stringBuilder.ToString());
                    WebClient webClient = new WebClient();
                    webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded;charset=utf-8;");
                    byte[] bytes2 = webClient.UploadData(url, "POST", bytes);
                    text = Encoding.UTF8.GetString(bytes2);
                    log.Info("http post response:" + text);
                }
                return text;
            }
            catch (Exception exception)
            {
                log.Error("http post ERROR", exception);
                return null;
            }
        }

        public static string UploadFile(string url, string filepath)
        {
            if (File.Exists(filepath))
            {
                try
                {
                    WebClient webClient = new WebClient();
                    byte[] bytes = webClient.UploadFile(url, "POST", filepath);
                    return Encoding.UTF8.GetString(bytes);
                }
                catch (Exception exception)
                {
                    log.Error("UploadFile ERROR", exception);
                    return null;
                }
            }
            return null;
        }

        public static string UploadData(string url, Dictionary<string, object> parameters, string fileName, byte[] uploadData)
        {
            Encoding uTF = Encoding.UTF8;
            if (uploadData != null && uploadData.Length != 0)
            {
                try
                {
                    string str = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                    WebClient webClient = new WebClient();
                    webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + str);
                    string s = "--" + str + "--\r\n";
                    byte[] bytes = uTF.GetBytes(s);
                    ArrayList arrayList = new ArrayList();
                    string format = "--" + str + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
                    if (parameters != null)
                    {
                        foreach (string key in parameters.Keys)
                        {
                            string s2 = string.Format(format, key, parameters[key]);
                            arrayList.Add(uTF.GetBytes(s2));
                        }
                    }
                    string s3 = "\r\n";
                    format = "--" + str + "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                    string arg = "application/octet-stream";
                    string s4 = string.Format(format, "file", fileName, arg);
                    byte[] bytes2 = uTF.GetBytes(s4);
                    byte[] bytes3 = uTF.GetBytes(s3);
                    byte[] array = new byte[bytes2.Length + uploadData.Length + bytes3.Length];
                    bytes2.CopyTo(array, 0);
                    uploadData.CopyTo(array, bytes2.Length);
                    bytes3.CopyTo(array, bytes2.Length + uploadData.Length);
                    arrayList.Add(array);
                    arrayList.Add(bytes);
                    int num = 0;
                    foreach (byte[] item in arrayList)
                    {
                        num += item.Length;
                    }
                    byte[] array3 = new byte[num];
                    int num2 = 0;
                    foreach (byte[] item2 in arrayList)
                    {
                        item2.CopyTo(array3, num2);
                        num2 += item2.Length;
                    }
                    byte[] array5;
                    try
                    {
                        log.Debug("UploadData data length:" + array3.Length.ToString());
                        array5 = webClient.UploadData(url, "POST", array3);
                        string @string = uTF.GetString(array5);
                    }
                    catch (WebException ex)
                    {
                        Stream responseStream = ex.Response.GetResponseStream();
                        array5 = new byte[ex.Response.ContentLength];
                        responseStream.Read(array5, 0, array5.Length);
                    }
                    return uTF.GetString(array5);
                }
                catch (Exception exception)
                {
                    log.Error("UploadData ERROR", exception);
                    return null;
                }
            }
            return null;
        }

        public static string readAppConfig(string name)
        {
            string result = "";
    
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (configuration.AppSettings.Settings[name] != null)
            {
                result = configuration.AppSettings.Settings[name].Value;
            }
            return result;
        }

        public static void writeAppConfig(string name, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (configuration.AppSettings.Settings[name] != null)
            {
                configuration.AppSettings.Settings[name].Value = value;
            }
            else
            {
                configuration.AppSettings.Settings.Add(name, value);
            }
            configuration.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public static string GetDefaultPrinterByWMI()
        {
            string queryString = "SELECT * FROM Win32_Printer WHERE Default=true";
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(queryString);
            ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
            if (managementObjectCollection != null)
            {
                using (ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator = managementObjectCollection.GetEnumerator())
                {
                    if (managementObjectEnumerator.MoveNext())
                    {
                        ManagementObject managementObject = (ManagementObject)managementObjectEnumerator.Current;
                        return managementObject["Name"].ToString();
                    }
                }
                return string.Empty;
            }
            return string.Empty;
        }

        public static void SetDefaultPrinterByWMI(string PrinterName)
        {
            string queryString = "SELECT * FROM Win32_Printer";
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(queryString);
            ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
            if (managementObjectCollection != null && managementObjectCollection.Count > 0)
            {
                foreach (ManagementObject item in managementObjectCollection)
                {
                    if (string.Compare(item["Name"].ToString(), PrinterName, ignoreCase: true) == 0)
                    {
                        item.InvokeMethod("SetDefaultPrinter", null);
                        break;
                    }
                }
            }
        }

        public static String OpenSelectFolderDialog_EMPTY = "0";
        public static String OpenSelectFolderDialog_CANCEL = "-1";
        public static String OpenSelectFolderDialog(Form form)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择数据生成文件夹";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    MessageBox.Show(form, "文件夹路径不能为空", "提示");
                    return OpenSelectFolderDialog_EMPTY;
                }
                else
                {
                    return dialog.SelectedPath;
                }
              
            }
            return OpenSelectFolderDialog_CANCEL;
        }
        public static string ObjToJson<T>(T t)
        {
            return JsonConvert.SerializeObject(t);
        }

        public static T JsonToObj<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static List<Dictionary<String,String>> ToJson( DataTable dt)
        {

            List<Dictionary<String, String>> listDict = new List<Dictionary<string, string>>();
            foreach (DataRow dataRow in dt.Rows)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();  //实例化一个参数集合
                foreach (DataColumn dataColumn in dt.Columns)
                {
                    dictionary.Add(dataColumn.ColumnName, dataRow[dataColumn.ColumnName].ToString());
                }
                listDict.Add(dictionary); //ArrayList集合中添加键值
            }

            return listDict;  //返回一个json字符串
        }


    }
}
