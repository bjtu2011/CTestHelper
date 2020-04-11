using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTestHelper.Kernels
{
    public interface IDecode
    {
         event DecodeEndHandler OneDecodeEnd;
        void DataDecode(long instrumentId, string dbTypeof, string dbServer, string dbUser, string dbPwd);

    }
}
