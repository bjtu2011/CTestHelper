/************************************************************************
* Copyright (c) 2020 All Rights Reserved.
*命名空间：CTestHelper.Kernel
*文件名： Sample
*创建人： XXX
*创建时间：2020/4/10 20:03:27
*描述
*=======================================================================
*修改标记
*修改时间：2020/4/10 20:03:27
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
   public class Sample
    {
        private Dictionary<string, ItemValues> _itemDict;

        public string sampleNo
        {
            get;
            set;
        }

        public List<ItemValues> items
        {
            get;
            set;
        }

        public Sample(string sampleNo)
        {
            _itemDict = new Dictionary<string, ItemValues>();
            items = new List<ItemValues>();
            this.sampleNo = sampleNo;
        }

        public void Add(string name, string key, string value, string itemParallelFlag, string dataType)
        {
            ItemValues itemValues = null;
            if (_itemDict.ContainsKey(name))
            {
                itemValues = _itemDict[name];
            }
            else
            {
                itemValues = new ItemValues(name, itemParallelFlag);
                items.Add(itemValues);
                _itemDict.Add(name, itemValues);
            }
            itemValues.Add(key, value, dataType);
        }

        public string ToItemString()
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            Dictionary<string, string> dictionary = null;
            for (int i = 0; i < items.Count; i++)
            {
                ItemValues itemValues = items[i];
                string parallelFlag = itemValues.parallelFlag;
                for (int j = 0; j < itemValues.values.Count; j++)
                {
                    for (int k = 0; k < itemValues.values[j].value.Count; k++)
                    {
                        string text = "";
                        text = ((!string.IsNullOrEmpty(parallelFlag)) ? (itemValues.item + "." + parallelFlag + "[" + (k + 1).ToString() + "]." + itemValues.values[j].key) : (itemValues.item + "." + itemValues.values[j].key));
                        if (list.Count > k)
                        {
                            dictionary = list[k];
                        }
                        else
                        {
                            dictionary = new Dictionary<string, string>();
                            list.Add(dictionary);
                        }
                        dictionary.Add(text, itemValues.values[j].value[k]);
                    }
                }
            }
            return Utils.ObjToJson(list);
        }

        public string ToItemString1()
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            for (int i = 0; i < items.Count; i++)
            {
                ItemValues itemValues = items[i];
                string parallelFlag = itemValues.parallelFlag;
                for (int j = 0; j < itemValues.values.Count; j++)
                {
                    for (int k = 0; k < itemValues.values[j].value.Count; k++)
                    {
                        string text = "";
                        text = ((!string.IsNullOrEmpty(parallelFlag)) ? (itemValues.item + "." + parallelFlag + "[" + (k + 1).ToString() + "]." + itemValues.values[j].key) : (itemValues.item + "." + itemValues.values[j].key));
                        Dictionary<string, string> dictionary = new Dictionary<string, string>();
                        dictionary.Add(text, itemValues.values[j].value[k]);
                        list.Add(dictionary);
                    }
                }
            }
            return Utils.ObjToJson(list);
        }
    }
}
