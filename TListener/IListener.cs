using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TListener
{
    public interface IListener<TModel>
        where TModel:class
    {
        ListenerContext<TModel> Context { get;}

        /// <summary>
        /// 设置监听操作，当函数返回true则执行Success，如果抛出异常则执行Error
        /// </summary>
        /// <param name="_handler">包含监听操作的函数体</param>
        /// <returns>监听器实例</returns>
        IListener<TModel> Listen(Func<ListenerContext<TModel>, bool> _handler);
        /// <summary>
        /// 当监听函数返回true则执行此函数
        /// </summary>
        /// <param name="_handler">无返回值函数体</param>
        /// <returns></returns>
        IListener<TModel> Success(Action<ListenerContext<TModel>> _handler);
        /// <summary>
        /// 当监听函数抛出异常则执行此函数
        /// </summary>
        /// <param name="_handler">异常处理函数体</param>
        /// <returns></returns>
        IListener<TModel> Error(Action<ListenerContext<TModel>, Exception> _handler);

        IListener<TModel> Log(Action<ListenerContext<TModel>> _handler);


        /// <summary>
        /// 设置监听函数执行的间隔
        /// </summary>
        /// <param name="millseconds">间隔时间，单位是毫秒</param>
        /// <returns></returns>
        IListener<TModel> Interval(int millseconds);

        IListener<TModel> Times(int times, Action<ListenerContext<TModel>> _on_timesout = null);

        IListener<TModel> Exit(Action<ListenerContext<TModel>> _handler);
        /// <summary>
        /// 构建监听器实例
        /// </summary>
        /// <returns>监听器实例</returns>
        IListener<TModel> Build();
        void Start();
        void Stop();
    }
}
