using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTestHelper.DecodeLib
{
    internal class DecodeMessage : InstrumentMessage
    {
        public string strComData
        {
            get;
            set;
        }

        public byte[] comData
        {
            get;
            set;
        }

        public string path
        {
            get;
            set;
        }

        public string fileName
        {
            get;
            set;
        }

        public DecodeMessage(Instrument instrument, string strComData, byte[] comData, string path, string fileName)
        {
            base.type = GetType().ToString();
            base.instrument = instrument;
            this.strComData = strComData;
            this.comData = comData;
            this.path = path;
            this.fileName = fileName;
        }
    }
}
