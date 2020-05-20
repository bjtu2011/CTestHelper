/************************************************************************
* Copyright (c) 2020 All Rights Reserved.
*命名空间：CTestHelper.Kernels
*文件名： TaskEndHandler
*创建人： XXX
*创建时间：2020/4/11 16:51:24
*描述
*=======================================================================
*修改标记
*修改时间：2020/4/11 16:51:24
*修改人：XXX
*描述：
************************************************************************/
using System;
using System.Collections.Generic;

namespace CTestHelper.Kernels
{
    public delegate void TaskEndHanlder(object sender, NSampleModel msg,String response);//发送任务结束之后通知界面更新、记录到数据库等操作
}
