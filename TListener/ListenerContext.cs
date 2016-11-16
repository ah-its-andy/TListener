using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TListener
{
    public class ListenerContext
    {
        public ListenerContext()
        {
            SyncContext = new SynchronizationContext();
            IsRunning = false;
            Counter = new TListener.Counter();
        }

        public volatile SynchronizationContext SyncContext;

        public volatile bool IsRunning;

        public volatile Hashtable TempData;

        public volatile Thread WorkThread;

        public volatile ICounter Counter;

        public void RequestStop()
        {
            IsRunning = false;
        }
    }
    public class ListenerContext<TModel>: ListenerContext
        where TModel:class
    {
        public ListenerContext(TModel model)
            :base()
        {
            Model = model;
        }

        public volatile TModel Model;
    }
}
