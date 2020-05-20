using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace CTestHelper.Kernels
{
    /// <summary>
    /// 获取消息，并开启两个线程进行处理：解析线程，发送线程
    /// </summary>
    public class Kernel : IDisposable
    {
        private ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Queue<Message4Kernel> _queue_decode;//解析队列
        private Queue<NSampleModel> _queue_send;//发送队列
        private object _sync_decode;//解析锁
        private object _sync_send;//发送锁
        private string _serverUrl;
        private ManualResetEvent _exited_decode;//解析信号灯，要等解析线程先结束，才能直接close完成
        private ManualResetEvent _exited_send;//发送信号灯,要等发送线程先结束，才能直接close完成
        private volatile bool _enabled;//是否执行完队列就结束线程
        private Message4Kernel cureentMessage;

        public int Count_decode
        {
            get
            {
                lock (_sync_decode)
                {
                    return _queue_decode.Count;
                }
            }
        }

        public int Count_send
        {
            get
            {
                lock (_sync_send)
                {
                    return _queue_send.Count;
                }
            }
        }

        public Kernel(String serverUrl)
        {
            _serverUrl = serverUrl;
            _enabled = true;
            _exited_decode = new ManualResetEvent(initialState: false);
            _queue_decode = new Queue<Message4Kernel>();
            _sync_decode = ((ICollection)_queue_decode).SyncRoot;
            //解析线程
            ThreadPool.QueueUserWorkItem(delegate
            {
                while (_enabled || Count_decode > 0)
                {
                    Message4Kernel decodeMessage = dequeue_decode();
                    cureentMessage = decodeMessage;
                    if (decodeMessage != null)
                    {
                        log.Info("开始处理文件: " + decodeMessage.fileName);
                        try
                        {
                            DecodeFactory.Instance.Decode(decodeMessage);
                        }
                        catch (Exception exception)
                        {
                            log.Error("Decode ", exception);
                        }
                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                }

                _exited_decode.Set();
            });
            _exited_send = new ManualResetEvent(initialState: false);
            _queue_send = new Queue<NSampleModel>();
            _sync_send = ((ICollection)_queue_send).SyncRoot;
            //发送线程
            ThreadPool.QueueUserWorkItem(delegate
            {
                while (_enabled || Count_send > 0)
                {
                    NSampleModel sendMessage = dequeue_send();
                    if (sendMessage != null && sendMessage.sampleModel != null)
                    {
                        log.Info("开始发送样本号: " + sendMessage.sampleModel.sampleNo);
                        try
                        {
                            DecodeFactory.Instance.SendToServer(sendMessage);
                        }
                        catch (Exception exception)
                        {
                            log.Error("Send ", exception);
                        }
                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                }
                _exited_send.Set();
            });
        }

        //从解析队列中取出
        private Message4Kernel dequeue_decode()
        {
            lock (_sync_decode)
            {
                return (_queue_decode.Count > 0) ? _queue_decode.Dequeue() : null;
            }
        }

        //从发送队列中取出
        private NSampleModel dequeue_send()
        {
            lock (_sync_send)
            {
                return (_queue_send.Count > 0) ? _queue_send.Dequeue() : null;
            }
        }

        //通知核心将解析消息加入解析队列
        public void Notify_decode(Message4Kernel msg)
        {
            lock (_sync_decode)
            {
                if (_enabled)
                {
                    _queue_decode.Enqueue(msg);
                    log.Debug("解析消息添加到队列：" + msg.filePath);
                    /**
                    //如果当前队列和正在处理的消息中不包含msg，则添加msg
                    if (cureentMessage != null)
                    {
                        if (cureentMessage.fileName != msg.fileName && !isInQueue(msg))
                        {
                            _queue_decode.Enqueue(msg);
                            log.Debug("解析消息添加到队列：" + msg.filePath);
                        }
                    }
                    else
                    {
                        if(!isInQueue(msg))
                        { 
                        _queue_decode.Enqueue(msg);
                        log.Debug("解析消息添加到队列：" + msg.filePath);
                        }
                    }**/
                }
            }
        }

        private Boolean isInQueue(Message4Kernel msg)
        {
            foreach (Message4Kernel message4Kernel in _queue_decode)
            {
                if (msg.fileName == message4Kernel.fileName)
                {
                    return true;
                }
            }
            return false;
        }

        //通知核心将解析消息加入解析队列
        public void Notify_send(NSampleModel msg)
        {
            lock (_sync_send)
            {
                if (_enabled)
                {
                    _queue_send.Enqueue(msg);
                    log.Debug("发送消息添加到队列：" + msg.sampleModel.sampleNo);
                }
            }
        }

        public void Close()
        {
            if (_enabled)
            {
                _enabled = false;
                _exited_decode.WaitOne();
                _exited_send.WaitOne();
                _exited_decode.Close();
                _exited_send.Close();
                log.Debug("核心线程关闭完成。");
            }
        }

        void IDisposable.Dispose()
        {
            Close();
        }
    }
}