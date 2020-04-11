using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTestHelper.DecodeLib
{
    class Instrument
    {
        public long id
        {
            get;
            set;
        }

        public string code
        {
            get;
            set;
        }

        public string name
        {
            get;
            set;
        }

        public InterTypeEnum interTypeof
        {
            get;
            set;
        }

        public string filePath
        {
            get;
            set;
        }

        public string fileExt
        {
            get;
            set;
        }

        public bool fileNeedUpload
        {
            get;
            set;
        }

        public string fileUploadUrl
        {
            get;
            set;
        }

        public string dbTypeof
        {
            get;
            set;
        }

        public string dbServer
        {
            get;
            set;
        }

        public string dbUser
        {
            get;
            set;
        }

        public string dbPwd
        {
            get;
            set;
        }

        public string comPort
        {
            get;
            set;
        }

        public string comBaudrate
        {
            get;
            set;
        }

        public string comDatabits
        {
            get;
            set;
        }

        public string comParity
        {
            get;
            set;
        }

        public string comStopbits
        {
            get;
            set;
        }

        public string tcpIp
        {
            get;
            set;
        }

        public string tcpPort
        {
            get;
            set;
        }

        public string decodeLib
        {
            get;
            set;
        }
    }
}
