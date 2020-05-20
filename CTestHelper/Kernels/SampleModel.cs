/************************************************************************
* Copyright (c) 2020 All Rights Reserved.
*命名空间：CTestHelper.Kernel
*文件名： SampleModel
*创建人： XXX
*创建时间：2020/4/10 20:02:32
*描述
*=======================================================================
*修改标记
*修改时间：2020/4/10 20:02:32
*修改人：XXX
*描述：
************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTestHelper.Kernels
{
   public class SampleModel
    {
        public string sampleNo { get; set; }

        public string instrumentId { get; set; }
        public string instrumentName { get; set; }

        public Boolean isProgressShow = false;

        public List<Dictionary<string, string>> sampleDataList;
 
        public SampleModel()
        {
          
        }
        public override String ToString()
        { 
            return Utils.ObjToJson<SampleModel>(this);
        }
        //string  -> SampleModel
        public static explicit operator SampleModel(String x)
         {
            return Utils.JsonToObj<SampleModel>(x);
         }

}
}
