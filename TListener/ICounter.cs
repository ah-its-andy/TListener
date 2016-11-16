using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TListener
{
    public interface ICounter
    {
        ICounter Configure(int _times);
        ICounter End(Action _handler);
        void Increase();

        int Count();
    }
}
