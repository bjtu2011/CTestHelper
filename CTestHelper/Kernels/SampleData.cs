/************************************************************************
* Copyright (c) 2020 All Rights Reserved.
*命名空间：CTestHelper.Kernel
*文件名： SampleData
*创建人： XXX
*创建时间：2020/4/10 20:01:31
*描述
*=======================================================================
*修改标记
*修改时间：2020/4/10 20:01:31
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
    public class SampleData
    {
        public long instrumentId
        {
            get;
            set;
        }

        public string itemName
        {
            get;
            set;
        }

        public string itemValue
        {
            get;
            set;
        }

        public string testNo
        {
            get;
            set;
        }

        public DateTime? testTime
        {
            get;
            set;
        }

        public string sampleNo
        {
            get;
            set;
        }

        public string sampleData
        {
            get;
            set;
        }

        public DateTime sampleTime
        {
            get;
            set;
        }

        public string fileFullpath
        {
            get;
            set;
        }

        public SampleData(long instrumentId, string itemName, string itemValue, string testNo, DateTime? testTime, string sampleNo, string sampleData, DateTime sampleTime)
        {
            this.instrumentId = instrumentId;
            this.itemName = itemName;
            this.itemValue = itemValue;
            this.testNo = testNo;
            this.testTime = testTime;
            this.sampleNo = sampleNo;
            this.sampleData = sampleData;
            this.sampleTime = sampleTime;
        }

        public override string ToString()
        {
            return $"instrumentId:{instrumentId} itemName:{itemName}, testNo:{testNo}, sampleNo:{sampleNo}, sampleData:{sampleData}";
        }
    }
}
