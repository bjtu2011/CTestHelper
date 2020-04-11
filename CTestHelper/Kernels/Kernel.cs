using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace CTestHelper.Kernels
{
    /// <summary>
    /// 获取消息，并开启线程进行处理
    /// </summary>
    public class Kernel : IDisposable
    {
        private ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Queue<Message4Kernel> _queue;
        private object _sync;
        private string _serverUrl;
        private ManualResetEvent _exited;
        private volatile bool _enabled;
        //定义事件
     
        public int Count
        {
            get
            {
                lock (_sync)
                {
                    return _queue.Count;
                }
            }
        }
   
        public Kernel( String serverUrl)
        {
            _serverUrl = serverUrl;
            _enabled = true;
            _exited = new ManualResetEvent(initialState: false);
            _queue = new Queue<Message4Kernel>();
            _sync = ((ICollection)_queue).SyncRoot;
            ThreadPool.QueueUserWorkItem(delegate
            {
                while (_enabled || Count > 0)
                {
                    Message4Kernel decodeMessage = dequeue();
                    if (decodeMessage != null)
                    {
                        log.Info("Notifier: " + decodeMessage.ToString());
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
                _exited.Set();
            });
        }
        private Message4Kernel dequeue()
        {
            lock (_sync)
            {
                return (_queue.Count > 0) ? _queue.Dequeue() : null;
            }
        }
        public void Notify(Message4Kernel msg)
        {
            lock (_sync)
            {
                if (_enabled)
                {
                    _queue.Enqueue(msg);
                }
            }
        }
        public void Close()
        {
            if (_enabled)
            {
                _enabled = false;
                _exited.WaitOne();
                _exited.Close();
            }
        }
        void IDisposable.Dispose()
        {
            Close();
        }

    }
}
