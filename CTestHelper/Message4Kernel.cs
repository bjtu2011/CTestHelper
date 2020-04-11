using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTestHelper
{
    public class Message4Kernel
    {
        /// <summary>
        /// 
        ///
        /// </summary>
        public long id//设备id
        {
            get;
            set;
        }

        public string name//设备名称
        {
            get;
            set;
        }

        public string filePath//文件路径
        {
            get;
            set;
        }
        public string fileType//文件类型
        {
            get;
            set;
        }

        public string fileName//文件路径
        {
            get;
            set;
        }

        //public string fileExt//文件扩展名
        //{
        //    get;
        //    set;
        //}

        public string dbServer//数据库地址
        {
            get;
            set;
        }

        public string dbUser//数据库用户
        {
            get;
            set;
        }

        public string dbPwd//数据库密码
        {
            get;
            set;
        }

        public string dbPort//数据库端口
        {
            get;
            set;
        }

    }
}
