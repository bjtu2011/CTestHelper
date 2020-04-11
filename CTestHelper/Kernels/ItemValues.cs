/************************************************************************
* Copyright (c) 2020 All Rights Reserved.
*命名空间：CTestHelper.Kernel
*文件名： ItemValues
*创建人： XXX
*创建时间：2020/4/10 20:04:19
*描述
*=======================================================================
*修改标记
*修改时间：2020/4/10 20:04:19
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
    public class ItemValues
    {
        private Dictionary<string, KeyValue> _valuesDict;

        public string item
        {
            get;
            set;
        }

        public List<KeyValue> values
        {
            get;
            set;
        }

        public string parallelFlag
        {
            get;
            set;
        }

        public ItemValues(string item, string parallelFlag)
        {
            _valuesDict = new Dictionary<string, KeyValue>();
            values = new List<KeyValue>();
            this.item = item;
            this.parallelFlag = parallelFlag;
        }

        public void Add(string key, string value, string dataType)
        {
            KeyValue keyValue = null;
            if (_valuesDict.ContainsKey(key))
            {
                keyValue = _valuesDict[key];
                keyValue.value.Add(value);
            }
            else
            {
                keyValue = new KeyValue(key, value, dataType);
                values.Add(keyValue);
                _valuesDict.Add(key, keyValue);
            }
        }
    }
}
