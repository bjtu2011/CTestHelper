/************************************************************************
* Copyright (c) 2020 All Rights Reserved.
*命名空间：CTestHelper.Kernel
*文件名： KeyValue
*创建人： XXX
*创建时间：2020/4/10 20:04:56
*描述
*=======================================================================
*修改标记
*修改时间：2020/4/10 20:04:56
*修改人：XXX
*描述：
************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CTestHelper.Kernels
{
   public class KeyValue
    {
        public string key
        {
            get;
            set;
        }

        public string dataType
        {
            get;
            set;
        }

        public List<string> value
        {
            get;
            set;
        }

        public KeyValue(string key, string value, string dataType)
        {
            this.key = key;
            this.dataType = dataType;
            this.value = new List<string>();
            this.value.Add(value);
        }
    }
}
