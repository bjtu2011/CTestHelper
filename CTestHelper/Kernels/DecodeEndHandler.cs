using System;
using System.Collections.Generic;

namespace CTestHelper.Kernels
{ 
    public delegate void DecodeEndHandler(object sender, int status, Exception ex, string strSampleData, SampleModel sampleModel);
}
