using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TListener
{
    public class Listener<TModel> : IListener<TModel>
        where TModel:class
    {
        public Listener(TModel model = null)
        {
            _build = false;
            _context = new TListener.ListenerContext<TModel>(model);
            _interval = 0;
            _times = 0;
        }

        public ListenerContext<TModel> Context
        {
            get
            {
                return _context;
            }
        }

        private volatile ListenerContext<TModel> _context;

        private volatile Func<ListenerContext<TModel>, bool> _listen_handler;
        private volatile Action<ListenerContext<TModel>> _success_handler;
        private volatile Action<ListenerContext<TModel>> _log_handler;
        private volatile Action<ListenerContext<TModel>> _timesout_handler;
        private volatile Action<ListenerContext<TModel>> _exit_handler;
        private volatile Action<ListenerContext<TModel>, Exception> _error_handler;
        private volatile bool _build;
        private int _interval;
        private int _times;
        #region Configure Methods
        public IListener<TModel> Listen(Func<ListenerContext<TModel>, bool> _handler)
        {
            _listen_handler = _handler;
            return this;
        }
        public IListener<TModel> Error(Action<ListenerContext<TModel>, Exception> _handler)
        {
            _error_handler = _handler;
            return this;
        }
        public IListener<TModel> Success(Action<ListenerContext<TModel>> _handler)
        {
            _success_handler = _handler;
            return this;
        }
        public IListener<TModel> Log(Action<ListenerContext<TModel>> _handler)
        {
            _log_handler = _handler;
            return this;
        }
        public IListener<TModel> Exit(Action<ListenerContext<TModel>> _handler)
        {
            _exit_handler = _handler;
            return this;
        }
        public IListener<TModel> Times(int times, Action<ListenerContext<TModel>> _on_timesout = null)
        {
            _times = times;
            _timesout_handler = _on_timesout;
            return this;
        }
        public IListener<TModel> Interval(int millseconds)
        {
            _interval = millseconds;
            return this;
        }
       
        public IListener<TModel> Build()
        {
            if (!_build)
            {
                _assert(_listen_handler == null, _build_default_listen_handler);                
                _assert(_error_handler == null, _build_default_error_handler);
                _assert(_success_handler == null, () => { return; });
                _assert(_log_handler == null, () => { return; });
                _assert(_exit_handler == null, () => { return; });

                _build_counter();
                _build_work_thread();
            }
            return this;
        }
        #endregion

        #region Private Methods
        private void _build_work_thread()
        {
            _context.WorkThread = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                object lockObj = new object();

                while (_context.IsRunning)
                {
                    lock (lockObj)
                    {
                        _context.TempData = new Hashtable();
                        try
                        {
                            var flag = _invoke_listen_handler_wait();
                            _assert(flag, _invoke_success_handler_wait);

                            _assert((flag && _times > 0), _invoke_increase_handler_wait);

                            _assert(_times == _context.Counter.Count(), _invoke_timesout_handler_wait);
                        }
                        catch (Exception ex)
                        {
                            _assert(_error_handler != null, () => _invoke_error_handler_wait(ex));
                        }

                        _invoke_log_handler_wait();
                    }
                    _assert(_interval > 0, () => Thread.Sleep(_interval));
                }

                _assert(_exit_handler != null, _invoke_exit_handler_wait);
            }));
            _build = true;
        }
        private void _build_default_listen_handler()
        {
            _listen_handler = x => { return true; };
        }
        private void _build_default_error_handler()
        {
            _error_handler = (x, ex) => { throw ex; };
        }
        private void _build_counter()
        {
            _context.Counter = _context.Counter
                               .Configure(_times)
                               .End(() =>
                               {
                                  _context.RequestStop();
                                });
        }
        private bool _invoke_listen_handler_wait()
        {
            var t_flag = Task.Run<bool>(() =>
            {
                var _flag = this._listen_handler.Invoke(_context);
                return _flag;
            });
            t_flag.Wait();
            var flag = t_flag.Result;
            return flag;
        }
        private void _invoke_success_handler_wait()
        {
            var t_success = Task.Run(() =>
            {
                this._success_handler.Invoke(_context);
            });
            t_success.Wait();
        }
        private void _invoke_increase_handler_wait()
        {
            var t_increase = Task.Run(() =>
            {
                _context.Counter.Increase();
            });
            t_increase.Wait();
        }
        private void _invoke_error_handler_wait(Exception ex)
        {
            var t_error = Task.Run(() =>
            {
                _error_handler.Invoke(_context, ex);
            });
            t_error.Wait();
        }
        private void _invoke_log_handler_wait()
        {
            var t_log = Task.Run(() =>
            {
                _log_handler.Invoke(_context);
            });
            t_log.Wait();
        }

        private void _invoke_exit_handler_wait()
        {
            var t_exit = Task.Run(() =>
            {
                _exit_handler.Invoke(_context);
            });
            t_exit.Wait();
        }

        private void _invoke_timesout_handler_wait()
        {
            var t_timesout = Task.Run(() =>
            {
                _timesout_handler.Invoke(_context);
            });
            t_timesout.Wait();
        }
        private void _assert(bool flag,Action _handler)
        {
            if (flag) _handler.Invoke();
        }
        #endregion

        #region Control Methods
        public void Start()
        {
            if (this._build == false) throw new Exception("必须先调用Build()构建监听器。");
            _context.IsRunning = true;
            _context.WorkThread.Start();
        }
        public void Stop()
        {
            if (this._build == false) throw new Exception("必须先调用Build()构建监听器。");
            this._build = false;
            _context.IsRunning = false;
            _context.WorkThread.Join();
        }

        #endregion



    }
}
