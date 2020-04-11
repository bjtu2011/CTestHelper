/************************************************************************
* Copyright (c) 2020 All Rights Reserved.
*命名空间：CTestHelper.Kernel
*文件名： Item
*创建人： XXX
*创建时间：2020/4/10 20:05:54
*描述
*=======================================================================
*修改标记
*修改时间：2020/4/10 20:05:54
*修改人：XXX
*描述：
************************************************************************/
using System.Collections;
using System.Collections.Generic;

namespace CTestHelper.Kernels
{
    class Item
    {
        public object this[string key]
        {
            get
            {
                return this[key];
            }
            set
            {
                this[key] = value;
            }
        }

        public ICollection<string> Keys => Keys;

        public ICollection<object> Values => Values;

        public bool IsReadOnly => false;

        public void Add(string key, object value)
        {
            Add(key, value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            Add(item);
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex);
        }

        public bool Remove(string key)
        {
            return Remove(key);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return Remove(item);
        }

        public bool TryGetValue(string key, out object value)
        {
            return TryGetValue(key, out value);
        }


        //IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        //{
        //    return (IEnumerator<KeyValuePair<string, object>>)GetEnumerator();
        //}
    }
}
