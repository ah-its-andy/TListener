using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TListener.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var lastDate = DateTime.Now;
            var listener = new TListener.Listener<string>()
                           .Listen(x =>
                           {
                               return true;
                           })
                           .Success(x =>
                           {
                               x.SyncContext.Post(s =>
                               {
                                   Console.WriteLine(DateTime.Now.ToString());
                               }, null);
                           })
                           .Log(x => Console.WriteLine("Log - Times : " + x.Counter.Count().ToString()))
                           .Exit(x => Console.WriteLine("Exit"))
                           .Interval(5000)
                           .Times(5, x => Console.WriteLine("Times Out"))
                           .Build();
            listener.Start();

            Console.ReadKey();
        }
    }
}
