using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TListener
{
    public class Counter : ICounter
    {
        private int times;
        private int _t = 0;
        private Action OnEnd;
        public ICounter Configure(int _times)
        {
            times = _times;
            return this;
        }

        public ICounter End(Action _handler)
        {
            OnEnd = _handler;
            return this;
        }

        public void Increase()
        {
            if(_t < times)
            {
                _t = _t + 1;
            }
            if(_t >= times)
            {
                OnEnd.Invoke();
            }
        }

        public int Count()
        {
            return _t;
        }
    }
}
