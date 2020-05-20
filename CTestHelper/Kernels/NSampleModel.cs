/************************************************************************
* Copyright (c) 2020 All Rights Reserved.
*命名空间：CTestHelper.Kernels
*文件名： NSampleModel
*创建人： 王华斌
*创建时间：2020/4/28 14:12:09
*描述：该文件为存储到数据库的SampleModel，包含了SampleModel数据，id，发送状态
*=======================================================================
*修改标记
*修改时间：2020/4/28 14:12:09
*修改人：王华斌
*描述：
************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTestHelper.Kernels
{
    public class NSampleModel
    {
        public String id { get; set; }
        public SampleModel sampleModel { get; set; }
        //错误码：
        //-2:解析失败或者数据表为空
        //-1：发送失败，网络出现问题
        //0：解析成功，发送成功
        public int status { get; set; }

        public static Boolean operator ==(NSampleModel nSampleModel1, NSampleModel nSampleModel2)
        {
            if ((nSampleModel1 as object) != null && (nSampleModel2 as object) != null)
            {
                if (nSampleModel1.id == nSampleModel2.id &&
                     nSampleModel1.sampleModel.ToString() == nSampleModel2.sampleModel.ToString())
                    return true;
                else
                    return false;
            }
            else if ((nSampleModel1 as object) == null && (nSampleModel2 as object) == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static Boolean operator !=(NSampleModel nSampleModel1, NSampleModel nSampleModel2)
        {
            if((nSampleModel1 as object)!=null && (nSampleModel2 as object)!=null)
            { 
            if (nSampleModel1.id == nSampleModel2.id &&
                 nSampleModel1.sampleModel.ToString() == nSampleModel2.sampleModel.ToString())
                return false;
            else
                return true;
            }
            else if((nSampleModel1 as object) == null && (nSampleModel2 as object) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
          
        }

    }
}
